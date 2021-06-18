using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AltinnApplicationOwnerSystem.Functions.Services.Interface
{
    /// <summary>
    /// Interface defining the MaskinPorten Client wrapper responsible for authentication of Application Owner System
    /// </summary>
    public interface IMaskinPortenClientWrapper
    {
        /// <summary>
        /// Post MaskinPorten Authentication request
        /// </summary>
        Task<string> PostToken(FormUrlEncodedContent bearer); 
    }
}
