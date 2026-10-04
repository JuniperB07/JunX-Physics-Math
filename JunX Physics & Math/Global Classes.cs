using JunX.Mathematics.Geometry;
using JunX.Physics.BaseUnits;
using JunX.Physics.ClassicalMechanics;
using JunX.Physics.Electromagnetism;
using JunX.Physics.Kinematics;
using JunX.Physics.Thermodynamics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.SqlTypes;
using System.Net.Quic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.XPath;

namespace JunX
{
    public static class Constants
    {
        public static readonly Velocity c = new(299792458);
        public static readonly Velocity SpeedOfLight = c;
        public static readonly Acceleration EarthGravity = new(9.8);
        public static readonly ElectricCharge ElementaryCharge = new(1.602176634e-19);
        public static readonly ElectricResistance VonKlitzing = new(25812.807);
        public static readonly ElectricResistance FreeSpaceImpedance = new(376.730313);
        public static readonly ElectricConductance ConductanceQuantum = new(7.74809e-5);
        public static readonly MagneticFlux MagneticFluxQuantum = new(2.067833848e-15);
        public static readonly Entropy Boltzmann = new(1.380649e-23);


        public static readonly TernaryQuotientUnit<
            Numerator<CompositeProduct<Force, UnitSquared<Length, LengthUnits>>>,
            Force, ForceUnits,
            UnitSquared<Length, LengthUnits>, LengthUnits,
            UnitSquared<Mass, MassUnits>, MassUnits> GravitationalConstant
            = new TernaryQuotientUnit<Numerator<CompositeProduct<Force, UnitSquared<Length, LengthUnits>>>, Force, ForceUnits, UnitSquared<Length, LengthUnits>, LengthUnits, UnitSquared<Mass, MassUnits>, MassUnits>(6.674e-11)
            .SetScales(ForceUnits.Newton, LengthUnits.Meter, MassUnits.Kilogram);

        public static readonly TernaryQuotientUnit<
            Numerator<CompositeProduct<Force, UnitSquared<Length, LengthUnits>>>,
            Force, ForceUnits,
            UnitSquared<Length, LengthUnits>, LengthUnits,
            UnitSquared<ElectricCharge, ElectricChargeUnits>, ElectricChargeUnits> CoulombConstant
            = new TernaryQuotientUnit<Numerator<CompositeProduct<Force, UnitSquared<Length, LengthUnits>>>, Force, ForceUnits, UnitSquared<Length, LengthUnits>, LengthUnits, UnitSquared<ElectricCharge, ElectricChargeUnits>, ElectricChargeUnits>(8.99e9)
            .SetScales(ForceUnits.Newton, LengthUnits.Meter, ElectricChargeUnits.Coulomb);
    }

    /// <summary>
    /// Provides general-purpose static utility methods for scale transformation and value mapping 
    /// across unit scale enumerations within the dimensional framework.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="Methods"/> class serves as a centralized helper class for performing low-level scalar 
    /// scaling operations using enumeration-based conversion mappers. 
    /// </para>
    /// <para>
    /// It complements the static abstract interface methods of primary unit types by offering flexible, 
    /// dictionary-backed scale conversions for generic enumeration values without requiring full 1D unit struct instantiation.
    /// </para>
    /// </remarks>
    public static class Methods
    {
        public static double Scale<T>(double magnitude, T scale, Dictionary<T, double> mapper) where T : Enum
            => magnitude * mapper[scale];
    }

    /// <summary>
    /// Provides centralized error message constants used for exception handling across the dimensional analysis framework.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="ErrorMsg"/> class defines standard string constants thrown in runtime exceptions (such as 
    /// <see cref="InvalidOperationException"/> or <see cref="ArgumentException"/>) when dimensional constraints, 
    /// scale matching rules, or type safety requirements are violated.
    /// </para>
    /// <para>
    /// Centralizing these messages ensures consistent error reporting across single-dimensional, multidimensional, 
    /// and higher-order composite unit operations.
    /// </para>
    /// </remarks>
    public static class ErrorMsg
    {
        public const string OPERAND_DIMENSION_MISMATCH = "Cannot operate on operands with different dimensions.";
        public const string RESULTING_NEGATIVE_DIMENSION = "Resulting dimension is negative.";
        public const string NON_ROOTABLE_DIMENSION = "Current dimension of the instance is not rootable in this case.";
        public const string INVALID_DIMENSION_CASTING = "Unable to cast current instance into the desired dimension.";
        public const string TYPE_PARAMETER_MISMATCH = "Unable to compare instances with different type parameters.";
        public const string COMPOSITE_UNIT_SCALE_MISMATCH = "Cannot operate on composite unit instances with different scale values.";
        public const string DIMENSIONAL_CONVERSION = "Unable to convert to the desired unit. Please check the dimensionality of each element.";
        public const string ARRAY_LENGTH_MISMATCH = "Length of both arrays must be the same size.";
        public const string COMPOSITE_SCALES_MISMATCH = "Scales of the specified composite unit does not match the requirement to transpose.";
        public const string INVALID_RECIPROCAL_NUMERATOR = "Reciprocal numerator cannot be negative.";
        public const string NON_ZERO_RECIPROCAL_NUMERATOR = "Reciprocal numerator must be 0.";
        public const string NON_INVERSE_RECIPROCAL = "ReciprocalUnit numerator value is not 1.";
        public const string NON_NORMALIZED_OPERAND_SCALES = "Scale values of both operands must all be normalized.";
        public const string GENERIC_PARAMETER_TYPE_MISMATCH = "Parameter generic types does not match the requirement.";

        public static string Insufficient_Array_Length(int minimumLength)
            => $"Array length must not be less than {minimumLength}.";
    }

    /// <summary>
    /// Indicates that a generic structure or class requires two distinct unit scale enumeration types 
    /// at the specified generic type argument positions.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This attribute is used by code generation tools, source generators, and static analyzers 
    /// within the framework to enforce design-time validation on composite unit types. It ensures 
    /// that generic parameters representing unit scales (e.g., <c>TEnum1</c> and <c>TEnum2</c>) are not 
    /// inadvertently instantiated with the same enumeration type when distinct scales are required.
    /// </para>
    /// <para>
    /// By default, it targets the generic type arguments at 1-based indices 1 and 3 (typically corresponding 
    /// to <c>En1</c> and <c>En2</c> in multi-factor composite unit signatures).
    /// </para>
    /// </remarks>
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
    /// <summary>
    /// Provides an abstract base definition and static evaluation dispatch hub for higher-order composite 
    /// unit operators, transmutations, and multi-factor algebraic reductions.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="CompositeOperator"/> serves as a core computational engine for multi-factor dimensional algebra. 
    /// It implements generic static transformations across complex composite structures, including nested quotient transmutations, 
    /// higher-power reductions (e.g., dividing squared-product units <c>A²B² / A</c> or multiplying squared-quotients <c>(A²/B²) * B</c>), 
    /// and quaternary-to-ternary product reductions (<c>ABCD / D</c>).
    /// </para>
    /// <para>
    /// All static operation methods enforce scale consistency across constituent unit terms by checking ordinal scale matching, 
    /// throwing an <see cref="InvalidOperationException"/> when scale mismatches are encountered.
    /// </para>
    /// </remarks>
    public abstract class CompositeOperator
    {
        public abstract CompositeOperators Operator { get; }

        #region TRANSMUTE
        public static TernaryProductUnit<CompositeQuotient<Str3, Str2>, Str1, En1, Str2, En2, Str3, En3> Transmute<Str1, En1, Str2, En2, Str3, En3>
            (TernaryQuotientUnit<Denominator<CompositeQuotient<Str2, Str3>>, Str1, En1, Str2, En2, Str3, En3> nested)
            where En1: Enum
            where En2: Enum
            where En3: Enum
            where Str1: struct, ILinearUnit<Str1, En1>
            where Str2: struct, ILinearUnit<Str2, En2>
            where Str3: struct, ILinearUnit<Str3, En3>
        {
            var unit = TernaryProductUnit<CompositeQuotient<Str3, Str2>, Str1, En1, Str2, En2, Str3, En3>.Create(nested.Magnitude);
            return unit.SetScales(nested.Scales.Scale1, nested.Scales.Scale2, nested.Scales.Scale3);
        }
        #endregion

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
        public static ProductUnit<Str1, En1, Str2, En2> Divide<Str1, En1, Str2, En2>
            (ProductUnit<UnitSquared<Str1, En1>, En1, UnitSquared<Str2, En2>, En2> l,
            ProductUnit<Str1, En1, Str2, En2> r)
            where En1 : Enum
            where En2 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
        {
            if (l.Original.Scale1Ordinal != r.Original.Scale1Ordinal ||
                l.Original.Scale2Ordinal != r.Original.Scale2Ordinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude / r.Original.Magnitude;
            var unit = ProductUnit<Str1, En1, Str2, En2>.Create(mag);
            return unit.SetScales(l.Original.Scale1, r.Original.Scale2);
        }
        #endregion

        #region (A^2)/(B^2)
        public static QuotientUnit<UnitSquared<Str1, En1>, En1, Str2, En2> Multiply<Str1, En1, Str2, En2>
            (QuotientUnit<UnitSquared<Str1, En1>, En1, UnitSquared<Str2, En2>, En2> l,
            Str2 r)
            where En1 : Enum
            where En2 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
        {//     (A^2)/B =   (A^2)/(B^2) * B

            if (l.Original.Scale2Ordinal != r.Original.ScaleOrdinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude * r.Original.Magnitude;
            var unit = QuotientUnit<UnitSquared<Str1, En1>, En1, Str2, En2>.Create(mag);
            return unit.SetScales(l.Original.Scale1, r.Original.Scale);
        }
        public static QuotientUnit<UnitSquared<Str1, En1>, En1, Str2, En2> Multiply<Str1, En1, Str2, En2>
            (Str2 l,
            QuotientUnit<UnitSquared<Str1, En1>, En1, UnitSquared<Str2, En2>, En2> r)
            where En1 : Enum
            where En2 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
            => Multiply(r, l);
        public static QuotientUnit<Str1, En1, UnitSquared<Str2, En2>, En2> Divide<Str1, En1, Str2, En2>
            (Str1 l,
            QuotientUnit<UnitSquared<Str1, En1>, En1, UnitSquared<Str2, En2>, En2> r)
            where En1 : Enum
            where En2 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
        {//     A/(B^2) =   A / (A^2)/(B^2)

            if (l.Original.ScaleOrdinal != r.Original.Scale1Ordinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude / r.Original.Magnitude;
            var unit = QuotientUnit<Str1, En1, UnitSquared<Str2, En2>, En2>.Create(mag);
            return unit.SetScales(r.Original.Scale1, r.Original.Scale2);
        }
        public static QuotientUnit<Str1, En1, Str2, En2> Multiply<Str1, En1, Str2, En2>
            (QuotientUnit<UnitSquared<Str1, En1>, En1, UnitSquared<Str2, En2>, En2> l,
            QuotientUnit<Str2, En2, Str1, En1> r)
            where En1 : Enum
            where En2 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
        {
            if (l.Original.Scale1Ordinal != r.Original.Scale2Ordinal ||
                l.Original.Scale2Ordinal != r.Original.Scale1Ordinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude * r.Original.Magnitude;
            var unit = QuotientUnit<Str1, En1, Str2, En2>.Create(mag);
            return unit.SetScales(l.Original.Scale1, l.Original.Scale2);
        }
        public static QuotientUnit<Str1, En1, Str2, En2> Multiply<Str1, En1, Str2, En2>
            (QuotientUnit<Str2, En2, Str1, En1> l,
            QuotientUnit<UnitSquared<Str1, En1>, En1, UnitSquared<Str2, En2>, En2> r)
            where En1 : Enum
            where En2 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
            => Multiply(r, l);
        #endregion

        #region AB * CD
        public static TernaryProductUnit<CompositeProduct<Str1, Str2>, Str1, En1, Str2, En2, Str3, En3> Divide<Str1, En1, Str2, En2, Str3, En3, Str4, En4>
            (QuaternaryProductUnit<CompositeProduct<Str1, Str2>, Str1, En1, Str2, En2, Str3, En3, Str4, En4> l,
            Str4 r)
            where En1: Enum
            where En2: Enum
            where En3: Enum
            where En4: Enum
            where Str1: struct, ILinearUnit<Str1, En1>
            where Str2: struct, ILinearUnit<Str2, En2>
            where Str3: struct, ILinearUnit<Str3, En3>
            where Str4: struct, ILinearUnit<Str4, En4>
        {//     ABC =   ABCD / D

            if (l.Scales.Ordinal4 != r.Original.ScaleOrdinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Magnitude / r.Original.Magnitude;
            var unit = TernaryProductUnit<CompositeProduct<Str1, Str2>, Str1, En1, Str2, En2, Str3, En3>.Create(mag);
            return unit.SetScales(l.Scales.Scale1, l.Scales.Scale2, l.Scales.Scale3);
        }
        #endregion

        #region A(B^2) & (A^2)B
        public static ProductUnit<Str1, En1, Str2, En2> Divide<Str1, En1, Str2, En2>
            (ProductUnit<Str1, En1, UnitSquared<Str2, En2>, En2> l,
            Str2 r)
            where En1 : Enum
            where En2 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
        {
            if (l.Original.Scale2Ordinal != r.Original.ScaleOrdinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude / r.Original.Magnitude;
            var unit = ProductUnit<Str1, En1, Str2, En2>.Create(mag);
            return unit.SetScales(l.Original.Scale1, r.Original.Scale);
        }
        public static ProductUnit<Str1, En1, Str2, En2> Divide<Str1, En1, Str2, En2>
            (ProductUnit<UnitSquared<Str1, En1>, En1, Str2, En2> l,
            Str1 r)
            where En1 : Enum
            where En2 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
        {
            if (l.Original.Scale1Ordinal != r.Original.ScaleOrdinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude / r.Original.Magnitude;
            var unit = ProductUnit<Str1, En1, Str2, En2>.Create(mag);
            return unit.SetScales(r.Original.Scale, l.Original.Scale2);
        }
        #endregion

        #region A/(B^2)
        public static QuotientUnit<Str1, En1, Str2, En2> Multiply<Str1, En1, Str2, En2>
            (QuotientUnit<Str1, En1, UnitSquared<Str2, En2>, En2> l,
            Str2 r)
            where En1 : Enum
            where En2 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
        {
            if (l.Original.Scale2Ordinal != r.Original.ScaleOrdinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude * r.Original.Magnitude;
            var unit = QuotientUnit<Str1, En1, Str2, En2>.Create(mag);
            return unit.SetScales(l.Original.Scale1, r.Original.Scale);
        }
        public static QuotientUnit<Str1, En1, Str2, En2> Multiply<Str1, En1, Str2, En2>
            (Str2 l,
            QuotientUnit<Str1, En1, UnitSquared<Str2, En2>, En2> r)
            where En1 : Enum
            where En2 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
            => Multiply(r, l);
        public static UnitSquared<Str2, En2> Divide<Str1, En1, Str2, En2>
            (Str1 l,
            QuotientUnit<Str1, En1, UnitSquared<Str2, En2>, En2> r)
            where En1 : Enum
            where En2 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
        {
            if (r.Original.Scale1Ordinal != l.Original.ScaleOrdinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude / r.Original.Magnitude;
            return UnitSquared<Str2, En2>.Create(mag, r.Original.Scale2);
        }
        #endregion

        #region (A^2)/B
        public static QuotientUnit<Str1, En1, Str2, En2> Divide<Str1, En1, Str2, En2>
            (Str1 l,
            QuotientUnit<UnitSquared<Str1, En1>, En1, Str2, En2> r)
            where En1 : Enum
            where En2 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
        {
            if (l.Original.ScaleOrdinal != r.Original.Scale1Ordinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude / r.Original.Magnitude;
            var unit = QuotientUnit<Str1, En1, Str2, En2>.Create(mag);
            return unit.SetScales(l.Original.Scale, r.Original.Scale2);
        }
        public static UnitSquared<Str1, En1> Multiply<Str1, En1, Str2, En2>
            (QuotientUnit<UnitSquared<Str1, En1>, En1, Str2, En2> l,
            Str2 r)
            where En1 : Enum
            where En2 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
        {
            if (l.Original.Scale2Ordinal != r.Original.ScaleOrdinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude * r.Original.Magnitude;
            return UnitSquared<Str1, En1>.Create(mag, l.Original.Scale1);
        }
        public static UnitSquared<Str1, En1> Multiply<Str1, En1, Str2, En2>
            (Str2 l,
            QuotientUnit<UnitSquared<Str1, En1>, En1, Str2, En2> r)
            where En1 : Enum
            where En2 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
            => Multiply(r, l);
        #endregion

        #region SIMPLIFY
        public static QuotientUnit<Str1, En1, Str3, En3> Divide<Str1, En1, Str2, En2, Str3, En3>
            (ProductUnit<Str1, En1, Str2, En2> l,
            ProductUnit<Str2, En2, Str3, En3> r)
            where En1 : Enum
            where En2 : Enum
            where En3 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
            where Str3 : struct, ILinearUnit<Str3, En3>
        {
            if (l.Original.Scale2Ordinal != r.Original.Scale1Ordinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude / r.Original.Magnitude;
            var unit = QuotientUnit<Str1, En1, Str3, En3>.Create(mag);
            return unit.SetScales(l.Original.Scale1, r.Original.Scale2);
        }
        public static QuotientUnit<Str3, En3, Str1, En1> Divide<Str1, En1, Str2, En2, Str3, En3>
            (ProductUnit<Str2, En2, Str3, En3> l,
            ProductUnit<Str1, En1, Str2, En2> r)
            where En1 : Enum
            where En2 : Enum
            where En3 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
            where Str3 : struct, ILinearUnit<Str3, En3>
            => Divide(r, l).Reciprocate();
        public static QuotientUnit<Str2, En2, Str3, En3> Divide<Str1, En1, Str2, En2, Str3, En3>
            (ProductUnit<Str1, En1, Str2, En2> l,
            ProductUnit<Str1, En1, Str3, En3> r)
            where En1 : Enum
            where En2 : Enum
            where En3 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
            where Str3 : struct, ILinearUnit<Str3, En3>
        {
            if (l.Original.Scale1Ordinal != r.Original.Scale1Ordinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude / r.Original.Magnitude;
            var unit = QuotientUnit<Str2, En2, Str3, En3>.Create(mag);
            return unit.SetScales(l.Original.Scale2, r.Original.Scale2);
        }
        public static QuotientUnit<Str3, En3, Str2, En2> Divide<Str1, En1, Str2, En2, Str3, En3>
            (ProductUnit<Str1, En1, Str3, En3> l,
            ProductUnit<Str1, En1, Str2, En2> r)
            where En1 : Enum
            where En2 : Enum
            where En3 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
            where Str3 : struct, ILinearUnit<Str3, En3>
        {
            if (l.Original.Scale1Ordinal != r.Original.Scale1Ordinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude / r.Original.Magnitude;
            var unit = QuotientUnit<Str3, En3, Str2, En2>.Create(mag);
            return unit.SetScales(l.Original.Scale2, r.Original.Scale2);
        }
        public static QuotientUnit<Str1, En1, Str2, En2> Divide<Str1, En1, Str2, En2, Str3, En3>
            (ProductUnit<Str1, En1, Str3, En3> l,
            ProductUnit<Str2, En2, Str3, En3> r)
            where En1 : Enum
            where En2 : Enum
            where En3 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
            where Str3 : struct, ILinearUnit<Str3, En3>
        {
            if (l.Original.Scale2Ordinal != r.Original.Scale2Ordinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude / r.Original.Magnitude;
            var unit = QuotientUnit<Str1, En1, Str2, En2>.Create(mag);
            return unit.SetScales(l.Original.Scale1, r.Original.Scale1);
        }
        public static QuotientUnit<Str2, En2, Str1, En1> Divide<Str1, En1, Str2, En2, Str3, En3>
            (ProductUnit<Str2, En2, Str3, En3> l,
            ProductUnit<Str1, En1, Str3, En3> r)
            where En1 : Enum
            where En2 : Enum
            where En3 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
            where Str3 : struct, ILinearUnit<Str3, En3>
        {
            if (l.Original.Scale2Ordinal != r.Original.Scale2Ordinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude / r.Original.Magnitude;
            var unit = QuotientUnit<Str2, En2, Str1, En1>.Create(mag);
            return unit.SetScales(l.Original.Scale1, r.Original.Scale1);
        }
        #endregion

        #region ABC
        public static TernaryProductUnit<CompositeProduct<Str1, Str2>, Str1, En1, Str2, En2, Str3, En3> Multiply<Str1, En1, Str2, En2, Str3, En3>
            (ProductUnit<Str1, En1, Str2, En2> l, Str3 r, En3 rEn)
            where En1 : Enum
            where En2 : Enum
            where En3 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
            where Str3 : struct, ILinearUnit<Str3, En3>
        {
            double mag = l.Original.Magnitude * r.Original.Magnitude;
            var unit = TernaryProductUnit<CompositeProduct<Str1, Str2>, Str1, En1, Str2, En2, Str3, En3>.Create(mag);
            return unit.SetScales(l.Original.Scale1, l.Original.Scale2, r.Original.Scale);
        }
        public static ProductUnit<Str1, En1, Str2, En2> Divide<Str1, En1, Str2, En2, Str3, En3>
            (TernaryProductUnit<CompositeProduct<Str1, Str2>, Str1, En1, Str2, En2, Str3, En3> l,
            Str3 r)
            where En1 : Enum
            where En2 : Enum
            where En3 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
            where Str3 : struct, ILinearUnit<Str3, En3>
        {
            if (l.Scales.Ordinal3 != r.Original.ScaleOrdinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Magnitude / r.Original.Magnitude;
            var unit = ProductUnit<Str1, En1, Str2, En2>.Create(mag);
            return unit.SetScales(l.Scales.Scale1, l.Scales.Scale2);
        }
        public static ProductUnit<Str1, En1, Str3, En3> Divide<Str1, En1, Str2, En2, Str3, En3>
            (TernaryProductUnit<CompositeProduct<Str1, Str2>, Str1, En1, Str2, En2, Str3, En3> l,
            Str2 r)
            where En1 : Enum
            where En2 : Enum
            where En3 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
            where Str3 : struct, ILinearUnit<Str3, En3>
        {
            if (l.Scales.Ordinal2 != r.Original.ScaleOrdinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Magnitude / r.Original.Magnitude;
            var unit = ProductUnit<Str1, En1, Str3, En3>.Create(mag);
            return unit.SetScales(l.Scales.Scale1, l.Scales.Scale3);
        }
        public static ProductUnit<Str2, En2, Str3, En3> Divide<Str1, En1, Str2, En2, Str3, En3>
            (TernaryProductUnit<CompositeProduct<Str1, Str2>, Str1, En1, Str2, En2, Str3, En3> l,
            Str1 r)
            where En1 : Enum
            where En2 : Enum
            where En3 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
            where Str3 : struct, ILinearUnit<Str3, En3>
        {
            if (l.Scales.Ordinal1 == r.Original.ScaleOrdinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Magnitude / r.Original.Magnitude;
            var unit = ProductUnit<Str2, En2, Str3, En3>.Create(mag);
            return unit.SetScales(l.Scales.Scale2, l.Scales.Scale3);
        }
        #endregion

        #region AB/C
        public static TernaryQuotientUnit<Numerator<CompositeProduct<Str1, Str2>>, Str1, En1, Str2, En2, Str3, En3> Divide<Str1, En1, Str2, En2, Str3, En3>
            (ProductUnit<Str1, En1, Str2, En2> l, Str3 r, En3 rEn)
            where En1 : Enum
            where En2 : Enum
            where En3 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
            where Str3 : struct, ILinearUnit<Str3, En3>
        {
            double mag = l.Original.Magnitude / r.Original.Magnitude;
            var unit = TernaryQuotientUnit<Numerator<CompositeProduct<Str1, Str2>>, Str1, En1, Str2, En2, Str3, En3>.Create(mag);
            return unit.SetScales(l.Original.Scale1, l.Original.Scale2, r.Original.Scale);
        }
        public static ProductUnit<Str1, En1, Str2, En2> Multiply<Str1, En1, Str2, En2, Str3, En3>
            (TernaryQuotientUnit<Numerator<CompositeProduct<Str1, Str2>>, Str1, En1, Str2, En2, Str3, En3> l,
            Str3 r)
            where En1 : Enum
            where En2 : Enum
            where En3 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
            where Str3 : struct, ILinearUnit<Str3, En3>
        {
            if (l.Scales.Ordinal3 != r.Original.ScaleOrdinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Magnitude * r.Original.Magnitude;
            var unit = ProductUnit<Str1, En1, Str2, En2>.Create(mag);
            return unit.SetScales(l.Scales.Scale1, l.Scales.Scale2);
        }
        public static ProductUnit<Str1, En1, Str2, En2> Multiply<Str1, En1, Str2, En2, Str3, En3>
            (Str3 l,
            TernaryQuotientUnit<Numerator<CompositeProduct<Str1, Str2>>, Str1, En1, Str2, En2, Str3, En3> r)
            where En1 : Enum
            where En2 : Enum
            where En3 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
            where Str3 : struct, ILinearUnit<Str3, En3>
            => Multiply(r, l);
        public static QuotientUnit<Str1, En1, Str3, En3> Divide<Str1, En1, Str2, En2, Str3, En3>
            (Str2 l,
            TernaryQuotientUnit<Numerator<CompositeProduct<Str1, Str2>>, Str1, En1, Str2, En2, Str3, En3> r)
            where En1 : Enum
            where En2 : Enum
            where En3 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
            where Str3 : struct, ILinearUnit<Str3, En3>
        {
            if (l.Original.ScaleOrdinal != r.Scales.Ordinal2)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude / r.Magnitude;
            var unit = QuotientUnit<Str1, En1, Str3, En3>.Create(mag);
            return unit.SetScales(r.Scales.Scale1, r.Scales.Scale3);
        }
        public static QuotientUnit<Str2, En2, Str3, En3> Divide<Str1, En1, Str2, En2, Str3, En3>
            (Str1 l,
            TernaryQuotientUnit<Numerator<CompositeProduct<Str1, Str2>>, Str1, En1, Str2, En2, Str3, En3> r)
            where En1 : Enum
            where En2 : Enum
            where En3 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
            where Str3 : struct, ILinearUnit<Str3, En3>
        {
            if (l.Original.ScaleOrdinal != r.Scales.Ordinal1)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude / r.Magnitude;
            var unit = QuotientUnit<Str2, En2, Str3, En3>.Create(mag);
            return unit.SetScales(r.Scales.Scale2, r.Scales.Scale3);
        }
        #endregion

        #region A/BC
        public static TernaryQuotientUnit<Denominator<CompositeProduct<Str2, Str3>>, Str1, En1, Str2, En2, Str3, En3> Divide<Str1, En1, Str2, En2, Str3, En3>
            (Str1 l, En1 lEn, ProductUnit<Str2, En2, Str3, En3> r)
            where En1 : Enum
            where En2 : Enum
            where En3 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
            where Str3 : struct, ILinearUnit<Str3, En3>
        {
            double mag = l.Original.Magnitude / r.Original.Magnitude;
            var unit = TernaryQuotientUnit<Denominator<CompositeProduct<Str2, Str3>>, Str1, En1, Str2, En2, Str3, En3>.Create(mag);
            return unit.SetScales(l.Original.Scale, r.Original.Scale1, r.Original.Scale2);
        }
        public static QuotientUnit<Str1, En1, Str2, En2> Multiply<Str1, En1, Str2, En2, Str3, En3>
            (TernaryQuotientUnit<Denominator<CompositeProduct<Str2, Str3>>, Str1, En1, Str2, En2, Str3, En3> l,
            Str3 r)
            where En1 : Enum
            where En2 : Enum
            where En3 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
            where Str3 : struct, ILinearUnit<Str3, En3>
        {
            if (l.Scales.Ordinal3 != r.Original.ScaleOrdinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Magnitude * r.Original.Magnitude;
            var unit = QuotientUnit<Str1, En1, Str2, En2>.Create(mag);
            return unit.SetScales(l.Scales.Scale1, l.Scales.Scale2);
        }
        public static QuotientUnit<Str1, En1, Str2, En2> Multiply<Str1, En1, Str2, En2, Str3, En3>
            (Str3 l,
            TernaryQuotientUnit<Denominator<CompositeProduct<Str2, Str3>>, Str1, En1, Str2, En2, Str3, En3> r)
            where En1 : Enum
            where En2 : Enum
            where En3 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
            where Str3 : struct, ILinearUnit<Str3, En3>
            => Multiply(r, l);
        public static QuotientUnit<Str1, En1, Str3, En3> Multiply<Str1, En1, Str2, En2, Str3, En3>
            (TernaryQuotientUnit<Denominator<CompositeProduct<Str2, Str3>>, Str1, En1, Str2, En2, Str3, En3> l,
            Str2 r)
            where En1 : Enum
            where En2 : Enum
            where En3 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
            where Str3 : struct, ILinearUnit<Str3, En3>
        {
            if (l.Scales.Ordinal2 != r.Original.ScaleOrdinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Magnitude * r.Original.Magnitude;
            var unit = QuotientUnit<Str1, En1, Str3, En3>.Create(mag);
            return unit.SetScales(l.Scales.Scale1, l.Scales.Scale3);
        }
        public static QuotientUnit<Str1, En1, Str3, En3> Multiply<Str1, En1, Str2, En2, Str3, En3>
            (Str2 l,
            TernaryQuotientUnit<Denominator<CompositeProduct<Str2, Str3>>, Str1, En1, Str2, En2, Str3, En3> r)
            where En1 : Enum
            where En2 : Enum
            where En3 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
            where Str3 : struct, ILinearUnit<Str3, En3>
            => Multiply(r, l);
        public static ProductUnit<Str2, En2, Str3, En3> Divide<Str1, En1, Str2, En2, Str3, En3>
            (Str1 l,
            TernaryQuotientUnit<Denominator<CompositeProduct<Str2, Str3>>, Str1, En1, Str2, En2, Str3, En3> r)
            where En1 : Enum
            where En2 : Enum
            where En3 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
            where Str3 : struct, ILinearUnit<Str3, En3>
        {
            if (l.Original.ScaleOrdinal != r.Scales.Ordinal1)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude / r.Magnitude;
            var unit = ProductUnit<Str2, En2, Str3, En3>.Create(mag);
            return unit.SetScales(r.Scales.Scale2, r.Scales.Scale3);
        }
        #endregion

        #region QUATERNARY UNITS
        public static QuotientUnit<Str1, En1, Str3, En3> Multiply<Str1, En1, Str2, En2, Str3, En3, Str4, En4>
            (QuotientUnit<Str4, En4, Str2, En2> l,
            QuaternaryQuotientUnit<
                BinaryComposite<
                    Numerator<CompositeProduct<Str1, Str2>>,
                    Denominator<CompositeProduct<Str3, Str4>>>,
                Str1, En1, Str2, En2, Str3, En3, Str4, En4> r)
            where En1 : Enum
            where En2 : Enum
            where En3 : Enum
            where En4 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
            where Str3 : struct, ILinearUnit<Str3, En3>
            where Str4 : struct, ILinearUnit<Str4, En4>
        {
            if (l.Original.Scale1Ordinal != r.Scales.Ordinal4 ||
                l.Original.Scale2Ordinal != r.Scales.Ordinal2)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude / r.Magnitude;
            var unit = QuotientUnit<Str1, En1, Str3, En3>.Create(mag);
            return unit.SetScales(r.Scales.Scale1, r.Scales.Scale3);
        }
        public static QuotientUnit<Str1, En1, Str3, En3> Multiply<Str1, En1, Str2, En2, Str3, En3, Str4, En4>
            (QuaternaryQuotientUnit<
                BinaryComposite<
                    Numerator<CompositeProduct<Str1, Str2>>,
                    Denominator<CompositeProduct<Str3, Str4>>>,
                Str1, En1, Str2, En2, Str3, En3, Str4, En4> l,
            QuotientUnit<Str4, En4, Str2, En2> r)
            where En1 : Enum
            where En2 : Enum
            where En3 : Enum
            where En4 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
            where Str3 : struct, ILinearUnit<Str3, En3>
            where Str4 : struct, ILinearUnit<Str4, En4>
            => Multiply(r, l);
        #endregion

        #region SPECIALIZED
        //  AB'  =   AB/C * C/B'
        //  Where B = B'^2 and B' = Sqrt(B)
        //  Where B & B' have the same base unit.
        public static ProductUnit<Str1, En1, Str2P, En2P> Multiply<Str1, En1, Str2, En2, Str3, En3, Str2P, En2P>
            (TernaryQuotientUnit<
                Numerator<CompositeProduct<Str1, Str2>>,
                Str1, En1, Str2, En2, Str3, En3> l,
            QuotientUnit<Str3, En3, Str2P, En2P> r)
            where En1 : Enum
            where En2 : Enum
            where En3 : Enum
            where En2P : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
            where Str3 : struct, ILinearUnit<Str3, En3>
            where Str2P : struct, ILinearUnit<Str2P, En2P>
        {
            if (typeof(Str2) != typeof(UnitSquared<Str2P, En2P>))
                throw new InvalidOperationException(ErrorMsg.GENERIC_PARAMETER_TYPE_MISMATCH);

            if (l.Scales.Ordinal2 != r.Original.Scale2Ordinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Magnitude * r.Original.Magnitude;
            var unit = ProductUnit<Str1, En1, Str2P, En2P>.Create(mag);
            return unit.SetScales(l.Scales.Scale1, r.Original.Scale2);
        }
        public static ProductUnit<Str1, En1, Str2P, En2P> Multiply<Str1, En1, Str2, En2, Str3, En3, Str2P, En2P>
            (QuotientUnit<Str3, En3, Str2P, En2P> l,
            TernaryQuotientUnit<
                Numerator<CompositeProduct<Str1, Str2>>,
                Str1, En1, Str2, En2, Str3, En3> r)
            where En1 : Enum
            where En2 : Enum
            where En3 : Enum
            where En2P : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
            where Str3 : struct, ILinearUnit<Str3, En3>
            where Str2P : struct, ILinearUnit<Str2P, En2P>
            => Multiply(r, l);
        #endregion
    }
    /// <summary>
    /// Represents a concrete composite operator strategy specializing in multi-factor multiplication operations across composite unit structures.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="CompositeMultiplication"/> inherits from <see cref="CompositeOperator"/> to identify and handle algebraic 
    /// product evaluations within the dimensional framework's composite calculation pipeline.
    /// </para>
    /// <para>
    /// It binds the abstract <see cref="CompositeOperator.Operator"/> property to <see cref="CompositeOperators.Multiplication"/>, 
    /// allowing runtime and static operator dispatchers to categorize and execute multiplicative transmutations and compound factor reductions.
    /// </para>
    /// </remarks>
    public class CompositeMultiplication : CompositeOperator
    {
        public override CompositeOperators Operator => CompositeOperators.Multiplication;
    }
    /// <summary>
    /// Represents a concrete composite operator strategy specializing in multi-factor division operations across composite unit structures.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="CompositeDivision"/> inherits from <see cref="CompositeOperator"/> to identify and handle algebraic 
    /// quotient evaluations within the dimensional framework's composite calculation pipeline.
    /// </para>
    /// <para>
    /// It binds the abstract <see cref="CompositeOperator.Operator"/> property to <see cref="CompositeOperators.Division"/>, 
    /// allowing runtime and static operator dispatchers to categorize and execute division transmutations and dimensional reductions.
    /// </para>
    /// </remarks>
    public class CompositeDivision : CompositeOperator
    {
        public override CompositeOperators Operator => CompositeOperators.Division;

    }
    /// <summary>
    /// Provides static reduction algorithms and mathematical operation delegates for higher-order composite unit structures.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="CompositeReductor"/> class handles mathematical reduction—specifically multiplication and division—across 
    /// multi-variable unit types including squared structures, binary products, quotients, and ternary composite relationships.
    /// </para>
    /// <para>
    /// It enforces strict ordinal scale compatibility across constituent terms before executing magnitude arithmetic, ensuring 
    /// complex algebraic expressions resolve safely back into their base linear units or scalar values.
    /// </para>
    /// </remarks>
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

        #region (A^2)/B
        public static Str1 Multiply<Str1, En1, Str2, En2>
            (QuotientUnit<UnitSquared<Str1, En1>, En1, Str2, En2> l,
            QuotientUnit<Str2, En2, Str1, En1> r)
            where En1 : Enum
            where En2 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
        {//     A   =   (A^2)/B * B/A

            if (l.Original.Scale1Ordinal != r.Original.Scale1Ordinal ||
                l.Original.Scale2Ordinal != r.Original.Scale2Ordinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude * r.Original.Magnitude;
            return Str1.Create(mag, l.Original.Scale1);
        }
        public static Str1 Multiply<Str1, En1, Str2, En2>
            (QuotientUnit<Str2, En2, Str1, En1> l,
            QuotientUnit<UnitSquared<Str1, En1>, En1, Str2, En2> r)
            where En1 : Enum
            where En2 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
            => Multiply(r, l);
        public static Str2 Divide<Str1, En1, Str2, En2>
            (UnitSquared<Str1, En1> l,
            QuotientUnit<UnitSquared<Str1, En1>, En1, Str2, En2> r)
            where En1 : Enum
            where En2 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
        {//     B   =   A^2 / (A^2)/B

            if (l.Original.ScaleOrdinal != r.Original.Scale1Ordinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude / r.Original.Magnitude;
            return Str2.Create(mag, r.Original.Scale2);
        }
        public static double Divide<Str1, En1, Str2, En2>
            (QuotientUnit<UnitSquared<Str1, En1>, En1, Str2, En2> l,
            QuotientUnit<UnitSquared<Str1, En1>, En1, Str2, En2> r)
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

        #region (A^2)/(B^2)
        public static Str1 Multiply<Str1, En1, Str2, En2>
            (QuotientUnit<UnitSquared<Str1, En1>, En1, UnitSquared<Str2, En2>, En2> l,
            QuotientUnit<UnitSquared<Str2, En2>, En2, Str1, En1> r)
            where En1 : Enum
            where En2 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
        {//     A   =   (A^2)/(B^2) * (B^2)/A

            if (l.Original.Scale1Ordinal != r.Original.Scale1Ordinal ||
                l.Original.Scale2Ordinal != r.Original.Scale2Ordinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude * r.Original.Magnitude;
            return Str1.Create(mag, l.Original.Scale1);
        }
        public static Str1 Multiply<Str1, En1, Str2, En2>
            (QuotientUnit<UnitSquared<Str2, En2>, En2, Str1, En1> l,
            QuotientUnit<UnitSquared<Str1, En1>, En1, UnitSquared<Str2, En2>, En2> r)
            where En1 : Enum
            where En2 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
            => Multiply(r, l);
        public static Str2 Divide<Str1, En1, Str2, En2>
            (QuotientUnit<UnitSquared<Str1, En1>, En1, Str2, En2> l,
            QuotientUnit<UnitSquared<Str1, En1>, En1, UnitSquared<Str2, En2>, En2> r)
            where En1 : Enum
            where En2 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
        {//     B   =   (A^2)/B / (A^2)/(B^2)

            if (l.Original.Scale1Ordinal != r.Original.Scale1Ordinal ||
                l.Original.Scale2Ordinal != r.Original.Scale2Ordinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude / r.Original.Magnitude;
            return Str2.Create(mag, l.Original.Scale2);
        }
        public static double Divide<Str1, En1, Str2, En2>
            (QuotientUnit<UnitSquared<Str1, En1>, En1, UnitSquared<Str2, En2>, En2> l,
            QuotientUnit<UnitSquared<Str1, En1>, En1, UnitSquared<Str2, En2>, En2> r)
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

        #region AB * C
        public static Str1 Divide<Str1, En1, Str2, En2, Str3, En3>
            (TernaryProductUnit<CompositeProduct<Str1, Str2>, Str1, En1, Str2, En2, Str3, En3> l,
            ProductUnit<Str2, En2, Str3, En3> r)
            where En1 : Enum
            where En2 : Enum
            where En3 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
            where Str3 : struct, ILinearUnit<Str3, En3>
        {//     A   =   ABC / BC

            if (l.Scales.Ordinal2 != r.Original.Scale1Ordinal ||
                l.Scales.Ordinal3 != r.Original.Scale2Ordinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Magnitude / r.Original.Magnitude;
            return Str1.Create(mag, l.Scales.Scale1);
        }
        public static Str2 Divide<Str1, En1, Str2, En2, Str3, En3>
            (TernaryProductUnit<CompositeProduct<Str1, Str2>, Str1, En1, Str2, En2, Str3, En3> l,
            ProductUnit<Str1, En1, Str3, En3> r)
            where En1 : Enum
            where En2 : Enum
            where En3 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
            where Str3 : struct, ILinearUnit<Str3, En3>
        {//     B   =   ABC / AC

            if (l.Scales.Ordinal1 != r.Original.Scale1Ordinal ||
                l.Scales.Ordinal3 != r.Original.Scale2Ordinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Magnitude / r.Original.Magnitude;
            return Str2.Create(mag, l.Scales.Scale2);
        }
        public static Str3 Divide<Str1, En1, Str2, En2, Str3, En3>
            (TernaryProductUnit<CompositeProduct<Str1, Str2>, Str1, En1, Str2, En2, Str3, En3> l,
            ProductUnit<Str1, En1, Str2, En2> r)
            where En1 : Enum
            where En2 : Enum
            where En3 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
            where Str3 : struct, ILinearUnit<Str3, En3>
        {//     C   =   ABC / AB

            if (l.Scales.Ordinal1 != r.Original.Scale1Ordinal ||
                l.Scales.Ordinal2 != r.Original.Scale2Ordinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Magnitude / r.Original.Magnitude;
            return Str3.Create(mag, l.Scales.Scale3);
        }
        #endregion

        #region AB / C
        public static Str1 Divide<Str1, En1, Str2, En2, Str3, En3>
            (QuotientUnit<Str3, En3, Str2, En2> l,
            TernaryQuotientUnit<Numerator<CompositeProduct<Str1, Str2>>, Str1, En1, Str2, En2, Str3, En3> r)
            where En1 : Enum
            where En2 : Enum
            where En3 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
            where Str3 : struct, ILinearUnit<Str3, En3>
        {//     A   =   C/B / (AB/C)

            if (l.Original.Scale1Ordinal != r.Scales.Ordinal3 ||
                l.Original.Scale2Ordinal != r.Scales.Ordinal2)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude / r.Magnitude;
            return Str1.Create(mag, r.Scales.Scale1);
        }
        public static Str1 Multiply<Str1, En1, Str2, En2, Str3, En3>
            (TernaryQuotientUnit<Numerator<CompositeProduct<Str1, Str2>>, Str1, En1, Str2, En2, Str3, En3> l,
            QuotientUnit<Str3, En3, Str2, En2> r)
            where En1 : Enum
            where En2 : Enum
            where En3 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
            where Str3 : struct, ILinearUnit<Str3, En3>
        {//     A   =   (AB/C) * (C/B)
            if (r.Original.Scale1Ordinal != l.Scales.Ordinal3 ||
                r.Original.Scale2Ordinal != l.Scales.Ordinal2)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Magnitude * r.Original.Magnitude;
            return Str1.Create(mag, l.Scales.Scale1);
        }
        public static Str2 Divide<Str1, En1, Str2, En2, Str3, En3>
            (QuotientUnit<Str3, En3, Str1, En1> l,
            TernaryQuotientUnit<Numerator<CompositeProduct<Str1, Str2>>, Str1, En1, Str2, En2, Str3, En3> r)
            where En1 : Enum
            where En2 : Enum
            where En3 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
            where Str3 : struct, ILinearUnit<Str3, En3>
        {//     B   =   C/A / (AB/C)

            if (l.Original.Scale1Ordinal != r.Scales.Ordinal3 ||
                l.Original.Scale2Ordinal != r.Scales.Ordinal1)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude / r.Magnitude;
            return Str2.Create(mag, r.Scales.Scale2);
        }
        public static Str3 Divide<Str1, En1, Str2, En2, Str3, En3>
            (QuotientUnit<Str1, En1, Str2, En2> l,
            TernaryQuotientUnit<Numerator<CompositeProduct<Str1, Str2>>, Str1, En1, Str2, En2, Str3, En3> r)
            where En1 : Enum
            where En2 : Enum
            where En3 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
            where Str3 : struct, ILinearUnit<Str3, En3>
        {//     C   =   AB / (AB/C)

            if (l.Original.Scale1Ordinal != r.Scales.Ordinal1 ||
                l.Original.Scale2Ordinal != r.Scales.Ordinal2)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude / r.Magnitude;
            return Str3.Create(mag, r.Scales.Scale3);
        }
        #endregion

        #region A / BC
        public static Str1 Multiply<Str1, En1, Str2, En2, Str3, En3>
            (TernaryQuotientUnit<Denominator<CompositeProduct<Str2, Str3>>, Str1, En1, Str2, En2, Str3, En3> l,
            ProductUnit<Str2, En2, Str3, En3> r)
            where En1: Enum
            where En2: Enum
            where En3: Enum
            where Str1: struct, ILinearUnit<Str1, En1>
            where Str2: struct, ILinearUnit<Str2, En2>
            where Str3: struct, ILinearUnit<Str3, En3>
        {//     A   =   (A/BC) * BC

            if (l.Scales.Ordinal2 != r.Original.Scale1Ordinal ||
                l.Scales.Ordinal3 != r.Original.Scale2Ordinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Magnitude * r.Original.Magnitude;
            return Str1.Create(mag, l.Scales.Scale1);
        }
        public static Str1 Multiply<Str1, En1, Str2, En2, Str3, En3>
            (ProductUnit<Str2, En2, Str3, En3> l,
            TernaryQuotientUnit<Denominator<CompositeProduct<Str2, Str3>>, Str1, En1, Str2, En2, Str3, En3> r)
            where En1 : Enum
            where En2 : Enum
            where En3 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
            where Str3 : struct, ILinearUnit<Str3, En3>
            => Multiply(r, l);
        public static Str2 Divide<Str1, En1, Str2, En2, Str3, En3>
            (QuotientUnit<Str3, En3, Str1, En1> l,
            TernaryQuotientUnit<Denominator<CompositeProduct<Str2, Str3>>, Str1, En1, Str2, En2, Str3, En3> r)
            where En1 : Enum
            where En2 : Enum
            where En3 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
            where Str3 : struct, ILinearUnit<Str3, En3>
        {//     B   =   C/A / (A/BC)

            if (l.Original.Scale1Ordinal != r.Scales.Ordinal3 ||
                l.Original.Scale2Ordinal != r.Scales.Ordinal1)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude / r.Magnitude;
            return Str2.Create(mag, r.Scales.Scale2);
        }
        public static Str3 Divide<Str1, En1, Str2, En2, Str3, En3>
            (QuotientUnit<Str2, En2, Str1, En1> l,
            TernaryQuotientUnit<Denominator<CompositeProduct<Str2, Str3>>, Str1, En1, Str2, En2, Str3, En3> r)
            where En1 : Enum
            where En2 : Enum
            where En3 : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
            where Str3 : struct, ILinearUnit<Str3, En3>
        {//     C   =   B/A / (A/BC)

            if (l.Original.Scale1Ordinal != r.Scales.Ordinal2 ||
                l.Original.Scale2Ordinal != r.Scales.Ordinal1)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude / r.Magnitude;
            return Str3.Create(mag, r.Scales.Scale3);
        }
        #endregion
    }
    #endregion

    #region DIVISION OPERANDS
    /// <summary>
    /// Represents the base operand abstraction for structured composite unit division expressions.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="DivisionOperands"/> implements <see cref="ICompositeUnit"/> to serve as the foundational type 
    /// for specialized division terms within the composite calculation framework.
    /// </para>
    /// <para>
    /// It acts as the non-generic parent class for operand wrappers such as <see cref="Numerator{TComp}"/> 
    /// and <see cref="Denominator{TComp}"/>, providing a unified contract for algebraic expression trees and dispatcher pipelines.
    /// </para>
    /// </remarks>
    public class DivisionOperands : ICompositeUnit { }
    /// <summary>
    /// Represents the numerator component of a composite unit division operation.
    /// </summary>
    /// <typeparam name="TComp">The underlying composite unit type occupying the numerator position.</typeparam>
    /// <remarks>
    /// <para>
    /// <see cref="Numerator{TComp}"/> inherits from <see cref="DivisionOperands"/> to wrap and identify the primary 
    /// dividend term within a composite dimensional division evaluation.
    /// </para>
    /// <para>
    /// It constrains <typeparamref name="TComp"/> to valid <see cref="ICompositeUnit"/> implementations, allowing 
    /// runtime dispatchers and reduction routines to safely extract and resolve numerator dimensional factors.
    /// </para>
    /// </remarks>
    public sealed class Numerator<TComp> : DivisionOperands
        where TComp : class, ICompositeUnit
    { }
    /// <summary>
    /// Represents the denominator component of a composite unit division operation.
    /// </summary>
    /// <typeparam name="TComp">The underlying composite unit type occupying the denominator position.</typeparam>
    /// <remarks>
    /// <para>
    /// <see cref="Denominator{TComp}"/> inherits from <see cref="DivisionOperands"/> to wrap and identify the primary 
    /// divisor term within a composite dimensional division evaluation.
    /// </para>
    /// <para>
    /// It constrains <typeparamref name="TComp"/> to valid <see cref="ICompositeUnit"/> implementations, allowing 
    /// runtime dispatchers and reduction routines to safely extract and invert denominator dimensional factors.
    /// </para>
    /// </remarks>
    public sealed class Denominator<TComp> : DivisionOperands
        where TComp : class, ICompositeUnit
    { }
    #endregion

    /// <summary>
    /// Represents a composite unit structure formed by the dimensional product of two concrete value-type units.
    /// </summary>
    /// <typeparam name="Str1">The first underlying struct unit type, representing the primary multiplicand factor.</typeparam>
    /// <typeparam name="Str2">The second underlying struct unit type, representing the secondary multiplier factor.</typeparam>
    /// <remarks>
    /// <para>
    /// <see cref="CompositeProduct{Str1, Str2}"/> implements <see cref="ICompositeUnit"/> to encapsulate and evaluate binary 
    /// multiplicative relationships across concrete dimensional units within the calculation pipeline.
    /// </para>
    /// <para>
    /// It constrains both <typeparamref name="Str1"/> and <typeparamref name="Str2"/> to value types implementing <see cref="IDimensionAccessible"/> 
    /// and <see cref="INormalizable{TSelf}"/>, ensuring that constituent factors can be queried and normalized during composite reduction operations.
    /// </para>
    /// </remarks>
    public class CompositeProduct<Str1, Str2> : ICompositeUnit
        where Str1 : struct, IDimensionAccessible, INormalizable<Str1>
        where Str2 : struct, IDimensionAccessible, INormalizable<Str2> 
    { }
    /// <summary>
    /// Represents a composite unit structure formed by the dimensional quotient of two concrete value-type units.
    /// </summary>
    /// <typeparam name="Str1">The underlying struct unit type occupying the dividend (numerator) position.</typeparam>
    /// <typeparam name="Str2">The underlying struct unit type occupying the divisor (denominator) position.</typeparam>
    /// <remarks>
    /// <para>
    /// <see cref="CompositeQuotient{Str1, Str2}"/> implements <see cref="ICompositeUnit"/> to encapsulate and evaluate binary 
    /// division relationships across concrete dimensional units within the calculation pipeline.
    /// </para>
    /// <para>
    /// It constrains both <typeparamref name="Str1"/> and <typeparamref name="Str2"/> to value types implementing <see cref="IDimensionAccessible"/> 
    /// and <see cref="INormalizable{TSelf}"/>, ensuring that ratio components can be queried and normalized during composite reduction operations.
    /// </para>
    /// </remarks>
    public class CompositeQuotient<Str1, Str2> : ICompositeUnit
        where Str1 : struct, IDimensionAccessible, INormalizable<Str1>
        where Str2 : struct, IDimensionAccessible, INormalizable<Str2>
    { }
    /// <summary>
    /// Represents a higher-order composite unit structure composed of two nested composite unit expressions.
    /// </summary>
    /// <typeparam name="TComp1">The first underlying composite unit expression.</typeparam>
    /// <typeparam name="TComp2">The second underlying composite unit expression.</typeparam>
    /// <remarks>
    /// <para>
    /// <see cref="BinaryComposite{TComp1, TComp2}"/> implements <see cref="ICompositeUnit"/> to model complex binary trees 
    /// composed of existing composite unit structures within the dimensional framework.
    /// </para>
    /// <para>
    /// It constrains both <typeparamref name="TComp1"/> and <typeparamref name="TComp2"/> to class types implementing <see cref="ICompositeUnit"/>, 
    /// allowing runtime dispatchers and expression processors to recursively analyze and flatten deeply nested operational structures.
    /// </para>
    /// </remarks>
    public class BinaryComposite<TComp1, TComp2> : DivisionOperands, ICompositeUnit
        where TComp1 : class, ICompositeUnit
        where TComp2 : class, ICompositeUnit
    { }

    public class BinaryOperator
    {
        //  AB' =   A/B' * B
        //  Where B = B'^2 and B' = Sqrt(B)
        //  Where B and B' have the same base unit
        public static ProductUnit<Str1, En1, Str2P, En2P> Multiply<Str1, En1, Str2, En2, Str2P, En2P>
            (QuotientUnit<Str1, En1, Str2P, En2P> l, (Str2 Unit, En2 Scale) r)
            where En1: Enum
            where En2: Enum
            where En2P: Enum
            where Str1: struct, ILinearUnit<Str1, En1>
            where Str2: struct, ILinearUnit<Str2, En2>
            where Str2P: struct, ILinearUnit<Str2P, En2P>
        {
            if (typeof(Str2) != typeof(UnitSquared<Str2P, En2P>))
                throw new InvalidOperationException(ErrorMsg.GENERIC_PARAMETER_TYPE_MISMATCH);

            if (l.Original.Scale2Ordinal != r.Unit.Normalized.ScaleOrdinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude * r.Unit.Normalized.Magnitude;
            var unit = ProductUnit<Str1, En1, Str2P, En2P>.Create(mag);
            return unit.SetScales(l.Original.Scale1, l.Original.Scale2);
        }
        public static ProductUnit<Str1, En1, Str2P, En2P> Multiply<Str1, En1, Str2, En2, Str2P, En2P>
            ((Str2 Unit, En2 Scale) l, QuotientUnit<Str1, En1, Str2P, En2P> r)
            where En1 : Enum
            where En2 : Enum
            where En2P : Enum
            where Str1 : struct, ILinearUnit<Str1, En1>
            where Str2 : struct, ILinearUnit<Str2, En2>
            where Str2P : struct, ILinearUnit<Str2P, En2P>
            => Multiply(r, l);
    }
}
