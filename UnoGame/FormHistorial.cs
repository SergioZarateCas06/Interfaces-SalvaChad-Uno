using System; // Funciones básicas de C#
using System.Collections.Generic; // Permite usar lista
using System.Data; // Permite usar DataTable
using System.Windows.Forms;  // Ventanas y controles de Windows Forms

namespace UnoGame
{
    // Es la ventana del historial
    public partial class FormHistorial : Form
    {
        // Objeto para comunicarse con el servidor
        private readonly ApiCliente api = new ApiCliente();

        // Se ejecuta al crear la ventana
        // Prepara los controles del diseñador
        public FormHistorial()
        {
            InitializeComponent();
        }

        // Se ejecuta automáticamente cuando la ventana se abre
        private async void FormHistorial_Load(object sender, EventArgs e)
        {
            labelEstadoHistorial.Text = "Cargando datos...";

            try
            {
                // Estadísticas de cada jugador
                List<EstadisticaJugador> estadisticas = await api.ObtenerEstadisticas();
                MostrarEstadisticas(estadisticas);
                // Historial de partidas, y la lista para elegir una partida
                List<HistorialItem> historial = await api.ObtenerHistorial();
                MostrarHistorial(historial);
                CargarComboPartidas(historial);

                labelEstadoHistorial.Text = "";  // Todo salió bien: se borra el mensaje
            }
            catch (Exception ex)
            {
                labelEstadoHistorial.Text = "No se pudo cargar el historial: " + ex.Message;
            }
        }
        // Llena la lista desplegable con las partidas jugadas
        private void CargarComboPartidas(List<HistorialItem> historial)
        {
            // Cada partida aparece varias veces (una por jugador); nos quedamos con los id únicos
            List<int> idsUnicos = new List<int>();
            foreach (HistorialItem item in historial)
            {
                if (!idsUnicos.Contains(item.PartidaId)) // Solo se agrega si aún no está
                    idsUnicos.Add(item.PartidaId);
            }
            idsUnicos.Sort();  // Ordena de menor a mayor
            idsUnicos.Reverse(); // más reciente primero
             // Vaciamos la lista y la llenamos con los textos
            comboPartidas.Items.Clear();
            foreach (int id in idsUnicos)
            {
                comboPartidas.Items.Add("Partida #" + id);
            }
            // Si hay partidas, se selecciona la primera
            if (comboPartidas.Items.Count > 0)
                comboPartidas.SelectedIndex = 0;
        }
        // Se ejecuta cuando se elige otra partida en la lista desplegable
        private async void comboPartidas_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboPartidas.SelectedItem == null) return; // Si no hay nada elegido, no hace nada
             // Del texto "Partida #2" nos quedamos solo con el número 
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
        // Llena la tabla de movimientos de una partida
        private void MostrarMovimientos(List<MovimientoItem> movimientos)
        {
            // Creamos una tabla en memoria con sus columnas
            DataTable tabla = new DataTable();
            tabla.Columns.Add("Turno", typeof(int));
            tabla.Columns.Add("Jugador");
            tabla.Columns.Add("Acción");
            tabla.Columns.Add("Carta");
            // Agregamos una fila por cada movimiento
            foreach (MovimientoItem m in movimientos)
            {
                tabla.Rows.Add(m.NumeroTurno, m.NombreJugador, m.Accion, m.DescripcionCarta ?? "");
            }
            // Mostramos la tabla en la cuadrícula y ajustamos las columnas al ancho disponible
            dgvMovimientos.DataSource = tabla;
            dgvMovimientos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
        // Llena la tabla de estadísticas

        private void MostrarEstadisticas(List<EstadisticaJugador> estadisticas)
        {
            DataTable tabla = new DataTable();
            tabla.Columns.Add("Jugador");
            tabla.Columns.Add("Victorias", typeof(int));
            tabla.Columns.Add("Derrotas", typeof(int));
            // Una fila por jugador
            foreach (EstadisticaJugador item in estadisticas)
            {
                tabla.Rows.Add(item.NombreJugador, item.Victorias, item.Derrotas);
            }

            dgvEstadisticas.DataSource = tabla;
            dgvEstadisticas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
        // Llena la tabla del historial de partidas
        private void MostrarHistorial(List<HistorialItem> historial)
        {
            DataTable tabla = new DataTable();
            tabla.Columns.Add("Partida");
            tabla.Columns.Add("Fecha");
            tabla.Columns.Add("Jugador");
            tabla.Columns.Add("Resultado");

            foreach (HistorialItem item in historial)
            {
                // Intentamos darle a la fecha un formato fácil de leer
                string fechaFormateada = item.Fecha;
                DateTime fechaParseada;
                if (DateTime.TryParse(item.Fecha, out fechaParseada))
                {
                    fechaFormateada = fechaParseada.ToString("dd/MM/yyyy HH:mm");
                }
                // Agregamos la fila. "Gano" es true/false; lo convertimos a texto
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