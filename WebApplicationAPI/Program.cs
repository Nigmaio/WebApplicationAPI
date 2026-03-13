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

// Task 2: Create a new Author
app.MapPost("/api/authors", async (Author author, SqlConnection db) =>
{
    var sql = "INSERT INTO Authors (AuthorId, FirstName, LastName, Country) VALUES (@AuthorId, @FirstName, @LastName, @Country); SELECT SCOPE_IDENTITY();";
    var newId = await db.ExecuteScalarAsync<int>(sql, author);
    author.AuthorId = newId;
    return Results.Created($"/api/authors/{newId}", author);
});

// Task 3: Delete an Author
app.MapDelete("/api/authors/{id}", async (int id, SqlConnection db) =>
{
    var existing = await db.QuerySingleOrDefaultAsync<Author>(
        "SELECT * FROM Authors WHERE AuthorId = @Id", new { Id = id });

    if (existing is null)
        return Results.NotFound($"Author with ID {id} not found.");

    await db.ExecuteAsync("DELETE FROM Authors WHERE AuthorId = @Id", new { Id = id });

    return Results.NoContent();
});

// Task 4: Get a single Author by ID
app.MapGet("/api/authors/{id}", async (int id, SqlConnection db) =>
{
    var author = await db.QuerySingleOrDefaultAsync<Author>(
        "SELECT * FROM Authors WHERE AuthorId = @Id", new { Id = id });

    return author is null
        ? Results.NotFound($"Author with ID {id} not found.")
        : Results.Ok(author);
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
