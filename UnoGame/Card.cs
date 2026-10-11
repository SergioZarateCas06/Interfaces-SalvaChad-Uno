using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnoGame
{
    public enum CardColor { Rojo, Azul, Verde, Amarillo, Ninguno };
    public enum CardType { Numero, Salta, Reversa, Mas2, Comodin, Mas4 };

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
                case CardType.Mas4:
                    return "Comodin +4";
                case CardType.Comodin:
                    return "Comodin";
                default:
                    return "No existe la carta";
            }
        }
    }
}
