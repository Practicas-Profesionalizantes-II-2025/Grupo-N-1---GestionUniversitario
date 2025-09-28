using Microsoft.AspNetCore.Mvc;

namespace Front.Controllers
{
    public class InformacionUsuarioController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AsistenciaController> _logger;

        public InformacionUsuarioController(IHttpClientFactory httpClientFactory, ILogger<AsistenciaController> logger)
        {
            _httpClient = httpClientFactory.CreateClient("ApiPrincipal");
            _logger = logger;
        }
            public IActionResult CalendarioAcademico()
            {
                return View();
            }
        

    }
}
