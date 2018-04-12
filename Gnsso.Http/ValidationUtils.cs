using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnsso.Http
{
    internal static class ValidationUtils
    {
        public static void NotNull(object argument, string name)
        {
            if (argument == null) throw new ArgumentNullException(name);
        }
    }
}
