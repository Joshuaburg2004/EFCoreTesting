using BlazorWebAppEFCore.Components;
using Microsoft.EntityFrameworkCore;
using BlazorWebAppEFCore.Data;
using BlazorWebAppEFCore.Grid;
using System;
using System.Linq;
using Microsoft.Extensions.Hosting;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Mvc;

using var db = new OnderdeelContext();

// Note: This sample requires the database to be created before running.
Console.WriteLine($"Database path: {db.DbPath}.");

// Create
// Console.WriteLine("Inserting a new Onderdeel");
// db.Add(new Onderdeel { Naam = "Schroeven", Comment = null, Hoeveelheid = 100, Locatie = "Arnhem", SerieNummer = "LS-99989", Soort = "Schroeven" });
// db.SaveChanges();

// Read
// Console.WriteLine("Querying for an onderdeel");
// var blog = db.Onderdelen
//     .OrderBy(b => b.SerieNummer)
//     .First();

// // Update
// Console.WriteLine("Updating the Onderdeel and changing the soort");
// blog.Soort = "Kleine onderdelen";
// db.SaveChanges();

// Delete
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register factory and configure the options
#region snippet1
builder.Services.AddDbContextFactory<OnderdeelContext>(opt =>
    opt.UseSqlite($"Data Source={nameof(OnderdeelContext.Onderdelen)}.db"));
#endregion

// Pager
builder.Services.AddScoped<IPageHelper, PageHelper>();

// Filters
builder.Services.AddScoped<IOnderdeelFilters, GridControls>();
builder.Services.AddScoped<IInstallatieOnderdeelFilters, GridControls>();

// Query adapter (applies filter to contact request)
builder.Services.AddScoped<GridQueryAdapter>();

// Service to communicate success on edit between pages
builder.Services.AddScoped<EditSuccess>();

builder.Services.AddControllers();

var app = builder.Build();

// This section sets up and seeds the database. Seeding is NOT normally
// handled this way in production. The following approach is used in this
// sample app to make the sample simpler. The app can be cloned. The
// connection string is configured. The app can be run.
await using var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateAsyncScope();
var options = scope.ServiceProvider.GetRequiredService<DbContextOptions<OnderdeelContext>>();
/*await DatabaseUtility.EnsureDbCreatedAndSeedWithCountOfAsync(options, 500);*/

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.MapControllers();

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
