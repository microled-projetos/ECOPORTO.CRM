using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ecoporto.CRM.Business.Helpers
{
    public static class ObjetoHelpers
    {
        public static bool GetValue(object currentObject, string propName, out object value)
        {
            // call helper function that keeps track of which objects we've seen before
            return GetValue(currentObject, propName, out value, new HashSet<object>());
        }

        public static bool GetValue(object currentObject, string propName, out object value,
                             HashSet<object> searchedObjects)
        {
            PropertyInfo propInfo = currentObject.GetType().GetProperty(propName);
            if (propInfo != null)
            {
                value = propInfo.GetValue(currentObject, null);
                return true;
            }
            // search child properties
            foreach (PropertyInfo propInfo2 in currentObject.GetType().GetProperties())
            {   // ignore indexed properties
                if (propInfo2.GetIndexParameters().Length == 0)
                {
                    object newObject = propInfo2.GetValue(currentObject, null);
                    if (newObject != null && searchedObjects.Add(newObject) &&
                        GetValue(newObject, propName, out value, searchedObjects))
                        return true;
                }
            }
            // property not found here
            value = null;
            return false;
        }

        public static string ObterValorPropriedade(object objeto, string propriedadeBusca)
        {
            foreach (var propriedadeAntiga in objeto.GetType().GetTypeInfo().DeclaredProperties)
            {
                if (propriedadeAntiga.PropertyType.IsClass && propriedadeAntiga.PropertyType != typeof(string))
                {
                    var propriedadeFilha = propriedadeAntiga.PropertyType.GetTypeInfo();

                    foreach (var filha in propriedadeFilha.DeclaredProperties)
                    {
                        if (filha.Name == propriedadeBusca)
                        {

                            var selectedProperty = from property in objeto.GetType().GetProperties()
                                                   where property.Name == filha.Name
                                                   select property.GetValue(null, null);


                            var teste = propriedadeFilha.GetType().GetProperty(filha.Name);
                            var valor = filha.GetValue(objeto, null).ToString();

                            if (valor == propriedadeBusca)
                                return valor;
                        }
                    }
                }

                if (propriedadeAntiga.Name == propriedadeBusca)
                {
                    var valor = propriedadeAntiga.GetValue(objeto, null).ToString();

                    if (valor == propriedadeBusca)
                        return valor;
                }
            }

            return string.Empty;
        }
    }
}