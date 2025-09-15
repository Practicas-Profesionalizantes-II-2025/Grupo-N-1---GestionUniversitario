using Front.Models.Crear;
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