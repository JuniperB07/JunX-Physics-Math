using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Transactions;
using System.Xml.Schema;
using JunX.Mathematics;

namespace JunX
{
    /// <summary>
    /// Represents a generic two-dimensional (squared) unit measurement capable of scale conversion, 
    /// normalization, cross-dimensional arithmetic, and dimensional reduction via square root operations.
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
    /// The structure handles automatic unit scaling and scale conversions by leveraging the transformation 
    /// rules defined by the underlying linear unit <typeparamref name="Str"/>. Normalization and equality 
    /// operations evaluate values against the base unit scale.
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
        IEquatable<UnitSquared<Str, En>>

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

        public Str Sqrt()
            => Str.Create(Math.Sqrt(Normalized.Magnitude));
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
    /// normalization, cross-dimensional arithmetic, and dimensional reduction via cube root operations.
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
    /// The structure handles automatic unit scaling and scale conversions by leveraging the transformation 
    /// rules defined by the underlying linear unit <typeparamref name="Str"/>. Normalization and equality 
    /// operations evaluate values against the base unit scale.
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
        IEquatable<UnitCubed<Str, En>>

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

        public Str CubeRt()
            => Str.Create(Radical.Root(Normalized.Magnitude, Dimension), Normalized.Scale);
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

    public struct HyperUnit<Str, En> :
        IDimensionAccessible,
        IInitializable<HyperUnit<Str, En>>, IInitializable<HyperUnit<Str, En>, double>, IInitializable<HyperUnit<Str, En>, double, En>,
        INormalized<En>, INormalizable<HyperUnit<Str, En>>,
        IScaleConvertible<HyperUnit<Str, En>, En>,
        IDuplicatable<HyperUnit<Str, En>>,
        IValueAccessible<En>,
        IEquatable<HyperUnit<Str, En>>

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
        public static HyperUnit<Str, En> operator *(HyperUnit<Str, En> l, )
        #endregion
    }
}
