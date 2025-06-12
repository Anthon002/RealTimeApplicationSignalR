using RealTimeApplication.Infrastructure.Models;

namespace RealTimeApplication.Operations.Repo.Services.Interfaces;

public interface IEmailService
{
    Task<BaseResponse> SendEmail(EmailRequest request, CancellationToken cancellationToken);
}

