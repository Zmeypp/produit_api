using Microsoft.EntityFrameworkCore;
using ProduitsAPI;

var builder = WebApplication.CreateBuilder(args);

// Configuration de la base de données
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=produitsservice.db"));

// Configuration de Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Activation de Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Création automatique de la base de données
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

// CRUD Routes
app.MapGet("/produits", async (AppDbContext db) =>
    await db.Produits.ToListAsync());

app.MapGet("/produits/{id}", async (int id, AppDbContext db) =>
    await db.Produits.FindAsync(id) is Produit produit ? Results.Ok(produit) : Results.NotFound());

app.MapPost("/produits", async (AppDbContext db, Produit produit) =>
{
    db.Produits.Add(produit);
    await db.SaveChangesAsync();
    return Results.Created($"/produits/{produit.Id}", produit);
});

app.MapPut("/produits/{id}", async (int id, AppDbContext db, Produit inputProduct) =>
{
    var product = await db.Produits.FindAsync(id);
    if (product == null) return Results.NotFound();

    product.Id = inputProduct.Id;
    product.Libelle = inputProduct.Libelle;
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/produits/{id}", async (int id, AppDbContext db) =>
{
    var product = await db.Produits.FindAsync(id);
    if (product == null) return Results.NotFound();

    db.Produits.Remove(product);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();
