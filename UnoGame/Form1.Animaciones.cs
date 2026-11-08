using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using UnoGame.Red;

// Animaciones del modo en red. Son decorativas: el estado que manda el servidor se dibuja
// de inmediato y estas cartas solo "viajan" encima para que se note lo que pasó.
// Se calculan comparando el estado anterior con el nuevo.
namespace UnoGame
{
    public partial class Form1
    {
        class Vuelo
        {
            public Control Carta;
            public Point Inicio, Destino;
            public int Paso, Espera;
            public bool Agregada;
            public Action AlAterrizar;
        }

        readonly List<Vuelo> vuelos = new List<Vuelo>();
        readonly System.Windows.Forms.Timer timerVuelos = new System.Windows.Forms.Timer();
        bool repartoPendiente;                 // el primer estado ya llegó, falta animar el reparto
        const int TICKS_ENTRE_CARTAS = 4;      // cada tick son ~15 ms

        // Se llama una vez desde el constructor de red
        void IniciarAnimaciones()
        {
            timerVuelos.Interval = 15;
            timerVuelos.Tick += TimerVuelos_Tick;
            // El reparto se anima cuando la ventana ya se ve (antes no se pueden calcular posiciones)
            this.Shown += (s, e) =>
            {
                if (!repartoPendiente) return;
                repartoPendiente = false;
                AnimarReparto();
            };
        }

        // Se llama desde OnFormClosed
        void DetenerAnimaciones()
        {
            timerVuelos.Stop();
            foreach (Vuelo v in vuelos) QuitarVuelo(v);
            vuelos.Clear();
        }

        // ---------- Motor de vuelos ----------

        void LanzarVuelo(Control carta, Point inicio, Point destino, int espera, Action alAterrizar = null)
        {
            vuelos.Add(new Vuelo
            {
                Carta = carta,
                Inicio = inicio,
                Destino = destino,
                Espera = espera,
                AlAterrizar = alAterrizar
            });
            if (!timerVuelos.Enabled) timerVuelos.Start();
        }

        void TimerVuelos_Tick(object sender, EventArgs e)
        {
            if (IsDisposed) { timerVuelos.Stop(); return; }

            bool aterrizo = false;
            foreach (Vuelo v in vuelos.ToArray())
            {
                if (v.Espera > 0) { v.Espera--; continue; }

                if (!v.Agregada)
                {
                    v.Carta.Location = v.Inicio;
                    Controls.Add(v.Carta);
                    v.Carta.BringToFront();
                    v.Agregada = true;
                }

                v.Paso++;
                v.Carta.Location = new Point(
                    v.Inicio.X + (v.Destino.X - v.Inicio.X) * v.Paso / PASOS_VUELO,
                    v.Inicio.Y + (v.Destino.Y - v.Inicio.Y) * v.Paso / PASOS_VUELO);

                if (v.Paso >= PASOS_VUELO)
                {
                    vuelos.Remove(v);
                    QuitarVuelo(v);
                    aterrizo = true;
                    if (v.AlAterrizar != null) v.AlAterrizar();
                }
            }

            if (aterrizo) Sonar("carta");
            if (vuelos.Count == 0) timerVuelos.Stop();
        }

        void QuitarVuelo(Vuelo v)
        {
            PictureBox pb = v.Carta as PictureBox;
            if (pb != null) pb.Image = null;   // el bitmap pertenece al caché de reversos: no se libera aquí
            Controls.Remove(v.Carta);
            v.Carta.Dispose();
        }

        // ---------- Posiciones (coordenadas de la ventana) ----------

        Point PuntoEn(Panel panel, Point local)
        {
            return this.PointToClient(panel.PointToScreen(local));
        }

        // 0 = yo (abajo), 1 = izquierda, 2 = arriba, 3 = derecha
        Panel PanelDePos(int pos)
        {
            switch (pos)
            {
                case 1: return panel_rival_izquierda;
                case 2: return panel_rival_arriba;
                case 3: return panel_rival_derecha;
                default: return panel_mano_jugador1;
            }
        }

        // Desde dónde sale una carta que juega ese asiento
        Point PuntoDeSalida(int pos)
        {
            Panel p = PanelDePos(pos);
            if (pos == 0) return PuntoEn(p, new Point(Math.Max(10, p.Width / 2 - 35), 10));
            if (pos == 2) return PuntoEn(p, new Point(Math.Max(5, p.Width / 2 - 20), 5));
            return PuntoEn(p, new Point(5, Math.Max(5, p.Height / 2 - 30)));
        }

        // Dónde queda la carta número "indice" de un asiento que tendrá "total" cartas
        Point PuntoDeLlegada(int pos, int indice, int total)
        {
            Panel p = PanelDePos(pos);
            if (pos == 0)
            {
                int x = Math.Min(10 + indice * SeparacionMano(total), Math.Max(10, p.Width - 80));
                return PuntoEn(p, new Point(x, 10));
            }
            bool horizontal = pos == 2;
            int offset = 5 + indice * SeparacionRival(p, total, horizontal);
            return PuntoEn(p, horizontal ? new Point(offset, 5) : new Point(5, offset));
        }

        // Misma separación que calcula MostrarManoJugador
        int SeparacionMano(int cantidad)
        {
            int anchoCarta = 70, separacion = 75, separacionMinima = 40;
            if (cantidad <= 1) return separacion;
            int espacio = panel_mano_jugador1.Width - anchoCarta;
            int real = Math.Min(separacion, espacio / (cantidad - 1));
            return Math.Max(real, separacionMinima);
        }

        // Misma separación que calcula MostrarRival
        int SeparacionRival(Panel p, int cantidad, bool horizontal)
        {
            if (cantidad <= 1) return 15;
            int espacio = (horizontal ? p.Width : p.Height) - 40;
            return Math.Max(10, Math.Min(15, espacio / (cantidad - 1)));
        }

        // ---------- Cartas que vuelan ----------

        Control CartaDeVuelo(Card carta)
        {
            CartaControl cc = new CartaControl();
            cc.Carta = carta;
            cc.BocaAbajo = false;
            return cc;
        }

        Control ReversoDeVuelo(int pos)
        {
            RotateFlipType giro = pos == 1 ? RotateFlipType.Rotate90FlipNone
                                : pos == 2 ? RotateFlipType.Rotate180FlipNone
                                : pos == 3 ? RotateFlipType.Rotate270FlipNone
                                           : RotateFlipType.RotateNoneFlipNone;
            Bitmap bmp = ObtenerReverso(giro);
            PictureBox pb = new PictureBox();
            pb.Image = bmp;
            pb.Size = bmp.Size;
            pb.BackColor = Color.Transparent;
            return pb;
        }

        // ---------- Reparto inicial ----------

        void AnimarReparto()
        {
            EstadoJuego e = estadoActual;
            if (e == null) return;

            const int N = 4;
            int yo = e.TuIndice;
            Point mazo = PuntoEn(panelBurra, new Point(5, 5));
            Point descarte = PuntoEn(panelDescarte, new Point(5, 5));
            Panel[] zonas = { panel_mano_jugador1, panel_rival_izquierda, panel_rival_arriba, panel_rival_derecha };

            // Primero se calculan todos los destinos (con las zonas visibles) y luego se ocultan
            int total = 0;
            for (int ronda = 0; ronda < 7; ronda++)
            {
                for (int pos = 0; pos < N; pos++)
                {
                    int asiento = (yo + pos) % N;
                    int finales = CartasDe(e, asiento);
                    if (ronda >= finales) continue;

                    Control carta = null;
                    if (pos == 0 && e.TuMano != null && ronda < e.TuMano.Count)
                    {
                        Card c;
                        if (e.TuMano[ronda].TryACarta(out c)) carta = CartaDeVuelo(c);
                    }
                    if (carta == null) carta = ReversoDeVuelo(pos);

                    LanzarVuelo(carta, mazo, PuntoDeLlegada(pos, ronda, finales), total * TICKS_ENTRE_CARTAS);
                    total++;
                }
            }

            // Última: la primera carta de la mesa. Al aterrizar se muestra todo.
            Card superior = null;
            if (e.CartaSuperior != null) e.CartaSuperior.TryACarta(out superior);
            Action mostrarTodo = () =>
            {
                foreach (Panel z in zonas) z.Visible = true;
                panelDescarte.Visible = true;
            };

            foreach (Panel z in zonas) z.Visible = false;
            panelDescarte.Visible = false;

            if (superior != null)
                LanzarVuelo(CartaDeVuelo(superior), mazo, descarte, total * TICKS_ENTRE_CARTAS, mostrarTodo);
            else
                LanzarVuelo(ReversoDeVuelo(0), mazo, descarte, total * TICKS_ENTRE_CARTAS, mostrarTodo);
        }

        // ---------- Jugar, robar y comer ----------

        // previo = estado anterior, nuevo = el que acaba de llegar, sup = carta superior del descarte
        void AnimarCambios(EstadoJuego previo, EstadoJuego nuevo, Card sup)
        {
            const int N = 4;
            if (repartoPendiente || previo == null || nuevo == null) return;
            if (previo.CartasPorJugador == null || nuevo.CartasPorJugador == null) return;
            if (previo.CartasPorJugador.Count < N || nuevo.CartasPorJugador.Count < N) return;

            int yo = nuevo.TuIndice;
            int actor = previo.Turno;   // quien hizo la acción
            bool jugada = nuevo.CartasDescarte > previo.CartasDescarte;

            Point mazo = PuntoEn(panelBurra, new Point(5, 5));
            Point descarte = PuntoEn(panelDescarte, new Point(5, 5));

            // Robó una carta jugable y se jugó sola: su cantidad no cambia pero sí hubo robo y jugada
            int deltaActor = nuevo.CartasPorJugador[actor] - previo.CartasPorJugador[actor];
            bool robaYJuega = jugada && deltaActor == 0;

            if (jugada && sup != null)
            {
                int posActor = (actor - yo + N) % N;
                LanzarVuelo(CartaDeVuelo(sup), PuntoDeSalida(posActor), descarte, robaYJuega ? 12 : 0);
            }

            for (int i = 0; i < N; i++)
            {
                int ganadas = nuevo.CartasPorJugador[i] - previo.CartasPorJugador[i];
                if (jugada && i == actor) ganadas++;   // la que jugó no cuenta como pérdida
                if (ganadas <= 0) continue;

                int pos = (i - yo + N) % N;
                int antes = previo.CartasPorJugador[i];
                int totalFinal = nuevo.CartasPorJugador[i];
                int espera = (jugada && !robaYJuega) ? 10 : 0;   // primero la jugada, luego lo que come

                for (int k = 0; k < ganadas; k++)
                {
                    Control carta = null;
                    if (pos == 0)
                    {
                        Card c = null;
                        if (robaYJuega && i == actor) c = sup;
                        else if (nuevo.TuMano != null)
                        {
                            int idx = nuevo.TuMano.Count - ganadas + k;
                            if (idx >= 0 && idx < nuevo.TuMano.Count) nuevo.TuMano[idx].TryACarta(out c);
                        }
                        if (c != null) carta = CartaDeVuelo(c);
                    }
                    if (carta == null) carta = ReversoDeVuelo(pos);

                    LanzarVuelo(carta, mazo, PuntoDeLlegada(pos, antes + k, Math.Max(totalFinal, antes + k + 1)), espera + k * 5);
                }
            }
        }
    }
}
