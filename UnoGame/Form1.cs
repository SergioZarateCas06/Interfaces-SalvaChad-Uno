using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace UnoGame
{
    public partial class Form1 : Form
    {
        int numJugadores = 4;
        Card ultimaCarta;
        int turno = 1;

        Stack<Card> burra = new Stack<Card>();
        List<Card> Cartas1 = new List<Card>();
        List<Card> Cartas2 = new List<Card>();
        List<Card> Cartas3 = new List<Card>();
        List<Card> Cartas4 = new List<Card>();
       List<Player> jugadores = new List<Player>();

        public Form1()
        {
            InitializeComponent();
            jugadores.Add(new Player("Jugador 1", Cartas1));
            jugadores.Add(new Player("Jugador 2", Cartas2));
            jugadores.Add(new Player("Jugador 3", Cartas3));
            jugadores.Add(new Player("Jugador 4", Cartas4));
        }

        Card SacarCarta()
        {
            if (burra.Count > 0)
            {
                return burra.Pop();
            }
            return null;
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
                mazo.Add(new Card(-1, CardColor.Ninguno, CardType.ComodinMas4));
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

        private void button1_Click(object sender, EventArgs e)
        {
            BarajarMazo();
            richTextBox1.Text = string.Join(", ", burra.Select(c => c.ToString()));
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }
    }

    public enum CardColor { Rojo, Azul, Verde, Amarillo, Ninguno };
    public enum CardType { Numero, Salta, Reversa, Mas2, Comodin, ComodinMas4 };

    public class Card
    {
        public int Number;
        public CardColor Color;
        public CardType Type;

        public Card(int cardNumber, CardColor cardColor, CardType cardType)
        {
            this.Number = cardNumber;
            this.Color = cardColor;
            this.Type = cardType;
        }

        public override string ToString()
        {
            switch (this.Type)
            {
                case CardType.Numero:
                    return $"{Color} {Number}";
                case CardType.Salta:
                    return $"{Color} Salta";
                case CardType.Reversa:
                    return $"{Color} Reversa";
                case CardType.Mas2:
                    return $"{Color} +2";
                case CardType.ComodinMas4:
                    return "Comodin +4";
                case CardType.Comodin:
                    return "Comodin";
                default:
                    return "No existe la carta";
            }
        }
    }
    public class Player
    {
        public string Nombre;
        private List<Card> mano;

        public Player(string nombre, List<Card>mano)
        {
            this.Nombre = nombre;
            this.mano=mano;
        }
        public int cantidad_cartas
        {
            get {  return mano.Count; }
        }
        public void Agregar_carta(Card carta)
        {
            if (carta != null)
            {
                mano.Add(carta);
            }
        }
        public bool Quitar_carta(Card carta)
        {
            return mano.Remove(carta);
        }
    }
    public enum CardColor { Rojo, Azul, Verde, Amarillo, Ninguno };
    public enum CardType { Numero, Salta, Reversa, Mas2, Comodin, ComodinMas4 };
    public class Card
    {
        // Variables de la carta
        public int Number;
        public CardColor Color;
        public CardType Type;
        
        // Constructor de la carta
        public Card(int cardNumber, CardColor cardColor, CardType cardType)
        {
            this.Number = cardNumber;
            this.Color = cardColor;
            this.Type = cardType;
        }

        // Obtener nombre
        public override string ToString()
        {
            switch(this.Type)
            {
                case CardType.Numero:
                    return $"{Color} {Number}";
                case CardType.Salta:
                    return $"{Color} Salta";
                case CardType.Reversa:
                    return $"{Color} Reversa";
                case CardType.Mas2:
                    return $"{Color} +2";
                case CardType.ComodinMas4:
                    return "Comodin +4";
                case CardType.Comodin:
                    return "Comodin";
                default:
                    return "No existe la carta";
            }
        }
    }

    public void repartirCartas()
    {

    }
}
