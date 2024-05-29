using Xunit;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

public class OnderwegOnderdeelTests
{
    [Fact]
    public void TestOnderwegOnderdeelValidation()
    {
        // Arrange
        var onderdeel = new OnderwegOnderdeel
        {
            SerieNummer = new string('*', 101), // Exceeds the maximum length
            WerkerNaam = new string('*', 101), // Exceeds the maximum length
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
        Assert.Equal(4, results.Count); // There should be 4 validation errors
    }
}