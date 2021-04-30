using System;

namespace Ecoporto.CRM.Business.Helpers
{
    public class DateTimeHelpers
    {
        public static bool IsDate(object data)
        {
            return DateTime.TryParse(data?.ToString(), out _);
        }

        public static bool IsNotDefaultDate(object data)
        {
            if (DateTime.TryParse(data?.ToString(), out DateTime convertido))
            {
                return convertido != default(DateTime);
            }

            return false;
        }
    }
}
