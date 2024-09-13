using SmartElectronicsApi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Interfaces
{
    public interface ITokenService
    {
        string GetToken(string SecretKey, string Audience, string Issuer, AppUser existUser, IList<string> roles);

    }
}
