using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace swashbuckletest.Infrastructure
{
    public static class StringExtensions
    {

        public static string FirstCharToLowerCase(this string value)
        {
            var startsWithlowercase = Char.ToLowerInvariant(value[0]) + value.Substring(1);

            return startsWithlowercase;
        }
    }
}
