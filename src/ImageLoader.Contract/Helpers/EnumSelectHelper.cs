using System;
using System.Collections.Generic;

namespace ImageLoader.Contract.Helpers
{
    public static class EnumSelectHelper
    {
        public static T GetSelectValueEnum<T>() where T : struct
        {
            T enumValue;
            var description = GetDescriptionEnum<T>();
            
            Console.Write($"Select {typeof(T).Name} ({description}): ");
            var consoleLine = Console.ReadLine();
            while (!Enum.TryParse(consoleLine, out enumValue) || 
                   !Enum.IsDefined(typeof(T), enumValue))
            {
                Console.Write("Not a valid number, try again: ");
                consoleLine = Console.ReadLine();
            }
            return enumValue;
        }

        private static string GetDescriptionEnum<T>()
        { 
            var i = 0;
            var list = new List<string>();
            foreach (var enumValue in Enum.GetValues(typeof(T)))
            {
                list.Add($"{i} - {enumValue}");
                i++;
            }
            return string.Join(", ", list);
        }
    }
}