using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
        public const string RESULTING_NEGATIVE_DIMENSION = "Resulting dimension is negative.";
        public const string NON_ROOTABLE_DIMENSION = "Current dimension of the instance is not rootable in this case.";
        public const string INVALID_DIMENSION_CASTING = "Unable to cast current instance into the desired dimension.";
    }

    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class DistinctEnumTypesAttribute : Attribute
    {
        public int FirstEnumIndex { get; }
        public int SecondEnumIndex { get; }

        public DistinctEnumTypesAttribute(int firstEnumIndex = 1, int secondEnumIndex = 3)
        {
            FirstEnumIndex = firstEnumIndex;
            SecondEnumIndex = secondEnumIndex;
        }
    }
}
