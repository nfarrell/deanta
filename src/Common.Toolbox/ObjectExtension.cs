using System;
using System.Reflection;

namespace Common.Toolbox
{
    public static class ObjectExtension
    {
        public static T CloneObject<T>(this T objSource)
        {
            //Get the type of source object and create a new instance of that type
            var typeSource = objSource.GetType();
            var objTarget = (T)Activator.CreateInstance<T>();

            //Get all the properties of source object type
            var propertyInfo =
                typeSource.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            //Assign all source property to taget object 's properties
            foreach (var property in propertyInfo)
            {
                //Check whether property can be written to
                if (property.CanWrite)
                {
                    //check whether property type is value type, enum or string type
                    if (property.PropertyType.IsValueType || property.PropertyType.IsEnum ||
                        property.PropertyType == typeof(string))
                    {
                        property.SetValue(objTarget, property.GetValue(objSource, null), null);
                    }
                    //else property type is object/complex types, so need to recursively call this method until the end of the tree is reached
                    else
                    {
                        var objPropertyValue = property.GetValue(objSource, null);
                        if (objPropertyValue == null)
                        {
                            property.SetValue(objTarget, null, null);
                        }
                        else
                        {
                            property.SetValue(objTarget, objPropertyValue.CloneObject(), null);
                        }
                    }
                }
            }
            return objTarget;
        }
    }
}