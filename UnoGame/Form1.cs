using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace UnoGame
{
    public partial class Form1 : Form
    {
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

        public Form1()
        {
            InitializeComponent();

            jugadores.Add(new Player("Jugador 1", Cartas1));
            jugadores.Add(new Player("Jugador 2", Cartas2));
            jugadores.Add(new Player("Jugador 3", Cartas3));
            jugadores.Add(new Player("Jugador 4", Cartas4));
            manosJugadores = new List<Card>[] { Cartas1, Cartas2, Cartas3, Cartas4 };
            TurnoCambiado += MostrarTurno;
            RepartirCartas();
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
            richTextBox1.Text = "Turno de: " + jugadores[t].Nombre;
        }


        void AvanzarTurno()
        {
            turno = (turno + sentido + numJugadores) % numJugadores;
            if (TurnoCambiado != null)
            {
                TurnoCambiado.Invoke(turno);
            }
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

            if (jugadores[turno].cantidad_cartas == 1 && !jugadores[turno].DijoUno)
            {
                MessageBox.Show($"{jugadores[turno].Nombre} no dijo UNO a tiempo. ¡Penalización de 2 cartas!");
                for (int i = 0; i < 2; i++)
                    jugadores[turno].Agregar_carta(SacarCarta());
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

            MessageBox.Show($"¡{ganador.Nombre} ganó la partida!", "Fin de la partida");
            richTextBox1.Text = $"Partida terminada. Ganador: {ganador.Nombre}";

            if (PartidaTerminada != null)
                PartidaTerminada.Invoke(ganador);
        }

        private void JugarTurno()
        {
            if (partidaTerminada)
            {
                MessageBox.Show("La partida ya terminó. Inicia una nueva partida para seguir jugando.");
                return;
            }

            List<Card> manoActual = manosJugadores[turno];
            string nombreJugadorActual = jugadores[turno].Nombre;

            if (manoActual.Count == 2 && !jugadores[turno].DijoUno)
            {
                DialogResult respuesta = MessageBox.Show(
                    $"{nombreJugadorActual}, tienes 2 cartas. ¿Quieres decir UNO antes de jugar?",
                    "¡UNO!", MessageBoxButtons.YesNo);

                if (respuesta == DialogResult.Yes)
                {
                    CallUno(jugadores[turno]);
                    MessageBox.Show($"{nombreJugadorActual} dijo UNO.");
                }
            }

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
                        colorElegido = PedirColor();


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
            Card cartaRobada = SacarCarta();

            if (cartaRobada == null)
            {
                MessageBox.Show("No quedan cartas para robar.");
                AvanzarTurno();
                return;
            }

            if (JugadaValida(cartaRobada))
            {
                CardColor colorElegido = CardColor.Ninguno;
                if (cartaRobada.Type == CardType.Comodin || cartaRobada.Type == CardType.Mas4)
                    colorElegido = PedirColor();

                MessageBox.Show($"{nombreJugadorActual} robó {cartaRobada} y se jugó automáticamente.");
                jugadores[turno].Agregar_carta(cartaRobada);
                JugarCarta(cartaRobada, colorElegido);
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
            richTextBox1.Text =
            $"J1: {string.Join(", ", Cartas1.Select(c => c.ToString()))}\n" +
            $"J2: {string.Join(", ", Cartas2.Select(c => c.ToString()))}\n" +
            $"J3: {string.Join(", ", Cartas3.Select(c => c.ToString()))}\n" +
            $"J4: {string.Join(", ", Cartas4.Select(c => c.ToString()))}\n" +
            $"Carta volteada: {ultimaCarta}\n" +
            $"Cartas restantes en la burra: {CartasRestantes()}";
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
            richTextBox1.Text = string.Join(", ", burra.Select(c => c.ToString()));
        }

        private void Avanzar_turno_Click(object sender, EventArgs e)
        {
            AvanzarTurno();
        }

        private void Invertir_turno_Click(object sender, EventArgs e)
        {
            InvertirSentido();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            JugarTurno();
        }
    }
}