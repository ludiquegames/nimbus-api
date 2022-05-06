namespace Ludique.Nimbus.Web.Settings
{
    public class JwtSettings
    {
        public string Audience { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Secret { get; set; } = string.Empty;
        public int Lifetime { get; set; }
        public string Type { get; set; } = string.Empty;
    }
}
