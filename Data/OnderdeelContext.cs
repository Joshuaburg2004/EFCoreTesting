using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Data;

public class OnderdeelContext : DbContext
{
    public DbSet<Onderdeel> Onderdelen { get; set; }
    public string DbPath { get; }
    public OnderdeelContext()
    {
        var path = AppContext.BaseDirectory;
        DbPath = Path.Join(path, "OnderdelenDb.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");


}
public class Onderdeel
{

    [Required]
    [StringLength(100, ErrorMessage = "Serie nummer kan niet langer dan 100 characters zijn.")]
    [Key]
    public string? SerieNummer { get; set; }

    [StringLength(100, ErrorMessage = "De naam van het onderdeel kan niet langer dan 100 characters zijn.")]
    public string? Naam { get; set; }

    [StringLength(25, ErrorMessage = "Locatie naam kan niet langer dan 25 characters zijn.")]
    public string? Locatie { get; set; }

    [Required]
    public int Hoeveelheid { get; set; }

    [Required]
    [StringLength(50, ErrorMessage = "Het soort onderdeel kan niet langer zijn dan 50 characters.")]
    public string? Soort { get; set; }

    [Required]
    public string? Comment { get; set; }
}