using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace UnoGame
{
    public class ApiCliente
    {
        private static readonly HttpClient cliente = new HttpClient();
        private const string BaseUrl = "http://127.0.0.1:5000";

        // POST /partidas -> devuelve el partida_id creado
        public async Task<int> CrearPartida(List<string> jugadores)
        {
            var body = new { jugadores = jugadores };
            string json = JsonSerializer.Serialize(body);
            var contenido = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage respuesta = await cliente.PostAsync($"{BaseUrl}/partidas", contenido);
            respuesta.EnsureSuccessStatusCode();

            string respuestaJson = await respuesta.Content.ReadAsStringAsync();
            using (JsonDocument doc = JsonDocument.Parse(respuestaJson))
            {
                return doc.RootElement.GetProperty("partida_id").GetInt32();
            }
        }

        // POST /partidas/{partida_id}/movimientos
        public async Task RegistrarMovimiento(int partidaId, int numeroTurno, string nombreJugador, string accion, string descripcionCarta)
        {
            var body = new
            {
                numero_turno = numeroTurno,
                nombre_jugador = nombreJugador,
                accion = accion,
                descripcion_carta = descripcionCarta
            };
            string json = JsonSerializer.Serialize(body);
            var contenido = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage respuesta = await cliente.PostAsync($"{BaseUrl}/partidas/{partidaId}/movimientos", contenido);
            respuesta.EnsureSuccessStatusCode();
        }

        // PUT /partidas/{partida_id}/ganador
        public async Task RegistrarGanador(int partidaId, string nombreJugador)
        {
            var body = new { nombre_jugador = nombreJugador };
            string json = JsonSerializer.Serialize(body);
            var contenido = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage respuesta = await cliente.PutAsync($"{BaseUrl}/partidas/{partidaId}/ganador", contenido);
            respuesta.EnsureSuccessStatusCode();
        }

        // GET /historial
        public async Task<List<HistorialItem>> ObtenerHistorial()
        {
            string json = await cliente.GetStringAsync($"{BaseUrl}/historial");
            return JsonSerializer.Deserialize<List<HistorialItem>>(json);
        }

        // GET /estadisticas
        // GET /estadisticas
        public async Task<List<EstadisticaJugador>> ObtenerEstadisticas()
        {
            string json = await cliente.GetStringAsync($"{BaseUrl}/estadisticas");
            return JsonSerializer.Deserialize<List<EstadisticaJugador>>(json);
        }

        // GET /partidas/{partida_id}/movimientos
        public async Task<List<MovimientoItem>> ObtenerMovimientos(int partidaId)
        {
            string json = await cliente.GetStringAsync($"{BaseUrl}/partidas/{partidaId}/movimientos");
            return JsonSerializer.Deserialize<List<MovimientoItem>>(json);
        }
    }

    public class HistorialItem
    {
        [System.Text.Json.Serialization.JsonPropertyName("partida_id")]
        public int PartidaId { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("fecha")]
        public string Fecha { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("nombre_jugador")]
        public string NombreJugador { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("gano")]
        public int GanoNumero { get; set; }

        public bool Gano => GanoNumero != 0;
    }
    public class EstadisticaJugador
    {
        [System.Text.Json.Serialization.JsonPropertyName("nombre_jugador")]
        public string NombreJugador { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("victorias")]
        public int Victorias { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("derrotas")]
        public int Derrotas { get; set; }
    }

    public class MovimientoItem
    {
        [System.Text.Json.Serialization.JsonPropertyName("id")]
        public int Id { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("partida_id")]
        public int PartidaId { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("numero_turno")]
        public int NumeroTurno { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("nombre_jugador")]
        public string NombreJugador { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("accion")]
        public string Accion { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("descripcion_carta")]
        public string DescripcionCarta { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("fecha")]
        public string Fecha { get; set; }
    }
}