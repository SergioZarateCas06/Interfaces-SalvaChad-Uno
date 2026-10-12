using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//   Define cómo es una carta del juego UNO
namespace UnoGame
{
    // Lista de los colores posibles de una carta
    public enum CardColor { Rojo, Azul, Verde, Amarillo, Ninguno };
    // Lista de los tipos de carta que existen
    public enum CardType { Numero, Salta, Reversa, Mas2, Comodin, Mas4 };

    // Lista de los tipos de carta que existen
    public class Card
    {
        public int Number;
        public CardColor Color;
        public CardType Type;

        // Recibe el número, el color y el tipo, y los guarda
        public Card(int cardNumber, CardColor cardColor, CardType cardType)
        {
            this.Number = cardNumber;
            this.Color = cardColor;
            this.Type = cardType;
        }

        // Devuelve el nombre de la carta como texto, para poder mostrarlo en pantalla
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
                    // Si llega un tipo que no conocemos, avisamos
                    return "No existe la carta";
            }
        }
    }
}
