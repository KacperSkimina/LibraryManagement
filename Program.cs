using LibraryManagement.Data;
using LibraryManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

// --- 1. KONFIGURACJA US£UG (Services) ---
var builder = WebApplication.CreateBuilder(args);

// Konfiguracja bazy danych SQLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// Konfiguracja systemu Identity (U¿ytkownicy i Role)
builder.Services.AddDefaultIdentity<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// --- 2. KONFIGURACJA POTOKU HTTP (Middleware) ---
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// --- 3. SEKCJA SEED DANYCH (Inicjalizacja) ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();

    // TA LINIA JEST KLUCZOWA: Tworzy bazê danych i wszystkie tabele (w tym AspNetRoles)
    // jeœli plik bazy danych jest pusty lub nie istnieje.
    await context.Database.MigrateAsync();

    // A. Role i U¿ytkownicy (Identity)
    await SeedRolesAndUsers(services);

    // B. Seed Autorów
    if (!context.Authors.Any())
    {
        var authors = new List<Author>
        {
            new Author { FirstName = "Jan", LastName = "Brzechwa", BirthDate = new DateTime(1898, 8, 15) },
            new Author { FirstName = "Andrzej", LastName = "Sapkowski", BirthDate = new DateTime(1948, 6, 21) },
            new Author { FirstName = "Remigiusz", LastName = "Mróz", BirthDate = new DateTime(1987, 1, 15) },
            new Author { FirstName = "Olga", LastName = "Tokarczuk", BirthDate = new DateTime(1962, 1, 29) },
            new Author { FirstName = "Stephen", LastName = "King", BirthDate = new DateTime(1947, 9, 21) }
        };

        context.Authors.AddRange(authors);
        await context.SaveChangesAsync();
    }

// C. Seed Ksi¹¿ek (na podstawie autorów w bazie)
if (!context.Books.Any())
    {
        var dbAuthors = await context.Authors.ToListAsync();
        var books = new List<Book>();

        var brzechwa = dbAuthors.FirstOrDefault(a => a.LastName == "Brzechwa");
        var sapkowski = dbAuthors.FirstOrDefault(a => a.LastName == "Sapkowski");
        var mroz = dbAuthors.FirstOrDefault(a => a.LastName == "Mróz");
        var tokarczuk = dbAuthors.FirstOrDefault(a => a.LastName == "Tokarczuk");
        var king = dbAuthors.FirstOrDefault(a => a.LastName == "King");

        if (brzechwa != null)
            books.Add(new Book { Title = "Akademia Pana Kleksy", ISBN = "9788375174489", PageCount = 139, PublicationDate = new DateTime(1946, 1, 1), AuthorId = brzechwa.Id });

        if (sapkowski != null)
        {
            books.Add(new Book { Title = "Ostatnie ¿yczenie", ISBN = "9788375906257", PageCount = 332, PublicationDate = new DateTime(1993, 1, 1), AuthorId = sapkowski.Id });
            books.Add(new Book { Title = "Miecz przeznaczenia", ISBN = "9788375900989", PageCount = 384, PublicationDate = new DateTime(1992, 1, 1), AuthorId = sapkowski.Id });
        }

        if (mroz != null)
            books.Add(new Book { Title = "Kasacja", ISBN = "9788379762699", PageCount = 480, PublicationDate = new DateTime(2015, 1, 1), AuthorId = mroz.Id });

        if (tokarczuk != null)
            books.Add(new Book { Title = "Bieguni", ISBN = "9788308064887", PageCount = 456, PublicationDate = new DateTime(2007, 10, 5), AuthorId = tokarczuk.Id });

        if (king != null)
            books.Add(new Book { Title = "Lœnienie", ISBN = "9788381105125", PageCount = 512, PublicationDate = new DateTime(1977, 1, 28), AuthorId = king.Id });

        context.Books.AddRange(books);
        await context.SaveChangesAsync();
    }

    // D. Seed Czytelników (Patrons)
    if (!context.Patrons.Any())
    {
        var patrons = new List<Patron>
        {
            new Patron { FirstName = "Adam", LastName = "Nowak", Email = "adam.nowak@poczta.pl", PhoneNumber = "123456789" },
            new Patron { FirstName = "Anna", LastName = "Kowalska", Email = "anna.k@gmail.com", PhoneNumber = "987654321" },
            new Patron { FirstName = "Marek", LastName = "Zieliñski", Email = "marek.z@wp.pl", PhoneNumber = "555666777" }
        };

        context.Patrons.AddRange(patrons);
        await context.SaveChangesAsync();
    }
}

// --- 4. MAPOWANIE ŒCIE¯EK (Routing) ---
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// URUCHOMIENIE APLIKACJI
app.Run();

// --- 5. DEFINICJA FUNKCJI ---
async Task SeedRolesAndUsers(IServiceProvider services)
{
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

    if (!await roleManager.RoleExistsAsync("Administrator"))
        await roleManager.CreateAsync(new IdentityRole("Administrator"));

    var adminEmail = "admin@biblioteka.pl";
    var admin = await userManager.FindByEmailAsync(adminEmail);

    if (admin == null)
    {
        admin = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
        await userManager.CreateAsync(admin, "Admin123!");
        await userManager.AddToRoleAsync(admin, "Administrator");
    }
}