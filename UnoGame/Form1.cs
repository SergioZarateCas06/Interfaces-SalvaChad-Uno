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
        bool silenciado = false, cerrando = false, robando = false;  // El sonido está apagado
        bool esperandoColorRobada = false;   // la carta robada ya aterrizó y espera que se elija color
        private bool partidaTerminada = false, cierreForzado = false; // Ya hay ganador
        int pasoVuelo;
        const int PASOS_VUELO = 10;   // más pasos = vuelo más lento
        Point inicioVuelo, destinoVuelo, locRojo, locAzul, locAmarillo, locVerde;
        Point origenVuelo = new Point(390, 211);   // de aquí salen las cartas
        Size tamanoOriginalBoton;
        Action alAterrizar;

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

        // Dibuja las cartas del jugador en turno, una al lado de otra
        private void MostrarManoJugador(List<Card> mano, Func<Card, bool> esJugable)
        {
            panel_mano_jugador1.Controls.Clear();



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
                cc.EsJugable = esJugable(carta);   // Se resalta si se puede jugar
                cc.Location = new Point(x, 10);
                Card cartaCapturada = carta;
                cc.Click += (s, e) => ClicCartaRed(cartaCapturada);

                panel_mano_jugador1.Controls.Add(cc);
                x += separacionReal;
            }
        }
    
        // Dibuja las cartas de un rival boca abajo, en forma de abanico
        private void MostrarRival(Panel panel, int cantidad, RotateFlipType giro, bool horizontal)
        {

            panel.Controls.Clear();

          

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
        private void MostrarDescarte(Card sup)
        {
            panelDescarte.Controls.Clear();
            
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
            cc.Click += (s, e) => ClicBurraRed();
            panelBurra.Controls.Add(cc);
        }
        // Muestra el color activo en un cuadro de color y en una etiqueta de texto
        private void MostrarColorActivo(CardColor colorActivo)
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
        private void MostrarSentido(int sentido)
        {
            if (sentido == 1)
                labelSentido.Text = "Sentido: Izquierda";
            else
                labelSentido.Text = "Sentido: Derecha";
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
            ColorSeleccionadoRed(colorElegido);
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
            cliente.DecirUno();
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