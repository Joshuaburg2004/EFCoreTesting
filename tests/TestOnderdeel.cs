using Xunit;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

public class OnderdeelTests
{
    [Fact]
    public void TestOnderdeelValidation()
    {
        // Arrange
        var onderdeel = new Onderdeel
        {
            SerieNummer = new string('*', 101), // Exceeds the maximum length
            Naam = new string('*', 101), // Exceeds the maximum length
            Locatie = new string('*', 26), // Exceeds the maximum length
            Hoeveelheid = -1, // Negative quantity
            Soort = new string('*', 51), // Exceeds the maximum length
            Comment = null // Comment can be null
        };

        var context = new ValidationContext(onderdeel);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(onderdeel, context, results, true);

        // Assert
        Assert.False(isValid);
        Assert.Equal(5, results.Count); // There should be 5 validation errors
    }
}

// Path: ProjectD/EFCoreTesting/Data/Onderdeel.cs