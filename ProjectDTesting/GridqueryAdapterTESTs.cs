using Xunit;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BlazorWebAppEFCore.Grid;
using BlazorWebAppEFCore.Data;

public class GridQueryAdapterTests
{
    [Fact]
    public async Task FetchAsync_ReturnsCorrectCollection()
    {
        // Arrange
        var mockControls = new Mock<IOnderdeelFilters>();
        mockControls.Setup(c => c.PageHelper).Returns(new PageHelper());
        var mockControllers = new Mock<IInstallatieOnderdeelFilters>();
        mockControllers.Setup(c => c.PageHelper).Returns(new PageHelper());
        var adapter = new GridQueryAdapter(mockControls.Object, mockControllers.Object);
        var query = new List<Onderdeel>().AsQueryable();

        // Act
        var result = await adapter.FetchAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<List<Onderdeel>>(result);
    }

    [Fact]
    public async Task CountAsync_SetsTotalItemCount()
    {
        // Arrange
        var pageHelper = new PageHelper();
        var mockControls = new Mock<IOnderdeelFilters>();
        mockControls.Setup(c => c.PageHelper).Returns(pageHelper);
        var mockControllers = new Mock<IInstallatieOnderdeelFilters>();
        mockControllers.Setup(c => c.PageHelper).Returns(new PageHelper());
        var adapter = new GridQueryAdapter(mockControls.Object, mockControllers.Object);
        var query = new List<Onderdeel> { new Onderdeel(), new Onderdeel() }.AsQueryable();

        // Act
        await adapter.CountAsync(query);

        // Assert
        Assert.Equal(2, pageHelper.TotalItemCount);
    }

    [Fact]
    public void FetchPageQuery_ReturnsCorrectQuery()
    {
        // Arrange
        var pageHelper = new PageHelper { Skip = 1, PageSize = 1 };
        var mockControls = new Mock<IOnderdeelFilters>();
        mockControls.Setup(c => c.PageHelper).Returns(pageHelper);
        var mockControllers = new Mock<IInstallatieOnderdeelFilters>();
        mockControllers.Setup(c => c.PageHelper).Returns(new PageHelper());
        var adapter = new GridQueryAdapter(mockControls.Object, mockControllers.Object);
        var query = new List<Onderdeel> { new Onderdeel(), new Onderdeel() }.AsQueryable();

        // Act
        var result = adapter.FetchPageQuery(query);

        // Assert
        Assert.Equal(1, result.Count());
    }
}