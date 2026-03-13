using Microsoft.Data.SqlClient;
using Dapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddTransient<SqlConnection>(_ =>
    new SqlConnection(builder.Configuration.GetConnectionString("BookshopDB")));


//Temporary connection test — remove after verifying - above builder.build()
using var testConnection = new SqlConnection(
builder.Configuration.GetConnectionString("BookshopDB"));

// Before builder.Build()
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();




try
{
    await testConnection.OpenAsync();
    Console.WriteLine("Database connection successful");
}
catch (Exception ex)
{
    Console.WriteLine($"Connection failed: {ex.Message}");
}


// Task 1: Get all Authors 

app.MapGet("/api/authors", async (SqlConnection db) =>
{
    var authors = await db.QueryAsync<Author>("SELECT * FROM Authors");
    return Results.Ok(authors);
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
