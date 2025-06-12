using RealTimeApplication.Infrastructure.Models;

namespace RealTimeApplication.Operations.Repo.Integrations.Interfaces;

public interface IEmailGatewayService
{
    public string Provider { get; set; }
    Task<BaseResponse> SendEmailAsync(EmailRequest request, CancellationToken cancellationToken);
}
