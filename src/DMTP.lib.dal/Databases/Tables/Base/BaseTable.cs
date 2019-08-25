using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace DMTP.lib.dal.Databases.Tables.Base
{
    public class BaseTable
    {
        public Guid ID { get; set; }

        public bool Active { get; set; }

        public bool IsValid()
        {
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                var requiredAttribute = property.GetCustomAttribute<RequiredAttribute>();

                if (requiredAttribute == null)
                {
                    continue;
                }

                if (property.GetValue(this) != null)
                {
                    continue;
                }

                return false;
            }

            return true;
        }
    }
}