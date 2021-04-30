using FluentValidation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ecoporto.CRM.Business.Extensions
{
    public static class ObjectExtensions
    {
        public static string ToJson(this object objeto)
        {
            if (objeto != null)
            {
                var entidade = new Dictionary<string, string>();

                string json = string.Empty;

                Type type = objeto.GetType();

                foreach (PropertyInfo property in type.GetProperties())
                {
                    if (PropriedadeValida(property.PropertyType))                    
                        entidade.Add(property.Name, property.GetValue(objeto, null)?.ToString());                    
                }

                return JsonConvert.SerializeObject(entidade);
            }

            return string.Empty;
        }

        public static bool PropriedadeValida(Type type) 
            => (type == typeof(string) || type == typeof(int) || type == typeof(decimal) || type == typeof(bool) || type == typeof(DateTime) || type.IsEnum) && !type.IsAssignableFrom(typeof(CascadeMode));

        public static List<string> ObterPropriedades(this object objeto)
        {
            var propriedades = objeto
                .GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(c => !c.Name.Contains("Id") && !c.PropertyType.IsGenericType);

            var campos = new List<string>();

            foreach (var propriedade in propriedades)
            {
                if (propriedade.PropertyType.IsClass && propriedade.PropertyType != typeof(string))
                {
                    var propriedadeFilha = propriedade.PropertyType.GetTypeInfo();

                    foreach (var filha in propriedadeFilha.DeclaredProperties)
                        campos.Add(filha.Name);
                }
                else
                {
                    campos.Add(propriedade.Name);
                }
            }

            return campos;
        }
    }
}
