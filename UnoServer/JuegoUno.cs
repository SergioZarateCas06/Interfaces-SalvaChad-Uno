using System;
using System.Collections.Generic;
using System.Linq;
using UnoGame;
using UnoGame.Red;

namespace UnoServer
{
    // Una jugada o robo que después se guarda en la base de datos (a través de la API)
    public class MovimientoRegistro
    {
        public int NumeroTurno;
        public string Jugador;
        public string Accion;   // "Jugar" o "Robar"
        public string Carta;
    }

    // Las reglas del UNO. No sabe nada de sockets ni de pantallas: recibe acciones, las valida,
    // cambia el estado de la partida y anota en "Eventos" qué pasó para que ServidorUno se lo cuente a todos.
    // Cada acción devuelve null si salió bien, o el texto del error si no era válida.
    public class JuegoUno
    {
        public const int NumJugadores = 4;

        readonly Random azar = new Random();
        readonly Stack<Card> burra = new Stack<Card>();
        readonly Stack<Card> descarte = new Stack<Card>();
        readonly bool[] dijoUno = new bool[NumJugadores];
        Card pendienteColor;      // comodín robado que está esperando que su dueño elija color
        int numeroMovimiento;

        public readonly List<Card>[] Manos = new List<Card>[NumJugadores];
        public string[] Nombres = new string[NumJugadores];
        public int Turno { get; private set; }
        public int Sentido { get; private set; }
        public CardColor ColorActivo { get; private set; }
        public bool Terminada { get; private set; }
        public int Ganador { get; private set; }
        public bool EsperaColor { get { return pendienteColor != null; } }

        // Lo ocurrido en la última acción; ServidorUno lo difunde y lo vacía
        public readonly List<string> Eventos = new List<string>();
        public readonly List<MovimientoRegistro> Movimientos = new List<MovimientoRegistro>();

        // ---------------- Inicio de la partida ----------------

        public void Iniciar(string[] nombres)
        {
            Nombres = nombres;
            Terminada = false;
            Ganador = -1;
            Turno = 0;
            Sentido = 1;
            pendienteColor = null;
            numeroMovimiento = 0;
            Eventos.Clear();
            Movimientos.Clear();
            burra.Clear();
            descarte.Clear();

            for (int i = 0; i < NumJugadores; i++)
            {
                Manos[i] = new List<Card>();
                dijoUno[i] = false;
            }

            CrearYBarajar();

            // 7 cartas a cada jugador, repartidas de una en una
            for (int ronda = 0; ronda < 7; ronda++)
                for (int j = 0; j < NumJugadores; j++)
                    Manos[j].Add(burra.Pop());

            // Primera carta de la mesa: si sale Comodín +4 se devuelve al mazo y se saca otra
            Card primera = burra.Pop();
            while (primera.Type == CardType.Mas4)
            {
                DevolverYBarajar(primera);
                primera = burra.Pop();
            }
            descarte.Push(primera);

            // Si la primera es un Comodín normal no tiene color: se sortea uno
            ColorActivo = primera.Color != CardColor.Ninguno ? primera.Color : (CardColor)azar.Next(4);

            Eventos.Add("Comienza la partida. Carta inicial: " + primera + ".");
        }

        void CrearYBarajar()
        {
            List<Card> mazo = new List<Card>();
            CardColor[] colores = { CardColor.Rojo, CardColor.Azul, CardColor.Verde, CardColor.Amarillo };
            foreach (CardColor color in colores)
            {
                mazo.Add(new Card(0, color, CardType.Numero));   // un solo 0 por color
                for (int i = 1; i <= 9; i++)
                {
                    mazo.Add(new Card(i, color, CardType.Numero));
                    mazo.Add(new Card(i, color, CardType.Numero));
                }
                for (int i = 0; i < 2; i++)   // dos de cada carta especial
                {
                    mazo.Add(new Card(-1, color, CardType.Mas2));
                    mazo.Add(new Card(-1, color, CardType.Reversa));
                    mazo.Add(new Card(-1, color, CardType.Salta));
                }
            }
            for (int i = 0; i < 4; i++)
            {
                mazo.Add(new Card(-1, CardColor.Ninguno, CardType.Comodin));
                mazo.Add(new Card(-1, CardColor.Ninguno, CardType.Mas4));
            }

            Barajar(mazo);
            burra.Clear();
            foreach (Card carta in mazo)
                burra.Push(carta);
        }

        // Fisher-Yates: cada carta se intercambia con otra posición al azar
        void Barajar(List<Card> cartas)
        {
            int n = cartas.Count;
            while (n > 1)
            {
                n--;
                int k = azar.Next(n + 1);
                Card temporal = cartas[k];
                cartas[k] = cartas[n];
                cartas[n] = temporal;
            }
        }

        void DevolverYBarajar(Card carta)
        {
            List<Card> restante = new List<Card>(burra);
            restante.Add(carta);
            Barajar(restante);
            burra.Clear();
            foreach (Card c in restante)
                burra.Push(c);
        }

        // ---------------- Reglas ----------------

        static bool EsComodin(Card carta)
        {
            return carta.Type == CardType.Comodin || carta.Type == CardType.Mas4;
        }

        public bool JugadaValida(Card carta)
        {
            Card tope = descarte.Peek();
            if (EsComodin(carta)) return true;
            if (carta.Color == ColorActivo) return true;
            if (carta.Type == CardType.Numero && tope.Type == CardType.Numero && carta.Number == tope.Number) return true;
            if (carta.Type != CardType.Numero && carta.Type == tope.Type) return true;
            return false;
        }

        int Siguiente()
        {
            return (Turno + Sentido + NumJugadores) % NumJugadores;
        }

        void AvanzarTurno()
        {
            Turno = Siguiente();
        }

        // Roba la carta de arriba de la burra; si está vacía, la rellena con el descarte
        Card Sacar()
        {
            if (burra.Count == 0) ReponerMazo();
            if (burra.Count == 0) return null;
            return burra.Pop();
        }

        void ReponerMazo()
        {
            if (descarte.Count <= 1) return;
            Card tope = descarte.Pop();
            List<Card> resto = descarte.ToList();
            descarte.Clear();
            descarte.Push(tope);
            Barajar(resto);
            foreach (Card c in resto)
                burra.Push(c);
            Eventos.Add("Se acabó la burra. Se barajó el descarte para formar una nueva.");
        }

        // Al recibir una carta se borra el "UNO" dicho
        void Agregar(int jugador, Card carta)
        {
            if (carta == null) return;
            Manos[jugador].Add(carta);
            dijoUno[jugador] = false;
        }

        void Registrar(int jugador, string accion, Card carta)
        {
            numeroMovimiento++;
            Movimientos.Add(new MovimientoRegistro
            {
                NumeroTurno = numeroMovimiento,
                Jugador = Nombres[jugador],
                Accion = accion,
                Carta = carta.ToString()
            });
        }

        string ValidarTurno(int jugador)
        {
            if (Terminada) return "La partida ya terminó.";
            if (jugador != Turno) return "No es tu turno.";
            return null;
        }

        // ---------------- Acciones de los jugadores ----------------

        // El jugador quiere jugar una carta de su mano (si es comodín, trae el color elegido)
        public string Jugar(int jugador, Card pedida, CardColor color)
        {
            string error = ValidarTurno(jugador);
            if (error != null) return error;
            if (pendienteColor != null) return "Primero debes elegir el color del comodín que robaste.";

            // La carta se busca por valores, no por posición, para que no importe el orden de la mano
            int indice = Manos[jugador].FindIndex(c => c.Color == pedida.Color && c.Type == pedida.Type && c.Number == pedida.Number);
            if (indice < 0) return "Esa carta no está en tu mano.";

            Card carta = Manos[jugador][indice];
            if (!JugadaValida(carta)) return "Esa carta no es una jugada válida.";
            if (EsComodin(carta) && color == CardColor.Ninguno) return "Debes elegir un color para el comodín.";

            AplicarJugada(jugador, carta, color);
            return null;
        }

        // El jugador roba una carta. Si se puede jugar, se juega sola; si no, se la queda y pasa el turno
        public string Robar(int jugador)
        {
            string error = ValidarTurno(jugador);
            if (error != null) return error;
            if (pendienteColor != null) return "Primero debes elegir el color del comodín que robaste.";

            Card robada = Sacar();
            if (robada == null)
            {
                Eventos.Add("No quedan cartas para robar. " + Nombres[jugador] + " pierde el turno.");
                AvanzarTurno();
                return null;
            }

            if (JugadaValida(robada))
            {
                if (EsComodin(robada))
                {
                    // Espera a que su dueño elija color (mensaje "elegirColor")
                    pendienteColor = robada;
                    Eventos.Add(Nombres[jugador] + " robó un comodín y está eligiendo color.");
                    return null;
                }
                Eventos.Add(Nombres[jugador] + " robó " + robada + " y se jugó automáticamente.");
                Agregar(jugador, robada);
                AplicarJugada(jugador, robada, CardColor.Ninguno);
                return null;
            }

            // No es jugable: se la queda (a los demás no se les dice cuál era)
            Agregar(jugador, robada);
            Registrar(jugador, "Robar", robada);
            Eventos.Add(Nombres[jugador] + " robó una carta y no era jugable, se la queda.");
            AvanzarTurno();
            return null;
        }

        // Color del comodín que se acaba de robar
        public string ElegirColor(int jugador, CardColor color)
        {
            string error = ValidarTurno(jugador);
            if (error != null) return error;
            if (pendienteColor == null) return "No tienes ningún comodín esperando color.";
            if (color == CardColor.Ninguno) return "Elige un color válido.";

            Card carta = pendienteColor;
            pendienteColor = null;
            Agregar(jugador, carta);
            AplicarJugada(jugador, carta, color);
            return null;
        }

        // Solo cuenta con exactamente 2 cartas, antes de jugar la penúltima
        public string DecirUno(int jugador)
        {
            string error = ValidarTurno(jugador);
            if (error != null) return error;
            if (Manos[jugador].Count != 2) return "Solo puedes decir UNO cuando tienes 2 cartas.";
            dijoUno[jugador] = true;
            return null;
        }

        // Pasa la carta al descarte, revisa si ganó o si olvidó el UNO y aplica el efecto de la carta
        void AplicarJugada(int jugador, Card carta, CardColor colorElegido)
        {
            Manos[jugador].Remove(carta);
            descarte.Push(carta);
            Registrar(jugador, "Jugar", carta);

            string texto = Nombres[jugador] + " jugó " + carta;
            if (EsComodin(carta)) texto += " y eligió " + colorElegido;
            Eventos.Add(texto + ".");

            // Sin cartas: ganó
            if (Manos[jugador].Count == 0)
            {
                Terminada = true;
                Ganador = jugador;
                Eventos.Add("¡" + Nombres[jugador] + " ganó la partida!");
                return;
            }

            // Le queda una: debió decir UNO; si no, come 2
            if (Manos[jugador].Count == 1)
            {
                if (dijoUno[jugador])
                {
                    Eventos.Add(Nombres[jugador] + " dijo UNO.");
                }
                else
                {
                    Eventos.Add(Nombres[jugador] + " no dijo UNO, come 2 cartas.");
                    for (int i = 0; i < 2; i++)
                        Agregar(jugador, Sacar());
                }
            }

            ColorActivo = EsComodin(carta) ? colorElegido : carta.Color;

            if (carta.Type == CardType.Reversa)
            {
                Sentido = -Sentido;
                Eventos.Add("Se invierte el sentido del juego.");
            }

            if (carta.Type == CardType.Salta)
            {
                Eventos.Add(Nombres[Siguiente()] + " pierde turno.");
                AvanzarTurno();
            }

            if (carta.Type == CardType.Mas2 || carta.Type == CardType.Mas4)
            {
                int cantidad = carta.Type == CardType.Mas2 ? 2 : 4;
                int victima = Siguiente();
                for (int i = 0; i < cantidad; i++)
                    Agregar(victima, Sacar());
                Eventos.Add(Nombres[victima] + " come " + cantidad + " cartas y pierde su turno.");
                AvanzarTurno();   // pasa a la víctima; el AvanzarTurno de abajo la salta
            }

            AvanzarTurno();
        }

        // ---------------- Lo que ve cada jugador ----------------

        public EstadoJuego EstadoPara(int jugador)
        {
            bool esMiTurno = jugador == Turno;
            EstadoJuego estado = new EstadoJuego
            {
                Nombres = Nombres.ToList(),
                TuIndice = jugador,
                Turno = Turno,
                Sentido = Sentido,
                ColorActivo = ColorActivo.ToString(),
                CartaSuperior = descarte.Count > 0 ? CartaDto.De(descarte.Peek()) : null,
                CartasBurra = burra.Count,
                CartasDescarte = descarte.Count,
                CartasPorJugador = Manos.Select(m => m.Count).ToList(),
                TuMano = Manos[jugador].Select(c => CartaDto.De(c)).ToList(),
                CartaPendiente = (pendienteColor != null && esMiTurno) ? CartaDto.De(pendienteColor) : null,
                Terminada = Terminada
            };

            // Mismo criterio que el botón UNO de antes: 2 cartas, su turno y con alguna jugada posible
            estado.PuedesDecirUno =
                !Terminada &&
                pendienteColor == null &&
                esMiTurno &&
                Manos[jugador].Count == 2 &&
                !dijoUno[jugador] &&
                Manos[jugador].Any(c => JugadaValida(c));

            return estado;
        }
    }
}
