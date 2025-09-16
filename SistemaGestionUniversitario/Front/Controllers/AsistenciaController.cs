using Front.Models.Crear;
using Front.Models.Respuestas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        // GET: /Asistencia/GetInasistenciaDNI/dni
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
                        string error = await response.Content.ReadAsStringAsync();
                        ModelState.AddModelError("", error);
                        return RedirectToAction("GetInscripcionesAsistencia", "Inscripcion");
                    }
                }

                TempData["Success"] = "Inscripcion dada de alta correctamente.";
                return RedirectToAction("GetInscripcionesAsistencia", "Inscripcion");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al dar de alta inscripcion.");
                return RedirectToAction("GetInscripcionesAsistencia", "Inscripcion");
            }
        }
    }
}