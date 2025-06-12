using Azure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RealTimeApplication.Infrastructure.Data.Entities;
using RealTimeApplication.Infrastructure.Models;

namespace RealTimeApplication.Operations.Handlers.Chat;

public sealed record GetMessagesRequest : IRequest<BaseResponse<PaginatedData<MessageResponse>>>
{
    internal string Token { get; set; } = default!;
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

public sealed class GetMessagesRequestHandler : IRequestHandler<GetMessagesRequest, BaseResponse<PaginatedData<MessageResponse>>>
{
    private readonly AppDbContext _context;
    private readonly ILogger<GetMessagesRequestHandler> _logger;
    public GetMessagesRequestHandler(AppDbContext context, ILogger<GetMessagesRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<BaseResponse<PaginatedData<MessageResponse>>> Handle(GetMessagesRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var messages = (from message in _context.Messages.Where(x => x.ChatToken == request.Token)
                            from sender in _context.AppUsers.Where(x => x.Id == message.SenderId)
                            from receiver in _context.AppUsers.Where(x => x.Id == message.ReceiverId)
                            orderby message.TimeCreated descending
                            select new
                            {
                                message.Content,
                                message.TimeCreated,
                                sender = sender.Email,
                                receiver = receiver.Email
                            }
                            );
            if (messages is null)
                return new BaseResponse<PaginatedData<MessageResponse>>(false, "No messages found");

            var totalRecordsCount = await messages.CountAsync(cancellationToken);
            var response = await messages.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).Select(x => new MessageResponse
            {
                Message = x.Content,
                ReceiverName = x.receiver,
                SenderName = x.sender,
                TimeStamp = x.TimeCreated.ToString()
            }).ToListAsync(cancellationToken);

            return new BaseResponse<PaginatedData<MessageResponse>>(true, "Message sent successfully.", new PaginatedData<MessageResponse>(response, totalRecordsCount, request.PageNumber, request.PageSize));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetMessagesRequestHandler => Application ran into an error while trying to get messages");
            return new BaseResponse<PaginatedData<MessageResponse>>(false, "An error occurred while processing your request");
        }
    }
}
