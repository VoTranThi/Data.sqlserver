using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data.SqlServer.utis
{
    internal static class ExceptionHelper
    {
        internal static ArgumentException CreateArgumentNullOrEmptyException(string paramName)
        {
            return new ArgumentException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, paramName);
        }
    }
}
