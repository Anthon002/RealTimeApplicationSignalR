using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RealTimeApplication.Infrastructure.Data.Entities;
using RealTimeApplication.Infrastructure.Models;

namespace RealTimeApplication.Operations.Handlers.Chat;

public sealed record GetUsersRequest : IRequest<BaseResponse<PaginatedData<UsersResponse>>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public sealed class GetUsersRequestHandler : IRequestHandler<GetUsersRequest, BaseResponse<PaginatedData<UsersResponse>>>
{
    private readonly AppDbContext _context;
    private readonly ILogger<GetUsersRequestHandler> _logger;

    public GetUsersRequestHandler(AppDbContext context, ILogger<GetUsersRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }
    public async Task<BaseResponse<PaginatedData<UsersResponse>>> Handle(GetUsersRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var iusers = from user in _context.AppUsers.Select(x => new { x.FirstName, x.LastName, x.UserIdentifier })
                         select new UsersResponse
                         {
                             FirstName = user.FirstName,
                             LastName = user.LastName,
                             UserIdentifier = user.UserIdentifier,
                         };

            var totalRecordsCount = await iusers.CountAsync(cancellationToken);
            var response = await iusers.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToArrayAsync(cancellationToken);
            return new BaseResponse<PaginatedData<UsersResponse>>(true, "Users retrieved successfully.", new PaginatedData<UsersResponse>(response, totalRecordsCount, request.PageNumber, request.PageSize));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetUsersRequest => Application ran into an error while trying to retrieve users.");
            return new BaseResponse<PaginatedData<UsersResponse>>(false, "An error occurred while trying to verify Chat Dm user.");
        }
    }
}
