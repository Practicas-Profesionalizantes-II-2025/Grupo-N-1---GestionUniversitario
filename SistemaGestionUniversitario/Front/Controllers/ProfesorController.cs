using Front.Models.Respuestas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Front.Controllers
{
    public class ProfesorController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProfesorController> _logger;

        public ProfesorController(IHttpClientFactory httpClientFactory, ILogger<ProfesorController> logger)
        {
            _httpClient = httpClientFactory.CreateClient("ApiPrincipal");
            _logger = logger;
        }

        // GET: /Profesor/GetProfesores
        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public async Task<IActionResult> GetProfesores()
        {
            try
            {
                List<ProfesorFront>? profesores = await _httpClient.GetFromJsonAsync<List<ProfesorFront>>("Profesor");
                return View(profesores ?? new List<ProfesorFront>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los profesores desde la API");
                return Content($"Error al obtener profesores: {ex.Message}\n\n{ex.StackTrace}");
            }
        }

        // GET: /Profesor/GetProfesorDNI/dni
        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public async Task<IActionResult> GetProfesorDNI(string dni)
        {
            try
            {
                ProfesorFront? profesor = await _httpClient.GetFromJsonAsync<ProfesorFront>($"Profesor/{dni}");

                if (profesor == null)
                {
                    TempData["Error"] = "Profesor inexistente o no encontrado.";
                    return RedirectToAction("Index");
                }

                return View(profesor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener profesor por DNI");
                return RedirectToAction("GetProfesores");
            }
        }
    }
}
