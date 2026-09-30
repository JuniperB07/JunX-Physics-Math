using JunX.Mathematics.Geometry;
using JunX.Physics.BaseUnits;
using JunX.Physics.Kinematics;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace JunX.Physics.ClassicalMechanics
{
    /// <summary>
    /// Represents a one-dimensional mechanical force structure supporting scale conversions, binary and ternary composite transpositions, linear arithmetic, and higher-order dimensional exponentiation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="Force"/> implements foundational physical measurement contracts including <see cref="ILinearUnit{TSelf, TEnum}"/>, <see cref="INormalizable{TSelf}"/>, 
    /// <see cref="IBinaryCompositeTransposable{TSelf, TTransposed}"/>, <see cref="ICompositeUnit"/>, and <see cref="IExponentiable{TSquared, TCubed, THyper}"/> to manage linear mechanical force quantities represented by the standard symbol <see cref="SYMBOL"/> (F).
    /// </para>
    /// <para>
    /// It maintains a mechanical force dimension of 1 and normalizes values relative to the SI base unit (<see cref="ForceUnits.Newton"/>). 
    /// The structure provides scale conversion pipelines across CGS (dyne), gravitational metric (kgf, tf), imperial/customary (lbf, poundal, kip), and quantum/theoretical physics scales (atomic force, Planck force). 
    /// It supports bi-directional binary composite transposition with Newtonian dynamics (F = m * a via <see cref="ProductUnit{T1, E1, T2, E2}"/> mapping <see cref="Mass"/> and <see cref="Acceleration"/>) as well as ternary composite decomposition into base SI dimensions (kg * m / s² via <see cref="TernaryProductUnit{TComp, T1, E1, T2, E2, T3, E3}"/>), alongside relational comparison operators, linear arithmetic, and higher-order dimensional exponentiation (F², F³, and Fⁿ).
    /// </para>
    /// </remarks>
    public struct Force :
        IInitializable<Force>, IInitializable<Force, double>, IInitializable<Force, double, ForceUnits>,
        IScaleMappable<ForceUnits>, IScaleConvertible<Force, ForceUnits>,
        INormalized<ForceUnits>, INormalizable<Force>,
        IDimensionAccessible,
        IValueAccessible<ForceUnits>,
        IDuplicatable<Force>,
        IEquatable<Force>,
        IBinaryCompositeTransposable<Force, ProductUnit<Mass, MassUnits, Acceleration, AccelerationUnits>>,
        IExponentiable<UnitSquared<Force, ForceUnits>, UnitCubed<Force, ForceUnits>, HyperUnit<Force, ForceUnits>>,
        ILinearUnit<Force, ForceUnits>,
        ICompositeUnit
    {
        private readonly double _N;
        public const char SYMBOL = 'F';

        #region PROPERTIES
        public int Dimension => 1;
        public static Dictionary<ForceUnits, double> Mapper => new()
        {
            { ForceUnits.Newton,        1.0 },
            { ForceUnits.Kilonewton,    1e3 },
            { ForceUnits.Meganewton,    1e6 },
            { ForceUnits.Dyne,          1e-5 },

            // Gravitational Metric Units
            { ForceUnits.GramForce,     0.00980665 },
            { ForceUnits.KilogramForce, 9.80665 },
            { ForceUnits.TonneForce,    9806.65 },

            // Imperial / Customary Units
            { ForceUnits.PoundForce,    4.4482216152605 },
            { ForceUnits.OunceForce,    0.27801385095378125 }, // (PoundForce / 16)
            { ForceUnits.Poundal,       0.138254954376 },
            { ForceUnits.Kip,           4448.2216152605 },    // (1000 * PoundForce)
            { ForceUnits.ShortTonForce, 8896.443230521 },     // (2000 * PoundForce)
            { ForceUnits.LongTonForce,  9964.01641818352 },   // (2240 * PoundForce)

            // Natural / Theoretical Physics Scales
            { ForceUnits.AtomicForce,   8.23872349825e-8 },   // Hartree per Bohr radius (E_h / a_0)
            { ForceUnits.PlanckForce,   1.21027e44 }
        };

        public static ForceUnits BaseScale => ForceUnits.Newton;
        public (double Magnitude, ForceUnits Scale, int ScaleOrdinal) Normalized => (_N, BaseScale, (int)BaseScale);

        public (double Magnitude, ForceUnits Scale, int ScaleOrdinal) Original { get; private set; } = (0, BaseScale, (int)BaseScale);
        public (double Magnitude, ForceUnits Scale, int ScaleOrdinal) Converted { get; private set; } = (0, BaseScale, (int)BaseScale);
        #endregion

        #region CONSTRUCTORS
        public Force() => _N = 0;
        public Force(Force instance) => this = instance;
        public Force(double magnitude)
        {
            Original = (magnitude, BaseScale, (int)BaseScale);
            _N = magnitude;
        }
        public Force(double magnitude, ForceUnits scale)
        {
            Original = (magnitude, scale, (int)scale);
            _N = Methods.Scale(magnitude, scale, Mapper) / Mapper[BaseScale];
        }
        #endregion

        #region METHODS
        public static Force Initialize() => new();
        public static Force Create(Force instance) => new(instance);
        public static Force Create(double magnitude) => new(magnitude);
        public static Force Create(double magnitude, ForceUnits scale) => new(magnitude, scale);

        public Force Duplicate() => new(this);
        public bool Equals(Force other) => Normalized.Magnitude == other.Normalized.Magnitude;

        public Force Convert(ForceUnits toScale)
        {
            double mag = Methods.Scale(Original.Magnitude, Original.Scale, Mapper) / Mapper[toScale];
            Converted = (mag, toScale, (int)toScale);
            return this;
        }
        public double As(ForceUnits scale) => Duplicate().Convert(scale).Converted.Magnitude;
        public Force Normalize()
        {
            Original = (_N, BaseScale, (int)BaseScale);
            return this;
        }

        public UnitSquared<Force, ForceUnits> Squared() => this * this;
        public UnitCubed<Force, ForceUnits> Cubed() => this * this * this;
        public HyperUnit<Force, ForceUnits> Pow(int exp)
            => new HyperUnit<Force, ForceUnits>(Math.Pow(Normalized.Magnitude, exp)).SetDimension(exp);

        public ProductUnit<Mass, MassUnits, Acceleration, AccelerationUnits> ToComposite()
            => new ProductUnit<Mass, MassUnits, Acceleration, AccelerationUnits>(Normalized.Magnitude)
            .SetScales(MassUnits.Kilogram, AccelerationUnits.MetersPerSecondSquared);
        public TernaryProductUnit<CompositeQuotient<Length, UnitSquared<Time, TimeUnits>>, Mass, MassUnits, Length, LengthUnits, UnitSquared<Time, TimeUnits>, TimeUnits> ToTernaryUnit()
            => new TernaryProductUnit<CompositeQuotient<Length, UnitSquared<Time, TimeUnits>>, Mass, MassUnits, Length, LengthUnits, UnitSquared<Time, TimeUnits>, TimeUnits>(Normalized.Magnitude)
            .SetScales(MassUnits.Kilogram, LengthUnits.Meter, TimeUnits.Second);
        public static Force FromComposite(ProductUnit<Mass, MassUnits, Acceleration, AccelerationUnits> composite)
            => composite.Original.Scale1 == MassUnits.Kilogram && composite.Original.Scale2 == AccelerationUnits.MetersPerSecondSquared ?
            new(composite.Original.Magnitude) :
            throw new InvalidOperationException(ErrorMsg.COMPOSITE_SCALES_MISMATCH);

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
        public static Force Derive(Mass m, Acceleration a) => m * a;
        #endregion

        #region CONDITIONAL OPERATORS
        public static bool operator ==(Force l, Force r) => l.Equals(r);
        public static bool operator !=(Force l, Force r) => !l.Equals(r);
        public static bool operator <(Force l, Force r) => l.Normalized.Magnitude < r.Normalized.Magnitude;
        public static bool operator >(Force l, Force r) => l.Normalized.Magnitude > r.Normalized.Magnitude;
        public static bool operator <=(Force l, Force r) => l < r || l == r;
        public static bool operator >=(Force l, Force r) => l > r || l == r;
        #endregion

        #region SELF ARITHMETIC OPERATORS
        public static Force operator +(Force l, Force r)
            => new(l.Normalized.Magnitude + r.Normalized.Magnitude);
        public static Force operator -(Force l, Force r)
            => new(l.Normalized.Magnitude - r.Normalized.Magnitude);
        public static Force operator *(Force l, double r)
            => new(l.Normalized.Magnitude * r);
        public static Force operator *(double l, Force r) => r * l;
        public static Force operator /(Force l, double r)
            => new(l.Normalized.Magnitude / r);
        #endregion

        #region CROSS-DIMENSIONAL ARITHMETIC OPERATORS
        public static UnitSquared<Force, ForceUnits> operator *(Force l, Force r)
            => new(l.Normalized.Magnitude * r.Normalized.Magnitude);

        public static double operator /(Force l, Force r)
            => l.Normalized.Magnitude / r.Normalized.Magnitude;
        #endregion

        #region CROSS-UNIT OPERATORS
        public static Mass operator /(Force l, Acceleration r)
            => new(l.Normalized.Magnitude / r.Normalized.Magnitude);
        public static Acceleration operator /(Force l, Mass r)
            => new(l.Normalized.Magnitude / r.Normalized.Magnitude);
        #endregion
    }

    /// <summary>
    /// Represents a one-dimensional thermodynamic and physical energy quantity supporting multi-scale unit conversions, binary composite transposition, non-negativity validation, and higher-order dimensional exponentiation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="Energy"/> implements foundational physical measurement contracts including <see cref="ILinearUnit{TSelf, TEnum}"/>, <see cref="INormalizable{TSelf}"/>, 
    /// <see cref="IValidatable"/>, <see cref="IBinaryCompositeTransposable{TSelf, TTransposed}"/>, <see cref="ICompositeUnit"/>, and <see cref="IExponentiable{TSquared, TCubed, THyper}"/> to manage scalar energy quantities represented by the standard symbol <see cref="SYMBOL"/> (E).
    /// </para>
    /// <para>
    /// It maintains an energy dimension of 1 and normalizes values relative to the SI base unit (<see cref="EnergyUnits.Joule"/>). 
    /// The structure provides scale conversion pipelines across mechanical work (foot-pound force), electrical energy (kilowatt-hour), thermal units (calorie, BTU, therm), quantum/particle physics scales (electronvolt, Rydberg, Hartree, wave number), fuel and mass equivalents (toe, tce, boe, TNT, amu via E = mc²), and Planck scale domains. 
    /// It supports non-negativity state validation via <see cref="IsValid"/>, bi-directional binary composite transposition with mechanical work definitions (W = F * d via <see cref="ProductUnit{T1, E1, T2, E2}"/> mapping <see cref="Force"/> and <see cref="Length"/>), relational comparison operators, linear arithmetic, and higher-order dimensional exponentiation (E², E³, and Eⁿ).
    /// </para>
    /// </remarks>
    public struct Energy :
        IInitializable<Energy>, IInitializable<Energy, double>, IInitializable<Energy, double, EnergyUnits>,
        IScaleMappable<EnergyUnits>, IScaleConvertible<Energy, EnergyUnits>,
        INormalized<EnergyUnits>, INormalizable<Energy>,
        IDimensionAccessible,
        IValueAccessible<EnergyUnits>,
        IDuplicatable<Energy>,
        IEquatable<Energy>,
        IValidatable,
        IBinaryCompositeTransposable<Energy, ProductUnit<Force, ForceUnits, Length, LengthUnits>>,
        IExponentiable<UnitSquared<Energy, EnergyUnits>, UnitCubed<Energy, EnergyUnits>, HyperUnit<Energy, EnergyUnits>>,
        ILinearUnit<Energy, EnergyUnits>,
        ICompositeUnit
    {
        private readonly double _J;
        public const char SYMBOL = 'E';

        #region PROPERTIES
        public int Dimension => 1;
        public static Dictionary<EnergyUnits, double> Mapper => new()
        {
            { EnergyUnits.Joule, 1.0 },
            { EnergyUnits.Kilojoule, 1e3 },
            { EnergyUnits.Megajoule, 1e6 },
            { EnergyUnits.Gigajoule, 1e9 },
            { EnergyUnits.Erg, 1e-7 },

            // Electrical & Power-Based Units
            { EnergyUnits.WattHour, 3600.0 },
            { EnergyUnits.KilowattHour, 3.6e6 },
            { EnergyUnits.MegawattHour, 3.6e9 },
            { EnergyUnits.GigawattHour, 3.6e12 },

            // Thermal & Food Units
            { EnergyUnits.Calorie, 4.184 },
            { EnergyUnits.Kilocalorie, 4184.0 },
            { EnergyUnits.BritishThermalUnit, 1055.05585262 },
            { EnergyUnits.Therm, 105505585.262 },      // 100,000 BTU
            { EnergyUnits.Quad, 1.05505585262e18 },    // 10^15 BTU

            // Mechanical & Imperial Units
            { EnergyUnits.FootPoundForce, 1.3558179483314004 },
            { EnergyUnits.FootPoundal, 0.0421401100938048 },
            { EnergyUnits.HorsepowerHour, 2684519.5376961727 },

            // Atomic, Particle & Quantum Units
            { EnergyUnits.Electronvolt, 1.602176634e-19 },
            { EnergyUnits.Kiloelectronvolt, 1.602176634e-16 },
            { EnergyUnits.Megaelectronvolt, 1.602176634e-13 },
            { EnergyUnits.Gigaelectronvolt, 1.602176634e-10 },
            { EnergyUnits.Teraelectronvolt, 1.602176634e-7 },
            { EnergyUnits.Rydberg, 2.1798723611035e-18 },
            { EnergyUnits.Hartree, 4.3597447222071e-18 },
            { EnergyUnits.WaveNumber, 1.98644586e-23 }, // h * c * 100 m^-1

            // Fuel Equivalents & Mass Energy
            { EnergyUnits.OilTonne, 41868000000.0 },   // Tonne of oil equivalent (toe)
            { EnergyUnits.CoalTonne, 29307600000.0 },  // Tonne of coal equivalent (tce)
            { EnergyUnits.OilBarrel, 6120000000.0 },   // Barrel of oil equivalent (boe)
            { EnergyUnits.TNT_Tonne, 4184000000.0 },   // 1 ton TNT (4.184 GJ)
            { EnergyUnits.AtomicMassUnit, 1.49241808560e-10 }, // 1 u in Joules via E=mc^2

            // Theoretical Units
            { EnergyUnits.PlanckEnergy, 1.9561e9 }
        };

        public static EnergyUnits BaseScale => EnergyUnits.Joule;
        public (double Magnitude, EnergyUnits Scale, int ScaleOrdinal) Normalized => (_J, BaseScale, (int)BaseScale);

        public (double Magnitude, EnergyUnits Scale, int ScaleOrdinal) Original { get; private set; } = (0, BaseScale, (int)BaseScale);
        public (double Magnitude, EnergyUnits Scale, int ScaleOrdinal) Converted { get; private set; } = (0, BaseScale, (int)BaseScale);
        #endregion

        #region CONSTRUCTORS
        public Energy() => _J = 0;
        public Energy(Energy instance) => this = instance;
        public Energy(double magnitude)
        {
            Original = (magnitude, BaseScale, (int)BaseScale);
            _J = magnitude;
        }
        public Energy(double magnitude, EnergyUnits scale)
        {
            Original = (magnitude, scale, (int)scale);
            _J = Methods.Scale(magnitude, scale, Mapper) / Mapper[BaseScale];
        }
        #endregion

        #region METHODS
        public static Energy Initialize() => new();
        public static Energy Create(Energy instance) => new(instance);
        public static Energy Create(double magnitude) => new(magnitude);
        public static Energy Create(double magnitude, EnergyUnits scale) => new(magnitude, scale);

        public Energy Duplicate() => new(this);
        public bool Equals(Energy other) => _J == other._J;
        public bool IsValid() => Original.Magnitude >= 0;

        public Energy Convert(EnergyUnits toScale)
        {
            double mag = Methods.Scale(Original.Magnitude, Original.Scale, Mapper) / Mapper[toScale];
            Converted = (mag, toScale, (int)toScale);
            return this;
        }
        public double As(EnergyUnits scale) => Duplicate().Convert(scale).Converted.Magnitude;
        public Energy Normalize()
        {
            Original = (_J, BaseScale, (int)BaseScale);
            return this;
        }

        public UnitSquared<Energy, EnergyUnits> Squared() => this * this;
        public UnitCubed<Energy, EnergyUnits> Cubed() => this * this * this;
        public HyperUnit<Energy, EnergyUnits> Pow(int exp)
            => new HyperUnit<Energy, EnergyUnits>(Math.Pow(_J, exp)).SetDimension(exp);

        public ProductUnit<Force, ForceUnits, Length, LengthUnits> ToComposite()
            => new ProductUnit<Force, ForceUnits, Length, LengthUnits>(_J).SetScales(ForceUnits.Newton, LengthUnits.Meter);
        public static Energy FromComposite(ProductUnit<Force, ForceUnits, Length, LengthUnits> composite)
            => composite.Original.Scale1 == ForceUnits.Newton && composite.Original.Scale2 == LengthUnits.Meter ?
            new(composite.Original.Magnitude) :
            throw new InvalidOperationException(ErrorMsg.COMPOSITE_SCALES_MISMATCH);

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
        public static bool operator ==(Energy l, Energy r) => l.Equals(r);
        public static bool operator !=(Energy l, Energy r) => !l.Equals(r);
        public static bool operator <(Energy l, Energy r) => l.Normalized.Magnitude < r.Normalized.Magnitude;
        public static bool operator >(Energy l, Energy r) => l.Normalized.Magnitude > r.Normalized.Magnitude;
        public static bool operator <=(Energy l, Energy r) => l < r || l == r;
        public static bool operator >=(Energy l, Energy r) => l > r || l == r;
        #endregion

        #region SELF ARITHMETIC OPERATORS
        public static Energy operator +(Energy l, Energy r)
            => new(l.Normalized.Magnitude + r.Normalized.Magnitude);
        public static Energy operator -(Energy l, Energy r)
            => new(l.Normalized.Magnitude - r.Normalized.Magnitude);
        public static Energy operator *(Energy l, double r)
            => new(l.Normalized.Magnitude * r);
        public static Energy operator *(double l, Energy r) => r * l;
        public static Energy operator /(Energy l, double r)
            => new(l.Normalized.Magnitude / r);
        #endregion

        #region CROSS-DIMENSIONAL ARITHMETIC OPERATORS
        public static UnitSquared<Energy, EnergyUnits> operator *(Energy l, Energy r)
            => new(l.Normalized.Magnitude * r.Normalized.Magnitude);

        public static double operator /(Energy l, Energy r)
            => l.Normalized.Magnitude / r.Normalized.Magnitude;
        #endregion
    }

    /// <summary>
    /// Represents a one-dimensional rate of energy transfer or work done structure supporting scale conversions across mechanical, thermal, electrical, and theoretical domains, binary composite transposition, linear arithmetic, and higher-order dimensional exponentiation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="Power"/> implements foundational physical measurement contracts including <see cref="ILinearUnit{TSelf, TEnum}"/>, <see cref="INormalizable{TSelf}"/>, 
    /// <see cref="IBinaryCompositeTransposable{TSelf, TTransposed}"/>, <see cref="ICompositeUnit"/>, and <see cref="IExponentiable{TSquared, TCubed, THyper}"/> to manage power quantities represented by the standard symbol <see cref="SYMBOL"/> (P).
    /// </para>
    /// <para>
    /// It maintains a power dimension of 1 and normalizes values relative to the SI base unit (<see cref="PowerUnits.Watt"/>). 
    /// The structure provides scale conversion pipelines across SI prefixes (mW to TW), mechanical and electrical horsepower variants (mechanical, metric, electrical, boiler, air), HVAC and thermal rate units (BTU/h, ton of refrigeration, cal/s, kcal/h), imperial mechanical work rates (ft·lbf/s, ft·lbf/min), and theoretical physics extremes (Planck power, Hartree per atomic unit of time). 
    /// It supports bi-directional binary composite transposition with energy rate dynamics (P = dE / dt via <see cref="QuotientUnit{T1, E1, T2, E2}"/> mapping <see cref="Energy"/> over <see cref="Time"/>), relational comparison operators, linear arithmetic, and higher-order dimensional exponentiation (P², P³, and Pⁿ).
    /// </para>
    /// </remarks>
    public struct Power :
        IInitializable<Power>, IInitializable<Power, double>, IInitializable<Power, double, PowerUnits>,
        IScaleMappable<PowerUnits>, IScaleConvertible<Power, PowerUnits>,
        INormalized<PowerUnits>, INormalizable<Power>,
        IDimensionAccessible,
        IValueAccessible<PowerUnits>,
        IDuplicatable<Power>,
        IBinaryCompositeTransposable<Power, QuotientUnit<Energy, EnergyUnits, Time, TimeUnits>>,
        IExponentiable<Power, PowerUnits>,
        ILinearUnit<Power, PowerUnits>,
        ICompositeUnit
    {
        private readonly double _W;
        public const char SYMBOL = 'P';

        #region PROPERTIES
        public int Dimension => 1;
        public static Dictionary<PowerUnits, double> Mapper => new()
        {
            { PowerUnits.Watt, 1.0 },
            { PowerUnits.Milliwatt, 1e-3 },
            { PowerUnits.Kilowatt, 1e3 },
            { PowerUnits.Megawatt, 1e6 },
            { PowerUnits.Gigawatt, 1e9 },
            { PowerUnits.Terawatt, 1e12 },
            { PowerUnits.ErgPerSecond, 1e-7 },

            // Horsepower Variants
            { PowerUnits.Horsepower_Mechanical, 745.6998715822702 }, // 550 ft·lbf/s
            { PowerUnits.Horsepower_Metric, 735.49875 },            // 75 kgf·m/s
            { PowerUnits.Horsepower_Electrical, 746.0 },             // Exact US standard
            { PowerUnits.Horsepower_Boiler, 9809.5 },                // ~9.81 kW
            { PowerUnits.Horsepower_Air, 745.6998715822702 },        // Equivalent to mechanical output delivered to fluid

            // Imperial & US Customary Units
            { PowerUnits.FootPoundForcePerSecond, 1.3558179483314004 },
            { PowerUnits.FootPooundForcePerMinute, 0.02259696580552334 },
            { PowerUnits.BTU_PerHour, 0.2930710701722222 },
            { PowerUnits.TonRefrigeration, 3516.852842066667 },       // 12,000 BTU/h

            // Thermal & Caloric Rates
            { PowerUnits.CaloriePerSecond, 4.184 },
            { PowerUnits.KilocaloriePerHour, 1.1622222222222223 },

            // Theoretical & Quantum Units
            { PowerUnits.PlanckPower, 3.62831e52 },                   // c^5 / G (~3.62831 x 10^52 W)
            { PowerUnits.HartreePerAtomicUnitTime, 0.08462350917267 } // E_h / tau_0 (~8.4624 x 10^-2 W)
        };

        public static PowerUnits BaseScale => PowerUnits.Watt;
        public (double Magnitude, PowerUnits Scale, int ScaleOrdinal) Normalized => (_W, BaseScale, (int)BaseScale);

        public (double Magnitude, PowerUnits Scale, int ScaleOrdinal) Original { get; private set; } = (0, BaseScale, (int)BaseScale);
        public (double Magnitude, PowerUnits Scale, int ScaleOrdinal) Converted { get; private set; } = (0, BaseScale, (int)BaseScale);
        #endregion

        #region CONSTRUCTORS
        public Power() => _W = 0;
        public Power(Power instance) => this = instance;
        public Power(double magnitude)
        {
            Original = (magnitude, BaseScale, (int)BaseScale);
            _W = magnitude;
        }
        public Power(double magnitude, PowerUnits scale)
        {
            Original = (magnitude, scale, (int)scale);
            _W = Methods.Scale(magnitude, scale, Mapper) / Mapper[BaseScale];
        }
        #endregion

        #region METHODS
        public static Power Initialize() => new();
        public static Power Create(Power instance) => new(instance);
        public static Power Create(double magnitude) => new(magnitude);
        public static Power Create(double magnitude, PowerUnits scale) => new(magnitude, scale);

        public Power Duplicate() => new(this);
        public bool Equals(Power other) => _W == other._W;
        
        public Power Convert(PowerUnits toScale)
        {
            double mag = Methods.Scale(Original.Magnitude, Original.Scale, Mapper) / Mapper[toScale];
            Converted = (mag, toScale, (int)toScale);
            return this;
        }
        public double As(PowerUnits scale) => Duplicate().Convert(scale).Converted.Magnitude;
        public Power Normalize()
        {
            Original = (_W, BaseScale, (int)BaseScale);
            return this;
        }

        public UnitSquared<Power, PowerUnits> Squared() => this * this;
        public UnitCubed<Power, PowerUnits> Cubed() => this * this * this;
        public HyperUnit<Power, PowerUnits> Pow(int exp)
            => new HyperUnit<Power, PowerUnits>(Math.Pow(_W, exp)).SetDimension(exp);

        public QuotientUnit<Energy, EnergyUnits, Time, TimeUnits> ToComposite()
            => new QuotientUnit<Energy, EnergyUnits, Time, TimeUnits>(_W).SetScales(EnergyUnits.Joule, TimeUnits.Second);
        public static Power FromComposite(QuotientUnit<Energy, EnergyUnits, Time, TimeUnits> composite)
            => composite.Original.Scale1 == EnergyUnits.Joule && composite.Original.Scale2 == TimeUnits.Second ?
            new(composite.Original.Magnitude) :
            throw new InvalidOperationException(ErrorMsg.COMPOSITE_SCALES_MISMATCH);

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
        public static bool operator ==(Power l, Power r) => l.Equals(r);
        public static bool operator !=(Power l, Power r) => !l.Equals(r);
        public static bool operator <(Power l, Power r) => l.Normalized.Magnitude < r.Normalized.Magnitude;
        public static bool operator >(Power l, Power r) => l.Normalized.Magnitude > r.Normalized.Magnitude;
        public static bool operator <=(Power l, Power r) => l < r || l == r;
        public static bool operator >=(Power l, Power r) => l > r || l == r;
        #endregion

        #region SELF ARITHMETIC OPERATORS
        public static Power operator +(Power l, Power r)
            => new(l.Normalized.Magnitude + r.Normalized.Magnitude);
        public static Power operator -(Power l, Power r)
            => new(l.Normalized.Magnitude - r.Normalized.Magnitude);
        public static Power operator *(Power l, double r)
            => new(l.Normalized.Magnitude * r);
        public static Power operator *(double l, Power r) => r * l;
        public static Power operator /(Power l, double r)
            => new(l.Normalized.Magnitude / r);
        #endregion

        #region CROSS-DIMENSIONAL ARITHMETIC OPERATORS
        public static UnitSquared<Power, PowerUnits> operator *(Power l, Power r)
            => new(l.Normalized.Magnitude * r.Normalized.Magnitude);

        public static double operator /(Power l, Power r)
            => l.Normalized.Magnitude / r.Normalized.Magnitude;
        #endregion
    }

    /// <summary>
    /// Represents a one-dimensional fluid and mechanical pressure structure supporting multi-system scale conversions, binary composite transposition, linear arithmetic, and higher-order dimensional exponentiation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="Pressure"/> implements foundational physical measurement contracts including <see cref="ILinearUnit{TSelf, TEnum}"/>, <see cref="INormalizable{TSelf}"/>, 
    /// <see cref="IBinaryCompositeTransposable{TSelf, TTransposed}"/>, <see cref="ICompositeUnit"/>, and <see cref="IExponentiable{TSelf, TEnum}"/> to manage continuous mechanical and fluid pressure quantities represented by the standard symbol <see cref="SYMBOL"/> (P).
    /// </para>
    /// <para>
    /// It maintains a pressure dimension of 1 and normalizes values relative to the SI base unit (<see cref="PressureUnits.Pascal"/>). 
    /// The structure provides scale conversion pipelines across SI and metric prefixes (Pa to GPa, bar, mbar, barye), standard and technical atmospheres, manometric liquid head columns (Torr, mmHg, inHg, cmH₂O, mmH₂O, inH₂O, ftH₂O), imperial and customary stress scales (psi, psf, ksi, oz/in², pdl/ft², ton/in²), and theoretical extremes (Planck pressure, atomic pressure). 
    /// It supports bi-directional binary composite transposition with force distribution dynamics (P = F / A via <see cref="QuotientUnit{T1, E1, T2, E2}"/> mapping <see cref="Force"/> over <see cref="Area"/>), relational comparison operators, linear arithmetic, and higher-order dimensional exponentiation (P², P³, and Pⁿ).
    /// </para>
    /// </remarks>
    public struct Pressure :
        IInitializable<Pressure>, IInitializable<Pressure, double>, IInitializable<Pressure, double, PressureUnits>,
        IScaleMappable<PressureUnits>, IScaleConvertible<Pressure, PressureUnits>,
        INormalized<PressureUnits>, INormalizable<Pressure>,
        IDimensionAccessible,
        IValueAccessible<PressureUnits>,
        IDuplicatable<Pressure>,
        IBinaryCompositeTransposable<Pressure, QuotientUnit<Force, ForceUnits, Area, AreaUnits>>,
        IExponentiable<Pressure, PressureUnits>,
        ILinearUnit<Pressure, PressureUnits>,
        ICompositeUnit
    {
        private readonly double _Pa = 0;
        public const char SYMBOL = 'P';

        #region PROPERTIES
        public int Dimension => 1;
        public static Dictionary<PressureUnits, double> Mapper => new()
        {
            // SI & Metric System
            { PressureUnits.Pascal, 1.0 },
            { PressureUnits.Hectopascal, 1e2 },
            { PressureUnits.Kilopascal, 1e3 },
            { PressureUnits.Megapascal, 1e6 },
            { PressureUnits.Gigapascal, 1e9 },
            { PressureUnits.Bar, 1e5 },
            { PressureUnits.Millibar, 100.0 },
            { PressureUnits.Barye, 0.1 },

            // Atmospheric & Standard Units
            { PressureUnits.Atmosphere_Standard, 101325.0 },
            { PressureUnits.Atmosphere_Technical, 98066.5 }, // 1 kgf/cm²

            // Manometric Units (Liquid Head Column at Standard Conditions)
            { PressureUnits.Torr, 101325.0 / 760.0 },         // ~133.32236842105263 Pa
            { PressureUnits.Mercury_Millimeter, 133.322387415 }, // 1 mmHg at 0°C
            { PressureUnits.Mercury_Inch, 3386.38815789 },     // 1 inHg at 0°C
            { PressureUnits.Water_Centimeter, 98.0665 },        // 1 cmH₂O at 4°C
            { PressureUnits.Water_Millimeter, 9.80665 },        // 1 mmH₂O at 4°C
            { PressureUnits.Water_Inch, 249.088908333 },        // 1 inH₂O at 4°C
            { PressureUnits.Water_Foot, 2989.0669 },            // 1 ftH₂O at 4°C

            // Imperial & US Customary Units
            { PressureUnits.PoundPerSquareInch, 6894.757293168361 },
            { PressureUnits.PoundPerSquareFoot, 47.88025898033584 },
            { PressureUnits.KipPerSquareInch, 6894757.293168361 },  // 1,000 psi
            { PressureUnits.OuncePerSquareInch, 430.9223308230225 },
            { PressureUnits.PoundalPerSquareFoot, 1.4881639435698514 },
            { PressureUnits.LongTonPerSquareInch, 15444256.3072033 }, // 2,240 lbf/in²
            { PressureUnits.ShortTonPerSquareInch, 13789514.58633672 }, // 2,000 lbf/in²

            // Theoretical & Atomic Units
            { PressureUnits.PlanckPressure, 4.63309e113 },       // c^7 / (hbar * G^2) (~4.63309 x 10^113 Pa)
            { PressureUnits.AtomicPressure, 2.9421015696686e13 } // E_h / a_0^3 (~2.9421 x 10^13 Pa)
        };

        public static PressureUnits BaseScale => PressureUnits.Pascal;
        public (double Magnitude, PressureUnits Scale, int ScaleOrdinal) Normalized => (_Pa, BaseScale, (int)BaseScale);

        public (double Magnitude, PressureUnits Scale, int ScaleOrdinal) Original { get; private set; } = (0, BaseScale, (int)BaseScale);
        public (double Magnitude, PressureUnits Scale, int ScaleOrdinal) Converted { get; private set; } = (0, BaseScale, (int)BaseScale);
        #endregion

        #region CONSTRUCTORS
        public Pressure() => _Pa = 0;
        public Pressure(Pressure instance) => this = instance;
        public Pressure(double magnitude)
        {
            Original = (magnitude, BaseScale, (int)BaseScale);
            _Pa = magnitude;
        }
        public Pressure(double magnitude, PressureUnits scale)
        {
            Original = (magnitude, scale, (int)scale);
            _Pa = Methods.Scale(magnitude, scale, Mapper) / Mapper[BaseScale];
        }
        #endregion

        #region METHODS
        public static Pressure Initialize() => new();
        public static Pressure Create(Pressure instance) => new(instance);
        public static Pressure Create(double magnitude) => new(magnitude);
        public static Pressure Create(double magnitude, PressureUnits scale) => new(magnitude, scale);

        public Pressure Duplicate() => new(this);
        public bool Equals(Pressure other) => _Pa == other._Pa;

        public Pressure Convert(PressureUnits toScale)
        {
            double mag = Methods.Scale(Original.Magnitude, Original.Scale, Mapper) / Mapper[toScale];
            Converted = (mag, toScale, (int)toScale);
            return this;
        }
        public double As(PressureUnits scale) => Duplicate().Convert(scale).Converted.Magnitude;
        public Pressure Normalize()
        {
            Original = (_Pa, BaseScale, (int)BaseScale);
            return this;
        }

        public UnitSquared<Pressure, PressureUnits> Squared() => this * this;
        public UnitCubed<Pressure, PressureUnits> Cubed() => this * this * this;
        public HyperUnit<Pressure, PressureUnits> Pow(int exp)
            => new HyperUnit<Pressure, PressureUnits>(Math.Pow(_Pa, exp)).SetDimension(exp);

        public QuotientUnit<Force, ForceUnits, Area, AreaUnits> ToComposite()
            => new QuotientUnit<Force, ForceUnits, Area, AreaUnits>(_Pa).SetScales(ForceUnits.Newton, AreaUnits.SquareMeter);
        public static Pressure FromComposite(QuotientUnit<Force, ForceUnits, Area, AreaUnits> composite)
            => composite.Original.Scale1 == ForceUnits.Newton && composite.Original.Scale2 == AreaUnits.SquareMeter ?
            new(composite.Original.Magnitude) :
            throw new InvalidOperationException(ErrorMsg.COMPOSITE_SCALES_MISMATCH);

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
        public static bool operator ==(Pressure l, Pressure r) => l.Equals(r);
        public static bool operator !=(Pressure l, Pressure r) => !l.Equals(r);
        public static bool operator <(Pressure l, Pressure r) => l.Normalized.Magnitude < r.Normalized.Magnitude;
        public static bool operator >(Pressure l, Pressure r) => l.Normalized.Magnitude > r.Normalized.Magnitude;
        public static bool operator <=(Pressure l, Pressure r) => l < r || l == r;
        public static bool operator >=(Pressure l, Pressure r) => l > r || l == r;
        #endregion

        #region SELF ARITHMETIC OPERATORS
        public static Pressure operator +(Pressure l, Pressure r)
            => new(l.Normalized.Magnitude + r.Normalized.Magnitude);
        public static Pressure operator -(Pressure l, Pressure r)
            => new(l.Normalized.Magnitude - r.Normalized.Magnitude);
        public static Pressure operator *(Pressure l, double r)
            => new(l.Normalized.Magnitude * r);
        public static Pressure operator *(double l, Pressure r) => r * l;
        public static Pressure operator /(Pressure l, double r)
            => new(l.Normalized.Magnitude / r);
        #endregion

        #region CROSS-DIMENSIONAL ARITHMETIC OPERATORS
        public static UnitSquared<Pressure, PressureUnits> operator *(Pressure l, Pressure r)
            => new(l.Normalized.Magnitude * r.Normalized.Magnitude);

        public static double operator /(Pressure l, Pressure r)
            => l.Normalized.Magnitude / r.Normalized.Magnitude;
        #endregion

    }

    /// <summary>
    /// Represents a one-dimensional translational momentum and impulse physical measurement structure supporting multi-scale unit conversions, binary composite transposition, linear arithmetic, and higher-order dimensional exponentiation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="Momentum"/> implements foundational physical measurement contracts including <see cref="ILinearUnit{TSelf, TEnum}"/>, <see cref="INormalizable{TSelf}"/>, 
    /// <see cref="IBinaryCompositeTransposable{TSelf, TTransposed}"/>, <see cref="ICompositeUnit"/>, and <see cref="IExponentiable{TSelf, TEnum}"/> to manage linear momentum and mechanical impulse quantities represented by the standard symbol <see cref="SYMBOL"/> (p).
    /// </para>
    /// <para>
    /// It maintains a momentum dimension of 1 and normalizes values relative to the SI base unit (<see cref="MomentumUnits.NewtonPerSecond"/>). 
    /// The structure provides scale conversion pipelines across SI and metric systems (N·s, g·cm/s, dyn·s, t·m/s), imperial and customary units (lb·ft/s, lbf·s, pdl·s, slug·ft/s, oz·in/s), high-energy relativistic and astronomical scales (eV/c to TeV/c, M☉·AU/yr), and theoretical physics domains (Planck momentum, atomic momentum). 
    /// It supports bi-directional binary composite transposition with impulse dynamics (J = F * t via <see cref="ProductUnit{T1, E1, T2, E2}"/> mapping <see cref="Force"/> and <see cref="Time"/>), relational comparison operators, linear arithmetic, and higher-order dimensional exponentiation (p², p³, and pⁿ).
    /// </para>
    /// </remarks>
    public struct Momentum :
        IInitializable<Momentum>, IInitializable<Momentum, double>, IInitializable<Momentum, double, MomentumUnits>,
        IScaleMappable<MomentumUnits>, IScaleConvertible<Momentum, MomentumUnits>,
        INormalized<MomentumUnits>, INormalizable<Momentum>,
        IDimensionAccessible,
        IValueAccessible<MomentumUnits>,
        IDuplicatable<Momentum>,
        IBinaryCompositeTransposable<Momentum, ProductUnit<Force, ForceUnits, Time, TimeUnits>>,
        IExponentiable<Momentum, MomentumUnits>,
        ILinearUnit<Momentum, MomentumUnits>,
        ICompositeUnit
    {
        private readonly double _Nps;
        public const char SYMBOL = 'p';

        #region PROPERTIES
        public int Dimension => 1;
        public static Dictionary<MomentumUnits, double> Mapper => new()
        {
            // SI & Metric System
            { MomentumUnits.NewtonPerSecond, 1.0 },                   // Equivalent to 1 N·s or 1 kg·m/s
            { MomentumUnits.Gram_Centimeter_PerSecond, 1e-5 },         // 1 g·cm/s = 10^-5 N·s
            { MomentumUnits.DyneSecond, 1e-5 },                        // 1 dyn·s = 10^-5 N·s
            { MomentumUnits.Tonne_Meter_PerSecond, 1000.0 },           // 1 t·m/s = 1,000 N·s

            // Imperial & US Customary Units
            { MomentumUnits.Pound_Foot_PerSecond, 0.138254954376 },    // 1 lb·ft/s
            { MomentumUnits.Pound_ForceS_econd, 4.4482216152605 },     // 1 lbf·s
            { MomentumUnits.Poundal_Second, 0.138254954376 },          // 1 pdl·s
            { MomentumUnits.Slug_Foot_PerSecond, 4.4482216152605 },    // 1 slug·ft/s
            { MomentumUnits.Ounce_Inch_PerSecond, 0.0007199737207083 },// 1 oz·in/s

            // High-Energy Particle Physics & Astronomy
            { MomentumUnits.Electronvolt_Per_C, 5.34428599268e-28 },
            { MomentumUnits.KiloElectronvolt_Per_C, 5.34428599268e-25 },
            { MomentumUnits.MegaElectronvolt_Per_C, 5.34428599268e-22 },
            { MomentumUnits.GigaElectronvolt_Per_C, 5.34428599268e-19 },
            { MomentumUnits.TeraElectronvolt_Per_C, 5.34428599268e-16 },
            { MomentumUnits.SolarMass_AstronomicalUnit_PerYear, 9.4705503816e38 }, // M_sun * AU / yr

            // Theoretical & Atomic Units
            { MomentumUnits.PlanckMomentum, 6.52485 },                 // m_P * c (~6.52485 N·s)
            { MomentumUnits.AtomicMomentum, 1.99285191410e-24 }        // hbar / a_0 (~1.99285 x 10^-24 N·s)
        };

        public static MomentumUnits BaseScale => MomentumUnits.NewtonPerSecond;
        public (double Magnitude, MomentumUnits Scale, int ScaleOrdinal) Normalized => (_Nps, BaseScale, (int)BaseScale);

        public (double Magnitude, MomentumUnits Scale, int ScaleOrdinal) Original { get; private set; } = (0, BaseScale, (int)BaseScale);
        public (double Magnitude, MomentumUnits Scale, int ScaleOrdinal) Converted { get; private set; } = (0, BaseScale, (int)BaseScale);
        #endregion

        #region CONSTRUCTORS
        public Momentum() => _Nps = 0;
        public Momentum(Momentum instance) => this = instance;
        public Momentum(double magnitude)
        {
            Original = (magnitude, BaseScale, (int)BaseScale);
            _Nps = magnitude;
        }
        public Momentum(double magnitude, MomentumUnits scale)
        {
            Original = (magnitude, scale, (int)scale);
            _Nps = Methods.Scale(magnitude, scale, Mapper) / Mapper[BaseScale];
        }
        #endregion

        #region METHODS
        public static Momentum Initialize() => new();
        public static Momentum Create(Momentum instance) => new(instance);
        public static Momentum Create(double magnitude) => new(magnitude);
        public static Momentum Create(double magnitude, MomentumUnits scale) => new(magnitude, scale);

        public Momentum Duplicate() => new(this);
        public bool Equals(Momentum other) => _Nps == other._Nps;

        public Momentum Convert(MomentumUnits toScale)
        {
            double mag = Methods.Scale(Original.Magnitude, Original.Scale, Mapper) / Mapper[toScale];
            Converted = (mag, toScale, (int)toScale);
            return this;
        }
        public double As(MomentumUnits scale) => Duplicate().Convert(scale).Converted.Magnitude;
        public Momentum Normalize()
        {
            Original = (_Nps, BaseScale, (int)BaseScale);
            return this;
        }

        public UnitSquared<Momentum, MomentumUnits> Squared() => this * this;
        public UnitCubed<Momentum, MomentumUnits> Cubed() => this * this * this;
        public HyperUnit<Momentum, MomentumUnits> Pow(int exp)
            => new HyperUnit<Momentum, MomentumUnits>(Math.Pow(_Nps, exp)).SetDimension(exp);

        public ProductUnit<Force, ForceUnits, Time, TimeUnits> ToComposite()
            => new ProductUnit<Force, ForceUnits, Time, TimeUnits>(_Nps).SetScales(ForceUnits.Newton, TimeUnits.Second);
        public static Momentum FromComposite(ProductUnit<Force, ForceUnits, Time, TimeUnits> comp)
            => comp.Original.Scale1 == ForceUnits.Newton && comp.Original.Scale2 == TimeUnits.Second ?
            new(comp.Original.Magnitude) :
            throw new InvalidOperationException(ErrorMsg.COMPOSITE_SCALES_MISMATCH);

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
        public static bool operator ==(Momentum l, Momentum r) => l.Equals(r);
        public static bool operator !=(Momentum l, Momentum r) => !l.Equals(r);
        public static bool operator <(Momentum l, Momentum r) => l.Normalized.Magnitude < r.Normalized.Magnitude;
        public static bool operator >(Momentum l, Momentum r) => l.Normalized.Magnitude > r.Normalized.Magnitude;
        public static bool operator <=(Momentum l, Momentum r) => l < r || l == r;
        public static bool operator >=(Momentum l, Momentum r) => l > r || l == r;
        #endregion

        #region SELF ARITHMETIC OPERATORS
        public static Momentum operator +(Momentum l, Momentum r)
            => new(l.Normalized.Magnitude + r.Normalized.Magnitude);
        public static Momentum operator -(Momentum l, Momentum r)
            => new(l.Normalized.Magnitude - r.Normalized.Magnitude);
        public static Momentum operator *(Momentum l, double r)
            => new(l.Normalized.Magnitude * r);
        public static Momentum operator *(double l, Momentum r) => r * l;
        public static Momentum operator /(Momentum l, double r)
            => new(l.Normalized.Magnitude / r);
        #endregion

        #region CROSS-DIMENSIONAL ARITHMETIC OPERATORS
        public static UnitSquared<Momentum, MomentumUnits> operator *(Momentum l, Momentum r)
            => new(l.Normalized.Magnitude * r.Normalized.Magnitude);

        public static double operator /(Momentum l, Momentum r)
            => l.Normalized.Magnitude / r.Normalized.Magnitude;
        #endregion

    }
}
