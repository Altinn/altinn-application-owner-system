using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AltinnApplicationOwnerSystem.Functions.Services.Interface
{
    public interface IMaskinPortenClientWrapper
    {
        Task<string> PostToken(FormUrlEncodedContent bearer); 
    }
}
