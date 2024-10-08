using Microsoft.EntityFrameworkCore;
using WebAPI_Docker.Data;
using WebAPI_Docker.StartUpHelpers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<WebApiDockerDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"), sqlOptions =>
    {
        sqlOptions.MigrationsAssembly(typeof(Program).Assembly.FullName);
    });
});

builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using var scope = app.Services.CreateScope();
var serviceProvider = scope.ServiceProvider;

DatabaseConfiguration.MigrateDatabase(serviceProvider);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
