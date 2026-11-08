using Microsoft.EntityFrameworkCore;
using PractitionerDirectory.Integrations;
using PractitionerDirectory.Repositories;
using PractitionerDirectory.Repositories.Repositories;
using PractitionerDirectory.Services;
using PractitionerDirectory.Services.Mappers;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

var connectionString = builder.Configuration.GetConnectionString("ApplicationDb");
builder.Services.AddDbContext<PractitionerDirectoryDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<IPractitionerRepository, PractitionerRepository>();
builder.Services.AddScoped<IPractitionerService, PractitionerService>();
builder.Services.AddScoped<IPractitionerMapper, PractitionerMapper>();

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider
                  .GetRequiredService<PractitionerDirectoryDbContext>();
    await DbInitializer.UpsertAsync(db);
}

host.Run();
