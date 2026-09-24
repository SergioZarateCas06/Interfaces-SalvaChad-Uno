using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UnoGame
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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
}
