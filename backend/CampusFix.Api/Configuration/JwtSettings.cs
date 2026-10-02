namespace CampusFix.Api.Configuration;

public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Key { get; set; } = string.Empty;

    public string Issuer { get; set; } = "CampusFix.Api";

    public string Audience { get; set; } = "CampusFix.Angular";

    public int ExpirationMinutes { get; set; } = 60;
}