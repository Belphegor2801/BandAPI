using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BandAPI.Services
{
    public interface IPropertyValidationService
    {
        bool HasValideProperties<T>(string fields);
    }
}
