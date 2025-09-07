using Front.Models.Crear;
using Front.Models.Modificar;
using Front.Models.Respuestas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

    private readonly Dictionary<string, string> _nombresAmigables = new()
    {
        { "RolUsuarioDescripcion", "Rol" },
        { "Nombre", "Nombre" },
        { "Apellido", "Apellido" },
        { "Direccion", "Dirección" },
        { "Localidad", "Localidad" },
        { "NumeroTelefono", "Teléfono" },
        { "CaracteristicaTelefono", "Característica" },
        { "DNI", "Usuario (DNI)" },
        { "Password", "Contraseña" }
    };
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
        private async Task<List<SelectListItem>> ObtenerRolesParaVistaAsync()
        {
            try
            {
                // Llamamos al RolUsuarioController vía HttpClient o a un servicio compartido
                var roles = await _httpClient.GetFromJsonAsync<List<RolUsuarioFront>>("RolUsuario");
                return roles?.Select(r => new SelectListItem
                {
                    Value = r.Descripcion, // o r.Id si usás IDs
                    Text = r.Descripcion
                }).ToList() ?? new List<SelectListItem>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "No se pudieron cargar los roles");
                // Devolver lista vacía para no romper el select
                return new List<SelectListItem>();
            }
        }

        // GET: /Usuario/GetUsuarioDNI/dni
        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public async Task<IActionResult> GetUsuarioDNI(string dni)
        {
            try
            {
                UsuarioFront? usuario = await _httpClient.GetFromJsonAsync<UsuarioFront>($"Usuario/{dni}");
                
                if (usuario == null)
                {
                    TempData["Error"] = "Usuario inexistente o no encontrado.";
                    return RedirectToAction("Index");
                }

                return View(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario por DNI");
                return RedirectToAction("GetUsuarios");
            }
        }
        // GET: /Usuario/CreateUsuario        para la vista de creación de usuario
        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public async Task<IActionResult> CreateUsuario()
        {
            List<RolUsuarioFront>? rolesUsuario = await _httpClient.GetFromJsonAsync<List<RolUsuarioFront>>("RolUsuario");

            ViewBag.Roles = new SelectList(rolesUsuario, "Descripcion", "Descripcion");
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

                        if (root.TryGetProperty("mensaje", out var mensaje))
                        {
                            mensajeError = mensaje.GetString() ?? mensajeError;

                            var match = System.Text.RegularExpressions.Regex.Match(mensajeError, @"\(Parameter\s'([^']+)'\)");
                            if (match.Success)
                            {
                                string campo = match.Groups[1].Value;
                                if (_nombresAmigables.TryGetValue(campo, out var nombreAmigable))
                                    campo = nombreAmigable;

                                mensajeError = $"El campo {campo} es inválido.";
                            }

                            ModelState.AddModelError(string.Empty, mensajeError);
                        }
                        else if (root.TryGetProperty("errors", out var errores))
                        {
                            var listaCampos = new List<string>();

                            foreach (var campo in errores.EnumerateObject())
                            {
                                string nombreCampo = campo.Name;
                                if (_nombresAmigables.TryGetValue(nombreCampo, out var nombreAmigable))
                                    nombreCampo = nombreAmigable;

                                if (!listaCampos.Contains(nombreCampo))
                                    listaCampos.Add(nombreCampo);
                            }

                            if (listaCampos.Any())
                            {
                                string camposUnidos = string.Join(", ", listaCampos);
                                mensajeError = $"El campo {camposUnidos} es inválido.";
                                ModelState.AddModelError(string.Empty, mensajeError);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, errorContent);
                        }
                    }
                    catch
                    {
                        ModelState.AddModelError(string.Empty, errorContent);
                    }

                    // 🔹 Recargar roles para que el select no se vacíe y mantenga la selección
                    List<RolUsuarioFront>? rolesUsuario = await _httpClient.GetFromJsonAsync<List<RolUsuarioFront>>("RolUsuario");
                    ViewBag.Roles = new SelectList(rolesUsuario, "Descripcion", "Descripcion", usuario.RolUsuarioDescripcion);

                    return View(usuario); // asp-for mantiene la opción seleccionada
                }

                TempData["Success"] = "Usuario creado correctamente.";
                return RedirectToAction("GetUsuarios");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear usuario");

                // 🔹 Recargar roles en caso de excepción
                List<RolUsuarioFront>? rolesUsuario = await _httpClient.GetFromJsonAsync<List<RolUsuarioFront>>("RolUsuario");
                ViewBag.Roles = new SelectList(rolesUsuario, "Descripcion", "Descripcion", usuario.RolUsuarioDescripcion);

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
                        TempData["Error"] = "No se encontró el usuario con ese DNI.";
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        string error = await response.Content.ReadAsStringAsync();
                        TempData["Error"] = $"Error de validación: {error}";
                    }
                    else
                    {
                        TempData["Error"] = "Ocurrió un error inesperado al actualizar el usuario.";
                    }

                    return RedirectToAction("GetUsuarios"); // 👈 volvés al listado
                }

                TempData["Success"] = "Usuario actualizado correctamente.";
                return RedirectToAction("GetUsuarios");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar usuario");
                TempData["Error"] = "Error inesperado al actualizar el usuario.";
                return RedirectToAction("GetUsuarios");
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