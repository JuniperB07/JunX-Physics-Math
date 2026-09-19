using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Text;
using System.Transactions;
using System.Xml.Schema;
using JunX.Mathematics;

namespace JunX
{
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
        IExponentiable<HyperUnit<Str, En>>

        where En : Enum
        where Str: struct, IDimensionAccessible,
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
            if(toScale.Equals(Original.Scale))
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
        IExponentiable<HyperUnit<Str, En>>

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
        IExponentiable<HyperUnit<Str, En>>

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

    public struct ProductUnit<Str1, En1, Str2, En2> :
        IDimensionAccessible,
        IInitializable<ProductUnit<Str1, En1, Str2, En2>>,
        IInitializable<ProductUnit<Str1, En1, Str2, En2>, double>,
        IInitializable<ProductUnit<Str1, En1, Str2, En2>, double, En1>,
        IDuplicatable<ProductUnit<Str1, En1, Str2, En2>>,
        IEquatable<ProductUnit<Str1, En1, Str2, En2>>

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

        public Type Struct1 = typeof(Str1);
        public Type Struct2 = typeof(Str2);
        public Type Enum1 = typeof(En1);
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
            => new ProductUnit<Str2, En2, Str1, En1>(Original.Magnitude).SetScales(Original.Scale2, Original.Scale1);
        
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
            => Original.Scale1Ordinal == other.Original.Scale1Ordinal && Original.Scale1Ordinal == other.Original.Scale2Ordinal;

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
        public static ProductUnit<UnitSquared<Str1, En1>, En1, Str2, En2> operator *(ProductUnit<Str1, En1, Str2, En2> l, Str1 r)
        {
            if (!l.Original.Scale1.Equals(r.Original.Scale))
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double res = l.Original.Magnitude * r.Original.Magnitude;
            return ProductUnit<UnitSquared<Str1, En1>, En1, Str2, En2>.Create(res)
                .SetScales(r.Original.Scale, l.Original.Scale2);
        }

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
    }
}
