using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Temporary connection test — remove after verifying
using var testConnection = new SqlConnection(
    builder.Configuration.GetConnectionString("BookshopDB"));

try
{
    await testConnection.OpenAsync();
    Console.WriteLine("Database connection successful");
}
catch (Exception ex)
{
    Console.WriteLine($"Connection failed: {ex.Message}");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
