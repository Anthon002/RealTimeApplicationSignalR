namespace RealTimeApplication.Infrastructure.ApplicationSettingsOptions;

public sealed record AppSettingsOptions
{
    public ConnectionString? connectionString { get; set; }
    public EmailGateway? EmailGateway { get; set; }
}
public sealed record ConnectionString
{
    public string? Default { get; set; }
}

public sealed record EmailGateway
{
    public string? Provider { get; set; }
    public Brevo? Brevo { get; set; }
}

public sealed record Brevo
{
    public string? BaseURL { get; set; }
    public string? ApiKey { get; set; }
} 