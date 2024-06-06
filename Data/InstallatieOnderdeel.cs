using System.ComponentModel.DataAnnotations;

public class InstallatieOnderdeel
{
    [Required]
    [StringLength(100, ErrorMessage = "Serie nummer kan niet langer dan 100 characters zijn.")]
    [Key]
    public string? SerieNummer { get; set; }
    [Required]
    [StringLength(100, ErrorMessage = "De naam van de buitendienst medewerker kan niet langer dan 100 characters zijn.")]
    public string? WerkerID { get; set; }
    [Required]
    public string? Adres { get; set; }
    [Required]
    public int Hoeveelheid { get; set; }

    [Required]
    [StringLength(50, ErrorMessage = "Het soort onderdeel kan niet langer zijn dan 50 characters.")]
    public string? Soort { get; set; }

    public string? Comment { get; set; }
}
