using JunX.Mathematics.Geometry;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.SqlTypes;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.XPath;

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

    public class CompositeMultiplicator : CompositeOperator
    {
        public override CompositeOperators Operator => CompositeOperators.Multiplication;

    }
    public class CompositeDivider : CompositeOperator
    {
        public override CompositeOperators Operator => CompositeOperators.Division;

        #region (A^2)(B^2)
        public static ProductUnit<Str1, En1, UnitSquared<Str2, En2>, En2> Divide<Str1, En1, Str2, En2>
            (ProductUnit<UnitSquared<Str1, En1>, En1, UnitSquared<Str2, En2>, En2> l,
            Str1 r)
            where En1 : Enum
            where En2 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
        {//     A(B^2)  =   (A^2)(B^2) / A

            if (l.Original.Scale1Ordinal != r.Original.ScaleOrdinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude / r.Original.Magnitude;
            var unit = ProductUnit<Str1, En1, UnitSquared<Str2, En2>, En2>.Create(mag);
            return unit.SetScales(l.Original.Scale1, l.Original.Scale2);
        }
        public static ProductUnit<UnitSquared<Str1, En1>, En1, Str2, En2> Divide<Str1, En1, Str2, En2>
            (ProductUnit<UnitSquared<Str1, En1>, En1, UnitSquared<Str2, En2>, En2> l,
            Str2 r)
            where En1 : Enum
            where En2 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
        {//     (A^2)B  =   (A^2)(B^2) / B

            if (l.Original.Scale2Ordinal != r.Original.ScaleOrdinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude / r.Original.Magnitude;
            var unit = ProductUnit<UnitSquared<Str1, En1>, En1, Str2, En2>.Create(mag);
            return unit.SetScales(l.Original.Scale1, l.Original.Scale2);
        }
        #endregion
    }
    public static class CompositeReductor
    {
        #region A(B^2) & (A^2)B
        public static Str1 Divide<Str1, En1, Str2, En2>
            (ProductUnit<Str1, En1, UnitSquared<Str2, En2>, En2> l,
            UnitSquared<Str2, En2> r)
            where En1 : Enum
            where En2 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
        {//     A   =   A(B^2) / B^2

            if (l.Original.Scale2Ordinal != r.Original.ScaleOrdinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude / r.Original.Magnitude;
            return Str1.Create(mag, l.Original.Scale1);
        }
        public static Str1 Divide<Str1, En1, Str2, En2>
            (ProductUnit<UnitSquared<Str1, En1>, En1, Str2, En2> l,
            ProductUnit<Str1, En1, Str2, En2> r)
            where En1 : Enum
            where En2 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
        {//     A   =   (A^2)B / AB

            if (l.Original.Scale1Ordinal != r.Original.Scale1Ordinal ||
                l.Original.Scale2Ordinal != r.Original.Scale2Ordinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude / r.Original.Magnitude;
            return Str1.Create(mag, l.Original.Scale1);
        }
        public static Str2 Divide<Str1, En1, Str2, En2>
            (ProductUnit<Str1, En1, UnitSquared<Str2, En2>, En2> l,
            ProductUnit<Str1, En1, Str2, En2> r)
            where En1 : Enum
            where En2 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
        {//     B   =   A(B^2) / AB

            if (l.Original.Scale1Ordinal != r.Original.Scale1Ordinal ||
                l.Original.Scale2Ordinal != r.Original.Scale2Ordinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude / r.Original.Magnitude;
            return Str2.Create(mag, l.Original.Scale2);
        }
        public static Str2 Divide<Str1, En1, Str2, En2>
            (ProductUnit<UnitSquared<Str1, En1>, En1, Str2, En2> l,
            UnitSquared<Str1, En1> r)
            where En1 : Enum
            where En2 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
        {//     B   =   (A^2)B / A^2

            if (l.Original.Scale1Ordinal != r.Original.ScaleOrdinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude / r.Original.Magnitude;
            return Str2.Create(mag, l.Original.Scale2);
        }
        public static double Divide<Str1, En1, Str2, En2>
            (ProductUnit<Str1, En1, UnitSquared<Str2, En2>, En2> l,
            ProductUnit<Str2, En2, UnitSquared<Str2, En2>, En2> r)
            where En1 : Enum
            where En2 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
        {//     N   =   Self / Self

            if (l.Original.Scale1Ordinal != r.Original.Scale1Ordinal ||
                l.Original.Scale2Ordinal != r.Original.Scale2Ordinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            return l.Original.Magnitude / r.Original.Magnitude;
        }
        public static double Divide<Str1, En1, Str2, En2>
            (ProductUnit<UnitSquared<Str1, En1>, En1, Str2, En2> l,
            ProductUnit<UnitSquared<Str1, En1>, En1, Str2, En2> r)
            where En1 : Enum
            where En2 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
        {//     N   =   Self / Self

            if (l.Original.Scale1Ordinal != r.Original.Scale1Ordinal ||
                l.Original.Scale2Ordinal != r.Original.Scale2Ordinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            return l.Original.Magnitude / r.Original.Magnitude;
        }
        #endregion

        #region (A^2)(B^2)
        public static Str1 Divide<Str1, En1, Str2, En2>
            (ProductUnit<UnitSquared<Str1, En1>, En1, UnitSquared<Str2, En2>, En2> l,
            ProductUnit<Str1, En1, UnitSquared<Str2, En2>, En2> r)
            where En1 : Enum
            where En2 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
        {//     A  =    (A^2)(B^2) / A(B^2)

            if (l.Original.Scale1Ordinal != r.Original.Scale1Ordinal ||
                l.Original.Scale2Ordinal != r.Original.Scale2Ordinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude / r.Original.Magnitude;
            return Str1.Create(mag, l.Original.Scale1);
        }
        public static Str2 Divide<Str1, En1, Str2, En2>
            (ProductUnit<UnitSquared<Str1, En1>, En1, UnitSquared<Str2, En2>, En2> l,
            ProductUnit<UnitSquared<Str1, En1>, En1, Str2, En2> r)
            where En1 : Enum
            where En2 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
        {//     B   =   (A^2)(B^2) / (A^2)B

            if (l.Original.Scale1Ordinal != r.Original.Scale1Ordinal ||
                l.Original.Scale2Ordinal != r.Original.Scale2Ordinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude / r.Original.Magnitude;
            return Str2.Create(mag, l.Original.Scale2);
        }
        public static double Divide<Str1, En1, Str2, En2>
            (ProductUnit<UnitSquared<Str1, En1>, En1, UnitSquared<Str2, En2>, En2> l,
            ProductUnit<UnitSquared<Str1, En1>, En1, UnitSquared<Str2, En2>, En2> r)
            where En1 : Enum
            where En2 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
        {//     N   =   Self / Self

            if (l.Original.Scale1Ordinal != r.Original.Scale1Ordinal ||
                l.Original.Scale2Ordinal != r.Original.Scale2Ordinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            return l.Original.Magnitude / r.Original.Magnitude;
        }
        #endregion

        #region A/(B^2)
        public static Str1 Multiply<Str1, En1, Str2, En2>
            (QuotientUnit<Str1, En1, UnitSquared<Str2, En2>, En2> l,
            UnitSquared<Str2, En2> r)
            where En1 : Enum
            where En2 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
        {//     A   =   A/(B^2) / B^2

            if (l.Original.Scale2Ordinal != r.Original.ScaleOrdinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude * r.Original.Magnitude;
            return Str1.Create(mag, l.Original.Scale1);
        }
        public static Str1 Multiply<Str1, En1, Str2, En2>
            (UnitSquared<Str2, En2> l,
            QuotientUnit<Str1, En1, UnitSquared<Str2, En2>, En2> r)
            where En1 : Enum
            where En2 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
            => Multiply(r, l);
        public static Str2 Multiply<Str1, En1, Str2, En2>
            (QuotientUnit<Str1, En1, UnitSquared<Str2, En2>, En2> l,
            QuotientUnit<Str2, En2, Str1, En1> r)
            where En1 : Enum
            where En2 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
        {//     B   =   A/(B^2) * B/A

            if (l.Original.Scale1Ordinal != r.Original.Scale1Ordinal ||
                l.Original.Scale2Ordinal != r.Original.Scale2Ordinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude * r.Original.Magnitude;
            return Str2.Create(mag, l.Original.Scale2);
        }
        public static Str2 Multiply<Str1, En1, Str2, En2>
            (QuotientUnit<Str2, En2, Str1, En1> l,
            QuotientUnit<Str1, En1, UnitSquared<Str2, En2>, En2> r)
            where En1 : Enum
            where En2 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
            => Multiply(r, l);
        public static double Divide<Str1, En1, Str2, En2>
            (QuotientUnit<Str1, En1, UnitSquared<Str2, En2>, En2> l,
            QuotientUnit<Str1, En1, UnitSquared<Str2, En2>, En2> r)
            where En1 : Enum
            where En2 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
        {
            if (l.Original.Scale1Ordinal != r.Original.Scale1Ordinal ||
                l.Original.Scale2Ordinal != r.Original.Scale2Ordinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            return l.Original.Magnitude / r.Original.Magnitude;
        }
        #endregion
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
