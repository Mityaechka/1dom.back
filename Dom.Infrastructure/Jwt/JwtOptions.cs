namespace Taxiverk.Infrastructure.Jwt
{
    public class JwtOptions
    {
        public const string Path = "Jwt";
        public const string ClaimsId = "id";
        public const string ClaimsType = "type";
        public const string ClaimsRole = "role";
        public string Secret { get; set; }
        public int TtlInMinutes { get; set; }
    }
}
