using System;
using UnoGame.Red;

namespace UnoServer
{
    class Program
    {
        // Uso:
        //   UnoServer.exe             -> servidor en el puerto 11111
        //   UnoServer.exe 12345       -> servidor en otro puerto
        //   UnoServer.exe simular     -> prueba las reglas con 1000 partidas de bots (sin red)
        static void Main(string[] args)
        {
            Console.Title = "UnoServer";

            if (args.Length > 0 && args[0].ToLower() == "simular")
            {
                Simulacion.Ejecutar(1000);
                Console.WriteLine("Presiona una tecla para salir...");
                Console.ReadKey();
                return;
            }

            if (args.Length > 0 && args[0].ToLower() == "bots")
            {
                ClienteBots.Ejecutar(args);
                Console.WriteLine("Presiona una tecla para salir...");
                Console.ReadKey();
                return;
            }

            if (args.Length > 0 && args[0].ToLower() == "validar")
            {
                PruebasValidacion.Ejecutar();
                Console.WriteLine("Presiona una tecla para salir...");
                Console.ReadKey();
                return;
            }

            int puerto = Protocolo.PuertoPorDefecto;
            int indicado;
            if (args.Length > 0 && int.TryParse(args[0], out indicado))
                puerto = indicado;

            new ServidorUno().Iniciar(puerto);

            //Se prueba si el servidor espera a los jugadores para iniciar la partida
        }
    }
}
