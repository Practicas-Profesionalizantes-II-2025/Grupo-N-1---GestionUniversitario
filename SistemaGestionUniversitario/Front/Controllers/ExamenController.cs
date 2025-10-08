using Front.Models.Crear;
using Front.Models.Modificar;
using Front.Models.Respuestas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
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
               List<HorarioFront>? horarios = await _httpClient.GetFromJsonAsync<List<HorarioFront>>("horario");
                ViewBag.Horarios = new SelectList(horarios ?? new List<HorarioFront>(), "Descripcion", "Descripcion");

                string dniProfesor = User.FindFirstValue(ClaimTypes.NameIdentifier);

                List<MateriaFront>? materiasDelProfesor = await _httpClient
                    .GetFromJsonAsync<List<MateriaFront>>($"Materia/DNIProfesor/{dniProfesor}");

                ViewBag.Materias = new SelectList(materiasDelProfesor ?? new List<MateriaFront>(), "Nombre", "Nombre");

                List<ExamenFront>? examenes = await _httpClient.GetFromJsonAsync<List<ExamenFront>>("Examen");

                if (materiasDelProfesor != null && examenes != null)
                {
                    var nombresMateriasProfesor = materiasDelProfesor.Select(m => m.Nombre).ToHashSet();
                    examenes = examenes.Where(e => nombresMateriasProfesor.Contains(e.NombreMateria)).ToList();
                }

                return View(examenes ?? new List<ExamenFront>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener exámenes desde la API");
                return Content($"Error al obtener exámenes: {ex.Message}\n\n{ex.StackTrace}");
            }
        }

        // GET: /Examen/GetAlumnosPorMateria/nombreMateria
        [Authorize(Roles = "Profesor")]
        [HttpGet]
        public async Task<IActionResult> GetAlumnosPorMateria(string nombreMateria)
        {
            try
            {
                var alumnosInscriptos = await _httpClient.GetFromJsonAsync<List<InscripcionFront>>(
                    $"Inscripcion/PorMateria/{nombreMateria}");

                return Json(alumnosInscriptos ?? new List<InscripcionFront>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener inscripciones");
                return BadRequest(new { mensaje = "Error al traer alumnos inscriptos" });
            }
        }

        // GET: /Usuario/CreateExamen       para la vista de creación de examen
        [Authorize(Roles = "Profesor")]
        [HttpGet]
        public async Task<IActionResult> CreateExamen()
        {
            List<HorarioFront>? horario = await _httpClient.GetFromJsonAsync<List<HorarioFront>>("horario");
            ViewBag.Horario = new SelectList(horario, "Descripcion", "Descripcion");

            string dniProfesor = User.FindFirstValue(ClaimTypes.NameIdentifier);

            List<MateriaFront>? materiasDelProfesor = await _httpClient
                .GetFromJsonAsync<List<MateriaFront>>($"Materia/DNIProfesor/{dniProfesor}");

            ViewBag.Materias = new SelectList(materiasDelProfesor ?? new List<MateriaFront>(), "Nombre", "Nombre");

            return View();
        }
        [Authorize(Roles = "Profesor")]
        [HttpPost]
        public async Task<IActionResult> CreateExamen(CrearExamenFront examen, string DescripcionDiaHorario)
        {
            try
            {
                // 🔹 Lista para acumular errores
                var errores = new List<string>();

                if (string.IsNullOrWhiteSpace(examen.Tipo))
                    errores.Add("Tipo");
                if (string.IsNullOrWhiteSpace(examen.NombreMateria))
                    errores.Add("Materia");
                if (string.IsNullOrWhiteSpace(DescripcionDiaHorario))
                    errores.Add("Horario");
                if (examen.Fecha == default)
                    errores.Add("Fecha");

                if (errores.Any())
                {
                    string campos = string.Join(", ", errores);
                    ModelState.AddModelError(string.Empty, $"Los campos {campos} son obligatorios.");

                    // 🔹 Recargar selects
                    ViewBag.Horario = new SelectList(await _httpClient.GetFromJsonAsync<List<HorarioFront>>("horario"), "ID", "Descripcion");
                    ViewBag.Materias = new SelectList(await _httpClient.GetFromJsonAsync<List<MateriaFront>>("materia"), "Nombre", "Nombre");

                    return View(examen);
                }

                // Combinar día de la semana con el horario seleccionado
                examen.DescripcionDiaHorario = $"{examen.Fecha.ToString("dddd", new CultureInfo("es-ES"))} {DescripcionDiaHorario}";

                HttpResponseMessage response = await _httpClient.PostAsJsonAsync("Examen", examen);

                if (!response.IsSuccessStatusCode)
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, "Error al crear examen: " + errorContent);

                    ViewBag.Horario = new SelectList(await _httpClient.GetFromJsonAsync<List<HorarioFront>>("horario"), "ID", "Descripcion");
                    ViewBag.Materias = new SelectList(await _httpClient.GetFromJsonAsync<List<MateriaFront>>("materia"), "Nombre", "Nombre");

                    return View(examen);
                }

                TempData["Success"] = "Examen creado correctamente.";
                return RedirectToAction("GetExamenes");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear examen");

                ViewBag.Horario = new SelectList(await _httpClient.GetFromJsonAsync<List<HorarioFront>>("horario"), "ID", "Descripcion");
                ViewBag.Materias = new SelectList(await _httpClient.GetFromJsonAsync<List<MateriaFront>>("materia"), "Nombre", "Nombre");

                ModelState.AddModelError(string.Empty, ex.Message);
                return View(examen);
            }
        }

        // POST: /Examen/UpdateExamen/{id}
        [Authorize(Roles = "Profesor")]
        [HttpPost]
        public async Task<IActionResult> DeleteExamen(string nombreMateria, string descripcionDiaHorario, DateTime fecha)
        {
            try
            {
                string fechaStr = fecha.ToString("yyyy-MM-dd");

                HttpResponseMessage response = await _httpClient.DeleteAsync(
                    $"Examen/{nombreMateria}/{descripcionDiaHorario}?fecha={fechaStr}"
                );

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        TempData["Error"] = "No se encontró el examen a eliminar.";
                    else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                        TempData["Error"] = "Error al eliminar examen.";
                    else
                        TempData["Error"] = "Ocurrió un error inesperado al eliminar el examen.";

                    return RedirectToAction("GetExamenes");
                }

                TempData["Success"] = "Examen eliminado correctamente.";
                return RedirectToAction("GetExamenes");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar examen");
                TempData["Error"] = "Ocurrió un error al eliminar el examen.";
                return RedirectToAction("GetExamenes");
            }
        }
        [Authorize(Roles = "Profesor")]
        [HttpPost]
        public async Task<IActionResult> UpdateExamen(ModificarExamenFront examen)
        {
            try
            {
                var errores = new List<string>();

                if (string.IsNullOrWhiteSpace(examen.Tipo)) errores.Add("Tipo");
                if (string.IsNullOrWhiteSpace(examen.NombreMateria)) errores.Add("Materia");
                if (string.IsNullOrWhiteSpace(examen.DescripcionDiaHorario)) errores.Add("Nuevo Horario");
                if (examen.Fecha == default) errores.Add("Fecha");

                if (errores.Any())
                {
                    ModelState.AddModelError(string.Empty,
                        $"Los campos {string.Join(", ", errores)} son obligatorios.");

                    ViewBag.Horario = new SelectList(await _httpClient.GetFromJsonAsync<List<HorarioFront>>("horario"),
                                                     "Descripcion", "Descripcion");
                    ViewBag.Materias = new SelectList(await _httpClient.GetFromJsonAsync<List<MateriaFront>>("materia"),
                                                      "Nombre", "Nombre");
                    if (errores.Any())
                    {
                        TempData["Error"] = $"Los campos {string.Join(", ", errores)} son obligatorios.";
                        return RedirectToAction("GetExamenes");
                    }
                }

                // Combinar día con el nuevo horario
                examen.DescripcionDiaHorario =$"{examen.Fecha.ToString("dddd", new CultureInfo("es-ES"))} {examen.DescripcionDiaHorario}";
               
                HttpResponseMessage response = await _httpClient.PutAsJsonAsync("Examen", examen);


                if (!response.IsSuccessStatusCode)
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, "Error al actualizar examen: " + errorContent);

                    ViewBag.Horario = new SelectList(await _httpClient.GetFromJsonAsync<List<HorarioFront>>("horario"),
                                                     "Descripcion", "Descripcion");
                    ViewBag.Materias = new SelectList(await _httpClient.GetFromJsonAsync<List<MateriaFront>>("materia"),
                                                      "Nombre", "Nombre");
                    return View(examen);
                }

                TempData["Success"] = "Examen actualizado correctamente.";
                return RedirectToAction("GetExamenes");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar examen");

                ViewBag.Horario = new SelectList(await _httpClient.GetFromJsonAsync<List<HorarioFront>>("horario"),
                                                 "Descripcion", "Descripcion");
                ViewBag.Materias = new SelectList(await _httpClient.GetFromJsonAsync<List<MateriaFront>>("materia"),
                                                  "Nombre", "Nombre");

                ModelState.AddModelError(string.Empty, ex.Message);
                return View(examen);
            }
        }
        
    }
}