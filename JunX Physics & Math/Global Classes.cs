using System;
using System.Collections.Generic;
using System.Text;

namespace JunX
{
    public static class Methods
    {
        public static double Scale<T>(double magnitude, T scale, Dictionary<T, double> mapper) where T : Enum
            => magnitude * mapper[scale];
    }

    public static class ErrorMsg
    {
        public const string OPERAND_DIMENSION_MISMATCH = "Cannot operate on operands with different dimensions.";
    }
}
