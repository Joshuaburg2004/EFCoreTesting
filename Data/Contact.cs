using System.ComponentModel.DataAnnotations;

namespace BlazorWebAppEFCore.Data;

public class Contact
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

    public string? Comment { get; set; }
    // public int Id { get; set; }

    // [Required]
    // [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters.")]
    // public string? FirstName { get; set; }

    // [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters.")]
    // public string? LastName { get; set; }

    // [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 digits.")]
    // public string? Phone { get; set; }

    // [Required]
    // [StringLength(100, ErrorMessage = "Street cannot exceed 100 characters.")]
    // public string? Street { get; set; }

    // [Required]
    // [StringLength(50, ErrorMessage = "City cannot exceed 50 characters.")]
    // public string? City { get; set; }

    // [Required]
    // [StringLength(3, ErrorMessage = "State abbreviation cannot exceed 3 characters.")]
    // public string? State { get; set; }

    // [Required]
    // [RegularExpression(@"^\d{5}(?:[-\s]\d{4})?$", ErrorMessage = "Enter a valid zipcode in 55555 or 55555-5555 format")]
    // public string? ZipCode { get; set; }
}

// a public class Onderdelen Serienummer, Naam, Locatie, Hoeveelheid, Soort, Comment
// public class Onderdeel
// {
//     [Required]
//     [StringLength(100, ErrorMessage = "Serie nummer kan niet langer dan 100 characters zijn.")]
//     [Key]
//     public string? SerieNummer { get; set; }

//     [StringLength(100, ErrorMessage = "De naam van het onderdeel kan niet langer dan 100 characters zijn.")]
//     public string? Naam { get; set; }

//     [StringLength(25, ErrorMessage = "Locatie naam kan niet langer dan 25 characters zijn.")]
//     public string? Locatie { get; set; }

//     [Required]
//     public int Hoeveelheid { get; set; }

//     [Required]
//     [StringLength(50, ErrorMessage = "Het soort onderdeel kan niet langer zijn dan 50 characters.")]
//     public string? Soort { get; set; }

//     public string? Comment { get; set; }
// }