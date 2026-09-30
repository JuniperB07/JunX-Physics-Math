using JunX.Mathematics.Geometry;
using JunX.Physics.ClassicalMechanics;
using JunX.Physics.Kinematics;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Principal;
using System.Text;

namespace JunX.Physics.BaseUnits
{
    /// <summary>
    /// Represents a one-dimensional temporal unit structure supporting scale conversions, normalization, temporal arithmetic, and higher-order dimensional exponentiation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="Time"/> implements foundational physical measurement contracts including <see cref="ILinearUnit{TSelf, TEnum}"/>, <see cref="INormalizable{TSelf}"/>, 
    /// and <see cref="IExponentiable{TSquared, TCubed, THyper}"/> to manage temporal durations spanning quantum, subatomic, civil, astronomical, and cosmological scales.
    /// </para>
    /// <para>
    /// It maintains a temporal dimension of 1 and normalizes values relative to the SI base unit (<see cref="TimeUnits.Second"/>). 
    /// The structure facilitates scale conversions across diverse time units (ranging from Planck time to galactic years and eons), 
    /// equality and relational comparisons, linear arithmetic operations, and dimensional scaling into higher-order temporal constructs (<see cref="UnitSquared{TUnit, TEnum}"/>, <see cref="UnitCubed{TUnit, TEnum}"/>, and <see cref="HyperUnit{TUnit, TEnum}"/>).
    /// </para>
    /// </remarks>
    public struct Time :
        IInitializable<Time>, IInitializable<Time, double>, IInitializable<Time, double, TimeUnits>,
        IScaleMappable<TimeUnits>, IScaleConvertible<Time, TimeUnits>,
        INormalized<TimeUnits>, INormalizable<Time>,
        IDimensionAccessible,
        IValueAccessible<TimeUnits>,
        IDuplicatable<Time>,
        IEquatable<Time>,
        IExponentiable<UnitSquared<Time, TimeUnits>, UnitCubed<Time, TimeUnits>, HyperUnit<Time, TimeUnits>>,
        ILinearUnit<Time, TimeUnits>
    {
        private readonly double _s;

        #region PROPERTIES
        public int Dimension => 1;
        public static Dictionary<TimeUnits, double> Mapper => new()
        {
            { TimeUnits.PlanckTime, 5.391247e-44 },
            { TimeUnits.Attosecond, 1e-18 },
            { TimeUnits.Femtosecond, 1e-15 },
            { TimeUnits.Picosecond, 1e-12 },
            { TimeUnits.Nanosecond, 1e-9 },
            { TimeUnits.Microsecond, 1e-6 },
            { TimeUnits.Millisecond, 1e-3 },
            { TimeUnits.Jiffy, 1e-2 },
            { TimeUnits.Second, 1.0 },

            { TimeUnits.Minute, 60.0 },
            { TimeUnits.Hour, 3600.0 },
            { TimeUnits.Day, 86400.0 },
            { TimeUnits.Week, 604800.0 },
            { TimeUnits.Month, 2.628e6 },
            { TimeUnits.Year, 3.1556952e7 },
            { TimeUnits.Decade, 3.1556952e8 },
            { TimeUnits.Century, 3.1556952e9 },
            { TimeUnits.Millenium, 3.1556952e10 },

            { TimeUnits.Megaannum, 3.1556952e13 },  // 1 Million Years (Ma)
            { TimeUnits.SiderialDay, 86164.0905 }, // 23h 56m 4.0905s relative to fixed stars
            { TimeUnits.SynodicMonth, 2551442.8 }, // Lunar month cycle (~29.53059 days)
            { TimeUnits.JulianYear, 3.15576e7 },   // Exact 365.25 days (SI astronomical standard)
            { TimeUnits.GalacticYear, 7.2576e15 }, // ~230 million Julian years (orbit around Milky Way core)
            { TimeUnits.Eon, 3.1556952e16 }
        };

        public static TimeUnits BaseScale => TimeUnits.Second;
        public (double Magnitude, TimeUnits Scale, int ScaleOrdinal) Normalized => (_s, BaseScale, (int)BaseScale);

        public (double Magnitude, TimeUnits Scale, int ScaleOrdinal) Original { get; private set; }
        public (double Magnitude, TimeUnits Scale, int ScaleOrdinal) Converted { get; private set; }
        #endregion

        #region CONSTRUCTORS
        public Time()
        {
            Original = (0, BaseScale, (int)BaseScale);
            Converted = (0, BaseScale, (int)BaseScale);
            _s = 0;
        }
        public Time(Time instance)
        {
            this = instance;
        }
        public Time(double magnitude)
        {
            Original = (magnitude, BaseScale, (int)BaseScale);
            Converted = (0, BaseScale, (int)BaseScale);
            _s = magnitude;
        }
        public Time(double magnitude, TimeUnits scale)
        {
            Original = (magnitude, scale, (int)scale);
            Converted = (0, BaseScale, (int)BaseScale);
            _s = Methods.Scale(magnitude, scale, Mapper) / Mapper[BaseScale];
        }
        #endregion

        #region METHODS
        public static Time Initialize() => new();
        public static Time Create(Time instance) => new(instance);
        public static Time Create(double magnitude) => new(magnitude);
        public static Time Create(double magnitude, TimeUnits scale) => new(magnitude, scale);

        public Time Duplicate() => new(this);
        public bool Equals(Time other) => Normalized.Magnitude == other.Normalized.Magnitude;
        
        public Time Convert(TimeUnits toScale)
        {
            double mag = Methods.Scale(Original.Magnitude, Original.Scale, Mapper) / Mapper[toScale];
            Converted = (mag, toScale, (int)toScale);
            return this;
        }
        public double As(TimeUnits scale) => Duplicate().Convert(scale).Converted.Magnitude;
        public Time Normalize()
        {
            Original = (Normalized.Magnitude, Normalized.Scale, Normalized.ScaleOrdinal);
            return this;
        }

        public UnitSquared<Time, TimeUnits> Squared() => this * this;
        public UnitCubed<Time, TimeUnits> Cubed() => this * this * this;
        public HyperUnit<Time, TimeUnits> Pow(int exp)
            => new HyperUnit<Time, TimeUnits>(Math.Pow(Normalized.Magnitude, exp)).SetDimension(Dimension * exp);

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
        public static Time Derive(Length length, Velocity velocity) => length / velocity;
        public static Time Derive(Length length, Acceleration acceleration) => (length / acceleration).Sqrt();

        public static Time DeltaT(Time initial, Time final) => final - initial;
        #endregion

        #region CONDITIONAL OPERATORS
        public static bool operator ==(Time l, Time r) => l.Equals(r);
        public static bool operator !=(Time l, Time r) => !(l == r);
        public static bool operator <(Time l, Time r) => l.Normalized.Magnitude < r.Normalized.Magnitude;
        public static bool operator >(Time l, Time r) => l.Normalized.Magnitude > r.Normalized.Magnitude;
        public static bool operator <=(Time l, Time r) => l < r || l == r;
        public static bool operator >=(Time l, Time r) => l > r || l == r;
        #endregion

        #region SELF ARITHMETIC OPERATORS
        public static Time operator +(Time l, Time r)
            => new(l.Normalized.Magnitude + r.Normalized.Magnitude);
        public static Time operator -(Time l, Time r)
            => new(l.Normalized.Magnitude - r.Normalized.Magnitude);
        public static Time operator *(Time l, double r)
            => new(l.Normalized.Magnitude * r);
        public static Time operator *(double l, Time r) => r * l;
        public static Time operator /(Time l, double r)
            => new(l.Normalized.Magnitude / r);
        #endregion

        #region CROSS DIMENSIONAL ARITHMETIC OPERATORS
        public static UnitSquared<Time, TimeUnits> operator *(Time l, Time r)
            => new(l.Normalized.Magnitude * r.Normalized.Magnitude);

        public static double operator /(Time l, Time r) => l.Normalized.Magnitude / r.Normalized.Magnitude;
        #endregion

        #region CROSS-UNIT OPERATORS
        public static Length operator *(Time l, Velocity r) => r * l;
        public static Velocity operator *(Time l, Acceleration r) => r * l;
        public static Angle operator *(Time l, AngularVelocity r) => r * l;
        public static AngularVelocity operator *(Time l, AngularAcceleration r) => r * l;
        #endregion
    }

    /// <summary>
    /// Represents a one-dimensional mass unit structure supporting scale conversions, normalization, validation, and higher-order dimensional exponentiation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="Mass"/> implements foundational physical measurement contracts including <see cref="ILinearUnit{TSelf, TEnum}"/>, <see cref="INormalizable{TSelf}"/>, 
    /// <see cref="IValidatable"/>, and <see cref="IExponentiable{TSquared, TCubed, THyper}"/> to manage mass quantities across quantum/subatomic ($eV/c^2$, Da), 
    /// avoirdupois/apothecary, metric, and astronomical/cosmological scales ($M_{\odot}$, $M_{\oplus}$).
    /// </para>
    /// <para>
    /// It maintains a mass dimension of 1 and normalizes values relative to the SI base unit (<see cref="MassUnits.Kilogram"/>). 
    /// The structure provides scale conversion pipelines, non-negativity validation via <see cref="IsValid"/>, relational comparisons, linear arithmetic operations, 
    /// and dimensional exponentiation into higher-order mass structures (<see cref="UnitSquared{TUnit, TEnum}"/>, <see cref="UnitCubed{TUnit, TEnum}"/>, and <see cref="HyperUnit{TUnit, TEnum}"/>).
    /// </para>
    /// </remarks>
    public struct Mass :
        IInitializable<Mass>, IInitializable<Mass, double>, IInitializable<Mass, double, MassUnits>,
        IScaleMappable<MassUnits>, IScaleConvertible<Mass, MassUnits>,
        INormalized<MassUnits>, INormalizable<Mass>,
        IDimensionAccessible,
        IValueAccessible<MassUnits>,
        IDuplicatable<Mass>,
        IEquatable<Mass>,
        IValidatable,
        IExponentiable<UnitSquared<Mass, MassUnits>, UnitCubed<Mass, MassUnits>, HyperUnit<Mass, MassUnits>>,
        ILinearUnit<Mass, MassUnits>
    {
        private readonly double _kg;

        #region PROPERTIES
        public int Dimension => 1;
        public static Dictionary<MassUnits, double> Mapper => new()
        {
            { MassUnits.PlanckMass, 2.176434e-8 },                    // mₚ = √(ħc/G) (~2.176434 × 10⁻⁸ kg)
            { MassUnits.Dalton, 1.66053906660e-27 },                  // Da / u (unified atomic mass unit, exact per CODATA)
            { MassUnits.ElectronvoltEquivalent, 1.78266192162790e-36 },// eV/c²
            { MassUnits.MegaelectronvoltEquivalent, 1.78266192162790e-30 }, // MeV/c²
            { MassUnits.GigaelectronvoltEquivalent, 1.78266192162790e-27 },

            { MassUnits.Microgram, 1e-9 },                             // µg
            { MassUnits.Milligram, 1e-6 },                             // mg
            { MassUnits.Gram, 1e-3 },                                  // g
            { MassUnits.Carat, 2e-4 },                                 // CD (0.2 g, exact)
            { MassUnits.Kilogram, 1.0 },                               // kg (SI Base Unit)
            { MassUnits.MetricTon, 1e3 },                              // t (1,000 kg)
            { MassUnits.Kiloton, 1e6 },                                // kt
            { MassUnits.Megaton, 1e9 },                                // Mt
            { MassUnits.Gigaton, 1e12 },

            { MassUnits.Grain, 6.479891e-5 },                          // gr (exact: 64.79891 mg)
            { MassUnits.Dram, 1.7718451953125e-3 },                    // dr (16 drams = 1 oz)
            { MassUnits.Ounce, 0.028349523125 },                       // oz (exact international avoirdupois)
            { MassUnits.Pound, 0.45359237 },                           // lb (exact international avoirdupois)
            { MassUnits.Slug, 14.5939029372 },                         // slug (1 lbf·s²/ft)
            { MassUnits.Stone, 6.35029318 },                           // st (14 lb)
            { MassUnits.Pennyweight, 1.55517384e-3 },

            { MassUnits.ApothecariesScruple, 1.2959782e-3 },           // ℈ (20 grains)
            { MassUnits.ApothecariesDram, 3.8879346e-3 },              // ʒ (60 grains / 3 scruples)
            { MassUnits.ApothecariesOunce, 0.0311034768 },             // ℥ / troy oz (480 grains / 8 drams)
            { MassUnits.ApothecariesPound, 0.3732417216 },

            { MassUnits.EarthMass, 5.972168e24 },                      // M⊕ (IAU 2015 nominal)
            { MassUnits.JupiterMass, 1.8981246e27 },                   // Mⱼ (IAU 2015 nominal)
            { MassUnits.SolarMass, 1.988416e30 }
        };

        public static MassUnits BaseScale => MassUnits.Kilogram;
        public (double Magnitude, MassUnits Scale, int ScaleOrdinal) Normalized => (_kg, BaseScale, (int)BaseScale);

        public (double Magnitude, MassUnits Scale, int ScaleOrdinal) Original { get; private set; }
        public (double Magnitude, MassUnits Scale, int ScaleOrdinal) Converted { get; private set; }
        #endregion

        #region CONSTRUCTORS
        public Mass()
        {
            Original = (0, BaseScale, (int)BaseScale);
            Converted = (0, BaseScale, (int)BaseScale);
            _kg = 0;
        }
        public Mass(Mass instance) => this = instance;
        public Mass(double magnitude)
        {
            Original = (magnitude, BaseScale, (int)BaseScale);
            Converted = (0, BaseScale, (int)BaseScale);
            _kg = magnitude;
        }
        public Mass(double magnitude, MassUnits scale)
        {
            Original = (magnitude, scale, (int)scale);
            Converted = (0, BaseScale, (int)BaseScale);
            _kg = Methods.Scale(magnitude, scale, Mapper) / Mapper[BaseScale];
        }
        #endregion

        #region METHODS
        public static Mass Initialize() => new();
        public static Mass Create(Mass instance) => new(instance);
        public static Mass Create(double magnitude) => new(magnitude);
        public static Mass Create(double magnitude, MassUnits scale) => new(magnitude, scale);

        public Mass Duplicate() => new(this);
        public bool Equals(Mass other) => Normalized.Magnitude == other.Normalized.Magnitude;
        public bool IsValid() => Original.Magnitude >= 0;

        public Mass Convert(MassUnits toScale)
        {
            double mag = Methods.Scale(Original.Magnitude, Original.Scale, Mapper) / Mapper[toScale];
            Converted = (mag, toScale, (int)toScale);
            return this;
        }
        public double As(MassUnits scale) => Duplicate().Convert(scale).Converted.Magnitude;
        public Mass Normalize()
        {
            Original = (Normalized.Magnitude, Normalized.Scale, Normalized.ScaleOrdinal);
            return this;
        }

        public UnitSquared<Mass, MassUnits> Squared() => this * this;
        public UnitCubed<Mass, MassUnits> Cubed() => this * this * this;
        public HyperUnit<Mass, MassUnits> Pow(int exp)
            => new HyperUnit<Mass, MassUnits>(Math.Pow(Normalized.Magnitude, exp)).SetDimension(Dimension * exp);

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
        public static bool operator ==(Mass l, Mass r) => l.Equals(r);
        public static bool operator !=(Mass l, Mass r) => !(l == r);
        public static bool operator <(Mass l, Mass r) => l.Normalized.Magnitude < r.Normalized.Magnitude;
        public static bool operator >(Mass l, Mass r) => l.Normalized.Magnitude > r.Normalized.Magnitude;
        public static bool operator <=(Mass l, Mass r) => l < r || l == r;
        public static bool operator >=(Mass l, Mass r) => l > r || l == r;
        #endregion

        #region SELF ARITHMETIC OPERATORS
        public static Mass operator +(Mass l, Mass r)
            => new(l.Normalized.Magnitude + r.Normalized.Magnitude);
        public static Mass operator -(Mass l, Mass r)
            => new(l.Normalized.Magnitude - r.Normalized.Magnitude);
        public static Mass operator *(Mass l, double r)
            => new(l.Normalized.Magnitude * r);
        public static Mass operator *(double l, Mass r) => r * l;
        public static Mass operator /(Mass l, double r)
            => new(l.Normalized.Magnitude / r);
        #endregion

        #region CROSS DIMENSIONAL ARITHMETIC OPERATORS
        public static UnitSquared<Mass, MassUnits> operator *(Mass l, Mass r)
            => new(l.Normalized.Magnitude * r.Normalized.Magnitude);
        public static double operator /(Mass l, Mass r)
            => l.Normalized.Magnitude / r.Normalized.Magnitude;
        #endregion

        #region CROSS-UNIT OPERATORS
        public static Force operator *(Mass l, Acceleration r)
            => new(l.Normalized.Magnitude * r.Normalized.Magnitude);
        #endregion
    }


    /// <summary>
    /// Represents a one-dimensional electric current unit structure supporting scale conversions, normalization, linear arithmetic, and higher-order dimensional exponentiation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="Current"/> implements foundational physical measurement contracts including <see cref="ILinearUnit{TSelf, TEnum}"/>, <see cref="INormalizable{TSelf}"/>, 
    /// and <see cref="IExponentiable{TSquared, TCubed, THyper}"/> to manage electric current quantities across subatomic, microelectronic, industrial, and electromagnetic system scales (e.g., abampere, statampere, biot).
    /// </para>
    /// <para>
    /// It maintains an electrical dimension of 1 and normalizes values relative to the SI base unit (<see cref="CurrentUnits.Ampere"/>). 
    /// The structure provides unit conversion pipelines across standard SI prefixes and CGS/EMU/ESU units, relational evaluation, linear arithmetic operations, 
    /// and dimensional scaling into higher-order current structures (<see cref="UnitSquared{TUnit, TEnum}"/>, <see cref="UnitCubed{TUnit, TEnum}"/>, and <see cref="HyperUnit{TUnit, TEnum}"/>).
    /// </para>
    /// </remarks>
    public struct Current :
        IInitializable<Current>, IInitializable<Current, double>, IInitializable<Current, double, CurrentUnits>,
        IScaleMappable<CurrentUnits>, IScaleConvertible<Current, CurrentUnits>,
        INormalized<CurrentUnits>, INormalizable<Current>,
        IDimensionAccessible,
        IValueAccessible<CurrentUnits>,
        IDuplicatable<Current>,
        IEquatable<Current>,
        IExponentiable<UnitSquared<Current, CurrentUnits>, UnitCubed<Current, CurrentUnits>, HyperUnit<Current, CurrentUnits>>,
        ILinearUnit<Current, CurrentUnits>
    {
        private readonly double _A;

        #region PROPERTIES
        public int Dimension => 1;
        public static Dictionary<CurrentUnits, double> Mapper => new()
        {
            { CurrentUnits.Ampere, 1.0 },

            { CurrentUnits.Milliampere, 1e-3 },
            { CurrentUnits.Microampere, 1e-6 },
            { CurrentUnits.Nanoampere, 1e-9 },
            { CurrentUnits.Picoampere, 1e-12 },
            { CurrentUnits.Femtoampere, 1e-15 },
            { CurrentUnits.Attoampere, 1e-18 },
            { CurrentUnits.Kiloampere, 1e3 },
            { CurrentUnits.Megaampere, 1e6 },
            { CurrentUnits.Gigaampere, 1e9 },

            { CurrentUnits.Biot, 10.0 },         // 1 Bi = 10 A
            { CurrentUnits.Abampere, 10.0 },     // 1 abA = 10 A (equivalent to Biot in EMU)
            { CurrentUnits.Statampere, 3.33564e-10 }
        };

        public static CurrentUnits BaseScale => CurrentUnits.Ampere;
        public (double Magnitude, CurrentUnits Scale, int ScaleOrdinal) Normalized => (_A, BaseScale, (int)BaseScale);

        public (double Magnitude, CurrentUnits Scale, int ScaleOrdinal) Original { get; private set; } = (0, BaseScale, (int)BaseScale);
        public (double Magnitude, CurrentUnits Scale, int ScaleOrdinal) Converted { get; private set; } = (0, BaseScale, (int)BaseScale);
        #endregion

        #region CONSTRUCTORS
        public Current()
        {
            _A = 0;
        }
        public Current(Current instance) => this = instance;
        public Current(double magnitude)
        {
            Original = (magnitude, BaseScale, (int)BaseScale);
            _A = magnitude;
        }
        public Current(double magnitude, CurrentUnits scale)
        {
            Original = (magnitude, scale, (int)scale);
            _A = Methods.Scale(magnitude, scale, Mapper) / Mapper[BaseScale];
        }
        #endregion

        #region METHODS
        public static Current Initialize() => new();
        public static Current Create(Current instance) => new(instance);
        public static Current Create(double magnitude) => new(magnitude);
        public static Current Create(double magnitude, CurrentUnits scale) => new(magnitude, scale);

        public Current Duplicate() => new(this);
        public bool Equals(Current other) => Normalized.Magnitude == other.Normalized.Magnitude;
        
        public Current Convert(CurrentUnits toScale)
        {
            double mag = Methods.Scale(Original.Magnitude, Original.Scale, Mapper) / Mapper[toScale];
            Converted = (mag, toScale, (int)toScale);
            return this;
        }
        public double As(CurrentUnits scale) => Duplicate().Convert(scale).Converted.Magnitude;
        public Current Normalize()
        {
            Original = (Normalized.Magnitude, Normalized.Scale, Normalized.ScaleOrdinal);
            return this;
        }

        public UnitSquared<Current, CurrentUnits> Squared() => this * this;
        public UnitCubed<Current, CurrentUnits> Cubed() => this * this * this;
        public HyperUnit<Current, CurrentUnits> Pow(int exp)
            => new HyperUnit<Current, CurrentUnits>(Math.Pow(Normalized.Magnitude, exp)).SetDimension(exp);

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
        public static bool operator ==(Current l, Current r) => l.Equals(r);
        public static bool operator !=(Current l, Current r) => !(l == r);
        public static bool operator <(Current l, Current r) => l.Normalized.Magnitude < r.Normalized.Magnitude;
        public static bool operator >(Current l, Current r) => l.Normalized.Magnitude > r.Normalized.Magnitude;
        public static bool operator <=(Current l, Current r) => l < r || l == r;
        public static bool operator >=(Current l, Current r) => l > r || l == r;
        #endregion

        #region SELF ARITHMETIC OPERATORS
        public static Current operator +(Current l, Current r)
            => new(l.Normalized.Magnitude + r.Normalized.Magnitude);
        public static Current operator -(Current l, Current r)
            => new(l.Normalized.Magnitude - r.Normalized.Magnitude);
        public static Current operator *(Current l, double r)
            => new(l.Normalized.Magnitude * r);
        public static Current operator *(double l, Current r) => r * l;
        public static Current operator /(Current l, double r)
            => new(l.Normalized.Magnitude / r);
        #endregion

        #region CROSS DIMENSIONAL OPERATORS
        public static UnitSquared<Current, CurrentUnits> operator *(Current l, Current r)
            => new(l.Normalized.Magnitude * r.Normalized.Magnitude);

        public static double operator /(Current l, Current r) => l.Normalized.Magnitude / r.Normalized.Magnitude;
        #endregion
    }

    /// <summary>
    /// Represents a one-dimensional thermodynamic temperature unit structure supporting affine scale conversions, normalization, non-negativity validation, and higher-order dimensional exponentiation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="Temperature"/> implements foundational physical measurement contracts including <see cref="ILinearUnit{TSelf, TEnum}"/>, <see cref="INormalizable{TSelf}"/>, 
    /// <see cref="IValidatable"/>, and <see cref="IExponentiable{TSquared, TCubed, THyper}"/> to manage absolute and relative temperature quantities across scientific and empirical scales (<see cref="TemperatureUnits.Kelvin"/>, <see cref="TemperatureUnits.Celsius"/>, and <see cref="TemperatureUnits.Fahrenheit"/>).
    /// </para>
    /// <para>
    /// It maintains a thermodynamic dimension of 1 and normalizes values relative to the SI base unit (<see cref="TemperatureUnits.Kelvin"/>). 
    /// Unlike purely multiplicative physical units, the structure handles affine offset transformations for Celsius and Fahrenheit scales during conversion and instantiation. 
    /// It also enforces absolute zero physical validity via <see cref="IsValid"/>, and provides relational comparison, linear arithmetic, and dimensional exponentiation into higher-order temperature structures (<see cref="UnitSquared{TUnit, TEnum}"/>, <see cref="UnitCubed{TUnit, TEnum}"/>, and <see cref="HyperUnit{TUnit, TEnum}"/>).
    /// </para>
    /// </remarks>
    public struct Temperature :
        IInitializable<Temperature>, IInitializable<Temperature, double>, IInitializable<Temperature, double, TemperatureUnits>,
        //IScaleMappable<CurrentUnits>, 
        IScaleConvertible<Temperature, TemperatureUnits>,
        INormalized<TemperatureUnits>, INormalizable<Temperature>,
        IDimensionAccessible,
        IValueAccessible<TemperatureUnits>,
        IDuplicatable<Temperature>,
        IEquatable<Temperature>,
        IValidatable,
        IExponentiable<UnitSquared<Temperature, TemperatureUnits>, UnitCubed<Temperature, TemperatureUnits>, HyperUnit<Temperature, TemperatureUnits>>,
        ILinearUnit<Temperature, TemperatureUnits>
    {
        private readonly double _K;

        #region PROPERTIES
        public int Dimension => 1;

        public static TemperatureUnits BaseScale => TemperatureUnits.Kelvin;
        public (double Magnitude, TemperatureUnits Scale, int ScaleOrdinal) Normalized => (_K, BaseScale, (int)BaseScale);

        public (double Magnitude, TemperatureUnits Scale, int ScaleOrdinal) Original { get; private set; } = (0, BaseScale, (int)BaseScale);
        public (double Magnitude, TemperatureUnits Scale, int ScaleOrdinal) Converted { get; private set; } = (0, BaseScale, (int)BaseScale);
        #endregion

        #region CONSTRUCTORS
        public Temperature() => _K = 0;
        public Temperature(Temperature instance) => this = instance;
        public Temperature(double magnitude)
        {
            Original = (magnitude, BaseScale, (int)BaseScale);
            _K = magnitude;
        }
        public Temperature(double magnitude, TemperatureUnits scale)
        {
            Original = (magnitude, scale, (int)scale);

            if (scale == TemperatureUnits.Celsius)
                _K = magnitude + 273.15;
            else if (scale == TemperatureUnits.Fahrenheit)
                _K = (magnitude - 32.0) * (5.0 / 9.0) + 273.15;
            else
                _K = magnitude;
        }
        #endregion

        #region METHODS
        public static Temperature Initialize() => new();
        public static Temperature Create(Temperature instance) => new(instance);
        public static Temperature Create(double magnitude) => new(magnitude);
        public static Temperature Create(double magnitude, TemperatureUnits scale) => new(magnitude, scale);

        public Temperature Duplicate() => new(this);
        public bool Equals(Temperature other) => Normalized.Magnitude == other.Normalized.Magnitude;
        public bool IsValid() => Normalized.Magnitude >= 0;

        public Temperature Convert(TemperatureUnits toScale)
        {
            double mag;

            if (toScale == TemperatureUnits.Fahrenheit)
                mag = (_K - 273.15) * (9.0 / 5.0) + 32;
            else if (toScale == TemperatureUnits.Celsius)
                mag = _K - 273.15;
            else
                mag = _K;

            Converted = (mag, toScale, (int)toScale);
            return this;
        }
        public double As(TemperatureUnits scale) => Duplicate().Convert(scale).Converted.Magnitude;
        public Temperature Normalize()
        {
            Original = (_K, BaseScale, (int)BaseScale);
            return this;
        }

        public UnitSquared<Temperature, TemperatureUnits> Squared() => this * this;
        public UnitCubed<Temperature, TemperatureUnits> Cubed() => this * this * this;
        public HyperUnit<Temperature, TemperatureUnits> Pow(int exp)
            => new HyperUnit<Temperature, TemperatureUnits>(Math.Pow(Normalized.Magnitude, exp)).SetDimension(exp);

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
        public static bool operator ==(Temperature l, Temperature r) => l.Equals(r);
        public static bool operator !=(Temperature l, Temperature r) => !l.Equals(r);
        public static bool operator <(Temperature l, Temperature r) => l.Normalized.Magnitude < r.Normalized.Magnitude;
        public static bool operator >(Temperature l, Temperature r) => l.Normalized.Magnitude > r.Normalized.Magnitude;
        public static bool operator <=(Temperature l, Temperature r) => l < r || l == r;
        public static bool operator >=(Temperature l, Temperature r) => l > r || l == r;
        #endregion

        #region SELF ARITHMETIC OPERATOR
        public static Temperature operator +(Temperature l, Temperature r)
            => new(l.Normalized.Magnitude + r.Normalized.Magnitude);
        public static Temperature operator -(Temperature l, Temperature r)
            => new(l.Normalized.Magnitude - r.Normalized.Magnitude);
        public static Temperature operator *(Temperature l, double r)
            => new(l.Normalized.Magnitude * r);
        public static Temperature operator *(double l, Temperature r) => r * l;
        public static Temperature operator /(Temperature l, double r)
            => new(l.Normalized.Magnitude / r);
        #endregion

        #region CROSS DIMENSIONAL ARITHMETIC OPERATOR 
        public static UnitSquared<Temperature, TemperatureUnits> operator *(Temperature l, Temperature r)
            => new(l.Normalized.Magnitude * r.Normalized.Magnitude);

        public static double operator /(Temperature l, Temperature r)
            => l.Normalized.Magnitude / r.Normalized.Magnitude;
        #endregion
    }
}
