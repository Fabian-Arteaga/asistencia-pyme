using AsistenciaPyme.Infrastructure;
using AsistenciaPyme.Application;

var builder = WebApplication.CreateBuilder(args);

// Controladores
builder.Services.AddControllers();

// Generación del documento OpenAPI
builder.Services.AddOpenApi();

// Capas de la aplicación
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// OpenAPI y Swagger solamente en desarrollo
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "AsistenciaPyme API v1"
        );
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();