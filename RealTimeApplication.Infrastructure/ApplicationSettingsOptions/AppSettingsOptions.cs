namespace RealTimeApplication.Infrastructure.ApplicationSettingsOptions;

public sealed record AppSettingsOptions
{
    public ConnectionString? connectionString;
}
public sealed record ConnectionString
{
    public string? Default { get; set; }
}