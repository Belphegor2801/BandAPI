using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using System.Reflection;

namespace BandAPI.Services
{
    public class PropertyValidationService: IPropertyValidationService
    {
        public bool HasValideProperties<T>(string fields)
        {
            if (string.IsNullOrWhiteSpace(fields)) return true;

            var fieldsAfterSplit = fields.Split(",");

            foreach(var field in fieldsAfterSplit)
            {
                var propertyName = fields.Trim();
                var propertyInfo = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (propertyInfo == null) return false;
            }

            return true;
        }
    }
}
