using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.SqlTypes;
using System.Runtime.CompilerServices;
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
        public const string TYPE_PARAMETER_MISMATCH = "Unable to compare instances with different type parameters.";
        public const string COMPOSITE_UNIT_SCALE_MISMATCH = "Cannot operate on composite unit instances with different scale values.";

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

    #region COMPOSITE OPERATOR CLASSES
    public abstract class CompositeOperator
    {
        public abstract CompositeOperators Operator { get; }
    }

    public class Multiplication : CompositeOperator
    {
        public override CompositeOperators Operator => CompositeOperators.Multiplication;
    }
    public class Division : CompositeOperator
    {
        public override CompositeOperators Operator => CompositeOperators.Division;
    }
    #endregion

    public class CompositeProduct<Str1, Str2> : ICompositeUnit
        where Str1 : struct, IDimensionAccessible, INormalizable<Str1>
        where Str2 : struct, IDimensionAccessible, INormalizable<Str2> 
    { }
    public class CompositeQuotient<Str1, Str2> : ICompositeUnit
        where Str1 : struct, IDimensionAccessible, INormalizable<Str1>
        where Str2 : struct, IDimensionAccessible, INormalizable<Str2>
    { }
    public class BinaryComposite<TComp1, TComp2> : ICompositeUnit
        where TComp1 : class, ICompositeUnit
        where TComp2 : class, ICompositeUnit
    { }
}
