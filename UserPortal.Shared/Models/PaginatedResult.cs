using System;
using System.Collections.Generic;

namespace UserPortal.Shared.Models;

public class PaginatedResult<T>
{
    public IEnumerable<T> Items { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalPages { get; }
    public int TotalItems { get; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public PaginatedResult(IEnumerable<T> items, int totalItems, PaginationParams parameters)
    {
        ArgumentNullException.ThrowIfNull(items, nameof(items));
        ArgumentNullException.ThrowIfNull(parameters, nameof(parameters));

        if (totalItems < 0)
            throw new ArgumentException("Total items cannot be negative", nameof(totalItems));

        Items = items;
        PageNumber = parameters.PageNumber;
        PageSize = parameters.PageSize;
        TotalItems = totalItems;
        TotalPages = (int)Math.Ceiling(totalItems / (double)parameters.PageSize);
    }

    public static PaginatedResult<T> Empty(PaginationParams parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters, nameof(parameters));
        return new PaginatedResult<T>(
            items: Array.Empty<T>(),
            totalItems: 0,
            parameters: parameters
        );
    }

    public static PaginatedResult<T> Create(IEnumerable<T> items, int totalItems, PaginationParams parameters)
    {
        return new PaginatedResult<T>(items, totalItems, parameters);
    }
}