using System;

namespace Deanta.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class NoAutoHistoricAttribute : Attribute
    {
        public NoAutoHistoricAttribute()
        {
        }
    }
}
