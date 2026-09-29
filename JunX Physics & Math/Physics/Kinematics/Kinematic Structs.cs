using JunX.Physics.BaseUnits;
using JunX.Mathematics.Geometry;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Numerics;

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
        IBinaryCompositeTransposable<Velocity, QuotientUnit<Length, LengthUnits, Time, TimeUnits>>,
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

        #region DERIVATIONS
        public static Velocity Derive(Length length, Time time) => length / time;

        public static Velocity Average(Velocity[] velocities)
        {
            Velocity sum = new();

            for (int i = 0; i < velocities.Length; i++)
                sum += velocities[i];

            return sum / velocities.Length;
        }
        public static Velocity Average(Length deltaX, Time deltaT) => deltaX / deltaT;
        public static Velocity Average(Length initialX, Length finalX, Time initialT, Time finalT)
            => Average(Length.DeltaX(initialX, finalX), Time.DeltaT(initialT, finalT));
        public static Velocity Averate(Velocity v0, Velocity v) => (v0 + v) / 2;

        public static Velocity Final(Velocity v0, Acceleration a, Time t) => v0 + (a * t);
        public static Velocity Final(Velocity v0, Acceleration a, Length deltaX)
            => (v0.Squared() + (2 * a * deltaX)).Sqrt();

        public static Velocity Delta(Velocity initial, Velocity final) => final - initial;
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

        #region CROSS-UNIT OPERATORS
        public static Length operator *(Velocity l, Time r)
            => new(l.Normalized.Magnitude * r.Normalized.Magnitude);
        public static Acceleration operator /(Velocity l, Time r)
            => new(l.Normalized.Magnitude / r.Normalized.Magnitude);
        #endregion
    }

    /// <summary>
    /// Represents a one-dimensional acceleration structure supporting scale conversions, binary composite transposition, and higher-order dimensional exponentiation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="Acceleration"/> implements foundational physical measurement contracts including <see cref="ILinearUnit{TSelf, TEnum}"/>, <see cref="INormalizable{TSelf}"/>, 
    /// <see cref="IBinaryCompositeTransposable{TSelf, TTransposed}"/>, <see cref="ICompositeUnit"/>, and <see cref="IExponentiable{TSquared, TCubed, THyper}"/> to manage linear rate of change of velocity across standard SI, gravimetric (Galileo, Milligal), imperial, and gravitational constant scales (<see cref="AccelerationUnits.StandardGravity"/>).
    /// </para>
    /// <para>
    /// It maintains a kinematic dimension of 1 and normalizes values relative to the SI base unit (<see cref="AccelerationUnits.MetersPerSecondSquared"/>). 
    /// The structure provides scale conversion pipelines, binary composite transposition and bi-directional synthesis with fundamental composite ratio units (<see cref="QuotientUnit{T1, E1, T2, E2}"/> mapping <see cref="Length"/> over <see cref="UnitSquared{Time, TimeUnits}"/>), relational comparisons, linear arithmetic, and dimensional exponentiation into higher-order acceleration structures (<see cref="UnitSquared{TUnit, TEnum}"/>, <see cref="UnitCubed{TUnit, TEnum}"/>, and <see cref="HyperUnit{TUnit, TEnum}"/>).
    /// </para>
    /// </remarks>
    public struct Acceleration :
        IInitializable<Acceleration>, IInitializable<Acceleration, double>, IInitializable<Acceleration, double, AccelerationUnits>,
        IScaleMappable<AccelerationUnits>, IScaleConvertible<Acceleration, AccelerationUnits>,
        INormalized<AccelerationUnits>, INormalizable<Acceleration>,
        IDimensionAccessible,
        IValueAccessible<AccelerationUnits>,
        IDuplicatable<Acceleration>,
        IEquatable<Acceleration>,
        IBinaryCompositeTransposable<Acceleration, QuotientUnit<Length, LengthUnits, UnitSquared<Time, TimeUnits>, TimeUnits>>,
        IExponentiable<UnitSquared<Acceleration, AccelerationUnits>, UnitCubed<Acceleration, AccelerationUnits>, HyperUnit<Acceleration, AccelerationUnits>>,
        ILinearUnit<Acceleration, AccelerationUnits>,
        ICompositeUnit
    {
        private readonly double _mps2;

        #region PROPERTIES
        public int Dimension => 1;
        public static Dictionary<AccelerationUnits, double> Mapper => new()
        {
            { AccelerationUnits.MetersPerSecondSquared,     1.0 },

            { AccelerationUnits.Galileo,                    0.01 },         // 1 Gal = 1 cm/s² = 0.01 m/s²
            { AccelerationUnits.Milligal,                   1e-5 },         // 1 mGal = 10⁻³ Gal = 10⁻⁵ m/s²
            { AccelerationUnits.Microgal,                   1e-8 },

            { AccelerationUnits.KilometerspPerSecondSquared, 1e3 },         // 1 km/s² = 1,000 m/s²
            { AccelerationUnits.KilometersPerHourPerSecond,  1.0 / 3.6 },

            { AccelerationUnits.FeetPerSecondSquared,       0.3048 },       // 1 ft/s² = 0.3048 m/s² (exact)
            { AccelerationUnits.InchesPerSecondSquared,      0.0254 },       // 1 in/s² = 0.0254 m/s² (exact)
            { AccelerationUnits.MilesPerHourPerSecond,      0.44704 },

            { AccelerationUnits.StandardGravity,            9.80665 },      // 1 g₀ = 9.80665 m/s² (standard gravity)
            { AccelerationUnits.Milligravities,             9.80665e-3 }
        };

        public static AccelerationUnits BaseScale => AccelerationUnits.MetersPerSecondSquared;
        public (double Magnitude, AccelerationUnits Scale, int ScaleOrdinal) Normalized => (_mps2, BaseScale, (int)BaseScale);

        public (double Magnitude, AccelerationUnits Scale, int ScaleOrdinal) Original { get; private set; } = (0, BaseScale, (int)BaseScale);
        public (double Magnitude, AccelerationUnits Scale, int ScaleOrdinal) Converted { get; private set; } = (0, BaseScale, (int)BaseScale);
        #endregion

        #region CONSTRUCTORS
        public Acceleration() => _mps2 = 0;
        public Acceleration(Acceleration instance) => this = instance;
        public Acceleration(double magnitude)
        {
            Original = (magnitude, BaseScale, (int)BaseScale);
            _mps2 = magnitude;
        }
        public Acceleration(double magnitude, AccelerationUnits scale)
        {
            Original = (magnitude, scale, (int)scale);
            _mps2 = Methods.Scale(magnitude, scale, Mapper) / Mapper[BaseScale];
        }
        #endregion

        #region METHODS
        public static Acceleration Initialize() => new();
        public static Acceleration Create(Acceleration instance) => new(instance);
        public static Acceleration Create(double magnitude) => new(magnitude);
        public static Acceleration Create(double magnitude, AccelerationUnits scale) => new(magnitude, scale);

        public Acceleration Duplicate() => new(this);
        public bool Equals(Acceleration other) => Normalized.Magnitude == other.Normalized.Magnitude;

        public Acceleration Convert(AccelerationUnits toScale)
        {
            double mag = Methods.Scale(Original.Magnitude, Original.Scale, Mapper) / Mapper[toScale];
            Converted = (mag, toScale, (int)toScale);
            return this;
        }
        public double As(AccelerationUnits scale) => Duplicate().Convert(scale).Converted.Magnitude;
        public Acceleration Normalize()
        {
            Original = (_mps2, BaseScale, (int)BaseScale);
            return this;
        }

        public UnitSquared<Acceleration, AccelerationUnits> Squared() => this * this;
        public UnitCubed<Acceleration, AccelerationUnits> Cubed() => this * this * this;
        public HyperUnit<Acceleration, AccelerationUnits> Pow(int exp)
            => new HyperUnit<Acceleration, AccelerationUnits>(Math.Pow(Normalized.Magnitude, exp)).SetDimension(exp);

        public QuotientUnit<Length, LengthUnits, UnitSquared<Time, TimeUnits>, TimeUnits> ToComposite()
            => new QuotientUnit<Length, LengthUnits, UnitSquared<Time, TimeUnits>, TimeUnits>(Normalized.Magnitude)
            .SetScales(LengthUnits.Meter, TimeUnits.Second);
        public static Acceleration FromComposite(QuotientUnit<Length, LengthUnits, UnitSquared<Time, TimeUnits>, TimeUnits> composite)
            => new(composite.Original.Magnitude);

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region DERIVATIONS
        public static Acceleration Derive(Velocity velocity, Time time) => velocity / time;
        public static Acceleration Derive((Velocity Final, Velocity Initial) v, Length deltaX)
            => (v.Final.Squared() - v.Initial.Squared()) / (2 * deltaX);
        public static Acceleration Derive((Length Initial, Length Final) x, Velocity v0, Time t)
            => (2 * (Length.DeltaX(x.Initial, x.Final) - (v0 * t))) / t.Squared();

        public static Acceleration Average(Velocity deltaV, Time deltaT) => deltaV / deltaT;
        public static Acceleration Average((Velocity Initial, Velocity Final) v, (Time Initial, Time Final) t)
            => Average(Velocity.Delta(v.Initial, v.Final), Time.DeltaT(t.Initial, t.Final));
        #endregion

        #region CONDITIONAL OPERATORS
        public static bool operator ==(Acceleration l, Acceleration r) => l.Equals(r);
        public static bool operator !=(Acceleration l, Acceleration r) => !l.Equals(r);
        public static bool operator <(Acceleration l, Acceleration r) => l.Normalized.Magnitude < r.Normalized.Magnitude;
        public static bool operator >(Acceleration l, Acceleration r) => l.Normalized.Magnitude > r.Normalized.Magnitude;
        public static bool operator <=(Acceleration l, Acceleration r) => l < r || l == r;
        public static bool operator >=(Acceleration l, Acceleration r) => l > r || l == r;
        #endregion

        #region SELF ARITHMETIC OPERATORS
        public static Acceleration operator +(Acceleration l, Acceleration r)
            => new(l.Normalized.Magnitude + r.Normalized.Magnitude);
        public static Acceleration operator -(Acceleration l, Acceleration r)
            => new(l.Normalized.Magnitude - r.Normalized.Magnitude);
        public static Acceleration operator *(Acceleration l, double r)
            => new(l.Normalized.Magnitude * r);
        public static Acceleration operator *(double l, Acceleration r) => r * l;
        public static Acceleration operator /(Acceleration l, double r)
            => new(l.Normalized.Magnitude / r);
        #endregion

        #region CROSS-DIMENSIONAL ARITHMETIC OPERATORS
        public static UnitSquared<Acceleration, AccelerationUnits> operator *(Acceleration l, Acceleration r)
            => new(l.Normalized.Magnitude * r.Normalized.Magnitude);

        public static double operator /(Acceleration l, Acceleration r) => l.Normalized.Magnitude / r.Normalized.Magnitude;
        #endregion

        #region CROSS-UNIT OPERATORS
        public static Velocity operator *(Acceleration l, Time r)
            => new(l.Normalized.Magnitude * r.Normalized.Magnitude);
        public static Length operator *(Acceleration l, UnitSquared<Time, TimeUnits> r)
            => new(l.Normalized.Magnitude * r.Normalized.Magnitude);
        public static Length operator *(UnitSquared<Time, TimeUnits> l, Acceleration r) => r * l;
        public static UnitSquared<Velocity, VelocityUnits> operator *(Acceleration l, Length r) => r * l;
        #endregion
    }

    public struct AngularVelocity :
        IInitializable<AngularVelocity>, IInitializable<AngularVelocity, double>, IInitializable<AngularVelocity, double, AngularVelocityUnits>,
        IScaleConvertible<AngularVelocity, AngularVelocityUnits>,
        INormalized<AngularVelocityUnits>, INormalizable<AngularVelocity>,
        IDimensionAccessible,
        IValueAccessible<AngularVelocityUnits>,
        IDuplicatable<AngularVelocity>,
        IEquatable<AngularVelocity>,
        IValidatable,
        IBinaryCompositeTransposable<AngularVelocity, QuotientUnit<Angle, AngleUnits, Time, TimeUnits>>,
        IExponentiable<UnitSquared<AngularVelocity, AngularVelocityUnits>, UnitCubed<AngularVelocity, AngularVelocityUnits>, HyperUnit<AngularVelocity, AngularVelocityUnits>>,
        ILinearUnit<AngularVelocity, AngularVelocityUnits>,
        ICompositeUnit
    {
        private readonly double _rps;

        #region PROPERTIES
        public int Dimension => 1;

        public static AngularVelocityUnits BaseScale => AngularVelocityUnits.RadiansPerSecond;
        public (double Magnitude, AngularVelocityUnits Scale, int ScaleOrdinal) Normalized => (_rps, BaseScale, (int)BaseScale);

        public (double Magnitude, AngularVelocityUnits Scale, int ScaleOrdinal) Original { get; private set; } = (0, BaseScale, (int)BaseScale);
        public (double Magnitude, AngularVelocityUnits Scale, int ScaleOrdinal) Converted { get; private set; } = (0, BaseScale, (int)BaseScale);
        #endregion

        #region CONSTRUCTORS
        public AngularVelocity() => _rps = 0;
        public AngularVelocity(AngularVelocity instance) => this = instance;
        public AngularVelocity(double magnitude)
        {
            Original = (magnitude, BaseScale, (int)BaseScale);
            _rps = magnitude;
        }
        #endregion

        #region METHODS
        public static AngularVelocity Initialize() => new();
        public static AngularVelocity Create(AngularVelocity instance) => new(instance);
        public static AngularVelocity Create(double magnitude) => new(magnitude);
        [Obsolete("Method not applicable in this struct.", false)]
        public static AngularVelocity Create(double magnitude, AngularVelocityUnits scale) => new(magnitude);

        public AngularVelocity Duplicate() => new(this);
        public bool Equals(AngularVelocity other) => Normalized.Magnitude.Equals(other.Normalized.Magnitude);
        public bool IsValid() => Original.Magnitude >= 0;

        [Obsolete("Method not applicable in this struct.", false)]
        public AngularVelocity Convert(AngularVelocityUnits toScale)
        {
            Converted = Original;
            return this;
        }
        [Obsolete("Method not applicable in this struct.", false)]
        public double As(AngularVelocityUnits scale) => Duplicate().Convert(scale).Converted.Magnitude;
        [Obsolete("Method not applicable in this struct.", false)]
        public AngularVelocity Normalize()
        {
            Original = Normalized;
            return this;
        }

        public UnitSquared<AngularVelocity, AngularVelocityUnits> Squared() => this * this;
        public UnitCubed<AngularVelocity, AngularVelocityUnits> Cubed() => this * this * this;
        public HyperUnit<AngularVelocity, AngularVelocityUnits> Pow(int exp)
            => new HyperUnit<AngularVelocity, AngularVelocityUnits>(Math.Pow(Normalized.Magnitude, exp)).SetDimension(exp);

        public QuotientUnit<Angle, AngleUnits, Time, TimeUnits> ToComposite()
            => new QuotientUnit<Angle, AngleUnits, Time, TimeUnits>(Normalized.Magnitude)
            .SetScales(AngleUnits.Radians, TimeUnits.Second);
        public static AngularVelocity FromComposite(QuotientUnit<Angle, AngleUnits, Time, TimeUnits> composite)
            => composite.ScaleValues.Scale1 == AngleUnits.Radians || composite.ScaleValues.Scale2 == TimeUnits.Second ?
            new(composite.Original.Magnitude) :
            throw new InvalidOperationException(ErrorMsg.COMPOSITE_SCALES_MISMATCH);

        public override bool Equals([NotNullWhen(true)] object? obj) => obj is AngularVelocity other && Equals(other);
        public override int GetHashCode() => Normalized.Magnitude.GetHashCode();
        #endregion

        #region CONDITIONAL OPERATORS
        public static bool operator ==(AngularVelocity l, AngularVelocity r) => l.Equals(r);
        public static bool operator !=(AngularVelocity l, AngularVelocity r) => !(l == r);
        public static bool operator <(AngularVelocity l, AngularVelocity r) => l.Normalized.Magnitude < r.Normalized.Magnitude;
        public static bool operator >(AngularVelocity l, AngularVelocity r) => l.Normalized.Magnitude > r.Normalized.Magnitude;
        public static bool operator <=(AngularVelocity l, AngularVelocity r) => l < r || l == r;
        public static bool operator >=(AngularVelocity l, AngularVelocity r) => l > r || l == r;
        #endregion

        #region SELF ARITHMETIC OPERATORS
        public static AngularVelocity operator +(AngularVelocity l, AngularVelocity r)
            => new(l.Normalized.Magnitude + r.Normalized.Magnitude);
        public static AngularVelocity operator -(AngularVelocity l, AngularVelocity r)
            => new(l.Normalized.Magnitude - r.Normalized.Magnitude);
        public static AngularVelocity operator *(AngularVelocity l, double r)
            => new(l.Normalized.Magnitude * r);
        public static AngularVelocity operator *(double l, AngularVelocity r) => r * l;
        public static AngularVelocity operator /(AngularVelocity l, double r)
            => new(l.Normalized.Magnitude / r);
        #endregion

        #region CROSS-DIMENSIONAL ARITHMETIC OPERATORS
        public static UnitSquared<AngularVelocity, AngularVelocityUnits> operator *(AngularVelocity l, AngularVelocity r)
            => new(l.Normalized.Magnitude * r.Normalized.Magnitude);

        public static double operator /(AngularVelocity l, AngularVelocity r)
            => l.Normalized.Magnitude / r.Normalized.Magnitude;
        #endregion
    }
}