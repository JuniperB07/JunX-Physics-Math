using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Quic;
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

        /// <summary>
        /// Gets the spatial dimensionality of the measurement, which is fixed at 2 for a squared unit.
        /// </summary>
        public int Dimension => 2;

        /// <summary>
        /// Gets the base (unscaled default) unit scale enumeration value defined by the underlying 1D unit <typeparamref name="Str"/>.
        /// </summary>
        public static En BaseScale => Str.BaseScale;
        /// <summary>
        /// Gets the normalized state of the 2D measurement expressed in terms of the fundamental base unit scale.
        /// </summary>
        /// <value>
        /// A tuple containing:
        /// <list type="bullet">
        /// <item><description><c>Magnitude</c>: The 2D area value scaled to the base unit scale.</description></item>
        /// <item><description><c>Scale</c>: The base unit scale enumeration value.</description></item>
        /// <item><description><c>ScaleOrdinal</c>: The underlying integer ordinal of the base unit scale.</description></item>
        /// </list>
        /// </value>
        /// <remarks>
        /// If the current unit scale matches <see cref="BaseScale"/>, the property directly returns <see cref="Original"/>. 
        /// Otherwise, it performs 2D normalization by extracting the square root of the magnitude, converting the 
        /// 1D linear magnitude to the base scale, and squaring the normalized result back into a 2D magnitude.
        /// </remarks>
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

        /// <summary>
        /// Gets the original, unconverted measurement state as created or directly assigned.
        /// </summary>
        /// <value>
        /// A tuple holding the magnitude, unit scale, and scale ordinal index of the measurement prior to scale conversions.
        /// </value>
        public (double Magnitude, En Scale, int ScaleOrdinal) Original { get; private set; }
        /// <summary>
        /// Gets the measurement state resulting from the most recent explicit unit scale conversion.
        /// </summary>
        /// <value>
        /// A tuple holding the magnitude, unit scale, and scale ordinal index following a conversion operation.
        /// </value>
        public (double Magnitude, En Scale, int ScaleOrdinal) Converted { get; private set; }
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitSquared{Str, En}"/> struct with a zero magnitude and default base scale.
        /// </summary>
        public UnitSquared()
        {
            Original = (0, BaseScale, BaseScaleOrdinal);
            Converted = Original;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitSquared{Str, En}"/> struct by copying the state of an existing instance.
        /// </summary>
        /// <param name="instance">The existing 2D unit measurement instance to duplicate.</param>
        public UnitSquared(UnitSquared<Str, En> instance)
        {
            this = instance;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitSquared{Str, En}"/> struct with a specified magnitude using the default base scale.
        /// </summary>
        /// <param name="magnitude">The scalar value representing the 2D spatial quantity.</param>
        public UnitSquared(double magnitude)
        {
            Original = (magnitude, BaseScale, BaseScaleOrdinal);
            Converted = (0, BaseScale, BaseScaleOrdinal);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitSquared{Str, En}"/> struct with a specified magnitude and unit scale.
        /// </summary>
        /// <param name="magnitude">The scalar value representing the 2D spatial quantity.</param>
        /// <param name="scale">The unit scale or prefix enumeration value for the measurement.</param>
        public UnitSquared(double magnitude, En scale)
        {
            Original = (magnitude, scale, Unsafe.As<En, int>(ref scale));
            Converted = (0, BaseScale, BaseScaleOrdinal);
        }
        #endregion

        #region METHODS
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitSquared{Str, En}"/> struct with default values.
        /// </summary>
        /// <returns>A new <see cref="UnitSquared{Str, En}"/> instance set to a magnitude of zero and the default base scale.</returns>
        public static UnitSquared<Str, En> Initialize() => new();
        /// <summary>
        /// Creates a new instance of the <see cref="UnitSquared{Str, En}"/> struct by copying an existing instance.
        /// </summary>
        /// <param name="instance">The 2D unit measurement instance to duplicate.</param>
        /// <returns>A new <see cref="UnitSquared{Str, En}"/> instance with identical state to <paramref name="instance"/>.</returns>
        public static UnitSquared<Str, En> Create(UnitSquared<Str, En> instance) => new(instance);
        /// <summary>
        /// Creates a new instance of the <see cref="UnitSquared{Str, En}"/> struct with the specified magnitude and default base scale.
        /// </summary>
        /// <param name="magnitude">The scalar magnitude of the 2D measurement.</param>
        /// <returns>A new <see cref="UnitSquared{Str, En}"/> instance configured with <paramref name="magnitude"/> and <see cref="BaseScale"/>.</returns>
        public static UnitSquared<Str, En> Create(double magnitude) => new(magnitude);
        /// <summary>
        /// Creates a new instance of the <see cref="UnitSquared{Str, En}"/> struct with the specified magnitude and unit scale.
        /// </summary>
        /// <param name="magnitude">The scalar magnitude of the 2D measurement.</param>
        /// <param name="scale">The unit scale or prefix enumeration value.</param>
        /// <returns>A new <see cref="UnitSquared{Str, En}"/> instance configured with <paramref name="magnitude"/> and <paramref name="scale"/>.</returns>
        public static UnitSquared<Str, En> Create(double magnitude, En scale) => new(magnitude, scale);

        /// <summary>
        /// Returns a new <see cref="UnitSquared{Str, En}"/> instance normalized to the fundamental base unit scale.
        /// </summary>
        /// <returns>
        /// A new <see cref="UnitSquared{Str, En}"/> instance whose magnitude reflects the 2D area value scaled to <see cref="BaseScale"/>.
        /// </returns>
        /// <remarks>
        /// This method uses the <see cref="Normalized"/> property to convert the 1D linear components of the measurement 
        /// to the base scale before converting the overall 2D magnitude.
        /// </remarks>
        public UnitSquared<Str, En> Normalize()
            => new UnitSquared<Str, En>(Normalized.Magnitude, Normalized.Scale);

        /// <summary>
        /// Creates a new copy of the current <see cref="UnitSquared{Str, En}"/> instance.
        /// </summary>
        /// <returns>A new <see cref="UnitSquared{Str, En}"/> instance with identical state to this instance.</returns>
        public UnitSquared<Str, En> Duplicate() => new(this);
        /// <summary>
        /// Indicates whether the current 2D unit measurement is equal to another <see cref="UnitSquared{Str, En}"/> measurement 
        /// by comparing their normalized magnitudes.
        /// </summary>
        /// <param name="us">An instance of <see cref="UnitSquared{Str, En}"/> to compare with this measurement.</param>
        /// <returns>
        /// <see langword="true"/> if both measurements represent equal 2D magnitudes when normalized to their fundamental base scale; 
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public bool Equals(UnitSquared<Str, En> us)
            => Normalized.Magnitude == us.Normalized.Magnitude;

        /// <summary>
        /// Converts the current 2D measurement to the specified target unit scale, updating the internal <see cref="Converted"/> state.
        /// </summary>
        /// <param name="toScale">The target unit scale enumeration value to convert this 2D measurement into.</param>
        /// <returns>
        /// A reference to the current <see cref="UnitSquared{Str, En}"/> instance with its <see cref="Converted"/> property set to the converted state.
        /// </returns>
        /// <remarks>
        /// <para>
        /// If <paramref name="toScale"/> matches <see cref="Original"/>.<c>Scale</c>, <see cref="Converted"/> is directly synchronized with <see cref="Original"/>.
        /// </para>
        /// <para>
        /// For cross-scale conversions, 2D scale transformation is performed by taking the square root of the 2D magnitude, converting the underlying 
        /// 1D unit <typeparamref name="Str"/> to <paramref name="toScale"/>, and then squaring the resulting converted magnitude.
        /// </para>
        /// </remarks>
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
        /// <summary>
        /// Returns the magnitude of this 2D measurement converted to the specified unit scale without modifying the original instance.
        /// </summary>
        /// <param name="scale">The unit scale in which to express the 2D magnitude.</param>
        /// <returns>The calculated 2D scalar magnitude in terms of <paramref name="scale"/>.</returns>
        public double As(En scale) => Duplicate().Convert(scale).Converted.Magnitude;

        /// <summary>
        /// Returns a new <see cref="HyperUnit{Str, En}"/> representing the current measurement squared (raised to the power of 2).
        /// </summary>
        /// <returns>
        /// A <see cref="HyperUnit{Str, En}"/> instance resulting from self-multiplication, effectively doubling the dimensional degree.
        /// </returns>
        public HyperUnit<Str, En> Squared() => this * this;
        /// <summary>
        /// Returns a new <see cref="HyperUnit{Str, En}"/> representing the current measurement cubed (raised to the power of 3).
        /// </summary>
        /// <returns>
        /// A <see cref="HyperUnit{Str, En}"/> instance resulting from multiplying this measurement by itself twice, tripling the dimensional degree.
        /// </returns>
        public HyperUnit<Str, En> Cubed() => this * this * this;
        /// <summary>
        /// Raises the current measurement to an integer exponent, adjusting both its magnitude and dimensional degree accordingly.
        /// </summary>
        /// <param name="exp">The integer exponent to raise the unit measurement to.</param>
        /// <returns>
        /// A new <see cref="HyperUnit{Str, En}"/> instance with its normalized magnitude raised to <paramref name="exp"/> 
        /// and its dimension scaled by <paramref name="exp"/>.
        /// </returns>
        /// <remarks>
        /// The operation normalizes the base magnitude before applying <see cref="Math.Pow(double, double)"/> to maintain consistent scale semantics across hyper-dimensional transformations.
        /// </remarks>
        public HyperUnit<Str, En> Pow(int exp)
            => new HyperUnit<Str, En>(Math.Pow(Normalized.Magnitude, exp)).SetDimension(Dimension * exp);

        /// <summary>
        /// Calculates the square root of the 2D measurement, reducing it back to its 1D fundamental linear unit <typeparamref name="Str"/>.
        /// </summary>
        /// <returns>
        /// A new 1D unit instance of <typeparamref name="Str"/> initialized with the square root of this measurement's normalized magnitude.
        /// </returns>
        public Str Sqrt()
            => Str.Create(Math.Sqrt(Normalized.Magnitude));

        /// <summary>
        /// Converts the current 2D measurement into a generalized, dynamic-dimensional <see cref="HyperUnit{Str, En}"/> representation.
        /// </summary>
        /// <returns>
        /// A <see cref="HyperUnit{Str, En}"/> instance carrying the original magnitude, scale, and dimensional degree of <c>2</c>.
        /// </returns>
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
        /// <summary>
        /// Multiplies a 2D squared unit measurement (<see cref="UnitSquared{Str, En}"/>) by a 1D linear unit measurement (<typeparamref name="Str"/>) 
        /// to yield a 3D cubed unit measurement (<see cref="UnitCubed{Str, En}"/>).
        /// </summary>
        /// <param name="l">The left-hand 2D squared unit operand.</param>
        /// <param name="r">The right-hand 1D unit operand.</param>
        /// <returns>
        /// A new <see cref="UnitCubed{Str, En}"/> instance initialized with the product of their normalized magnitudes.
        /// </returns>
        public static UnitCubed<Str, En> operator *(UnitSquared<Str, En> l, Str r)
            => new(l.Normalized.Magnitude * r.Normalized.Magnitude);
        /// <summary>
        /// Multiplies two 2D squared unit measurements to produce a 4D generalized hyper-dimensional unit (<see cref="HyperUnit{Str, En}"/>).
        /// </summary>
        /// <param name="l">The left-hand 2D squared unit operand.</param>
        /// <param name="r">The right-hand 2D squared unit operand.</param>
        /// <returns>
        /// A new <see cref="HyperUnit{Str, En}"/> instance with a dimension of 4 and a magnitude equal to the product of their normalized magnitudes.
        /// </returns>
        public static HyperUnit<Str, En> operator *(UnitSquared<Str, En> l, UnitSquared<Str, En> r)
            => new HyperUnit<Str, En>(l.Normalized.Magnitude * r.Normalized.Magnitude).SetDimension(l.Dimension + r.Dimension);
        /// <summary>
        /// Multiplies a 2D squared unit measurement by a 3D cubed unit measurement (<see cref="UnitCubed{Str, En}"/>) 
        /// to produce a 5D generalized hyper-dimensional unit (<see cref="HyperUnit{Str, En}"/>).
        /// </summary>
        /// <param name="l">The left-hand 2D squared unit operand.</param>
        /// <param name="r">The right-hand 3D cubed unit operand.</param>
        /// <returns>
        /// A new <see cref="HyperUnit{Str, En}"/> instance with a dimension of 5 and a magnitude equal to the product of their normalized magnitudes.
        /// </returns>
        public static HyperUnit<Str, En> operator *(UnitSquared<Str, En> l, UnitCubed<Str, En> r)
            => new HyperUnit<Str, En>(l.Normalized.Magnitude * r.Normalized.Magnitude).SetDimension(l.Dimension + r.Dimension);
        /// <summary>
        /// Multiplies a 2D squared unit measurement by a generalized hyper-dimensional unit (<see cref="HyperUnit{Str, En}"/>), 
        /// adding their dimensions together.
        /// </summary>
        /// <param name="l">The left-hand 2D squared unit operand.</param>
        /// <param name="r">The right-hand hyper-dimensional unit operand.</param>
        /// <returns>
        /// A new <see cref="HyperUnit{Str, En}"/> instance whose dimension is increased by 2 and whose magnitude is the product of their normalized magnitudes.
        /// </returns>
        public static HyperUnit<Str, En> operator *(UnitSquared<Str, En> l, HyperUnit<Str, En> r)
            => new HyperUnit<Str, En>(l.Normalized.Magnitude * r.Normalized.Magnitude).SetDimension(l.Dimension + r.Dimension);

        /// <summary>
        /// Divides a 2D squared unit measurement (<see cref="UnitSquared{Str, En}"/>) by a 1D linear unit measurement (<typeparamref name="Str"/>) 
        /// to perform dimensional reduction back to a 1D linear unit.
        /// </summary>
        /// <param name="l">The left-hand 2D squared unit operand (dividend).</param>
        /// <param name="r">The right-hand 1D linear unit operand (divisor).</param>
        /// <returns>
        /// A new 1D linear unit instance of <typeparamref name="Str"/> initialized with the quotient of their normalized magnitudes.
        /// </returns>
        public static Str operator /(UnitSquared<Str, En> l, Str r)
            => Str.Create(l.Normalized.Magnitude / r.Normalized.Magnitude);
        /// <summary>
        /// Divides a 2D squared unit measurement by another 2D squared unit measurement to produce a dimensionless scalar ratio.
        /// </summary>
        /// <param name="l">The left-hand 2D squared unit operand (dividend).</param>
        /// <param name="r">The right-hand 2D squared unit operand (divisor).</param>
        /// <returns>
        /// A <see cref="double"/> scalar value representing the ratio of the normalized magnitudes.
        /// </returns>
        public static double operator /(UnitSquared<Str, En> l, UnitSquared<Str, En> r)
            => l.Normalized.Magnitude / r.Normalized.Magnitude;
        #endregion

        #region A/(B^2)
        public static UnitSquared<Str2, En2> Divide<Str2, En2>
            (Str l,
            QuotientUnit<Str, En, UnitSquared<Str2, En2>, En2> r)
            where En2: Enum
            where Str2: struct, ILinearUnit<Str2, En2>
        {//     B^2 =   A / A/(B^2)

            if (l.Original.ScaleOrdinal != r.Original.Scale1Ordinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude / r.Original.Magnitude;
            return UnitSquared<Str2, En2>.Create(mag, r.Original.Scale2);
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
    }

    public struct BinaryUnit<Str1, En1, Str2, En2> :
        IDimensionAccessible,
        IInitializable<BinaryUnit<Str1, En1, Str2, En2>, double>,
        ICompositeUnit

        where En1 : Enum
        where En2 : Enum
        where Str1 : struct, IDimensionAccessible,
            IInitializable<Str1>, IInitializable<Str1, double>, IInitializable<Str1, double, En1>,
            INormalized<En1>, INormalizable<Str1>,
            IScaleConvertible<Str1, En1>, IValueAccessible<En1>
        where Str2 : struct, IDimensionAccessible,
            IInitializable<Str2>, IInitializable<Str2, double>, IInitializable<Str2, double, En2>,
            INormalized<En2>, INormalizable<Str2>,
            IScaleConvertible<Str2, En2>, IValueAccessible<En2>
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

        public (double Magnitude, En1 Scale1, En2 Scale2, int Ordinal1, int Ordinal2) Components { get; private set; }

        public static En1 BaseScale1 => Str1.BaseScale;
        public static En2 BaseScale2 => Str2.BaseScale;
        #endregion

        #region CONSTRUCTORS
        public BinaryUnit(double magnitude)
        {
            Components = (magnitude, BaseScale1, BaseScale2, BaseScale1Ordinal, BaseScale2Ordinal);
        }
        #endregion

        #region METHODS
        public static BinaryUnit<Str1, En1, Str2, En2> Create(double magnitude) => new(magnitude);

        public BinaryUnit<Str1, En1, Str2, En2> SetScales(En1 scale1, En2 scale2)
        {
            double mag = Components.Magnitude;
            Components = (mag, scale1, scale2,
                Unsafe.As<En1, int>(ref scale1),
                Unsafe.As<En2, int>(ref scale2));
            return this;
        }

        public static ProductUnit<Str1, En1, UnitSquared<Str2, En2>, En2> Divide(ProductUnit<UnitSquared<Str1, En1>, En1, UnitSquared<Str2, En2>, En2> left, Str1 right)
        {// (A^2)(B^2) / A  =   A(B^2)

            if (left.Original.Scale1Ordinal != right.Original.ScaleOrdinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = left.Original.Magnitude / right.Original.Magnitude;

            var unit = ProductUnit<Str1, En1, UnitSquared<Str2, En2>, En2>.Create(mag);
            return unit.SetScales(left.Original.Scale1, left.Original.Scale2);
        }
        public static ProductUnit<UnitSquared<Str1, En1>, En1, Str2, En2> Divide(ProductUnit<UnitSquared<Str1, En1>, En1, UnitSquared<Str2, En2>, En2> left, Str2 right)
        {// (A^2)(B^2) / B  =   (A^2)B

            if (left.Original.Scale2Ordinal != right.Original.ScaleOrdinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = left.Original.Magnitude / right.Original.Magnitude;

            var unit = ProductUnit<UnitSquared<Str1, En1>, En1, Str2, En2>.Create(mag);
            return unit.SetScales(left.Original.Scale1, left.Original.Scale2);
        }
        public static UnitSquared<Str1, En1> Divide(ProductUnit<UnitSquared<Str1, En1>, En1, UnitSquared<Str2, En2>, En2> left, UnitSquared<Str2, En2> right)
        {// (A^2)(B^2) / B^2    =   A^2

            if (left.Original.Scale2Ordinal != right.Original.ScaleOrdinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = left.Original.Magnitude / right.Original.Magnitude;
            return UnitSquared<Str1, En1>.Create(mag, left.Original.Scale1);
        }
        public static UnitSquared<Str2, En2> Divide(ProductUnit<UnitSquared<Str1, En1>, En1, UnitSquared<Str2, En2>, En2> left, UnitSquared<Str1, En1> right)
        {// (A^2)(B^2) / A^2    =   B^2

            if (left.Original.Scale1Ordinal != right.Original.ScaleOrdinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = left.Original.Magnitude / right.Original.Magnitude;
            return UnitSquared<Str2, En2>.Create(mag, left.Original.Scale2);
        }

        public static QuotientUnit<Str1, En1, Str2, En2> Multiply(QuotientUnit<Str1, En1, UnitSquared<Str2, En2>, En2> left, Str2 right)
        {// (A/B^2) * B =   A/B

            if (left.Original.Scale2Ordinal != right.Original.ScaleOrdinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = left.Original.Magnitude * right.Original.Magnitude;
            var unit = QuotientUnit<Str1, En1, Str2, En2>.Create(mag);
            return unit.SetScales(left.Original.Scale1, left.Original.Scale2);
        }
        public static Str1 Multiply(QuotientUnit<Str1, En1, UnitSquared<Str2, En2>, En2> left, UnitSquared<Str2, En2> right)
        {// (A/B^2) * B^2   =   A

            if (left.Original.Scale2Ordinal != right.Original.ScaleOrdinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = left.Original.Magnitude * right.Original.Magnitude;
            return Str1.Create(mag, left.Original.Scale1);
        }
        public static UnitSquared<Str2, En2> Divide(Str1 left, QuotientUnit<Str1, En1, UnitSquared<Str2, En2>, En2> right)
        {// A / (A/B^2) =   B^2

            if (left.Original.ScaleOrdinal != right.Original.Scale1Ordinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = left.Original.Magnitude / right.Original.Magnitude;
            return UnitSquared<Str2, En2>.Create(mag, right.Original.Scale2);
        }
        #endregion
    }

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

        #region FOR: AB * C
        public static TernaryProductUnit<CompositeProduct<Str1, Str2>, Str1, En1, Str2, En2, Str3, En3> Multiply
            (ProductUnit<Str1, En1, Str2, En2> l, Str3 r)
        { // ABC = AB * C
            double mag = l.Original.Magnitude * r.Original.Magnitude;
            var unit = TernaryProductUnit<CompositeProduct<Str1, Str2>, Str1, En1, Str2, En2, Str3, En3>.Create(mag);
            return unit.SetScales(l.Original.Scale1, l.Original.Scale2, r.Original.Scale);
        }
        public static TernaryProductUnit<CompositeProduct<Str1, Str2>, Str1, En1, Str2, En2, Str3, En3> Multiply
            (Str3 l, ProductUnit<Str1, En1, Str2, En2> r) => Multiply(r, l);
        public static ProductUnit<Str1, En1, Str2, En2> Divide
            (TernaryProductUnit<CompositeProduct<Str1, Str2>, Str1, En1, Str2, En2, Str3, En3> l,
            Str3 r)
        {// AB  =   ABC / C

            if (l.Scales.Ordinal3 != r.Original.ScaleOrdinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Magnitude / r.Original.Magnitude;
            var unit = ProductUnit<Str1, En1, Str2, En2>.Create(mag);
            return unit.SetScales(l.Scales.Scale1, l.Scales.Scale2);
        }
        public static ProductUnit<Str1, En1, Str3, En3> Divide
            (TernaryProductUnit<CompositeProduct<Str1, Str2>, Str1, En1, Str2, En2, Str3, En3> l,
            Str2 r)
        {// AC  =   ABC / B

            if (l.Scales.Ordinal2 != r.Original.ScaleOrdinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Magnitude / r.Original.Magnitude;
            var unit = ProductUnit<Str1, En1, Str3, En3>.Create(mag);
            return unit.SetScales(l.Scales.Scale1, l.Scales.Scale3);
        }
        public static ProductUnit<Str2, En2, Str3, En3> Divide
            (TernaryProductUnit<CompositeProduct<Str1, Str2>, Str1, En1, Str2, En2, Str3, En3> l,
            Str1 r)
        {// BC  =   ABC / A

            if (l.Scales.Ordinal1 != r.Original.ScaleOrdinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Magnitude / r.Original.Magnitude;
            var unit = ProductUnit<Str2, En2, Str3, En3>.Create(mag);
            return unit.SetScales(l.Scales.Scale2, l.Scales.Scale3);
        }
        public static Str1 Divide
            (TernaryProductUnit<CompositeProduct<Str1, Str2>, Str1, En1, Str2, En2, Str3, En3> l,
            ProductUnit<Str2, En2, Str3, En3> r)
        {// A   =   ABC / BC

            if (l.Scales.Ordinal2 != r.Original.Scale1Ordinal ||
                l.Scales.Ordinal3 != r.Original.Scale2Ordinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Magnitude / r.Original.Magnitude;
            return Str1.Create(mag, l.Scales.Scale1);
        }
        public static Str2 Divide
            (TernaryProductUnit<CompositeProduct<Str1, Str2>, Str1, En1, Str2, En2, Str3, En3> l,
            ProductUnit<Str1, En1, Str3, En3> r)
        {// B   =   ABC / AC

            if (l.Scales.Ordinal1 != r.Original.Scale1Ordinal ||
                l.Scales.Ordinal3 != r.Original.Scale2Ordinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Magnitude / r.Original.Magnitude;
            return Str2.Create(mag, l.Scales.Scale2);
        }
        public static Str3 Divide
            (TernaryProductUnit<CompositeProduct<Str1, Str2>, Str1, En1, Str2, En2, Str3, En3> l,
            ProductUnit<Str1, En1, Str2, En2> r)
        {// C   =   ABC / AB

            if (l.Scales.Ordinal1 != r.Original.Scale1Ordinal ||
                l.Scales.Ordinal2 != r.Original.Scale2Ordinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Magnitude / r.Original.Magnitude;
            return Str3.Create(mag, l.Scales.Scale3);
        }
        #endregion

        #region FOR: A/B * C
        public static TernaryQuotientUnit<CompositeQuotient<Str1, Str3>, Str1, En1, Str2, En2, Str3, En3> Multiply
            (QuotientUnit<Str1, En1, Str2, En2> l, Str3 r)
        {// AC/B    =   A/B * C
            double mag = l.Original.Magnitude * r.Original.Magnitude;
            var unit = TernaryQuotientUnit<CompositeQuotient<Str1, Str3>, Str1, En1, Str2, En2, Str3, En3>.Create(mag);
            return unit.SetScales(l.Original.Scale1, l.Original.Scale2, r.Original.Scale);
        }
        public static TernaryQuotientUnit<CompositeQuotient<Str1, Str3>, Str1, En1, Str2, En2, Str3, En3> Multiply
            (Str3 l, QuotientUnit<Str1, En1, Str2, En2> r) => Multiply(r, l);
        #endregion
    }

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

        #region FOR: AB / C
        public static TernaryQuotientUnit<CompositeProduct<Str1, Str2>, Str1, En1, Str2, En2, Str3, En3> Divide
            (ProductUnit<Str1, En1, Str2, En2> l, Str3 r)
        {// AB/C    =   AB / C

            double mag = l.Original.Magnitude / r.Original.Magnitude;
            var unit = TernaryQuotientUnit<CompositeProduct<Str1, Str2>, Str1, En1, Str2, En2, Str3, En3>.Create(mag);
            return unit.SetScales(l.Original.Scale1, l.Original.Scale2, r.Original.Scale);
        }
        public static ProductUnit<Str1, En1, Str2, En2> Multiply
            (TernaryQuotientUnit<CompositeProduct<Str1, Str2>, Str1, En1, Str2, En2, Str3, En3> l,
            Str3 r)
        {// AB  =   (AB/C) * C

            if (l.Scales.Ordinal3 != r.Original.ScaleOrdinal)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Magnitude * r.Original.Magnitude;
            var unit = ProductUnit<Str1, En1, Str2, En2>.Create(mag);
            return unit.SetScales(l.Scales.Scale1, l.Scales.Scale2);
        }
        public static ProductUnit<Str1, En1, Str2, En2> Multiply
            (Str3 l,
            TernaryQuotientUnit<CompositeProduct<Str1, Str2>, Str1, En1, Str2, En2, Str3, En3> r)
            => Multiply(r, l);
        public static QuotientUnit<Str1, En1, Str3, En3> Divide
            (Str2 l,
            TernaryQuotientUnit<CompositeProduct<Str1, Str2>, Str1, En1, Str2, En2, Str3, En3> r)
        {// A/C =   B / (AB/C)

            if (l.Original.ScaleOrdinal != r.Scales.Ordinal2)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude / r.Magnitude;
            var unit = QuotientUnit<Str1, En1, Str3, En3>.Create(mag);
            return unit.SetScales(r.Scales.Scale1, r.Scales.Scale3);
        }
        public static QuotientUnit<Str2, En2, Str3, En3> Divide
            (Str1 l,
            TernaryQuotientUnit<CompositeProduct<Str1, Str2>, Str1, En1, Str2, En2, Str3, En3> r)
        {// B/C =   A / (AB/C)

            if (l.Original.ScaleOrdinal != r.Scales.Ordinal1)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_UNIT_SCALE_MISMATCH);

            double mag = l.Original.Magnitude / r.Magnitude;
            var unit = QuotientUnit<Str2, En2, Str3, En3>.Create(mag);
            return unit.SetScales(r.Scales.Scale2, r.Scales.Scale3);
        }

        #endregion
    }
}