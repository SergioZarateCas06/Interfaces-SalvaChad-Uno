using Microsoft.VisualBasic;  // Permite usar Interaction.InputBox
using NAudio.Wave;  // Librería externa para reproducir la música MP3
using System;  // Funciones básicas de C#
using System.Collections.Generic; // Listas, pilas y diccionarios
using System.Drawing;  // Dibujo: Color, Point, Size, Bitmap, Font, etc.
using System.Drawing.Drawing2D; // Dibujo avanzado: formas redondeadas y suavizado
using System.Drawing.Imaging; // Formatos de imagen
using System.IO;  // Manejo de archivos y flujos de datos
using System.Media; //Reproduce los efectos de sonido
using System.Windows.Forms; // Ventanas y controles de Windows Forms

namespace UnoGame
{
    public partial class Form1 : Form
    {
        // Guarda los reversos de carta ya dibujados para no repetirlos
        Dictionary<RotateFlipType, Bitmap> cacheReversos = new Dictionary<RotateFlipType, Bitmap>();
        // Efectos de sonido, buscados por nombre
        Dictionary<string, SoundPlayer> sonidos = new Dictionary<string, SoundPlayer>();
        Mp3FileReader lectorMusica; // Lee el archivo MP3 de la música de fondo
        WaveOutEvent salidaMusica; // Reproduce la música por las bocinas

        System.Windows.Forms.Timer timerReparto = new System.Windows.Forms.Timer();
        Control cartaVolando;   // CartaControl o PictureBox, según el destino

        CartaControl controlPendiente;  // Carta visual que espera a que se elija un color
        Card cartaEnVuelo, cartaPendiente = null; //carta que viaja al repartir / comer
        Stack<Card> burra = new Stack<Card>(), descarte = new Stack<Card>();
        // Las manos de cada uno de los 4 jugadores
        List<Card> Cartas1 = new List<Card>(), Cartas2 = new List<Card>(), Cartas3 = new List<Card>(), Cartas4 = new List<Card>();
        List<Card>[] manosJugadores;  // Arreglo con las 4 manos juntas
        List<Player> jugadores = new List<Player>(); // Lista de los 4 jugadores
        string jugador1, jugador2, jugador3, jugador4; // Nombres de los jugadores
        bool silenciado = false, cerrando = false, repartiendo = false, robando = false, comiendo = false;  // El sonido está apagado
        bool esperandoColorRobada = false;   // la carta robada ya aterrizó y espera que se elija color
        bool esCartaRobada = false; // El comodín pendiente viene de robar
        private bool partidaTerminada = false, cierreForzado = false; // Ya hay ganador
         // Cantidad de jugadores
        int numJugadores = 4, pasoVuelo, pausa, repartoRonda = 0, repartoJugador = 0, turno = 0, sentido = 1, comerRestantes, comerJugador;
        private int partidaIdActual = -1, numeroTurnoActual = 0; 
        const int PASOS_VUELO = 10;   // más pasos = vuelo más lento
        const int TICKS_PAUSA = 5;    // pausa entre carta y carta (cada tick son ~15 ms)
        Point inicioVuelo, destinoVuelo, locRojo, locAzul, locAmarillo, locVerde;
        Point origenVuelo = new Point(390, 211);   // de aquí salen las cartas
        Size tamanoOriginalBoton;
        Action alTerminarComer, alAterrizar;   // alAterrizar: lo que se hace cuando la carta llega a su destino
        public CardColor colorActivo;
        public event Action<int> TurnoCambiado;
        public event Action<Player> PartidaTerminada;
        private readonly ApiCliente api = new ApiCliente();


        // Se ejecuta al abrir la ventana, recibe los nombres de los 4 jugadores y deja todo listo para empezar
        public Form1(String j1, String j2, String j3, String j4)
        {
            jugador1 = j1;
            jugador2 = j2;
            jugador3 = j3;
            jugador4 = j4;

            InitializeComponent(); // Crea los botones, paneles y etiquetas del diseñador
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint |
                          ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();
            CargarSonidos(); // Carga los efectos de sonido
            buttonSilencio.Click += (s, e) => AlternarSilencio();
            IniciarMusica(); // Empieza la música de fondo
            iniciarTamanoDeBotones(); // Guarda tamaño y posición de los botones de color
            iniciarColoresDeBotones(); // Pone los colores normales a los botones
            // Creamos los 4 jugadores, cada uno con su mano
            jugadores.Add(new Player(jugador1, Cartas1));
            jugadores.Add(new Player(jugador2, Cartas2));
            jugadores.Add(new Player(jugador3, Cartas3));
            jugadores.Add(new Player(jugador4, Cartas4));

            manosJugadores = new List<Card>[] { Cartas1, Cartas2, Cartas3, Cartas4 };
            TurnoCambiado += MostrarTurno; // Cuando cambie el turno, se actualiza la pantalla
            timerReparto.Interval = 15;  // El reloj late cada 15 milisegundos
            timerReparto.Tick += TimerReparto_Tick;  // Qué hacer en cada latido
            this.Shown += (s, e) => RepartirCartas();  // Al mostrarse la ventana, se reparten las cartas
        }

        // Se ejecuta cuando el usuario intenta cerrar la ventana
        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Si el cierre fue forzado por el programa, no preguntamos
            if (cierreForzado)
            {
                return;
            }
            // Si el usuario cierra la ventana, preguntamos
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult res = MessageBox.Show("Quieres salir del juego? Se perdera el progreso", "Salir del juego", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (res == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }
        // Se ejecuta cuando la ventana ya se cerró
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            cerrando = true;
            if (salidaMusica != null) salidaMusica.Dispose();
            if (lectorMusica != null) lectorMusica.Dispose();
            foreach (Bitmap b in cacheReversos.Values) b.Dispose();
            base.OnFormClosed(e);
        }
        // Carga todos los efectos de sonido desde los Resources del proyecto
        void CargarSonidos()
        {
            AgregarSonido("carta", Properties.Resources.carta);
            AgregarSonido("boton", Properties.Resources.boton);
            AgregarSonido("error", Properties.Resources.error);
            AgregarSonido("comodin", Properties.Resources.comodina);
            AgregarSonido("victoria", Properties.Resources.victoria);

        }
        // Inicia la música de fondo en bucle
        void IniciarMusica()
        {
            try
            {
                lectorMusica = new Mp3FileReader(Properties.Resources.musica);
                salidaMusica = new WaveOutEvent();
                salidaMusica.Init(lectorMusica);  // Repetir en bucle al terminar
                salidaMusica.Volume = 0.25f;   // 0.0 a 1.0

                // Repetir en bucle al terminar
                salidaMusica.PlaybackStopped += (s, e) =>
                {
                    if (cerrando) return; // Si la ventana se cierra, no repetir
                    lectorMusica.Position = 0; // Vuelve al inicio de la canción
                    salidaMusica.Play();
                };

                salidaMusica.Play();
            }
            catch (Exception ex)
            {
                // Si algo falla, avisamos pero el juego sigue
                MessageBox.Show("No se pudo iniciar la música: " + ex.Message);
            }
        }

        // Guarda un efecto de sonido en el diccionario sonidos
        void AgregarSonido(string nombre, Stream flujo)
        {
            SoundPlayer sp = new SoundPlayer(flujo);
            sp.Load(); // Lo carga en memoria para que suene rápido
            sonidos[nombre] = sp;
        }

        // Reproduce un efecto de sonido por su nombre
        void Sonar(string nombre)
        {
            if (silenciado) return;   // NUEVO
            SoundPlayer sp;
            if (sonidos.TryGetValue(nombre, out sp))
                sp.Play();
        }

        // Enciende o apaga todo el sonido

        void AlternarSilencio()
        {
            silenciado = !silenciado; // Cambia de verdadero a falso, o al revés

            if (silenciado)
            {
                if (salidaMusica != null) salidaMusica.Pause();
                foreach (SoundPlayer sp in sonidos.Values) sp.Stop(); // corta el efecto que esté sonando
            }
            else
            {
                if (salidaMusica != null) salidaMusica.Play();
            }
            // Cambia el texto del botón según el estado
            buttonSilencio.Text = silenciado ? "🔇 Sonido: OFF" : "🔊 Sonido: ON";
        }




        // Crea las 108 cartas del UNO, las baraja y las pone en la burra
        private void BarajarMazo()
        {
            List<Card> mazo = new List<Card>();
            CardColor[] colores = { CardColor.Rojo, CardColor.Azul, CardColor.Verde, CardColor.Amarillo }; 
            // Por cada color se crean sus cartas
            foreach (CardColor color in colores)
            {
                mazo.Add(new Card(0, color, CardType.Numero));  // Un solo 0 por color
                for (int i = 1; i <= 9; i++)
                {
                    mazo.Add(new Card(i, color, CardType.Numero));
                    mazo.Add(new Card(i, color, CardType.Numero));
                }
                for (int i = 0; i < 2; i++) // Dos de cada carta especial
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

            // se va intercambiando cada carta con otra posición al azar
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
            // Pasamos las cartas ya barajadas a la burra
            burra.Clear();
            foreach (Card carta in mazo)
                burra.Push(carta);
        }
        // Roba la carta de arriba de la burra
        Card SacarCarta()
        {
            if (burra.Count == 0)
                ReponerMazo();

            if (burra.Count == 0)
            {
                MostrarBurra();
                return null;
            }

            Card carta = burra.Pop();

            if (burra.Count == 0)
                AlAcabarseLaBurra();

            MostrarBurra();
            return carta;
        }
        // Rellena la burra con las cartas del descarte (menos la de arriba) ya barajadas
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
        // Regresa una carta a la burra y vuelve a barajar todo
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
        // Se ejecuta cuando la burra se queda sin cartas
        private void AlAcabarseLaBurra()
        {
            string mensaje;
            if (descarte.Count > 1)
            {
                ReponerMazo();   //BARAJEAR BURRA
                mensaje = "Se acabó la burra. Se barajó el descarte para formar una nueva.";
            }
            else
            {
                mensaje = "Se acabó la burra y no hay cartas en el descarte para reponerla.";
            }
            // BeginInvoke muestra el aviso un poco después, para no interrumpir lo que se está haciendo

            this.BeginInvoke(new Action(() => MostrarAviso(mensaje)));
        }
        // Muestra un mensaje en pantalla pausando la animación mientras tanto
        private void MostrarAviso(string mensaje)
        {
            if (IsDisposed) return;
            bool corria = timerReparto.Enabled;
            timerReparto.Stop();
            MessageBox.Show(mensaje, "Burra");
            if (corria && !IsDisposed) timerReparto.Start();

        }
        // Revisa si una carta se puede jugar sobre la carta que está en la mesa
        public bool JugadaValida(Card cartaJugada)
        {
            Card ultimaCarta = VerCartaSuperior();
            if (ultimaCarta == null) return false;
            // Los comodines se pueden jugar siempre
            if (cartaJugada.Type == CardType.Comodin || cartaJugada.Type == CardType.Mas4)
                return true;
            // Si la carta de la mesa no tiene color (comodín), se usa el color elegido
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
        // Revisa si en una mano hay al menos una carta que se pueda jugar
        public bool TieneJugadaValida(List<Card> mano)
        {
            foreach (Card carta in mano)
            {
                if (JugadaValida(carta))
                    return true;
            }
            return false;
        }
        // Devuelve la carta de arriba del descarte sin quitarla

        public Card VerCartaSuperior()
        {
            if (descarte.Count > 0)
            {
                return descarte.Peek();
            }
            return null;
        }
        // Devuelve cuántas cartas quedan en la burra
        int CartasRestantes()
        {
            return burra.Count;
        }
        void AvanzarTurno()
        {
            turno = (turno + sentido + numJugadores) % numJugadores;
            if (TurnoCambiado != null)
            {
                TurnoCambiado.Invoke(turno);
            }
        }
        // El jugador canta "UNO"
        void CallUno(Player jugador)
        {
            jugador.callUno();
        }
        // Cambia el sentido del juego
        void InvertirSentido()
        {
            sentido = -sentido;
        }
        // Juega una carta: la valida, la pasa al descarte, registra el movimiento
        // revisa si ganó o si olvidó decir UNO, y aplica el efecto de la carta
        private async void JugarCarta(Card cartaJugada, CardColor colorElegido = CardColor.Ninguno)
        {
            if (cartaPendiente != null) return; // Si hay un comodín esperando color, no hacemos nada
            if (partidaTerminada)
            {
                MessageBox.Show("La partida ya terminó.");
                return;
            }

            if (!JugadaValida(cartaJugada))  // Si la jugada no es válida, suena un error y se detiene.
            {
                Sonar("error");
                MessageBox.Show("La jugada no es valida");
                return;
            }

            // La carta pasa de la mano del jugador al descarte
            descarte.Push(cartaJugada);
            jugadores[turno].Quitar_carta(cartaJugada);
            // Guardamos el movimiento en el servidor
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
            // Si se quedó sin cartas, ganó
            if (jugadores[turno].cantidad_cartas == 0)
            {
                TerminarPartida(jugadores[turno]);
                return;
            }
            // Si le queda 1 carta, debió decir UNO. Si no lo dijo, come 2 cartas de castigo
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
            // Actualiza el color activo: si es comodín, el color que se eligió

            if (cartaJugada.Type == CardType.Comodin || cartaJugada.Type == CardType.Mas4)
                colorActivo = colorElegido;
            else
                colorActivo = cartaJugada.Color;
            // Cambia el sentido
            if (cartaJugada.Type == CardType.Reversa)
                InvertirSentido();
            // Salta al siguiente jugador pierde su turno
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
                // Anima las cartas que come y, al terminar, avanza dos turnos.
                ComerConAnimacion(siguienteJugador, cantidad, () =>
                {
                    MessageBox.Show($"{jugadores[siguienteJugador].Nombre} come {cantidad} cartas.");
                    AvanzarTurno();   // el que comió pierde su turno
                    AvanzarTurno();
                });
                return;
            }

            AvanzarTurno();  // Si no fue ninguna carta especial, solo pasa el turno

        }
        // muestra la mano y pide escribir el número de la carta a jugar
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
            // Se repite hasta que el jugador haga una jugada correcta
            while (true)
            {
                // Armamos el texto con la lista de cartas numeradas
                string mano = "";
                for (int i = 0; i < manoActual.Count; i++)
                    mano += $"{i}: {manoActual[i]}\n";
                // Mostramos el cuadro para escribir el número de la carta
                string entrada = Interaction.InputBox(
                    $"Turno de {nombreJugadorActual}\nCarta en mesa: {VerCartaSuperior()} (color activo: {colorActivo})\n\n{mano}\n" +
                    "Escribe el número de la carta que quieres jugar, o -1 si no tienes ninguna jugable:",
                    "Jugar carta", "-1");
                // Si lo escrito no es un número, volvemos a preguntar
                int indice;
                if (!int.TryParse(entrada, out indice))
                {
                    MessageBox.Show("Entrada no válida, intenta de nuevo.");
                    continue;
                }
                // -1 = robar carta
                if (indice == -1)
                {
                    TomarCartaYIntentarJugar(nombreJugadorActual);
                    return;
                }
                // Número fuera de la mano
                if (indice < 0 || indice >= manoActual.Count)
                {
                    MessageBox.Show("Ese número no corresponde a ninguna carta de tu mano. Intenta de nuevo.");
                    continue;
                }

                Card cartaElegida = manoActual[indice];

                if (JugadaValida(cartaElegida))
                {
                    CardColor colorElegido = CardColor.Ninguno;
                    if (cartaElegida.Type == CardType.Comodin || cartaElegida.Type == CardType.Mas4) // Si es comodín, mostramos los botones para elegir el color y esperamos
                    {
                        cartaPendiente = cartaElegida;
                        esCartaRobada = false;
                        SeleccionColor_Rojo.Visible = true;
                        SeleccionColor_Azul.Visible = true;
                        SeleccionColor_Amarillo.Visible = true;
                        SeleccionColor_Verde.Visible = true;
                        return;
                    }

                    // Se ejecuta cuando el jugador hace clic en una carta de su mano
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
        // Recibe la carta elegida y su control visual
        private void IntentarJugarCarta(Card cartaElegida, CartaControl origen = null)
        {
            // Si hay una animación o un comodín pendiente, ignoramos el clic.
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
            //mostramos los 4 botones de color y esperamos la elección
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

        // Termina la partida, suena la victoria, guarda el ganador en el servidor
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
            // Ventana que muestra quién ganó
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
            // Avisamos a quien esté escuchando que la partida terminó
            if (PartidaTerminada != null)
                PartidaTerminada.Invoke(ganador);

            ActualizarVista();
        }
        // Reinicia el juego, turno y sentido como al inicio, y reparte otra vez
        private void ReiniciarPartida()
        {
            turno = 0;
            sentido = 1;
            RepartirCartas();
        }

        // Roba una carta de la burra para el jugador en turno
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
            // Si ya no hay cartas, se pasa el turno
            if (cartaRobada == null)
            {
                Sonar("error");
                MessageBox.Show("No quedan cartas para robar.");
                AvanzarTurno();
                return;
            }

            robando = true;
            // Creamos la carta visual que va a volar
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
                // Vuela al descarte y, al llegar, pide el color; la carta se queda a la vista
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
                alAterrizar = () =>  // La carta vuela al descarte y al llegar se juega automáticamente
                {
                    jugadores[turno].Agregar_carta(cartaRobada);
                    JugarCarta(cartaRobada, CardColor.Ninguno);
                };
                IniciarVuelo(panelDescarte, new Point(5, 5));
            }
            else
            {
                // No se puede jugar, vuela a la mano del jugador y se la queda
                Sonar("error");
                int n = jugadores[turno].cantidad_cartas;
                int x = 10 + n * SeparacionMano(n + 1);
                x = Math.Min(x, panel_mano_jugador1.Width - 80);

                alAterrizar = () => ResolverCartaRobada(cartaRobada, nombreJugadorActual);
                IniciarVuelo(panel_mano_jugador1, new Point(x, 10));
            }

            timerReparto.Start();  // Arranca la animación
        }
        // Se ejecuta cuando una carta robada llega a la mano
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




        // Vuelve a dibujar toda la mesa con el estado actual del juego
        void ActualizarVista()
        {
            SuspendLayout();
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
            ResumeLayout();
        }
        // Se ejecuta cuando cambia el turno: solo actualiza la pantalla
        void MostrarTurno(int t)
        {
            ActualizarVista();
        }
        // Dibuja las cartas del jugador en turno, una al lado de otra
        private void MostrarManoJugador()
        {
            panel_mano_jugador1.Controls.Clear();
            List<Card> mano = manosJugadores[turno]; // Borra lo que había dibujado

            int anchoCarta = 70;  // Ancho de cada carta
            int separacion = 75;  // Distancia ideal entre cartas
            int separacionMinima = 40; // Distancia mínima

            int espacioDisponible = panel_mano_jugador1.Width - anchoCarta;
            int separacionReal;  // La distancia que realmente se usar

            if (mano.Count > 1)
            {
                separacionReal = Math.Min(separacion, espacioDisponible / (mano.Count - 1));
                if (separacionReal < separacionMinima)
                {
                    separacionReal = separacionMinima;
                }
            }
            else
            {
                separacionReal = separacion;
            }
            // Si no caben, se activa el desplazamiento
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
            // Creamos una carta visual por cada carta de la mano
            int x = 10;
            foreach (Card carta in mano)
            {
                CartaControl cc = new CartaControl();
                cc.Carta = carta;
                cc.BocaAbajo = false;
                cc.EsJugable = JugadaValida(carta); // Se resalta si se puede jugar
                cc.Location = new Point(x, 10);
                // Guardamos copias para que el clic use la carta correcta
                Card cartaCapturada = carta;
                CartaControl ccCapturado = cc;
                cc.Click += (s, e) => IntentarJugarCarta(cartaCapturada, ccCapturado);

                panel_mano_jugador1.Controls.Add(cc);
                x += separacionReal;
            }
        }
        // Dibuja las cartas de un rival boca abajo, en forma de abanico
        private void MostrarRival(Panel panel, int indiceJugador, RotateFlipType giro, bool horizontal)
        {

            panel.Controls.Clear();
            int cantidad = jugadores[indiceJugador].cantidad_cartas;

            int pos = 5;  // Posición de la primera carta
            int anchoCartaMini = 40;  // Ancho de la carta pequeña
            int separacionIdeal = 15; // Distancia ideal entre carta
            int separacionMinima = 10;  // Distancia mínima entre cartas

            int espacioDisponible = (horizontal ? panel.Width : panel.Height) - anchoCartaMini;
            int separacionReal;
            // Si hay muchas cartas, se acercan para que quepan
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
            // Una imagen de reverso por cada carta del rival
            for (int i = 0; i < cantidad; i++)
            {
                Bitmap bmp = ObtenerReverso(giro);

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
        // Dibuja la carta de arriba del descarte
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

        // Dibuja la burra boca abajo
        // Al hacer clic, el jugador roba una carta
        private void MostrarBurra()
        {
            panelBurra.Controls.Clear();

            if (partidaTerminada) return;  // Si ya terminó la partida, no se dibuja

            CartaControl cc = new CartaControl();
            cc.BocaAbajo = true;
            cc.Location = new Point(5, 5);
            cc.Cursor = Cursors.Hand;
            cc.Click += (s, e) => TomarCartaYIntentarJugar(jugadores[turno].Nombre);
            panelBurra.Controls.Add(cc);
        }
        // Muestra el color activo en un cuadro de color y en una etiqueta de texto
        private void MostrarColorActivo()
        {
            Color c;
            switch (colorActivo)
            {
                case CardColor.Rojo: c = Color.Red; break;
                case CardColor.Azul: c = Color.RoyalBlue; break;
                case CardColor.Verde: c = Color.Green; break;
                case CardColor.Amarillo: c = Color.Gold; break;
                default: c = Color.Gray; break;  // Sin color definido
            }
            panelColor.BackColor = c;
            labelColor.Text = "Color actual: " + colorActivo;
        }
        // Muestra en una etiqueta hacia dónde va el sentido del juego
        private void MostrarSentido()
        {
            if (sentido == 1)
                labelSentido.Text = "Sentido: Izquierda";
            else
                labelSentido.Text = "Sentido: Derecha";
        }
        // Muestra el botón "UNO" solo cuando el jugador en turno
        private void ActualizarBotonUno()
        {
            bool debeAparecer =
                !partidaTerminada &&
                jugadores[turno].cantidad_cartas == 2 &&
                !jugadores[turno].DijoUno &&
                TieneJugadaValida(manosJugadores[turno]);

            botonUno.Visible = debeAparecer;
        }
        // Vuelve a acomodar las cartas de un panel con una separación dada
        private void ReacomodarPanel(Panel panel, bool horizontal, int inicio, int fijo, int separacion)
        {
            for (int i = 0; i < panel.Controls.Count; i++)
            {
                panel.Controls[i].Location = horizontal
                    ? new Point(inicio + i * separacion, fijo)
                    : new Point(fijo, inicio + i * separacion);
            }
        }
        // Agrega UNA carta a la mano del jugador

        private void AgregarCartaMano(Card carta)
        {
            CartaControl cc = new CartaControl();
            cc.Carta = carta;
            cc.BocaAbajo = false;
            cc.EsJugable = JugadaValida(carta);

            Card cartaCapturada = carta;
            CartaControl ccCapturado = cc;
            cc.Click += (s, e) => IntentarJugarCarta(cartaCapturada, ccCapturado);

            panel_mano_jugador1.SuspendLayout();
            panel_mano_jugador1.Controls.Add(cc);
            // Reacomoda todas las cartas con la nueva separación
            int cantidad = panel_mano_jugador1.Controls.Count;
            int sep = SeparacionMano(cantidad);
            ReacomodarPanel(panel_mano_jugador1, true, 10, 10, sep);
            // Si ya no caben, activa el desplazamiento
            int anchoTotal = 70 + (cantidad - 1) * sep;
            if (anchoTotal > panel_mano_jugador1.Width)
            {
                panel_mano_jugador1.AutoScroll = true;
                panel_mano_jugador1.AutoScrollMinSize = new Size(anchoTotal, 0);
            }
            panel_mano_jugador1.ResumeLayout();
        }
        // Agrega una carta boca abajo al abanico de un rival
        private void AgregarReverso(Panel panel, RotateFlipType giro, bool horizontal)
        {
            Bitmap bmp = ObtenerReverso(giro);
            PictureBox pb = new PictureBox();
            pb.Image = bmp;
            pb.Size = bmp.Size;
            pb.BackColor = Color.Transparent;

            int cantidad = panel.Controls.Count + 1;
            int espacio = (horizontal ? panel.Width : panel.Height) - 40;
            int sep = cantidad > 1 ? Math.Max(10, Math.Min(15, espacio / (cantidad - 1))) : 15;

            panel.SuspendLayout();
            panel.Controls.Add(pb);
            ReacomodarPanel(panel, horizontal, 5, 5, sep);
            panel.ResumeLayout();
        }
        // Dibuja en la mesa la carta que se acaba de repartir al jugador 
        private void DibujarCartaRepartida(int j, Card carta)
        {
            int pos = (((j - turno) * sentido) % numJugadores + numJugadores) % numJugadores;

            switch (pos)
            {
                case 0: AgregarCartaMano(carta); break;
                case 1: AgregarReverso(panel_rival_izquierda, RotateFlipType.Rotate90FlipNone, false); break;
                case 2: AgregarReverso(panel_rival_arriba, RotateFlipType.Rotate180FlipNone, true); break;
                case 3: AgregarReverso(panel_rival_derecha, RotateFlipType.Rotate270FlipNone, false); break;
            }
            // Actualiza los contadores de cartas y de la burra
            Label[] etiquetas = { label_jugador1, label_jugador2, label_jugador3, label_jugador4 };
            etiquetas[pos].Text = jugadores[j].Nombre + ": " + jugadores[j].cantidad_cartas + " cartas";
            labelBurra.Text = "Burra: " + CartasRestantes();
        }
        // Devuelve la imagen del reverso de una carta (pequeña) con el giro indicado
        Bitmap ObtenerReverso(RotateFlipType giro)
        {
            Bitmap bmp;
            if (cacheReversos.TryGetValue(giro, out bmp)) return bmp; // Ya existe: la reutilizamos

            bmp = new Bitmap(40, 60, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle area = new Rectangle(1, 1, bmp.Width - 3, bmp.Height - 3);
                using (GraphicsPath path = CartaControl.RedondearRectangulo(area, 8))
                {
                    // Usa la imagen reverso y le dibuja un borde negro
                    Image imgReverso = CartaControl.ObtenerImagen("reverso");  // Gira la imagen (90°, 180° o 270°)
                    if (imgReverso != null)
                        CartaControl.DibujarImagenRecortada(g, path, imgReverso);
                    using (Pen pen = new Pen(Color.Black, 2))
                        g.DrawPath(pen, path);
                }
            }
            bmp.RotateFlip(giro);
            cacheReversos[giro] = bmp; // La guardamos para la próxima vez
            return bmp;
        }
        // Crea un PictureBox con el reverso de una carta ya girado
        private PictureBox CrearReversoGirado(RotateFlipType giro, Color fondo)
        {
            CartaControl cc = new CartaControl();
            cc.BocaAbajo = true;
            cc.Size = new Size(40, 60);
            cc.BackColor = fondo;
            // Convertimos la carta visual en una imagen y la giramos
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






        //animacion
        // Prepara y arranca una partida nueva: limpia todo, baraja, crea la partida en el servidor y empieza a repartir
        private async void RepartirCartas()
        {
            // Reinicia las banderas y detiene cualquier animación anterior
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
            // Vacía las manos y el descarte
            Cartas1.Clear();
            Cartas2.Clear();
            Cartas3.Clear();
            Cartas4.Clear();
            descarte.Clear();

            BarajarMazo();
            // Limpia todos los paneles de la mesa
            panel_mano_jugador1.Controls.Clear();
            panel_rival_arriba.Controls.Clear();
            panel_rival_izquierda.Controls.Clear();
            panel_rival_derecha.Controls.Clear();
            panelDescarte.Controls.Clear();
            MostrarBurra();
            // Crea la partida en el servidor y guarda su id
            numeroTurnoActual = 0;
            try
            {
                List<string> nombres = new List<string> { jugador1, jugador2, jugador3, jugador4 };
                partidaIdActual = await api.CrearPartida(nombres);
            }
            catch (Exception ex)
            {
                // Si falla, el juego continúa sin guardar datos
                partidaIdActual = -1;
                MessageBox.Show("No se pudo registrar la partida en el servidor: " + ex.Message);
            }

            timerReparto.Start(); // Empieza la animación del reparto
        }
        // Se ejecuta en cada latido del reloj
        private void TimerReparto_Tick(object sender, EventArgs e)
        {
            if (IsDisposed) { timerReparto.Stop(); return; }
            // Si hay una pausa pendiente, se espera un latido más
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
                    accion();  // Ejecuta lo que había que hacer al llega
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
                    DibujarCartaRepartida(comerJugador, cartaEnVuelo);
                    cartaEnVuelo = null;
                    comerRestantes--;
                    pausa = TICKS_PAUSA / 2;
                    return;
                }

                // Aterrizó: carta del reparto
                if (repartoRonda < 7)
                {
                    jugadores[repartoJugador].Agregar_carta(cartaEnVuelo);
                    DibujarCartaRepartida(repartoJugador, cartaEnVuelo);

                    repartoJugador++;
                    if (repartoJugador == numJugadores)
                    {
                        repartoJugador = 0;
                        repartoRonda++;
                    }
                }
                else
                {
                    // Última carta:la primera del descarte (da inicio al juego)
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
                // Mientras falten cartas por comer y haya cartas disponibles, lanza otra
                bool hayCartas = burra.Count > 0 || descarte.Count > 1;
                if (comerRestantes > 0 && hayCartas)
                {
                    LanzarCarta(comerJugador); // Sigue el reparto
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
                LanzarCartaInicial();  // Pone la primera carta en la mesa
            }
            else
            {
                timerReparto.Stop();  // Reparto terminado: se detiene el reloj y se dibuja la mesa completa
                repartiendo = false;
                ActualizarVista();
            }
        }
        // Hace que un jugador "coma" (robe) varias cartas con animación (por +2 / +4)
        private void ComerConAnimacion(int jugador, int cantidad, Action alTerminar)
        {
            comiendo = true;
            comerJugador = jugador;
            comerRestantes = cantidad;
            alTerminarComer = alTerminar;
            pausa = 0;
            timerReparto.Start();
        }

        // Saca una carta de la burra y la hace volar hacia el otro jugador
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
        // Saca la primera carta de la mesa y la hace volar al descarte
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
        //la carta vuela desde la mano hasta el descarte
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
        // Prepara el vuelo de la carta "cartaVolando" hacia un panel
        private void IniciarVuelo(Panel destino, Point local, Point? desde = null)
        {
            inicioVuelo = desde ?? origenVuelo;
            cartaVolando.Location = inicioVuelo;
            destinoVuelo = this.PointToClient(destino.PointToScreen(local));
            pasoVuelo = 0; // Convierte a coordenadas de la ventana

            this.Controls.Add(cartaVolando); // La carta se pone sobre la ventana para poder moverse librement
            cartaVolando.BringToFront(); // Se dibuja por encima de todo
        }

        // Quita de la pantalla la carta que estaba volando y libera su memoria.
        private void QuitarCartaVolando()
        {

            esperandoColorRobada = false;   // NUEVO
            if (cartaVolando == null) return;
            alAterrizar = null;
            robando = false;
            Control c = cartaVolando;
            cartaVolando = null;
            // Si es un PictureBox, soltamos su imagen antes de destruirlo
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


        //colores
        void iniciarTamanoDeBotones()
        {
            tamanoOriginalBoton = SeleccionColor_Rojo.Size;
            locRojo = SeleccionColor_Rojo.Location;
            locAzul = SeleccionColor_Azul.Location;
            locAmarillo = SeleccionColor_Amarillo.Location;
            locVerde = SeleccionColor_Verde.Location;

        }
        // Pone a los botones de color su tono normal
        void iniciarColoresDeBotones()
        {

            SeleccionColor_Rojo.BackColor = Color.FromArgb(180, 40, 40);
            SeleccionColor_Azul.BackColor = Color.FromArgb(0, 90, 180);
            SeleccionColor_Amarillo.BackColor = Color.FromArgb(210, 160, 20);
            SeleccionColor_Verde.BackColor = Color.FromArgb(30, 140, 60);
        }
        // Esconde los botones de color y los deja como estaban (tamaño, lugar y color)
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
        // Se ejecuta cuando el jugador elige un color para su comodín
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
                    // El comodín venía de robar: se agrega a la mano y se juega
                    esCartaRobada = false;
                    QuitarCartaVolando();   // quita la carta que esperaba sobre el descarte
                    jugadores[turno].Agregar_carta(cartaAProcesar);
                    JugarCarta(cartaAProcesar, colorElegido);
                }
                else
                {
                    // El comodín venía de la mano: se juega con animación
                    CartaControl origen = controlPendiente;
                    controlPendiente = null;
                    JugarConAnimacion(origen, cartaAProcesar, colorElegido);
                }
            }
        }

        // Al pasar el mouse: se aclara y se agranda un poco

        private void SeleccionColor_Rojo_MouseEnter(object sender, EventArgs e)
        {
            SeleccionColor_Rojo.BackColor = Color.Red;
            SeleccionColor_Rojo.Size = new Size(tamanoOriginalBoton.Width + 12, tamanoOriginalBoton.Height + 12);
            SeleccionColor_Rojo.Location = new Point(locRojo.X - 6, locRojo.Y - 6);
        }
        // Al salir el mouse: vuelve a su color y tamaño normal
        private void SeleccionColor_Rojo_MouseLeave(object sender, EventArgs e)
        {
            SeleccionColor_Rojo.BackColor = Color.FromArgb(180, 40, 40);
            SeleccionColor_Rojo.Size = tamanoOriginalBoton;
            SeleccionColor_Rojo.Location = locRojo;
        }
        // Al hacer clic: elige el color rojo
        private void SeleccionColor_Rojo_Click(object sender, EventArgs e)
        {
            ColorSeleccionado(CardColor.Rojo);
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
        private void SeleccionColor_Amarillo_Click(object sender, EventArgs e)
        {
            ColorSeleccionado(CardColor.Amarillo);
        }
        //Botón AZUL (mismo comportamiento que el rojo)
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
        private void SeleccionColor_Azul_Click(object sender, EventArgs e)
        {
            ColorSeleccionado(CardColor.Azul);
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
        private void SeleccionColor_Verde_Click(object sender, EventArgs e)
        {
            ColorSeleccionado(CardColor.Verde);
        }


        //eventos
        private void Form1_Click(object sender, EventArgs e)
        {

        }
        
        private void button3_Click(object sender, EventArgs e)
        {
            Sonar("boton");
            CallUno(jugadores[turno]);
            botonUno.Visible = false;
        }

        // Clic en el botón de prueba: juega un turno con cuadros de texto
        private void button2_Click(object sender, EventArgs e)
        {
            JugarTurno();
        }
    }

    // jugador
    public class Player
    {
        public string Nombre; // Nombre del jugado
        private List<Card> mano;  // Cartas que tiene en la mano
        public bool DijoUno;  // true = ya dijo "UNO"
         // El jugador dice "UNO". Solo cuenta si tiene exactamente 2 cartas
        public void callUno()
        {
            if (mano.Count == 2)
            {
                DijoUno = true;
            }
        }
        //Crea al jugador con su nombre y su lista de cartas
        public Player(string nombre, List<Card> mano)
        {
            this.Nombre = nombre;
            this.mano = mano;
            this.DijoUno = false;
        }
        // Propiedad que devuelve cuántas cartas tiene el jugador
        public int cantidad_cartas
        {
            get { return mano.Count; }
        }
        // Agrega una carta a la mano. Al recibir una carta, se borra el "UNO" dicho
        public void Agregar_carta(Card carta)
        {
            if (carta != null)
            {


                mano.Add(carta);
                DijoUno = false;
            }
        }
        // Quita una carta de la mano. Devuelve true si la carta estaba y se quitó
        public bool Quitar_carta(Card carta)
        {
            return mano.Remove(carta);
        }
    }

    // Un Panel normal, pero con doble búfer, se dibuja primero en memoria y luego en pantalla, para evitar parpadeos
    public class PanelBuffered : Panel
    {
        public PanelBuffered()
        {
            this.DoubleBuffered = true;
            this.ResizeRedraw = true;
        }
    }
}