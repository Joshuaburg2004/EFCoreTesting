using System.ComponentModel.DataAnnotations;

public class Onderdeel
{
    [Key]
    public int OnderdeelID { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "Serie nummer kan niet langer dan 100 characters zijn.")]
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

    public string? Comment { get; set; }
}
