using System.ComponentModel.DataAnnotations;

public class OnderwegOnderdeel
{
    [Required]
    [StringLength(100, ErrorMessage = "Serie nummer kan niet langer dan 100 characters zijn.")]
    [Key]
    public string? SerieNummer { get; set; }

    [StringLength(100, ErrorMessage = "De naam van de buitendienst medewerker kan niet langer dan 100 characters zijn.")]
    public string? WerkerNaam { get; set; }

    [Required]
    [Range(0, 1000000, ErrorMessage = "Moet positief zijn")]
    public int Hoeveelheid { get; set; }

    [Required]
    [StringLength(50, ErrorMessage = "Het soort onderdeel kan niet langer zijn dan 50 characters.")]
    public string? Soort { get; set; }

    public string? Comment { get; set; }
}
