using System.Security.Claims;
using System.Threading.Tasks;

namespace NG_Core_Auth.JWT {
    public interface IJwtFactory {
        Task<string> GenerateEncodedToken (string userName, ClaimsIdentity identity);
        ClaimsIdentity GenerateClaimsIdentity (string userName, string id);
    }
}