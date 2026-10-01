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
            RepartirCartas();
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

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
                RepartirCartas();
                richTextBox1.Text =
                $"J1: {string.Join(", ", Cartas1.Select(c => c.ToString()))}\n" +
                $"J2: {string.Join(", ", Cartas2.Select(c => c.ToString()))}\n" +
                $"J3: {string.Join(", ", Cartas3.Select(c => c.ToString()))}\n" +
                $"J4: {string.Join(", ", Cartas4.Select(c => c.ToString()))}\n" +
                $"Carta volteada: {ultimaCarta}\n" +
                $"Cartas restantes en la burra: {CartasRestantes()}";
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

            public Player(string nombre, List<Card> mano)
            {
                this.Nombre = nombre;
                this.mano = mano;
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
                }
            }
            public bool Quitar_carta(Card carta)
            {
                return mano.Remove(carta);
            }
        }
        private void RepartirCartas()
        {
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
            while (ultimaCarta != null && ultimaCarta.Type == CardType.ComodinMas4)
            {
                DevolverYBarajar(ultimaCarta);
                ultimaCarta = SacarCarta();
            }
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
}