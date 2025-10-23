using Front.Models.Crear;
using Front.Models.Respuestas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using Front.Metricas;

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

        // GET: /Inscripcion/GetInscripcionMateria/nombreMateria
        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public async Task<IActionResult> GetInscripcionMateria(string nombreMateria)
        {
            try
            {
                // Todas las materias para el listado
                ViewBag.Materias = await _httpClient.GetFromJsonAsync<List<MateriaFront>>("Materia") ?? new List<MateriaFront>();

                // Guardo la materia seleccionada (puede ser null si recién entra a la vista)
                ViewBag.SelectedMateria = nombreMateria;

                // Traer inscripciones de la materia seleccionada
                List<InscripcionFront>? inscripciones = new List<InscripcionFront>();
                if (!string.IsNullOrEmpty(nombreMateria))
                {
                    inscripciones = await _httpClient.GetFromJsonAsync<List<InscripcionFront>>($"Inscripcion/PorMateria/{nombreMateria}");
                }

                return View(inscripciones);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener inscripciones por nombre de materia");
                TempData["Error"] = "Ocurrió un error al buscar inscripciones. Intente nuevamente.";
                return RedirectToAction("GetInscripcionMateria");
            }
        }

        // GET: /Inscripcion/GetInscripcionMateria/nombreMateria
        [Authorize(Roles = "Profesor")]
        [HttpGet]
        public async Task<IActionResult> GetInscripcionesAsistencia(string nombreMateria)
        {
            try
            {
                // Profesor Logueado
                string? dniUsuarioLogueado = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Todas las materias para el listado
                ViewBag.Materias = await _httpClient.GetFromJsonAsync<List<MateriaFront>>($"Materia/DNIProfesor/{dniUsuarioLogueado}") ?? new List<MateriaFront>();

                // Guardo la materia seleccionada (puede ser null si recién entra a la vista)
                ViewBag.SelectedMateria = nombreMateria;

                // Traer inscripciones de la materia seleccionada
                List<InscripcionFront>? inscripciones = new List<InscripcionFront>();
                if (!string.IsNullOrEmpty(nombreMateria))
                {
                    inscripciones = await _httpClient.GetFromJsonAsync<List<InscripcionFront>>($"Inscripcion/PorMateria/{nombreMateria}");
                }

                return View(inscripciones);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener inscripciones por nombre de materia");
                TempData["Error"] = "Ocurrió un error al buscar inscripciones. Intente nuevamente.";
                return View(new List<InscripcionFront>());
            }
        }

        // GET: /Inscripcion/GetInscripcionesDNI/DNI
        [Authorize(Roles = "Alumno")]
        [HttpGet]
        public async Task<IActionResult> GetInscripcionesDNI()
        {
            try
            {
                // DNI Usuario Logueado
                string? dniUsuarioLogueado = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Todas las materias
                List<MateriaFront> materias = await _httpClient.GetFromJsonAsync<List<MateriaFront>>("Materia") ?? new List<MateriaFront>();

                // Inscripciones del alumno
                List<InscripcionFront> inscripciones = await _httpClient.GetFromJsonAsync<List<InscripcionFront>>($"Inscripcion/PorDNI/{dniUsuarioLogueado}") ?? new List<InscripcionFront>();

                // Lista combinada
                List<InscripcionMateriaFront> inscripcionesMateria = materias.Select(m => new InscripcionMateriaFront
                {
                    IdMateria = m.ID,
                    Nombre = m.Nombre,
                    Anio = m.Anio,
                    Modalidad = m.Modalidad,
                    NombresProfesores = m.NombresProfesores,
                    DescripcionDiasHorarios = m.DescripcionDiasHorarios,
                    EstaInscripto = inscripciones.Any(i => i.IdMateria == m.ID)
                }).ToList();

                return View(inscripcionesMateria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener inscripciones");
                TempData["Error"] = "Ocurrió un error al cargar las inscripciones.";
                return RedirectToAction("Index", "Home");
            }
        }


        // POST: /Inscripcion
        [Authorize(Roles = "Alumno")]
        [HttpPost]
        public async Task<IActionResult> CreateInscripcion(string nombreMateria)
        {
            try
            {
                // DNI Usuario Logueado
                string? dniUsuarioLogueado = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                CrearInscripcionFront inscripcion = new CrearInscripcionFront()
                {
                    DNIAlumno = dniUsuarioLogueado,
                    NombreMateria = nombreMateria,
                };

                HttpResponseMessage response = await _httpClient.PostAsJsonAsync("Inscripcion", inscripcion);

                if (!response.IsSuccessStatusCode)
                {
                    string error = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", error);
                    return View("GetInscripcionesDNI");
                }

                TempData["Success"] = "Inscripcion dada de alta correctamente.";

                // Incrementar métrica Prometheus
                DefinicionesMetricas.InscripcionesCounter.Inc();

                return RedirectToAction("GetInscripcionesDNI");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al dar de alta inscripcion.");
                return View("GetInscripcionesDNI");
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

                    return RedirectToAction("GetInscripcionMateria", new { nombreMateria });
                }

                TempData["Success"] = "Inscripcion dada de baja correctamente.";
                return RedirectToAction("GetInscripcionMateria", new { nombreMateria });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al dar de baja la inscripcion");
                TempData["Error"] = "Ocurrió un error al dar de baja la inscripcion.";
                return RedirectToAction("GetInscripcionMateria", new { nombreMateria });
            }
        }
    }
}
