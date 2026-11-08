using Hl7.Fhir.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PractitionerDirectory.API.Mappers;
using PractitionerDirectory.API.Middleware;
using PractitionerDirectory.Repositories;
using PractitionerDirectory.Repositories.Repositories;
using PractitionerDirectory.Services;
using System.Reflection;
using ServiceMapper = PractitionerDirectory.Services.Mappers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("Practitioner-v1", new OpenApiInfo
    {
        Title = "Practitioner  API",
        Version = "v1"
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    if (File.Exists(xmlPath))
    {
        option.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    }
});

var connectionString = builder.Configuration.GetConnectionString("ApplicationDb");
builder.Services.AddDbContext<PractitionerDirectoryDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<IPractitionerRepository, PractitionerRepository>();
builder.Services.AddScoped<IPractitionerService, PractitionerService>();
builder.Services.AddScoped<ServiceMapper.IPractitionerMapper, ServiceMapper.PractitionerMapper>();
builder.Services.AddScoped<IPractitionerMapper, PractitionerMapper>();
builder.Services.AddScoped<IFhirPractitionerMapper, FhirPractitionerMapper>();
builder.Services.AddSingleton<FhirJsonSerializer>();

var app = builder.Build();

app.UseExceptionHandling();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
                  .GetRequiredService<PractitionerDirectoryDbContext>();
    await DbInitializer.UpsertAsync(db);
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(option =>
{
    option.SwaggerEndpoint("/swagger/Practitioner-v1/swagger.json", "Practitioner API - v1");
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
