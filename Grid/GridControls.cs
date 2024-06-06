namespace BlazorWebAppEFCore.Grid;

// State of grid filters.
public class GridControls : IOnderdeelFilters, IInstallatieOnderdeelFilters
{
    // Keep state of paging.
    public IPageHelper PageHelper { get; set; }

    public GridControls(IPageHelper pageHelper)
    {
        PageHelper = pageHelper;
    }

    // Avoid multiple concurrent requests.
    public bool Loading { get; set; }

    // Column to sort by.
    public OnderdeelFilterColumns SortColumn { get; set; }
        = OnderdeelFilterColumns.Naam;

    public InstallatieOnderdeelFilterColumns SortColumns { get; set; }
        = InstallatieOnderdeelFilterColumns.WerkerID;

    // True when sorting ascending, otherwise sort descending.
    public bool SortAscending { get; set; } = true;

    // Column filtered text is against.
    public OnderdeelFilterColumns FilterColumn { get; set; } = OnderdeelFilterColumns.Naam;

    public InstallatieOnderdeelFilterColumns FilterColumns { get; set; } = InstallatieOnderdeelFilterColumns.WerkerID;

    // Text to filter on.
    public string? FilterText { get; set; }
}
