// Helpers/PaginationMetadata.cs
public class PaginationMetadata
{
    public int TotalItems { get; set; }       // Total number of items
    public int Page { get; set; }            // Current page number
    public int PageSize { get; set; }        // Number of items per page
    public int TotalPages { get; set; }      // Total number of pages
}