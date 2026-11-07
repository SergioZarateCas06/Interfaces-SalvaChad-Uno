using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

// Todo lo que el cliente (UnoGame) y el servidor (UnoServer) necesitan para entenderse:
// los tipos de mensaje, los datos que viajan y la conexión que los envía línea por línea.
// Este archivo vive en el proyecto UnoGame y UnoServer lo agrega "como vínculo".
namespace UnoGame.Red
{
    // Valores del campo "tipo" de cada mensaje
    public static class Tipos
    {
        // Cliente -> Servidor
        public const string Unirse = "unirse";            // "quiero entrar a la partida con este nombre"
        public const string Jugar = "jugar";              // "juego esta carta" (con color si es comodín)
        public const string Robar = "robar";              // "robo una carta de la burra"
        public const string ElegirColor = "elegirColor";  // color del comodín que robé
        public const string DecirUno = "decirUno";        // "UNO"

        // Servidor -> Cliente
        public const string Bienvenida = "bienvenida";    // te aceptaron, este es tu asiento
        public const string Rechazo = "rechazo";          // no te aceptaron (sala llena, nombre repetido...)
        public const string Sala = "sala";                // quiénes están esperando
        public const string Estado = "estado";            // cómo está la mesa (personalizado para cada jugador)
        public const string Evento = "evento";            // texto de lo que acaba de pasar
        public const string Error = "error";              // tu acción no fue válida
        public const string Fin = "fin";                  // la partida terminó
        public const string Abortada = "abortada";        // alguien se fue y se canceló
    }

    // Una carta tal como viaja por la red (con texto, para que el JSON sea legible)
    public class CartaDto
    {
        public int Numero { get; set; }
        public string Color { get; set; }   // "Rojo", "Azul", "Verde", "Amarillo" o "Ninguno"
        public string Tipo { get; set; }    // "Numero", "Salta", "Reversa", "Mas2", "Comodin" o "Mas4"

        public static CartaDto De(Card carta)
        {
            return new CartaDto
            {
                Numero = carta.Number,
                Color = carta.Color.ToString(),
                Tipo = carta.Type.ToString()
            };
        }

        // Convierte de vuelta a Card; devuelve false si el texto no corresponde a una carta real
        public bool TryACarta(out Card carta)
        {
            carta = null;
            CardColor color;
            CardType tipo;
            if (!Enum.TryParse(Color, out color)) return false;
            if (!Enum.TryParse(Tipo, out tipo)) return false;
            carta = new Card(Numero, color, tipo);
            return true;
        }
    }

    // Foto de la mesa que el servidor le manda a UN jugador: su mano completa,
    // y de los demás solo cuántas cartas tienen
    public class EstadoJuego
    {
        public List<string> Nombres { get; set; }
        public int TuIndice { get; set; }
        public int Turno { get; set; }
        public int Sentido { get; set; }                 // 1 o -1
        public string ColorActivo { get; set; }
        public CartaDto CartaSuperior { get; set; }
        public int CartasBurra { get; set; }
        public int CartasDescarte { get; set; }
        public List<int> CartasPorJugador { get; set; }
        public List<CartaDto> TuMano { get; set; }
        public CartaDto CartaPendiente { get; set; }     // comodín robado que debes elegir color (o null)
        public bool PuedesDecirUno { get; set; }
        public bool Terminada { get; set; }
    }

    // Un solo tipo de mensaje con campos opcionales: según "tipo" se usan unos u otros
    public class Mensaje
    {
        public string Tipo { get; set; }
        public string Nombre { get; set; }         // unirse
        public int? Indice { get; set; }           // bienvenida, fin
        public CartaDto Carta { get; set; }        // jugar
        public string Color { get; set; }          // jugar, elegirColor
        public string Texto { get; set; }          // evento, error, rechazo, fin, abortada, sala
        public List<string> Nombres { get; set; }  // sala
        public EstadoJuego Estado { get; set; }    // estado
    }

    public static class Protocolo
    {
        public const int PuertoPorDefecto = 11111;

        // camelCase en el JSON ("cartaSuperior"), y los campos vacíos no se envían
        public static readonly JsonSerializerOptions Opciones = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    // Envuelve un Socket TCP para mandar y recibir un mensaje JSON por línea.
    // TCP es un "chorro" de bytes sin separaciones, así que usamos el salto de línea como
    // delimitador (el JSON de System.Text.Json nunca contiene saltos de línea reales).
    public class ConexionJson
    {
        readonly Socket socket;
        readonly StreamReader lector;
        readonly StreamWriter escritor;
        readonly object candadoEnvio = new object();

        public ConexionJson(Socket socket)
        {
            this.socket = socket;
            NetworkStream flujo = new NetworkStream(socket, false);
            UTF8Encoding utf8 = new UTF8Encoding(false);
            lector = new StreamReader(flujo, utf8);
            escritor = new StreamWriter(flujo, utf8);
            escritor.NewLine = "\n";
            escritor.AutoFlush = true;
        }

        // Puede llamarse desde varios hilos a la vez: el candado evita que se mezclen dos mensajes
        public void Enviar(Mensaje mensaje)
        {
            string json = JsonSerializer.Serialize(mensaje, Protocolo.Opciones);
            lock (candadoEnvio)
            {
                escritor.WriteLine(json);
            }
        }

        // Espera (bloqueando el hilo) hasta que llegue una línea completa.
        // Devuelve null si el otro lado cerró la conexión.
        public Mensaje Recibir()
        {
            string linea = lector.ReadLine();
            if (linea == null) return null;
            return JsonSerializer.Deserialize<Mensaje>(linea, Protocolo.Opciones);
        }

        public void Cerrar()
        {
            try { socket.Shutdown(SocketShutdown.Both); } catch { }
            try { socket.Close(); } catch { }
        }
    }
}
