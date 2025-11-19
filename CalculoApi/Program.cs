using Microsoft.Data.SqlClient;
using System.Data;
using Calculo.Service.v1;
using Calculo.Service.v1.Interfaces;
using Serilog;
using Calculo.Repository.v1.Repositories.Interfaces;
using Calculo.Repository.v1.Repositories;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
  .ReadFrom.Configuration(builder.Configuration)
  .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowAngularApp",
    builder =>
    {
      builder.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
  options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
  options.SwaggerDoc("v1", new OpenApiInfo
  {
    Title = "API de Cálculo",
    Version = "v1",
    Description = "API para gerenciamento de cálculos e acordos.",
  });
});

builder.Services.AddSingleton(Log.Logger);

builder.Services.AddScoped<IAcordoService, AcordoService>();
builder.Services.AddScoped<ITituloService, TituloService>();

builder.Services.AddScoped<IAcordoRepository, AcordoRepository>();
builder.Services.AddScoped<IDevedorRepository, DevedorRepository>();
builder.Services.AddScoped<IFaixaComissaoRepository, FaixaComissaoRepository>();

builder.Services.AddScoped<IDbConnection>(sp =>
{
  var configuration = sp.GetRequiredService<IConfiguration>();
  return new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

app.UseCors("AllowAngularApp");

app.UseSwagger();
app.UseSwaggerUI(options =>
{
  options.SwaggerEndpoint("./swagger/v1/swagger.json", "API de Cálculo");
  options.RoutePrefix = string.Empty;
});

app.MapOpenApi();

app.UseSerilogRequestLogging();
//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();