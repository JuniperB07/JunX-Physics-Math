using JunX.Physics.BaseUnits;
using JunX.Mathematics.Geometry;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace JunX.Physics.Kinematics
{
    /// <summary>
    /// Represents a one-dimensional speed/velocity magnitude structure supporting scale conversions, composite conversions, non-negativity validation, and higher-order dimensional exponentiation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="Velocity"/> implements foundational physical measurement contracts including <see cref="ILinearUnit{TSelf, TEnum}"/>, <see cref="INormalizable{TSelf}"/>, 
    /// <see cref="IValidatable"/>, <see cref="ICompositeUnit"/>, and <see cref="IExponentiable{TSquared, TCubed, THyper}"/> to manage kinematic speed quantities across SI, imperial, nautical, and geological/astronomical scales (e.g., meters per second, knots, miles per hour, kilometers per million years).
    /// </para>
    /// <para>
    /// It maintains a kinematic dimension of 1 and normalizes values relative to the SI base unit (<see cref="VelocityUnits.MetersPerSecond"/>). 
    /// The structure provides scale conversion pipelines, non-negativity validation via <see cref="IsValid"/> based on original magnitude, two-way decomposition and synthesis with fundamental composite ratio units (<see cref="QuotientUnit{T1, E1, T2, E2}"/> mapping <see cref="Length"/> over <see cref="Time"/>), relational comparisons, linear arithmetic, and dimensional exponentiation into higher-order velocity structures (<see cref="UnitSquared{TUnit, TEnum}"/>, <see cref="UnitCubed{TUnit, TEnum}"/>, and <see cref="HyperUnit{TUnit, TEnum}"/>).
    /// </para>
    /// </remarks>
    public struct Velocity :
        IInitializable<Velocity>, IInitializable<Velocity, double>, IInitializable<Velocity, double, VelocityUnits>,
        IScaleMappable<VelocityUnits>, IScaleConvertible<Velocity, VelocityUnits>,
        INormalized<VelocityUnits>, INormalizable<Velocity>,
        IDimensionAccessible,
        IValueAccessible<VelocityUnits>,
        IDuplicatable<Velocity>,
        IEquatable<Velocity>,
        IValidatable,
        IExponentiable<UnitSquared<Velocity, VelocityUnits>, UnitCubed<Velocity, VelocityUnits>, HyperUnit<Velocity, VelocityUnits>>,
        ILinearUnit<Velocity, VelocityUnits>,
        ICompositeUnit
    {
        private readonly double _mps;

        #region PROPERTIES
        public int Dimension => 1;
        public static Dictionary<VelocityUnits, double> Mapper => new()
        {
            { VelocityUnits.MetersPerSecond, 1.0 },

            { VelocityUnits.KilometersPerSecond,      1e3 },          // 1 km/s = 1,000 m/s
            { VelocityUnits.KilometersPerHour,        1.0 / 3.6 },    // 1 km/h = 0.2777777777777778 m/s
            { VelocityUnits.MillimetersPerSecond,     1e-3 },         // 1 mm/s = 0.001 m/s
            { VelocityUnits.MicrometersPerSecond,     1e-6 },

            { VelocityUnits.FeetPerSecond,            0.3048 },       // 1 ft/s = 0.3048 m/s (exact)
            { VelocityUnits.MilesPerHour,             0.44704 },      // 1 mph = 0.44704 m/s (exact)
            { VelocityUnits.InchesPerSecond,           0.0254 },

            { VelocityUnits.Knot,                     1852.0 / 3600.0 },
            { VelocityUnits.KilometersPerMillionYears, 1e3 / (1e6 * 365.25 * 86400.0) }
        };

        public static VelocityUnits BaseScale => VelocityUnits.MetersPerSecond;
        public (double Magnitude, VelocityUnits Scale, int ScaleOrdinal) Normalized => (_mps, BaseScale, (int)BaseScale);

        public (double Magnitude, VelocityUnits Scale, int ScaleOrdinal) Original { get; private set; } = (0, BaseScale, (int)BaseScale);
        public (double Magnitude, VelocityUnits Scale, int ScaleOrdinal) Converted { get; private set; } = (0, BaseScale, (int)BaseScale);
        #endregion

        #region CONSTRUCTORS
        public Velocity() => _mps = 0;
        public Velocity(Velocity instance) => this = instance;
        public Velocity(double magnitude)
        {
            Original = (magnitude, BaseScale, (int)BaseScale);
            _mps = magnitude;
        }
        public Velocity(double magnitude, VelocityUnits scale)
        {
            Original = (magnitude, scale, (int)scale);
            _mps = Methods.Scale(magnitude, scale, Mapper) / Mapper[BaseScale];
        }
        #endregion

        #region METHODS
        public static Velocity Initialize() => new();
        public static Velocity Create(Velocity instance) => new(instance);
        public static Velocity Create(double magnitude) => new(magnitude);
        public static Velocity Create(double magnitude, VelocityUnits scale) => new(magnitude, scale);

        public Velocity Duplicate() => new(this);
        public bool Equals(Velocity other) => Normalized.Magnitude == other.Normalized.Magnitude;
        public bool IsValid() => Original.Magnitude >= 0;

        public Velocity Convert(VelocityUnits toScale)
        {
            double mag = Methods.Scale(Original.Magnitude, Original.Scale, Mapper) / Mapper[toScale];
            Converted = (mag, toScale, (int)toScale);
            return this;
        }
        public double As(VelocityUnits scale) => Duplicate().Convert(scale).Converted.Magnitude;
        public Velocity Normalize()
        {
            Original = (Normalized.Magnitude, Normalized.Scale, Normalized.ScaleOrdinal);
            return this;
        }

        public UnitSquared<Velocity, VelocityUnits> Squared() => this * this;
        public UnitCubed<Velocity, VelocityUnits> Cubed() => this * this * this;
        public HyperUnit<Velocity, VelocityUnits> Pow(int exp)
            => new HyperUnit<Velocity, VelocityUnits>(Math.Pow(Normalized.Magnitude, exp)).SetDimension(exp);

        public QuotientUnit<Length, LengthUnits, Time, TimeUnits> ToComposite()
            => new QuotientUnit<Length, LengthUnits, Time, TimeUnits>(Normalized.Magnitude)
            .SetScales(LengthUnits.Meter, TimeUnits.Second);
        public static Velocity FromComposite(QuotientUnit<Length, LengthUnits, Time, TimeUnits> composite)
        {
            if (composite.Original.Scale1 != LengthUnits.Meter || composite.Original.Scale2 != TimeUnits.Second)
                throw new InvalidOperationException(ErrorMsg.COMPOSITE_SCALES_MISMATCH);
            return new(composite.Original.Magnitude);
        }

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
        public static bool operator ==(Velocity l, Velocity r) => l.Equals(r);
        public static bool operator !=(Velocity l, Velocity r) => !l.Equals(r);
        public static bool operator <(Velocity l, Velocity r) => l.Normalized.Magnitude < r.Normalized.Magnitude;
        public static bool operator >(Velocity l, Velocity r) => l.Normalized.Magnitude > r.Normalized.Magnitude;
        public static bool operator <=(Velocity l, Velocity r) => l < r || l == r;
        public static bool operator >=(Velocity l, Velocity r) => l > r || l == r;
        #endregion

        #region SELF ARITHMETIC OPERATORS
        public static Velocity operator +(Velocity l, Velocity r)
            => new(l.Normalized.Magnitude + r.Normalized.Magnitude);
        public static Velocity operator -(Velocity l, Velocity r)
            => new(l.Normalized.Magnitude - r.Normalized.Magnitude);
        public static Velocity operator *(Velocity l, double r)
            => new(l.Normalized.Magnitude * r);
        public static Velocity operator *(double l, Velocity r) => r * l;
        public static Velocity operator /(Velocity l, double r)
            => new(l.Normalized.Magnitude / r);
        #endregion

        #region CROSS-DIMENSIONAL ARITHMETIC OPERATORS
        public static UnitSquared<Velocity, VelocityUnits> operator *(Velocity l, Velocity r)
            => new(l.Normalized.Magnitude * r.Normalized.Magnitude);

        public static double operator /(Velocity l, Velocity r)
            => l.Normalized.Magnitude / r.Normalized.Magnitude;
        #endregion
    }


}
