using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using FondsSocial.API.Filters;
using FondsSocial.API.Middleware;
using FondsSocial.Infrastructure;
using FondsSocial.Application;
using System.Reflection;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Serilog: console pour le développement, fichier journalier pour la traçabilité des exceptions.
builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
    .WriteTo.File("logs/fonds-social-.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30));

builder.Services.AddControllers(options => options.Filters.Add<ValidationFilter>());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // include XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath)) c.IncludeXmlComments(xmlPath);
});

// Configuration: connection string expected in appsettings
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

// CORS: liste d'origines autorisées définie par environnement (appsettings.json / appsettings.Development.json),
// jamais AllowAnyOrigin. Une liste vide bloque toute requête cross-origin par défaut (fail-safe).
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(options => options.AddDefaultPolicy(p => p
    .WithOrigins(allowedOrigins)
    .AllowAnyHeader()
    .AllowAnyMethod()));

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Plus de UseStaticFiles(): les pièces justificatives (wwwroot/uploads) ne sont plus
// exposées telles quelles - accès uniquement via DemandeController.DownloadPiece,
// qui vérifie que la pièce existe réellement en base avant de servir le fichier.
app.UseRouting();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
