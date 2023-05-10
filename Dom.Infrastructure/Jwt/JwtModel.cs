namespace Taxiverk.Infrastructure.Jwt
{
    public class JwtModel
    {
        public string Id { get; set; }
        public JwtUserType Type { get; set; }
        public DateTime GenerateDate { get; set; }
    }

    public enum JwtUserType
    {
        Client = 1,
        User = 2
    }
}
