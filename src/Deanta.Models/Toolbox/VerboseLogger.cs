using System;
using System.Collections.Generic;
using System.Linq;
using Deanta.Models.Attributes;

namespace Deanta.Models.Toolbox
{
    /// <summary>
    /// This code is primarily for use within the History Entry Manager - Serilog typically handles the rest.
    /// </summary>
    public class VerboseLogger
    {
        public static string Log(object inputObject)
        {
            var dictionary = DictionaryFromType(inputObject);
            var newEntries = new List<string>();
            foreach (var key in dictionary.Keys)
            {
                if (dictionary[key] != null)
                {
                    newEntries.Add("[ " + key + " : " + dictionary[key] + " ]");
                }
            }
            return string.Join(',', newEntries);
        }

        public static string Log(object inputObject1, object inputObject2)
        {
            var dictionary1 = DictionaryFromType(inputObject1);
            var dictionary2 = DictionaryFromType(inputObject2);
            var differentEntries = new List<string>();
            foreach (var key in dictionary2.Keys)
            {
                if (dictionary1[key] != dictionary2[key])
                {
                    differentEntries.Add("[ " + key + " : " + dictionary1[key] + " -> " + dictionary2[key] + " ]");
                }
            }
            return string.Join(',', differentEntries);
        }

        public static Dictionary<string, object> DictionaryFromType(object atype)
        {
            if (atype == null) return new Dictionary<string, object>();
            var t = atype.GetType();
            var props = t.GetProperties().Where(
                prop => !Attribute.IsDefined(prop, typeof(NoAutoHistoricAttribute))).ToArray();
            var dict = new Dictionary<string, object>();
            foreach (var prp in props)
            {
                var value = prp.GetValue(atype, new object[] { });
                dict.Add(prp.Name, value);
            }
            return dict;
        }
    }
}
