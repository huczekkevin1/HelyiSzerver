using Scalar.AspNetCore; // <-- EZ VAN A LEGELEGÉRE, SEMMI SEM ELŐZHETI MEG!

var builder = WebApplication.CreateBuilder(args);

// OpenAPI generátor bekapcsolása (hogy látszódjanak a végpontok)
builder.Services.AddOpenApi(); 
builder.Services.AddCors();

var app = builder.Build();

// Fejlesztői környezet ellenőrzése
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

// =========================================================================
// ADATOK (A MEMÓRIÁBAN)
// =========================================================================
var felhasznalok = new List<Felhasznalo>
{
    new Felhasznalo(1, "Kovács János", "janos@email.com"),
    new Felhasznalo(2, "Nagy Anna", "anna@empty.com")
};

// =========================================================================
// HTTP ENDPOINTS (API VÉGPONTOK)
// =========================================================================

// 1. GET - Kilistázás
app.MapGet("/api/users", () => {
    return Results.Ok(felhasznalok);
});

// 2. POST - Létrehozás
app.MapPost("/api/users", (Felhasznalo ujUser) => {
    int ujId = felhasznalok.Any() ? felhasznalok.Max(u => u.Id) + 1 : 1;
    var mentendoUser = new Felhasznalo(ujId, ujUser.Nev, ujUser.Email);
    felhasznalok.Add(mentendoUser);
    return Results.Created($"/api/users/{ujId}", mentendoUser);
});

// 3. PUT - Módosítás
app.MapPut("/api/users/{id}", (int id, Felhasznalo frissitettUser) => {
    var regiUser = felhasznalok.FirstOrDefault(u => u.Id == id);
    if (regiUser == null) return Results.NotFound("Nincs ilyen felhasználó!");
    
    felhasznalok.Remove(regiUser);
    felhasznalok.Add(frissitettUser with { Id = id });
    return Results.Ok("Felhasználó frissítve!");
});

// 4. DELETE - Törlés
app.MapDelete("/api/users/{id}", (int id) => {
    var user = felhasznalok.FirstOrDefault(u => u.Id == id);
    if (user == null) return Results.NotFound("Nincs mit törölni.");
    
    felhasznalok.Remove(user);
    return Results.NoContent();
});

app.Run(); // A futtatható rész lezárása

// =========================================================================
// DEKLARÁCIÓK (Mindig a fájl legvégén!)
// =========================================================================
record Felhasznalo(int Id, string Nev, string Email);