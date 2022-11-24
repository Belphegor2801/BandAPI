using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BandAPI.Services
{
    public class PropertyMapping<Tsource, TDestination> : IPropertyMappingMarker
    {
        public Dictionary<string, PropertyMappingValue> MappingDictionany { get; set; }

        public PropertyMapping(Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            MappingDictionany = mappingDictionary ??
                throw new ArgumentNullException(nameof(mappingDictionary));
        }
    }
}
