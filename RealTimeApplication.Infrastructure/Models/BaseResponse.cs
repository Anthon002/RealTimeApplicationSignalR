using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealTimeApplication.Infrastructure.Models;
public sealed record BaseResponse(bool Status, string Message);
public sealed record BaseResponse<T>(bool Status, string Message, T? Value = default);

public sealed record PaginatedData<T> where T : class 
{
    public PaginatedData(IEnumerable<T> records, long totalRecordsCount, int pageNumber = 1, int pageSize = 10)
    {
        Records = records;
        TotalRecordsCount = totalRecordsCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
        CurrentRecordsCount = records.Count();
        TotalPageCount = (int)Math.Round((decimal)totalRecordsCount/pageSize);
    }
    public IEnumerable<T> Records { get; set; } = default!;
    public long TotalRecordsCount { get; set; }
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
    public int CurrentRecordsCount { get; set; }
    public int TotalPageCount { get; set; }
}

