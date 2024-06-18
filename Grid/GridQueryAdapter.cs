using System.Diagnostics;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using BlazorWebAppEFCore.Data;
using Org.BouncyCastle.Asn1.Crmf;
using Org.BouncyCastle.Crmf;

namespace BlazorWebAppEFCore.Grid;

// Creates the correct expressions to filter and sort.
public class GridQueryAdapter
{
    // Holds state of the grid.
    private readonly IOnderdeelFilters _controls;
    private readonly IInstallatieOnderdeelFilters _controlsInstalled;

    // Expressions for sorting
    // verander de City, Phone, Name, State, Street, ZipCode naar SerieNummer, Naam, Locatie, Hoeveelheid, Soort, Comment.
    private readonly Dictionary<OnderdeelFilterColumns, Expression<Func<Onderdeel, string>>> _expressions
        = new()
        {
            { OnderdeelFilterColumns.SerieNummer, c => c != null && c.SerieNummer != null ? c.SerieNummer : string.Empty },
            { OnderdeelFilterColumns.Naam, c => c != null && c.Naam != null ? c.Naam : string.Empty },
            { OnderdeelFilterColumns.Locatie, c => c != null && c.Locatie != null ? c.Locatie : string.Empty },
            { OnderdeelFilterColumns.Hoeveelheid, c => string.Empty },
            { OnderdeelFilterColumns.Soort, c => c != null && c.Soort != null ? c.Soort : string.Empty },
            { OnderdeelFilterColumns.Comment, c => c != null && c.Comment != null ? c.Comment : string.Empty }
        };

    private readonly Dictionary<InstallatieOnderdeelFilterColumns, Expression<Func<InstallatieOnderdeel, string>>> _expressionsInstalled
        = new()
        {
            { InstallatieOnderdeelFilterColumns.SerieNummer, c => c != null && c.SerieNummer != null ? c.SerieNummer : string.Empty },
            { InstallatieOnderdeelFilterColumns.WerkerID, c => c != null && c.WerkerID != null ? c.WerkerID : string.Empty },
            { InstallatieOnderdeelFilterColumns.Adres, c => c != null && c.Adres != null ? c.Adres : string.Empty },
            { InstallatieOnderdeelFilterColumns.Soort, c => c != null && c.Soort != null ? c.Soort : string.Empty },
            { InstallatieOnderdeelFilterColumns.Comment, c => c != null && c.Comment != null ? c.Comment : string.Empty }
        };

    // Queryables for filtering.
    private readonly Dictionary<OnderdeelFilterColumns, Func<IQueryable<Onderdeel>, IQueryable<Onderdeel>>> _filterQueries = 
        new Dictionary<OnderdeelFilterColumns, Func<IQueryable<Onderdeel>, IQueryable<Onderdeel>>>();

    private readonly Dictionary<InstallatieOnderdeelFilterColumns, Func<IQueryable<InstallatieOnderdeel>, IQueryable<InstallatieOnderdeel>>> _filterQueriesInstalled =
        new Dictionary<InstallatieOnderdeelFilterColumns, Func<IQueryable<InstallatieOnderdeel>, IQueryable<InstallatieOnderdeel>>>();

    // Creates a new instance of the GridQueryAdapter class.
    // controls: The IOnderdeelFilters" to use.
    public GridQueryAdapter(IOnderdeelFilters controls, IInstallatieOnderdeelFilters controllers)
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
        _controlsInstalled = controllers;
        _filterQueriesInstalled = new()
        {
            { InstallatieOnderdeelFilterColumns.SerieNummer, cs => cs.Where(c => c != null && c.SerieNummer != null && _controls.FilterText != null ? c.SerieNummer.Contains(_controls.FilterText) : false ) },
            { InstallatieOnderdeelFilterColumns.WerkerID, cs => cs.Where(c => c != null && c.WerkerID != null && _controls.FilterText != null ? c.WerkerID.Contains(_controls.FilterText) : false ) },
            { InstallatieOnderdeelFilterColumns.Adres, cs => cs.Where(c => c != null && c.Adres != null && _controls.FilterText != null ? c.Adres.Contains(_controls.FilterText) : false ) },
            { InstallatieOnderdeelFilterColumns.Hoeveelheid, cs => cs.Where(c => c != null && _controls.FilterText != null ? Convert.ToString(c.Hoeveelheid).Contains(_controls.FilterText) : false ) },
            { InstallatieOnderdeelFilterColumns.Soort, cs => cs.Where(c => c != null && c.Soort != null && _controls.FilterText != null ? c.Soort.Contains(_controls.FilterText) : false ) },
            { InstallatieOnderdeelFilterColumns.Comment, cs => cs.Where(c => c != null && c.Comment != null && _controls.FilterText != null ? c.Comment.Contains(_controls.FilterText) : false ) }
        };
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

    public async Task<ICollection<InstallatieOnderdeel>> FetchAsync(IQueryable<InstallatieOnderdeel> query)
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

    public async Task CountAsync(IQueryable<InstallatieOnderdeel> query)
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

    public IQueryable<InstallatieOnderdeel> FetchPageQuery(IQueryable<InstallatieOnderdeel> query)
    {
        return query
            .Skip(_controlsInstalled.PageHelper.Skip)
            .Take(_controlsInstalled.PageHelper.PageSize)
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

    private IQueryable<InstallatieOnderdeel> FilterAndQuery(IQueryable<InstallatieOnderdeel> root)
    {
        var sb = new System.Text.StringBuilder();

        // Apply a filter?
        if (!string.IsNullOrWhiteSpace(_controlsInstalled.FilterText))
        {
            var filter = _filterQueriesInstalled[_controlsInstalled.FilterColumns];
            sb.Append($"Filter: '{_controlsInstalled.FilterColumns}' ");
            root = filter(root);
        }

        // Apply the expression.
        var expression = _expressionsInstalled[_controlsInstalled.SortColumns];
        sb.Append($"Sort: '{_controlsInstalled.SortColumns}' ");

        var sortDir = _controlsInstalled.SortAscending ? "ASC" : "DESC";
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
