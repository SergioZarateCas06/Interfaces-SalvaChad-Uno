using System;
using System.Collections.Generic;
using System.Windows.Forms;
using UnoGame.Red;
using System.Drawing;

// Parte "en red" de Form1: constructor que recibe la conexión y el primer estado,
// y el dibujo de la mesa a partir del EstadoJuego que manda el servidor.
namespace UnoGame
{
    public partial class Form1
    {
        ClienteRed cliente;          // conexión con el servidor (null en el modo local de antes)
        bool modoRed;                // true: la mesa la maneja el servidor, no el reparto local
        EstadoJuego estadoActual;    // último estado recibido
        Card comodinDeMano;

        // Abre la mesa para una partida en red. Lo llama la sala de espera con el primer estado.
        // Reutiliza el constructor anterior con los nombres que vienen en el estado.
        public Form1(ClienteRed cliente, EstadoJuego estadoInicial)
            : this(Nombre(estadoInicial, 0), Nombre(estadoInicial, 1), Nombre(estadoInicial, 2), Nombre(estadoInicial, 3))
        {
            this.cliente = cliente;
            modoRed = true;

            // Primero suscribirse y después CambiarInterfaz: lo que llegó mientras tanto se entrega en orden
            cliente.MensajeRecibido += AlRecibirMensajeRed;
            cliente.Desconectado += AlDesconectarRed;
            cliente.CambiarInterfaz(this);

            this.FormClosed += (s, e) =>
            {
                cliente.MensajeRecibido -= AlRecibirMensajeRed;
                cliente.Desconectado -= AlDesconectarRed;
                cliente.Cerrar();
            };

            AplicarEstado(estadoInicial);
        }

        static string Nombre(EstadoJuego estado, int i)
        {
            if (estado == null || estado.Nombres == null || i >= estado.Nombres.Count) return "Jugador " + (i + 1);
            return estado.Nombres[i];
        }

        // Cuántas cartas tiene el jugador del asiento i (0 si el dato no existe)
        static int CartasDe(EstadoJuego estado, int i)
        {
            if (estado == null || estado.CartasPorJugador == null || i >= estado.CartasPorJugador.Count) return 0;
            return estado.CartasPorJugador[i];
        }

        void AlRecibirMensajeRed(Mensaje mensaje)
        {
            if (mensaje.Tipo == Tipos.Estado)
            {
                AplicarEstado(mensaje.Estado);
            }
            else if (mensaje.Tipo == Tipos.Evento)
            {
                AgregarEvento(mensaje.Texto);
            }
            else if (mensaje.Tipo == Tipos.Error)
            {
                AgregarEvento("Error: " + mensaje.Texto);
            }
            else if (mensaje.Tipo == Tipos.Abortada)
            {
                CerrarPorRed(string.IsNullOrWhiteSpace(mensaje.Texto) ? "La partida fue cancelada." : mensaje.Texto);
            }
            // TODO (otro issue): Fin
        }

        void AlDesconectarRed(string motivo)
        {
            CerrarPorRed(motivo);
        }

        void CerrarPorRed(string aviso)
        {
            if (cierreForzado) return;
            cierreForzado = true;   // que FormClosing no pregunte "¿quieres salir?"
            MessageBox.Show(this, aviso, "Partida", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.Cancel;
        }

        // Redibuja toda la mesa a partir de lo que mandó el servidor.
        // Asientos fijos: yo abajo, (yo+1)%4 izquierda, (yo+2)%4 arriba, (yo+3)%4 derecha.
        void AplicarEstado(EstadoJuego estado)
        {
            if (estado == null) return;
            estadoActual = estado;
            partidaTerminada = estado.Terminada;   // MostrarBurra ya consulta este campo

            const int N = 4;
            int yo = estado.TuIndice;
            int idxIzquierda = (yo + 1) % N;
            int idxArriba = (yo + 2) % N;
            int idxDerecha = (yo + 3) % N;

            // Mi mano: CartaDto -> Card
            List<Card> mano = new List<Card>();
            if (estado.TuMano != null)
            {
                foreach (CartaDto dto in estado.TuMano)
                {
                    Card c;
                    if (dto.TryACarta(out c)) mano.Add(c);
                }
            }

            // Carta superior del descarte y color activo
            Card sup = null;
            if (estado.CartaSuperior != null) estado.CartaSuperior.TryACarta(out sup);
            CardColor color;
            if (!Enum.TryParse(estado.ColorActivo, out color)) color = CardColor.Ninguno;

            // Resaltar jugables solo si es mi turno y no tengo un comodín robado por resolver
            bool esMiTurno = estado.Turno == yo && estado.CartaPendiente == null && !estado.Terminada;

            SuspendLayout();

            int siguiente = (estado.Turno + estado.Sentido + N) % N;
            labelTurno.Text = "Turno de: " + Nombre(estado, estado.Turno) +
                              "  →  Siguiente turno es: " + Nombre(estado, siguiente);
            labelBurra.Text = "Burra: " + estado.CartasBurra;
            labelDescarte.Text = "Descarte: " + estado.CartasDescarte + " cartas";

            label_jugador1.Text = Nombre(estado, yo) + ": " + CartasDe(estado, yo) + " cartas";
            label_jugador2.Text = Nombre(estado, idxIzquierda) + ": " + CartasDe(estado, idxIzquierda) + " cartas";
            label_jugador3.Text = Nombre(estado, idxArriba) + ": " + CartasDe(estado, idxArriba) + " cartas";
            label_jugador4.Text = Nombre(estado, idxDerecha) + ": " + CartasDe(estado, idxDerecha) + " cartas";

            MostrarManoJugador(mano, c => esMiTurno && JugadaValidaVista(c));
            MostrarDescarte(sup);
            MostrarBurra();
            MostrarRival(panel_rival_arriba, CartasDe(estado, idxArriba), RotateFlipType.Rotate180FlipNone, true);
            MostrarRival(panel_rival_izquierda, CartasDe(estado, idxIzquierda), RotateFlipType.Rotate90FlipNone, false);
            MostrarRival(panel_rival_derecha, CartasDe(estado, idxDerecha), RotateFlipType.Rotate270FlipNone, false);

            MostrarColorActivo(color);
            MostrarSentido(estado.Sentido);
            botonUno.Visible = estado.PuedesDecirUno;

            // Si ya no es mi turno, se cancela cualquier elección de color a medias
            if (estado.Turno != yo || estado.Terminada) comodinDeMano = null;

            // El selector de color se ve si hay comodín robado o uno de mi mano esperando color
            if (estado.CartaPendiente != null || comodinDeMano != null) MostrarSelectorColor();
            else OcultarSelectorColor();


            ResumeLayout();
        }

        // Copia de JugadaValida del servidor, leyendo estadoActual. Solo es ayuda visual:
        // quien decide si una jugada es válida es el servidor.
        bool JugadaValidaVista(Card carta)
        {
            if (estadoActual == null || estadoActual.CartaSuperior == null) return false;
            Card tope;
            if (!estadoActual.CartaSuperior.TryACarta(out tope)) return false;
            CardColor activo;
            if (!Enum.TryParse(estadoActual.ColorActivo, out activo)) return false;

            if (carta.Type == CardType.Comodin || carta.Type == CardType.Mas4) return true;
            if (carta.Color == activo) return true;
            if (carta.Type == CardType.Numero && tope.Type == CardType.Numero && carta.Number == tope.Number) return true;
            if (carta.Type != CardType.Numero && carta.Type == tope.Type) return true;
            return false;
        }

        void AgregarEvento(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto) || listaEventos.IsDisposed) return;

            // Se conservan solo las últimas 100 líneas
            string[] lineas = listaEventos.Lines;
            if (lineas.Length > 150)
            {
                string[] ultimas = new string[100];
                Array.Copy(lineas, lineas.Length - 100, ultimas, 0, 100);
                listaEventos.Lines = ultimas;
            }

            listaEventos.AppendText(texto + "\n");
            listaEventos.SelectionStart = listaEventos.TextLength;
            listaEventos.ScrollToCaret();
        }

        bool EsMiTurnoRed()
        {
            return estadoActual != null && !estadoActual.Terminada
                && estadoActual.Turno == estadoActual.TuIndice;
        }

        void MostrarSelectorColor()
        {
            SeleccionColor_Rojo.Visible = true;
            SeleccionColor_Azul.Visible = true;
            SeleccionColor_Amarillo.Visible = true;
            SeleccionColor_Verde.Visible = true;
        }

        // Clic en una carta de mi mano (solo manda el mensaje, no cambia nada local)
        void ClicCartaRed(Card carta)
        {
            if (!EsMiTurnoRed()) return;
            if (estadoActual.CartaPendiente != null || comodinDeMano != null) return;

            if (!JugadaValidaVista(carta))
            {
                Sonar("error");
                return;
            }

            if (carta.Type == CardType.Comodin || carta.Type == CardType.Mas4)
            {
                // Primero se elige el color; la carta se manda en ColorSeleccionadoRed
                Sonar("comodin");
                comodinDeMano = carta;
                MostrarSelectorColor();
                return;
            }

            cliente.Jugar(carta, CardColor.Ninguno);
        }

        // Clic en la burra
        void ClicBurraRed()
        {
            if (!EsMiTurnoRed()) return;
            if (estadoActual.CartaPendiente != null || comodinDeMano != null) return;
            cliente.Robar();
        }

        // Se eligió un color en el selector: o es un comodín de mi mano o es el que robé
        void ColorSeleccionadoRed(CardColor color)
        {
            if (comodinDeMano != null)
            {
                Card carta = comodinDeMano;
                comodinDeMano = null;
                cliente.Jugar(carta, color);
            }
            else if (estadoActual != null && estadoActual.CartaPendiente != null)
            {
                cliente.ElegirColor(color);
            }
        }
    }
}
