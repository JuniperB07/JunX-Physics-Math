using JunX.Mathematics.Geometry;
using JunX.Physics.BaseUnits;
using JunX.Physics.ClassicalMechanics;
using JunX.Physics.Kinematics;
using JunX.Physics.Thermodynamics;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace JunX.Physics.FluidDynamics
{
    public struct DynamicViscosity :
        IInitializable<DynamicViscosity>, IInitializable<DynamicViscosity, double>, IInitializable<DynamicViscosity, double, DynamicViscosityUnits>,
        IScaleMappable<DynamicViscosityUnits>, IScaleConvertible<DynamicViscosity, DynamicViscosityUnits>,
        INormalized<DynamicViscosityUnits>, INormalizable<DynamicViscosity>,
        IDimensionAccessible,
        IValueAccessible<DynamicViscosityUnits>,
        IDuplicatable<DynamicViscosity>,
        IEquatable<DynamicViscosity>,
        IBinaryCompositeTransposable<DynamicViscosity, ProductUnit<Pressure, PressureUnits, Time, TimeUnits>>,
        IExponentiable<DynamicViscosity, DynamicViscosityUnits>,
        ILinearUnit<DynamicViscosity, DynamicViscosityUnits>,
        ICompositeUnit
    {
        private readonly double _Pas = 0;
        public const char SYMBOL = 'μ';

        #region PROPERTIES
        public int Dimension => 1;
        public static Dictionary<DynamicViscosityUnits, double> Mapper => new()
        {
            // SI & Metric System
            { DynamicViscosityUnits.PascalSecond, 1.0 },
            { DynamicViscosityUnits.MillipascalSecond, 1e-3 },             // 1 mPa·s = 1 cP = 10^-3 Pa·s
            { DynamicViscosityUnits.MicropascalSecond, 1e-6 },             // 10^-6 Pa·s
            { DynamicViscosityUnits.KilopascalSecond, 1e3 },              // 1000 Pa·s

            // CGS & Non-SI Metric System
            { DynamicViscosityUnits.Piose, 0.1 },                         // 1 P = 1 dyn·s/cm² = 0.1 Pa·s
            { DynamicViscosityUnits.Centipoise, 1e-3 },                   // 1 cP = 10^-2 P = 10^-3 Pa·s
            { DynamicViscosityUnits.Millipoise, 1e-4 },                   // 1 mP = 10^-3 P = 10^-4 Pa·s
            { DynamicViscosityUnits.Micropoise, 1e-7 },                   // 1 µP = 10^-6 P = 10^-7 Pa·s

            // Imperial & US Customary System
            { DynamicViscosityUnits.PoundForceSecond_PerSquareFoot, 47.88025898033584 }, // 1 lbf·s/ft² (~47.8803 Pa·s)
            { DynamicViscosityUnits.PoundForceSecond_PerSquareInch, 6894.757293168361 }, // 1 Reyn = 1 lbf·s/in² (~6894.76 Pa·s)
            { DynamicViscosityUnits.Pound_PerFootHour, 0.0004133788708333333 },         // 1 lb/(ft·h) (~0.00041338 Pa·s)
            { DynamicViscosityUnits.Pound_PerFootSecond, 1.48816394356918 },             // 1 lb/(ft·s) (~1.48816 Pa·s)
            { DynamicViscosityUnits.Slug_PerFootSecond, 47.88025898033584 },             // 1 slug/(ft·s) = 1 lbf·s/ft² (~47.8803 Pa·s)

            // Technical Metric Units
            { DynamicViscosityUnits.KilogramForceSecond_PerSquareMeter, 9.80665 },        // 1 kgf·s/m² = 9.80665 Pa·s
            { DynamicViscosityUnits.Gram_PerCentimeterSecond, 0.1 },                     // 1 g/(cm·s) = 1 P = 0.1 Pa·s

            // Atomic & Theoretical Units
            { DynamicViscosityUnits.AtomicDynamicViscosity, 704.5427387 },               // hbar / a_0^3 (~704.543 Pa·s)
            { DynamicViscosityUnits.PlanckDynamicViscosity, 3.034e118 }                  // c^4 / (hbar * G^2) (~3.034 x 10^118 Pa·s)
        };

        public static DynamicViscosityUnits BaseScale => DynamicViscosityUnits.PascalSecond;
        public (double Magnitude, DynamicViscosityUnits Scale, int ScaleOrdinal) Normalized => new(_Pas, BaseScale, (int)BaseScale);

        public (double Magnitude, DynamicViscosityUnits Scale, int ScaleOrdinal) Original { get; private set; } = (0, BaseScale, (int)BaseScale);
        public (double Magnitude, DynamicViscosityUnits Scale, int ScaleOrdinal) Converted { get; private set; } = (0, BaseScale, (int)BaseScale);
        #endregion

        #region CONSTRUCTORS
        public DynamicViscosity() { }
        public DynamicViscosity(DynamicViscosity instance) => this = instance;
        public DynamicViscosity(double magnitude)
        {
            Original = (magnitude, BaseScale, (int)BaseScale);
            _Pas = magnitude;
        }
        public DynamicViscosity(double magnitude, DynamicViscosityUnits scale)
        {
            Original = (magnitude, scale, (int)scale);
            _Pas = Methods.Scale(magnitude, scale, Mapper) / Mapper[BaseScale];
        }
        #endregion

        #region METHODS
        public static DynamicViscosity Initialize() => new();
        public static DynamicViscosity Create(DynamicViscosity instance) => new(instance);
        public static DynamicViscosity Create(double magnitude) => new(magnitude);
        public static DynamicViscosity Create(double magnitude, DynamicViscosityUnits scale) => new(magnitude, scale);

        public DynamicViscosity Duplicate() => new(this);
        public bool Equals(DynamicViscosity other) => _Pas == other._Pas;

        public DynamicViscosity Convert(DynamicViscosityUnits toScale)
        {
            double mag = Methods.Scale(Original.Magnitude, Original.Scale, Mapper) / Mapper[toScale];
            Converted = (mag, toScale, (int)toScale);
            return this;
        }
        public double As(DynamicViscosityUnits scale) => Duplicate().Convert(scale).Converted.Magnitude;
        public DynamicViscosity Normalize()
        {
            Original = (_Pas, BaseScale, (int)BaseScale);
            return this;
        }

        public ProductUnit<Pressure, PressureUnits, Time, TimeUnits> ToComposite()
            => new ProductUnit<Pressure, PressureUnits, Time, TimeUnits>(_Pas).SetScales(PressureUnits.Pascal, TimeUnits.Second);
        public static DynamicViscosity FromComposite(ProductUnit<Pressure, PressureUnits, Time, TimeUnits> comp)
            => comp.Original.Scale1 == PressureUnits.Pascal && comp.Original.Scale2 == TimeUnits.Second ?
            new(comp.Original.Magnitude) :
            throw new InvalidOperationException(ErrorMsg.COMPOSITE_SCALES_MISMATCH);

        public TernaryQuotientUnit<
            Denominator<CompositeProduct<Length, Time>>,
            Mass, MassUnits,
            Length, LengthUnits,
            Time, TimeUnits> ToKilogram_PerMeterSecond()
            => new TernaryQuotientUnit<Denominator<CompositeProduct<Length, Time>>, Mass, MassUnits, Length, LengthUnits, Time, TimeUnits>(_Pas)
            .SetScales(Mass.BaseScale, Length.BaseScale, Time.BaseScale);
        public static DynamicViscosity FromKilogram_PerMeterSecond(TernaryQuotientUnit<
            Denominator<CompositeProduct<Length, Time>>,
            Mass, MassUnits,
            Length, LengthUnits,
            Time, TimeUnits> comp)
            => comp.Scales.Scale1 == Mass.BaseScale && comp.Scales.Scale2 == Length.BaseScale && comp.Scales.Scale3 == Time.BaseScale ?
            new(comp.Magnitude) :
            throw new InvalidOperationException(ErrorMsg.COMPOSITE_SCALES_MISMATCH);

        public UnitSquared<DynamicViscosity, DynamicViscosityUnits> Squared() => this * this;
        public UnitCubed<DynamicViscosity, DynamicViscosityUnits> Cubed() => this * this * this;
        public HyperUnit<DynamicViscosity, DynamicViscosityUnits> Pow(int exp)
            => new HyperUnit<DynamicViscosity, DynamicViscosityUnits>(Math.Pow(_Pas, exp)).SetDimension(exp);

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
        public static bool operator ==(DynamicViscosity l, DynamicViscosity r) => l.Equals(r);
        public static bool operator !=(DynamicViscosity l, DynamicViscosity r) => !l.Equals(r);
        public static bool operator <(DynamicViscosity l, DynamicViscosity r) => l.Normalized.Magnitude < r.Normalized.Magnitude;
        public static bool operator >(DynamicViscosity l, DynamicViscosity r) => l.Normalized.Magnitude > r.Normalized.Magnitude;
        public static bool operator <=(DynamicViscosity l, DynamicViscosity r) => l < r || l == r;
        public static bool operator >=(DynamicViscosity l, DynamicViscosity r) => l > r || l == r;
        #endregion

        #region SELF ARITHMETIC OPERATORS
        public static DynamicViscosity operator +(DynamicViscosity l, DynamicViscosity r)
            => new(l.Normalized.Magnitude + r.Normalized.Magnitude);
        public static DynamicViscosity operator -(DynamicViscosity l, DynamicViscosity r)
            => new(l.Normalized.Magnitude - r.Normalized.Magnitude);
        public static DynamicViscosity operator *(DynamicViscosity l, double r)
            => new(l.Normalized.Magnitude * r);
        public static DynamicViscosity operator *(double l, DynamicViscosity r) => r * l;
        public static DynamicViscosity operator /(DynamicViscosity l, double r)
            => new(l.Normalized.Magnitude / r);
        #endregion

        #region CROSS-DIMENSIONAL ARITHMETIC OPERATORS
        public static UnitSquared<DynamicViscosity, DynamicViscosityUnits> operator *(DynamicViscosity l, DynamicViscosity r)
            => new(l.Normalized.Magnitude * r.Normalized.Magnitude);

        public static double operator /(DynamicViscosity l, DynamicViscosity r)
            => l.Normalized.Magnitude / r.Normalized.Magnitude;
        #endregion

        #region CROSS-UNIT OPERATORS
        public static MassFlowRate operator *(DynamicViscosity l, Length r) => new(l.Normalized.Magnitude * r.Normalized.Magnitude);
        public static MassFlowRate operator *(Length l, DynamicViscosity r) => r * l;
        #endregion
    }

    public struct KinematicViscosity :
        IInitializable<KinematicViscosity>, IInitializable<KinematicViscosity, double>, IInitializable<KinematicViscosity, double, KinematicViscosityUnits>,
        IScaleMappable<KinematicViscosityUnits>, IScaleConvertible<KinematicViscosity, KinematicViscosityUnits>,
        INormalized<KinematicViscosityUnits>, INormalizable<KinematicViscosity>,
        IDimensionAccessible,
        IValueAccessible<KinematicViscosityUnits>,
        IDuplicatable<KinematicViscosity>,
        IEquatable<KinematicViscosity>,
        IBinaryCompositeTransposable<KinematicViscosity, QuotientUnit<Area, AreaUnits, Time, TimeUnits>>,
        IExponentiable<KinematicViscosity, KinematicViscosityUnits>,
        ILinearUnit<KinematicViscosity, KinematicViscosityUnits>,
        ICompositeUnit
    {
        private readonly double _m2_s = 0;
        public const char SYMBOL = 'v';

        #region PROPERTIES
        public int Dimension => 1;
        public static Dictionary<KinematicViscosityUnits, double> Mapper => new()
        {
            // SI & Metric System
            { KinematicViscosityUnits.SquareMeter_PerSecond, 1.0 },
            { KinematicViscosityUnits.SquareMillimeter_PerSecond, 1e-6 }, // 1 mm²/s = 1 cSt = 10^-6 m²/s
            { KinematicViscosityUnits.SquareCentimeter_PerSecond, 1e-4 }, // 1 cm²/s = 1 St = 10^-4 m²/s

            // CGS & Non-SI Metric System
            { KinematicViscosityUnits.Stokes, 1e-4 },                     // 1 St = 1 cm²/s = 10^-4 m²/s
            { KinematicViscosityUnits.Centistokes, 1e-6 },                 // 1 cSt = 1 mm²/s = 10^-6 m²/s
            { KinematicViscosityUnits.Millistokes, 1e-7 },                 // 1 mSt = 10^-3 St = 10^-7 m²/s
            { KinematicViscosityUnits.Microstokes, 1e-10 },                // 1 µSt = 10^-6 St = 10^-10 m²/s

            // Imperial & US Customary System
            { KinematicViscosityUnits.SquareFoot_PerSecond, 0.09290304 },  // 1 ft²/s (exact: 0.3048² m²/s)
            { KinematicViscosityUnits.SquareFoot_PerHour, 2.58064e-5 },    // 1 ft²/h = 0.09290304 / 3600 m²/s
            { KinematicViscosityUnits.SquareInch_PerSecond, 0.00064516 },   // 1 in²/s (exact: 0.0254² m²/s)

            // Atomic & Theoretical Units
            { KinematicViscosityUnits.AtomicKinematicViscosity, 1.157675793e-4 }, // hbar / m_e (~1.15768 x 10^-4 m²/s)
            { KinematicViscosityUnits.PlanckKinematicViscosity, 4.850818e-35 }    // sqrt(hbar * G / c) (~4.85082 x 10^-35 m²/s)
        };

        public static KinematicViscosityUnits BaseScale => KinematicViscosityUnits.SquareMeter_PerSecond;
        public (double Magnitude, KinematicViscosityUnits Scale, int ScaleOrdinal) Normalized => new(_m2_s, BaseScale, (int)BaseScale);

        public (double Magnitude, KinematicViscosityUnits Scale, int ScaleOrdinal) Original { get; private set; } = (0, BaseScale, (int)BaseScale);
        public (double Magnitude, KinematicViscosityUnits Scale, int ScaleOrdinal) Converted { get; private set; } = (0, BaseScale, (int)BaseScale);
        #endregion

        #region CONSTRUCTORS
        public KinematicViscosity() { }
        public KinematicViscosity(KinematicViscosity instance) => this = instance;
        public KinematicViscosity(double magnitude)
        {
            Original = (magnitude, BaseScale, (int)BaseScale);
            _m2_s = magnitude;
        }
        public KinematicViscosity(double magnitude, KinematicViscosityUnits scale)
        {
            Original = (magnitude, scale, (int)scale);
            _m2_s = Methods.Scale(magnitude, scale, Mapper) / Mapper[BaseScale];
        }
        #endregion

        #region METHODS
        public static KinematicViscosity Initialize() => new();
        public static KinematicViscosity Create(KinematicViscosity instance) => new(instance);
        public static KinematicViscosity Create(double magnitude) => new(magnitude);
        public static KinematicViscosity Create(double magnitude, KinematicViscosityUnits scale) => new(magnitude, scale);

        public KinematicViscosity Duplicate() => new(this);
        public bool Equals(KinematicViscosity other) => _m2_s == other._m2_s;

        public KinematicViscosity Convert(KinematicViscosityUnits toScale)
        {
            double mag = Methods.Scale(Original.Magnitude, Original.Scale, Mapper) / Mapper[toScale];
            Converted = (mag, toScale, (int)toScale);
            return this;
        }
        public double As(KinematicViscosityUnits scale) => Duplicate().Convert(scale).Converted.Magnitude;
        public KinematicViscosity Normalize()
        {
            Original = (_m2_s, BaseScale, (int)BaseScale);
            return this;
        }

        public QuotientUnit<Area, AreaUnits, Time, TimeUnits> ToComposite()
            => new QuotientUnit<Area, AreaUnits, Time, TimeUnits>(_m2_s).SetScales(AreaUnits.SquareMeter, TimeUnits.Second);
        public static KinematicViscosity FromComposite(QuotientUnit<Area, AreaUnits, Time, TimeUnits> comp)
            => comp.Original.Scale1 == Area.BaseScale && comp.Original.Scale2 == Time.BaseScale ?
            new(comp.Original.Magnitude) :
            throw new InvalidOperationException(ErrorMsg.COMPOSITE_SCALES_MISMATCH);
        public QuotientUnit<UnitSquared<Length, LengthUnits>, LengthUnits, Time, TimeUnits> ToSquareMeter_PerSecond()
            => new QuotientUnit<UnitSquared<Length, LengthUnits>, LengthUnits, Time, TimeUnits>(_m2_s).SetScales(LengthUnits.Meter, TimeUnits.Second);
        public static KinematicViscosity FromSquareMeter_PerSecond(QuotientUnit<UnitSquared<Length, LengthUnits>, LengthUnits, Time, TimeUnits> comp)
            => comp.Original.Scale1 == Length.BaseScale && comp.Original.Scale2 == Time.BaseScale ?
            new(comp.Original.Magnitude) :
            throw new InvalidOperationException(ErrorMsg.COMPOSITE_SCALES_MISMATCH);

        public UnitSquared<KinematicViscosity, KinematicViscosityUnits> Squared() => this * this;
        public UnitCubed<KinematicViscosity, KinematicViscosityUnits> Cubed() => this * this * this;
        public HyperUnit<KinematicViscosity, KinematicViscosityUnits> Pow(int exp)
            => new HyperUnit<KinematicViscosity, KinematicViscosityUnits>(Math.Pow(_m2_s, exp)).SetDimension(exp);

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
        public static bool operator ==(KinematicViscosity l, KinematicViscosity r) => l.Equals(r);
        public static bool operator !=(KinematicViscosity l, KinematicViscosity r) => !l.Equals(r);
        public static bool operator <(KinematicViscosity l, KinematicViscosity r) => l.Normalized.Magnitude < r.Normalized.Magnitude;
        public static bool operator >(KinematicViscosity l, KinematicViscosity r) => l.Normalized.Magnitude > r.Normalized.Magnitude;
        public static bool operator <=(KinematicViscosity l, KinematicViscosity r) => l < r || l == r;
        public static bool operator >=(KinematicViscosity l, KinematicViscosity r) => l > r || l == r;
        #endregion

        #region SELF ARITHMETIC OPERATORS
        public static KinematicViscosity operator +(KinematicViscosity l, KinematicViscosity r)
            => new(l.Normalized.Magnitude + r.Normalized.Magnitude);
        public static KinematicViscosity operator -(KinematicViscosity l, KinematicViscosity r)
            => new(l.Normalized.Magnitude - r.Normalized.Magnitude);
        public static KinematicViscosity operator *(KinematicViscosity l, double r)
            => new(l.Normalized.Magnitude * r);
        public static KinematicViscosity operator *(double l, KinematicViscosity r) => r * l;
        public static KinematicViscosity operator /(KinematicViscosity l, double r)
            => new(l.Normalized.Magnitude / r);
        #endregion

        #region CROSS-DIMENSIONAL ARITHMETIC OPERATORS
        public static UnitSquared<KinematicViscosity, KinematicViscosityUnits> operator *(KinematicViscosity l, KinematicViscosity r)
            => new(l.Normalized.Magnitude * r.Normalized.Magnitude);

        public static double operator /(KinematicViscosity l, KinematicViscosity r)
            => l.Normalized.Magnitude / r.Normalized.Magnitude;
        #endregion
    }

    public struct Density :
        IInitializable<Density>, IInitializable<Density, double>, IInitializable<Density, double, DensityUnits>,
        IScaleMappable<DensityUnits>, IScaleConvertible<Density, DensityUnits>,
        INormalized<DensityUnits>, INormalizable<Density>,
        IDimensionAccessible,
        IValueAccessible<DensityUnits>,
        IDuplicatable<Density>,
        IEquatable<Density>,
        IBinaryCompositeTransposable<Density, QuotientUnit<Mass, MassUnits, Volume, VolumeUnits>>,
        IExponentiable<Density, DensityUnits>,
        ILinearUnit<Density, DensityUnits>,
        ICompositeUnit
    {
        private readonly double _kg_m3 = 0;
        public const char SYMBOL = 'ρ';

        #region PROPERTIES
        public int Dimension => 1;
        public static Dictionary<DensityUnits, double> Mapper => new()
        {
            // SI & Metric System
            { DensityUnits.Kilogram_PerCubicMeter, 1.0 },
            { DensityUnits.Gram_PerCubicCentimeter, 1000.0 },              // 1 g/cm³ = 1000 kg/m³
            { DensityUnits.Milligram_PerCubicMeter, 1e-6 },                // 1 mg/m³ = 10^-6 kg/m³
            { DensityUnits.Kilogram_PerLiter, 1000.0 },                    // 1 kg/L = 1000 kg/m³
            { DensityUnits.Gram_PerMilliliter, 1000.0 },                   // 1 g/mL = 1000 kg/m³
            { DensityUnits.Gram_PerLiter, 1.0 },                           // 1 g/L = 1 kg/m³
            { DensityUnits.MetricTon_PerCubicMeter, 1000.0 },              // 1 t/m³ = 1000 kg/m³
            { DensityUnits.Milligram_PerCubicCentimeter, 1.0 },            // 1 mg/cm³ = 1 kg/m³

            // Imperial & US Customary System
            { DensityUnits.Pound_PerCubicFoot, 16.01846337396014 },        // 1 lb/ft³ (~16.0185 kg/m³)
            { DensityUnits.Pound_PerCubicInch, 27679.90471020311 },        // 1 lb/in³ (~27679.9 kg/m³)
            { DensityUnits.Pound_PerGallon_US, 119.8264273168966 },        // 1 lb/gal (US) (~119.826 kg/m³)
            { DensityUnits.Pound_PerGallon_Imperial, 99.77637266310185 },  // 1 lb/gal (Imp) (~99.7764 kg/m³)
            { DensityUnits.Slug_PerCubicFoot, 515.3788184937803 },         // 1 slug/ft³ (~515.379 kg/m³)
            { DensityUnits.Ounce_PerCubicInch, 1729.994044387694 },        // 1 oz/in³ (~1729.99 kg/m³)
            { DensityUnits.Ounce_PerFluidOunce, 958.6114170366601 },       // 1 oz/fl oz (US) (~958.611 kg/m³)
            { DensityUnits.LongTon_PerCubicYard, 1328.939183606992 },      // 1 long ton/yd³ (~1328.94 kg/m³)
            { DensityUnits.ShordTon_PerCubicYard, 1186.552842506243 },     // 1 short ton/yd³ (~1186.55 kg/m³)

            // Atomic & Theoretical Units
            { DensityUnits.AtomicDensity, 6138.8115082 },                  // m_e / a_0^3 (~6138.81 kg/m³)
            { DensityUnits.PlanckDensity, 5.155e96 }                       // c^5 / (hbar * G^2) (~5.155 x 10^96 kg/m³)
        };

        public static DensityUnits BaseScale => DensityUnits.Kilogram_PerCubicMeter;
        public (double Magnitude, DensityUnits Scale, int ScaleOrdinal) Normalized => new(_kg_m3, BaseScale, (int)BaseScale);

        public (double Magnitude, DensityUnits Scale, int ScaleOrdinal) Original { get; private set; } = (0, BaseScale, (int)BaseScale);
        public (double Magnitude, DensityUnits Scale, int ScaleOrdinal) Converted { get; private set; } = (0, BaseScale, (int)BaseScale);
        #endregion

        #region CONSTRUCTORS
        public Density() { }
        public Density(Density instance) => this = instance;
        public Density(double magnitude)
        {
            Original = (magnitude, BaseScale, (int)BaseScale);
            _kg_m3 = magnitude;
        }
        public Density(double magnitude, DensityUnits scale)
        {
            Original = (magnitude, scale, (int)scale);
            _kg_m3 = Methods.Scale(magnitude, scale, Mapper) / Mapper[BaseScale];
        }
        #endregion

        #region METHODS
        public static Density Initialize() => new();
        public static Density Create(Density instance) => new(instance);
        public static Density Create(double magnitude) => new(magnitude);
        public static Density Create(double magnitude, DensityUnits scale) => new(magnitude, scale);

        public Density Duplicate() => new(this);
        public bool Equals(Density other) => _kg_m3 == other._kg_m3;

        public Density Convert(DensityUnits toScale)
        {
            double mag = Methods.Scale(Original.Magnitude, Original.Scale, Mapper) / Mapper[toScale];
            Converted = (mag, toScale, (int)toScale);
            return this;
        }
        public double As(DensityUnits scale) => Duplicate().Convert(scale).Converted.Magnitude;
        public Density Normalize()
        {
            Original = (_kg_m3, BaseScale, (int)BaseScale);
            return this;
        }

        public QuotientUnit<Mass, MassUnits, Volume, VolumeUnits> ToComposite()
            => new QuotientUnit<Mass, MassUnits, Volume, VolumeUnits>(_kg_m3).SetScales(Mass.BaseScale, Volume.BaseScale);
        public static Density FromComposite(QuotientUnit<Mass, MassUnits, Volume, VolumeUnits> comp)
            => comp.Original.Scale1 == Mass.BaseScale && comp.Original.Scale2 == Volume.BaseScale ?
            new(comp.Original.Magnitude) :
            throw new InvalidOperationException(ErrorMsg.COMPOSITE_SCALES_MISMATCH);

        public UnitSquared<Density, DensityUnits> Squared() => this * this;
        public UnitCubed<Density, DensityUnits> Cubed() => this * this * this;
        public HyperUnit<Density, DensityUnits> Pow(int exp)
            => new HyperUnit<Density, DensityUnits>(Math.Pow(_kg_m3, exp)).SetDimension(exp);

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
        public static bool operator ==(Density l, Density r) => l.Equals(r);
        public static bool operator !=(Density l, Density r) => !l.Equals(r);
        public static bool operator <(Density l, Density r) => l.Normalized.Magnitude < r.Normalized.Magnitude;
        public static bool operator >(Density l, Density r) => l.Normalized.Magnitude > r.Normalized.Magnitude;
        public static bool operator <=(Density l, Density r) => l < r || l == r;
        public static bool operator >=(Density l, Density r) => l > r || l == r;
        #endregion

        #region SELF ARITHMETIC OPERATORS
        public static Density operator +(Density l, Density r)
            => new(l.Normalized.Magnitude + r.Normalized.Magnitude);
        public static Density operator -(Density l, Density r)
            => new(l.Normalized.Magnitude - r.Normalized.Magnitude);
        public static Density operator *(Density l, double r)
            => new(l.Normalized.Magnitude * r);
        public static Density operator *(double l, Density r) => r * l;
        public static Density operator /(Density l, double r)
            => new(l.Normalized.Magnitude / r);
        #endregion

        #region CROSS-DIMENSIONAL ARITHMETIC OPERATORS
        public static UnitSquared<Density, DensityUnits> operator *(Density l, Density r)
            => new(l.Normalized.Magnitude * r.Normalized.Magnitude);

        public static double operator /(Density l, Density r)
            => l.Normalized.Magnitude / r.Normalized.Magnitude;
        #endregion

        #region CROSS-UNIT OPERATORS
        public static TernaryQuotientUnit<
            Denominator<CompositeProduct<Length, UnitSquared<Time, TimeUnits>>>,
            Mass, MassUnits,
            Length, LengthUnits,
            UnitSquared<Time, TimeUnits>, TimeUnits> operator *(Density l, UnitSquared<Velocity, VelocityUnits> r)
        {
            double mag = l.Normalized.Magnitude * r.Normalized.Magnitude;
            var unit = TernaryQuotientUnit<Denominator<CompositeProduct<Length, UnitSquared<Time, TimeUnits>>>, Mass, MassUnits, Length, LengthUnits, UnitSquared<Time, TimeUnits>, TimeUnits>.Create(mag);
            return unit.SetScales(Mass.BaseScale, Length.BaseScale, Time.BaseScale);
        }
        public static TernaryQuotientUnit<
            Denominator<CompositeProduct<Length, UnitSquared<Time, TimeUnits>>>,
            Mass, MassUnits,
            Length, LengthUnits,
            UnitSquared<Time, TimeUnits>, TimeUnits> operator *(UnitSquared<Velocity, VelocityUnits> l, Density r)
            => r * l;

        #endregion
    }

    public struct SpecificWeight :
        IInitializable<SpecificWeight>, IInitializable<SpecificWeight, double>, IInitializable<SpecificWeight, double, SpecificWeightUnits>,
        IScaleMappable<SpecificWeightUnits>, IScaleConvertible<SpecificWeight, SpecificWeightUnits>,
        INormalized<SpecificWeightUnits>, INormalizable<SpecificWeight>,
        IDimensionAccessible,
        IValueAccessible<SpecificWeightUnits>,
        IDuplicatable<SpecificWeight>,
        IEquatable<SpecificWeight>,
        IBinaryCompositeTransposable<SpecificWeight, QuotientUnit<Force, ForceUnits, Volume, VolumeUnits>>,
        IExponentiable<SpecificWeight, SpecificWeightUnits>,
        ILinearUnit<SpecificWeight, SpecificWeightUnits>,
        ICompositeUnit
    {
        private readonly double _N_m3 = 0;
        public const char SYMBOL = 'γ';

        #region PROPERTIES
        public int Dimension => 1;
        public static Dictionary<SpecificWeightUnits, double> Mapper => new()
        {
            // SI & Metric System
            { SpecificWeightUnits.Newton_PerCubicMeter, 1.0 },
            { SpecificWeightUnits.Kilonewton_PerCubicMeter, 1e3 },                  // 1 kN/m³ = 1000 N/m³
            { SpecificWeightUnits.Meganewton_PerCubicMeter, 1e6 },                  // 1 MN/m³ = 10^6 N/m³
            { SpecificWeightUnits.Millinewton_PerCubicMeter, 1e-3 },                 // 1 mN/m³ = 10^-3 N/m³

            // CGS & Metric Technical System
            { SpecificWeightUnits.Dyne_PerCubicCentimeter, 10.0 },                  // 1 dyn/cm³ = 10 N/m³
            { SpecificWeightUnits.KilogramForce_PerCubicMeter, 9.80665 },            // 1 kgf/m³ = 9.80665 N/m³
            { SpecificWeightUnits.GramForce_PerCubicCentimeter, 9806.65 },           // 1 gf/cm³ = 9806.65 N/m³
            { SpecificWeightUnits.KilogramForce_PerLiter, 9806.65 },                 // 1 kgf/L = 9806.65 N/m³

            // Imperial & US Customary System
            { SpecificWeightUnits.PoundForce_PerCubicFoot, 157.0874638462462 },      // 1 lbf/ft³ (pcf) (~157.087 N/m³)
            { SpecificWeightUnits.PoundForce_PerCubicInch, 271447.1375663134 },      // 1 lbf/in³ (pci) (~271447 N/m³)
            { SpecificWeightUnits.PoundForce_PerGallon, 1175.126835222004 },        // 1 lbf/gal (US) (~1175.13 N/m³)
            { SpecificWeightUnits.OunceForce_perCubicInch, 16965.44609789459 },      // 1 ozf/in³ (~16965.4 N/m³)
            { SpecificWeightUnits.OunceForce_PerCubicFoot, 9.817966490390388 },      // 1 ozf/ft³ (~9.81797 N/m³)
            { SpecificWeightUnits.Kip_PerCubicFoot, 157087.4638462462 },             // 1 kip/ft³ (kcf) (~157087 N/m³)
            { SpecificWeightUnits.LongTonForce_PerCubicYard, 13032.48981604117 },    // 1 long tonf/yd³ (~13032.5 N/m³)
            { SpecificWeightUnits.ShortTonForce_PerCubicYard, 11636.15162146533 },   // 1 short tonf/yd³ (~11636.2 N/m³)

            // Atomic & Theoretical Units
            { SpecificWeightUnits.AtomicSpecificWeight, 60200.56363 },               // (m_e * g_0) / a_0^3 (~60200.6 N/m³)
            { SpecificWeightUnits.PlanckSpecificWeight, 2.8974e148 }                  // c^8 / (hbar^(3/2) * G^(5/2)) (~2.8974 x 10^148 N/m³)
        };

        public static SpecificWeightUnits BaseScale => SpecificWeightUnits.Newton_PerCubicMeter;
        public (double Magnitude, SpecificWeightUnits Scale, int ScaleOrdinal) Normalized => new(_N_m3, BaseScale, (int)BaseScale);

        public (double Magnitude, SpecificWeightUnits Scale, int ScaleOrdinal) Original { get; private set; } = (0, BaseScale, (int)BaseScale);
        public (double Magnitude, SpecificWeightUnits Scale, int ScaleOrdinal) Converted { get; private set; } = (0, BaseScale, (int)BaseScale);
        #endregion

        #region CONSTRUCTORS
        public SpecificWeight() { }
        public SpecificWeight(SpecificWeight instance) => this = instance;
        public SpecificWeight(double magnitude)
        {
            Original = (magnitude, BaseScale, (int)BaseScale);
            _N_m3 = magnitude;
        }
        public SpecificWeight(double magnitude, SpecificWeightUnits scale)
        {
            Original = (magnitude, scale, (int)scale);
            _N_m3 = Methods.Scale(magnitude, scale, Mapper) / Mapper[BaseScale];
        }
        #endregion

        #region METHODS
        public static SpecificWeight Initialize() => new();
        public static SpecificWeight Create(SpecificWeight instance) => new(instance);
        public static SpecificWeight Create(double magnitude) => new(magnitude);
        public static SpecificWeight Create(double magnitude, SpecificWeightUnits scale) => new(magnitude, scale);

        public SpecificWeight Duplicate() => new(this);
        public bool Equals(SpecificWeight other) => _N_m3 == other._N_m3;

        public SpecificWeight Convert(SpecificWeightUnits toScale)
        {
            double mag = Methods.Scale(Original.Magnitude, Original.Scale, Mapper) / Mapper[toScale];
            Converted = (mag, toScale, (int)toScale);
            return this;
        }
        public double As(SpecificWeightUnits scale) => Duplicate().Convert(scale).Converted.Magnitude;
        public SpecificWeight Normalize()
        {
            Original = (_N_m3, BaseScale, (int)BaseScale);
            return this;
        }

        public QuotientUnit<Force, ForceUnits, Volume, VolumeUnits> ToComposite()
            => new QuotientUnit<Force, ForceUnits, Volume, VolumeUnits>(_N_m3).SetScales(Force.BaseScale, Volume.BaseScale);
        public static SpecificWeight FromComposite(QuotientUnit<Force, ForceUnits, Volume, VolumeUnits> comp)
            => comp.Original.Scale1 == Force.BaseScale && comp.Original.Scale2 == Volume.BaseScale ?
            new(comp.Original.Magnitude) :
            throw new InvalidOperationException(ErrorMsg.COMPOSITE_SCALES_MISMATCH);

        public UnitSquared<SpecificWeight, SpecificWeightUnits> Squared() => this * this;
        public UnitCubed<SpecificWeight, SpecificWeightUnits> Cubed() => this * this * this;
        public HyperUnit<SpecificWeight, SpecificWeightUnits> Pow(int exp)
            => new HyperUnit<SpecificWeight, SpecificWeightUnits>(Math.Pow(_N_m3, exp)).SetDimension(exp);

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
        public static bool operator ==(SpecificWeight l, SpecificWeight r) => l.Equals(r);
        public static bool operator !=(SpecificWeight l, SpecificWeight r) => !l.Equals(r);
        public static bool operator <(SpecificWeight l, SpecificWeight r) => l.Normalized.Magnitude < r.Normalized.Magnitude;
        public static bool operator >(SpecificWeight l, SpecificWeight r) => l.Normalized.Magnitude > r.Normalized.Magnitude;
        public static bool operator <=(SpecificWeight l, SpecificWeight r) => l < r || l == r;
        public static bool operator >=(SpecificWeight l, SpecificWeight r) => l > r || l == r;
        #endregion

        #region SELF ARITHMETIC OPERATORS
        public static SpecificWeight operator +(SpecificWeight l, SpecificWeight r)
            => new(l.Normalized.Magnitude + r.Normalized.Magnitude);
        public static SpecificWeight operator -(SpecificWeight l, SpecificWeight r)
            => new(l.Normalized.Magnitude - r.Normalized.Magnitude);
        public static SpecificWeight operator *(SpecificWeight l, double r)
            => new(l.Normalized.Magnitude * r);
        public static SpecificWeight operator *(double l, SpecificWeight r) => r * l;
        public static SpecificWeight operator /(SpecificWeight l, double r)
            => new(l.Normalized.Magnitude / r);
        #endregion

        #region CROSS-DIMENSIONAL ARITHMETIC OPERATORS
        public static UnitSquared<SpecificWeight, SpecificWeightUnits> operator *(SpecificWeight l, SpecificWeight r)
            => new(l.Normalized.Magnitude * r.Normalized.Magnitude);

        public static double operator /(SpecificWeight l, SpecificWeight r)
            => l.Normalized.Magnitude / r.Normalized.Magnitude;
        #endregion
    }

    public struct VolumetricFlowRate :
        IInitializable<VolumetricFlowRate>, IInitializable<VolumetricFlowRate, double>, IInitializable<VolumetricFlowRate, double, VolumetricFlowRateUnits>,
        IScaleMappable<VolumetricFlowRateUnits>, IScaleConvertible<VolumetricFlowRate, VolumetricFlowRateUnits>,
        INormalized<VolumetricFlowRateUnits>, INormalizable<VolumetricFlowRate>,
        IDimensionAccessible,
        IValueAccessible<VolumetricFlowRateUnits>,
        IDuplicatable<VolumetricFlowRate>,
        IEquatable<VolumetricFlowRate>,
        IBinaryCompositeTransposable<VolumetricFlowRate, QuotientUnit<Volume, VolumeUnits, Time, TimeUnits>>,
        IExponentiable<VolumetricFlowRate, VolumetricFlowRateUnits>,
        ILinearUnit<VolumetricFlowRate, VolumetricFlowRateUnits>,
        ICompositeUnit
    {
        private readonly double _m3_s;
        public const char SYMBOL = 'Q';

        #region PROPERTIES
        public int Dimension => 1;
        public static Dictionary<VolumetricFlowRateUnits, double> Mapper => new()
        {
            // SI & Metric System
            { VolumetricFlowRateUnits.CubicMeter_PerSecond, 1.0 },
            { VolumetricFlowRateUnits.CubicMeter_PerHour, 0.0002777777777777778 },  // 1 / 3600 m³/s
            { VolumetricFlowRateUnits.Liter_PerSecond, 1e-3 },                     // 1 L/s = 0.001 m³/s
            { VolumetricFlowRateUnits.Liter_PerMinute, 1.6666666666666667e-5 },    // 1 / 60,000 m³/s
            { VolumetricFlowRateUnits.Liter_PerHour, 2.7777777777777778e-7 },       // 1 / 3,600,000 m³/s
            { VolumetricFlowRateUnits.Milliliter_PerSecond, 1e-6 },                // 1 mL/s = 10^-6 m³/s
            { VolumetricFlowRateUnits.Milliliter_PerMinute, 1.6666666666666667e-8 },// 1 / 60,000,000 m³/s

            // Imperial & US Customary System
            { VolumetricFlowRateUnits.CubicFoot_PerSecond, 0.028316846592 },       // 1 cfs (exact: 0.3048³ m³/s)
            { VolumetricFlowRateUnits.CubicFoot_PerMinute, 0.0004719474432 },      // 1 CFM (0.028316846592 / 60 m³/s)
            { VolumetricFlowRateUnits.CubicInch_PerSecond, 1.6387064e-5 },        // 1 in³/s (exact: 0.0254³ m³/s)
            { VolumetricFlowRateUnits.Gallon_PerMinute_US, 6.30901964e-5 },       // 1 GPM (US) (~6.30902 x 10^-5 m³/s)
            { VolumetricFlowRateUnits.Gallon_PerHour, 1.051503273e-6 },           // 1 GPH (US) (~1.05150 x 10^-6 m³/s)
            { VolumetricFlowRateUnits.Gallon_PerDay, 4.381263638888889e-8 },       // 1 GPD (US) (~4.38126 x 10^-8 m³/s)
            { VolumetricFlowRateUnits.Gallon_PerMinute_Imperial, 7.57682e-5 },    // 1 Imp GPM (~7.57682 x 10^-5 m³/s)
            { VolumetricFlowRateUnits.MillionGallons_PerDay, 0.04381263638888889 },// 1 MGD (~0.0438126 m³/s)

            // Specialized Industry Units
            { VolumetricFlowRateUnits.Barrel_PerDay, 1.8401307283333333e-6 },      // 42 US gal / day (~1.84013 x 10^-6 m³/s)
            { VolumetricFlowRateUnits.AcreFoot_PerDay, 0.014276410156944444 },     // 43,560 ft³ / day (~0.0142764 m³/s)
            { VolumetricFlowRateUnits.MinersInch, 0.0007079211648 },               // CA/AZ Standard: 1/40 cfs (~0.0007079 m³/s)

            // Atomic & Theoretical Units
            { VolumetricFlowRateUnits.AtomicVolumetricFlowRate, 6.126043e-15 },    // a_0^3 * E_h / hbar (~6.12604 x 10^-15 m³/s)
            { VolumetricFlowRateUnits.PlanckVolumetricFlowRate, 2.42542e-51 }      // c * l_P^2 (~2.42542 x 10^-51 m³/s)
        };

        public static VolumetricFlowRateUnits BaseScale => VolumetricFlowRateUnits.CubicMeter_PerSecond;
        public (double Magnitude, VolumetricFlowRateUnits Scale, int ScaleOrdinal) Normalized => new(_m3_s, BaseScale, (int)BaseScale);

        public (double Magnitude, VolumetricFlowRateUnits Scale, int ScaleOrdinal) Original { get; private set; } = (0, BaseScale, (int)BaseScale);
        public (double Magnitude, VolumetricFlowRateUnits Scale, int ScaleOrdinal) Converted { get; private set; } = (0, BaseScale, (int)BaseScale);
        #endregion

        #region CONSTRUCTORS
        public VolumetricFlowRate() { }
        public VolumetricFlowRate(VolumetricFlowRate instance) => this = instance;
        public VolumetricFlowRate(double magnitude)
        {
            Original = (magnitude, BaseScale, (int)BaseScale);
            _m3_s = magnitude;
        }
        public VolumetricFlowRate(double magnitude, VolumetricFlowRateUnits scale)
        {
            Original = (magnitude, scale, (int)scale);
            _m3_s = Methods.Scale(magnitude, scale, Mapper) / Mapper[BaseScale];
        }
        #endregion

        #region METHODS
        public static VolumetricFlowRate Initialize() => new();
        public static VolumetricFlowRate Create(VolumetricFlowRate instance) => new(instance);
        public static VolumetricFlowRate Create(double magnitude) => new(magnitude);
        public static VolumetricFlowRate Create(double magnitude, VolumetricFlowRateUnits scale) => new(magnitude, scale);

        public VolumetricFlowRate Duplicate() => new(this);
        public bool Equals(VolumetricFlowRate other) => _m3_s == other._m3_s;

        public VolumetricFlowRate Convert(VolumetricFlowRateUnits toScale)
        {
            double mag = Methods.Scale(Original.Magnitude, Original.Scale, Mapper) / Mapper[toScale];
            Converted = (mag, toScale, (int)toScale);
            return this;
        }
        public double As(VolumetricFlowRateUnits scale) => Duplicate().Convert(scale).Converted.Magnitude;
        public VolumetricFlowRate Normalize()
        {
            Original = (_m3_s, BaseScale, (int)BaseScale);
            return this;
        }

        public QuotientUnit<Volume, VolumeUnits, Time, TimeUnits> ToComposite()
            => new QuotientUnit<Volume, VolumeUnits, Time, TimeUnits>(_m3_s).SetScales(Volume.BaseScale, Time.BaseScale);
        public static VolumetricFlowRate FromComposite(QuotientUnit<Volume, VolumeUnits, Time, TimeUnits> comp)
            => comp.Original.Scale1 == Volume.BaseScale && comp.Original.Scale2 == Time.BaseScale ?
            new(comp.Original.Magnitude) :
            throw new InvalidOperationException(ErrorMsg.COMPOSITE_SCALES_MISMATCH);

        public UnitSquared<VolumetricFlowRate, VolumetricFlowRateUnits> Squared() => this * this;
        public UnitCubed<VolumetricFlowRate, VolumetricFlowRateUnits> Cubed() => this * this * this;
        public HyperUnit<VolumetricFlowRate, VolumetricFlowRateUnits> Pow(int exp)
            => new HyperUnit<VolumetricFlowRate, VolumetricFlowRateUnits>(Math.Pow(_m3_s, exp)).SetDimension(exp);

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
        public static bool operator ==(VolumetricFlowRate l, VolumetricFlowRate r) => l.Equals(r);
        public static bool operator !=(VolumetricFlowRate l, VolumetricFlowRate r) => !l.Equals(r);
        public static bool operator <(VolumetricFlowRate l, VolumetricFlowRate r) => l.Normalized.Magnitude < r.Normalized.Magnitude;
        public static bool operator >(VolumetricFlowRate l, VolumetricFlowRate r) => l.Normalized.Magnitude > r.Normalized.Magnitude;
        public static bool operator <=(VolumetricFlowRate l, VolumetricFlowRate r) => l < r || l == r;
        public static bool operator >=(VolumetricFlowRate l, VolumetricFlowRate r) => l > r || l == r;
        #endregion

        #region SELF ARITHMETIC OPERATORS
        public static VolumetricFlowRate operator +(VolumetricFlowRate l, VolumetricFlowRate r)
            => new(l.Normalized.Magnitude + r.Normalized.Magnitude);
        public static VolumetricFlowRate operator -(VolumetricFlowRate l, VolumetricFlowRate r)
            => new(l.Normalized.Magnitude - r.Normalized.Magnitude);
        public static VolumetricFlowRate operator *(VolumetricFlowRate l, double r)
            => new(l.Normalized.Magnitude * r);
        public static VolumetricFlowRate operator *(double l, VolumetricFlowRate r) => r * l;
        public static VolumetricFlowRate operator /(VolumetricFlowRate l, double r)
            => new(l.Normalized.Magnitude / r);
        #endregion

        #region CROSS-DIMENSIONAL ARITHMETIC OPERATORS
        public static UnitSquared<VolumetricFlowRate, VolumetricFlowRateUnits> operator *(VolumetricFlowRate l, VolumetricFlowRate r)
            => new(l.Normalized.Magnitude * r.Normalized.Magnitude);

        public static double operator /(VolumetricFlowRate l, VolumetricFlowRate r)
            => l.Normalized.Magnitude / r.Normalized.Magnitude;
        #endregion
    }

    public struct MassFlowRate :
        IInitializable<MassFlowRate>, IInitializable<MassFlowRate, double>, IInitializable<MassFlowRate, double, MassFlowRateUnits>,
        IScaleMappable<MassFlowRateUnits>, IScaleConvertible<MassFlowRate, MassFlowRateUnits>,
        INormalized<MassFlowRateUnits>, INormalizable<MassFlowRate>,
        IDimensionAccessible,
        IValueAccessible<MassFlowRateUnits>,
        IDuplicatable<MassFlowRate>,
        IEquatable<MassFlowRate>,
        IBinaryCompositeTransposable<MassFlowRate, QuotientUnit<Mass, MassUnits, Time, TimeUnits>>,
        IExponentiable<MassFlowRate, MassFlowRateUnits>,
        ILinearUnit<MassFlowRate, MassFlowRateUnits>,
        ICompositeUnit
    {
        private readonly double _kg_s = 0;
        public const char SYMBOL = 'ṁ';

        #region PROPERTIES
        public int Dimension => 1;
        public static Dictionary<MassFlowRateUnits, double> Mapper => new()
        {
            // SI & Metric System
            { MassFlowRateUnits.Kilogram_PerSecond, 1.0 },
            { MassFlowRateUnits.Kilogram_PerHour, 0.0002777777777777778 },  // 1 / 3600 kg/s
            { MassFlowRateUnits.Kilogram_PerMinute, 0.016666666666666666 }, // 1 / 60 kg/s
            { MassFlowRateUnits.Gram_PerSecond, 1e-3 },                     // 1 g/s = 0.001 kg/s
            { MassFlowRateUnits.Gram_PerMinute, 1.6666666666666667e-5 },    // 1 / 60,000 kg/s
            { MassFlowRateUnits.Gram_PerHour, 2.7777777777777778e-7 },       // 1 / 3,600,000 kg/s
            { MassFlowRateUnits.Milligram_PerSecond, 1e-6 },                // 1 mg/s = 10^-6 kg/s
            { MassFlowRateUnits.MetricTon_PerHour, 0.2777777777777778 },    // 1000 / 3600 kg/s
            { MassFlowRateUnits.MetricTon_PerDay, 0.011574074074074073 },   // 1000 / 86400 kg/s

            // Imperial & US Customary System
            { MassFlowRateUnits.Pound_PerSecond, 0.45359237 },              // Exact conversion factor (lb to kg)
            { MassFlowRateUnits.Pound_PerMinute, 0.007559872833333333 },    // 0.45359237 / 60 kg/s
            { MassFlowRateUnits.Pound_PerHour, 0.00012599788055555556 },    // 0.45359237 / 3600 kg/s
            { MassFlowRateUnits.Ounce_PerSecond, 0.028349523125 },          // 0.45359237 / 16 kg/s
            { MassFlowRateUnits.Ounce_PerMinute, 0.0004724920520833333 },   // 0.028349523125 / 60 kg/s
            { MassFlowRateUnits.Slug_PerMinute, 0.24323171808333333 },      // 14.593902935 / 60 kg/s
            { MassFlowRateUnits.LongTon_PerHour, 0.28223528055555555 },     // 1016.0469088 / 3600 kg/s
            { MassFlowRateUnits.ShortTon_PerHour, 0.2519957611111111 },     // 907.18474 / 3600 kg/s
            { MassFlowRateUnits.ShortTon_PerDay, 0.01049982337962963 },     // 907.18474 / 86400 kg/s

            // Atomic & Theoretical Units
            { MassFlowRateUnits.AtomicMassFlowRate, 3.7534509e-14 },        // m_e * E_h / hbar (~3.75345 x 10^-14 kg/s)
            { MassFlowRateUnits.PlanckMassFlowRate, 4.0371e35 }             // c^3 / G (~4.0371 x 10^35 kg/s)
        };

        public static MassFlowRateUnits BaseScale => MassFlowRateUnits.Kilogram_PerSecond;
        public (double Magnitude, MassFlowRateUnits Scale, int ScaleOrdinal) Normalized => new(_kg_s, BaseScale, (int)BaseScale);

        public (double Magnitude, MassFlowRateUnits Scale, int ScaleOrdinal) Original { get; private set; } = (0, BaseScale, (int)BaseScale);
        public (double Magnitude, MassFlowRateUnits Scale, int ScaleOrdinal) Converted { get; private set; } = (0, BaseScale, (int)BaseScale);
        #endregion

        #region CONSTRUCTORS
        public MassFlowRate() { }
        public MassFlowRate(MassFlowRate instance) => this = instance;
        public MassFlowRate(double magnitude)
        {
            Original = (magnitude, BaseScale, (int)BaseScale);
            _kg_s = magnitude;
        }
        public MassFlowRate(double magnitude, MassFlowRateUnits scale)
        {
            Original = (magnitude, scale, (int)scale);
            _kg_s = Methods.Scale(magnitude, scale, Mapper) / Mapper[BaseScale];
        }
        #endregion

        #region METHODS
        public static MassFlowRate Initialize() => new();
        public static MassFlowRate Create(MassFlowRate instance) => new(instance);
        public static MassFlowRate Create(double magnitude) => new(magnitude);
        public static MassFlowRate Create(double magnitude, MassFlowRateUnits scale) => new(magnitude, scale);

        public MassFlowRate Duplicate() => new(this);
        public bool Equals(MassFlowRate other) => _kg_s == other._kg_s;

        public MassFlowRate Convert(MassFlowRateUnits toScale)
        {
            double mag = Methods.Scale(Original.Magnitude, Original.Scale, Mapper) / Mapper[toScale];
            Converted = (mag, toScale, (int)toScale);
            return this;
        }
        public double As(MassFlowRateUnits scale) => Duplicate().Convert(scale).Converted.Magnitude;
        public MassFlowRate Normalize()
        {
            Original = (_kg_s, BaseScale, (int)BaseScale);
            return this;
        }

        public QuotientUnit<Mass, MassUnits, Time, TimeUnits> ToComposite()
            => new QuotientUnit<Mass, MassUnits, Time, TimeUnits>(_kg_s).SetScales(Mass.BaseScale, Time.BaseScale);
        public static MassFlowRate FromComposite(QuotientUnit<Mass, MassUnits, Time, TimeUnits> comp)
            => comp.Original.Scale1 == Mass.BaseScale && comp.Original.Scale2 == Time.BaseScale ?
            new(comp.Original.Magnitude) :
            throw new InvalidOperationException(ErrorMsg.COMPOSITE_SCALES_MISMATCH);

        public UnitSquared<MassFlowRate, MassFlowRateUnits> Squared() => this * this;
        public UnitCubed<MassFlowRate, MassFlowRateUnits> Cubed() => this * this * this;
        public HyperUnit<MassFlowRate, MassFlowRateUnits> Pow(int exp)
            => new HyperUnit<MassFlowRate, MassFlowRateUnits>(Math.Pow(_kg_s, exp)).SetDimension(exp);

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
        public static bool operator ==(MassFlowRate l, MassFlowRate r) => l.Equals(r);
        public static bool operator !=(MassFlowRate l, MassFlowRate r) => !l.Equals(r);
        public static bool operator <(MassFlowRate l, MassFlowRate r) => l.Normalized.Magnitude < r.Normalized.Magnitude;
        public static bool operator >(MassFlowRate l, MassFlowRate r) => l.Normalized.Magnitude > r.Normalized.Magnitude;
        public static bool operator <=(MassFlowRate l, MassFlowRate r) => l < r || l == r;
        public static bool operator >=(MassFlowRate l, MassFlowRate r) => l > r || l == r;
        #endregion

        #region SELF ARITHMETIC OPERATORS
        public static MassFlowRate operator +(MassFlowRate l, MassFlowRate r)
            => new(l.Normalized.Magnitude + r.Normalized.Magnitude);
        public static MassFlowRate operator -(MassFlowRate l, MassFlowRate r)
            => new(l.Normalized.Magnitude - r.Normalized.Magnitude);
        public static MassFlowRate operator *(MassFlowRate l, double r)
            => new(l.Normalized.Magnitude * r);
        public static MassFlowRate operator *(double l, MassFlowRate r) => r * l;
        public static MassFlowRate operator /(MassFlowRate l, double r)
            => new(l.Normalized.Magnitude / r);
        #endregion

        #region CROSS-DIMENSIONAL ARITHMETIC OPERATORS
        public static UnitSquared<MassFlowRate, MassFlowRateUnits> operator *(MassFlowRate l, MassFlowRate r)
            => new(l.Normalized.Magnitude * r.Normalized.Magnitude);

        public static double operator /(MassFlowRate l, MassFlowRate r)
            => l.Normalized.Magnitude / r.Normalized.Magnitude;
        #endregion

        #region CROSS-UNIT OPERATORS
        public static Force operator *(MassFlowRate l, Velocity r) => new(l.Normalized.Magnitude * r.Normalized.Magnitude);
        public static Force operator *(Velocity l, MassFlowRate r) => r * l;
        #endregion
    }

    public struct SurfaceTension :
        IInitializable<SurfaceTension>, IInitializable<SurfaceTension, double>, IInitializable<SurfaceTension, double, SurfaceTensionUnits>,
        IScaleMappable<SurfaceTensionUnits>, IScaleConvertible<SurfaceTension, SurfaceTensionUnits>,
        INormalized<SurfaceTensionUnits>, INormalizable<SurfaceTension>,
        IDimensionAccessible,
        IValueAccessible<SurfaceTensionUnits>,
        IDuplicatable<SurfaceTension>,
        IEquatable<SurfaceTension>,
        IBinaryCompositeTransposable<SurfaceTension, QuotientUnit<Force, ForceUnits, Length, LengthUnits>>,
        IExponentiable<SurfaceTension, SurfaceTensionUnits>,
        ILinearUnit<SurfaceTension, SurfaceTensionUnits>,
        ICompositeUnit
    {
        private readonly double _N_m = 0;
        public const char SYMBOL = 'σ';

        #region PROPERTIES
        public int Dimension => 1;
        public static Dictionary<SurfaceTensionUnits, double> Mapper => new()
        {
            // SI & Metric System
            { SurfaceTensionUnits.Newton_PerMeter, 1.0 },
            { SurfaceTensionUnits.Millinewton_PerMeter, 1e-3 },             // 1 mN/m = 10^-3 N/m
            { SurfaceTensionUnits.Joule_PerSquareMeter, 1.0 },              // 1 J/m² = 1 N/m
            { SurfaceTensionUnits.Millijoule_PerSquareMeter, 1e-3 },         // 1 mJ/m² = 10^-3 N/m
            { SurfaceTensionUnits.Micronewton_PerMeter, 1e-6 },             // 1 µN/m = 10^-6 N/m

            // CGS & Metric Technical System
            { SurfaceTensionUnits.Dyne_PerCentimeter, 1e-3 },               // 1 dyn/cm = 10^-3 N/m
            { SurfaceTensionUnits.Erg_PerSquareCentimeter, 1e-3 },          // 1 erg/cm² = 10^-3 N/m
            { SurfaceTensionUnits.KilogramForce_PerMeter, 9.80665 },        // 1 kgf/m = 9.80665 N/m
            { SurfaceTensionUnits.GramForce_PerCentimeter, 0.980665 },      // 1 gf/cm = 0.980665 N/m

            // Imperial & US Customary System
            { SurfaceTensionUnits.PoundForce_PerInch, 175.1268352464764 },  // 1 lbf/in (~175.127 N/m)
            { SurfaceTensionUnits.PoundForce_PerFoot, 14.59390293720637 },  // 1 lbf/ft (~14.5939 N/m)
            { SurfaceTensionUnits.OunceForce_PerInch, 10.94542720290477 },  // 1 ozf/in (~10.9454 N/m)
            { SurfaceTensionUnits.Poundal_PerInch, 5.443108441312384 },     // 1 pdl/in (~5.44311 N/m)

            // Atomic & Theoretical Units
            { SurfaceTensionUnits.AtomicSurfaceTension, 1556.893112 },      // E_h / a_0^2 (~1556.89 N/m)
            { SurfaceTensionUnits.PlanckSurfaceTension, 3.03403e83 }        // c^5 / (hbar * G) (~3.03403 x 10^83 N/m)
        };

        public static SurfaceTensionUnits BaseScale => SurfaceTensionUnits.Newton_PerMeter;
        public (double Magnitude, SurfaceTensionUnits Scale, int ScaleOrdinal) Normalized => new(_N_m, BaseScale, (int)BaseScale);

        public (double Magnitude, SurfaceTensionUnits Scale, int ScaleOrdinal) Original { get; private set; } = (0, BaseScale, (int)BaseScale);
        public (double Magnitude, SurfaceTensionUnits Scale, int ScaleOrdinal) Converted { get; private set; } = (0, BaseScale, (int)BaseScale);
        #endregion

        #region CONSTRUCTORS
        public SurfaceTension() { }
        public SurfaceTension(SurfaceTension instance) => this = instance;
        public SurfaceTension(double magnitude)
        {
            Original = (magnitude, BaseScale, (int)BaseScale);
            _N_m = magnitude;
        }
        public SurfaceTension(double magnitude, SurfaceTensionUnits scale)
        {
            Original = (magnitude, scale, (int)scale);
            _N_m = Methods.Scale(magnitude, scale, Mapper) / Mapper[BaseScale];
        }
        #endregion

        #region METHODS
        public static SurfaceTension Initialize() => new();
        public static SurfaceTension Create(SurfaceTension instance) => new(instance);
        public static SurfaceTension Create(double magnitude) => new(magnitude);
        public static SurfaceTension Create(double magnitude, SurfaceTensionUnits scale) => new(magnitude, scale);

        public SurfaceTension Duplicate() => new(this);
        public bool Equals(SurfaceTension other) => _N_m == other._N_m;

        public SurfaceTension Convert(SurfaceTensionUnits toScale)
        {
            double mag = Methods.Scale(Original.Magnitude, Original.Scale, Mapper) / Mapper[toScale];
            Converted = (mag, toScale, (int)toScale);
            return this;
        }
        public double As(SurfaceTensionUnits scale) => Duplicate().Convert(scale).Converted.Magnitude;
        public SurfaceTension Normalize()
        {
            Original = (_N_m, BaseScale, (int)BaseScale);
            return this;
        }

        public QuotientUnit<Force, ForceUnits, Length, LengthUnits> ToComposite()
            => new QuotientUnit<Force, ForceUnits, Length, LengthUnits>(_N_m).SetScales(Force.BaseScale, Length.BaseScale);
        public static SurfaceTension FromComposite(QuotientUnit<Force, ForceUnits, Length, LengthUnits> comp)
            => comp.Original.Scale1 == Force.BaseScale && comp.Original.Scale2 == Length.BaseScale ?
            new(comp.Original.Magnitude) :
            throw new InvalidOperationException(ErrorMsg.COMPOSITE_SCALES_MISMATCH);

        public UnitSquared<SurfaceTension, SurfaceTensionUnits> Squared() => this * this;
        public UnitCubed<SurfaceTension, SurfaceTensionUnits> Cubed() => this * this * this;
        public HyperUnit<SurfaceTension, SurfaceTensionUnits> Pow(int exp)
            => new HyperUnit<SurfaceTension, SurfaceTensionUnits>(Math.Pow(_N_m, exp)).SetDimension(exp);

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
        public static bool operator ==(SurfaceTension l, SurfaceTension r) => l.Equals(r);
        public static bool operator !=(SurfaceTension l, SurfaceTension r) => !l.Equals(r);
        public static bool operator <(SurfaceTension l, SurfaceTension r) => l.Normalized.Magnitude < r.Normalized.Magnitude;
        public static bool operator >(SurfaceTension l, SurfaceTension r) => l.Normalized.Magnitude > r.Normalized.Magnitude;
        public static bool operator <=(SurfaceTension l, SurfaceTension r) => l < r || l == r;
        public static bool operator >=(SurfaceTension l, SurfaceTension r) => l > r || l == r;
        #endregion

        #region SELF ARITHMETIC OPERATORS
        public static SurfaceTension operator +(SurfaceTension l, SurfaceTension r)
            => new(l.Normalized.Magnitude + r.Normalized.Magnitude);
        public static SurfaceTension operator -(SurfaceTension l, SurfaceTension r)
            => new(l.Normalized.Magnitude - r.Normalized.Magnitude);
        public static SurfaceTension operator *(SurfaceTension l, double r)
            => new(l.Normalized.Magnitude * r);
        public static SurfaceTension operator *(double l, SurfaceTension r) => r * l;
        public static SurfaceTension operator /(SurfaceTension l, double r)
            => new(l.Normalized.Magnitude / r);
        #endregion

        #region CROSS-DIMENSIONAL ARITHMETIC OPERATORS
        public static UnitSquared<SurfaceTension, SurfaceTensionUnits> operator *(SurfaceTension l, SurfaceTension r)
            => new(l.Normalized.Magnitude * r.Normalized.Magnitude);

        public static double operator /(SurfaceTension l, SurfaceTension r)
            => l.Normalized.Magnitude / r.Normalized.Magnitude;
        #endregion
    }
}
