using Front.Models.Respuestas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Front.Controllers
{
    public class RolUsuarioController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<RolUsuarioController> _logger;

        public RolUsuarioController(IHttpClientFactory httpClientFactory, ILogger<RolUsuarioController> logger)
        {
            _httpClient = httpClientFactory.CreateClient("ApiPrincipal");
            _logger = logger;
        }

        // GET: /RolUsuario/GetRolesUsuario
        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public async Task<IActionResult> GetRolesUsuario()
        {
            try
            {
                var rolesUsuario = await _httpClient.GetFromJsonAsync<List<RolUsuarioFront>>("RolUsuario");
                return Json(rolesUsuario ?? new List<RolUsuarioFront>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los roles de usuario desde la API");
                return Json(new { error = ex.Message });
            }
        }

    }
}