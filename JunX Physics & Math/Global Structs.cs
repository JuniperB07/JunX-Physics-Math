using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Quic;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Transactions;
using System.Xml.Schema;
using JunX.Mathematics;

namespace JunX
{
    /// <summary>
    /// Represents a static identity marker and unit magnitude constant for zero-dimensional scalar physical quantities.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="DimensionlessOne"/> serves as a lightweight, immutable value type contract defining an invariant base scale magnitude of 1.0 for non-dimensional scalar operations.
    /// </para>
    /// <para>
    /// It is primarily utilized within physical measurement frameworks to provide unit unity values during scale conversions, non-dimensional ratio evaluations, normalized product cancellation pipelines, and dimensionless quantity initializations.
    /// </para>
    /// </remarks>
    public readonly struct DimensionlessOne
    {
        public static double Magnitude => 1.0;
    }

    /// <summary>
    /// Represents a generic one-dimensional inverse or reciprocal physical unit wrapper supporting variable numerators, scale conversions, normalization, linear projection, and higher-order dimensional exponentiation.
    /// </summary>
    /// <typeparam name="Str">The underlying target dimensional struct type implementing physical measurement initialization and normalization contracts.</typeparam>
    /// <typeparam name="En">The enumeration type representing unit scales associated with <typeparamref name="Str"/>.</typeparam>
    /// <remarks>
    /// <para>
    /// <see cref="ReciprocalUnit{Str, En}"/> implements foundational physical measurement contracts including <see cref="IDimensionalUnit"/>, <see cref="INormalizable{TSelf}"/>, 
    /// <see cref="IScaleConvertible{TSelf, TEnum}"/>, <see cref="IValueAccessible{TEnum}"/>, and <see cref="IExponentiable{TSelf, TEnum}"/> to model inverted physical dimensions (X⁻¹) relative to a target struct <typeparamref name="Str"/>.
    /// </para>
    /// <para>
    /// The generic struct wraps inverse dimensional quantities while managing scalar inversion through a configurable <see cref="Numerator"/> (defaulting to 1.0). 
    /// Scale normalization and unit conversions are lazily computed via inverse delegation to the underlying struct <typeparamref name="Str"/>. 
    /// It supports projection back to the linear domain via <see cref="ToLinearUnit"/> (valid when <see cref="Numerator"/> equals zero), relational comparison operators, linear scalar arithmetic, cross-unit division, and higher-order dimensional exponentiation (U⁻² and U⁻ⁿ).
    /// </para>
    /// </remarks>
    public struct ReciprocalUnit<Str, En> :
        IDimensionAccessible,
        IInitializable<ReciprocalUnit<Str, En>>, IInitializable<ReciprocalUnit<Str, En>, double>, IInitializable<ReciprocalUnit<Str, En>, double, En>,
        INormalized<En>, INormalizable<ReciprocalUnit<Str, En>>,
        IScaleConvertible<ReciprocalUnit<Str, En>, En>,
        IDuplicatable<ReciprocalUnit<Str, En>>,
        IValueAccessible<En>,
        IEquatable<ReciprocalUnit<Str, En>>,
        IExponentiable<ReciprocalUnit<Str, En>, En>,
        IDimensionalUnit

        where En : Enum
        where Str : struct, IDimensionAccessible,
            IInitializable<Str>, IInitializable<Str, double>, IInitializable<Str, double, En>,
            INormalized<En>, INormalizable<Str>,
            IScaleConvertible<Str, En>, IValueAccessible<En>
    {
        #region PROPERTIES
        private static int BaseScaleOrdinal
        {
            get
            {
                En bs = BaseScale;
                return Unsafe.As<En, int>(ref bs);
            }
        }

        public int Dimension => 1;
        public double Numerator { get; private set; } = 1.0;

        public static En BaseScale => Str.BaseScale;
        public (double Magnitude, En Scale, int ScaleOrdinal) Normalized
        {
            get
            {
                if (Original.Scale.Equals(BaseScale))
                    return Original;

                double bMag = Numerator / Original.Magnitude;
                Str bUnit = Str.Create(bMag, Original.Scale);

                return (Numerator / bUnit.Normalized.Magnitude,
                    bUnit.Normalized.Scale,
                    bUnit.Normalized.ScaleOrdinal);
            }
        }

        public (double Magnitude, En Scale, int ScaleOrdinal) Original { get; private set; } = (0, BaseScale, BaseScaleOrdinal);
        public (double Magnitude, En Scale, int ScaleOrdinal) Converted { get; private set; } = (0, BaseScale, BaseScaleOrdinal);
        #endregion

        #region CONSTRUCTORS
        public ReciprocalUnit() { }
        public ReciprocalUnit(ReciprocalUnit<Str, En> instance) => this = instance;
        public ReciprocalUnit(double magnitude) => Original = (magnitude, BaseScale, BaseScaleOrdinal);
        public ReciprocalUnit(double magnitude, En scale) => Original = (magnitude, scale, Unsafe.As<En, int>(ref scale));
        #endregion

        #region METHODS
        public static ReciprocalUnit<Str, En> Initialize() => new();
        public static ReciprocalUnit<Str, En> Create(ReciprocalUnit<Str, En> instance) => new(instance);
        public static ReciprocalUnit<Str, En> Create(double magnitude) => new(magnitude);
        public static ReciprocalUnit<Str, En> Create(double magnitude, En scale) => new(magnitude, scale);

        public ReciprocalUnit<Str, En> SetNumerator(double numerator)
        {
            Numerator = numerator;
            return this;
        }

        public ReciprocalUnit<Str, En> Duplicate() => new(this);
        public bool Equals(ReciprocalUnit<Str, En> other)
            => Normalized.Magnitude == other.Normalized.Magnitude;

        public ReciprocalUnit<Str, En> Convert(En toScale)
        {
            if (toScale.Equals(Original.Scale))
            {
                Converted = (Original.Magnitude, Original.Scale, Original.ScaleOrdinal);
                return this;
            }

            double bMag = Numerator / Original.Magnitude;
            Str bUnit = Str.Create(bMag, Original.Scale).Convert(toScale);
            Converted = (1.0 / bUnit.Converted.Magnitude, toScale, Unsafe.As<En, int>(ref toScale));
            return this;
        }
        public double As(En scale) => Duplicate().Convert(scale).Converted.Magnitude;
        public ReciprocalUnit<Str, En> Normalize()
        {
            Original = (Normalized.Magnitude, Normalized.Scale, Normalized.ScaleOrdinal);
            return this;
        }

        public Str ToLinearUnit()
            => Numerator == 0 ?
            Str.Create(Normalized.Magnitude) :
            throw new InvalidOperationException(ErrorMsg.NON_ZERO_RECIPROCAL_NUMERATOR);

        public UnitSquared<ReciprocalUnit<Str, En>, En> Squared() => this * this;
        public UnitCubed<ReciprocalUnit<Str, En>, En> Cubed() => this * this * this;
        public HyperUnit<ReciprocalUnit<Str, En>, En> Pow(int exp)
            => new HyperUnit<ReciprocalUnit<Str, En>, En>(Math.Pow(Normalized.Magnitude, exp)).SetDimension(exp);

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region CONDITIONAL OPERATORS
        public static bool operator ==(ReciprocalUnit<Str, En> l, ReciprocalUnit<Str, En> r) => l.Equals(r);
        public static bool operator !=(ReciprocalUnit<Str, En> l, ReciprocalUnit<Str, En> r) => !l.Equals(r);
        public static bool operator <(ReciprocalUnit<Str, En> l, ReciprocalUnit<Str, En> r)
            => l.Normalized.Magnitude < r.Normalized.Magnitude;
        public static bool operator >(ReciprocalUnit<Str, En> l, ReciprocalUnit<Str, En> r)
            => l.Normalized.Magnitude > r.Normalized.Magnitude;
        public static bool operator <=(ReciprocalUnit<Str, En> l, ReciprocalUnit<Str, En> r) => l < r || l == r;
        public static bool operator >=(ReciprocalUnit<Str, En> l, ReciprocalUnit<Str, En> r) => l > r || l == r;
        #endregion

        #region SELF ARITHMETIC OPERATORS
        public static ReciprocalUnit<Str, En> operator +(ReciprocalUnit<Str, En> l, ReciprocalUnit<Str, En> r)
            => new(l.Normalized.Magnitude + r.Normalized.Magnitude);
        public static ReciprocalUnit<Str, En> operator -(ReciprocalUnit<Str, En> l, ReciprocalUnit<Str, En> r)
            => new(l.Normalized.Magnitude - r.Normalized.Magnitude);
        public static ReciprocalUnit<Str, En> operator *(ReciprocalUnit<Str, En> l, double r)
            => new(l.Normalized.Magnitude * r);
        public static ReciprocalUnit<Str, En> operator *(double l, ReciprocalUnit<Str, En> r) => r * l;
        public static ReciprocalUnit<Str, En> operator /(ReciprocalUnit<Str, En> l, double r)
            => new(l.Normalized.Magnitude / r);
        #endregion

        #region CROSS-DIMENSIONAL ARITHMETIC OPERATORS
        public static UnitSquared<ReciprocalUnit<Str, En>, En> operator *(ReciprocalUnit<Str, En> l, ReciprocalUnit<Str, En> r)
            => new(l.Normalized.Magnitude * r.Normalized.Magnitude);

        public static double operator /(ReciprocalUnit<Str, En> l, ReciprocalUnit<Str, En> r)
            => l.Normalized.Magnitude / r.Normalized.Magnitude;
        #endregion

        #region CROSS-UNIT OPERATORS
        public static ReciprocalUnit<Str, En> operator /(double l, ReciprocalUnit<Str, En> r)
            => l <= r.Numerator ?
            new ReciprocalUnit<Str, En>(l / r.Normalized.Magnitude).SetNumerator(r.Numerator - l) :
            throw new InvalidOperationException(ErrorMsg.INVALID_RECIPROCAL_NUMERATOR);
        #endregion
    }

    /// <summary>
    /// Represents a generic two-dimensional (squared) unit measurement capable of scale conversion, 
    /// normalization, cross-dimensional arithmetic, exponentiation, and dimensional reduction.
    /// </summary>
    /// <typeparam name="Str">
    /// The underlying one-dimensional base unit structure type. Must be a value type implementing 
    /// dimension access, normalization, scale conversion, and initialization contracts.
    /// </typeparam>
    /// <typeparam name="En">
    /// The unit scale enumeration type representing the valid scales or prefixes for the unit.
    /// </typeparam>
    /// <remarks>
    /// <para>
    /// <see cref="UnitSquared{Str, En}"/> serves as a generic wrapper for squared physical quantities 
    /// (such as area, where <typeparamref name="Str"/> represents a linear unit like meters). It provides 
    /// explicit 2D dimensional context (<c>Dimension = 2</c>) and implements generic math contracts.
    /// </para>
    /// <para>
    /// This structure supports scale conversions and normalization by leveraging the transformation 
    /// rules defined by the underlying 1D unit <typeparamref name="Str"/>. High-dimensional products 
    /// and exponentiation operations (such as squaring or raising to an arbitrary power) dynamically 
    /// elevate the measurement into a higher-dimensional <c>HyperUnit&lt;Str, En&gt;</c>.
    /// </para>
    /// </remarks>
    public struct UnitSquared<Str, En> :
        IDimensionAccessible,
        IInitializable<UnitSquared<Str, En>>, IInitializable<UnitSquared<Str, En>, double>, IInitializable<UnitSquared<Str, En>, double, En>,
        INormalized<En>, INormalizable<UnitSquared<Str, En>>,
        IScaleConvertible<UnitSquared<Str, En>, En>,
        IDuplicatable<UnitSquared<Str, En>>,
        IValueAccessible<En>,
        ISquareRootable<Str>,
        IEquatable<UnitSquared<Str, En>>,
        IExponentiable<HyperUnit<Str, En>>,
        IDimensionalUnit,
        ILinearUnit<UnitSquared<Str, En>, En>

        where En : Enum
        where Str : struct, IDimensionAccessible,
            IInitializable<Str>, IInitializable<Str, double>, IInitializable<Str, double, En>,
            INormalized<En>, INormalizable<Str>,
            IScaleConvertible<Str, En>, IValueAccessible<En>
    {
        #region PROPERTIES
        private static int BaseScaleOrdinal
        {
            get
            {
                En bScale = BaseScale;
                return Unsafe.As<En, int>(ref bScale);
            }
        }

        public int Dimension => 2;

        public static En BaseScale => Str.BaseScale;
        public (double Magnitude, En Scale, int ScaleOrdinal) Normalized
        {
            get
            {
                if (Original.Scale.Equals(BaseScale))
                    return Original;

                double baseMag = Math.Sqrt(Original.Magnitude);
                Str baseUnit = Str.Create(baseMag, Original.Scale);

                return (Math.Pow(baseUnit.Normalized.Magnitude, Dimension),
                    baseUnit.Normalized.Scale,
                    baseUnit.Normalized.ScaleOrdinal);
            }
        }

        public (double Magnitude, En Scale, int ScaleOrdinal) Original { get; private set; }
        public (double Magnitude, En Scale, int ScaleOrdinal) Converted { get; private set; }
        #endregion

        #region CONSTRUCTORS
        public UnitSquared()
        {
            Original = (0, BaseScale, BaseScaleOrdinal);
            Converted = Original;
        }
        public UnitSquared(UnitSquared<Str, En> instance)
        {
            this = instance;
        }
        public UnitSquared(double magnitude)
        {
            Original = (magnitude, BaseScale, BaseScaleOrdinal);
            Converted = (0, BaseScale, BaseScaleOrdinal);
        }
        public UnitSquared(double magnitude, En scale)
        {
            Original = (magnitude, scale, Unsafe.As<En, int>(ref scale));
            Converted = (0, BaseScale, BaseScaleOrdinal);
        }
        #endregion

        #region METHODS
        public static UnitSquared<Str, En> Initialize() => new();
        public static UnitSquared<Str, En> Create(UnitSquared<Str, En> instance) => new(instance);
        public static UnitSquared<Str, En> Create(double magnitude) => new(magnitude);
        public static UnitSquared<Str, En> Create(double magnitude, En scale) => new(magnitude, scale);

        public UnitSquared<Str, En> Normalize()
            => new UnitSquared<Str, En>(Normalized.Magnitude, Normalized.Scale);

        public UnitSquared<Str, En> Duplicate() => new(this);
        public bool Equals(UnitSquared<Str, En> us)
            => Normalized.Magnitude == us.Normalized.Magnitude;

        public UnitSquared<Str, En> Convert(En toScale)
        {
            if (toScale.Equals(Original.Scale))
            {
                Converted = Original;
                return this;
            }

            double baseMag = Math.Sqrt(Original.Magnitude);
            Str baseUnit = Str.Create(baseMag, Original.Scale).Convert(toScale);
            Converted = (Math.Pow(baseUnit.Converted.Magnitude, Dimension), toScale, Unsafe.As<En, int>(ref toScale));
            return this;
        }
        public double As(En scale) => Duplicate().Convert(scale).Converted.Magnitude;

        public HyperUnit<Str, En> Squared() => this * this;
        public HyperUnit<Str, En> Cubed() => this * this * this;
        public HyperUnit<Str, En> Pow(int exp)
            => new HyperUnit<Str, En>(Math.Pow(Normalized.Magnitude, exp)).SetDimension(Dimension * exp);

        public Str Sqrt()
            => Str.Create(Math.Sqrt(Normalized.Magnitude));

        public HyperUnit<Str, En> ToHyperUnit()
            => new HyperUnit<Str, En>(Original.Magnitude, Original.Scale).SetDimension(Dimension);

        public QuotientUnit<UnitSquared<Str, En>, En, Str2, En2> Divide<Str2, En2>(Str2 unit, En2 scale)
            where En2 : Enum
            where Str2 : struct, ILinearUnit<Str2, En2>
            => new QuotientUnit<UnitSquared<Str, En>, En, Str2, En2>(Normalized.Magnitude / unit.Normalized.Magnitude)
            .SetScales(Normalized.Scale, scale);
        #endregion

        #region OVERRIDES
        [Obsolete]
        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region CONDITIONAL OPERATORS
        public static bool operator ==(UnitSquared<Str, En> l, UnitSquared<Str, En> r) => l.Equals(r);
        public static bool operator !=(UnitSquared<Str, En> l, UnitSquared<Str, En> r) => !l.Equals(r);
        public static bool operator <(UnitSquared<Str, En> l, UnitSquared<Str, En> r)
            => l.Normalized.Magnitude < r.Normalized.Magnitude;
        public static bool operator >(UnitSquared<Str, En> l, UnitSquared<Str, En> r)
            => l.Normalized.Magnitude > r.Normalized.Magnitude;
        public static bool operator <=(UnitSquared<Str, En> l, UnitSquared<Str, En> r) => l < r || l == r;
        public static bool operator >=(UnitSquared<Str, En> l, UnitSquared<Str, En> r) => l > r || l == r;
        #endregion

        #region SELF ARITHMETIC OPERATORS
        public static UnitSquared<Str, En> operator +(UnitSquared<Str, En> l, UnitSquared<Str, En> r)
            => new(l.Normalized.Magnitude + r.Normalized.Magnitude);
        public static UnitSquared<Str, En> operator -(UnitSquared<Str, En> l, UnitSquared<Str, En> r)
            => new(l.Normalized.Magnitude - r.Normalized.Magnitude);
        public static UnitSquared<Str, En> operator *(UnitSquared<Str, En> l, double r)
            => new(l.Normalized.Magnitude * r);
        public static UnitSquared<Str, En> operator *(double l, UnitSquared<Str, En> r) => r * l;
        public static UnitSquared<Str, En> operator /(UnitSquared<Str, En> l, double r)
            => new(l.Normalized.Magnitude / r);
        #endregion

        #region CROSS-DIMENSIONAL ARITHMETIC OPERATORS
        public static UnitCubed<Str, En> operator *(UnitSquared<Str, En> l, Str r)
            => new(l.Normalized.Magnitude * r.Normalized.Magnitude);
        public static UnitCubed<Str, En> operator *(Str l, UnitSquared<Str, En> r) => r * l;
        public static HyperUnit<Str, En> operator *(UnitSquared<Str, En> l, UnitSquared<Str, En> r)
            => new HyperUnit<Str, En>(l.Normalized.Magnitude * r.Normalized.Magnitude).SetDimension(l.Dimension + r.Dimension);
        public static HyperUnit<Str, En> operator *(UnitSquared<Str, En> l, UnitCubed<Str, En> r)
            => new HyperUnit<Str, En>(l.Normalized.Magnitude * r.Normalized.Magnitude).SetDimension(l.Dimension + r.Dimension);
        public static HyperUnit<Str, En> operator *(UnitSquared<Str, En> l, HyperUnit<Str, En> r)
            => new HyperUnit<Str, En>(l.Normalized.Magnitude * r.Normalized.Magnitude).SetDimension(l.Dimension + r.Dimension);

        public static Str operator /(UnitSquared<Str, En> l, Str r)
            => Str.Create(l.Normalized.Magnitude / r.Normalized.Magnitude);
        public static double operator /(UnitSquared<Str, En> l, UnitSquared<Str, En> r)
            => l.Normalized.Magnitude / r.Normalized.Magnitude;
        #endregion

        #region A/(B^2)
        public static UnitSquared<Str, En> Divide<Str1, En1>
            (Str l,
            QuotientUnit<Str1, En1, UnitSquared<Str, En>, En> r)
            where En1: Enum
            where Str1: struct, ILinearUnit<Str1, En1>
        {//     B^2 =   A / A/(B^2)

            if (l.Original.ScaleOrdinal != r.Original.Scale1Ordinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude / r.Original.Magnitude;
            return UnitSquared<Str, En>.Create(mag, r.Original.Scale2);
        }
        #endregion

        #region (A^2)/B
        public static UnitSquared<Str, En> Multiply<Str2, En2>
            (QuotientUnit<UnitSquared<Str, En>, En, Str2, En2> l,
            Str2 r)
            where En2: Enum
            where Str2: struct, ILinearUnit<Str2, En2>
        {//     A^2 =   (A^2)/B * B

            if (l.Original.Scale2Ordinal != r.Original.ScaleOrdinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude * r.Original.Magnitude;
            return UnitSquared<Str, En>.Create(mag, l.Original.Scale1);
        }
        #endregion
    }

    /// <summary>
    /// Represents a generic three-dimensional (cubed) unit measurement capable of scale conversion, 
    /// normalization, cross-dimensional arithmetic, exponentiation, and dimensional reduction.
    /// </summary>
    /// <typeparam name="Str">
    /// The underlying one-dimensional base unit structure type. Must be a value type implementing 
    /// dimension access, normalization, scale conversion, and initialization contracts.
    /// </typeparam>
    /// <typeparam name="En">
    /// The unit scale enumeration type representing the valid scales or prefixes for the unit.
    /// </typeparam>
    /// <remarks>
    /// <para>
    /// <see cref="UnitCubed{Str, En}"/> serves as a generic wrapper for cubed physical quantities 
    /// (such as volume, where <typeparamref name="Str"/> represents a linear unit like meters). It provides 
    /// explicit 3D dimensional context (<c>Dimension = 3</c>) and implements generic math contracts.
    /// </para>
    /// <para>
    /// This structure supports scale conversions and normalization by leveraging the transformation 
    /// rules defined by the underlying 1D unit <typeparamref name="Str"/>. Multiplication operations 
    /// and power functions dynamically promote the value into higher-dimensional <c>HyperUnit&lt;Str, En&gt;</c> 
    /// instances, while division operations allow dimensional reduction back to lower-dimensional units.
    /// </para>
    /// </remarks>
    public struct UnitCubed<Str, En> :
        IDimensionAccessible,
        IInitializable<UnitCubed<Str, En>>, IInitializable<UnitCubed<Str, En>, double>, IInitializable<UnitCubed<Str, En>, double, En>,
        INormalized<En>, INormalizable<UnitCubed<Str, En>>,
        IScaleConvertible<UnitCubed<Str, En>, En>,
        IDuplicatable<UnitCubed<Str, En>>,
        IValueAccessible<En>,
        ICubeRootable<Str>,
        IEquatable<UnitCubed<Str, En>>,
        IExponentiable<HyperUnit<Str, En>>,
        IDimensionalUnit,
        ILinearUnit<UnitCubed<Str, En>, En>

        where En : Enum
        where Str : struct, IDimensionAccessible,
            IInitializable<Str>, IInitializable<Str, double>, IInitializable<Str, double, En>,
            INormalized<En>, INormalizable<Str>,
            IScaleConvertible<Str, En>, IValueAccessible<En>
    {
        #region PROPERTIES
        private static int BaseScaleOrdinal
        {
            get
            {
                En bScale = BaseScale;
                return Unsafe.As<En, int>(ref bScale);
            }
        }

        public int Dimension => 3;

        public (double Magnitude, En Scale, int ScaleOrdinal) Original { get; private set; }
        public (double Magnitude, En Scale, int ScaleOrdinal) Converted { get; private set; }

        public static En BaseScale => Str.BaseScale;
        public (double Magnitude, En Scale, int ScaleOrdinal) Normalized
        {
            get
            {
                if (Original.Scale.Equals(BaseScale))
                    return (Original.Magnitude, Original.Scale, Original.ScaleOrdinal);

                double baseMag = Radical.Root(Original.Magnitude, Dimension);
                Str baseUnit = Str.Create(baseMag, Original.Scale);

                return (Math.Pow(baseUnit.Normalized.Magnitude, Dimension), BaseScale, BaseScaleOrdinal);
            }
        }
        #endregion

        #region CONSTRUCTORS
        public UnitCubed()
        {
            Original = (0, BaseScale, BaseScaleOrdinal);
            Converted = Original;
        }
        public UnitCubed(UnitCubed<Str, En> instance)
        {
            this = instance;
        }
        public UnitCubed(double magnitude)
        {
            Original = (magnitude, BaseScale, BaseScaleOrdinal);
            Converted = (0, BaseScale, BaseScaleOrdinal);
        }
        public UnitCubed(double magnitude, En scale)
        {
            Original = (magnitude, scale, Unsafe.As<En, int>(ref scale));
            Converted = (0, BaseScale, BaseScaleOrdinal);
        }
        #endregion

        #region METHODS
        public static UnitCubed<Str, En> Initialize() => new();
        public static UnitCubed<Str, En> Create(UnitCubed<Str, En> instance) => new(instance);
        public static UnitCubed<Str, En> Create(double magnitude) => new(magnitude);
        public static UnitCubed<Str, En> Create(double magnitude, En scale) => new(magnitude, scale);

        public UnitCubed<Str, En> Normalize()
            => new UnitCubed<Str, En>(Normalized.Magnitude, Normalized.Scale);

        public UnitCubed<Str, En> Duplicate() => new(this);
        public bool Equals(UnitCubed<Str, En> uc)
            => Normalized.Magnitude == uc.Normalized.Magnitude;

        public UnitCubed<Str, En> Convert(En toScale)
        {
            if(toScale.Equals(Original.Scale))
            {
                Converted = Original;
                return this;
            }

            double baseMag = Radical.Root(Original.Magnitude, Dimension);
            Str baseUnit = Str.Create(baseMag, Original.Scale).Convert(toScale);
            Converted = (Math.Pow(baseUnit.Converted.Magnitude, Dimension), toScale, Unsafe.As<En, int>(ref toScale));
            return this;
        }
        public double As(En scale) => Duplicate().Convert(scale).Converted.Magnitude;

        public HyperUnit<Str, En> Squared() => this * this;
        public HyperUnit<Str, En> Cubed() => this * this * this;
        public HyperUnit<Str, En> Pow(int exp)
            => new HyperUnit<Str, En>(Math.Pow(Normalized.Magnitude, exp)).SetDimension(Dimension * exp);

        public Str CubeRt()
            => Str.Create(Radical.Root(Normalized.Magnitude, Dimension), Normalized.Scale);

        public HyperUnit<Str, En> ToHyperUnit()
            => new HyperUnit<Str, En>(Original.Magnitude, Original.Scale).SetDimension(Dimension);
        #endregion

        #region OVERRIDES
        [Obsolete]
        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region CONDITIONAL OPERATORS
        public static bool operator ==(UnitCubed<Str, En> l, UnitCubed<Str, En> r) => l.Equals(r);
        public static bool operator !=(UnitCubed<Str, En> l, UnitCubed<Str, En> r) => !l.Equals(r);
        public static bool operator <(UnitCubed<Str, En> l, UnitCubed<Str, En> r)
            => l.Normalized.Magnitude < r.Normalized.Magnitude;
        public static bool operator >(UnitCubed<Str, En> l, UnitCubed<Str, En> r)
            => l.Normalized.Magnitude > r.Normalized.Magnitude;
        public static bool operator <=(UnitCubed<Str, En> l, UnitCubed<Str, En> r) => l < r || l == r;
        public static bool operator >=(UnitCubed<Str, En> l, UnitCubed<Str, En> r) => l > r || l == r;
        #endregion

        #region SELF ARITHMETIC OPERATORS
        public static UnitCubed<Str, En> operator +(UnitCubed<Str, En> l, UnitCubed<Str, En> r)
            => new(l.Normalized.Magnitude + r.Normalized.Magnitude);
        public static UnitCubed<Str, En> operator -(UnitCubed<Str, En> l, UnitCubed<Str, En> r)
            => new(l.Normalized.Magnitude - r.Normalized.Magnitude);
        public static UnitCubed<Str, En> operator *(UnitCubed<Str, En> l, double r)
            => new(l.Normalized.Magnitude * r);
        public static UnitCubed<Str, En> operator *(double l, UnitCubed<Str, En> r) => r * l;
        public static UnitCubed<Str, En> operator /(UnitCubed<Str, En> l, double r)
            => new(l.Normalized.Magnitude / r);
        #endregion

        #region CROSS-DIMENSIONAL ARITHMETIC OPERATORS
        public static HyperUnit<Str, En> operator *(UnitCubed<Str, En> l, Str r)
            => new HyperUnit<Str, En>(l.Normalized.Magnitude * r.Normalized.Magnitude).SetDimension(l.Dimension + 1);
        public static HyperUnit<Str, En> operator *(Str l, UnitCubed<Str, En> r) => r * l;
        public static HyperUnit<Str, En> operator *(UnitCubed<Str, En> l, UnitSquared<Str, En> r)
            => new HyperUnit<Str, En>(l.Normalized.Magnitude * r.Normalized.Magnitude).SetDimension(l.Dimension + r.Dimension);
        public static HyperUnit<Str, En> operator *(UnitCubed<Str, En> l, UnitCubed<Str, En> r)
            => new HyperUnit<Str, En>(l.Normalized.Magnitude * r.Normalized.Magnitude).SetDimension(l.Dimension * 2);
        public static HyperUnit<Str, En> operator *(UnitCubed<Str, En> l, HyperUnit<Str, En> r)
            => new HyperUnit<Str, En>(l.Normalized.Magnitude * r.Normalized.Magnitude).SetDimension(l.Dimension + r.Dimension);

        public static double operator /(UnitCubed<Str, En> l, UnitCubed<Str, En> r)
            => l.Normalized.Magnitude / r.Normalized.Magnitude;
        public static UnitSquared<Str, En> operator /(UnitCubed<Str, En> l, Str r)
            => new(l.Normalized.Magnitude / r.Normalized.Magnitude);
        public static Str operator /(UnitCubed<Str, En> l, UnitSquared<Str, En> r)
            => Str.Create(l.Normalized.Magnitude / r.Normalized.Magnitude);
        #endregion
    }

    /// <summary>
    /// Represents a generalized, arbitrary-dimensional unit measurement capable of dynamic dimensional 
    /// transformation, cross-dimensional arithmetic, scale conversion, root extraction, and exponentiation.
    /// </summary>
    /// <typeparam name="Str">
    /// The underlying one-dimensional base unit structure type. Must be a value type implementing 
    /// dimension access, normalization, scale conversion, and initialization contracts.
    /// </typeparam>
    /// <typeparam name="En">
    /// The unit scale enumeration type representing the valid scales or prefixes for the unit.
    /// </typeparam>
    /// <remarks>
    /// <para>
    /// Unlike specialized fixed-dimension types (such as linear units, <c>UnitSquared&lt;Str, En&gt;</c>, 
    /// or <c>UnitCubed&lt;Str, En&gt;</c>), <see cref="HyperUnit{Str, En}"/> tracks its <see cref="Dimension"/> 
    /// dynamically at runtime. This allows it to represent hyperspatial physical quantities (e.g., 4D+ hyper-volumes) 
    /// or results of complex cross-dimensional multiplication and division operations.
    /// </para>
    /// <para>
    /// <see cref="HyperUnit{Str, En}"/> handles automatic unit scaling and scale conversions by leveraging 
    /// the transformation rules defined by the underlying 1D unit <typeparamref name="Str"/>. Arithmetic 
    /// and root-taking operations enforce dimensional validity checks, throwing exceptions when attempting 
    /// operations that yield negative dimensions, invalid roots, or mismatched additive operands.
    /// </para>
    /// </remarks>
    public struct HyperUnit<Str, En> :
        IDimensionAccessible,
        IInitializable<HyperUnit<Str, En>>, IInitializable<HyperUnit<Str, En>, double>, IInitializable<HyperUnit<Str, En>, double, En>,
        INormalized<En>, INormalizable<HyperUnit<Str, En>>,
        IScaleConvertible<HyperUnit<Str, En>, En>,
        IDuplicatable<HyperUnit<Str, En>>,
        IValueAccessible<En>,
        IEquatable<HyperUnit<Str, En>>,
        ISquareRootable<HyperUnit<Str, En>>,
        ICubeRootable<HyperUnit<Str, En>>,
        IRootable<HyperUnit<Str, En>>,
        IExponentiable<HyperUnit<Str, En>>,
        IDimensionalUnit

        where En : Enum
        where Str : struct, IDimensionAccessible,
            IInitializable<Str>, IInitializable<Str, double>, IInitializable<Str, double, En>,
            INormalized<En>, INormalizable<Str>,
            IScaleConvertible<Str, En>, IValueAccessible<En>
    {
        #region PROPERTIES
        private static int BaseScaleOrdinal
        {
            get
            {
                En bS = BaseScale;
                return Unsafe.As<En, int>(ref bS);
            }
        }

        public int Dimension { get; private set; }

        public (double Magnitude, En Scale, int ScaleOrdinal) Original { get; private set; }
        public (double Magnitude, En Scale, int ScaleOrdinal) Converted { get; private set; }

        public static En BaseScale => Str.BaseScale;
        public (double Magnitude, En Scale, int ScaleOrdinal) Normalized
        {
            get
            {
                if (Original.Scale.Equals(BaseScale))
                    return Original;

                double baseMag = Radical.Root(Original.Magnitude, Dimension);
                Str baseUnit = Str.Create(baseMag, Original.Scale);

                return (Math.Pow(baseUnit.Normalized.Magnitude, Dimension),
                    baseUnit.Normalized.Scale, baseUnit.Normalized.ScaleOrdinal);
            }
        }
        #endregion

        #region CONSTRUCTORS
        public HyperUnit()
        {
            Original = (0, BaseScale, BaseScaleOrdinal);
            Converted = Original;
            Dimension = 1;
        }
        public HyperUnit(HyperUnit<Str, En> instance)
        {
            this = instance;
        }
        public HyperUnit(double magnitude)
        {
            Original = (magnitude, BaseScale, BaseScaleOrdinal);
            Converted = (0, BaseScale, BaseScaleOrdinal);
            Dimension = 1;
        }
        public HyperUnit(double magnitude, En scale)
        {
            Original = (magnitude, scale, Unsafe.As<En, int>(ref scale));
            Converted = (0, BaseScale, BaseScaleOrdinal);
            Dimension = 1;
        }
        #endregion

        #region METHODS
        public static HyperUnit<Str, En> Initialize() => new();
        public static HyperUnit<Str, En> Create(HyperUnit<Str, En> instance) => new(instance);
        public static HyperUnit<Str, En> Create(double magnitude) => new(magnitude);
        public static HyperUnit<Str, En> Create(double magnitude, En scale) => new(magnitude, scale);

        public HyperUnit<Str, En> SetDimension(int dimension)
        {
            Dimension = dimension;
            return this;
        }
        public HyperUnit<Str, En> Normalize()
            => new HyperUnit<Str, En>(Normalized.Magnitude, Normalized.Scale).SetDimension(Dimension);

        public HyperUnit<Str, En> Duplicate() => new(this);
        public bool Equals(HyperUnit<Str, En> hu)
            => Normalized.Magnitude == hu.Normalized.Magnitude && Dimension == hu.Dimension;

        public HyperUnit<Str, En> Convert(En toScale)
        {
            if (Original.Scale.Equals(toScale))
            {
                Converted = (Original.Magnitude, Original.Scale, Original.ScaleOrdinal);
                return this;
            }

            double baseMag = Radical.Root(Original.Magnitude, Dimension);
            Str baseUnit = Str.Create(baseMag, Original.Scale).Convert(toScale);
            Converted = (Math.Pow(baseUnit.Converted.Magnitude, Dimension), toScale, Unsafe.As<En, int>(ref toScale));
            return this;
        }
        public double As(En scale) => Duplicate().Convert(scale).Converted.Magnitude;

        public HyperUnit<Str, En> Squared() => this * this;
        public HyperUnit<Str, En> Cubed() => this * this * this;
        public HyperUnit<Str, En> Pow(int exp)
            => new HyperUnit<Str, En>(Math.Pow(Normalized.Magnitude, exp)).SetDimension(Dimension * exp);

        public HyperUnit<Str, En> Sqrt()
        {
            if (Dimension % 2 != 0)
                throw new ArgumentOutOfRangeException(ErrorMsg.NON_ROOTABLE_DIMENSION);
            return new HyperUnit<Str, En>(Math.Sqrt(Normalized.Magnitude)).SetDimension(Dimension / 2);
        }
        public HyperUnit<Str, En> CubeRt()
        {
            if (Dimension % 3 != 0)
                throw new ArgumentOutOfRangeException(ErrorMsg.NON_ROOTABLE_DIMENSION);

            return new HyperUnit<Str, En>(Math.Pow(Normalized.Magnitude, 1.0 / 3.0)).SetDimension(Dimension / 3);
        }
        public HyperUnit<Str, En> Root(int index)
        {
            if (Dimension % index != 0)
                throw new ArgumentOutOfRangeException(ErrorMsg.NON_ROOTABLE_DIMENSION);

            return new HyperUnit<Str, En>(Math.Pow(Normalized.Magnitude, 1.0 / index)).SetDimension(Dimension / index);
        }

        public double ToDimensionless()
            => Dimension == 0 ? Original.Magnitude : throw new InvalidOperationException(ErrorMsg.INVALID_DIMENSION_CASTING);
        public Str ToLinearUnit()
            => Dimension == 1 ?
            Str.Create(Original.Magnitude, Original.Scale) :
            throw new InvalidOperationException(ErrorMsg.INVALID_DIMENSION_CASTING);
        public UnitSquared<Str, En> ToUnitSquared()
            => Dimension == 2 ?
            new UnitSquared<Str, En>(Original.Magnitude, Original.Scale) :
            throw new InvalidOperationException(ErrorMsg.INVALID_DIMENSION_CASTING);
        public UnitCubed<Str, En> ToUnitCubed()
            => Dimension == 3 ?
            new UnitCubed<Str, En>(Original.Magnitude, Original.Scale) :
            throw new InvalidOperationException(ErrorMsg.INVALID_DIMENSION_CASTING);
        #endregion

        #region OVERRIDES
        [Obsolete]
        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region CONDITIONAL OPERATORS
        public static bool operator ==(HyperUnit<Str, En> l, HyperUnit<Str, En> r) => l.Equals(r);
        public static bool operator !=(HyperUnit<Str, En> l, HyperUnit<Str, En> r) => !l.Equals(r);
        public static bool operator <(HyperUnit<Str, En> l, HyperUnit<Str, En> r)
            => l.Dimension == r.Dimension && l.Normalized.Magnitude < r.Normalized.Magnitude;
        public static bool operator >(HyperUnit<Str, En> l, HyperUnit<Str, En> r)
            => l.Dimension == r.Dimension && l.Normalized.Magnitude > r.Normalized.Magnitude;
        public static bool operator <=(HyperUnit<Str, En> l, HyperUnit<Str, En> r) => l < r || l == r;
        public static bool operator >=(HyperUnit<Str, En> l, HyperUnit<Str, En> r) => l > r || l == r;
        #endregion

        #region SELF ARITHMETIC OPERATORS
        public static HyperUnit<Str, En> operator +(HyperUnit<Str, En> l, HyperUnit<Str, En> r)
            => l.Dimension == r.Dimension ?
            new HyperUnit<Str, En>(l.Normalized.Magnitude + r.Normalized.Magnitude).SetDimension(l.Dimension) :
            throw new InvalidOperationException(ErrorMsg.OPERAND_DIMENSION_MISMATCH);
        public static HyperUnit<Str, En> operator -(HyperUnit<Str, En> l, HyperUnit<Str, En> r)
            => l.Dimension == r.Dimension ?
            new HyperUnit<Str, En>(l.Normalized.Magnitude - r.Normalized.Magnitude).SetDimension(l.Dimension) :
            throw new InvalidOperationException(ErrorMsg.OPERAND_DIMENSION_MISMATCH);
        public static HyperUnit<Str, En> operator *(HyperUnit<Str, En> l, double r)
            => new HyperUnit<Str, En>(l.Normalized.Magnitude * r).SetDimension(l.Dimension);
        public static HyperUnit<Str, En> operator *(double l, HyperUnit<Str, En> r) => r * l;
        public static HyperUnit<Str, En> operator /(HyperUnit<Str, En> l, double r)
            => new HyperUnit<Str, En>(l.Normalized.Magnitude / r).SetDimension(l.Dimension);
        #endregion

        #region CROSS-DIMENSIONAL ARITHMETIC OPERATORS
        public static HyperUnit<Str, En> operator *(HyperUnit<Str, En> l, Str r)
            => new HyperUnit<Str, En>(l.Normalized.Magnitude * r.Normalized.Magnitude).SetDimension(l.Dimension + 1);
        public static HyperUnit<Str, En> operator *(Str l, HyperUnit<Str, En> r) => r * l;
        public static HyperUnit<Str, En> operator *(HyperUnit<Str, En> l, UnitSquared<Str, En> r)
            => new HyperUnit<Str, En>(l.Normalized.Magnitude * r.Normalized.Magnitude).SetDimension(l.Dimension + r.Dimension);
        public static HyperUnit<Str, En> operator *(HyperUnit<Str, En> l, UnitCubed<Str, En> r)
            => new HyperUnit<Str, En>(l.Normalized.Magnitude * r.Normalized.Magnitude).SetDimension(l.Dimension + r.Dimension);
        public static HyperUnit<Str, En> operator *(HyperUnit<Str, En> l, HyperUnit<Str, En> r)
            => new HyperUnit<Str, En>(l.Normalized.Magnitude * r.Normalized.Magnitude).SetDimension(l.Dimension + r.Dimension);

        public static HyperUnit<Str, En> operator /(HyperUnit<Str, En> l, HyperUnit<Str, En> r)
        {
            if (r.Dimension > l.Dimension)
                throw new ArgumentOutOfRangeException(nameof(r.Dimension), r.Dimension, ErrorMsg.RESULTING_NEGATIVE_DIMENSION);

            return new HyperUnit<Str, En>(l.Normalized.Magnitude / r.Normalized.Magnitude)
                .SetDimension(l.Dimension - r.Dimension);
        }
        public static HyperUnit<Str, En> operator /(HyperUnit<Str, En> l, UnitCubed<Str, En> r)
            => l.Dimension >= r.Dimension ? l / r.ToHyperUnit() :
            throw new ArgumentOutOfRangeException(ErrorMsg.RESULTING_NEGATIVE_DIMENSION);
        public static HyperUnit<Str, En> operator /(HyperUnit<Str, En> l, UnitSquared<Str, En> r)
            => l.Dimension >= r.Dimension ? l / r.ToHyperUnit() :
            throw new ArgumentOutOfRangeException(ErrorMsg.RESULTING_NEGATIVE_DIMENSION);
        public static HyperUnit<Str, En> operator /(HyperUnit<Str, En> l, Str r)
            => l.Dimension >= 1 ?
            new HyperUnit<Str, En>(l.Normalized.Magnitude / r.Normalized.Magnitude).SetDimension(l.Dimension - 1) :
            throw new ArgumentOutOfRangeException(ErrorMsg.RESULTING_NEGATIVE_DIMENSION);
        #endregion
    }

    /// <summary>
    /// Represents a composite, binary product measurement consisting of two multiplied unit components 
    /// (<typeparamref name="Str1"/> × <typeparamref name="Str2"/>), providing strongly typed cross-unit arithmetic, 
    /// scale management, and dimensional reduction.
    /// </summary>
    /// <typeparam name="Str1">
    /// The underlying structure type representing the primary unit factor in the product measurement.
    /// Must be a value type implementing dimension access, scale conversion, normalization, and initialization contracts.
    /// </typeparam>
    /// <typeparam name="En1">
    /// The unit scale enumeration type representing valid scales or prefixes for <typeparamref name="Str1"/>.
    /// </typeparam>
    /// <typeparam name="Str2">
    /// The underlying structure type representing the secondary unit factor in the product measurement.
    /// Must be a value type implementing dimension access, scale conversion, normalization, and initialization contracts.
    /// </typeparam>
    /// <typeparam name="En2">
    /// The unit scale enumeration type representing valid scales or prefixes for <typeparamref name="Str2"/>.
    /// </typeparam>
    /// <remarks>
    /// <para>
    /// <see cref="ProductUnit{Str1, En1, Str2, En2}"/> models compound physical quantities formed by multiplying 
    /// two distinct linear or higher-dimensional unit factors (e.g., Force × Distance for Torque/Work, or Mass × Acceleration for Force). 
    /// It tracks scale configurations independently for both component units while exposing unified cross-unit operators.
    /// </para>
    /// <para>
    /// The struct supports algebraic dimensional transformations, including commutative transposition via 
    /// <c>Commute()</c>, scalar multiplication/division, and factor cancellation via division by component units 
    /// (<typeparamref name="Str1"/> or <typeparamref name="Str2"/>). Arithmetic operations between composite units 
    /// enforce strict scale alignment across both unit dimensions, throwing an <see cref="InvalidOperationException"/> 
    /// if scales mismatch.
    /// </para>
    /// </remarks>
    public struct ProductUnit<Str1, En1, Str2, En2> :
        IDimensionAccessible,
        IInitializable<ProductUnit<Str1, En1, Str2, En2>>,
        IInitializable<ProductUnit<Str1, En1, Str2, En2>, double>,
        IInitializable<ProductUnit<Str1, En1, Str2, En2>, double, En1>,
        IDuplicatable<ProductUnit<Str1, En1, Str2, En2>>,
        IEquatable<ProductUnit<Str1, En1, Str2, En2>>,
        IScaleValueAccessible<En1, En2>,
        ICompositeUnit

        where Str1: struct, IDimensionAccessible,
            IInitializable<Str1>, IInitializable<Str1, double>, IInitializable<Str1, double, En1>,
            INormalized<En1>, INormalizable<Str1>,
            IScaleConvertible<Str1, En1>, IValueAccessible<En1>
        where Str2: struct, IDimensionAccessible,
            IInitializable<Str2>, IInitializable<Str2, double>, IInitializable<Str2, double, En2>,
            INormalized<En2>, INormalizable<Str2>,
            IScaleConvertible<Str2, En2>, IValueAccessible<En2>
        where En1: Enum
        where En2: Enum
    {
        #region PROPERTIES
        private static int BaseScale1Ordinal
        {
            get
            {
                En1 bs1 = BaseScale1;
                return Unsafe.As<En1, int>(ref bs1);
            }
        }
        private static int BaseScale2Ordinal
        {
            get
            {
                En2 bs2 = BaseScale2;
                return Unsafe.As<En2, int>(ref bs2);
            }
        }

        public int Dimension => 1;

        public static En1 BaseScale1 => Str1.BaseScale;
        public static En2 BaseScale2 => Str2.BaseScale;

        public (double Magnitude, En1 Scale1, En2 Scale2, int Scale1Ordinal, int Scale2Ordinal) Original { get; private set; }
        public (En1 Scale1, En2 Scale2) ScaleValues => (Original.Scale1, Original.Scale2);

        public Type Struct1 = typeof(Str1);
        public Type Struct2 = typeof(Str2);
        public  Type Enum1 = typeof(En1);
        public Type Enum2 = typeof(En2);
        #endregion

        #region CONSTRUCTORS
        public ProductUnit()
        {
            Original = (0, BaseScale1, BaseScale2, BaseScale1Ordinal, BaseScale2Ordinal);
        }
        public ProductUnit(ProductUnit<Str1, En1, Str2, En2> instance)
        {
            this = instance;
        }
        public ProductUnit(double magnitude)
        {
            Original = (magnitude, BaseScale1, BaseScale2, BaseScale1Ordinal, BaseScale2Ordinal);
        }
        public ProductUnit(double magnitude, En1 scale1)
        {
            Original = (magnitude, scale1, BaseScale2, Unsafe.As<En1, int>(ref scale1), BaseScale2Ordinal);
        }
        #endregion

        #region METHODS
        public static ProductUnit<Str1, En1, Str2, En2> Initialize() => new();
        public static ProductUnit<Str1, En1, Str2, En2> Create(ProductUnit<Str1, En1, Str2, En2> instance)
            => new(instance);
        public static ProductUnit<Str1, En1, Str2, En2> Create(double magnitude) => new(magnitude);
        public static ProductUnit<Str1, En1, Str2, En2> Create(double magnitude, En1 scale1) => new(magnitude, scale1);

        public ProductUnit<Str1, En1, Str2, En2> SetScale1(En1 scale1)
        {
            (double mag, En2 s2, int s2Ord) orig = (Original.Magnitude, Original.Scale2, Original.Scale2Ordinal);

            Original = (orig.mag, scale1, orig.s2, Unsafe.As<En1, int>(ref scale1), orig.s2Ord);
            return this;
        }
        public ProductUnit<Str1, En1, Str2, En2> SetScale2(En2 scale2)
        {
            (double mag, En1 s1, int s1Ord) orig = (Original.Magnitude, Original.Scale1, Original.Scale1Ordinal);

            Original = (orig.mag, orig.s1, scale2, orig.s1Ord, Unsafe.As<En2, int>(ref scale2));
            return this;
        }
        public ProductUnit<Str1, En1, Str2, En2> SetScales(En1 scale1, En2 scale2)
        {
            double mag = Original.Magnitude;

            Original = (mag, scale1, scale2, Unsafe.As<En1, int>(ref scale1), Unsafe.As<En2, int>(ref scale2));
            return this;
        }

        public ProductUnit<Str1, En1, Str2, En2> Duplicate() => new(this);
        public bool Equals(ProductUnit<Str1, En1, Str2, En2> pu)
        {
            if (Original.Scale1Ordinal != pu.Original.Scale1Ordinal || Original.Scale2Ordinal != pu.Original.Scale2Ordinal)
                return false;

            return Original.Magnitude == pu.Original.Magnitude;
        }

        public ProductUnit<Str2, En2, Str1, En1> Commute()
            => ProductUnit<Str2, En2, Str1, En1>.Create(Original.Magnitude).SetScales(Original.Scale2, Original.Scale1);
        
        public bool IsEqualTypeParams<Str3, En3, Str4, En4>(ProductUnit<Str3, En3, Str4, En4> other)
            where Str3 : struct, IDimensionAccessible,
                IInitializable<Str3>, IInitializable<Str3, double>, IInitializable<Str3, double, En3>,
                INormalized<En3>, INormalizable<Str3>,
                IScaleConvertible<Str3, En3>, IValueAccessible<En3>
            where Str4 : struct, IDimensionAccessible,
                IInitializable<Str4>, IInitializable<Str4, double>, IInitializable<Str4, double, En4>,
                INormalized<En4>, INormalizable<Str4>,
                IScaleConvertible<Str4, En4>, IValueAccessible<En4>
            where En3 : Enum
            where En4 : Enum
        {
            return
                (Struct1 == other.Struct1 && Struct2 == other.Struct2) &&
                (Enum1 == other.Enum1 && Enum2 == other.Enum2);
        }
        public bool IsEqualScales(ProductUnit<Str1, En1, Str2, En2> other)
            => Original.Scale1Ordinal == other.Original.Scale1Ordinal && Original.Scale2Ordinal == other.Original.Scale2Ordinal;

        public bool IsEqualTo<Str3, En3, Str4, En4>(ProductUnit<Str3, En3, Str4, En4> other)
            where Str3 : struct, IDimensionAccessible,
                IInitializable<Str3>, IInitializable<Str3, double>, IInitializable<Str3, double, En3>,
                INormalized<En3>, INormalizable<Str3>,
                IScaleConvertible<Str3, En3>, IValueAccessible<En3>
            where Str4 : struct, IDimensionAccessible,
                IInitializable<Str4>, IInitializable<Str4, double>, IInitializable<Str4, double, En4>,
                INormalized<En4>, INormalizable<Str4>,
                IScaleConvertible<Str4, En4>, IValueAccessible<En4>
            where En3 : Enum
            where En4 : Enum
        {
            if (!IsEqualTypeParams(other))
                return false;

            return Original.Magnitude == other.Original.Magnitude;
        }
        #endregion

        #region OVERRIDES
        [Obsolete]
        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region CONDITIONAL OPERATORS
        public static bool operator ==(ProductUnit<Str1, En1, Str2, En2> l, ProductUnit<Str1, En1, Str2, En2> r)
            => l.Equals(r);
        public static bool operator !=(ProductUnit<Str1, En1, Str2, En2> l, ProductUnit<Str1, En1, Str2, En2> r)
            => !l.Equals(r);
        public static bool operator <(ProductUnit<Str1, En1, Str2, En2> l, ProductUnit<Str1, En1, Str2, En2> r)
            => l.IsEqualScales(r) ?
            l.Original.Magnitude < r.Original.Magnitude :
            throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);
        public static bool operator >(ProductUnit<Str1, En1, Str2, En2> l, ProductUnit<Str1, En1, Str2, En2> r)
            => l.IsEqualScales(r) ?
            l.Original.Magnitude > r.Original.Magnitude :
            throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);
        public static bool operator <=(ProductUnit<Str1, En1, Str2, En2> l, ProductUnit<Str1, En1, Str2, En2> r) => l < r || l == r;
        public static bool operator >=(ProductUnit<Str1, En1, Str2, En2> l, ProductUnit<Str1, En1, Str2, En2> r) => l > r || l == r;
        #endregion

        #region SELF ARITHMETIC OPERATORS
        public static ProductUnit<Str1, En1, Str2, En2> operator +(ProductUnit<Str1, En1, Str2, En2> l, ProductUnit<Str1, En1, Str2, En2> r)
            => l.IsEqualScales(r) ?
            new ProductUnit<Str1, En1, Str2, En2>(l.Original.Magnitude + r.Original.Magnitude).SetScales(l.Original.Scale1, l.Original.Scale2) :
            throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);
        public static ProductUnit<Str1, En1, Str2, En2> operator -(ProductUnit<Str1, En1, Str2, En2> l, ProductUnit<Str1, En1, Str2, En2> r)
            => l.IsEqualScales(r) ?
            new ProductUnit<Str1, En1, Str2, En2>(l.Original.Magnitude - r.Original.Magnitude).SetScales(l.Original.Scale1, l.Original.Scale2) :
            throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);
        public static ProductUnit<Str1, En1, Str2, En2> operator *(ProductUnit<Str1, En1, Str2, En2> l, double r)
            => new ProductUnit<Str1, En1, Str2, En2>(l.Original.Magnitude * r).SetScales(l.Original.Scale1, l.Original.Scale2);
        public static ProductUnit<Str1, En1, Str2, En2> operator *(double l, ProductUnit<Str1, En1, Str2, En2> r) => r * l;
        public static ProductUnit<Str1, En1, Str2, En2> operator /(ProductUnit<Str1, En1, Str2, En2> l, double r)
            => new ProductUnit<Str1, En1, Str2, En2>(l.Original.Magnitude / r).SetScales(l.Original.Scale1, l.Original.Scale2);
        #endregion

        #region CROSS-UNIT ARITHMETIC OPERATORS
        public static ProductUnit<UnitSquared<Str1, En1>, En1, UnitSquared<Str2, En2>, En2> operator *(ProductUnit<Str1, En1, Str2, En2>l, ProductUnit<Str1, En1, Str2, En2> r)
        {
            if (!l.IsEqualScales(r))
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double res = l.Original.Magnitude * r.Original.Magnitude;
            return ProductUnit<UnitSquared<Str1, En1>, En1, UnitSquared<Str2, En2>, En2>.Create(res)
                .SetScales(l.Original.Scale1, l.Original.Scale2);
        }
        public static ProductUnit<Str1, En1, UnitSquared<Str2, En2>, En2> operator *(ProductUnit<Str1, En1, Str2, En2> l, Str2 r)
        {
            if (!l.Original.Scale2.Equals(r.Original.Scale))
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double res = l.Original.Magnitude * r.Original.Magnitude;
            return ProductUnit<Str1, En1, UnitSquared<Str2, En2>, En2>.Create(res)
                .SetScales(l.Original.Scale1, r.Original.Scale);
        }
        public static ProductUnit<Str1, En1, UnitSquared<Str2, En2>, En2> operator *(Str2 l, ProductUnit<Str1, En1, Str2, En2> r) => r * l;
        public static ProductUnit<UnitSquared<Str1, En1>, En1, Str2, En2> operator *(ProductUnit<Str1, En1, Str2, En2> l, Str1 r)
        {
            if (!l.Original.Scale1.Equals(r.Original.Scale))
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double res = l.Original.Magnitude * r.Original.Magnitude;
            return ProductUnit<UnitSquared<Str1, En1>, En1, Str2, En2>.Create(res)
                .SetScales(r.Original.Scale, l.Original.Scale2);
        }
        public static ProductUnit<UnitSquared<Str1, En1>, En1, Str2, En2> operator *(Str1 l, ProductUnit<Str1, En1, Str2, En2> r) => r * l;

        public static double operator /(ProductUnit<Str1, En1, Str2, En2> l, ProductUnit<Str1, En1, Str2, En2> r)
            => l.IsEqualScales(r) ? l.Original.Magnitude / r.Original.Magnitude :
            throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);
        public static Str1 operator /(ProductUnit<Str1, En1, Str2, En2> l, Str2 r)
        {
            if (!l.Original.Scale2.Equals(r.Original.Scale))
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double res = l.Original.Magnitude / r.Original.Magnitude;
            return Str1.Create(res, l.Original.Scale1);
        }
        public static Str2 operator /(ProductUnit<Str1, En1, Str2, En2> l, Str1 r)
        {
            if (!l.Original.Scale1.Equals(r.Original.Scale))
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double res = l.Original.Magnitude / r.Original.Magnitude;
            return Str2.Create(res, l.Original.Scale2);
        }
        #endregion

        #region A(B^2) & (A^2)B
        public static ProductUnit<Str1, En1, Str2, En2> Divide
            (ProductUnit<Str1, En1, UnitSquared<Str2, En2>, En2> l,
            Str2 r)
        {//    AB   =   A(B^2) / B

            if (l.Original.Scale2Ordinal != r.Original.ScaleOrdinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude / r.Original.Magnitude;
            var unit = ProductUnit<Str1, En1, Str2, En2>.Create(mag);
            return unit.SetScales(l.Original.Scale1, r.Original.Scale);
        }
        public static ProductUnit<Str1, En1, Str2, En2> Divide
            (ProductUnit<UnitSquared<Str1, En1>, En1, Str2, En2> l,
            Str1 r)
        {//     AB  =   (A^2)B / A

            if (l.Original.Scale1Ordinal != r.Original.ScaleOrdinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude / r.Original.Magnitude;
            var unit = ProductUnit<Str1, En1, Str2, En2>.Create(mag);
            return unit.SetScales(r.Original.Scale, l.Original.Scale2);
        }
        #endregion

        #region (A^2)(B^2)
        public static ProductUnit<Str1, En1, Str2, En2> operator /
            (ProductUnit<UnitSquared<Str1, En1>, En1, UnitSquared<Str2, En2>, En2> l,
            ProductUnit<Str1, En1, Str2, En2> r)
        {//     AB  =   (A^2)(B^2) / AB

            if (l.Original.Scale1Ordinal != r.Original.Scale1Ordinal ||
                l.Original.Scale2Ordinal != r.Original.Scale2Ordinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude / r.Original.Magnitude;
            var unit = ProductUnit<Str1, En1, Str2, En2>.Create(mag);
            return unit.SetScales(l.Original.Scale1, r.Original.Scale2);
        }
        #endregion

        #region AB * C
        public static ProductUnit<Str1, En1, Str2, En2> Divide<Str3, En3>
            (TernaryProductUnit<CompositeProduct<Str1, Str2>, Str1, En1, Str2, En2, Str3, En3> l,
            Str3 r)
            where En3: Enum
            where Str3: struct, ILinearUnit<Str3, En3>
        {//     AB  =   ABC / C

            if (l.Scales.Ordinal3 != r.Original.ScaleOrdinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Magnitude / r.Original.Magnitude;
            var unit = ProductUnit<Str1, En1, Str2, En2>.Create(mag);
            return unit.SetScales(l.Scales.Scale1, l.Scales.Scale2);
        }
        public static ProductUnit<Str1, En1, Str3, En3> Divide<Str3, En3>
            (TernaryProductUnit<CompositeProduct<Str1, Str2>, Str1, En1, Str2, En2, Str3, En3> l,
            Str2 r)
            where En3: Enum
            where Str3: struct, ILinearUnit<Str3, En3>
        {//     AC  =   ABC / B

            if (l.Scales.Ordinal2 != r.Original.ScaleOrdinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Magnitude / r.Original.Magnitude;
            var unit = ProductUnit<Str1, En1, Str3, En3>.Create(mag);
            return unit.SetScales(l.Scales.Scale1, l.Scales.Scale3);
        }
        public static ProductUnit<Str2, En2, Str3, En3> Divide<Str3, En3>
            (TernaryProductUnit<CompositeProduct<Str1, Str2>, Str1, En1, Str2, En2, Str3, En3> l,
            Str1 r)
            where En3: Enum
            where Str3: struct, ILinearUnit<Str3, En3>
        {//     BC  =   ABC / A

            if (l.Scales.Ordinal1 != r.Original.ScaleOrdinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Magnitude / r.Original.Magnitude;
            var unit = ProductUnit<Str2, En2, Str3, En3>.Create(mag);
            return unit.SetScales(l.Scales.Scale2, l.Scales.Scale3);
        }
        #endregion

        #region AB / C
        public static ProductUnit<Str1, En1, Str2, En2> Multiply<Str3, En3>
            (TernaryQuotientUnit<Numerator<CompositeProduct<Str1, Str2>>, Str1, En1, Str2, En2, Str3, En3> l,
            Str3 r)
            where En3: Enum
            where Str3: struct, ILinearUnit<Str3, En3>
        {//     AB  =   (AB/C) * C

            if (l.Scales.Ordinal3 != r.Original.ScaleOrdinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Magnitude * r.Original.Magnitude;
            var unit = ProductUnit<Str1, En1, Str2, En2>.Create(mag);
            return unit.SetScales(l.Scales.Scale1, l.Scales.Scale2);
        }
        public static ProductUnit<Str1, En1, Str2, En2> Multiply<Str3, En3>
            (Str3 l,
            TernaryQuotientUnit<Numerator<CompositeProduct<Str1, Str2>>, Str1, En1, Str2, En2, Str3, En3> r)
            where En3 : Enum
            where Str3 : struct, ILinearUnit<Str3, En3>
            => Multiply(r, l);
        #endregion

        #region A / BC
        public static ProductUnit<Str2, En2, Str3, En3> Divide<Str3, En3>
            (Str1 l,
            TernaryQuotientUnit<Denominator<CompositeProduct<Str2, Str3>>, Str1, En1, Str2, En2, Str3, En3> r)
            where En3: Enum
            where Str3: struct, ILinearUnit<Str3, En3>
        {//     BC  =   A / (A/BC)

            if (l.Original.ScaleOrdinal != r.Scales.Ordinal1)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude / r.Magnitude;
            var unit = ProductUnit<Str2, En2, Str3, En3>.Create(mag);
            return unit.SetScales(r.Scales.Scale2, r.Scales.Scale3);
        }
        #endregion

        #region TERNARY PRODUCT DIVISION
        public static QuotientUnit<Str1, En1, Str3, En3> Divide<Str3, En3>
            (ProductUnit<Str1, En1, Str2, En2> l,
            ProductUnit<Str2, En2, Str3, En3> r)
            where En3 : Enum
            where Str3 : struct, ILinearUnit<Str3, En3>
        {//     A/C =   AB / BC

            if (l.Original.Scale2Ordinal != r.Original.Scale1Ordinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude / r.Original.Magnitude;
            var unit = QuotientUnit<Str1, En1, Str3, En3>.Create(mag);
            return unit.SetScales(l.Original.Scale1, r.Original.Scale2);
        }
        public static QuotientUnit<Str2, En2, Str3, En3> Divide<Str3, En3>
            (ProductUnit<Str1, En1, Str2, En2> l,
            ProductUnit<Str1, En1, Str3, En3> r)
            where En3 : Enum
            where Str3 : struct, ILinearUnit<Str3, En3>
        {//     B/C =   AB / AC

            if (l.Original.Scale1Ordinal != r.Original.Scale1Ordinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude / r.Original.Magnitude;
            var unit = QuotientUnit<Str2, En2, Str3, En3>.Create(mag);
            return unit.SetScales(l.Original.Scale2, r.Original.Scale2);
        }
        public static QuotientUnit<Str1, En1, Str2, En2> Divide<Str3, En3>
            (ProductUnit<Str1, En1, Str3, En3> l,
            ProductUnit<Str2, En2, Str3, En3> r)
            where En3 : Enum
            where Str3 : struct, ILinearUnit<Str3, En3>
        {//     A/B =   AC / BC

            if (l.Original.Scale2Ordinal != r.Original.Scale2Ordinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude / r.Original.Magnitude;
            var unit = QuotientUnit<Str1, En1, Str2, En2>.Create(mag);
            return unit.SetScales(l.Original.Scale1, r.Original.Scale1);
        }
        #endregion

        #region AB * CD
        public QuaternaryProductUnit<CompositeProduct<Str1, Str2>, Str1, En1, Str2, En2, Str3, En3, Str4, En4> Multiply<Str3, En3, Str4, En4>
            (ProductUnit<Str1, En1, Str2, En2> l,
            ProductUnit<Str3, En3, Str4, En4> r)
            where En3: Enum
            where En4: Enum
            where Str3: struct, ILinearUnit<Str3, En3>
            where Str4: struct, ILinearUnit<Str4, En4>
        {//     ABCD    =   AB * CD

            double mag = l.Original.Magnitude * r.Original.Magnitude;
            var unit = QuaternaryProductUnit<CompositeProduct<Str1, Str2>, Str1, En1, Str2, En2, Str3, En3, Str4, En4>.Create(mag);
            return unit.SetScales(l.Original.Scale1, l.Original.Scale2, r.Original.Scale1, r.Original.Scale2);
        }
        #endregion
    }

    /// <summary>
    /// Represents a composite, binary quotient measurement consisting of a numerator unit component divided by a 
    /// denominator unit component (<typeparamref name="Str1"/> / <typeparamref name="Str2"/>), providing strongly typed 
    /// cross-unit arithmetic, scale management, and dimensional reduction.
    /// </summary>
    /// <typeparam name="Str1">
    /// The underlying structure type representing the numerator unit factor in the quotient measurement.
    /// Must be a value type implementing dimension access, scale conversion, normalization, and initialization contracts.
    /// </typeparam>
    /// <typeparam name="En1">
    /// The unit scale enumeration type representing valid scales or prefixes for <typeparamref name="Str1"/>.
    /// </typeparam>
    /// <typeparam name="Str2">
    /// The underlying structure type representing the denominator unit factor in the quotient measurement.
    /// Must be a value type implementing dimension access, scale conversion, normalization, and initialization contracts.
    /// </typeparam>
    /// <typeparam name="En2">
    /// The unit scale enumeration type representing valid scales or prefixes for <typeparamref name="Str2"/>.
    /// </typeparam>
    /// <remarks>
    /// <para>
    /// <see cref="QuotientUnit{Str1, En1, Str2, En2}"/> models derived physical quantities formed by dividing two distinct 
    /// unit factors (e.g., Distance / Time for Velocity, or Force / Area for Pressure). It maintains separate scale configurations 
    /// and ordinal indices for both numerator and denominator factors while exposing algebraic operations.
    /// </para>
    /// <para>
    /// The struct supports algebraic dimensional reductions, such as multiplying by a denominator unit (<typeparamref name="Str2"/>) 
    /// to yield the numerator unit (<typeparamref name="Str1"/>), or dividing a numerator unit (<typeparamref name="Str1"/>) 
    /// by the quotient to isolate the denominator unit (<typeparamref name="Str2"/>). Self-arithmetic, cross-multiplication, and 
    /// relational comparisons validate scale matching across component factors using cached ordinal comparisons, throwing an 
    /// <see cref="InvalidOperationException"/> if scale mismatches occur.
    /// </para>
    /// </remarks>
    public struct QuotientUnit<Str1, En1, Str2, En2> :
        IDimensionAccessible,
        IInitializable<QuotientUnit<Str1, En1, Str2, En2>>,
        IInitializable<QuotientUnit<Str1, En1, Str2, En2>, double>,
        IInitializable<QuotientUnit<Str1, En1, Str2, En2>, double, En1>,
        IDuplicatable<QuotientUnit<Str1, En1, Str2, En2>>,
        IEquatable<QuotientUnit<Str1, En1, Str2, En2>>,
        IScaleValueAccessible<En1, En2>,
        ICompositeUnit

        where Str1 : struct, IDimensionAccessible,
            IInitializable<Str1>, IInitializable<Str1, double>, IInitializable<Str1, double, En1>,
            INormalized<En1>, INormalizable<Str1>,
            IScaleConvertible<Str1, En1>, IValueAccessible<En1>
        where Str2 : struct, IDimensionAccessible,
            IInitializable<Str2>, IInitializable<Str2, double>, IInitializable<Str2, double, En2>,
            INormalized<En2>, INormalizable<Str2>,
            IScaleConvertible<Str2, En2>, IValueAccessible<En2>
        where En1 : Enum
        where En2 : Enum
    {
        #region PROPERTIES
        private static int BaseScale1Ordinal
        {
            get
            {
                En1 bs1 = BaseScale1;
                return Unsafe.As<En1, int>(ref bs1);
            }
        }
        private static int BaseScale2Ordinal
        {
            get
            {
                En2 bs2 = BaseScale2;
                return Unsafe.As<En2, int>(ref bs2);
            }
        }

        public int Dimension => 1;

        public static En1 BaseScale1 => Str1.BaseScale;
        public static En2 BaseScale2 => Str2.BaseScale;

        public (double Magnitude, En1 Scale1, En2 Scale2, int Scale1Ordinal, int Scale2Ordinal) Original { get; private set; }
        public (En1 Scale1, En2 Scale2) ScaleValues => (Original.Scale1, Original.Scale2);

        public Type Struct1 = typeof(Str1);
        public Type Struct2 = typeof(Str2);
        public Type Enum1 = typeof(En1);
        public Type Enum2 = typeof(En2);
        #endregion

        #region CONSTRUCTORS
        public QuotientUnit()
        {
            Original = (0, BaseScale1, BaseScale2, BaseScale1Ordinal, BaseScale2Ordinal);
        }
        public QuotientUnit(QuotientUnit<Str1, En1, Str2, En2> instance)
        {
            this = instance;
        }
        public QuotientUnit(double magnitude)
        {
            Original = (magnitude, BaseScale1, BaseScale2, BaseScale1Ordinal, BaseScale2Ordinal);
        }
        public QuotientUnit(double magnitude, En1 scale1)
        {
            Original = (magnitude, scale1, BaseScale2, Unsafe.As<En1, int>(ref scale1), BaseScale2Ordinal);
        }
        #endregion

        #region METHODS
        public static QuotientUnit<Str1, En1, Str2, En2> Initialize() => new();
        public static QuotientUnit<Str1, En1, Str2, En2> Create(QuotientUnit<Str1, En1, Str2, En2> instance)
            => new(instance);
        public static QuotientUnit<Str1, En1, Str2, En2> Create(double magnitude) => new(magnitude);
        public static QuotientUnit<Str1, En1, Str2, En2> Create(double magnitude, En1 scale1) => new(magnitude, scale1);


        public QuotientUnit<Str1, En1, Str2, En2> SetScale1(En1 scale1)
        {
            (double mag, En2 s2, int s2Ord) orig = (Original.Magnitude, Original.Scale2, Original.Scale2Ordinal);

            Original = (orig.mag, scale1, orig.s2, Unsafe.As<En1, int>(ref scale1), orig.s2Ord);
            return this;
        }
        public QuotientUnit<Str1, En1, Str2, En2> SetScale2(En2 scale2)
        {
            (double mag, En1 s1, int s1Ord) orig = (Original.Magnitude, Original.Scale1, Original.Scale1Ordinal);

            Original = (orig.mag, orig.s1, scale2, orig.s1Ord, Unsafe.As<En2, int>(ref scale2));
            return this;
        }
        public QuotientUnit<Str1, En1, Str2, En2> SetScales(En1 scale1, En2 scale2)
        {
            double mag = Original.Magnitude;

            Original = (mag, scale1, scale2, Unsafe.As<En1, int>(ref scale1), Unsafe.As<En2, int>(ref scale2));
            return this;
        }

        public QuotientUnit<Str1, En1, Str2, En2> Duplicate() => new(this);
        public bool Equals(QuotientUnit<Str1, En1, Str2, En2> pu)
        {
            if (Original.Scale1Ordinal != pu.Original.Scale1Ordinal || Original.Scale2Ordinal != pu.Original.Scale2Ordinal)
                return false;

            return Original.Magnitude == pu.Original.Magnitude;
        }

        public QuotientUnit<Str2, En2, Str1, En1> Reciprocate()
            => QuotientUnit<Str2, En2, Str1, En1>.Create(Original.Magnitude).SetScales(Original.Scale2, Original.Scale1);

        public bool IsEqualTypeParams<Str3, En3, Str4, En4>(QuotientUnit<Str3, En3, Str4, En4> other)
            where Str3 : struct, IDimensionAccessible,
                IInitializable<Str3>, IInitializable<Str3, double>, IInitializable<Str3, double, En3>,
                INormalized<En3>, INormalizable<Str3>,
                IScaleConvertible<Str3, En3>, IValueAccessible<En3>
            where Str4 : struct, IDimensionAccessible,
                IInitializable<Str4>, IInitializable<Str4, double>, IInitializable<Str4, double, En4>,
                INormalized<En4>, INormalizable<Str4>,
                IScaleConvertible<Str4, En4>, IValueAccessible<En4>
            where En3 : Enum
            where En4 : Enum
        {
            return
                (Struct1 == other.Struct1 && Struct2 == other.Struct2) &&
                (Enum1 == other.Enum1 && Enum2 == other.Enum2);
        }
        public bool IsEqualScales(QuotientUnit<Str1, En1, Str2, En2> other)
            => Original.Scale1Ordinal == other.Original.Scale1Ordinal && Original.Scale2Ordinal == other.Original.Scale2Ordinal;
        public bool IsEqualTo<Str3, En3, Str4, En4>(QuotientUnit<Str3, En3, Str4, En4> other)
            where Str3 : struct, IDimensionAccessible,
                IInitializable<Str3>, IInitializable<Str3, double>, IInitializable<Str3, double, En3>,
                INormalized<En3>, INormalizable<Str3>,
                IScaleConvertible<Str3, En3>, IValueAccessible<En3>
            where Str4 : struct, IDimensionAccessible,
                IInitializable<Str4>, IInitializable<Str4, double>, IInitializable<Str4, double, En4>,
                INormalized<En4>, INormalizable<Str4>,
                IScaleConvertible<Str4, En4>, IValueAccessible<En4>
            where En3 : Enum
            where En4 : Enum
        {
            if (!IsEqualTypeParams(other))
                return false;

            return Original.Magnitude == other.Original.Magnitude;
        }
        #endregion

        #region OVERRIDES
        [Obsolete]
        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region CONDITIONAL OPERATORS
        public static bool operator ==(QuotientUnit<Str1, En1, Str2, En2> l, QuotientUnit<Str1, En1, Str2, En2> r)
            => l.Equals(r);
        public static bool operator !=(QuotientUnit<Str1, En1, Str2, En2> l, QuotientUnit<Str1, En1, Str2, En2> r)
            => !l.Equals(r);
        public static bool operator <(QuotientUnit<Str1, En1, Str2, En2> l, QuotientUnit<Str1, En1, Str2, En2> r)
            => l.IsEqualScales(r) ?
            l.Original.Magnitude < r.Original.Magnitude :
            throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);
        public static bool operator >(QuotientUnit<Str1, En1, Str2, En2> l, QuotientUnit<Str1, En1, Str2, En2> r)
            => l.IsEqualScales(r) ?
            l.Original.Magnitude > r.Original.Magnitude :
            throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);
        public static bool operator <=(QuotientUnit<Str1, En1, Str2, En2> l, QuotientUnit<Str1, En1, Str2, En2> r)
            => l < r || l == r;
        public static bool operator >=(QuotientUnit<Str1, En1, Str2, En2> l, QuotientUnit<Str1, En1, Str2, En2> r)
            => l > r || l == r;
        #endregion

        #region SELF ARITHMETIC OPERATORS
        public static QuotientUnit<Str1, En1, Str2, En2> operator +(QuotientUnit<Str1, En1, Str2, En2> l, QuotientUnit<Str1, En1, Str2, En2> r)
            => l.IsEqualScales(r) ?
            QuotientUnit<Str1, En1, Str2, En2>.Create(l.Original.Magnitude + r.Original.Magnitude).SetScales(l.Original.Scale1, l.Original.Scale2) :
            throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);
        public static QuotientUnit<Str1, En1, Str2, En2> operator -(QuotientUnit<Str1, En1, Str2, En2> l, QuotientUnit<Str1, En1, Str2, En2> r)
            => l.IsEqualScales(r) ?
            QuotientUnit<Str1, En1, Str2, En2>.Create(l.Original.Magnitude - r.Original.Magnitude).SetScales(l.Original.Scale1, l.Original.Scale2) :
            throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);
        public static QuotientUnit<Str1, En1, Str2, En2> operator *(QuotientUnit<Str1, En1, Str2, En2> l, double r)
            => QuotientUnit<Str1, En1, Str2, En2>.Create(l.Original.Magnitude * r).SetScales(l.Original.Scale1, l.Original.Scale2);
        public static QuotientUnit<Str1, En1, Str2, En2> operator *(double l, QuotientUnit<Str1, En1, Str2, En2> r)
            => r * l;
        public static QuotientUnit<Str1, En1, Str2, En2> operator /(QuotientUnit<Str1, En1, Str2, En2> l, double r)
            => QuotientUnit<Str1, En1, Str2, En2>.Create(l.Original.Magnitude / r).SetScales(l.Original.Scale1, l.Original.Scale2);
        #endregion

        #region CROSS-UNIT ARITHMETIC OPERATORS
        public static QuotientUnit<UnitSquared<Str1, En1>, En1, UnitSquared<Str2, En2>, En2> operator *(QuotientUnit<Str1, En1, Str2, En2> l, QuotientUnit<Str1, En1, Str2, En2> r)
            => l.IsEqualScales(r) ?
            QuotientUnit<UnitSquared<Str1, En1>, En1, UnitSquared<Str2, En2>, En2>.Create(l.Original.Magnitude * r.Original.Magnitude).SetScales(l.Original.Scale1, r.Original.Scale2) :
            throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);
        public static QuotientUnit<UnitSquared<Str1, En1>, En1, Str2, En2> operator *(QuotientUnit<Str1, En1, Str2, En2> l, Str1 r)
            => l.Original.Scale1Ordinal == r.Original.ScaleOrdinal ?
            QuotientUnit<UnitSquared<Str1, En1>, En1, Str2, En2>.Create(l.Original.Magnitude * r.Original.Magnitude).SetScales(r.Original.Scale, l.Original.Scale2) :
            throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);
        public static Str1 operator *(QuotientUnit<Str1, En1, Str2, En2> l, Str2 r)
            => l.Original.Scale2Ordinal == r.Original.ScaleOrdinal ?
            Str1.Create(l.Original.Magnitude * r.Original.Magnitude, l.Original.Scale1) :
            throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

        public static double operator /(QuotientUnit<Str1, En1, Str2, En2> l, QuotientUnit<Str1, En1, Str2, En2> r)
            => l.IsEqualScales(r) ?
            l.Original.Magnitude / r.Original.Magnitude :
            throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);
        public static Str2 operator /(Str1 l, QuotientUnit<Str1, En1, Str2, En2> r)
            => l.Original.ScaleOrdinal == r.Original.Scale1Ordinal ?
            Str2.Create(l.Original.Magnitude / r.Original.Magnitude, r.Original.Scale2) :
            throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);
        #endregion

        #region A/(B^2)
        public static QuotientUnit<Str1, En1, Str2, En2> Multiply
            (QuotientUnit<Str1, En1, UnitSquared<Str2, En2>, En2> l,
            Str2 r)
        {//     A/B =   A/(B*2) * B

            if (l.Original.Scale2Ordinal != r.Original.ScaleOrdinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude * r.Original.Magnitude;
            var unit = QuotientUnit<Str1, En1, Str2, En2>.Create(mag);
            return unit.SetScales(l.Original.Scale1, r.Original.Scale);
        }
        public static QuotientUnit<Str1, En1, Str2, En2> Multiply
            (Str2 l,
            QuotientUnit<Str1, En1, UnitSquared<Str2, En2>, En2> r)
            => Multiply(r, l);
        #endregion

        #region (A^2)/B
        public static QuotientUnit<Str1, En1, Str2, En2> Divide
            (Str1 l,
            QuotientUnit<UnitSquared<Str1, En1>, En1, Str2, En2> r)
        {//     A/B =   A / (A^2)/B

            if (l.Original.ScaleOrdinal != r.Original.Scale1Ordinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude / r.Original.Magnitude;
            var unit = QuotientUnit<Str1, En1, Str2, En2>.Create(mag);
            return unit.SetScales(l.Original.Scale, r.Original.Scale2);
        }
        #endregion

        #region (A^2)/(B^2)
        public static QuotientUnit<Str1, En1, Str2, En2> Multiply
            (QuotientUnit<UnitSquared<Str1, En1>, En1, UnitSquared<Str2, En2>, En2> l,
            QuotientUnit<Str2, En2, Str1, En1> r)
        {//     A/B =   (A^2)/(B^2) * B/A

            if (l.Original.Scale1Ordinal != r.Original.Scale1Ordinal ||
                l.Original.Scale2Ordinal != r.Original.Scale2Ordinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude * r.Original.Magnitude;
            var unit = QuotientUnit<Str1, En1, Str2, En2>.Create(mag);
            return unit.SetScales(l.Original.Scale1, l.Original.Scale2);
        }
        public static QuotientUnit<Str1, En1, Str2, En2> Multiply
            (QuotientUnit<Str2, En2, Str1, En1> l,
            QuotientUnit<UnitSquared<Str1, En1>, En1, UnitSquared<Str2, En2>, En2> r)
            => Multiply(r, l);
        #endregion

        #region AB / C
        public static QuotientUnit<Str1, En1, Str3, En3> Divide<Str3, En3>
            (Str2 l,
            TernaryQuotientUnit<Numerator<CompositeProduct<Str1, Str2>>, Str1, En1, Str2, En2, Str3, En3> r)
            where En3: Enum
            where Str3: struct, ILinearUnit<Str3, En3>
        {//     A/C =   B / (AB/C)

            if (l.Original.ScaleOrdinal != r.Scales.Ordinal2)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude / r.Magnitude;
            var unit = QuotientUnit<Str1, En1, Str3, En3>.Create(mag);
            return unit.SetScales(r.Scales.Scale1, r.Scales.Scale3);
        }
        public static QuotientUnit<Str2, En2, Str3, En3> Divide<Str3, En3>
            (Str1 l,
            TernaryQuotientUnit<Numerator<CompositeProduct<Str1, Str2>>, Str1, En1, Str2, En2, Str3, En3> r)
            where En3: Enum
            where Str3: struct, ILinearUnit<Str3, En3>
        {//     B/C =   A / (AB/C)

            if (l.Original.ScaleOrdinal != r.Scales.Ordinal1)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude / r.Magnitude;
            var unit = QuotientUnit<Str2, En2, Str3, En3>.Create(mag);
            return unit.SetScales(r.Scales.Scale2, r.Scales.Scale3);
        }
        #endregion

        #region A / BC
        public static QuotientUnit<Str1, En1, Str2, En2> Multiply<Str3, En3>
            (TernaryQuotientUnit<Denominator<CompositeProduct<Str2, Str3>>, Str1, En1, Str2, En2, Str3, En3> l,
            Str3 r)
            where En3: Enum
            where Str3: struct, ILinearUnit<Str3, En3>
        {//     A/B =   (A/BC) * C

            if (l.Scales.Ordinal3 != r.Original.ScaleOrdinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Magnitude * r.Original.Magnitude;
            var unit = QuotientUnit<Str1, En1, Str2, En2>.Create(mag);
            return unit.SetScales(l.Scales.Scale1, l.Scales.Scale2);
        }
        public static QuotientUnit<Str1, En1, Str2, En2> Multiply<Str3, En3>
            (Str3 l,
            TernaryQuotientUnit<Denominator<CompositeProduct<Str2, Str3>>, Str1, En1, Str2, En2, Str3, En3> r)
            where En3 : Enum
            where Str3 : struct, ILinearUnit<Str3, En3>
            => Multiply(r, l);
        public static QuotientUnit<Str1, En1, Str3, En3> Multiply<Str3, En3>
            (TernaryQuotientUnit<Denominator<CompositeProduct<Str2, Str3>>, Str1, En1, Str2, En2, Str3, En3> l,
            Str2 r)
            where En3: Enum
            where Str3: struct, ILinearUnit<Str3, En3>
        {//     A/C =   (A/BC) * B

            if (l.Scales.Ordinal2 != r.Original.ScaleOrdinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Magnitude * r.Original.Magnitude;
            var unit = QuotientUnit<Str1, En1, Str3, En3>.Create(mag);
            return unit.SetScales(l.Scales.Scale1, l.Scales.Scale3);
        }
        public static QuotientUnit<Str1, En1, Str3, En3> Multiply<Str3, En3>
            (Str2 l,
            TernaryQuotientUnit<Denominator<CompositeProduct<Str2, Str3>>, Str1, En1, Str2, En2, Str3, En3> r)
            where En3 : Enum
            where Str3 : struct, ILinearUnit<Str3, En3>
            => Multiply(r, l);
        #endregion
    }

    /// <summary>
    /// Represents a composite, ternary product measurement consisting of three multiplied unit factors 
    /// (<typeparamref name="Str1"/> × <typeparamref name="Str2"/> × <typeparamref name="Str3"/>), providing scale management, 
    /// component ordinal tracking, and strongly typed structural encapsulation for 3-factor physical quantities.
    /// </summary>
    /// <typeparam name="TComp">
    /// A marker or backing composite unit interface/class type constraint used to bind or classify the aggregate ternary composite type within the framework architecture.
    /// </typeparam>
    /// <typeparam name="Str1">
    /// The underlying structure type representing the primary unit factor in the ternary product.
    /// Must be a value type implementing dimension access, scale conversion, normalization, and initialization contracts.
    /// </typeparam>
    /// <typeparam name="En1">
    /// The unit scale enumeration type representing valid scales or prefixes for <typeparamref name="Str1"/>.
    /// </typeparam>
    /// <typeparam name="Str2">
    /// The underlying structure type representing the secondary unit factor in the ternary product.
    /// Must be a value type implementing dimension access, scale conversion, normalization, and initialization contracts.
    /// </typeparam>
    /// <typeparam name="En2">
    /// The unit scale enumeration type representing valid scales or prefixes for <typeparamref name="Str2"/>.
    /// </typeparam>
    /// <typeparam name="Str3">
    /// The underlying structure type representing the tertiary unit factor in the ternary product.
    /// Must be a value type implementing dimension access, scale conversion, normalization, and initialization contracts.
    /// </typeparam>
    /// <typeparam name="En3">
    /// The unit scale enumeration type representing valid scales or prefixes for <typeparamref name="Str3"/>.
    /// </typeparam>
    /// <remarks>
    /// <para>
    /// <see cref="TernaryProductUnit{TComp, Str1, En1, Str2, En2, Str3, En3}"/> models compound physical quantities formed 
    /// by multiplying three distinct unit factors (e.g., Mass × Length × Time⁻² for Force when expanded, or Power × Time × Length). 
    /// It maintains independent scale configurations (<typeparamref name="En1"/>, <typeparamref name="En2"/>, <typeparamref name="En3"/>) 
    /// and cached ordinal representations for all three factors simultaneously.
    /// </para>
    /// <para>
    /// By implementing <see cref="ICompositeUnit"/>, it integrates into the framework's higher-order dimensional reduction 
    /// pipeline, allowing multi-factor algebraic operations to maintain strict type safety and zero-allocation performance.
    /// </para>
    /// </remarks>
    public struct TernaryProductUnit<TComp, Str1, En1, Str2, En2, Str3, En3> :
        IDimensionAccessible,
        IInitializable<TernaryProductUnit<TComp, Str1, En1, Str2, En2, Str3, En3>, double>,
        ICompositeUnit

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
        where TComp : class, ICompositeUnit
    {
        #region PROPERTIES
        private static int BaseScale1Ordinal
        {
            get
            {
                En1 bs1 = BaseScale1;
                return Unsafe.As<En1, int>(ref bs1);
            }
        }
        private static int BaseScale2Ordinal
        {
            get
            {
                En2 bs2 = BaseScale2;
                return Unsafe.As<En2, int>(ref bs2);
            }
        }
        private static int BaseScale3Ordinal
        {
            get
            {
                En3 bs3 = BaseScale3;
                return Unsafe.As<En3, int>(ref bs3);
            }
        }

        public int Dimension => 1;

        public double Magnitude { get; private set; }
        public (En1 Scale1, En2 Scale2, En3 Scale3, int Ordinal1, int Ordinal2, int Ordinal3) Scales { get; private set; }

        public static En1 BaseScale1 => Str1.BaseScale;
        public static En2 BaseScale2 => Str2.BaseScale;
        public static En3 BaseScale3 => Str3.BaseScale;
        #endregion

        #region CONSTRUCTORS
        public TernaryProductUnit(double magnitude)
        {
            Magnitude = magnitude;
            Scales = (BaseScale1, BaseScale2, BaseScale3,
                BaseScale1Ordinal, BaseScale2Ordinal, BaseScale3Ordinal);
        }
        #endregion

        #region METHODS
        public static TernaryProductUnit<TComp, Str1, En1, Str2, En2, Str3, En3> Create(double magnitude) => new(magnitude);
        
        public TernaryProductUnit<TComp, Str1, En1, Str2, En2, Str3, En3> SetScales(En1 scale1, En2 scale2, En3 scale3)
        {
            Scales = (scale1, scale2, scale3,
                Unsafe.As<En1, int>(ref scale1),
                Unsafe.As<En2, int>(ref scale2),
                Unsafe.As<En3, int>(ref scale3));
            return this;
        }
        #endregion

        #region AB * C
        public static TernaryProductUnit<CompositeProduct<Str1, Str2>, Str1, En1, Str2, En2, Str3, En3> Multiply
            (ProductUnit<Str1, En1, Str2, En2> l, Str3 r)
        {//     ABC =   AB * C

            double mag = l.Original.Magnitude * r.Original.Magnitude;
            var unit = TernaryProductUnit<CompositeProduct<Str1, Str2>, Str1, En1, Str2, En2, Str3, En3>.Create(mag);
            return unit.SetScales(l.Original.Scale1, l.Original.Scale2, r.Original.Scale);
        }
        public static TernaryProductUnit<CompositeProduct<Str1, Str2>, Str1, En1, Str2, En2, Str3, En3> Multiply
            (Str3 l, ProductUnit<Str1, En1, Str2, En2> r)
            => Multiply(r, l);
        #endregion
    }

    /// <summary>
    /// Represents a composite unit structure expressing a ternary quotient relationship across three distinct dimensional unit types.
    /// </summary>
    /// <typeparam name="TComp">The division operand type specifying the structural layout of the quotient terms.</typeparam>
    /// <typeparam name="Str1">The first constituent struct unit type.</typeparam>
    /// <typeparam name="En1">The unit scale enumeration type for the first constituent unit.</typeparam>
    /// <typeparam name="Str2">The second constituent struct unit type.</typeparam>
    /// <typeparam name="En2">The unit scale enumeration type for the second constituent unit.</typeparam>
    /// <typeparam name="Str3">The third constituent struct unit type.</typeparam>
    /// <typeparam name="En3">The unit scale enumeration type for the third constituent unit.</typeparam>
    /// <remarks>
    /// <para>
    /// <see cref="TernaryQuotientUnit{TComp, Str1, En1, Str2, En2, Str3, En3}"/> implements <see cref="IDimensionAccessible"/>, 
    /// <see cref="IInitializable{TSelf, TValue}"/>, and <see cref="ICompositeUnit"/> to represent multi-factor ratio expressions 
    /// within the composite calculation framework.
    /// </para>
    /// <para>
    /// It enforces rigorous constraint contracts on its constituent unit types—requiring them to support dimensional accessibility, 
    /// scale conversion, normalization, and value extraction—allowing runtime dispatchers and reduction routines to evaluate and transmute 
    /// complex three-factor quotient magnitudes safely.
    /// </para>
    /// </remarks>
    public struct TernaryQuotientUnit<TComp, Str1, En1, Str2, En2, Str3, En3> :
        IDimensionAccessible,
        IInitializable<TernaryQuotientUnit<TComp, Str1, En1, Str2, En2, Str3, En3>, double>,
        ICompositeUnit

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
        where TComp : DivisionOperands
    {
        #region PROPERTIES
        private static int BaseScale1Ordinal
        {
            get
            {
                En1 bs1 = BaseScale1;
                return Unsafe.As<En1, int>(ref bs1);
            }
        }
        private static int BaseScale2Ordinal
        {
            get
            {
                En2 bs2 = BaseScale2;
                return Unsafe.As<En2, int>(ref bs2);
            }
        }
        private static int BaseScale3Ordinal
        {
            get
            {
                En3 bs3 = BaseScale3;
                return Unsafe.As<En3, int>(ref bs3);
            }
        }

        public int Dimension => 1;

        public double Magnitude { get; private set; }
        public (En1 Scale1, En2 Scale2, En3 Scale3, int Ordinal1, int Ordinal2, int Ordinal3) Scales { get; private set; }

        public static En1 BaseScale1 => Str1.BaseScale;
        public static En2 BaseScale2 => Str2.BaseScale;
        public static En3 BaseScale3 => Str3.BaseScale;
        #endregion

        #region CONSTRUCTORS
        public TernaryQuotientUnit(double magnitude)
        {
            Magnitude = magnitude;
            Scales = (BaseScale1, BaseScale2, BaseScale3,
                BaseScale1Ordinal, BaseScale2Ordinal, BaseScale3Ordinal);
        }
        #endregion

        #region METHODS
        public static TernaryQuotientUnit<TComp, Str1, En1, Str2, En2, Str3, En3> Create(double magnitude) => new(magnitude);

        public TernaryQuotientUnit<TComp, Str1, En1, Str2, En2, Str3, En3> SetScales(En1 scale1, En2 scale2, En3 scale3)
        {
            Scales = (scale1, scale2, scale3,
                Unsafe.As<En1, int>(ref scale1),
                Unsafe.As<En2, int>(ref scale2),
                Unsafe.As<En3, int>(ref scale3));
            return this;
        }
        #endregion

        #region AB / C
        public static TernaryQuotientUnit<Numerator<CompositeProduct<Str1, Str2>>, Str1, En1, Str2, En2, Str3, En3> Divide
            (ProductUnit<Str1, En1, Str2, En2> l, Str3 r)
        {//     AB/C    =   AB / C

            double mag = l.Original.Magnitude / r.Original.Magnitude;
            var unit = TernaryQuotientUnit<Numerator<CompositeProduct<Str1, Str2>>, Str1, En1, Str2, En2, Str3, En3>.Create(mag);
            return unit.SetScales(l.Original.Scale1, l.Original.Scale2, r.Original.Scale);
        }
        #endregion

        #region A / BC
        public static TernaryQuotientUnit<Denominator<CompositeProduct<Str2, Str3>>, Str1, En1, Str2, En2, Str3, En3> Divide
            (Str1 l, ProductUnit<Str2, En2, Str3, En3> r)
        {//     A/BC    =   A / BC

            double mag = l.Original.Magnitude / r.Original.Magnitude;
            var unit = TernaryQuotientUnit<Denominator<CompositeProduct<Str2, Str3>>, Str1, En1, Str2, En2, Str3, En3>.Create(mag);
            return unit.SetScales(l.Original.Scale, r.Original.Scale1, r.Original.Scale2);
        }
        #endregion
    }

    /// <summary>
    /// Represents a composite unit structure expressing a quaternary product relationship across four distinct dimensional unit types.
    /// </summary>
    /// <typeparam name="TComp">The composite unit type specifying the underlying structural composition of the product expression.</typeparam>
    /// <typeparam name="Str1">The first constituent struct unit type.</typeparam>
    /// <typeparam name="En1">The unit scale enumeration type for the first constituent unit.</typeparam>
    /// <typeparam name="Str2">The second constituent struct unit type.</typeparam>
    /// <typeparam name="En2">The unit scale enumeration type for the second constituent unit.</typeparam>
    /// <typeparam name="Str3">The third constituent struct unit type.</typeparam>
    /// <typeparam name="En3">The unit scale enumeration type for the third constituent unit.</typeparam>
    /// <typeparam name="Str4">The fourth constituent struct unit type.</typeparam>
    /// <typeparam name="En4">The unit scale enumeration type for the fourth constituent unit.</typeparam>
    /// <remarks>
    /// <para>
    /// <see cref="QuaternaryProductUnit{TComp, Str1, En1, Str2, En2, Str3, En3, Str4, En4}"/> implements <see cref="IDimensionAccessible"/>, 
    /// <see cref="IInitializable{TSelf, TValue}"/>, and <see cref="ICompositeUnit"/> to represent four-factor multiplicative expressions 
    /// within the composite calculation framework.
    /// </para>
    /// <para>
    /// It enforces rigorous constraint contracts on its constituent unit types—requiring them to support dimensional accessibility, 
    /// scale conversion, normalization, and value extraction—allowing runtime dispatchers and reduction routines to evaluate and transmute 
    /// complex four-factor product magnitudes safely.
    /// </para>
    /// </remarks>
    public struct QuaternaryProductUnit<TComp, Str1, En1, Str2, En2, Str3, En3, Str4, En4> :
        IDimensionAccessible,
        IInitializable<QuaternaryProductUnit<TComp, Str1, En1, Str2, En2, Str3, En3, Str4, En4>, double>,
        ICompositeUnit

        where En1 : Enum
        where En2 : Enum
        where En3 : Enum
        where En4 : Enum
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
        where Str4 : struct, IDimensionAccessible,
            IInitializable<Str4>, IInitializable<Str4, double>, IInitializable<Str4, double, En4>,
            INormalized<En4>, INormalizable<Str4>,
            IScaleConvertible<Str4, En4>, IValueAccessible<En4>
        where TComp : class, ICompositeUnit
    {
        #region PROPERTIES
        private static int BaseScale1Ordinal
        {
            get
            {
                En1 bs1 = BaseScale1;
                return Unsafe.As<En1, int>(ref bs1);
            }
        }
        private static int BaseScale2Ordinal
        {
            get
            {
                En2 bs2 = BaseScale2;
                return Unsafe.As<En2, int>(ref bs2);
            }
        }
        private static int BaseScale3Ordinal
        {
            get
            {
                En3 bs3 = BaseScale3;
                return Unsafe.As<En3, int>(ref bs3);
            }
        }
        private static int BaseScale4Ordinal
        {
            get
            {
                En4 bs4 = BaseScale4;
                return Unsafe.As<En4, int>(ref bs4);
            }
        }


        public static En1 BaseScale1 => Str1.BaseScale;
        public static En2 BaseScale2 => Str2.BaseScale;
        public static En3 BaseScale3 => Str3.BaseScale;
        public static En4 BaseScale4 => Str4.BaseScale;

        public int Dimension => 1;

        public double Magnitude { get; private set; }
        public (En1 Scale1, En2 Scale2, En3 Scale3, En4 Scale4,
            int Oridnal1, int Ordinal2, int Ordinal3, int Ordinal4) Scales
        { get; private set; }
        #endregion

        #region CONSTRUCTORS
        public QuaternaryProductUnit(double magnitude)
        {
            Magnitude = magnitude;
            Scales = (BaseScale1, BaseScale2, BaseScale3, BaseScale4,
                BaseScale1Ordinal, BaseScale2Ordinal, BaseScale3Ordinal, BaseScale4Ordinal);
        }
        #endregion

        #region METHODS
        public static QuaternaryProductUnit<TComp, Str1, En1, Str2, En2, Str3, En3, Str4, En4> Create(double mag) => new(mag);
        public QuaternaryProductUnit<TComp, Str1, En1, Str2, En2, Str3, En3, Str4, En4> SetScales
            (En1 scale1, En2 scale2, En3 scale3, En4 scale4)
        {
            Scales = (scale1, scale2, scale3, scale4,
                Unsafe.As<En1, int>(ref scale1),
                Unsafe.As<En2, int>(ref scale2),
                Unsafe.As<En3, int>(ref scale3),
                Unsafe.As<En4, int>(ref scale4));
            return this;
        }
        #endregion
    }

    /// <summary>
    /// Represents a composite unit structure expressing a quaternary quotient relationship across four distinct dimensional unit types.
    /// </summary>
    /// <typeparam name="TComp">The division operand type specifying the structural layout of the quotient terms.</typeparam>
    /// <typeparam name="Str1">The first constituent struct unit type.</typeparam>
    /// <typeparam name="En1">The unit scale enumeration type for the first constituent unit.</typeparam>
    /// <typeparam name="Str2">The second constituent struct unit type.</typeparam>
    /// <typeparam name="En2">The unit scale enumeration type for the second constituent unit.</typeparam>
    /// <typeparam name="Str3">The third constituent struct unit type.</typeparam>
    /// <typeparam name="En3">The unit scale enumeration type for the third constituent unit.</typeparam>
    /// <typeparam name="Str4">The fourth constituent struct unit type.</typeparam>
    /// <typeparam name="En4">The unit scale enumeration type for the fourth constituent unit.</typeparam>
    /// <remarks>
    /// <para>
    /// <see cref="QuaternaryQuotientUnit{TComp, Str1, En1, Str2, En2, Str3, En3, Str4, En4}"/> implements <see cref="IDimensionAccessible"/>, 
    /// <see cref="IInitializable{TSelf, TValue}"/>, and <see cref="ICompositeUnit"/> to represent multi-factor ratio expressions 
    /// within the composite calculation framework.
    /// </para>
    /// <para>
    /// It enforces rigorous constraint contracts on its constituent unit types—requiring them to support dimensional accessibility, 
    /// scale conversion, normalization, and value extraction—allowing runtime dispatchers and reduction routines to evaluate and transmute 
    /// complex four-factor quotient magnitudes safely.
    /// </para>
    /// </remarks>
    public struct QuaternaryQuotientUnit<TComp, Str1, En1, Str2, En2, Str3, En3, Str4, En4> :
        IDimensionAccessible,
        IInitializable<QuaternaryQuotientUnit<TComp, Str1, En1, Str2, En2, Str3, En3, Str4, En4>, double>,
        ICompositeUnit

        where En1 : Enum
        where En2 : Enum
        where En3 : Enum
        where En4 : Enum
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
        where Str4 : struct, IDimensionAccessible,
            IInitializable<Str4>, IInitializable<Str4, double>, IInitializable<Str4, double, En4>,
            INormalized<En4>, INormalizable<Str4>,
            IScaleConvertible<Str4, En4>, IValueAccessible<En4>
        where TComp: DivisionOperands
    {
        #region PROPERTIES
        private static int BaseScale1Ordinal
        {
            get
            {
                En1 bs1 = BaseScale1;
                return Unsafe.As<En1, int>(ref bs1);
            }
        }
        private static int BaseScale2Ordinal
        {
            get
            {
                En2 bs2 = BaseScale2;
                return Unsafe.As<En2, int>(ref bs2);
            }
        }
        private static int BaseScale3Ordinal
        {
            get
            {
                En3 bs3 = BaseScale3;
                return Unsafe.As<En3, int>(ref bs3);
            }
        }
        private static int BaseScale4Ordinal
        {
            get
            {
                En4 bs4 = BaseScale4;
                return Unsafe.As<En4, int>(ref bs4);
            }
        }


        public static En1 BaseScale1 => Str1.BaseScale;
        public static En2 BaseScale2 => Str2.BaseScale;
        public static En3 BaseScale3 => Str3.BaseScale;
        public static En4 BaseScale4 => Str4.BaseScale;

        public int Dimension => 1;

        public double Magnitude { get; private set; }
        public (En1 Scale1, En2 Scale2, En3 Scale3, En4 Scale4,
            int Oridnal1, int Ordinal2, int Ordinal3, int Ordinal4) Scales
        { get; private set; }
        #endregion

        #region CONSTRUCTORS
        public QuaternaryQuotientUnit(double magnitude)
        {
            Magnitude = magnitude;
            Scales = (BaseScale1, BaseScale2, BaseScale3, BaseScale4,
                BaseScale1Ordinal, BaseScale2Ordinal, BaseScale3Ordinal, BaseScale4Ordinal);
        }
        #endregion

        #region METHODS
        public static QuaternaryQuotientUnit<TComp, Str1, En1, Str2, En2, Str3, En3, Str4, En4> Create(double mag) => new(mag);
        public QuaternaryQuotientUnit<TComp, Str1, En1, Str2, En2, Str3, En3, Str4, En4> SetScales
            (En1 scale1, En2 scale2, En3 scale3, En4 scale4)
        {
            Scales = (scale1, scale2, scale3, scale4,
                Unsafe.As<En1, int>(ref scale1),
                Unsafe.As<En2, int>(ref scale2),
                Unsafe.As<En3, int>(ref scale3),
                Unsafe.As<En4, int>(ref scale4));
            return this;
        }
        #endregion

    }
}