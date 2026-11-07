using System;
using System.Collections.Generic;
using System.Linq;
using UnoGame;

namespace UnoServer
{
    // Juega muchas partidas completas con 4 bots que eligen al azar entre las jugadas válidas.
    // No prueba la red, pero sí que las reglas no se atoren ni lancen errores en ningún caso
    // (mazo agotado, +2 y +4 seguidos, Reversa, UNO olvidado, comodines robados, etc.).
    public static class Simulacion
    {
        public static void Ejecutar(int partidas)
        {
            Random azar = new Random();
            int terminadas = 0;
            int atascadas = 0;
            long acciones = 0;
            int[] victorias = new int[JuegoUno.NumJugadores];

            for (int p = 0; p < partidas; p++)
            {
                JuegoUno juego = new JuegoUno();
                juego.Iniciar(new[] { "A", "B", "C", "D" });

                int n = 0;
                while (!juego.Terminada && n < 5000)
                {
                    n++;
                    int j = juego.Turno;
                    List<Card> mano = juego.Manos[j];
                    string error;

                    if (juego.EsperaColor)
                    {
                        error = juego.ElegirColor(j, ColorAzar(azar));
                    }
                    else
                    {
                        List<Card> jugables = mano.Where(c => juego.JugadaValida(c)).ToList();
                        if (jugables.Count == 0)
                        {
                            error = juego.Robar(j);
                        }
                        else
                        {
                            Card elegida = jugables[azar.Next(jugables.Count)];
                            // A veces "olvida" decir UNO, para que se pruebe la penalización
                            if (mano.Count == 2 && azar.Next(2) == 0)
                                juego.DecirUno(j);

                            bool comodin = elegida.Type == CardType.Comodin || elegida.Type == CardType.Mas4;
                            error = juego.Jugar(j, elegida, comodin ? ColorAzar(azar) : CardColor.Ninguno);
                        }
                    }

                    if (error != null)
                        throw new Exception("El motor rechazó una acción que debía ser válida: " + error);

                    juego.Eventos.Clear();
                    juego.Movimientos.Clear();
                }

                acciones += n;
                if (juego.Terminada)
                {
                    terminadas++;
                    victorias[juego.Ganador]++;
                }
                else
                {
                    atascadas++;
                }
            }

            Console.WriteLine("Partidas jugadas:      " + partidas);
            Console.WriteLine("Terminadas con ganador: " + terminadas);
            Console.WriteLine("Atascadas (5000 acc.):  " + atascadas);
            Console.WriteLine("Acciones por partida:   " + (acciones / partidas));
            Console.WriteLine("Victorias por asiento:  " + string.Join(" / ", victorias));
        }

        static CardColor ColorAzar(Random azar)
        {
            return (CardColor)azar.Next(4);   // Rojo, Azul, Verde o Amarillo
        }
    }
}
