using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Altinn.Platform.Storage.Interface.Models;

namespace AltinnApplicationOwnerSystem.Functions.Services.Interface
{
    /// <summary>
    /// Interface for handling form data related operations
    /// </summary>
    public interface IAltinnApp
    {
        /// <summary>
        /// Gets the instance metadata for a given app
        /// </summary>
        Task<Instance> GetInstance(string appId, string instanceId);

        /// <summary>
        /// Add complete confirmation.
        /// </summary>
        /// <remarks>
        /// Add to an instance that a given stakeholder considers the instance as no longer needed by them. The stakeholder has
        /// collected all the data and information they needed from the instance and expect no additional data to be added to it.
        /// The body of the request isn't used for anything despite this being a POST operation.
        /// </remarks>
        Task<Instance> AddCompleteConfirmation(string instanceUri);
    }
}
