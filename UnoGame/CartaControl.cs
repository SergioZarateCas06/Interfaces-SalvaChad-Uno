using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections.Generic;

namespace UnoGame
{
    public partial class CartaControl : UserControl
    {
        private Card carta;
        private bool bocaAbajo;
        private bool esJugable;
        private bool mouseEncima;


        private static readonly Dictionary<string, Image> cache = new Dictionary<string, Image>();

        private static Image ObtenerImagen(string nombre)
        {
            if (nombre == null) return null;

            Image img;
            if (!cache.TryGetValue(nombre, out img))
            {
                img = Properties.Resources.ResourceManager.GetObject(nombre) as Image;
                cache[nombre] = img;
            }
            return img;
        }

        private string ObtenerNombreImagen(Card c)
        {
            if (c.Type == CardType.Comodin) return "comodin";
            if (c.Type == CardType.Mas4) return "mas4";

            string color = c.Color.ToString().ToLower();

            switch (c.Type)
            {
                case CardType.Numero: return color + "_" + c.Number;
                case CardType.Salta: return color + "_salta";
                case CardType.Reversa: return color + "_reversa";
                case CardType.Mas2: return color + "_mas2";
            }
            return null;
        }
        private void DibujarImagenRecortada(Graphics g, GraphicsPath path, Image img)
        {
            RectangleF destino = path.GetBounds();
            GraphicsState estado = g.Save();
            g.SetClip(path);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.DrawImage(img, destino);
            g.Restore(estado);
        }

        public CartaControl()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.Size = new Size(70, 100);
            this.Cursor = Cursors.Hand;
            
            this.MouseEnter += CartaControl_MouseEnter;
            this.MouseLeave += CartaControl_MouseLeave;
        }

        // La carta que representa este control. Al asignarla se redibuja solo.
        public Card Carta
        {
            get { return carta; }
            set { carta = value; Invalidate(); }
        }

        // Si es true, se dibuja el reverso en vez de la cara de la carta.
        public bool BocaAbajo
        {
            get { return bocaAbajo; }
            set { bocaAbajo = value; Invalidate(); }
        }

        // Si es true, se resalta visualmente (carta jugable en el turno actual).
        public bool EsJugable
        {
            get { return esJugable; }
            set { esJugable = value; Invalidate(); }
        }

        private void CartaControl_MouseEnter(object sender, EventArgs e)
        {
            mouseEncima = true;
            Invalidate();
        }

        private void CartaControl_MouseLeave(object sender, EventArgs e)
        {
            mouseEncima = false;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle area = new Rectangle(2, 2, this.Width - 5, this.Height - 5);

            // Efecto al pasar el mouse o resaltar carta jugable: borde más grueso y de color
            int grosorBorde = 2;
            Color colorBorde = Color.Black;

            if (esJugable)
            {
                colorBorde = Color.Gold;
                grosorBorde = 3;
            }
            if (mouseEncima)
            {
                grosorBorde += 1;
                area.Inflate(-2, -2); // simula que la carta "crece" un poco
            }

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

                using (Pen pen = new Pen(colorBorde, grosorBorde))
                {
                    g.DrawPath(pen, path);
                }
            }
        }

        private void DibujarReverso(Graphics g, GraphicsPath path)
        {

            Image imgReverso = ObtenerImagen("reverso");
            if (imgReverso != null)
            {
                DibujarImagenRecortada(g, path, imgReverso);
                return;
            }

            using (Brush fondo = new SolidBrush(Color.FromArgb(30, 30, 30)))
                g.FillPath(fondo, path);

            using (Brush ovalo = new SolidBrush(Color.Red))
            {
                Rectangle centro = new Rectangle(
                    this.Width / 2 - 15, this.Height / 2 - 22, 30, 44);
                g.FillEllipse(ovalo, centro);
            }

            using (Font fuente = new Font("Arial", 10, FontStyle.Bold))
            using (Brush texto = new SolidBrush(Color.White))
            {
                SizeF tam = g.MeasureString("UNO", fuente);
                g.DrawString("UNO", fuente, texto,
                    this.Width / 2 - tam.Width / 2,
                    this.Height / 2 - tam.Height / 2);
            }
        }

        private void DibujarCara(Graphics g, GraphicsPath path)
        {

            Image imgCara = ObtenerImagen(ObtenerNombreImagen(carta));
            if (imgCara != null)
            {
                DibujarImagenRecortada(g, path, imgCara);
                return;
            }

            Color colorFondo = ObtenerColor(carta.Color);
            using (Brush fondo = new SolidBrush(colorFondo))
                g.FillPath(fondo, path);

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

            using (Font fuenteEsquina = new Font("Arial", 8, FontStyle.Bold))
            using (Brush brochaBlanca = new SolidBrush(Color.White))
            {
                g.DrawString(textoCentral, fuenteEsquina, brochaBlanca, 4, 4);
            }
        }

        private string ObtenerTextoSimbolo(Card c)
        {
            switch (c.Type)
            {
                case CardType.Numero: return c.Number.ToString();
                case CardType.Salta: return "⊘";
                case CardType.Reversa: return "⟲";
                case CardType.Mas2: return "+2";
                case CardType.Mas4: return "+4";
                case CardType.Comodin: return "★";
                default: return "?";
            }
        }

        private Color ObtenerColor(CardColor color)
        {
            switch (color)
            {
                case CardColor.Rojo: return Color.Firebrick;
                case CardColor.Azul: return Color.RoyalBlue;
                case CardColor.Verde: return Color.ForestGreen;
                case CardColor.Amarillo: return Color.Goldenrod;
                default: return Color.Black; // Comodines sin color aún elegido
            }
        }

        private GraphicsPath RedondearRectangulo(Rectangle bounds, int radio)
        {
            GraphicsPath path = new GraphicsPath();
            int d = radio * 2;
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}