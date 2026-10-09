using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace UnoGame
{
    public partial class Form1 : Form
    {
        String jugador1, jugador2, jugador3, jugador4;
        int numJugadores = 4;
        Card ultimaCarta;
        int turno = 0;
        int sentido = 1;
        public CardColor colorActivo;
        Stack<Card> burra = new Stack<Card>();
        Stack<Card> descarte = new Stack<Card>();
        List<Card> Cartas1 = new List<Card>();
        List<Card> Cartas2 = new List<Card>();
        List<Card> Cartas3 = new List<Card>();
        List<Card> Cartas4 = new List<Card>();
        List<Player> jugadores = new List<Player>();
        List<Card>[] manosJugadores;
        public event Action<int> TurnoCambiado;
        public event Action<Player> PartidaTerminada;
        private bool partidaTerminada = false;
        private bool cierreForzado = false;
        Card cartaPendiente = null;
         bool esCartaRobada = false;
        Size tamanoOriginalBoton;
        Point locRojo, locAzul, locAmarillo, locVerde;
        public Form1(String j1, String j2, String j3, String j4)
        {
            jugador1 = j1;
            jugador2 = j2;
            jugador3 = j3;
            jugador4 = j4;

            InitializeComponent();
            
            iniciarTamanoDeBotones();
            iniciarColoresDeBotones();
            jugadores.Add(new Player(jugador1, Cartas1));
            jugadores.Add(new Player(jugador2, Cartas2));
            jugadores.Add(new Player(jugador3, Cartas3));
            jugadores.Add(new Player(jugador4, Cartas4));
            manosJugadores = new List<Card>[] { Cartas1, Cartas2, Cartas3, Cartas4 };
            TurnoCambiado += MostrarTurno;
            RepartirCartas();
        }
        
        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(cierreForzado)
            {
                return;
            }
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult res = MessageBox.Show("Quieres salir del juego? Se perdera el progreso", "Salir del juego", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if(res == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        void iniciarTamanoDeBotones()
        {
            tamanoOriginalBoton = SeleccionColor_Rojo.Size;
            locRojo = SeleccionColor_Rojo.Location;
            locAzul = SeleccionColor_Azul.Location;
            locAmarillo = SeleccionColor_Amarillo.Location;
            locVerde = SeleccionColor_Verde.Location;

        }

        Card SacarCarta()
        {
            if (burra.Count > 0)
            {
                return burra.Pop();
            }
            else
            { 
                ReponerMazo();
                if(burra.Count > 0)
                {
                    return burra.Pop();
                }
            }
            return null;
        }
      
        void MostrarTurno(int t)
        {
            ActualizarVista();
        }


        void AvanzarTurno()
        {
            turno = (turno + sentido + numJugadores) % numJugadores;
            if (TurnoCambiado != null)
            {
                TurnoCambiado.Invoke(turno);
            }
        }

        void ActualizarVista()
        {
            // La mesa gira: el jugador en turno siempre aparece abajo
            int idxIzquierda = (turno + sentido + numJugadores) % numJugadores;
            int idxArriba = (turno + 2 * sentido + numJugadores) % numJugadores;
            int idxDerecha = (turno + 3 * sentido + numJugadores) % numJugadores;

            // Quién juega y quién sigue
            labelTurno.Text = "Turno de: " + jugadores[turno].Nombre +
                              "  →  Siguiente turno es: " + jugadores[idxIzquierda].Nombre;
            labelTurno.Font = new Font(labelTurno.Font, FontStyle.Bold);

            // Contadores de la burra y del descarte
            labelBurra.Text = "Burra: " + CartasRestantes();
            labelDescarte.Text = "Descarte: " + descarte.Count + " cartas";

            // Cantidad de cartas de los 4 jugadores (uno por posición)
            label_jugador1.Text = jugadores[turno].Nombre + ": " +
                                  jugadores[turno].cantidad_cartas + " cartas";
            label_jugador2.Text = jugadores[idxIzquierda].Nombre + ": " +
                                  jugadores[idxIzquierda].cantidad_cartas + " cartas";
            label_jugador3.Text = jugadores[idxArriba].Nombre + ": " +
                                  jugadores[idxArriba].cantidad_cartas + " cartas";
            label_jugador4.Text = jugadores[idxDerecha].Nombre + ": " +
                                  jugadores[idxDerecha].cantidad_cartas + " cartas";

            // Dibujar zonas
            MostrarManoJugador();
            MostrarDescarte();
            MostrarBurra();
            MostrarRival(panel_rival_arriba, idxArriba);
            MostrarRival(panel_rival_izquierda, idxIzquierda);
            MostrarRival(panel_rival_derecha, idxDerecha);

            // Indicadores de estado
            MostrarColorActivo();
            MostrarSentido();
            ActualizarBotonUno();
        }

        private void ActualizarBotonUno()
        {
            bool debeAparecer =
                !partidaTerminada &&
                jugadores[turno].cantidad_cartas == 2 &&
                !jugadores[turno].DijoUno &&
                TieneJugadaValida(manosJugadores[turno]);

            button3.Visible = debeAparecer;
        }

        private void MostrarColorActivo()
        {
            Color c;
            switch (colorActivo)
            {
                case CardColor.Rojo: c = Color.Red; break;
                case CardColor.Azul: c = Color.RoyalBlue; break;
                case CardColor.Verde: c = Color.Green; break;
                case CardColor.Amarillo: c = Color.Gold; break;
                default: c = Color.Gray; break;
            }
            panelColor.BackColor = c;
            labelColor.Text = "Color actual: " + colorActivo;
        }

        private void MostrarSentido()
        {
            if (sentido == 1)
                labelSentido.Text = "Sentido: Izquierda";
            else
                labelSentido.Text = "Sentido: Derecha";
        }

        private void MostrarRival(Panel panel, int indiceJugador)
        {
            panel.Controls.Clear();
            int cantidad = jugadores[indiceJugador].cantidad_cartas;

            int x = 5;
            for (int i = 0; i < cantidad; i++)
            {
                CartaControl cc = new CartaControl();
                cc.BocaAbajo = true;
                cc.Size = new Size(40, 60); // más chicas para que quepan varias
                cc.Location = new Point(x, 5);
                panel.Controls.Add(cc);
                cc.BringToFront();
                x += 15; // se van sobreponiendo, como un abanico
            }
        }

        private void MostrarManoJugador()
        {
            panel_mano_jugador1.Controls.Clear();
            List<Card> mano = manosJugadores[turno];

            int anchoCarta = 70;
            int separacion = 75;
            int separacionMinima = 40;

            int espacioDisponible = panel_mano_jugador1.Width - anchoCarta;

            int separacionReal;

            if(mano.Count > 1)
            {
                separacionReal = Math.Min(separacion, espacioDisponible / (mano.Count - 1));
                if(separacionReal < separacionMinima)
                {
                    separacionReal = separacionMinima;
                }
            }
            else
            {
                separacionReal = separacion;
            }

            int anchoTotal = anchoCarta + (mano.Count - 1) * separacionReal;
            panel_mano_jugador1.AutoScroll = true;
            if(anchoTotal > panel_mano_jugador1.Width)
            {
                panel_mano_jugador1.AutoScrollMinSize = new Size(anchoTotal, 0);
            }
            else
            {
                panel_mano_jugador1.AutoScrollMinSize = new Size(0, 0);
            }
            int x = 10;
            foreach (Card carta in mano)
            {
                CartaControl cc = new CartaControl();
                cc.Carta = carta;
                cc.BocaAbajo = false;
                cc.EsJugable = JugadaValida(carta);
                cc.Location = new Point(x, 10);

                Card cartaCapturada = carta; // evita el problema de captura en el foreach
                cc.Click += (s, e) => IntentarJugarCarta(cartaCapturada);

                panel_mano_jugador1.Controls.Add(cc);
                x += separacionReal;
            }
        }

        private void MostrarDescarte()
        {
            panelDescarte.Controls.Clear();
            Card sup = VerCartaSuperior();
            if (sup != null)
            {
                CartaControl cc = new CartaControl();
                cc.Carta = sup;
                cc.BocaAbajo = false;
                cc.Location = new Point(5, 5);
                panelDescarte.Controls.Add(cc);
            }
        }

        private void MostrarBurra()
        {
            panelBurra.Controls.Clear();
            if (!partidaTerminada && CartasRestantes() > 0)
            {
                CartaControl cc = new CartaControl();
                cc.BocaAbajo = true;
                cc.Location = new Point(5, 5);
                cc.Click += (s, e) => TomarCartaYIntentarJugar(jugadores[turno].Nombre);
                cc.Cursor = Cursors.Hand;
                panelBurra.Controls.Add(cc);
            }
        }

        private void IntentarJugarCarta(Card cartaElegida)
        {
            if (cartaPendiente != null) return;
            if (partidaTerminada)
            {
                MessageBox.Show("La partida ya terminó.");
                return;
            }

            if (!JugadaValida(cartaElegida))
            {
                MessageBox.Show("Esa carta no es una jugada válida.");
                return;
            }

            if (cartaElegida.Type == CardType.Comodin || cartaElegida.Type == CardType.Mas4)
            {
                cartaPendiente = cartaElegida;
                esCartaRobada = false;
                SeleccionColor_Rojo.Visible = true;
                SeleccionColor_Azul.Visible = true;
                SeleccionColor_Amarillo.Visible = true;
                SeleccionColor_Verde.Visible = true;

                return; 
            }

            // Si es una carta normal, se juega directamente
            JugarCarta(cartaElegida, CardColor.Ninguno);
        }
        void CallUno(Player jugador)
        {
            jugador.callUno();
        }

        void InvertirSentido()
        {
            sentido = -sentido;
        }

        int CartasRestantes()
        {
            return burra.Count;
        }
        private void BarajarMazo()
        {
            List<Card> mazo = new List<Card>();
            CardColor[] colores = { CardColor.Rojo, CardColor.Azul, CardColor.Verde, CardColor.Amarillo };
            foreach (CardColor color in colores)
            {
                mazo.Add(new Card(0, color, CardType.Numero));
                for (int i = 1; i <= 9; i++)
                {
                    mazo.Add(new Card(i, color, CardType.Numero));
                    mazo.Add(new Card(i, color, CardType.Numero));
                }
                for (int i = 0; i < 2; i++)
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

            Random rnd = new Random();
            int n = mazo.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                Card value = mazo[k];
                mazo[k] = mazo[n];
                mazo[n] = value;
            }
            burra.Clear();
            foreach (Card carta in mazo)
                burra.Push(carta);
        }
        private void JugarCarta(Card cartaJugada, CardColor colorElegido = CardColor.Ninguno)
        {
            if (cartaPendiente != null) return;
            if (partidaTerminada)
            {
                MessageBox.Show("La partida ya terminó.");
                return;
            }

            if (!JugadaValida(cartaJugada))
            {
                MessageBox.Show("La jugada no es valida");
                return;
            }


            descarte.Push(cartaJugada);
            jugadores[turno].Quitar_carta(cartaJugada);

            if (jugadores[turno].cantidad_cartas == 0)
            {
                TerminarPartida(jugadores[turno]);
                return;
            }

            if (jugadores[turno].cantidad_cartas == 1)
            {
                if (jugadores[turno].DijoUno)
                {
                    MessageBox.Show($"{jugadores[turno].Nombre} dijo UNO.");
                }
                else
                {
                    MessageBox.Show($"{jugadores[turno].Nombre} no dijo UNO, come 2 cartas.");
                    for (int i = 0; i < 2; i++)
                        jugadores[turno].Agregar_carta(SacarCarta());
                }
            }

            if (cartaJugada.Type == CardType.Comodin || cartaJugada.Type == CardType.Mas4)
                colorActivo = colorElegido;
            else
                colorActivo = cartaJugada.Color;

            if (cartaJugada.Type == CardType.Reversa)
                InvertirSentido();

            if (cartaJugada.Type == CardType.Salta)
            {
                int jugadorSaltado = (turno + sentido + numJugadores) % numJugadores;
                MessageBox.Show($"{jugadores[jugadorSaltado].Nombre} pierde turno.");
                AvanzarTurno();
            }

            if (cartaJugada.Type == CardType.Mas2)
            {
                int siguienteJugador = (turno + sentido + numJugadores) % numJugadores;
                for (int i = 0; i < 2; i++)
                {
                    jugadores[siguienteJugador].Agregar_carta(SacarCarta());
                }
                MessageBox.Show($"{jugadores[siguienteJugador].Nombre} come 2 cartas.");
                AvanzarTurno();
            }

            if (cartaJugada.Type == CardType.Mas4)
            {
                int siguienteJugador = (turno + sentido + numJugadores) % numJugadores;
                for (int i = 0; i < 4; i++)
                {
                    jugadores[siguienteJugador].Agregar_carta(SacarCarta());
                }
                MessageBox.Show($"{jugadores[siguienteJugador].Nombre} come 4 cartas.");
                AvanzarTurno();
            }

            AvanzarTurno();

        }
        public bool JugadaValida(Card cartaJugada)
        {
            Card ultimaCarta = VerCartaSuperior();
         

            if (cartaJugada.Type == CardType.Comodin || cartaJugada.Type == CardType.Mas4)
                return true;
            CardColor colorenjuego = (ultimaCarta.Color == CardColor.Ninguno) ? colorActivo : ultimaCarta.Color;
            if (cartaJugada.Color == colorenjuego)
                return true;
            if (cartaJugada.Type == CardType.Numero && ultimaCarta.Type == CardType.Numero)
                if (cartaJugada.Number == ultimaCarta.Number)
                    return true;
            if (cartaJugada.Type != CardType.Numero && cartaJugada.Type == ultimaCarta.Type)
                return true;
            return false;
        }
        public bool TieneJugadaValida(List<Card> mano)
        {
            foreach (Card carta in mano)
            {
                if (JugadaValida(carta))
                    return true;
            }
            return false;
        }
        public Card VerCartaSuperior()
        {
            if (descarte.Count > 0)
            {
                return descarte.Peek();
            }
            return null;
        }
        private void ReponerMazo()
        {
            if (descarte.Count > 0)
            {
                Card cartaSuperior = descarte.Pop();
                List<Card> cartasRestantes = new List<Card>(descarte);
                descarte.Clear();
                descarte.Push(cartaSuperior);
                Random rnd = new Random();
                int n = cartasRestantes.Count;
                while (n > 1)
                {
                    n--;
                    int k = rnd.Next(n + 1);
                    Card value = cartasRestantes[k];
                    cartasRestantes[k] = cartasRestantes[n];
                    cartasRestantes[n] = value;
                }
                foreach (Card c in cartasRestantes)
                    burra.Push(c);
            }
        }


        private void TerminarPartida(Player ganador)
        {
            partidaTerminada = true;
            FormFinPartida formFin = new FormFinPartida();
            formFin.MostrarGanador(ganador.Nombre);
            formFin.ShowDialog();

            if (formFin.DialogResult == DialogResult.Retry)
            {
                ReiniciarPartida();
            }

            else
            {
                cierreForzado = true;
                this.DialogResult = DialogResult.Abort;
                this.Close();
            }

            if (PartidaTerminada != null)
                PartidaTerminada.Invoke(ganador);

            ActualizarVista();
        }

        private void ReiniciarPartida()
        {
            turno = 0;
            sentido = 1;
            RepartirCartas();
        }
        private void JugarTurno()
        {
            if (cartaPendiente != null) return;
            if (partidaTerminada)
            {
                MessageBox.Show("La partida ya terminó. Inicia una nueva partida para seguir jugando.");
                return;
            }

            List<Card> manoActual = manosJugadores[turno];
            string nombreJugadorActual = jugadores[turno].Nombre;

            while (true)
            {
                string mano = "";
                for (int i = 0; i < manoActual.Count; i++)
                    mano += $"{i}: {manoActual[i]}\n";

                string entrada = Interaction.InputBox(
                    $"Turno de {nombreJugadorActual}\nCarta en mesa: {VerCartaSuperior()} (color activo: {colorActivo})\n\n{mano}\n" +
                    "Escribe el número de la carta que quieres jugar, o -1 si no tienes ninguna jugable:",
                    "Jugar carta", "-1");

                int indice;
                if (!int.TryParse(entrada, out indice))
                {
                    MessageBox.Show("Entrada no válida, intenta de nuevo.");
                    continue;
                }

                if (indice == -1)
                {
                    TomarCartaYIntentarJugar(nombreJugadorActual);
                    return;
                }

                if (indice < 0 || indice >= manoActual.Count)
                {
                    MessageBox.Show("Ese número no corresponde a ninguna carta de tu mano. Intenta de nuevo.");
                    continue;
                }

                Card cartaElegida = manoActual[indice];

                if (JugadaValida(cartaElegida))
                {
                    CardColor colorElegido = CardColor.Ninguno;
                    if (cartaElegida.Type == CardType.Comodin || cartaElegida.Type == CardType.Mas4)
                    {
                        cartaPendiente = cartaElegida;
                        esCartaRobada = false;
                        SeleccionColor_Rojo.Visible = true;
                        SeleccionColor_Azul.Visible = true;
                        SeleccionColor_Amarillo.Visible = true;
                        SeleccionColor_Verde.Visible = true;
                        return;
                    }


                    JugarCarta(cartaElegida, colorElegido);
                    MessageBox.Show($"{nombreJugadorActual} jugó {cartaElegida}");
                    return;
                }
                else
                {
                    MessageBox.Show("Esa carta no es una jugada válida. Elige otra.");
                    continue;
                }
            }
        }

        private void TomarCartaYIntentarJugar(string nombreJugadorActual)
        {
            if (cartaPendiente != null) return;
            if (partidaTerminada)
            {
                MessageBox.Show("La partida ya terminó.");
                return;
            }

            Card cartaRobada = SacarCarta();

            if (cartaRobada == null)
            {
                MessageBox.Show("No quedan cartas para robar.");
                AvanzarTurno();
                return;
            }

            if (JugadaValida(cartaRobada))
            {
            
                if (cartaRobada.Type == CardType.Comodin || cartaRobada.Type == CardType.Mas4)
                {
                    MessageBox.Show($"{nombreJugadorActual} robó un comodín y se jugó automáticamente.");
                    cartaPendiente = cartaRobada;
                    esCartaRobada = true;
                    SeleccionColor_Rojo.Visible = true;
                    SeleccionColor_Azul.Visible = true;
                    SeleccionColor_Amarillo.Visible = true;
                    SeleccionColor_Verde.Visible = true;
                    return; 
                }

                MessageBox.Show($"{nombreJugadorActual} robó {cartaRobada} y se jugó automáticamente.");
                jugadores[turno].Agregar_carta(cartaRobada);
                JugarCarta(cartaRobada, CardColor.Ninguno);
            }
            else
            {
                jugadores[turno].Agregar_carta(cartaRobada);
                MessageBox.Show($"{nombreJugadorActual} robó {cartaRobada} y no es jugable, se la queda.");
                AvanzarTurno();
            }
        }

        private CardColor PedirColor()
        {
            string entrada = Interaction.InputBox(
                "Elige un color (Rojo, Azul, Verde, Amarillo):", "Elegir color", "Rojo");

            CardColor color;
            if (Enum.TryParse(entrada, true, out color) && color != CardColor.Ninguno)
                return color;

            return CardColor.Rojo;
        }
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void botonRepartir(object sender, EventArgs e)
        {
            RepartirCartas();
        }


        public class Player
        {
            public string Nombre;
            private List<Card> mano;
            public bool DijoUno;

            public void callUno()
            {
                if (mano.Count == 2)
                {
                    DijoUno = true;
                }
            }
            public Player(string nombre, List<Card> mano)
            {
                this.Nombre = nombre;
                this.mano = mano;
                this.DijoUno = false;
            }
            public int cantidad_cartas
            {
                get { return mano.Count; }
            }
            public void Agregar_carta(Card carta)
            {
                if (carta != null)
                {
                    mano.Add(carta);
                    DijoUno = false;
                }
            }
            public bool Quitar_carta(Card carta)
            {
                return mano.Remove(carta);
            }
        }
        private void RepartirCartas()
        {
            partidaTerminada = false;

            Cartas1.Clear();
            Cartas2.Clear();
            Cartas3.Clear();
            Cartas4.Clear();

            BarajarMazo();

            foreach (Player jugador in jugadores)
            {
                for (int i = 0; i < 7; i++)
                {
                    jugador.Agregar_carta(SacarCarta());
                }
            }
            ultimaCarta = SacarCarta();

            // Si la primera carta es Comodín +4, se devuelve al mazo, se barajea junto con lo que queda y se voltea otra
            while (ultimaCarta != null && ultimaCarta.Type == CardType.Mas4)
            {
                DevolverYBarajar(ultimaCarta);
                ultimaCarta = SacarCarta();
            }
            descarte.Clear();
            descarte.Push(ultimaCarta);
            colorActivo = ultimaCarta.Color;
            ActualizarVista();
        }

        private void DevolverYBarajar(Card carta)
        {
            List<Card> restante = new List<Card>(burra);
            restante.Add(carta);

            Random rnd = new Random();
            int n = restante.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                Card value = restante[k];
                restante[k] = restante[n];
                restante[n] = value;
            }

            burra.Clear();
            foreach (Card c in restante)
                burra.Push(c);
        }
        private void botonBarajar(object sender, EventArgs e)
        {
            BarajarMazo();
        }

        private void labelBurra_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void cartaControl1_Load(object sender, EventArgs e)
        {

        }

        private void panel_mano_jugador1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void SeleccionColor_Rojo_MouseEnter(object sender, EventArgs e)
        {
            SeleccionColor_Rojo.BackColor = Color.Red;
            SeleccionColor_Rojo.Size = new Size(tamanoOriginalBoton.Width + 12, tamanoOriginalBoton.Height + 12);
            SeleccionColor_Rojo.Location = new Point(locRojo.X - 6, locRojo.Y - 6);
        }

        private void SeleccionColor_Rojo_MouseLeave(object sender, EventArgs e)
        {
            SeleccionColor_Rojo.BackColor = Color.FromArgb(180, 40, 40);
            SeleccionColor_Rojo.Size = tamanoOriginalBoton;
            SeleccionColor_Rojo.Location = locRojo;
        }

        private void SeleccionColor_Amarillo_MouseEnter(object sender, EventArgs e)
        {
            SeleccionColor_Amarillo.BackColor = Color.Yellow;
            SeleccionColor_Amarillo.Size = new Size(tamanoOriginalBoton.Width + 12, tamanoOriginalBoton.Height + 12);
            SeleccionColor_Amarillo.Location = new Point(locAmarillo.X - 6, locAmarillo.Y - 6);
        }

        private void SeleccionColor_Amarillo_MouseLeave(object sender, EventArgs e)
        {
            SeleccionColor_Amarillo.BackColor = Color.FromArgb(210, 160, 20);
            SeleccionColor_Amarillo.Size = tamanoOriginalBoton;
            SeleccionColor_Amarillo.Location = locAmarillo;
        }


        private void SeleccionColor_Azul_MouseEnter(object sender, EventArgs e)
        {
            SeleccionColor_Azul.BackColor = Color.DodgerBlue;
            SeleccionColor_Azul.Size = new Size(tamanoOriginalBoton.Width + 12, tamanoOriginalBoton.Height + 12);
            SeleccionColor_Azul.Location = new Point(locAzul.X - 6, locAzul.Y - 6);
        }

        private void SeleccionColor_Azul_MouseLeave(object sender, EventArgs e)
        {
            SeleccionColor_Azul.BackColor = Color.FromArgb(0, 90, 180);
            SeleccionColor_Azul.Size = tamanoOriginalBoton;
            SeleccionColor_Azul.Location = locAzul;
        }

        private void SeleccionColor_Verde_MouseEnter(object sender, EventArgs e)
        {
            SeleccionColor_Verde.BackColor = Color.LimeGreen;
            SeleccionColor_Verde.Size = new Size(tamanoOriginalBoton.Width + 12, tamanoOriginalBoton.Height + 12);
            SeleccionColor_Verde.Location = new Point(locVerde.X - 6, locVerde.Y - 6);
        }

        private void SeleccionColor_Verde_MouseLeave(object sender, EventArgs e)
        {
            SeleccionColor_Verde.BackColor = Color.FromArgb(30, 140, 60);
            SeleccionColor_Verde.Size = tamanoOriginalBoton;
            SeleccionColor_Verde.Location = locVerde;
        }

        private void SeleccionColor_Rojo_Click(object sender, EventArgs e)
{
    ColorSeleccionado(CardColor.Rojo);
}

private void SeleccionColor_Amarillo_Click(object sender, EventArgs e)
{
    ColorSeleccionado(CardColor.Amarillo);
}

private void SeleccionColor_Azul_Click(object sender, EventArgs e)
{
    ColorSeleccionado(CardColor.Azul);
}

private void SeleccionColor_Verde_Click(object sender, EventArgs e)
{
    ColorSeleccionado(CardColor.Verde);
}

        private void Form1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            CallUno(jugadores[turno]);
            button3.Visible = false;
        }

        private void ColorSeleccionado(CardColor colorElegido)
{
    OcultarSelectorColor();

    if (cartaPendiente != null)
    {
        Card cartaAProcesar = cartaPendiente;
        cartaPendiente = null;

        if (esCartaRobada)
        {
            esCartaRobada = false;
            jugadores[turno].Agregar_carta(cartaAProcesar);
        }

        JugarCarta(cartaAProcesar, colorElegido);
    }
}

        
        void iniciarColoresDeBotones() // son botones rgb  no salen en la paleta de colores predeterminada
        {
            
            SeleccionColor_Rojo.BackColor = Color.FromArgb(180, 40, 40);
            SeleccionColor_Azul.BackColor = Color.FromArgb(0, 90, 180);
            SeleccionColor_Amarillo.BackColor = Color.FromArgb(210, 160, 20);
            SeleccionColor_Verde.BackColor = Color.FromArgb(30, 140, 60);
        }
        private void OcultarSelectorColor()
        {
            SeleccionColor_Rojo.Size = tamanoOriginalBoton;
            SeleccionColor_Azul.Size = tamanoOriginalBoton;
            SeleccionColor_Amarillo.Size = tamanoOriginalBoton;
            SeleccionColor_Verde.Size = tamanoOriginalBoton;

            SeleccionColor_Rojo.Location = locRojo;
            SeleccionColor_Azul.Location = locAzul;
            SeleccionColor_Amarillo.Location = locAmarillo;
            SeleccionColor_Verde.Location = locVerde;

            iniciarColoresDeBotones(); 

            SeleccionColor_Rojo.Visible = false;
            SeleccionColor_Azul.Visible = false;
            SeleccionColor_Amarillo.Visible = false;
            SeleccionColor_Verde.Visible = false;
        }


        private void Avanzar_turno_Click(object sender, EventArgs e)
        {
            AvanzarTurno();
        }

        private void Invertir_turno_Click(object sender, EventArgs e)
        {
            InvertirSentido();
            ActualizarVista();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            JugarTurno();
        }
    }
}