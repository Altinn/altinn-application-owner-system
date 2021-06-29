using System.Threading.Tasks;

namespace AltinnApplicationOwnerSystem.Functions.Services.Interface
{
    /// <summary>
    /// Interface that defines the Authentication service responsible for converting MaskinPorten token to AltinnToken
    /// </summary>
    public interface IAuthenticationClientWrapper
    {
        /// <summary>
        /// Converts MaskinPortenToken to AltinnToken
        /// </summary>
        /// <returns>A new AltinnToken</returns>
        Task<string> ConvertToken(string maskinportenToken);
    }
}
