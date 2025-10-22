using System;
using System.Collections.Generic;
using CRM.Application.Interfaces;

namespace CRM.Application.DTOs.Shared;

public class PagedResultDto<T> : IPagedList where T : class
{
    public IEnumerable<T> List { get; set; } = new List<T>();
    public int TotalResult { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public string Query { get; set; }
    public Dictionary<string, string> Querys { get; set; } = new Dictionary<string, string>();
    public int StartPage { get; set; }
    public int EndPage { get; set; }
    public string ActionName { get; set; }
    public string ControllerName { get; set; }
    public int TotalPages { get; set; }

    public PagedResultDto(int totalResult, int pageIndex, int pageSize = 100)
    {
        if (pageSize < 1)
        {
            pageSize = 1;
        }

        if (pageSize > 100)
        {
            pageSize = 100;
        }

        int totalPages = (int)Math.Ceiling(totalResult / (decimal)pageSize);
        int currentPage = pageIndex;

        int startPage = currentPage - 5;
        int endPage = currentPage + 4;

        if (startPage <= 0)
        {
            endPage -= startPage - 1;
            startPage = 1;
        }

        if (endPage > totalPages)
        {
            endPage = totalPages;
            if (endPage > 10)
            {
                startPage = endPage - 9;
            }
        }

        if (currentPage <= 0)
        {
            currentPage = startPage;
        }

        if (currentPage > totalPages)
        {
            currentPage = endPage;
        }

        if (totalPages <= 0)
        {
            currentPage = 1;
        }

        TotalResult = totalResult;
        PageIndex = currentPage;
        PageSize = pageSize;
        TotalPages = totalPages;
        StartPage = startPage;
        EndPage = endPage;
    }
}
