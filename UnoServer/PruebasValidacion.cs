using System;
using System.Linq;
using UnoGame;

namespace UnoServer
{
    // Comprueba que el motor rechaza las acciones inválidas con el mensaje correcto
    // (el servidor solo reenvía ese texto al cliente en un mensaje "error").
    // Uso:  UnoServer.exe validar
    public static class PruebasValidacion
    {
        static int fallos;
        static int pruebas;

        public static void Ejecutar()
        {
            fallos = 0;
            pruebas = 0;

            // Partida nueva: es el turno del jugador 0
            JuegoUno juego = Nueva();

            Esperar("Robar fuera de turno", juego.Robar(1), "No es tu turno.");
            Esperar("Jugar fuera de turno", juego.Jugar(1, juego.Manos[1][0], CardColor.Ninguno), "No es tu turno.");
            Esperar("Carta que no está en la mano", juego.Jugar(0, new Card(99, CardColor.Rojo, CardType.Numero), CardColor.Ninguno), "Esa carta no está en tu mano.");
            Esperar("UNO con 7 cartas", juego.DecirUno(0), "Solo puedes decir UNO cuando tienes 2 cartas.");
            Esperar("Elegir color sin comodín pendiente", juego.ElegirColor(0, CardColor.Rojo), "No tienes ningún comodín esperando color.");

            Card noJugable = juego.Manos[0].FirstOrDefault(c => !juego.JugadaValida(c));
            if (noJugable != null)
                Esperar("Carta que no se puede jugar", juego.Jugar(0, noJugable, CardColor.Ninguno), "Esa carta no es una jugada válida.");
            else
                Console.WriteLine("(omitida) Carta que no se puede jugar: toda la mano era jugable");

            // Se buscan partidas hasta que el jugador 0 tenga un comodín
            JuegoUno conComodin = null;
            for (int i = 0; i < 500 && conComodin == null; i++)
            {
                JuegoUno intento = Nueva();
                if (intento.Manos[0].Any(c => EsComodin(c)))
                    conComodin = intento;
            }
            if (conComodin != null)
            {
                Card comodin = conComodin.Manos[0].First(c => EsComodin(c));
                Esperar("Comodín sin elegir color", conComodin.Jugar(0, comodin, CardColor.Ninguno), "Debes elegir un color para el comodín.");
            }
            else
            {
                Console.WriteLine("(omitida) Comodín sin color: no salió ningún comodín en 500 partidas");
            }

            // Una jugada válida debe aceptarse y pasar el turno
            JuegoUno limpio = Nueva();
            Card valida = limpio.Manos[0].FirstOrDefault(c => limpio.JugadaValida(c));
            if (valida != null)
            {
                string resultado = limpio.Jugar(0, valida, EsComodin(valida) ? CardColor.Rojo : CardColor.Ninguno);
                Esperar("Jugada válida", resultado, null);
                Esperar("El turno avanza", limpio.Turno != 0 ? null : "el turno sigue en 0", null);
            }
            else
            {
                Console.WriteLine("(omitida) Jugada válida: el jugador 0 no tenía ninguna jugable");
            }

            Console.WriteLine();
            Console.WriteLine("Pruebas: " + pruebas + "   Fallos: " + fallos);
        }

        static JuegoUno Nueva()
        {
            JuegoUno juego = new JuegoUno();
            juego.Iniciar(new[] { "A", "B", "C", "D" });
            return juego;
        }

        static bool EsComodin(Card carta)
        {
            return carta.Type == CardType.Comodin || carta.Type == CardType.Mas4;
        }

        static void Esperar(string nombre, string obtenido, string esperado)
        {
            pruebas++;
            if (obtenido == esperado)
            {
                Console.WriteLine("OK     " + nombre);
            }
            else
            {
                fallos++;
                Console.WriteLine("FALLO  " + nombre);
                Console.WriteLine("       esperado: " + (esperado ?? "(sin error)"));
                Console.WriteLine("       obtenido: " + (obtenido ?? "(sin error)"));
            }
        }
    }
}
