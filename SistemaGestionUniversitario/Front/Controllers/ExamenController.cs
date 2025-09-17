using Front.Models.Crear;
using Front.Models.Modificar;
using Front.Models.Respuestas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Front.Controllers
{
    public class ExamenController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ExamenController> _logger;

        public ExamenController(IHttpClientFactory httpClientFactory, ILogger<ExamenController> logger)
        {
            _httpClient = httpClientFactory.CreateClient("ApiPrincipal");
            _logger = logger;
        }

        // GET: /Examen/GetExamenes
        [Authorize(Roles = "Profesor")]
        [HttpGet]
        public async Task<IActionResult> GetExamenes()
        {
            try
            {
                List<ExamenFront>? examenes = await _httpClient.GetFromJsonAsync<List<ExamenFront>>("Examen");
                return View(examenes ?? new List<ExamenFront>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener exámenes desde la API");
                return Content($"Error al obtener exámenes: {ex.Message}\n\n{ex.StackTrace}");
            }
        }

        // GET: /Usuario/CreateExamen       para la vista de creación de examen
        [Authorize(Roles = "Profesor")]
        [HttpGet]
        public async Task<IActionResult> CreateExamen()
        {
            List<HorarioFront>? horario = await _httpClient.GetFromJsonAsync<List<HorarioFront>>("horario");
            ViewBag.Horario = new SelectList(horario, "Descripcion", "Descripcion");
            
            List<MateriaFront>? materia = await _httpClient.GetFromJsonAsync<List<MateriaFront>>("materia");
            ViewBag.Materias = new SelectList(materia, "Nombre", "Nombre");

            return View();
        }

        // POST: /Examen/CreateExamen
        [Authorize(Roles = "Profesor")]

        [HttpPost]
        public async Task<IActionResult> CreateExamen(CrearExamenFront examen)
        {

            var response = await _httpClient.PostAsJsonAsync("Examen", examen);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error al crear examen");
                return View();
            }
            return RedirectToAction("GetExamenes");
        }

        // PUT: /Examen/UpdateExamen/{id}
        [Authorize(Roles = "Profesor")]
        [HttpPut]
        public async Task<IActionResult> UpdateExamen(int id, ModificarExamenFront examen)
        {
            var response = await _httpClient.PutAsJsonAsync($"Examen/{id}", examen);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error al actualizar examen");
                return View("Error");
            }
            return RedirectToAction("GetExamenes");
        }

        // DELETE: /Examen/DeleteExamen/{id}
        [Authorize(Roles = "Profesor")]
        [HttpDelete]
        public async Task<IActionResult> DeleteExamen(int id)
        {
            var response = await _httpClient.DeleteAsync($"Examen/{id}");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error al eliminar examen");
                return View("Error");
            }
            return RedirectToAction("GetExamenes");
        }
    }
}
