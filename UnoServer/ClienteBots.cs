using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;
using UnoGame.Red;

namespace UnoServer
{
    // Jugadores automáticos que se conectan al servidor por TCP real, igual que lo hará UnoGame.
    // Sirven para probar el servidor sin necesidad de interfaz gráfica.
    //
    // Uso:  UnoServer.exe bots [ip] [puerto] [abandonarTrasEstados] [cuantos] [prefijo]
    //   ip, puerto             a dónde conectarse (por defecto 127.0.0.1 y 11111)
    //   abandonarTrasEstados   si es mayor que 0, el bot 3 se desconecta a propósito después de recibir
    //                          esa cantidad de estados (prueba de desconexión a media partida)
    //   cuantos                cuántos bots lanzar (por defecto 4; con menos se quedan en la sala de espera)
    //   prefijo                prefijo de los nombres (por defecto "Bot"); cámbialo para no repetir nombres
    //
    // Ejemplos:
    //   UnoServer.exe bots                              partida completa con 4 bots
    //   UnoServer.exe bots 127.0.0.1 11111 0 2          2 bots esperando en la sala
    //   UnoServer.exe bots 127.0.0.1 11111 25           el Bot3 se va a media partida
    public static class ClienteBots
    {
        const int PausaMs = 120;   // pausa antes de cada acción para poder seguir lo que pasa

        public static void Ejecutar(string[] args)
        {
            string ip = args.Length > 1 ? args[1] : "127.0.0.1";
            int puerto = Protocolo.PuertoPorDefecto;
            int abandonar = 0;
            int cuantos = 4;
            string prefijo = "Bot";
            int valor;
            if (args.Length > 2 && int.TryParse(args[2], out valor)) puerto = valor;
            if (args.Length > 3 && int.TryParse(args[3], out valor)) abandonar = valor;
            if (args.Length > 4 && int.TryParse(args[4], out valor)) cuantos = valor;
            if (args.Length > 5) prefijo = args[5];

            List<Thread> hilos = new List<Thread>();
            for (int i = 0; i < cuantos; i++)
            {
                string nombre = prefijo + (i + 1);
                bool imprimirTodo = i == 0;                  // solo el primero cuenta lo que pasa, para no repetirlo 4 veces
                int abandonaTras = (abandonar > 0 && i == 2) ? abandonar : 0;

                Thread hilo = new Thread(() => CorrerBot(nombre, ip, puerto, abandonaTras, imprimirTodo));
                hilo.IsBackground = true;
                hilos.Add(hilo);
                hilo.Start();
                Thread.Sleep(300);   // así los asientos se asignan en orden
            }

            foreach (Thread hilo in hilos)
                hilo.Join();
            Console.WriteLine("Todos los bots terminaron.");
        }

        static void CorrerBot(string nombre, string ip, int puerto, int abandonaTras, bool imprimirTodo)
        {
            Random azar = new Random(nombre.GetHashCode());
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.Connect(ip, puerto);
            }
            catch (Exception ex)
            {
                Log(nombre, "No se pudo conectar: " + ex.Message);
                return;
            }

            ConexionJson conexion = new ConexionJson(socket);
            EstadoJuego ultimo = null;
            int estadosRecibidos = 0;
            int erroresSeguidos = 0;

            try
            {
                conexion.Enviar(new Mensaje { Tipo = Tipos.Unirse, Nombre = nombre });

                while (true)
                {
                    Mensaje m = conexion.Recibir();
                    if (m == null)
                    {
                        Log(nombre, "El servidor cerró la conexión.");
                        break;
                    }

                    switch (m.Tipo)
                    {
                        case Tipos.Bienvenida:
                            Log(nombre, "Entró a la sala con el asiento " + m.Indice + ".");
                            break;

                        case Tipos.Rechazo:
                            Log(nombre, "RECHAZADO: " + m.Texto);
                            return;

                        case Tipos.Sala:
                            if (imprimirTodo)
                                Log(nombre, "Sala " + m.Texto + ": " + string.Join(", ", m.Nombres));
                            break;

                        case Tipos.Evento:
                            if (imprimirTodo)
                                Log(nombre, m.Texto);
                            break;

                        case Tipos.Error:
                            Log(nombre, "ERROR del servidor: " + m.Texto);
                            erroresSeguidos++;
                            if (erroresSeguidos > 3)
                            {
                                Log(nombre, "Demasiados errores seguidos; el bot se detiene.");
                                return;
                            }
                            // Si su jugada fue rechazada, se queda sin acción: roba para no trabar la partida
                            if (ultimo != null)
                                Actuar(conexion, ultimo, azar, true);
                            break;

                        case Tipos.Estado:
                            ultimo = m.Estado;
                            estadosRecibidos++;
                            erroresSeguidos = 0;

                            if (imprimirTodo && estadosRecibidos == 1)
                            {
                                JsonSerializerOptions bonito = new JsonSerializerOptions(Protocolo.Opciones);
                                bonito.WriteIndented = true;
                                Log(nombre, "Primer estado recibido (esto es todo lo que ve un jugador):\n"
                                    + JsonSerializer.Serialize(ultimo, bonito));
                            }

                            if (abandonaTras > 0 && estadosRecibidos >= abandonaTras)
                            {
                                Log(nombre, "Se desconecta a propósito.");
                                return;
                            }
                            Actuar(conexion, ultimo, azar, false);
                            break;

                        case Tipos.Fin:
                            Log(nombre, "FIN de la partida. Ganador: " + m.Texto);
                            return;

                        case Tipos.Abortada:
                            Log(nombre, "PARTIDA CANCELADA: " + m.Texto);
                            return;
                    }
                }
            }
            catch (Exception ex)
            {
                Log(nombre, "Conexión terminada: " + ex.Message);
            }
            finally
            {
                conexion.Cerrar();
            }
        }

        // Una sola acción por cada estado que llega en su turno
        static void Actuar(ConexionJson conexion, EstadoJuego e, Random azar, bool forzarRobar)
        {
            if (e.Terminada || e.Turno != e.TuIndice) return;
            Thread.Sleep(PausaMs);

            // Comodín robado: solo falta elegir el color
            if (e.CartaPendiente != null)
            {
                conexion.Enviar(new Mensaje { Tipo = Tipos.ElegirColor, Color = ColorMasComun(e.TuMano, azar) });
                return;
            }

            List<CartaDto> jugables = e.TuMano.Where(c => EsJugable(c, e)).ToList();
            if (forzarRobar || jugables.Count == 0)
            {
                conexion.Enviar(new Mensaje { Tipo = Tipos.Robar });
                return;
            }

            // 3 de cada 4 veces dice UNO; la otra "se le olvida" para probar la penalización
            if (e.PuedesDecirUno && azar.Next(4) != 0)
            {
                conexion.Enviar(new Mensaje { Tipo = Tipos.DecirUno });
                return;   // el servidor manda un estado nuevo y ahí juega
            }

            CartaDto elegida = jugables[azar.Next(jugables.Count)];
            bool comodin = elegida.Tipo == "Comodin" || elegida.Tipo == "Mas4";
            conexion.Enviar(new Mensaje
            {
                Tipo = Tipos.Jugar,
                Carta = elegida,
                Color = comodin ? ColorMasComun(e.TuMano, azar) : "Ninguno"
            });
        }

        // Copia de la regla para que el bot sepa qué intentar. Quien decide siempre es el servidor:
        // si esta copia y el servidor no coinciden, el servidor responde "error" y se nota en la consola.
        static bool EsJugable(CartaDto carta, EstadoJuego e)
        {
            if (carta.Tipo == "Comodin" || carta.Tipo == "Mas4") return true;
            if (carta.Color == e.ColorActivo) return true;
            CartaDto tope = e.CartaSuperior;
            if (carta.Tipo == "Numero" && tope.Tipo == "Numero" && carta.Numero == tope.Numero) return true;
            if (carta.Tipo != "Numero" && carta.Tipo == tope.Tipo) return true;
            return false;
        }

        static string ColorMasComun(List<CartaDto> mano, Random azar)
        {
            string[] colores = { "Rojo", "Azul", "Verde", "Amarillo" };
            string mejor = colores[azar.Next(colores.Length)];
            int maximo = 0;
            foreach (string color in colores)
            {
                int cantidad = mano.Count(c => c.Color == color);
                if (cantidad > maximo)
                {
                    maximo = cantidad;
                    mejor = color;
                }
            }
            return mejor;
        }

        static void Log(string nombre, string texto)
        {
            Console.WriteLine("[" + nombre + "] " + texto);
        }
    }
}
