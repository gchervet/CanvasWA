using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;

public static class Extensions
{
    public static NameValueCollection ToNameValueCollection<T>(this T dynamicObject)
    {
        var nameValueCollection = new NameValueCollection();
        foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(dynamicObject))
        {
            string value = propertyDescriptor.GetValue(dynamicObject).ToString();
            nameValueCollection.Add(propertyDescriptor.Name, value);
        }
        return nameValueCollection;
    }

    /// <summary>
    /// Busca un valor en objetos JSON genéricos
    /// </summary>
    /// <param name="jsonObject">Objeto JSON genérico</param>
    /// <param name="nameFilter">Nombre del campo que se filtrará</param>
    /// <returns>El valor string del campo, o null en su defecto</returns>
    public static string SearchValueInJObject(JObject jsonObject, string nameFilter)
    {
        foreach (JProperty jsonRootProperty in jsonObject.Properties())
        {
            if (jsonRootProperty.Name.Equals(nameFilter))
            {
                return jsonRootProperty.Value.ToString();
            }
            JToken value = jsonRootProperty.Value;
            if (value.Type == JTokenType.Object)
            {
                SearchValueInJObject((JObject)value, nameFilter);
            }
            else if (value.Type == JTokenType.Array)
            {
                foreach (JObject jsonArrayProperty in value)
                {
                    SearchValueInJObject((JObject)jsonArrayProperty, nameFilter);
                }
            }
        }
        return null;
    }
}