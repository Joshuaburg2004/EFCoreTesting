using System.Diagnostics;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using BlazorWebAppEFCore.Data;
using Org.BouncyCastle.Asn1.Crmf;

namespace BlazorWebAppEFCore.Grid;

// Creates the correct expressions to filter and sort.
public class GridQueryAdapter
{
    // Holds state of the grid.
    private readonly IOnderdeelFilters _controls;

    // Expressions for sorting
    // verander de City, Phone, Name, State, Street, ZipCode naar SerieNummer, Naam, Locatie, Hoeveelheid, Soort, Comment.
    private readonly Dictionary<OnderdeelFilterColumns, Expression<Func<Onderdeel, string>>> _expressions
        = new()
        {
            { OnderdeelFilterColumns.SerieNummer, c => c != null && c.SerieNummer != null ? c.SerieNummer : string.Empty },
            { OnderdeelFilterColumns.Naam, c => c != null && c.Naam != null ? c.Naam : string.Empty },
            { OnderdeelFilterColumns.Locatie, c => c != null && c.Locatie != null ? c.Locatie : string.Empty },
            { OnderdeelFilterColumns.Hoeveelheid, c => c != null ? $"{c.Hoeveelheid}" : "0" },
            { OnderdeelFilterColumns.Soort, c => c != null && c.Soort != null ? c.Soort : string.Empty },
            { OnderdeelFilterColumns.Comment, c => c != null && c.Comment != null ? c.Comment : string.Empty }
        };

    // Queryables for filtering.
    private readonly Dictionary<OnderdeelFilterColumns, Func<IQueryable<Onderdeel>, IQueryable<Onderdeel>>> _filterQueries = 
        new Dictionary<OnderdeelFilterColumns, Func<IQueryable<Onderdeel>, IQueryable<Onderdeel>>>();

    // Creates a new instance of the GridQueryAdapter class.
    // controls: The IOnderdeelFilters" to use.
    public GridQueryAdapter(IOnderdeelFilters controls)
    {
        _controls = controls;

        // Set up queries serienummer, naam, locatie, hoeveelheid, soort, comment.
        _filterQueries = new()
        {
            { OnderdeelFilterColumns.SerieNummer, cs => cs.Where(c => c != null && c.SerieNummer != null && _controls.FilterText != null ? c.SerieNummer.Contains(_controls.FilterText) : false ) },
            { OnderdeelFilterColumns.Naam, cs => cs.Where(c => c != null && c.Naam != null && _controls.FilterText != null ? c.Naam.Contains(_controls.FilterText) : false ) },
            { OnderdeelFilterColumns.Locatie, cs => cs.Where(c => c != null && c.Locatie != null && _controls.FilterText != null ? c.Locatie.Contains(_controls.FilterText) : false ) },
            { OnderdeelFilterColumns.Hoeveelheid, cs => cs.Where(c => c != null && _controls.FilterText != null ? Convert.ToString(c.Hoeveelheid).Contains(_controls.FilterText) : false ) },
            { OnderdeelFilterColumns.Soort, cs => cs.Where(c => c != null && c.Soort != null && _controls.FilterText != null ? c.Soort.Contains(_controls.FilterText) : false ) },
            { OnderdeelFilterColumns.Comment, cs => cs.Where(c => c != null && c.Comment != null && _controls.FilterText != null ? c.Comment.Contains(_controls.FilterText) : false ) }
        };
        // _filterQueries = new()
        // {
        //     { OnderdeelFilterColumns.City, cs => cs.Where(c => c != null && c.City != null && _controls.FilterText != null ? c.City.Contains(_controls.FilterText) : false ) },
        //     { OnderdeelFilterColumns.Phone, cs => cs.Where(c => c != null && c.Phone != null && _controls.FilterText != null ? c.Phone.Contains(_controls.FilterText) : false ) },
        //     { OnderdeelFilterColumns.Name, cs => cs.Where(c => c != null && c.FirstName != null && _controls.FilterText != null ? c.FirstName.Contains(_controls.FilterText) : false ) },
        //     { OnderdeelFilterColumns.State, cs => cs.Where(c => c != null && c.State != null && _controls.FilterText != null ? c.State.Contains(_controls.FilterText) : false ) },
        //     { OnderdeelFilterColumns.Street, cs => cs.Where(c => c != null && c.Street != null && _controls.FilterText != null ? c.Street.Contains(_controls.FilterText) : false ) },
        //     { OnderdeelFilterColumns.ZipCode, cs => cs.Where(c => c != null && c.ZipCode != null && _controls.FilterText != null ? c.ZipCode.Contains(_controls.FilterText) : false ) }
        // };
    }

    // Uses the query to return a count and a page.
    // query: The IQueryable{Onderdeel} to work from.
    // Returns the resulting ICollection{Onderdeel}.
    public async Task<ICollection<Onderdeel>> FetchAsync(IQueryable<Onderdeel> query)
    {
        query = FilterAndQuery(query);
        await CountAsync(query);
        var collection = await FetchPageQuery(query)
            .ToListAsync();
        _controls.PageHelper.PageItems = collection.Count;
        return collection;
    }

    // Get total filtered items count.
    // query: The IQueryable{Onderdeel} to use.
    public async Task CountAsync(IQueryable<Onderdeel> query)
    {
        _controls.PageHelper.TotalItemCount = await query.CountAsync();
    }

    // Build the query to bring back a single page.
    // query: The <see IQueryable{Onderdeel} to modify.
    // Returns the new IQueryable{Onderdeel} for a page.
    public IQueryable<Onderdeel> FetchPageQuery(IQueryable<Onderdeel> query)
    {
        return query
            .Skip(_controls.PageHelper.Skip)
            .Take(_controls.PageHelper.PageSize)
            .AsNoTracking();
    }

    // Builds the query.
    // root: The IQueryable{Onderdeel} to start with.
    // Returns the resulting IQueryable{Onderdeel} with sorts and filters applied.
    private IQueryable<Onderdeel> FilterAndQuery(IQueryable<Onderdeel> root)
    {
        var sb = new System.Text.StringBuilder();

        // Apply a filter?
        if (!string.IsNullOrWhiteSpace(_controls.FilterText))
        {
            var filter = _filterQueries[_controls.FilterColumn];
            sb.Append($"Filter: '{_controls.FilterColumn}' ");
            root = filter(root);
        }

        // Apply the expression.
        var expression = _expressions[_controls.SortColumn];
        sb.Append($"Sort: '{_controls.SortColumn}' ");

        var sortDir = _controls.SortAscending ? "ASC" : "DESC";
        sb.Append(sortDir);

        Debug.WriteLine(sb.ToString());
        if (_controls.SortColumn == OnderdeelFilterColumns.Hoeveelheid)
            return _controls.SortAscending ? root.OrderBy(c => c != null ? c.Hoeveelheid : 0) 
                : root.OrderByDescending(c => c != null ? c.Hoeveelheid : 0);
        // Return the unfiltered query for total count, and the filtered for fetching.
        return _controls.SortAscending ? root.OrderBy(expression)
            : root.OrderByDescending(expression);
    }
}
