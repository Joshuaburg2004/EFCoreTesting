namespace BlazorWebAppEFCore.Grid;

// Interface for filtering
public interface IOnderdeelFilters
{
    // The OnderdeelFilterColumns being filtered on.
    OnderdeelFilterColumns FilterColumn { get; set; }

    // Loading indicator.
    bool Loading { get; set; }

    // The text of the filter.
    string? FilterText { get; set; }

    // Paging state in PageHelper.
    IPageHelper PageHelper { get; set; }

    // Gets or sets a value indicating if the sort is ascending or descending.
    bool SortAscending { get; set; }

    // The OnderdeelFilterColumns being sorted.
    OnderdeelFilterColumns SortColumn { get; set; }
}
