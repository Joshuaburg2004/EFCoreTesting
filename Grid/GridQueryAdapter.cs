using System.Diagnostics;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using BlazorWebAppEFCore.Data;

namespace BlazorWebAppEFCore.Grid;

// Creates the correct expressions to filter and sort.
public class GridQueryAdapter
{
    // Holds state of the grid.
    private readonly IContactFilters _controls;

    // Expressions for sorting
    // verander de City, Phone, Name, State, Street, ZipCode naar SerieNummer, Naam, Locatie, Hoeveelheid, Soort, Comment.
    private readonly Dictionary<ContactFilterColumns, Expression<Func<Contact, string>>> _expressions
        = new()
        {
            { ContactFilterColumns.SerieNummer, c => c != null && c.SerieNummer != null ? c.SerieNummer : string.Empty },
            { ContactFilterColumns.Naam, c => c != null && c.Naam != null ? c.Naam : string.Empty },
            { ContactFilterColumns.Locatie, c => c != null && c.Locatie != null ? c.Locatie : string.Empty },
            { ContactFilterColumns.Hoeveelheid, c => c != null && c.Hoeveelheid != null ? $"{c.Hoeveelheid}" : string.Empty },
            { ContactFilterColumns.Soort, c => c != null && c.Soort != null ? c.Soort : string.Empty },
            { ContactFilterColumns.Comment, c => c != null && c.Comment != null ? c.Comment : string.Empty }
        };

    // Queryables for filtering.
    private readonly Dictionary<ContactFilterColumns, Func<IQueryable<Contact>, IQueryable<Contact>>> _filterQueries = 
        new Dictionary<ContactFilterColumns, Func<IQueryable<Contact>, IQueryable<Contact>>>();

    // Creates a new instance of the GridQueryAdapter class.
    // controls: The IContactFilters" to use.
    public GridQueryAdapter(IContactFilters controls)
    {
        _controls = controls;

        // Set up queries serienummer, naam, locatie, hoeveelheid, soort, comment.
        _filterQueries = new()
        {
            { ContactFilterColumns.SerieNummer, cs => cs.Where(c => c != null && c.SerieNummer != null && _controls.FilterText != null ? c.SerieNummer.Contains(_controls.FilterText) : false ) },
            { ContactFilterColumns.Naam, cs => cs.Where(c => c != null && c.Naam != null && _controls.FilterText != null ? c.Naam.Contains(_controls.FilterText) : false ) },
            { ContactFilterColumns.Locatie, cs => cs.Where(c => c != null && c.Locatie != null && _controls.FilterText != null ? c.Locatie.Contains(_controls.FilterText) : false ) },
            { ContactFilterColumns.Hoeveelheid, cs => cs.Where(c => c != null && c.Hoeveelheid != null && _controls.FilterText != null ? Convert.ToString(c.Hoeveelheid).Contains(_controls.FilterText) : false ) },
            { ContactFilterColumns.Soort, cs => cs.Where(c => c != null && c.Soort != null && _controls.FilterText != null ? c.Soort.Contains(_controls.FilterText) : false ) },
            { ContactFilterColumns.Comment, cs => cs.Where(c => c != null && c.Comment != null && _controls.FilterText != null ? c.Comment.Contains(_controls.FilterText) : false ) }
        };
        // _filterQueries = new()
        // {
        //     { ContactFilterColumns.City, cs => cs.Where(c => c != null && c.City != null && _controls.FilterText != null ? c.City.Contains(_controls.FilterText) : false ) },
        //     { ContactFilterColumns.Phone, cs => cs.Where(c => c != null && c.Phone != null && _controls.FilterText != null ? c.Phone.Contains(_controls.FilterText) : false ) },
        //     { ContactFilterColumns.Name, cs => cs.Where(c => c != null && c.FirstName != null && _controls.FilterText != null ? c.FirstName.Contains(_controls.FilterText) : false ) },
        //     { ContactFilterColumns.State, cs => cs.Where(c => c != null && c.State != null && _controls.FilterText != null ? c.State.Contains(_controls.FilterText) : false ) },
        //     { ContactFilterColumns.Street, cs => cs.Where(c => c != null && c.Street != null && _controls.FilterText != null ? c.Street.Contains(_controls.FilterText) : false ) },
        //     { ContactFilterColumns.ZipCode, cs => cs.Where(c => c != null && c.ZipCode != null && _controls.FilterText != null ? c.ZipCode.Contains(_controls.FilterText) : false ) }
        // };
    }

    // Uses the query to return a count and a page.
    // query: The IQueryable{Contact} to work from.
    // Returns the resulting ICollection{Contact}.
    public async Task<ICollection<Contact>> FetchAsync(IQueryable<Contact> query)
    {
        query = FilterAndQuery(query);
        await CountAsync(query);
        var collection = await FetchPageQuery(query)
            .ToListAsync();
        _controls.PageHelper.PageItems = collection.Count;
        return collection;
    }

    // Get total filtered items count.
    // query: The IQueryable{Contact} to use.
    public async Task CountAsync(IQueryable<Contact> query)
    {
        _controls.PageHelper.TotalItemCount = await query.CountAsync();
    }

    // Build the query to bring back a single page.
    // query: The <see IQueryable{Contact} to modify.
    // Returns the new IQueryable{Contact} for a page.
    public IQueryable<Contact> FetchPageQuery(IQueryable<Contact> query)
    {
        return query
            .Skip(_controls.PageHelper.Skip)
            .Take(_controls.PageHelper.PageSize)
            .AsNoTracking();
    }

    // Builds the query.
    // root: The IQueryable{Contact} to start with.
    // Returns the resulting IQueryable{Contact} with sorts and filters applied.
    private IQueryable<Contact> FilterAndQuery(IQueryable<Contact> root)
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

        // Fix name.
        if (_controls.SortColumn == ContactFilterColumns.Naam && _controls.ShowFirstNameFirst)
        {
            sb.Append($"(first name first) ");
            expression = c => c.Naam != null ? c.Naam : string.Empty;
        }

        var sortDir = _controls.SortAscending ? "ASC" : "DESC";
        sb.Append(sortDir);

        Debug.WriteLine(sb.ToString());

        // Return the unfiltered query for total count, and the filtered for fetching.
        return _controls.SortAscending ? root.OrderBy(expression)
            : root.OrderByDescending(expression);
    }
}
