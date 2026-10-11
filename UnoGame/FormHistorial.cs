using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace UnoGame
{
    public partial class FormHistorial : Form
    {
        private readonly ApiCliente api = new ApiCliente();

        public FormHistorial()
        {
            InitializeComponent();
        }


        private async void FormHistorial_Load(object sender, EventArgs e)
        {
            labelEstadoHistorial.Text = "Cargando datos...";

            try
            {
                List<EstadisticaJugador> estadisticas = await api.ObtenerEstadisticas();
                MostrarEstadisticas(estadisticas);

                List<HistorialItem> historial = await api.ObtenerHistorial();
                MostrarHistorial(historial);
                CargarComboPartidas(historial);

                labelEstadoHistorial.Text = "";
            }
            catch (Exception ex)
            {
                labelEstadoHistorial.Text = "No se pudo cargar el historial: " + ex.Message;
            }
        }

        private void CargarComboPartidas(List<HistorialItem> historial)
        {
            // Cada partida aparece varias veces (una por jugador); nos quedamos con los id únicos
            List<int> idsUnicos = new List<int>();
            foreach (HistorialItem item in historial)
            {
                if (!idsUnicos.Contains(item.PartidaId))
                    idsUnicos.Add(item.PartidaId);
            }
            idsUnicos.Sort();
            idsUnicos.Reverse(); // más reciente primero

            comboPartidas.Items.Clear();
            foreach (int id in idsUnicos)
            {
                comboPartidas.Items.Add("Partida #" + id);
            }

            if (comboPartidas.Items.Count > 0)
                comboPartidas.SelectedIndex = 0;
        }

        private async void comboPartidas_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboPartidas.SelectedItem == null) return;

            string texto = comboPartidas.SelectedItem.ToString(); // "Partida #2"
            int partidaId = int.Parse(texto.Replace("Partida #", ""));

            try
            {
                List<MovimientoItem> movimientos = await api.ObtenerMovimientos(partidaId);
                MostrarMovimientos(movimientos);
            }
            catch (Exception ex)
            {
                labelEstadoHistorial.Text = "No se pudieron cargar los movimientos: " + ex.Message;
            }
        }

        private void MostrarMovimientos(List<MovimientoItem> movimientos)
        {
            DataTable tabla = new DataTable();
            tabla.Columns.Add("Turno", typeof(int));
            tabla.Columns.Add("Jugador");
            tabla.Columns.Add("Acción");
            tabla.Columns.Add("Carta");

            foreach (MovimientoItem m in movimientos)
            {
                tabla.Rows.Add(m.NumeroTurno, m.NombreJugador, m.Accion, m.DescripcionCarta ?? "");
            }

            dgvMovimientos.DataSource = tabla;
            dgvMovimientos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void MostrarEstadisticas(List<EstadisticaJugador> estadisticas)
        {
            DataTable tabla = new DataTable();
            tabla.Columns.Add("Jugador");
            tabla.Columns.Add("Victorias", typeof(int));
            tabla.Columns.Add("Derrotas", typeof(int));

            foreach (EstadisticaJugador item in estadisticas)
            {
                tabla.Rows.Add(item.NombreJugador, item.Victorias, item.Derrotas);
            }

            dgvEstadisticas.DataSource = tabla;
            dgvEstadisticas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void MostrarHistorial(List<HistorialItem> historial)
        {
            DataTable tabla = new DataTable();
            tabla.Columns.Add("Partida");
            tabla.Columns.Add("Fecha");
            tabla.Columns.Add("Jugador");
            tabla.Columns.Add("Resultado");

            foreach (HistorialItem item in historial)
            {
                string fechaFormateada = item.Fecha;
                DateTime fechaParseada;
                if (DateTime.TryParse(item.Fecha, out fechaParseada))
                {
                    fechaFormateada = fechaParseada.ToString("dd/MM/yyyy HH:mm");
                }

                tabla.Rows.Add(
                    "#" + item.PartidaId,
                    fechaFormateada,
                    item.NombreJugador,
                    item.Gano ? "Victoria" : "Derrota"
                );
            }

            dgvHistorial.DataSource = tabla;
            dgvHistorial.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
    }
}