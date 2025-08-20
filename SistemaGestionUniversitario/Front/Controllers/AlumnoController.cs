using Front.Models.Respuestas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Front.Controllers
{
    public class AlumnoController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AlumnoController> _logger;

        public AlumnoController(IHttpClientFactory httpClientFactory, ILogger<AlumnoController> logger)
        {
            _httpClient = httpClientFactory.CreateClient("ApiPrincipal");
            _logger = logger;
        }

        // GET: /Alumno/GetAlumnos
        [Authorize(Roles = "Administrador, Profesor")]
        [HttpGet]
        public async Task<IActionResult> GetAlumnos()
        {
            try
            {
                List<AlumnoFront>? alumnos = await _httpClient.GetFromJsonAsync<List<AlumnoFront>>("Alumno");
                return View(alumnos ?? new List<AlumnoFront>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los alumno desde la API");
                return Content($"Error al obtener alumnos: {ex.Message}\n\n{ex.StackTrace}");
            }
        }

        // GET: /Alumno/GetAlumnoDNI/dni
        [Authorize(Roles = "Administrador, Profesor")]
        [HttpGet]
        public async Task<IActionResult> GetAlumnoDNI(string dni)
        {
            try
            {
                AlumnoFront? alumno = await _httpClient.GetFromJsonAsync<AlumnoFront>($"Alumno/{dni}");

                if (alumno == null)
                {
                    TempData["Error"] = "Alumno inexistente o no encontrado.";
                    return RedirectToAction("Index");
                }

                return View(alumno);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener alumno por DNI");
                return RedirectToAction("GetAlumnos");
            }
        }
    }
}
