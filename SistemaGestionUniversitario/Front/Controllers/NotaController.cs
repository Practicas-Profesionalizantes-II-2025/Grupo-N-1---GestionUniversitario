using Front.Models.Crear;
using Front.Models.Respuestas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Security.Claims;

namespace Front.Controllers
{
    public class NotaController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<NotaController> _logger;

        public NotaController(IHttpClientFactory httpClientFactory, ILogger<NotaController> logger)
        {
            _httpClient = httpClientFactory.CreateClient("ApiPrincipal");
            _logger = logger;
        }
        [Authorize(Roles = "Profesor")]
        [HttpPost]
        public async Task<IActionResult> CargarNotas([FromBody] List<CrearNotaAlumnoFront> notas)
        {
            try
            {
                foreach (var nota in notas)
                {
                    // Llamamos al endpoint de la API que guarda las notas
                    var response = await _httpClient.PostAsJsonAsync("NotaAlumno", nota);

                    if (!response.IsSuccessStatusCode)
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        _logger.LogError($"Error al guardar nota de {nota.DNIAlumno}: {error}");
                    }
                }

                return Ok(new { mensaje = "Notas guardadas correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar notas");
                return BadRequest(new { mensaje = "Error al guardar notas" });
            }
        }
        [HttpGet]
        public async Task<IActionResult> ObtenerNotasPorAlumno()
        {
            string dniAlumno = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(dniAlumno))
            {
                _logger.LogWarning("No se encontró el DNI en los claims del usuario logueado.");
                return Unauthorized("No se pudo identificar al Alumno.");
            }
            try
            {
                var response = await _httpClient.GetAsync($"NotaAlumno/dniAlumno/{dniAlumno}");
                if (response.IsSuccessStatusCode)
                {
                    var notas = await response.Content.ReadFromJsonAsync<List<NotaAlumnoFront>>();

                    // Ordenar por Materia y Tipo de Examen
                    var notasOrdenadas = notas
                        .OrderBy(n => n.ExamenMateriaNombre)
                        .ThenBy(n => n.ExamenTipo)
                        .ToList();

                    return Ok(notasOrdenadas);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    return NoContent();
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Error al obtener notas: {error}");
                    return StatusCode((int)response.StatusCode, new { mensaje = "Error al obtener notas" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener notas");
                return BadRequest(new { mensaje = "Error al obtener notas" });
            }
        }
    }
}
