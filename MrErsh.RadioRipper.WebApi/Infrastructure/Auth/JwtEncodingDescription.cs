using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MrErsh.RadioRipper.WebApi.Auth
{
    public interface IJwtEncodingDescription
    {
        string SigningAlgorithm { get; }

        SecurityKey Key { get; }
    }

    public sealed class JwtEncodingDescription : IJwtEncodingDescription
    {
        private readonly SymmetricSecurityKey _secretKey;

        public JwtEncodingDescription(string key) => _secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

        public string SigningAlgorithm => SecurityAlgorithms.HmacSha256;

        public SecurityKey Key => _secretKey;
    }
}
