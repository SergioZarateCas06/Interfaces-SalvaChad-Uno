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
    }
}