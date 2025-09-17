using Front.Models.Respuestas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Front.Controllers
{
    public class HorarioCOntroller : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HorarioCOntroller> _logger;

        public HorarioCOntroller(IHttpClientFactory httpClientFactory, ILogger<HorarioCOntroller> logger)
        {
            _httpClient = httpClientFactory.CreateClient("ApiPrincipal");
            _logger = logger;
        }


        // GET: /Horario/GetHorarios
        [Authorize(Roles = "Profesor")]
        [HttpGet]
        public async Task<IActionResult> GetHorarios()
        {
            try
            {
                var horarios = await _httpClient.GetFromJsonAsync<List<HorarioFront>>("Horario");
                return Json(horarios ?? new List<HorarioFront>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los horarios de usuario desde la API");
                return Json(new { error = ex.Message });
            }
        }
    }
}
