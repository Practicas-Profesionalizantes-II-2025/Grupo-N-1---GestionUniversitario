using Front.Models.Crear;
using Front.Models.Modificar;
using Front.Models.Respuestas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
namespace Front.Controllers
{
    public class MateriaController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MateriaController> _logger;

        public MateriaController(IHttpClientFactory httpClientFactory, ILogger<MateriaController> logger)
        {
            _httpClient = httpClientFactory.CreateClient("ApiPrincipal");
            _logger = logger;
        }

        // GET: /Materia/GetMaterias
        [HttpGet]
        public async Task<IActionResult> GetMaterias()
        {
            try
            {
                List<MateriaFront>? materias = await _httpClient.GetFromJsonAsync<List<MateriaFront>>("Materia");
                return View(materias ?? new List<MateriaFront>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener materias desde la API");
                return Content($"Error al obtener materias: {ex.Message}\n\n{ex.StackTrace}");
            }
        }

        // GET: /Materia/PlanDeEstudios
        public async Task<IActionResult> PlanDeEstudios()
        {
            try
            {
                List<MateriaFront>? materias = await _httpClient.GetFromJsonAsync<List<MateriaFront>>("Materia");
                return View("PlanDeEstudios", materias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener materias desde la API");
                return Content($"Error al obtener materias: {ex.Message}\n\n{ex.StackTrace}");
            }
        }

        // GET: /Materia/GetMateriaNombre/nombreMateria
        [HttpGet("{nombreMateria}")]
        public async Task<IActionResult> GetMateriaNombre(string nombreMateria)
        {
            try
            {
                MateriaFront? materia = await _httpClient.GetFromJsonAsync<MateriaFront>($"Materia/{nombreMateria}");

                if (materia == null)
                {
                    TempData["Error"] = "Materia inexistente o no encontrada.";
                    return RedirectToAction("GetMaterias");
                }
                return View(materia);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener materia por nombre");
                return RedirectToAction("GetMaterias");
            }
        }

        // POST: /Materia/PostMateria
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<IActionResult> CreateMateria(CrearMateriaFront materia)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync("Materia", materia);

                if (!response.IsSuccessStatusCode)
                {
                    string error = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", error);
                    return View(materia);
                }

                TempData["Success"] = "Materia creado correctamente.";
                return RedirectToAction("GetMaterias");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear materia");
                return View(materia);
            }
        }

        // PUT: /Materia/nombreMateria
        [Authorize(Roles = "Administrador")]
        [HttpPut("{nombreMateria}")]
        public async Task<IActionResult> UpdateMateria(string nombreMateria, ModificarMateriaFront materia)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"Materia/{nombreMateria}", materia);

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        ModelState.AddModelError("", "No se encontró una materia con ese nombre.");
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        string error = await response.Content.ReadAsStringAsync();
                        ModelState.AddModelError("", $"Error de validación: {error}");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Ocurrió un error inesperado al actualizar la materia.");
                    }

                    return View(materia);
                }

                TempData["Success"] = "Materia actualizado correctamente.";
                return RedirectToAction("GetMaterias");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar materia");
                return View(materia);
            }
        }

        // DELETE: /Materia/nombreMateria
        [Authorize(Roles = "Administrador")]
        [HttpDelete("{nombreMateria}")]
        public async Task<IActionResult> DeleteMateria(string nombreMateria)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.DeleteAsync($"Materia/{nombreMateria}");
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        TempData["Error"] = "No se encontró una materia con ese nombre.";
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        string error = await response.Content.ReadAsStringAsync();
                        TempData["Error"] = $"Error de validación: {error}";
                    }
                    else
                    {
                        TempData["Error"] = "Ocurrió un error inesperado al eliminar la materia.";
                    }

                    return RedirectToAction("GetMaterias");
                }
                TempData["Success"] = "Materia eliminada correctamente.";
                return RedirectToAction("GetMaterias");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar materia");
                TempData["Error"] = "Ocurrió un error al eliminar la materia.";
                return RedirectToAction("GetMaterias");
            }
        }
    }
}
