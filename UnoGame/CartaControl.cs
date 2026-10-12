using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections.Generic;


//dibuja una carta de UNO en pantalla
namespace UnoGame
{
    public partial class CartaControl : UserControl
    {
        // La carta que se va a dibujar (color, número, tipo)
        private Card carta;
        // true = se muestra el reverso de la carta (boca abajo)
        private bool bocaAbajo;
        // true = la carta se puede jugar en este momento (se resalta en dorado)
        private bool esJugable;
        // true = el mouse está encima de la carta
        private bool mouseEncima;
        private static readonly Dictionary<string, Image> cache = new Dictionary<string, Image>();

        // Se ejecuta cuando se crea una carta visual nueva
        public CartaControl()
        {
            InitializeComponent();
            this.Size = new Size(70, 100);
            this.Cursor = Cursors.Hand;
            this.BackColor = Color.Transparent;
            this.DoubleBuffered = true;
            this.MouseEnter += CartaControl_MouseEnter;
            this.MouseLeave += CartaControl_MouseLeave;
        }

        // Carta que se muestra
        public Card Carta
        {
            get { return carta; }
            set { carta = value; Invalidate(); }
        }

        // Si la carta está boca abajo o no
        public bool BocaAbajo
        {
            get { return bocaAbajo; }
            set { bocaAbajo = value; Invalidate(); }
        }

        // Si la carta se puede jugar o no
        public bool EsJugable
        {
            get { return esJugable; }
            set { esJugable = value; Invalidate(); }
        }
        // Configuración especial de la ventana del control, activando la transparencia
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000020; // WS_EX_TRANSPARENT
                return cp;
            }
        }





        // eventos de raton
        private void CartaControl_MouseEnter(object sender, EventArgs e)
        {
            mouseEncima = true;
            Invalidate();
        }

        // Marca que el mouse ya no está encima y vuelve a dibujar
        private void CartaControl_MouseLeave(object sender, EventArgs e)
        {
            mouseEncima = false;
            Invalidate();
        }



        //dibujos de control
        protected override void OnPaint(PaintEventArgs e)
        {

            base.OnPaint(e); // Dibujo básico del control
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            // Área donde se dibuja la carta
            Rectangle area = new Rectangle(2, 2, this.Width - 5, this.Height - 5);
            // Borde normal: negro y delgado
            int grosorBorde = 2;
            Color colorBorde = Color.Black;
            // Si la carta se puede jugar, el borde es dorado y más grueso
            if (esJugable)
            {
                colorBorde = Color.Gold;
                grosorBorde = 3;
            }
            if (mouseEncima)
            {
                grosorBorde += 1;
                area.Inflate(-2, -2);
            }
            //libera el recurso automáticamente al terminar el bloque
            using (GraphicsPath path = RedondearRectangulo(area, 10))
            {
                if (bocaAbajo || carta == null)
                {
                    DibujarReverso(g, path);
                }
                else
                {
                    DibujarCara(g, path);
                }
                // Dibujamos el borde encima de la carta
                using (Pen pen = new Pen(colorBorde, grosorBorde))
                {
                    g.DrawPath(pen, path);
                }
            }
        }

        // Dibuja el reverso (la parte de atrás) de la carta
        private void DibujarReverso(Graphics g, GraphicsPath path)
        {
            Image imgReverso = ObtenerImagen("reverso");
            if (imgReverso != null)
            {
                DibujarImagenRecortada(g, path, imgReverso);
                return;
            }
            // 1) Fondo gris muy oscuro
            using (Brush fondo = new SolidBrush(Color.FromArgb(30, 30, 30)))
                g.FillPath(fondo, path);
            // 2) Óvalo rojo en el centro
            using (Brush ovalo = new SolidBrush(Color.Red))
            {
                Rectangle centro = new Rectangle(
                    this.Width / 2 - 15, this.Height / 2 - 22, 30, 44);
                g.FillEllipse(ovalo, centro);
            }
            // 3) Texto "UNO" en blanco, centrado
            using (Font fuente = new Font("Arial", 10, FontStyle.Bold))
            using (Brush texto = new SolidBrush(Color.White))
            {
                SizeF tam = g.MeasureString("UNO", fuente);
                g.DrawString("UNO", fuente, texto,
                    this.Width / 2 - tam.Width / 2,
                    this.Height / 2 - tam.Height / 2);
            }
        }

        // Dibuja el frente de la carta
        private void DibujarCara(Graphics g, GraphicsPath path)
        {
            Image imgCara = ObtenerImagen(ObtenerNombreImagen(carta));
            if (imgCara != null)
            {
                DibujarImagenRecortada(g, path, imgCara);
                return;
            }
            // 1) Fondo del color de la carta
            Color colorFondo = ObtenerColor(carta.Color);
            using (Brush fondo = new SolidBrush(colorFondo))
                g.FillPath(fondo, path);
            // 2) Óvalo blanco en el centro
            using (Brush fondoBlanco = new SolidBrush(Color.White))
            {
                Rectangle ovaloCentro = new Rectangle(
                    6, this.Height / 2 - 28, this.Width - 12, 56);
                using (GraphicsPath ovalo = new GraphicsPath())
                {
                    ovalo.AddEllipse(ovaloCentro);
                    g.FillPath(fondoBlanco, ovalo);
                }
            }

            //    Los comodines llevan texto negro; las demás, del color de la carta
            string textoCentral = ObtenerTextoSimbolo(carta);
            Color colorTexto = (carta.Type == CardType.Comodin || carta.Type == CardType.Mas4)
                ? Color.Black
                : colorFondo;
                
            using (Font fuenteCentro = new Font("Arial", 16, FontStyle.Bold))
            using (Brush brochaTexto = new SolidBrush(colorTexto))
            {
                SizeF tam = g.MeasureString(textoCentral, fuenteCentro);
                g.DrawString(textoCentral, fuenteCentro, brochaTexto,
                    this.Width / 2 - tam.Width / 2,
                    this.Height / 2 - tam.Height / 2);
            }
            //Número o símbolo pequeño en la esquina superior izquierda
            using (Font fuenteEsquina = new Font("Arial", 8, FontStyle.Bold))
            using (Brush brochaBlanca = new SolidBrush(Color.White))
            {
                g.DrawString(textoCentral, fuenteEsquina, brochaBlanca, 4, 4);
            }
        }





        //utilidades  de dibujo
        internal static GraphicsPath RedondearRectangulo(Rectangle bounds, int radio)
        {
            GraphicsPath path = new GraphicsPath();
            int d = radio * 2;
            // Se dibuja un arco en cada esquina, empezando por arriba a la izquierda
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90); // Esquina superior izquierda
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90); // Esquina superior derecha
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90); // Esquina inferior derecha
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90); // Esquina inferior izquierda
            path.CloseFigure();
            return path;
        }

        // Dibuja una imagen ajustada al tamaño de la carta y recortada
        internal static void DibujarImagenRecortada(Graphics g, GraphicsPath path, Image img)
        {
            RectangleF destino = path.GetBounds(); // Zona donde irá la imagen
            GraphicsState estado = g.Save();
            g.SetClip(path);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.DrawImage(img, destino);
            g.Restore(estado);
        }

        // Busca una imagen por su nombre en los Resources del proyecto
        internal static Image ObtenerImagen(string nombre)
        {
            if (nombre == null) return null;

            Image img;
            // Si la imagen aún no está en el cache, la buscamos en los Resources y la guardamos
            if (!cache.TryGetValue(nombre, out img))
            {
                img = Properties.Resources.ResourceManager.GetObject(nombre) as Image;
                cache[nombre] = img;
            }
            return img;
        }


        //datos de la carta
        // Devuelve el nombre de la imagen que corresponde a una carta
        private string ObtenerNombreImagen(Card c)
        {
            if (c.Type == CardType.Comodin) return "comodin"; // Los comodines no tienen color, así que tienen nombre propio
            if (c.Type == CardType.Mas4) return "mas4";
            // Convertimos el color a minúsculas
            string color = c.Color.ToString().ToLower();
            // Armamos el nombre según el tipo de carta
            switch (c.Type)
            {
                case CardType.Numero: return color + "_" + c.Number;
                case CardType.Salta: return color + "_salta";
                case CardType.Reversa: return color + "_reversa";
                case CardType.Mas2: return color + "_mas2";
            }
            return null;  // Si no coincide con ningún tipo, no hay imagen
        }

        // Devuelve el texto o símbolo que se dibuja en el centro de la carta
        private string ObtenerTextoSimbolo(Card c)
        {
            switch (c.Type)
            {
                case CardType.Numero: return c.Number.ToString(); // El número de la carta
                case CardType.Salta: return "⊘";
                case CardType.Reversa: return "⟲";
                case CardType.Mas2: return "+2";
                case CardType.Mas4: return "+4";
                case CardType.Comodin: return "★";
                default: return "?";
            }
        }

        // Convierte el color de la carta en un color real
        private Color ObtenerColor(CardColor color)
        {
            switch (color)
            {
                case CardColor.Rojo: return Color.Firebrick;
                case CardColor.Azul: return Color.RoyalBlue;
                case CardColor.Verde: return Color.ForestGreen;
                case CardColor.Amarillo: return Color.Goldenrod;
                default: return Color.Black;
            }
        }



        
    }
}