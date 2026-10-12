using System; // Funciones básicas de C#
using System.Collections.Generic; // Permite usar listas
using System.Net.Http; // Permite usar listas
using System.Text;  // Permite definir cómo se escribe el texto
using System.Text.Json; // Permite convertir objetos a JSON y JSON a objetos
using System.Threading.Tasks;  // Permite usar "async/await"

namespace UnoGame
{
    public class ApiCliente
    {
        private static readonly HttpClient cliente = new HttpClient(); // Objeto que envía y recibe las peticiones al servidor
        private const string BaseUrl = "http://127.0.0.1:5000";

        
        public async Task<int> CrearPartida(List<string> jugadores)
        {
            var body = new { jugadores = jugadores }; //Armamos el paquete de datos que enviaremos
            string json = JsonSerializer.Serialize(body); // Convertimos ese paquete a texto JSON
            var contenido = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage respuesta = await cliente.PostAsync($"{BaseUrl}/partidas", contenido);  // Enviamos los datos al servidor y esperamos su respuesta
            respuesta.EnsureSuccessStatusCode();

            string respuestaJson = await respuesta.Content.ReadAsStringAsync(); // Leemos el texto JSON que nos respondió el servidor
            using (JsonDocument doc = JsonDocument.Parse(respuestaJson))
            {
                return doc.RootElement.GetProperty("partida_id").GetInt32(); // Abrimos el JSON y sacamos el valor partida_id
            }
        }

        // Guarda en el servidor una jugada hecha durante la partida
        public async Task RegistrarMovimiento(int partidaId, int numeroTurno, string nombreJugador, string accion, string descripcionCarta)
        {
            var body = new
            {
                numero_turno = numeroTurno,
                nombre_jugador = nombreJugador,
                accion = accion,
                descripcion_carta = descripcionCarta
            };
            // Convertimos los datos a JSON y los preparamos para enviar
            string json = JsonSerializer.Serialize(body);
            var contenido = new StringContent(json, Encoding.UTF8, "application/json");
            // Enviamos el movimiento a la partida indicada
            HttpResponseMessage respuesta = await cliente.PostAsync($"{BaseUrl}/partidas/{partidaId}/movimientos", contenido);
            respuesta.EnsureSuccessStatusCode();
        }

        // Guarda en el servidor quién ganó la partida
        public async Task RegistrarGanador(int partidaId, string nombreJugador)
        {
            var body = new { nombre_jugador = nombreJugador }; // Dato que se enviará: el nombre del ganador
            string json = JsonSerializer.Serialize(body);
            var contenido = new StringContent(json, Encoding.UTF8, "application/json");
            // Enviamos la actualización al servidor
            HttpResponseMessage respuesta = await cliente.PutAsync($"{BaseUrl}/partidas/{partidaId}/ganador", contenido);
            respuesta.EnsureSuccessStatusCode(); // Si hubo un error en el servidor, lanza una excepción
        }

        // Pide al servidor el historial de partidas jugadas
        public async Task<List<HistorialItem>> ObtenerHistorial()
        {
            string json = await cliente.GetStringAsync($"{BaseUrl}/historial");
            return JsonSerializer.Deserialize<List<HistorialItem>>(json); // Convertimos el texto JSON en una lista de objetos de C#
        }

        // Pide al servidor las estadísticas de cada jugador
        public async Task<List<EstadisticaJugador>> ObtenerEstadisticas()
        {
            string json = await cliente.GetStringAsync($"{BaseUrl}/estadisticas");
            return JsonSerializer.Deserialize<List<EstadisticaJugador>>(json);
        }

        // Pide al servidor todos los movimientos de una partida
        public async Task<List<MovimientoItem>> ObtenerMovimientos(int partidaId)
        {
            string json = await cliente.GetStringAsync($"{BaseUrl}/partidas/{partidaId}/movimientos");
            return JsonSerializer.Deserialize<List<MovimientoItem>>(json);
        }
    }
    // Representa UN registro del historial de partidas
    public class HistorialItem
    {
        // Número de la partida
        [System.Text.Json.Serialization.JsonPropertyName("partida_id")]
        public int PartidaId { get; set; }
        // Fecha en que se jugó la partida (en texto)
        [System.Text.Json.Serialization.JsonPropertyName("fecha")]
        public string Fecha { get; set; }
        // Nombre del jugador
        [System.Text.Json.Serialization.JsonPropertyName("nombre_jugador")]
        public string NombreJugador { get; set; }
        // El servidor manda 1 si ganó y 0 si perdió
        [System.Text.Json.Serialization.JsonPropertyName("gano")]
        public int GanoNumero { get; set; }

        public bool Gano => GanoNumero != 0;
    }
    // Representa las estadísticas de UN jugador
    public class EstadisticaJugador
    {
        // Nombre del jugador
        [System.Text.Json.Serialization.JsonPropertyName("nombre_jugador")]
        public string NombreJugador { get; set; }
        // Cantidad de partidas ganadas
        [System.Text.Json.Serialization.JsonPropertyName("victorias")]
        public int Victorias { get; set; }
        // Cantidad de partidas perdidas
        [System.Text.Json.Serialization.JsonPropertyName("derrotas")]
        public int Derrotas { get; set; }
    }
    // Representa un movimiento hecho durante una partida
    public class MovimientoItem
    {
        // Identificador único del movimiento
        [System.Text.Json.Serialization.JsonPropertyName("id")]
        public int Id { get; set; }
        // Partida a la que pertenece el movimiento
        [System.Text.Json.Serialization.JsonPropertyName("partida_id")]
        public int PartidaId { get; set; }
        // Número del turno en que ocurrió
        [System.Text.Json.Serialization.JsonPropertyName("numero_turno")]
        public int NumeroTurno { get; set; }
        // Jugador que hizo el movimiento
        [System.Text.Json.Serialization.JsonPropertyName("nombre_jugador")]
        public string NombreJugador { get; set; }
        // Acción realizada
        [System.Text.Json.Serialization.JsonPropertyName("accion")]
        public string Accion { get; set; }
        // Descripción de la carta usada
        [System.Text.Json.Serialization.JsonPropertyName("descripcion_carta")]
        public string DescripcionCarta { get; set; }
        // Fecha y hora en que se registró el movimiento
        [System.Text.Json.Serialization.JsonPropertyName("fecha")]
        public string Fecha { get; set; }
    }
}