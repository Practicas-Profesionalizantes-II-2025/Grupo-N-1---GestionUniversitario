using Front.Models.Crear;
using Front.Models.Respuestas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Front.Controllers
{
    public class InscripcionController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<InscripcionController> _logger;

        public InscripcionController(IHttpClientFactory httpClientFactory, ILogger<InscripcionController> logger)
        {
            _httpClient = httpClientFactory.CreateClient("ApiPrincipal");
            _logger = logger;
        }

        // GET: /Inscripcion/GetInscripcionDNI/DNI
        [Authorize(Roles = "Administrador, Profesor")]
        [HttpGet]
        public async Task<IActionResult> GetInscripcionMateria(string nombreMateria)
        {
            try
            {
                List<InscripcionFront>? inscripciones = await _httpClient.GetFromJsonAsync<List<InscripcionFront>>($"Inscripcion/PorMateria/{nombreMateria}");

                if (inscripciones == null)
                {
                    TempData["Error"] = "Inscripcion inexistente o no encontrada.";
                    return RedirectToAction("Index");
                }

                return View(inscripciones);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener inscripciones por nombre de materia");
                TempData["Error"] = "Ocurrió un error al buscar inscripciones. Intente nuevamente.";
                return RedirectToAction("Index");
            }
        }

        // GET: /Inscripcion/GetInscripcionDNI/DNI
        [Authorize(Roles = "Alumno")]
        [HttpGet]
        public async Task<IActionResult> GetInscripcionDNI(string dni)
        {
            try
            {
                List<InscripcionFront>? inscripciones = await _httpClient.GetFromJsonAsync<List<InscripcionFront>>($"Inscripcion/PorDNI/{dni}");

                if (inscripciones == null)
                {
                    TempData["Error"] = "Inscripcion inexistente o no encontrada.";
                    return RedirectToAction("Index");
                }

                return View(inscripciones);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener inscripciones por DNI");
                TempData["Error"] = "Ocurrió un error al buscar inscripciones. Intente nuevamente.";
                return RedirectToAction("Index");
            }
        }

        // POST: /Inscripcion
        [Authorize(Roles = "Alumno")]
        [HttpPost]
        public async Task<IActionResult> CreateInscripcion(CrearInscripcionFront inscripcion)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync("Inscripcion", inscripcion);

                if (!response.IsSuccessStatusCode)
                {
                    string error = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", error);
                    return View("Index");
                }

                TempData["Success"] = "Inscripcion dada de alta correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al dar de alta inscripcion.");
                return View("Index");
            }
        }

        // DELETE: /Inscripcion/nombreMateria/dni
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<IActionResult> DeleteInscripcion(string nombreMateria, string dni)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.DeleteAsync($"Inscripcion/{nombreMateria}/{dni}");

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        TempData["Error"] = "No se encontró la inscripcion asociada a la materia y DNI ingresados.";
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        string error = await response.Content.ReadAsStringAsync();
                        TempData["Error"] = $"Error de validación: {error}";
                    }
                    else
                    {
                        TempData["Error"] = "Ocurrió un error inesperado al dar de baja la inscripcion.";
                    }

                    return RedirectToAction("GetInscripcionMateria");
                }

                TempData["Success"] = "Inscripcion dada de baja correctamente.";
                return RedirectToAction("GetInscripcionMateria");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al dar de baja la inscripcion");
                TempData["Error"] = "Ocurrió un error al dar de baja la inscripcion.";
                return RedirectToAction("GetInscripcionMateria");
            }
        }
    }
}
