using Front.Models.Crear;
using Front.Models.Respuestas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace Front.Controllers
{
    public class AsistenciaController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AsistenciaController> _logger;

        public AsistenciaController(IHttpClientFactory httpClientFactory, ILogger<AsistenciaController> logger)
        {
            _httpClient = httpClientFactory.CreateClient("ApiPrincipal");
            _logger = logger;
        }

        // GET: /Asistencia/GetInasistenciaDNI
        [Authorize(Roles = "Alumno")]
        [HttpGet]
        public async Task<IActionResult> GetInasistenciaDNI()
        {
            try
            {
                // Alumno Logueado
                string? dniUsuarioLogueado = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                List<AsistenciaFront>? asistencias = await _httpClient.GetFromJsonAsync<List<AsistenciaFront>>($"Asistencia/DNI/{dniUsuarioLogueado}");

                return View(asistencias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener inasistencias por DNI");
                return RedirectToAction("Index");
            }
        }

        // GET: /Asistencia/GetAsistenciasMateria/nombreMateria
        [Authorize(Roles = "Profesor")]
        [HttpGet]
        public async Task<IActionResult> GetAsistenciasMateria(string? nombreMateria)
        {
            try
            {
                // Profesor Logueado
                string? dniUsuarioLogueado = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Todas las materias para el listado
                var materias = await _httpClient
                    .GetFromJsonAsync<List<MateriaFront>>($"Materia/DNIProfesor/{dniUsuarioLogueado}")
                    ?? new List<MateriaFront>();

                ViewBag.Materias = materias;
                ViewBag.SelectedMateria = nombreMateria;

                // Si todavía no seleccionó ninguna materia
                if (string.IsNullOrEmpty(nombreMateria) || materias == null)
                {
                    return View(new List<AsistenciaFront>());
                }

                // Obtener asistencias de esa materia
                var asistencias = await _httpClient
                    .GetFromJsonAsync<List<AsistenciaFront>>($"Asistencia/NombreMateria/{nombreMateria}")
                    ?? new List<AsistenciaFront>();

                return View(asistencias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener asistencias por Materia");
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: /Asistencia
        [Authorize(Roles = "Profesor")]
        [HttpPost]
        public async Task<IActionResult> CreateAsistencia(List<CrearAsistenciaFront> asistencias)
        {
            try
            {
                foreach(CrearAsistenciaFront asistencia in asistencias){

                    HttpResponseMessage response = await _httpClient.PostAsJsonAsync("Asistencia", asistencia);

                    if (!response.IsSuccessStatusCode)
                    {
                        var contenido = await response.Content.ReadAsStringAsync();
                        string mensajeError;

                        try
                        {
                            var errorObj = JsonSerializer.Deserialize<Dictionary<string, string>>(contenido);
                            mensajeError = errorObj?["mensaje"] ?? "Error desconocido.";
                        }
                        catch
                        {
                            mensajeError = contenido;
                        }

                        TempData["Error"] = mensajeError;

                        if (asistencias.Any())
                        {
                            return RedirectToAction("GetInscripcionesAsistencia", "Inscripcion", new { nombreMateria = asistencias.First().nombreMateria });
                        }

                        return RedirectToAction("GetInscripcionesAsistencia", "Inscripcion");
                    }

                }

                TempData["Success"] = "Asistencia dada de alta correctamente.";

                if (asistencias.Any())
                {
                    return RedirectToAction("GetInscripcionesAsistencia", "Inscripcion", new { nombreMateria = asistencias.First().nombreMateria });
                }

                return RedirectToAction("GetInscripcionesAsistencia", "Inscripcion");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al dar de alta asistencia.");

                if (asistencias.Any())
                {
                    return RedirectToAction("GetInscripcionesAsistencia", "Inscripcion", new { nombreMateria = asistencias.First().nombreMateria });
                }

                return RedirectToAction("GetInscripcionesAsistencia", "Inscripcion");
            }
        }

        // POST: /Asistencia
        [Authorize(Roles = "Profesor")]
        [HttpPost]
        public async Task<IActionResult> ModAsistencia(List<CrearAsistenciaFront> asistencias)
        {
            try
            {
                foreach (CrearAsistenciaFront asistencia in asistencias)
                {

                    HttpResponseMessage response = await _httpClient.PostAsJsonAsync("Asistencia", asistencia);

                    if (!response.IsSuccessStatusCode)
                    {
                        var contenido = await response.Content.ReadAsStringAsync();
                        string mensajeError;

                        try
                        {
                            var errorObj = JsonSerializer.Deserialize<Dictionary<string, string>>(contenido);
                            mensajeError = errorObj?["mensaje"] ?? "Error desconocido.";
                        }
                        catch
                        {
                            mensajeError = contenido;
                        }

                        TempData["Error"] = mensajeError;

                        if (asistencias.Any())
                        {
                            return RedirectToAction("GetAsistenciasMateria", "Asistencia", new { nombreMateria = asistencias.First().nombreMateria });
                        }

                        return RedirectToAction("GetAsistenciasMateria", "Asistencia");
                    }
                }

                TempData["Success"] = "Asistencias actualizadas correctamente.";

                // Redirige al GET pasando la materia seleccionada
                if (asistencias.Any())
                {
                    return RedirectToAction("GetAsistenciasMateria", "Asistencia", new { nombreMateria = asistencias.First().nombreMateria });
                }

                return RedirectToAction("GetAsistenciasMateria", "Asistencia");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar asistencia.");
                if (asistencias.Any())
                {
                    return RedirectToAction("GetAsistenciasMateria", "Asistencia", new { nombreMateria = asistencias.First().nombreMateria });
                }
                return RedirectToAction("GetAsistenciasMateria", "Asistencia");
            }
        }

        // DELETE: /Asistencia/nombreMateria/fecha
        [Authorize(Roles = "Profesor")]
        [HttpPost]
        public async Task<IActionResult> DeleteAsistencia(string nombreMateria, DateTime fecha)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.DeleteAsync($"Asistencia/{nombreMateria}/{fecha:yyyy-MM-dd}");

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        string error = await response.Content.ReadAsStringAsync();
                        TempData["Error"] = $"Error de validación: {error}";
                    }
                    else
                    {
                        TempData["Error"] = "Ocurrió un error inesperado al dar de baja la asistencia.";
                    }

                    return RedirectToAction("GetAsistenciasMateria", new { nombreMateria });
                }

                TempData["Success"] = "Asistencia dada de baja correctamente.";
                return RedirectToAction("GetAsistenciasMateria", new { nombreMateria });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al dar de baja la asistencia");
                TempData["Error"] = "Ocurrió un error al dar de baja la asistencia.";
                return RedirectToAction("GetAsistenciasMateria", new { nombreMateria });
            }
        }
    }
}