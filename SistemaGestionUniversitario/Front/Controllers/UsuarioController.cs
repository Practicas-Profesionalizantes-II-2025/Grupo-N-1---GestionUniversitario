using Front.Models.Crear;
using Front.Models.Modificar;
using Front.Models.Respuestas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Front.Controllers
{
    [Authorize]
    public class UsuarioController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UsuarioController> _logger;

        public UsuarioController(IHttpClientFactory httpClientFactory, ILogger<UsuarioController> logger)
        {
            _httpClient = httpClientFactory.CreateClient("ApiPrincipal");
            _logger = logger;
        }

        // GET: /Usuario/GetUsuarios
        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public async Task<IActionResult> GetUsuarios()
        {
            try
            {
                List<UsuarioFront>? usuarios = await _httpClient.GetFromJsonAsync<List<UsuarioFront>>("Usuario");
                return View(usuarios ?? new List<UsuarioFront>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios desde la API");
                return Content($"Error al obtener usuarios: {ex.Message}\n\n{ex.StackTrace}");
            }
        }

        // GET: /Usuario/GetUsuarioDNI/dni
        [Authorize(Roles = "Administrador")]
        [HttpGet("Usuario/GetByDni/{dni}")]
        public async Task<IActionResult> GetByDni(string dni)
        {
            var usuario = await _httpClient.GetFromJsonAsync<ModificarUsuarioFront>($"Usuario/{dni}");

            if (usuario == null)
                return NotFound(new { mensaje = "Usuario no encontrado" });

            return Json(usuario);  // devuelve en JSON todos los datos del usuario
        }

        // GET: /Usuario/CreateUsuario
        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public IActionResult CreateUsuario()
        {
            return View(); // Devuelve la vista con el formulario
        }
        // GET: /Usuario/UpdateUsuario
        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public async Task<IActionResult> UpdateUsuario()
        {
            var usuarios = await _httpClient.GetFromJsonAsync<List<UsuarioFront>>("Usuario");

            ViewBag.Usuarios = usuarios ?? new List<UsuarioFront>();
            return View();
        }


        // POST: /Usuario
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<IActionResult> CreateUsuario(CrearUsuarioFront usuario)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync("Usuario", usuario);

                if (!response.IsSuccessStatusCode)
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    string mensajeError = "Error al crear usuario.";

                    try
                    {
                        using var doc = JsonDocument.Parse(errorContent);
                        var root = doc.RootElement;

                        // ✅ Caso 1: API devuelve mensaje simple { "mensaje": "..."}
                        if (root.TryGetProperty("mensaje", out var mensaje))
                        {
                            mensajeError = mensaje.GetString() ?? mensajeError;
                            var match = System.Text.RegularExpressions.Regex.Match(mensajeError, @"\(Parameter\s'([^']+)'\)");
                            if (match.Success)
                            {
                                string campo = match.Groups[1].Value;
                                mensajeError = $"El campo {campo} es inválido.";
                            }

                            ModelState.AddModelError(string.Empty, mensajeError);
                        }
                        // ✅ Caso 2: JSON de validación estándar ASP.NET { "errors": { "Password": ["..."] } }
                        else if (root.TryGetProperty("errors", out var errores))
                        {
                            foreach (var campo in errores.EnumerateObject())
                            {
                                foreach (var msg in campo.Value.EnumerateArray())
                                {
                                    ModelState.AddModelError(campo.Name, msg.GetString());
                                }
                            }
                        }
                        else
                        {
                            // si es otro formato raro, mostrar el texto crudo
                            ModelState.AddModelError(string.Empty, errorContent);
                        }
                    }
                    catch
                    {
                        // si no es JSON válido, mostrar texto tal cual
                        ModelState.AddModelError(string.Empty, errorContent);
                    }

                    return View(usuario);
                }

                TempData["Success"] = "Usuario creado correctamente.";
                return RedirectToAction("GetUsuarios");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear usuario");
                ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado.");
                return View(usuario);
            }
        }

        // PUT: /Usuario/dni
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<IActionResult> UpdateUsuario(string dni, ModificarUsuarioFront usuario)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"Usuario/{dni}", usuario);

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        ModelState.AddModelError("", "No se encontró el usuario con ese DNI.");
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        string error = await response.Content.ReadAsStringAsync();
                        ModelState.AddModelError("", $"Error de validación: {error}");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Ocurrió un error inesperado al actualizar el usuario.");
                    }

                    return View(usuario);
                }

                TempData["Success"] = "Usuario actualizado correctamente.";
                return RedirectToAction("GetUsuarios");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar usuario");
                return View(usuario);
            }
        }

        // PUT: /Usuario/actualizarPassword/dni
        [Authorize(Roles = "Administrador, Profesor, Alumno")]
        [HttpPost]
        public async Task<IActionResult> UpdatePasswordUsuario(string dni, ModificarUsuarioFront usuario)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"Usuario/actualizarPassword/{dni}", usuario);

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        ModelState.AddModelError("", "No se encontró el usuario con ese DNI.");
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        string error = await response.Content.ReadAsStringAsync();
                        ModelState.AddModelError("", $"Error de validación: {error}");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Ocurrió un error inesperado al actualizar la contraseña.");
                    }

                    return View(usuario);
                }

                TempData["Success"] = "Contraseña actualizada correctamente.";
                return RedirectToAction("GetUsuarios");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la contraseña");
                return View(usuario);
            }
        }

        // DELETE: /Usuario/dni
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<IActionResult> DeleteUsuario(string dni)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.DeleteAsync($"Usuario/{dni}");

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        TempData["Error"] = "No se encontró el usuario con ese DNI.";
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        string error = await response.Content.ReadAsStringAsync();
                        TempData["Error"] = $"Error de validación: {error}";
                    }
                    else
                    {
                        TempData["Error"] = "Ocurrió un error inesperado al eliminar el usuario.";
                    }

                    return RedirectToAction("GetUsuarios");
                }

                TempData["Success"] = "Usuario eliminado correctamente.";
                return RedirectToAction("GetUsuarios");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar usuario");
                TempData["Error"] = "Ocurrió un error al eliminar el usuario.";
                return RedirectToAction("GetUsuarios");
            }
        }
    }
}