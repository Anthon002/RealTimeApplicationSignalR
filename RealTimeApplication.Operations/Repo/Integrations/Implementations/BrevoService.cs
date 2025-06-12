using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RealTimeApplication.Infrastructure.ApplicationSettingsOptions;
using RealTimeApplication.Infrastructure.Models;
using RealTimeApplication.Operations.Repo.Integrations.Interfaces;

namespace RealTimeApplication.Operations.Repo.Integrations.Implementations;

public sealed class BrevoService : IEmailGatewayService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly  AppSettingsOptions _options;

    public BrevoService(IHttpClientFactory httpClientFactory, IOptionsSnapshot<AppSettingsOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
    }
    public string Provider { get; set; } = "Brevo";

    public async Task<BaseResponse> SendEmailAsync(EmailRequest request, CancellationToken cancellationToken)
    {
        var baseurl = _options.EmailGateway?.Brevo?.BaseURL;
        var payload = new StringContent(JsonSerializer.Serialize(
            new
            {
                sender = new
                {
                    name = request.SenderName,
                    email = request.SenderEmail
                },
                to = new[]
                {
                    new
                    {
                        email = request.RecipientEmail,
                        name = request.RecipientName
                    }
                },
                subject = request.Subject,
                htmlContent = request.HtmlContent
            }
        , new JsonSerializerOptions(JsonSerializerDefaults.Web)), Encoding.UTF8, MediaTypeNames.Application.Json);
        using (var client = _httpClientFactory.CreateClient())
        {
            client.DefaultRequestHeaders.Add("api-key", _options.EmailGateway?.Brevo?.ApiKey);
            var httpResponse = await client.PostAsync($"{baseurl}/smtp/email", payload, cancellationToken);

            if (httpResponse.IsSuccessStatusCode)
            {
                return new BaseResponse(true, "Email sent successfully");
            }
            else
            {
                return new BaseResponse(false, $"Error while sending email. {httpResponse.Content.ReadAsStringAsync(cancellationToken)}");
            }
        }
        
    }
}
