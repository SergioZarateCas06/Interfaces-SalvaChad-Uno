using Microsoft.VisualBasic;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Media;
using System.Windows.Forms;

namespace UnoGame
{
    public partial class Form1 : Form
    {
        Dictionary<string, SoundPlayer> sonidos = new Dictionary<string, SoundPlayer>();
        Mp3FileReader lectorMusica;
        WaveOutEvent salidaMusica;
        bool silenciado = false;
        bool cerrando = false;
        String jugador1, jugador2, jugador3, jugador4;
        int numJugadores = 4;
        CartaControl controlPendiente;
        Point inicioVuelo;   // de dónde sale el vuelo actual
        System.Windows.Forms.Timer timerReparto = new System.Windows.Forms.Timer();
        Control cartaVolando;   // CartaControl o PictureBox, según el destino
        Card cartaEnVuelo;
        Point origenVuelo = new Point(390, 211);   // de aquí salen las cartas
        Point destinoVuelo;
        int pasoVuelo;
        int pausa;
        const int PASOS_VUELO = 10;   // más pasos = vuelo más lento
        const int TICKS_PAUSA = 5;   // pausa entre carta y carta (cada tick son ~15 ms)
        int repartoRonda = 0;
        int repartoJugador = 0;
        bool repartiendo = false;
        int turno = 0;
        int sentido = 1;
        public CardColor colorActivo;
        Stack<Card> burra = new Stack<Card>();
        Stack<Card> descarte = new Stack<Card>();
        List<Card> Cartas1 = new List<Card>();
        List<Card> Cartas2 = new List<Card>();
        List<Card> Cartas3 = new List<Card>();
        List<Card> Cartas4 = new List<Card>();
        bool robando = false;
        bool comiendo = false;
        int comerRestantes;
        int comerJugador;
        Action alTerminarComer;
        bool esperandoColorRobada = false;   // la carta robada ya aterrizó y espera que se elija color
        Action alAterrizar;   // lo que se hace cuando la carta robada llega a la mano
        List<Player> jugadores = new List<Player>();
        List<Card>[] manosJugadores;
        public event Action<int> TurnoCambiado;
        public event Action<Player> PartidaTerminada;
        private bool partidaTerminada = false;
        private readonly ApiCliente api = new ApiCliente();
        private int partidaIdActual = -1;
        private int numeroTurnoActual = 0;
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
            CargarSonidos();
            buttonSilencio.Click += (s, e) => AlternarSilencio();
            IniciarMusica();
            iniciarTamanoDeBotones();
            iniciarColoresDeBotones();
            jugadores.Add(new Player(jugador1, Cartas1));
            jugadores.Add(new Player(jugador2, Cartas2));
            jugadores.Add(new Player(jugador3, Cartas3));
            jugadores.Add(new Player(jugador4, Cartas4));
            manosJugadores = new List<Card>[] { Cartas1, Cartas2, Cartas3, Cartas4 };
            TurnoCambiado += MostrarTurno;
            timerReparto.Interval = 15;
            timerReparto.Tick += TimerReparto_Tick;
            this.Shown += (s, e) => RepartirCartas();
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
            MostrarRival(panel_rival_arriba, idxArriba, RotateFlipType.Rotate180FlipNone, true);
            MostrarRival(panel_rival_izquierda, idxIzquierda, RotateFlipType.Rotate90FlipNone, false);
            MostrarRival(panel_rival_derecha, idxDerecha, RotateFlipType.Rotate270FlipNone, false); 

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

            botonUno.Visible = debeAparecer;
        }
       
        void CargarSonidos()
        {
            AgregarSonido("carta", Properties.Resources.carta);
            AgregarSonido("boton", Properties.Resources.boton);
            AgregarSonido("error", Properties.Resources.error);
            AgregarSonido("comodin", Properties.Resources.comodina);
            AgregarSonido("victoria", Properties.Resources.victoria);
     
        
         
        }
        void IniciarMusica()
        {
            try
            {
                lectorMusica = new Mp3FileReader(Properties.Resources.musica);
                salidaMusica = new WaveOutEvent();
                salidaMusica.Init(lectorMusica);
                salidaMusica.Volume = 0.25f;   // 0.0 a 1.0

                // Repetir en bucle al terminar
                salidaMusica.PlaybackStopped += (s, e) =>
                {
                    if (cerrando) return;
                    lectorMusica.Position = 0;
                    salidaMusica.Play();
                };

                salidaMusica.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo iniciar la música: " + ex.Message);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            cerrando = true;
            if (salidaMusica != null) salidaMusica.Dispose();
            if (lectorMusica != null) lectorMusica.Dispose();
            base.OnFormClosed(e);
        }
        void AgregarSonido(string nombre, Stream flujo)
        {
            SoundPlayer sp = new SoundPlayer(flujo);
            sp.Load();
            sonidos[nombre] = sp;
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

        private void MostrarRival(Panel panel, int indiceJugador, RotateFlipType giro, bool horizontal)
        {

            panel.Controls.Clear();
            int cantidad = jugadores[indiceJugador].cantidad_cartas;

            int pos = 5;
            int anchoCartaMini = 40;
            int separacionIdeal = 15;
            int separacionMinima = 10; 

            int espacioDisponible = (horizontal ? panel.Width : panel.Height) - anchoCartaMini;
            int separacionReal;

            if (cantidad > 1)
            {
                separacionReal = Math.Min(separacionIdeal, espacioDisponible / (cantidad - 1));
                if (separacionReal < separacionMinima)
                    separacionReal = separacionMinima;
            }
            else
            {
                separacionReal = separacionIdeal;
            }
            for (int i = 0; i < cantidad; i++)
            {
                Bitmap bmp = new Bitmap(40, 60, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.Transparent);
                    g.SmoothingMode = SmoothingMode.AntiAlias;

                    Rectangle area = new Rectangle(1, 1, bmp.Width - 3, bmp.Height - 3);
                    using (GraphicsPath path = CartaControl.RedondearRectangulo(area, 8))
                    {
                        Image imgReverso = CartaControl.ObtenerImagen("reverso");
                        if (imgReverso != null)
                        {
                            CartaControl.DibujarImagenRecortada(g, path, imgReverso);
                        }
                        using (Pen pen = new Pen(Color.Black, 2))
                        {
                            g.DrawPath(pen, path);
                        }
                    }
                }
                bmp.RotateFlip(giro);

                PictureBox pb = new PictureBox();
                pb.Image = bmp;
                pb.Size = bmp.Size;
                pb.BackColor = Color.Transparent;
                pb.Location = horizontal ? new Point(pos, 5) : new Point(5, pos);

                panel.Controls.Add(pb);
                pb.BringToFront();
                pos += separacionReal; // se sobreponen como abanico
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
            if (anchoTotal > panel_mano_jugador1.Width)
            {
                panel_mano_jugador1.AutoScroll = true;
                panel_mano_jugador1.AutoScrollMinSize = new Size(anchoTotal, 0);
            }
            else
            {
                panel_mano_jugador1.AutoScroll = false;
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

                Card cartaCapturada = carta;
                cc.Click += (s, e) => IntentarJugarCarta(cartaCapturada);

                panel_mano_jugador1.Controls.Add(cc);
                x += separacionReal;
            }
        }
        void Sonar(string nombre)
        {
            if (silenciado) return;   // NUEVO
            SoundPlayer sp;
            if (sonidos.TryGetValue(nombre, out sp))
                sp.Play();
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

        private void IntentarJugarCarta(Card cartaElegida, CartaControl origen = null)
        {
            if (repartiendo || robando || comiendo) return;
            if (cartaPendiente != null) return;
            if (partidaTerminada)
            {
                MessageBox.Show("La partida ya terminó.");
                return;
            }

            if (!JugadaValida(cartaElegida))
            {
                Sonar("error");   
                MessageBox.Show("Esa carta no es una jugada válida.");
                return;
            }

            if (cartaElegida.Type == CardType.Comodin || cartaElegida.Type == CardType.Mas4)
            {
                Sonar("comodin");
                cartaPendiente = cartaElegida;
                controlPendiente = origen;
                esCartaRobada = false;
                SeleccionColor_Rojo.Visible = true;
                SeleccionColor_Azul.Visible = true;
                SeleccionColor_Amarillo.Visible = true;
                SeleccionColor_Verde.Visible = true;
                return;
            }

            JugarConAnimacion(origen, cartaElegida, CardColor.Ninguno);
        }
        void AlternarSilencio()
        {
            silenciado = !silenciado;

            if (silenciado)
            {
                if (salidaMusica != null) salidaMusica.Pause();
                foreach (SoundPlayer sp in sonidos.Values) sp.Stop();   // corta el efecto que esté sonando
            }
            else
            {
                if (salidaMusica != null) salidaMusica.Play();
            }

            buttonSilencio.Text = silenciado ? "🔇 Sonido: OFF" : "🔊 Sonido: ON";
        }
        private void JugarConAnimacion(CartaControl origen, Card carta, CardColor color)
        {
            // Sin control de origen (o ya redibujado): se juega directo
            if (origen == null || origen.IsDisposed || origen.Parent == null)
            {
                JugarCarta(carta, color);
                return;
            }

            Point desde = this.PointToClient(origen.Parent.PointToScreen(origen.Location));

            CartaControl cc = new CartaControl();
            cc.Carta = carta;
            cc.BocaAbajo = false;
            cc.EsJugable = true;
            cartaVolando = cc;

            origen.Visible = false;   // la carta sale de la mano
            robando = true;           // bloquea clics mientras vuela
            pausa = 0;

            alAterrizar = () => JugarCarta(carta, color);
            IniciarVuelo(panelDescarte, new Point(5, 5), desde);
            timerReparto.Start();
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
        private async void JugarCarta(Card cartaJugada, CardColor colorElegido = CardColor.Ninguno)
        {
            if (cartaPendiente != null) return;
            if (partidaTerminada)
            {
                MessageBox.Show("La partida ya terminó.");
                return;
            }

            if (!JugadaValida(cartaJugada))
            {
                Sonar("error");
                MessageBox.Show("La jugada no es valida");
                return;
            }


            descarte.Push(cartaJugada);
            jugadores[turno].Quitar_carta(cartaJugada);

            numeroTurnoActual++;
            if (partidaIdActual != -1)
            {
                try
                {
                    await api.RegistrarMovimiento(partidaIdActual, numeroTurnoActual, jugadores[turno].Nombre, "Jugar", cartaJugada.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No se pudo registrar el movimiento: " + ex.Message);
                }
            }

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
                    Sonar("error");
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

            if (cartaJugada.Type == CardType.Mas2 || cartaJugada.Type == CardType.Mas4)
            {
                int cantidad = (cartaJugada.Type == CardType.Mas2) ? 2 : 4;
                int siguienteJugador = (turno + sentido + numJugadores) % numJugadores;

                ActualizarVista();   // que la carta jugada ya se vea en el descarte antes de comer

                ComerConAnimacion(siguienteJugador, cantidad, () =>
                {
                    MessageBox.Show($"{jugadores[siguienteJugador].Nombre} come {cantidad} cartas.");
                    AvanzarTurno();   // el que comió pierde su turno
                    AvanzarTurno();
                });
                return;
            }

            AvanzarTurno();

        }
        public bool JugadaValida(Card cartaJugada)
        {
            Card ultimaCarta = VerCartaSuperior();
            if (ultimaCarta == null) return false;

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


        private async void TerminarPartida(Player ganador)
        {
            Sonar("victoria");
            partidaTerminada = true;

            if (partidaIdActual != -1)
            {
                try
                {
                    await api.RegistrarGanador(partidaIdActual, ganador.Nombre);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No se pudo registrar el ganador: " + ex.Message);
                }
            }
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
            if (repartiendo || robando || comiendo) return;
            if (cartaPendiente != null) return;
            if (partidaTerminada)
            {
                MessageBox.Show("La partida ya terminó.");
                return;
            }

            Card cartaRobada = SacarCarta();

            if (cartaRobada == null)
            {
                Sonar("error");
                MessageBox.Show("No quedan cartas para robar.");
                AvanzarTurno();
                return;
            }

            robando = true;

            CartaControl cc = new CartaControl();
            cc.Carta = cartaRobada;
            cc.BocaAbajo = false;
            cc.EsJugable = JugadaValida(cartaRobada);
            cartaVolando = cc;

            bool esComodin = cartaRobada.Type == CardType.Comodin || cartaRobada.Type == CardType.Mas4;
            bool seJuegaSola = JugadaValida(cartaRobada);

            pausa = 0;

            if (esComodin)
            {
                // Vuela al descarte y, al llegar, pide el color (la carta se queda a la vista)
                alAterrizar = () =>
                {
                    Sonar("comodin");
                    cartaPendiente = cartaRobada;
                    controlPendiente = null;
                    esCartaRobada = true;
                    esperandoColorRobada = true;

                    SeleccionColor_Rojo.Visible = true;
                    SeleccionColor_Azul.Visible = true;
                    SeleccionColor_Amarillo.Visible = true;
                    SeleccionColor_Verde.Visible = true;

                    // que la carta que vuela no tape los botones
                    SeleccionColor_Rojo.BringToFront();
                    SeleccionColor_Azul.BringToFront();
                    SeleccionColor_Amarillo.BringToFront();
                    SeleccionColor_Verde.BringToFront();
                };
                IniciarVuelo(panelDescarte, new Point(5, 5));
            }
            else if (seJuegaSola)
            {
                alAterrizar = () =>
                {
                    jugadores[turno].Agregar_carta(cartaRobada);
                    JugarCarta(cartaRobada, CardColor.Ninguno);
                };
                IniciarVuelo(panelDescarte, new Point(5, 5));
            }
            else
            {
                Sonar("error");
                int n = jugadores[turno].cantidad_cartas;
                int x = 10 + n * SeparacionMano(n + 1);
                x = Math.Min(x, panel_mano_jugador1.Width - 80);

                alAterrizar = () => ResolverCartaRobada(cartaRobada, nombreJugadorActual);
                IniciarVuelo(panel_mano_jugador1, new Point(x, 10));
            }

            timerReparto.Start();
        }

        private async void ResolverCartaRobada(Card cartaRobada, string nombreJugadorActual)
        {
            jugadores[turno].Agregar_carta(cartaRobada);
            MessageBox.Show($"{nombreJugadorActual} robó {cartaRobada} y no es jugable, se la queda.");

            numeroTurnoActual++;
            if (partidaIdActual != -1)
            {
                try
                {
                    await api.RegistrarMovimiento(partidaIdActual, numeroTurnoActual, nombreJugadorActual, "Robar", cartaRobada.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No se pudo registrar el movimiento: " + ex.Message);
                }
            }

            AvanzarTurno();
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
        private async void RepartirCartas()
        {
            timerReparto.Stop();
            comiendo = false;
            comerRestantes = 0;
            alTerminarComer = null;
            QuitarCartaVolando();
            pausa = 0;
            repartiendo = true;
            repartoRonda = 0;
            repartoJugador = 0;
            partidaTerminada = false;

            Cartas1.Clear();
            Cartas2.Clear();
            Cartas3.Clear();
            Cartas4.Clear();
            descarte.Clear();

            BarajarMazo();

            panel_mano_jugador1.Controls.Clear();
            panel_rival_arriba.Controls.Clear();
            panel_rival_izquierda.Controls.Clear();
            panel_rival_derecha.Controls.Clear();
            panelDescarte.Controls.Clear();
            MostrarBurra();

            numeroTurnoActual = 0;
            try
            {
                List<string> nombres = new List<string> { jugador1, jugador2, jugador3, jugador4 };
                partidaIdActual = await api.CrearPartida(nombres);
            }
            catch (Exception ex)
            {
                partidaIdActual = -1;
                MessageBox.Show("No se pudo registrar la partida en el servidor: " + ex.Message);
            }

            timerReparto.Start();
        }
        private void TimerReparto_Tick(object sender, EventArgs e)
        {
            if (IsDisposed) { timerReparto.Stop(); return; }

            if (pausa > 0) { pausa--; return; }

            // Carta volando: avanzarla un paso hacia su punto final
            if (cartaVolando != null)
            {
                pasoVuelo++;
                cartaVolando.Location = new Point(
    inicioVuelo.X + (destinoVuelo.X - inicioVuelo.X) * pasoVuelo / PASOS_VUELO,
    inicioVuelo.Y + (destinoVuelo.Y - inicioVuelo.Y) * pasoVuelo / PASOS_VUELO);

                if (pasoVuelo < PASOS_VUELO) return;
                Sonar("carta");
                // Aterrizó: carta robada (se queda a la vista mientras se resuelve)
                if (alAterrizar != null)
                {
                    timerReparto.Stop();
                    Action accion = alAterrizar;
                    alAterrizar = null;
                    accion();
                    if (alAterrizar == null && !esperandoColorRobada)   // CAMBIO
                    {
                        QuitarCartaVolando();
                        robando = false;
                    }
                    return;
                }

                QuitarCartaVolando();

                // Aterrizó: carta que se come por +2 / +4
                if (comiendo)
                {
                    jugadores[comerJugador].Agregar_carta(cartaEnVuelo);
                    DibujarCartaRepartida(comerJugador);
                    cartaEnVuelo = null;
                    comerRestantes--;
                    pausa = TICKS_PAUSA / 2;
                    return;
                }

                // Aterrizó: carta del reparto
                if (repartoRonda < 7)
                {
                    jugadores[repartoJugador].Agregar_carta(cartaEnVuelo);
                    DibujarCartaRepartida(repartoJugador);

                    repartoJugador++;
                    if (repartoJugador == numJugadores)
                    {
                        repartoJugador = 0;
                        repartoRonda++;
                    }
                }
                else
                {
                    descarte.Push(cartaEnVuelo);
                    colorActivo = cartaEnVuelo.Color;
                    MostrarDescarte();
                    repartoRonda++;
                }

                cartaEnVuelo = null;
                pausa = TICKS_PAUSA;
                return;
            }

            // Sin carta volando
            if (comiendo)
            {
                bool hayCartas = burra.Count > 0 || descarte.Count > 1;
                if (comerRestantes > 0 && hayCartas)
                {
                    LanzarCarta(comerJugador);
                }
                else
                {
                    timerReparto.Stop();
                    comiendo = false;
                    comerRestantes = 0;
                    Action fin = alTerminarComer;
                    alTerminarComer = null;
                    if (fin != null) fin();
                }
                return;
            }

            if (repartoRonda < 7)
            {
                LanzarCarta(repartoJugador);
            }
            else if (repartoRonda == 7)
            {
                LanzarCartaInicial();
            }
            else
            {
                timerReparto.Stop();
                repartiendo = false;
                ActualizarVista();
            }
        }
        private void ComerConAnimacion(int jugador, int cantidad, Action alTerminar)
        {
            comiendo = true;
            comerJugador = jugador;
            comerRestantes = cantidad;
            alTerminarComer = alTerminar;
            pausa = 0;
            timerReparto.Start();
        }
        private int PosicionDeJugador(int j)
        {
            // 0 abajo, 1 izquierda, 2 arriba, 3 derecha
            return (((j - turno) * sentido) % numJugadores + numJugadores) % numJugadores;
        }

        private Panel PanelDeJugador(int j)
        {
            Panel[] porPosicion = { panel_mano_jugador1, panel_rival_izquierda,
                            panel_rival_arriba, panel_rival_derecha };
            return porPosicion[PosicionDeJugador(j)];
        }

        // Misma separación que calcula MostrarManoJugador
        private int SeparacionMano(int cantidad)
        {
            int anchoCarta = 70, separacion = 75, separacionMinima = 40;
            if (cantidad <= 1) return separacion;
            int espacio = panel_mano_jugador1.Width - anchoCarta;
            int real = Math.Min(separacion, espacio / (cantidad - 1));
            return Math.Max(real, separacionMinima);
        }

        private void LanzarCarta(int j)
        {
            cartaEnVuelo = SacarCarta();
            int pos = PosicionDeJugador(j);
            Panel destino = PanelDeJugador(j);
            int n = jugadores[j].cantidad_cartas;   // la nueva carta será la número n
            Point local;

            if (pos == 0)
            {
                // Mano: boca arriba, tamaño completo, en su lugar exacto
                local = new Point(10 + n * SeparacionMano(n + 1), 10);

                CartaControl cc = new CartaControl();
                cc.Carta = cartaEnVuelo;
                cc.BocaAbajo = false;
                cartaVolando = cc;
            }
            else
            {
                // Rivales: reverso girado igual que en su abanico
                RotateFlipType giro = (pos == 1) ? RotateFlipType.Rotate90FlipNone
                                    : (pos == 2) ? RotateFlipType.Rotate180FlipNone
                                                 : RotateFlipType.Rotate270FlipNone;
                bool horizontal = (pos == 2);
                int offset = 5 + 15 * n;
                local = horizontal ? new Point(offset, 5) : new Point(5, offset);

                cartaVolando = CrearReversoGirado(giro, destino.BackColor);
            }

            IniciarVuelo(destino, local);
        }

        private void LanzarCartaInicial()
        {
            cartaEnVuelo = SacarCarta();
            while (cartaEnVuelo != null &&
            (cartaEnVuelo.Type == CardType.Mas4 || cartaEnVuelo.Type == CardType.Comodin))
            {
                DevolverYBarajar(cartaEnVuelo);
                cartaEnVuelo = SacarCarta();
            }

            CartaControl cc = new CartaControl();
            cc.Carta = cartaEnVuelo;
            cc.BocaAbajo = false;
            cartaVolando = cc;

            IniciarVuelo(panelDescarte, new Point(5, 5));   // donde la dibuja MostrarDescarte
        }

        private PictureBox CrearReversoGirado(RotateFlipType giro, Color fondo)
        {
            CartaControl cc = new CartaControl();
            cc.BocaAbajo = true;
            cc.Size = new Size(40, 60);
            cc.BackColor = fondo;

            Bitmap bmp = new Bitmap(cc.Width, cc.Height);
            cc.DrawToBitmap(bmp, new Rectangle(0, 0, cc.Width, cc.Height));
            bmp.RotateFlip(giro);
            cc.Dispose();

            PictureBox pb = new PictureBox();
            pb.Image = bmp;
            pb.Size = bmp.Size;
            pb.BackColor = Color.Transparent;
            return pb;
        }

        private void IniciarVuelo(Panel destino, Point local, Point? desde = null)
        {
            inicioVuelo = desde ?? origenVuelo;
            cartaVolando.Location = inicioVuelo;
            destinoVuelo = this.PointToClient(destino.PointToScreen(local));
            pasoVuelo = 0;

            this.Controls.Add(cartaVolando);
            cartaVolando.BringToFront();
        }

        private void QuitarCartaVolando()
        {

            esperandoColorRobada = false;   // NUEVO
            if (cartaVolando == null) return;
            alAterrizar = null;
            robando = false;
            Control c = cartaVolando;
            cartaVolando = null;

            PictureBox pb = c as PictureBox;
            Image img = null;
            if (pb != null)
            {
                img = pb.Image;
                pb.Image = null;      // el control ya no apunta a la imagen
            }

            this.Controls.Remove(c);
            c.Dispose();

            if (img != null) img.Dispose();   // ahora sí es seguro liberarla
        }
        private void DibujarCartaRepartida(int j)
        {
            // 0 abajo, 1 izquierda, 2 arriba, 3 derecha
            int pos = (((j - turno) * sentido) % numJugadores + numJugadores) % numJugadores;

            switch (pos)
            {
                case 0: MostrarManoJugador(); break;
                case 1: MostrarRival(panel_rival_izquierda, j, RotateFlipType.Rotate90FlipNone, false); break;
                case 2: MostrarRival(panel_rival_arriba, j, RotateFlipType.Rotate180FlipNone, true); break;
                case 3: MostrarRival(panel_rival_derecha, j, RotateFlipType.Rotate270FlipNone, false); break;
            }

            Label[] etiquetas = { label_jugador1, label_jugador2, label_jugador3, label_jugador4 };
            etiquetas[pos].Text = jugadores[j].Nombre + ": " + jugadores[j].cantidad_cartas + " cartas";
            labelBurra.Text = "Burra: " + CartasRestantes();
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
            Sonar("boton");
            CallUno(jugadores[turno]);
            botonUno.Visible = false;
        }

        private void ColorSeleccionado(CardColor colorElegido)
        {
            Sonar("boton");
            OcultarSelectorColor();

            if (cartaPendiente != null)
            {
                Card cartaAProcesar = cartaPendiente;
                cartaPendiente = null;

                if (esCartaRobada)
                {
                    esCartaRobada = false;
                    QuitarCartaVolando();   // quita la carta que esperaba sobre el descarte
                    jugadores[turno].Agregar_carta(cartaAProcesar);
                    JugarCarta(cartaAProcesar, colorElegido);
                }
                else
                {
                    CartaControl origen = controlPendiente;
                    controlPendiente = null;
                    JugarConAnimacion(origen, cartaAProcesar, colorElegido);
                }
            }
        }

        private void buttonSilencio_Click(object sender, EventArgs e)
        {

        }

        private void label_jugador2_Click(object sender, EventArgs e)
        {

        }

        // NUEVO: la carta robada vuela de la burra al descarte y se juega al aterrizar
        private void VolarRobadaAlDescarte(Card carta, CardColor color)
        {
            CartaControl cc = new CartaControl();
            cc.Carta = carta;
            cc.BocaAbajo = false;
            cc.EsJugable = true;
            cartaVolando = cc;

            robando = true;   // bloquea clics mientras vuela
            pausa = 0;

            alAterrizar = () =>
            {
                jugadores[turno].Agregar_carta(carta);
                JugarCarta(carta, color);
            };
            IniciarVuelo(panelDescarte, new Point(5, 5));
            timerReparto.Start();
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