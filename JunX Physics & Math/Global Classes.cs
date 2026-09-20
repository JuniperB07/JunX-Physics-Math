using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

    public static class CompositeReductions<Str1, En1, Str2, En2, Str3, En3>
        where En1 : Enum
        where En2 : Enum
        where En3 : Enum
        where Str1 : struct, IDimensionAccessible,
            IInitializable<Str1>, IInitializable<Str1, double>, IInitializable<Str1, double, En1>,
            INormalized<En1>, INormalizable<Str1>,
            IScaleConvertible<Str1, En1>, IValueAccessible<En1>
        where Str2 : struct, IDimensionAccessible,
            IInitializable<Str2>, IInitializable<Str2, double>, IInitializable<Str2, double, En2>,
            INormalized<En2>, INormalizable<Str2>,
            IScaleConvertible<Str2, En2>, IValueAccessible<En2>
        where Str3 : struct, IDimensionAccessible,
            IInitializable<Str3>, IInitializable<Str3, double>, IInitializable<Str3, double, En3>,
            INormalized<En3>, INormalizable<Str3>,
            IScaleConvertible<Str3, En3>, IValueAccessible<En3>
    {
        public static QuotientUnit<Str2, En2, Str3, En3> Divide(ProductUnit<Str1, En1, Str2, En2> l, ProductUnit<Str1, En1, Str3, En3> r)
        {// AB / AC     =   B/C

            if (l.Original.Scale1Ordinal != r.Original.Scale1Ordinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude / r.Original.Magnitude;
            var unit = QuotientUnit<Str2, En2, Str3, En3>.Create(mag);
            return unit.SetScales(l.Original.Scale2, r.Original.Scale2);
        }

    }
}
