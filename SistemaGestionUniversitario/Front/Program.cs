using Front.Metricas;
using Front.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Servicios existentes
builder.Services.AddControllersWithViews();

// Registro de HttpClient hacia APIs
builder.Services.AddHttpClient("ApiPrincipal", client =>
{
    client.BaseAddress = new Uri("https://localhost:7068/api/");
});

// Sesión
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(o =>
{
    o.IdleTimeout = TimeSpan.FromMinutes(30); // tiempo de inactividad
    o.Cookie.HttpOnly = true;
    o.Cookie.IsEssential = true;
});

// Cookie Authentication (para roles)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/Sesion/LogIn";
        o.AccessDeniedPath = "/Sesion/AccesoDenegado";
        o.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        o.SlidingExpiration = true;
    });

// Servicio de autenticación
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuthenticator, Authenticator>();


// servicios para Prometheus
builder.Services.AddSingleton<ActiveUserTracker>();
builder.Services.AddHostedService<ActiveUsersCleanupService>();

var app = builder.Build();

// ------------------------------
// Manejo de errores y seguridad HTTPS
// ------------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

//Middleware Prometheus
app.UseHttpMetrics();

app.UseMiddleware<ActivityTrackingMiddleware>();

// Resto de middlewares
app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

//Endpoint /metrics
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapMetrics();
});

// Ruta por defecto MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Sesion}/{action=LogIn}/{id?}");

app.Run();