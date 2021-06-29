using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AltinnApplicationOwnerSystem.Functions.Services.Interface
{
    /// <summary>
    /// Interface defining service responsible for the authentication process for Application Owner system
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Methods that return Altinn token. If not cached it will log the solution
        /// in to MaskinPorten and then exchange the Maskinporten token to an Altinn token.
        /// </summary>
        /// <returns></returns>
        Task<string> GetAltinnToken();
    }
}
