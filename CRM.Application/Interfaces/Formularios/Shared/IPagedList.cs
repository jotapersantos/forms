using System;
using System.Collections.Generic;

namespace CRM.Application.Interfaces;

public interface IPagedList
{
    public int TotalResult { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public string Query { get; set; }
    public Dictionary<string, string> Querys { get; set; }

    public int StartPage { get; set; }
    public int EndPage { get; set; }
    public string ActionName { get; set; }
    public string ControllerName { get; set; }
    public double TotalPages => Math.Ceiling((double)TotalResult / PageSize);
}
