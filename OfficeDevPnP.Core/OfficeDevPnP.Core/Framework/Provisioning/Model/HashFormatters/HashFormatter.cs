using OfficeDevPnP.Core.Framework.Provisioning.Model.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model.HashFormatters
{
    public static class HashFormatter<T>
    {
        public static int GetFormatter(T model)
        {
            List<string> memberVals = new List<string>();

            var modelType = model.GetType();
            string modelName = modelType.Name;

            MemberInfo[] members = modelType.GetMembers();
            foreach (MemberInfo member in members)
            {
                object[] attributes = member.GetCustomAttributes(true);

                foreach (var attr in attributes)
                {
                    HashCodeIdentifier hciaCandidate = attr as HashCodeIdentifier;
                    if (hciaCandidate != null)
                    {
                        object prop = modelType.GetProperty(member.Name).GetValue(model);
                        if (prop != null)
                        {
                            string propVal = prop.ToString();
                            memberVals.Add(propVal);
                        }
                    }
                }
            }

            return string.Join("|", memberVals).GetHashCode();
        }
    }

}
