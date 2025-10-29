using Front.Metricas;
using Front.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Servicios
builder.Services.AddControllersWithViews();
// Registro de HttpClient hacia APIs
builder.Services.AddHttpClient("ApiPrincipal", client =>
{
	client.BaseAddress = new Uri("https://localhost:7068/api/");
});

// Sesion
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(o =>
{
	o.IdleTimeout = TimeSpan.FromMinutes(30); // tiempo de inactividad
	o.Cookie.HttpOnly = true;
	o.Cookie.IsEssential = true;
});

// Cookie Authentication (para roles)-
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

// App
var app = builder.Build();


app.UseStaticFiles();

app.UseRouting();
app.UseMetricServer();
app.UseMiddleware<RequestTimingMiddleware>();

// Session y Authenticator
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
// Manejo de errores y seguridad HTTPS
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();



// Ruta por defecto MVC
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Sesion}/{action=LogIn}/{id?}");

app.Run();