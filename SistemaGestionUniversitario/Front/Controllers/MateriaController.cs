using Front.Models.Crear;
using Front.Models.Modificar;
using Front.Models.Respuestas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
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
        [HttpGet("Materia/GetMateriaJson/{nombreMateria}")]
        public async Task<IActionResult> GetMateriaJson(string nombreMateria)
        {
            try
            {
                MateriaFront? materia = await _httpClient.GetFromJsonAsync<MateriaFront>($"Materia/NombreMateria/{nombreMateria}");
                if (materia == null)
                    return NotFound(new { message = "Materia inexistente o no encontrada." });

                return Ok(materia);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener materia por nombre");
                return StatusCode(500, new { message = "Error interno al obtener materia." });
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public async Task<IActionResult> CreateMateria()
        {
            var vm = new CrearMateriaFront
            {
                Profesores = await _httpClient.GetFromJsonAsync<List<ProfesorFront>>("Profesor") ?? new(),
                DiasHorarios = await _httpClient.GetFromJsonAsync<List<DiaHorarioFront>>("DiaHorario") ?? new()
            };

            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> CreateMateria(CrearMateriaFront model)
        {
            if (!ModelState.IsValid)
            {
                // Volvemos a cargar las listas en caso de error
                model.Profesores = await _httpClient.GetFromJsonAsync<List<ProfesorFront>>("Profesor") ?? new();
                model.DiasHorarios = await _httpClient.GetFromJsonAsync<List<DiaHorarioFront>>("DiaHorario") ?? new();
                return View(model);
            }

            var materia = new
            {
                Nombre = model.Nombre,
                Anio = model.Anio,
                Modalidad = model.Modalidad,
                ProfesoresIDs = model.ProfesoresIDs,
                DiasHorariosIDs = model.DiasHorariosIDs
            };

            var response = await _httpClient.PostAsJsonAsync("Materia", materia);

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", error);

                model.Profesores = await _httpClient.GetFromJsonAsync<List<ProfesorFront>>("Profesor") ?? new();
                model.DiasHorarios = await _httpClient.GetFromJsonAsync<List<DiaHorarioFront>>("DiaHorario") ?? new();
                return View(model);
            }

            TempData["Success"] = "Materia creada correctamente.";
            return RedirectToAction("GetMaterias");
        }


        // PUT: /Materia/nombreMateria
        [Authorize(Roles = "Administrador")]
        [HttpGet("ModificarMateria")]
        public async Task<IActionResult> PutMateria()
        {
            var materias = await _httpClient.GetFromJsonAsync<List<MateriaFront>>("Materia") ?? new List<MateriaFront>();

            var vm = new ModificarMateriaFront
            {
                TodasMaterias = await _httpClient.GetFromJsonAsync<List<MateriaFront>>("Materia") ?? new(),
                Profesores = await _httpClient.GetFromJsonAsync<List<ProfesorFront>>("Profesor") ?? new(),
                DiasHorarios = await _httpClient.GetFromJsonAsync<List<DiaHorarioFront>>("DiaHorario") ?? new()
            };

            return View(vm);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<IActionResult> UpdateMateria(ModificarMateriaFront materia)
        {

            try
            {
                HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"Materia/{materia.MateriaSeleccionada}", materia);

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
                    materia.TodasMaterias = await _httpClient.GetFromJsonAsync<List<MateriaFront>>("Materia") ?? new();
                    materia.Profesores = await _httpClient.GetFromJsonAsync<List<ProfesorFront>>("Profesor") ?? new();
                    materia.DiasHorarios = await _httpClient.GetFromJsonAsync<List<DiaHorarioFront>>("DiaHorario") ?? new();
                    return View("PutMateria", materia);
                }

                TempData["Success"] = "Materia actualizada correctamente.";
                return RedirectToAction("GetMaterias");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar materia");
                return View("PutMateria", materia);
            }

        }



        // DELETE: /Materia/nombreMateria
        [Authorize(Roles = "Administrador")]
        [HttpDelete("Materia/{nombreMateria}")]
        public async Task<IActionResult> DeleteMateria(string nombreMateria)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.DeleteAsync($"Materia/{nombreMateria}");

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return NotFound("No se encontró una materia con ese nombre.");
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        string error = await response.Content.ReadAsStringAsync();
                        return BadRequest($"Error de validación: {error}");
                    }
                    else
                    {
                        return StatusCode((int)response.StatusCode, "Ocurrió un error inesperado al eliminar la materia.");
                    }
                }

                return Ok(new { message = "Materia eliminada correctamente." });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar materia");
                return StatusCode(500, "Ocurrió un error al eliminar la materia.");
            }
        }

        [Authorize(Roles = "Profesor")]
        [HttpGet("PorProfesor")]
        public async Task<IActionResult> GetMateriasPorProfesor()
        {
            try
            {
                // Obtener el DNI del profesor logueado desde los claims
                string dniProfesor = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(dniProfesor))
                {
                    _logger.LogWarning("No se encontró el DNI en los claims del usuario logueado.");
                    return Unauthorized("No se pudo identificar al profesor.");
                }

                // Llamar al endpoint del backend con el DNI del profesor
                List<MateriaFront>? materias = await _httpClient.GetFromJsonAsync<List<MateriaFront>>(
                    $"Materia/DNIProfesor/{dniProfesor}"
                );

                if (materias == null || !materias.Any())
                {
                    ViewBag.Mensaje = "No tenés materias asignadas.";
                    return View("GetMaterias", new List<MateriaFront>());
                }

                return View("GetMaterias", materias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener materias del profesor desde la API");
                return Content($"Error al obtener materias del profesor: {ex.Message}");
            }
        }
       

    }
}
