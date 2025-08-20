using Front.Models.Respuestas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Front.Controllers
{
    public class DiaHorarioController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<DiaHorarioController> _logger;

        public DiaHorarioController(IHttpClientFactory httpClientFactory, ILogger<DiaHorarioController> logger)
        {
            _httpClient = httpClientFactory.CreateClient("ApiPrincipal");
            _logger = logger;
        }

        // GET: /DiaHorario/GetDiasHorarios
        [Authorize(Roles = "Administrador, Profesor")]
        [HttpGet]
        public async Task<IActionResult> GetDiasHorarios()
        {
            try
            {
                List<DiaHorarioFront>? diasHorarios = await _httpClient.GetFromJsonAsync<List<DiaHorarioFront>>("DiaHorario");
                return View(diasHorarios ?? new List<DiaHorarioFront>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los dias y horarios desde la API");
                return Content($"Error al obtener dias y horarios: {ex.Message}\n\n{ex.StackTrace}");
            }
        }

        // GET: /DiaHorario/GetDiaHorarioID/id
        [Authorize(Roles = "Administrador, Profesor")]
        [HttpGet]
        public async Task<IActionResult> GetDiaHorarioID(string id)
        {
            try
            {
                DiaHorarioFront? diaHorario = await _httpClient.GetFromJsonAsync<DiaHorarioFront>($"DiaHorario/{id}");

                if (diaHorario == null)
                {
                    TempData["Error"] = "Dia/Horario inexistente o no encontrado.";
                    return RedirectToAction("Index");
                }

                return View(diaHorario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener dia y horario por ID");
                return RedirectToAction("GetDiasHorarios");
            }
        }
    }
}
