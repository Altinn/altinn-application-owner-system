using System.Threading.Tasks;

namespace AltinnApplicationOwnerSystem.Functions.Services.Interface
{
    /// <summary>
    /// Interface for interacting with key vault
    /// </summary>
    public interface IKeyVaultService
    {
        /// <summary>
        /// Gets the value of a secret from the given key vault.
        /// </summary>
        /// <param name="vaultUri">The URI of the key vault to ask for secret. </param>
        /// <param name="secretId">The id of the secret.</param>
        /// <returns>The secret value.</returns>
        Task<string> GetCertificateAsync(string vaultUri, string secretId);
    }
}
