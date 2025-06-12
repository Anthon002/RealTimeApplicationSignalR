using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RealTimeApplication.Infrastructure.ApplicationSettingsOptions;
using RealTimeApplication.Infrastructure.Models;
using RealTimeApplication.Operations.Repo.Integrations.Interfaces;
using RealTimeApplication.Operations.Repo.Services.Interfaces;

namespace RealTimeApplication.Operations.Repo.Services.Implementations;

public sealed record EmailService : IEmailService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly AppSettingsOptions _options;

    public EmailService(IServiceProvider serviceProvider, IOptionsSnapshot<AppSettingsOptions> options)
    {
        _serviceProvider = serviceProvider;
        _options = options.Value;
    }
    public async Task<BaseResponse> SendEmail(EmailRequest request, CancellationToken cancellationToken)
    {
        var serviceGateways = _serviceProvider.GetServices<IEmailGatewayService>();
        var emailService = serviceGateways.FirstOrDefault(x => x.Provider == _options.EmailGateway?.Provider);
        var response = await emailService!.SendEmailAsync(request, cancellationToken);
        return response;
    }
}
