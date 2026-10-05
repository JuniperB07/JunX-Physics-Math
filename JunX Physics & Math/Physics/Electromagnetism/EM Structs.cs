using JunX.Mathematics.Geometry;
using JunX.Physics.BaseUnits;
using JunX.Physics.ClassicalMechanics;
using JunX.Physics.Kinematics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace JunX.Physics.Electromagnetism
{
    public struct ElectricCharge :
        IInitializable<ElectricCharge>, IInitializable<ElectricCharge, double>, IInitializable<ElectricCharge, double, ElectricChargeUnits>,
        IScaleMappable<ElectricChargeUnits>, IScaleConvertible<ElectricCharge, ElectricChargeUnits>,
        INormalized<ElectricChargeUnits>, INormalizable<ElectricCharge>,
        IDimensionAccessible,
        IValueAccessible<ElectricChargeUnits>,
        IDuplicatable<ElectricCharge>,
        IEquatable<ElectricCharge>,
        IBinaryCompositeTransposable<ElectricCharge, ProductUnit<Current, CurrentUnits, Time, TimeUnits>>,
        IExponentiable<ElectricCharge, ElectricChargeUnits>,
        ILinearUnit<ElectricCharge, ElectricChargeUnits>,
        ICompositeUnit
    {
        private readonly double _C = 0;
        public const char SYMBOL = 'q';

        #region PROPERTIES
        public int Dimension => 1;
        public static Dictionary<ElectricChargeUnits, double> Mapper => new()
        {
            // SI & Metric System
            { ElectricChargeUnits.Coulomb, 1.0 },
            { ElectricChargeUnits.Millicoulomb, 1e-3 },
            { ElectricChargeUnits.Microcoulomb, 1e-6 },
            { ElectricChargeUnits.Nanocoulomb, 1e-9 },
            { ElectricChargeUnits.Picocoulomb, 1e-12 },
            { ElectricChargeUnits.Kilocoulomb, 1e3 },

            // CGS & Electromagnetic Units
            { ElectricChargeUnits.Statcoulomb, 3.33564095198152e-10 }, // 1 statC = 1 esu ≈ 1 / (10 * c) C
            { ElectricChargeUnits.Abcoulomb, 10.0 },                   // 1 abC = 1 emu = 10 C

            // Practical Energy & Battery Capacity Units
            { ElectricChargeUnits.Ampere_Hour, 3600.0 },              // 1 Ah = 3600 C
            { ElectricChargeUnits.Milliampere_Hour, 3.6 },            // 1 mAh = 3.6 C
            { ElectricChargeUnits.Ampere_Second, 1.0 },               // 1 A·s = 1 C
            { ElectricChargeUnits.Ampere_Minute, 60.0 },              // 1 A·min = 60 C

            // Chemical, Atomic & Theoretical Units
            { ElectricChargeUnits.ElementaryCharge, 1.602176634e-19 },// Exact SI defining constant e
            { ElectricChargeUnits.Faraday, 96485.33212331001 },        // N_A * e
            { ElectricChargeUnits.AtomicCharge, 1.602176634e-19 },    // Identical to elementary charge e
            { ElectricChargeUnits.PlanckCharge, 1.87554603778e-18 }   // sqrt(4 * pi * eps_0 * hbar * c)
        };

        public static ElectricChargeUnits BaseScale => ElectricChargeUnits.Coulomb;
        public (double Magnitude, ElectricChargeUnits Scale, int ScaleOrdinal) Normalized => new(_C, BaseScale, (int)BaseScale);

        public (double Magnitude, ElectricChargeUnits Scale, int ScaleOrdinal) Original { get; private set; } = (0, BaseScale, (int)BaseScale);
        public (double Magnitude, ElectricChargeUnits Scale, int ScaleOrdinal) Converted { get; private set; } = (0, BaseScale, (int)BaseScale);
        #endregion

        #region CONSTRUCTORS
        public ElectricCharge() { }
        public ElectricCharge(ElectricCharge instance) => this = instance;
        public ElectricCharge(double magnitude)
        {
            Original = (magnitude, BaseScale, (int)BaseScale);
            _C = magnitude;
        }
        public ElectricCharge(double magnitude, ElectricChargeUnits scale)
        {
            Original = (magnitude, scale, (int)scale);
            _C = Methods.Scale(magnitude, scale, Mapper) / Mapper[BaseScale];
        }
        #endregion

        #region METHODS
        public static ElectricCharge Initialize() => new();
        public static ElectricCharge Create(ElectricCharge instance) => new(instance);
        public static ElectricCharge Create(double magnitude) => new(magnitude);
        public static ElectricCharge Create(double magnitude, ElectricChargeUnits scale) => new(magnitude, scale);

        public ElectricCharge Duplicate() => new(this);
        public bool Equals(ElectricCharge other) => _C == other._C;

        public ElectricCharge Convert(ElectricChargeUnits toScale)
        {
            double mag = Methods.Scale(Original.Magnitude, Original.Scale, Mapper) / Mapper[toScale];
            Converted = (mag, toScale, (int)toScale);
            return this;
        }
        public double As(ElectricChargeUnits scale) => Duplicate().Convert(scale).Converted.Magnitude;
        public ElectricCharge Normalize()
        {
            Original = (_C, BaseScale, (int)BaseScale);
            return this;
        }

        public ProductUnit<Current, CurrentUnits, Time, TimeUnits> ToComposite()
            => new ProductUnit<Current, CurrentUnits, Time, TimeUnits>(_C).SetScales(CurrentUnits.Ampere, TimeUnits.Second);
        public static ElectricCharge FromComposite(ProductUnit<Current, CurrentUnits, Time, TimeUnits> comp)
            => comp.Original.Scale1 == CurrentUnits.Ampere && comp.Original.Scale2 == TimeUnits.Second ?
            new(comp.Original.Magnitude) :
            throw new InvalidOperationException(ErrorMsg.COMPOSITE_SCALES_MISMATCH);

        public UnitSquared<ElectricCharge, ElectricChargeUnits> Squared() => this * this;
        public UnitCubed<ElectricCharge, ElectricChargeUnits> Cubed() => this * this * this;
        public HyperUnit<ElectricCharge, ElectricChargeUnits> Pow(int exp)
            => new HyperUnit<ElectricCharge, ElectricChargeUnits>(Math.Pow(_C, exp)).SetDimension(exp);

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
        public static bool operator ==(ElectricCharge l, ElectricCharge r) => l.Equals(r);
        public static bool operator !=(ElectricCharge l, ElectricCharge r) => !l.Equals(r);
        public static bool operator <(ElectricCharge l, ElectricCharge r) => l.Normalized.Magnitude < r.Normalized.Magnitude;
        public static bool operator >(ElectricCharge l, ElectricCharge r) => l.Normalized.Magnitude > r.Normalized.Magnitude;
        public static bool operator <=(ElectricCharge l, ElectricCharge r) => l < r || l == r;
        public static bool operator >=(ElectricCharge l, ElectricCharge r) => l > r || l == r;
        #endregion

        #region SELF ARITHMETIC OPERATORS
        public static ElectricCharge operator +(ElectricCharge l, ElectricCharge r)
            => new(l.Normalized.Magnitude + r.Normalized.Magnitude);
        public static ElectricCharge operator -(ElectricCharge l, ElectricCharge r)
            => new(l.Normalized.Magnitude - r.Normalized.Magnitude);
        public static ElectricCharge operator *(ElectricCharge l, double r)
            => new(l.Normalized.Magnitude * r);
        public static ElectricCharge operator *(double l, ElectricCharge r) => r * l;
        public static ElectricCharge operator /(ElectricCharge l, double r)
            => new(l.Normalized.Magnitude / r);
        #endregion

        #region CROSS-DIMENSIONAL ARITHMETIC OPERATORS
        public static UnitSquared<ElectricCharge, ElectricChargeUnits> operator *(ElectricCharge l, ElectricCharge r)
            => new(l.Normalized.Magnitude * r.Normalized.Magnitude);

        public static double operator /(ElectricCharge l, ElectricCharge r)
            => l.Normalized.Magnitude / r.Normalized.Magnitude;
        #endregion

        #region CROSS-UNIT OPERATORS
        public static ProductUnit<ElectricCharge, ElectricChargeUnits, ElectricPotential, ElectricPotentialUnits> operator *(ElectricCharge l, ElectricPotential r)
            => new ProductUnit<ElectricCharge, ElectricChargeUnits, ElectricPotential, ElectricPotentialUnits>(l.Normalized.Magnitude * r.Normalized.Magnitude).SetScales(BaseScale, ElectricPotential.BaseScale);
        #endregion

    }

    public struct ElectricPotential :
        IInitializable<ElectricPotential>, IInitializable<ElectricPotential, double>, IInitializable<ElectricPotential, double, ElectricPotentialUnits>,
        IScaleMappable<ElectricPotentialUnits>, IScaleConvertible<ElectricPotential, ElectricPotentialUnits>,
        INormalized<ElectricPotentialUnits>, INormalizable<ElectricPotential>,
        IDimensionAccessible,
        IValueAccessible<ElectricPotentialUnits>,
        IDuplicatable<ElectricPotential>,
        IEquatable<ElectricPotential>,
        IBinaryCompositeTransposable<ElectricPotential, QuotientUnit<Energy, EnergyUnits, ElectricCharge, ElectricChargeUnits>>,
        IExponentiable<ElectricPotential, ElectricPotentialUnits>,
        ILinearUnit<ElectricPotential, ElectricPotentialUnits>,
        ICompositeUnit
    {
        private readonly double _V = 0;
        public static char SYMBOL = 'V';

        #region PROPERTIES
        public int Dimension => 1;
        public static Dictionary<ElectricPotentialUnits, double> Mapper => new()
        {
            // SI & Metric System
            { ElectricPotentialUnits.Volt, 1.0 },
            { ElectricPotentialUnits.Microvolt, 1e-6 },
            { ElectricPotentialUnits.Millivolt, 1e-3 },
            { ElectricPotentialUnits.Kilovolt, 1e3 },
            { ElectricPotentialUnits.Megavolt, 1e6 },
            { ElectricPotentialUnits.Gigavolt, 1e9 },
            { ElectricPotentialUnits.Teravolt, 1e12 },

            // CGS & Electromagnetic Units
            { ElectricPotentialUnits.Statvolt, 299.792458 },           // Exact value based on speed of light c / 10^6
            { ElectricPotentialUnits.Abvolt, 1e-8 },                    // 1 abV = 10^-8 V

            // Atomic & Theoretical Units
            { ElectricPotentialUnits.AtomicPotential, 27.211386245988 },// E_h / e (Hartree energy per elementary charge)
            { ElectricPotentialUnits.PlanckVoltage, 1.04295e27 }        // sqrt(c^5 * hbar / (G * q_P^2)) (~1.04295 x 10^27 V)
        };

        public static ElectricPotentialUnits BaseScale => ElectricPotentialUnits.Volt;
        public (double Magnitude, ElectricPotentialUnits Scale, int ScaleOrdinal) Normalized => new(_V, BaseScale, (int)BaseScale);

        public (double Magnitude, ElectricPotentialUnits Scale, int ScaleOrdinal) Original { get; private set; } = (0, BaseScale, (int)BaseScale);
        public (double Magnitude, ElectricPotentialUnits Scale, int ScaleOrdinal) Converted { get; private set; } = (0, BaseScale, (int)BaseScale);
        #endregion

        #region CONSTRUCTORS
        public ElectricPotential() { }
        public ElectricPotential(ElectricPotential instance) => this = instance;
        public ElectricPotential(double magnitude)
        {
            Original = (magnitude, BaseScale, (int)BaseScale);
            _V = magnitude;
        }
        public ElectricPotential(double magnitude, ElectricPotentialUnits scale)
        {
            Original = (magnitude, scale, (int)scale);
            _V = Methods.Scale(magnitude, scale, Mapper) / Mapper[BaseScale];
        }
        #endregion

        #region METHODS
        public static ElectricPotential Initialize() => new();
        public static ElectricPotential Create(ElectricPotential instance) => new(instance);
        public static ElectricPotential Create(double magnitude) => new(magnitude);
        public static ElectricPotential Create(double magnitude, ElectricPotentialUnits scale) => new(magnitude, scale);

        public ElectricPotential Duplicate() => new(this);
        public bool Equals(ElectricPotential other) => _V == other._V;

        public ElectricPotential Convert(ElectricPotentialUnits toScale)
        {
            double mag = Methods.Scale(Original.Magnitude, Original.Scale, Mapper) / Mapper[toScale];
            Converted = (mag, toScale, (int)toScale);
            return this;
        }
        public double As(ElectricPotentialUnits scale) => Duplicate().Convert(scale).Converted.Magnitude;
        public ElectricPotential Normalize()
        {
            Original = (_V, BaseScale, (int)BaseScale);
            return this;
        }

        public QuotientUnit<Energy, EnergyUnits, ElectricCharge, ElectricChargeUnits> ToComposite()
            => new QuotientUnit<Energy, EnergyUnits, ElectricCharge, ElectricChargeUnits>(_V).SetScales(EnergyUnits.Joule, ElectricChargeUnits.Coulomb);
        public static ElectricPotential FromComposite(QuotientUnit<Energy, EnergyUnits, ElectricCharge, ElectricChargeUnits> comp)
            => comp.Original.Scale1 == EnergyUnits.Joule && comp.Original.Scale2 == ElectricChargeUnits.Coulomb ?
            new(comp.Original.Magnitude) :
            throw new InvalidOperationException(ErrorMsg.COMPOSITE_SCALES_MISMATCH);
        public QuotientUnit<Power, PowerUnits, Current, CurrentUnits> ToWattPerAmpere()
            => new QuotientUnit<Power, PowerUnits, Current, CurrentUnits>(_V).SetScales(PowerUnits.Watt, CurrentUnits.Ampere);

        public UnitSquared<ElectricPotential, ElectricPotentialUnits> Squared() => this * this;
        public UnitCubed<ElectricPotential, ElectricPotentialUnits> Cubed() => this * this * this;
        public HyperUnit<ElectricPotential, ElectricPotentialUnits> Pow(int exp)
            => new HyperUnit<ElectricPotential, ElectricPotentialUnits>(Math.Pow(_V, exp)).SetDimension(exp);

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
        public static bool operator ==(ElectricPotential l, ElectricPotential r) => l.Equals(r);
        public static bool operator !=(ElectricPotential l, ElectricPotential r) => !l.Equals(r);
        public static bool operator <(ElectricPotential l, ElectricPotential r) => l.Normalized.Magnitude < r.Normalized.Magnitude;
        public static bool operator >(ElectricPotential l, ElectricPotential r) => l.Normalized.Magnitude > r.Normalized.Magnitude;
        public static bool operator <=(ElectricPotential l, ElectricPotential r) => l < r || l == r;
        public static bool operator >=(ElectricPotential l, ElectricPotential r) => l > r || l == r;
        #endregion

        #region SELF ARITHMETIC OPERATORS
        public static ElectricPotential operator +(ElectricPotential l, ElectricPotential r)
            => new(l.Normalized.Magnitude + r.Normalized.Magnitude);
        public static ElectricPotential operator -(ElectricPotential l, ElectricPotential r)
            => new(l.Normalized.Magnitude - r.Normalized.Magnitude);
        public static ElectricPotential operator *(ElectricPotential l, double r)
            => new(l.Normalized.Magnitude * r);
        public static ElectricPotential operator *(double l, ElectricPotential r) => r * l;
        public static ElectricPotential operator /(ElectricPotential l, double r)
            => new(l.Normalized.Magnitude / r);
        #endregion

        #region CROSS-DIMENSIONAL ARITHMETIC OPERATORS
        public static UnitSquared<ElectricPotential, ElectricPotentialUnits> operator *(ElectricPotential l, ElectricPotential r)
            => new(l.Normalized.Magnitude * r.Normalized.Magnitude);

        public static double operator /(ElectricPotential l, ElectricPotential r)
            => l.Normalized.Magnitude / r.Normalized.Magnitude;
        #endregion

        #region CROSS-UNIT OPERATORS
        public static ProductUnit<ElectricCharge, ElectricChargeUnits, ElectricPotential, ElectricPotentialUnits> operator *(ElectricPotential l, ElectricCharge r)
            => r * l;

        public static Power operator *(ElectricPotential l, Current r) => new(l.Normalized.Magnitude * r.Normalized.Magnitude);
        #endregion
    }

    public struct ElectricResistance :
        IInitializable<ElectricResistance>, IInitializable<ElectricResistance, double>, IInitializable<ElectricResistance, double, ElectricResistanceUnits>,
        IScaleMappable<ElectricResistanceUnits>, IScaleConvertible<ElectricResistance, ElectricResistanceUnits>,
        INormalized<ElectricResistanceUnits>, INormalizable<ElectricResistance>,
        IDimensionAccessible,
        IValueAccessible<ElectricResistanceUnits>,
        IDuplicatable<ElectricResistance>,
        IEquatable<ElectricResistance>,
        IBinaryCompositeTransposable<ElectricResistance, QuotientUnit<ElectricPotential, ElectricPotentialUnits, Current, CurrentUnits>>,
        IExponentiable<ElectricResistance, ElectricResistanceUnits>,
        ILinearUnit<ElectricResistance, ElectricResistanceUnits>,
        ICompositeUnit
    {
        private readonly double _Ohm = 0;
        public const char SYMBOL = 'R';

        #region PROPERTIES
        public int Dimension => 1;
        public static Dictionary<ElectricResistanceUnits, double> Mapper => new()
        {
            // SI & Metric System
            { ElectricResistanceUnits.Ohm, 1.0 },
            { ElectricResistanceUnits.Micohm, 1e-6 },
            { ElectricResistanceUnits.Milliohm, 1e-3 },
            { ElectricResistanceUnits.Kilohm, 1e3 },
            { ElectricResistanceUnits.Megohm, 1e6 },
            { ElectricResistanceUnits.Gigohm, 1e9 },
            { ElectricResistanceUnits.Terohm, 1e12 },

            // CGS System
            { ElectricResistanceUnits.Statohm, 8.987551787368176e11 }, // c^2 * 10^-7 Ω (~8.98755 x 10^11 Ω)
            { ElectricResistanceUnits.Abohm, 1e-9 },                    // 1 abΩ = 10^-9 Ω (1 nΩ)

            // Atomic & Theoretical Units
            { ElectricResistanceUnits.AtomicResistance, 4108.2358997 },  // hbar / (e^2 * 4pi eps_0) (~4108.24 Ω)
            { ElectricResistanceUnits.PlanckImpedance, 29.9792458 }     // 1 / (4pi eps_0 c) = c * 10^-7 Ω (~29.9792 Ω)
        };

        public static ElectricResistanceUnits BaseScale => ElectricResistanceUnits.Ohm;
        public (double Magnitude, ElectricResistanceUnits Scale, int ScaleOrdinal) Normalized => new(_Ohm, BaseScale, (int)BaseScale);

        public (double Magnitude, ElectricResistanceUnits Scale, int ScaleOrdinal) Original { get; private set; } = (0, BaseScale, (int)BaseScale);
        public (double Magnitude, ElectricResistanceUnits Scale, int ScaleOrdinal) Converted { get; private set; } = (0, BaseScale, (int)BaseScale);
        #endregion

        #region CONSTRUCTORS
        public ElectricResistance() { }
        public ElectricResistance(ElectricResistance instance) => this = instance;
        public ElectricResistance(double magnitude)
        {
            Original = (magnitude, BaseScale, (int)BaseScale);
            _Ohm = magnitude;
        }
        public ElectricResistance(double magnitude, ElectricResistanceUnits scale)
        {
            Original = (magnitude, scale, (int)scale);
            _Ohm = Methods.Scale(magnitude, scale, Mapper) / Mapper[BaseScale];
        }
        #endregion

        #region METHODS
        public static ElectricResistance Initialize() => new();
        public static ElectricResistance Create(ElectricResistance instance) => new(instance);
        public static ElectricResistance Create(double magnitude) => new(magnitude);
        public static ElectricResistance Create(double magnitude, ElectricResistanceUnits scale) => new(magnitude, scale);

        public ElectricResistance Duplicate() => new(this);
        public bool Equals(ElectricResistance other) => _Ohm == other._Ohm;

        public ElectricResistance Convert(ElectricResistanceUnits toScale)
        {
            double mag = Methods.Scale(Original.Magnitude, Original.Scale, Mapper) / Mapper[toScale];
            Converted = (mag, toScale, (int)toScale);
            return this;
        }
        public double As(ElectricResistanceUnits scale) => Duplicate().Convert(scale).Converted.Magnitude;
        public ElectricResistance Normalize()
        {
            Original = (_Ohm, BaseScale, (int)BaseScale);
            return this;
        }

        public QuotientUnit<ElectricPotential, ElectricPotentialUnits, Current, CurrentUnits> ToComposite()
            => new QuotientUnit<ElectricPotential, ElectricPotentialUnits, Current, CurrentUnits>(_Ohm).SetScales(ElectricPotentialUnits.Volt, CurrentUnits.Ampere);
        public static ElectricResistance FromComposite(QuotientUnit<ElectricPotential, ElectricPotentialUnits, Current, CurrentUnits> comp)
            => comp.Original.Scale1 == ElectricPotentialUnits.Volt && comp.Original.Scale2 == CurrentUnits.Ampere ?
            new(comp.Original.Magnitude) :
            throw new InvalidOperationException(ErrorMsg.COMPOSITE_SCALES_MISMATCH);

        public UnitSquared<ElectricResistance, ElectricResistanceUnits> Squared() => this * this;
        public UnitCubed<ElectricResistance, ElectricResistanceUnits> Cubed() => this * this * this;
        public HyperUnit<ElectricResistance, ElectricResistanceUnits> Pow(int exp)
            => new HyperUnit<ElectricResistance, ElectricResistanceUnits>(Math.Pow(_Ohm, exp)).SetDimension(exp);

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
        public static bool operator ==(ElectricResistance l, ElectricResistance r) => l.Equals(r);
        public static bool operator !=(ElectricResistance l, ElectricResistance r) => !l.Equals(r);
        public static bool operator <(ElectricResistance l, ElectricResistance r) => l.Normalized.Magnitude < r.Normalized.Magnitude;
        public static bool operator >(ElectricResistance l, ElectricResistance r) => l.Normalized.Magnitude > r.Normalized.Magnitude;
        public static bool operator <=(ElectricResistance l, ElectricResistance r) => l < r || l == r;
        public static bool operator >=(ElectricResistance l, ElectricResistance r) => l > r || l == r;
        #endregion

        #region SELF ARITHMETIC OPERATORS
        public static ElectricResistance operator +(ElectricResistance l, ElectricResistance r)
            => new(l.Normalized.Magnitude + r.Normalized.Magnitude);
        public static ElectricResistance operator -(ElectricResistance l, ElectricResistance r)
            => new(l.Normalized.Magnitude - r.Normalized.Magnitude);
        public static ElectricResistance operator *(ElectricResistance l, double r)
            => new(l.Normalized.Magnitude * r);
        public static ElectricResistance operator *(double l, ElectricResistance r) => r * l;
        public static ElectricResistance operator /(ElectricResistance l, double r)
            => new(l.Normalized.Magnitude / r);
        #endregion

        #region CROSS-DIMENSIONAL ARITHMETIC OPERATORS
        public static UnitSquared<ElectricResistance, ElectricResistanceUnits> operator *(ElectricResistance l, ElectricResistance r)
            => new(l.Normalized.Magnitude * r.Normalized.Magnitude);

        public static double operator /(ElectricResistance l, ElectricResistance r)
            => l.Normalized.Magnitude / r.Normalized.Magnitude;
        #endregion
    }

    public struct ElectricConductance :
        IInitializable<ElectricConductance>, IInitializable<ElectricConductance, double>, IInitializable<ElectricConductance, double, ElectricConductanceUnits>,
        IScaleMappable<ElectricConductanceUnits>, IScaleConvertible<ElectricConductance, ElectricConductanceUnits>,
        INormalized<ElectricConductanceUnits>, INormalizable<ElectricConductance>,
        IDimensionAccessible,
        IValueAccessible<ElectricConductanceUnits>,
        IDuplicatable<ElectricConductance>,
        IEquatable<ElectricConductance>,
        IBinaryCompositeTransposable<ElectricConductance, QuotientUnit<Current, CurrentUnits, ElectricPotential, ElectricPotentialUnits>>,
        IExponentiable<ElectricConductance, ElectricConductanceUnits>,
        ILinearUnit<ElectricConductance, ElectricConductanceUnits>,
        ICompositeUnit
    {
        private readonly double _S = 0;
        public const char SYMBOL = 'G';

        #region PROPERTIES
        public int Dimension => 1;
        public static Dictionary<ElectricConductanceUnits, double> Mapper => new()
        {
            // SI & Metric System
            { ElectricConductanceUnits.Siemens, 1.0 },
            { ElectricConductanceUnits.Microsiemens, 1e-6 },
            { ElectricConductanceUnits.Millisiemens, 1e-3 },
            { ElectricConductanceUnits.Kilosiemens, 1e3 },
            { ElectricConductanceUnits.Megasiemens, 1e6 },

            // Historical & Legacy Units
            { ElectricConductanceUnits.Mho, 1.0 },                      // 1 mho = 1 S

            // CGS System
            { ElectricConductanceUnits.Statsiemens, 1.1126500560536184e-12 }, // 1 / statohm = 10^7 / c^2 S (~1.11265 x 10^-12 S)
            { ElectricConductanceUnits.Absiemens, 1e9 },                       // 1 abS = 10^9 S (1 GS)

            // Atomic & Theoretical Units
            { ElectricConductanceUnits.AtomicConductance, 0.00024341348082218 },// 1 / AtomicResistance = e^2 * (4pi eps_0) / hbar (~2.43413 x 10^-4 S)
            { ElectricConductanceUnits.PlanckAdmittance, 0.0333564095198152 }  // 1 / PlanckImpedance = 4pi eps_0 c (~0.0333564 S)
        };

        public static ElectricConductanceUnits BaseScale => ElectricConductanceUnits.Siemens;
        public (double Magnitude, ElectricConductanceUnits Scale, int ScaleOrdinal) Normalized => new(_S, BaseScale, (int)BaseScale);

        public (double Magnitude, ElectricConductanceUnits Scale, int ScaleOrdinal) Original { get; private set; } = (0, BaseScale, (int)BaseScale);
        public (double Magnitude, ElectricConductanceUnits Scale, int ScaleOrdinal) Converted { get; private set; } = (0, BaseScale, (int)BaseScale);

        #endregion

        #region CONSTRUCTORS
        public ElectricConductance() { }
        public ElectricConductance(ElectricConductance instance) => this = instance;
        public ElectricConductance(double magnitude)
        {
            Original = (magnitude, BaseScale, (int)BaseScale);
            _S = magnitude;
        }
        public ElectricConductance(double magnitude, ElectricConductanceUnits scale)
        {
            Original = (magnitude, scale, (int)scale);
            _S = Methods.Scale(magnitude, scale, Mapper) / Mapper[BaseScale];
        }
        #endregion

        #region METHODS
        public static ElectricConductance Initialize() => new();
        public static ElectricConductance Create(ElectricConductance instance) => new(instance);
        public static ElectricConductance Create(double magnitude) => new(magnitude);
        public static ElectricConductance Create(double magnitude, ElectricConductanceUnits scale) => new(magnitude, scale);

        public ElectricConductance Duplicate() => new(this);
        public bool Equals(ElectricConductance other) => _S == other._S;

        public ElectricConductance Convert(ElectricConductanceUnits toScale)
        {
            double mag = Methods.Scale(Original.Magnitude, Original.Scale, Mapper) / Mapper[toScale];
            Converted = (mag, toScale, (int)toScale);
            return this;
        }
        public double As(ElectricConductanceUnits scale) => Duplicate().Convert(scale).Converted.Magnitude;
        public ElectricConductance Normalize()
        {
            Original = (_S, BaseScale, (int)BaseScale);
            return this;
        }

        public QuotientUnit<Current, CurrentUnits, ElectricPotential, ElectricPotentialUnits> ToComposite()
            => new QuotientUnit<Current, CurrentUnits, ElectricPotential, ElectricPotentialUnits>(_S).SetScales(CurrentUnits.Ampere, ElectricPotentialUnits.Volt);
        public static ElectricConductance FromComposite(QuotientUnit<Current, CurrentUnits, ElectricPotential, ElectricPotentialUnits> comp)
            => comp.Original.Scale1 == CurrentUnits.Ampere && comp.Original.Scale2 == ElectricPotentialUnits.Volt ?
            new(comp.Original.Magnitude) :
            throw new InvalidOperationException(ErrorMsg.COMPOSITE_SCALES_MISMATCH);
        public ReciprocalUnit<ElectricResistance, ElectricResistanceUnits> ToInverseResistance() => new(_S);
        public static ElectricConductance FromInverseResistance(ReciprocalUnit<ElectricResistance, ElectricResistanceUnits> inverse)
            => new(inverse.Normalized.Magnitude);

        public UnitSquared<ElectricConductance, ElectricConductanceUnits> Squared() => this * this;
        public UnitCubed<ElectricConductance, ElectricConductanceUnits> Cubed() => this * this * this;
        public HyperUnit<ElectricConductance, ElectricConductanceUnits> Pow(int exp)
            => new HyperUnit<ElectricConductance, ElectricConductanceUnits>(Math.Pow(_S, exp)).SetDimension(exp);

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
        public static bool operator ==(ElectricConductance l, ElectricConductance r) => l.Equals(r);
        public static bool operator !=(ElectricConductance l, ElectricConductance r) => !l.Equals(r);
        public static bool operator <(ElectricConductance l, ElectricConductance r) => l.Normalized.Magnitude < r.Normalized.Magnitude;
        public static bool operator >(ElectricConductance l, ElectricConductance r) => l.Normalized.Magnitude > r.Normalized.Magnitude;
        public static bool operator <=(ElectricConductance l, ElectricConductance r) => l < r || l == r;
        public static bool operator >=(ElectricConductance l, ElectricConductance r) => l > r || l == r;
        #endregion

        #region SELF ARITHMETIC OPERATORS
        public static ElectricConductance operator +(ElectricConductance l, ElectricConductance r)
            => new(l.Normalized.Magnitude + r.Normalized.Magnitude);
        public static ElectricConductance operator -(ElectricConductance l, ElectricConductance r)
            => new(l.Normalized.Magnitude - r.Normalized.Magnitude);
        public static ElectricConductance operator *(ElectricConductance l, double r)
            => new(l.Normalized.Magnitude * r);
        public static ElectricConductance operator *(double l, ElectricConductance r) => r * l;
        public static ElectricConductance operator /(ElectricConductance l, double r)
            => new(l.Normalized.Magnitude / r);
        #endregion

        #region CROSS-DIMENSIONAL ARITHMETIC OPERATORS
        public static UnitSquared<ElectricConductance, ElectricConductanceUnits> operator *(ElectricConductance l, ElectricConductance r)
            => new(l.Normalized.Magnitude * r.Normalized.Magnitude);

        public static double operator /(ElectricConductance l, ElectricConductance r)
            => l.Normalized.Magnitude / r.Normalized.Magnitude;
        #endregion
    }

    public struct Capacitance :
        IInitializable<Capacitance>, IInitializable<Capacitance, double>, IInitializable<Capacitance, double, CapacitanceUnits>,
        IScaleMappable<CapacitanceUnits>, IScaleConvertible<Capacitance, CapacitanceUnits>,
        INormalized<CapacitanceUnits>, INormalizable<Capacitance>,
        IDimensionAccessible,
        IValueAccessible<CapacitanceUnits>,
        IDuplicatable<Capacitance>,
        IEquatable<Capacitance>,
        IBinaryCompositeTransposable<Capacitance, QuotientUnit<ElectricCharge, ElectricChargeUnits, ElectricPotential, ElectricPotentialUnits>>,
        IExponentiable<Capacitance, CapacitanceUnits>,
        ILinearUnit<Capacitance, CapacitanceUnits>,
        ICompositeUnit
    {
        private readonly double _F = 0;
        public const char SYMBOL = 'C';

        #region PROPERTIES
        public int Dimension => 1;
        public static Dictionary<CapacitanceUnits, double> Mapper => new()
        {
            // SI & Metric System
            { CapacitanceUnits.Farad, 1.0 },
            { CapacitanceUnits.Millifarad, 1e-3 },
            { CapacitanceUnits.Microfarad, 1e-6 },
            { CapacitanceUnits.Nanofarad, 1e-9 },
            { CapacitanceUnits.Picofarad, 1e-12 },
            { CapacitanceUnits.Femtofarad, 1e-15 },
            { CapacitanceUnits.Kilofarad, 1e3 },

            // CGS System
            { CapacitanceUnits.Statfarad, 1.1126500560536184e-12 },   // 10^7 / c^2 F (~1.11265 pF)
            { CapacitanceUnits.Abfarad, 1e9 },                         // 1 abF = 10^9 F (1 GF)
            { CapacitanceUnits.Centimeter, 1.1126500560536184e-12 },   // 1 cm of capacitance = 1 statfarad

            // Derived Representation
            { CapacitanceUnits.Ampere_Second_PerVolt, 1.0 },           // 1 A·s/V = 1 F

            // Atomic & Theoretical Units
            { CapacitanceUnits.AtomicCapacitance, 1.1126500560536184e-20 }, // 4pi * eps_0 * a_0 (Bohr radius)
            { CapacitanceUnits.PlanckCapacitance, 1.797800040683692e-46 }    // 4pi * eps_0 * l_P (Planck length)
        };

        public static CapacitanceUnits BaseScale => CapacitanceUnits.Farad;
        public (double Magnitude, CapacitanceUnits Scale, int ScaleOrdinal) Normalized => new(_F, BaseScale, (int)BaseScale);

        public (double Magnitude, CapacitanceUnits Scale, int ScaleOrdinal) Original { get; private set; } = (0, BaseScale, (int)BaseScale);
        public (double Magnitude, CapacitanceUnits Scale, int ScaleOrdinal) Converted { get; private set; } = (0, BaseScale, (int)BaseScale);
        #endregion

        #region CONSTRUCTORS
        public Capacitance() { }
        public Capacitance(Capacitance instance) => this = instance;
        public Capacitance(double magnitude)
        {
            Original = (magnitude, BaseScale, (int)BaseScale);
            _F = magnitude;
        }
        public Capacitance(double magnitude, CapacitanceUnits scale)
        {
            Original = (magnitude, scale, (int)scale);
            _F = Methods.Scale(magnitude, scale, Mapper) / Mapper[BaseScale];
        }
        #endregion

        #region METHODS
        public static Capacitance Initialize() => new();
        public static Capacitance Create(Capacitance instance) => new(instance);
        public static Capacitance Create(double magnitude) => new(magnitude);
        public static Capacitance Create(double magnitude, CapacitanceUnits scale) => new(magnitude, scale);

        public Capacitance Duplicate() => new(this);
        public bool Equals(Capacitance other) => _F == other._F;

        public Capacitance Convert(CapacitanceUnits toScale)
        {
            double mag = Methods.Scale(Original.Magnitude, Original.Scale, Mapper) / Mapper[toScale];
            Converted = (mag, toScale, (int)toScale);
            return this;
        }
        public double As(CapacitanceUnits scale) => Duplicate().Convert(scale).Converted.Magnitude;
        public Capacitance Normalize()
        {
            Original = (_F, BaseScale, (int)BaseScale);
            return this;
        }

        public QuotientUnit<ElectricCharge, ElectricChargeUnits, ElectricPotential, ElectricPotentialUnits> ToComposite()
            => new QuotientUnit<ElectricCharge, ElectricChargeUnits, ElectricPotential, ElectricPotentialUnits>(_F).SetScales(ElectricChargeUnits.Coulomb, ElectricPotentialUnits.Volt);
        public static Capacitance FromComposite(QuotientUnit<ElectricCharge, ElectricChargeUnits, ElectricPotential, ElectricPotentialUnits> comp)
            => comp.Original.Scale1 == ElectricChargeUnits.Coulomb && comp.Original.Scale2 == ElectricPotentialUnits.Volt ?
            new Capacitance(comp.Original.Magnitude) :
            throw new InvalidOperationException(ErrorMsg.COMPOSITE_SCALES_MISMATCH);
        public TernaryQuotientUnit<Numerator<CompositeProduct<Current, Time>>, Current, CurrentUnits, Time, TimeUnits, ElectricPotential, ElectricPotentialUnits> ToAmpereSecondPerVolt()
            => new TernaryQuotientUnit<Numerator<CompositeProduct<Current, Time>>, Current, CurrentUnits, Time, TimeUnits, ElectricPotential, ElectricPotentialUnits>(_F)
            .SetScales(CurrentUnits.Ampere, TimeUnits.Second, ElectricPotentialUnits.Volt);

        public UnitSquared<Capacitance, CapacitanceUnits> Squared() => this * this;
        public UnitCubed<Capacitance, CapacitanceUnits> Cubed() => this * this * this;
        public HyperUnit<Capacitance, CapacitanceUnits> Pow(int exp)
            => new HyperUnit<Capacitance, CapacitanceUnits>(Math.Pow(_F, exp)).SetDimension(exp);

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
        public static bool operator ==(Capacitance l, Capacitance r) => l.Equals(r);
        public static bool operator !=(Capacitance l, Capacitance r) => !l.Equals(r);
        public static bool operator <(Capacitance l, Capacitance r) => l.Normalized.Magnitude < r.Normalized.Magnitude;
        public static bool operator >(Capacitance l, Capacitance r) => l.Normalized.Magnitude > r.Normalized.Magnitude;
        public static bool operator <=(Capacitance l, Capacitance r) => l < r || l == r;
        public static bool operator >=(Capacitance l, Capacitance r) => l > r || l == r;
        #endregion

        #region SELF ARITHMETIC OPERATORS
        public static Capacitance operator +(Capacitance l, Capacitance r)
            => new(l.Normalized.Magnitude + r.Normalized.Magnitude);
        public static Capacitance operator -(Capacitance l, Capacitance r)
            => new(l.Normalized.Magnitude - r.Normalized.Magnitude);
        public static Capacitance operator *(Capacitance l, double r)
            => new(l.Normalized.Magnitude * r);
        public static Capacitance operator *(double l, Capacitance r) => r * l;
        public static Capacitance operator /(Capacitance l, double r)
            => new(l.Normalized.Magnitude / r);
        #endregion

        #region CROSS-DIMENSIONAL ARITHMETIC OPERATORS
        public static UnitSquared<Capacitance, CapacitanceUnits> operator *(Capacitance l, Capacitance r)
            => new(l.Normalized.Magnitude * r.Normalized.Magnitude);

        public static double operator /(Capacitance l, Capacitance r)
            => l.Normalized.Magnitude / r.Normalized.Magnitude;
        #endregion
    }

    public struct MagneticFlux :
        IInitializable<MagneticFlux>, IInitializable<MagneticFlux, double>, IInitializable<MagneticFlux, double, MagneticFluxUnits>,
        IScaleMappable<MagneticFluxUnits>, IScaleConvertible<MagneticFlux, MagneticFluxUnits>,
        INormalized<MagneticFluxUnits>, INormalizable<MagneticFlux>,
        IDimensionAccessible,
        IValueAccessible<MagneticFluxUnits>,
        IDuplicatable<MagneticFlux>,
        IEquatable<MagneticFlux>,
        IBinaryCompositeTransposable<MagneticFlux, ProductUnit<ElectricPotential, ElectricPotentialUnits, Time, TimeUnits>>,
        IExponentiable<MagneticFlux, MagneticFluxUnits>,
        ILinearUnit<MagneticFlux, MagneticFluxUnits>,
        ICompositeUnit
    {
        private readonly double _Wb = 0;
        public const char SYMBOL = 'Φ';

        #region PROPERTIES
        public int Dimension => 1;
        public static Dictionary<MagneticFluxUnits, double> Mapper => new()
        {
            // SI & Metric System
            { MagneticFluxUnits.Weber, 1.0 },
            { MagneticFluxUnits.Microweber, 1e-6 },
            { MagneticFluxUnits.Milliweber, 1e-3 },
            { MagneticFluxUnits.Kiloweber, 1e3 },
            { MagneticFluxUnits.Megaweber, 1e6 },

            // CGS & Electromagnetic System
            { MagneticFluxUnits.Maxwell, 1e-8 },                       // 1 Mx = 1 G·cm² = 10^-8 Wb
            { MagneticFluxUnits.LineOfForce, 1e-8 },                   // Identical to Maxwell (1 line = 1 Mx = 10^-8 Wb)
            { MagneticFluxUnits.Kilomaxwell, 1e-5 },                   // 1000 Mx = 10^-5 Wb
            { MagneticFluxUnits.Statweber, 299.792458 },               // Exact c / 10^6 Wb (~299.792 Wb)

            // Atomic & Theoretical Units
            { MagneticFluxUnits.AtomicMagneticFlux, 1.054571817e-15 }, // hbar / e (~1.05457 x 10^-15 Wb)
            { MagneticFluxUnits.PlanckMagneticFlux, 1.1206e-5 }        // sqrt(hbar * c / (G * eps_0)) (~1.1206 x 10^-5 Wb)
        };

        public static MagneticFluxUnits BaseScale => MagneticFluxUnits.Weber;
        public (double Magnitude, MagneticFluxUnits Scale, int ScaleOrdinal) Normalized => new(_Wb, BaseScale, (int)BaseScale);

        public (double Magnitude, MagneticFluxUnits Scale, int ScaleOrdinal) Original { get; private set; } = (0, BaseScale, (int)BaseScale);
        public (double Magnitude, MagneticFluxUnits Scale, int ScaleOrdinal) Converted { get; private set; } = (0, BaseScale, (int)BaseScale);
        #endregion

        #region CONSTRUCTORS
        public MagneticFlux() { }
        public MagneticFlux(MagneticFlux instance) => this = instance;
        public MagneticFlux(double magnitude)
        {
            Original = (magnitude, BaseScale, (int)BaseScale);
            _Wb = magnitude;
        }
        public MagneticFlux(double magnitude, MagneticFluxUnits scale)
        {
            Original = (magnitude, scale, (int)scale);
            _Wb = Methods.Scale(magnitude, scale, Mapper) / Mapper[BaseScale];
        }
        #endregion

        #region METHODS
        public static MagneticFlux Initialize() => new();
        public static MagneticFlux Create(MagneticFlux instance) => new(instance);
        public static MagneticFlux Create(double magnitude) => new(magnitude);
        public static MagneticFlux Create(double magnitude, MagneticFluxUnits scale) => new(magnitude, scale);

        public MagneticFlux Duplicate() => new(this);
        public bool Equals(MagneticFlux other) => _Wb == other._Wb;

        public MagneticFlux Convert(MagneticFluxUnits toScale)
        {
            double mag = Methods.Scale(Original.Magnitude, Original.Scale, Mapper) / Mapper[toScale];
            Converted = (mag, toScale, (int)toScale);
            return this;
        }
        public double As(MagneticFluxUnits scale) => Duplicate().Convert(scale).Converted.Magnitude;
        public MagneticFlux Normalize()
        {
            Original = (_Wb, BaseScale, (int)BaseScale);
            return this;
        }

        public ProductUnit<ElectricPotential, ElectricPotentialUnits, Time, TimeUnits> ToComposite()
            => new ProductUnit<ElectricPotential, ElectricPotentialUnits, Time, TimeUnits>(_Wb).SetScales(ElectricPotentialUnits.Volt, TimeUnits.Second);
        public static MagneticFlux FromComposite(ProductUnit<ElectricPotential, ElectricPotentialUnits, Time, TimeUnits> comp)
            => comp.Original.Scale1 == ElectricPotentialUnits.Volt && comp.Original.Scale2 == TimeUnits.Second ?
            new(comp.Original.Magnitude) :
            throw new InvalidOperationException(ErrorMsg.COMPOSITE_SCALES_MISMATCH);
        public QuotientUnit<Energy, EnergyUnits, Current, CurrentUnits> ToJoulePerAmpere()
            => new QuotientUnit<Energy, EnergyUnits, Current, CurrentUnits>(_Wb).SetScales(EnergyUnits.Joule, CurrentUnits.Ampere);

        public UnitSquared<MagneticFlux, MagneticFluxUnits> Squared() => this * this;
        public UnitCubed<MagneticFlux, MagneticFluxUnits> Cubed() => this * this * this;
        public HyperUnit<MagneticFlux, MagneticFluxUnits> Pow(int exp)
            => new HyperUnit<MagneticFlux, MagneticFluxUnits>(Math.Pow(_Wb, exp)).SetDimension(exp);

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
        public static bool operator ==(MagneticFlux l, MagneticFlux r) => l.Equals(r);
        public static bool operator !=(MagneticFlux l, MagneticFlux r) => !l.Equals(r);
        public static bool operator <(MagneticFlux l, MagneticFlux r) => l.Normalized.Magnitude < r.Normalized.Magnitude;
        public static bool operator >(MagneticFlux l, MagneticFlux r) => l.Normalized.Magnitude > r.Normalized.Magnitude;
        public static bool operator <=(MagneticFlux l, MagneticFlux r) => l < r || l == r;
        public static bool operator >=(MagneticFlux l, MagneticFlux r) => l > r || l == r;
        #endregion

        #region SELF ARITHMETIC OPERATORS
        public static MagneticFlux operator +(MagneticFlux l, MagneticFlux r)
            => new(l.Normalized.Magnitude + r.Normalized.Magnitude);
        public static MagneticFlux operator -(MagneticFlux l, MagneticFlux r)
            => new(l.Normalized.Magnitude - r.Normalized.Magnitude);
        public static MagneticFlux operator *(MagneticFlux l, double r)
            => new(l.Normalized.Magnitude * r);
        public static MagneticFlux operator *(double l, MagneticFlux r) => r * l;
        public static MagneticFlux operator /(MagneticFlux l, double r)
            => new(l.Normalized.Magnitude / r);
        #endregion

        #region CROSS-DIMENSIONAL ARITHMETIC OPERATORS
        public static UnitSquared<MagneticFlux, MagneticFluxUnits> operator *(MagneticFlux l, MagneticFlux r)
            => new(l.Normalized.Magnitude * r.Normalized.Magnitude);

        public static double operator /(MagneticFlux l, MagneticFlux r)
            => l.Normalized.Magnitude / r.Normalized.Magnitude;
        #endregion
    }

    public struct Inductance :
        IInitializable<Inductance>, IInitializable<Inductance, double>, IInitializable<Inductance, double, InductanceUnits>,
        IScaleMappable<InductanceUnits>, IScaleConvertible<Inductance, InductanceUnits>,
        INormalized<InductanceUnits>, INormalizable<Inductance>,
        IDimensionAccessible,
        IValueAccessible<InductanceUnits>,
        IDuplicatable<Inductance>,
        IEquatable<Inductance>,
        IBinaryCompositeTransposable<Inductance, QuotientUnit<MagneticFlux, MagneticFluxUnits, Current, CurrentUnits>>,
        IExponentiable<Inductance, InductanceUnits>,
        ILinearUnit<Inductance, InductanceUnits>,
        ICompositeUnit
    {
        private readonly double _H = 0;
        public const char SYMBOL = 'L';

        #region PROPERTIES
        public int Dimension => 1;
        public static Dictionary<InductanceUnits, double> Mapper => new()
        {
            // SI & Metric System
            { InductanceUnits.Henry, 1.0 },
            { InductanceUnits.Nanohenry, 1e-9 },
            { InductanceUnits.Microhenry, 1e-6 },
            { InductanceUnits.Millihenry, 1e-3 },
            { InductanceUnits.Kilohenry, 1e3 },

            // CGS & Electromagnetic System
            { InductanceUnits.Abhenry, 1e-9 },                          // 1 abH = 10^-9 H (1 nH)
            { InductanceUnits.Stathenry, 8.987551787368176e11 },       // c^2 * 10^-7 H (~8.98755 x 10^11 H)
            { InductanceUnits.Centimeter, 1e-9 },                       // 1 cm of inductance = 1 abH = 10^-9 H

            // Atomic & Theoretical Units
            { InductanceUnits.AtomicInductance, 2.1798723611035e-19 },   // hbar^2 / (m_e * e^2) (~2.17987 x 10^-19 H)
            { InductanceUnits.PlanckInductance, 5.99584916e-44 }        // mu_0 * l_P / (4pi) (~5.99585 x 10^-44 H)
        };

        public static InductanceUnits BaseScale => InductanceUnits.Henry;
        public (double Magnitude, InductanceUnits Scale, int ScaleOrdinal) Normalized => new(_H, BaseScale, (int)BaseScale);

        public (double Magnitude, InductanceUnits Scale, int ScaleOrdinal) Original { get; private set; } = (0, BaseScale, (int)BaseScale);
        public (double Magnitude, InductanceUnits Scale, int ScaleOrdinal) Converted { get; private set; } = (0, BaseScale, (int)BaseScale);
        #endregion

        #region CONSTRUCTORS
        public Inductance() { }
        public Inductance(Inductance instance) => this = instance;
        public Inductance(double magnitude)
        {
            Original = (magnitude, BaseScale, (int)BaseScale);
            _H = magnitude;
        }
        public Inductance(double magnitude, InductanceUnits scale)
        {
            Original = (magnitude, scale, (int)scale);
            _H = Methods.Scale(magnitude, scale, Mapper) / Mapper[BaseScale];
        }
        #endregion

        #region METHODS
        public static Inductance Initialize() => new();
        public static Inductance Create(Inductance instance) => new(instance);
        public static Inductance Create(double magnitude) => new(magnitude);
        public static Inductance Create(double magnitude, InductanceUnits scale) => new(magnitude, scale);

        public Inductance Duplicate() => new(this);
        public bool Equals(Inductance other) => _H == other._H;

        public Inductance Convert(InductanceUnits toScale)
        {
            double mag = Methods.Scale(Original.Magnitude, Original.Scale, Mapper) / Mapper[toScale];
            Converted = (mag, toScale, (int)toScale);
            return this;
        }
        public double As(InductanceUnits scale) => Duplicate().Convert(scale).Converted.Magnitude;
        public Inductance Normalize()
        {
            Original = (_H, BaseScale, (int)BaseScale);
            return this;
        }

        public QuotientUnit<MagneticFlux, MagneticFluxUnits, Current, CurrentUnits> ToComposite()
            => new QuotientUnit<MagneticFlux, MagneticFluxUnits, Current, CurrentUnits>(_H).SetScales(MagneticFluxUnits.Weber, CurrentUnits.Ampere);
        public static Inductance FromComposite(QuotientUnit<MagneticFlux, MagneticFluxUnits, Current, CurrentUnits> comp)
            => comp.Original.Scale1 == MagneticFluxUnits.Weber && comp.Original.Scale2 == CurrentUnits.Ampere ?
            new(comp.Original.Magnitude) :
            throw new InvalidOperationException(ErrorMsg.COMPOSITE_SCALES_MISMATCH);
        public ProductUnit<ElectricResistance, ElectricResistanceUnits, Time, TimeUnits> ToOhmSecond()
            => new ProductUnit<ElectricResistance, ElectricResistanceUnits, Time, TimeUnits>(_H).SetScales(ElectricResistanceUnits.Ohm, TimeUnits.Second);
        public static Inductance FromComposite(ProductUnit<ElectricResistance, ElectricResistanceUnits, Time, TimeUnits> comp)
            => comp.Original.Scale1 == ElectricResistanceUnits.Ohm && comp.Original.Scale2 == TimeUnits.Second ?
            new(comp.Original.Magnitude) :
            throw new InvalidOperationException(ErrorMsg.COMPOSITE_SCALES_MISMATCH);
        public TernaryQuotientUnit<Numerator<CompositeProduct<ElectricPotential, Time>>, ElectricPotential, ElectricPotentialUnits, Time, TimeUnits, Current, CurrentUnits> ToVoltSecond_PerAmpere()
            => new TernaryQuotientUnit<Numerator<CompositeProduct<ElectricPotential, Time>>, ElectricPotential, ElectricPotentialUnits, Time, TimeUnits, Current, CurrentUnits>(_H)
            .SetScales(ElectricPotentialUnits.Volt, TimeUnits.Second, CurrentUnits.Ampere);


        public UnitSquared<Inductance, InductanceUnits> Squared() => this * this;
        public UnitCubed<Inductance, InductanceUnits> Cubed() => this * this * this;
        public HyperUnit<Inductance, InductanceUnits> Pow(int exp)
            => new HyperUnit<Inductance, InductanceUnits>(Math.Pow(_H, exp)).SetDimension(exp);

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
        public static bool operator ==(Inductance l, Inductance r) => l.Equals(r);
        public static bool operator !=(Inductance l, Inductance r) => !l.Equals(r);
        public static bool operator <(Inductance l, Inductance r) => l.Normalized.Magnitude < r.Normalized.Magnitude;
        public static bool operator >(Inductance l, Inductance r) => l.Normalized.Magnitude > r.Normalized.Magnitude;
        public static bool operator <=(Inductance l, Inductance r) => l < r || l == r;
        public static bool operator >=(Inductance l, Inductance r) => l > r || l == r;
        #endregion

        #region SELF ARITHMETIC OPERATORS
        public static Inductance operator +(Inductance l, Inductance r)
            => new(l.Normalized.Magnitude + r.Normalized.Magnitude);
        public static Inductance operator -(Inductance l, Inductance r)
            => new(l.Normalized.Magnitude - r.Normalized.Magnitude);
        public static Inductance operator *(Inductance l, double r)
            => new(l.Normalized.Magnitude * r);
        public static Inductance operator *(double l, Inductance r) => r * l;
        public static Inductance operator /(Inductance l, double r)
            => new(l.Normalized.Magnitude / r);
        #endregion

        #region CROSS-DIMENSIONAL ARITHMETIC OPERATORS
        public static UnitSquared<Inductance, InductanceUnits> operator *(Inductance l, Inductance r)
            => new(l.Normalized.Magnitude * r.Normalized.Magnitude);

        public static double operator /(Inductance l, Inductance r)
            => l.Normalized.Magnitude / r.Normalized.Magnitude;
        #endregion
    }

    public struct MagneticFluxDensity :
        IInitializable<MagneticFluxDensity>, IInitializable<MagneticFluxDensity, double>, IInitializable<MagneticFluxDensity, double, MagneticFluxDensityUnits>,
        IScaleMappable<MagneticFluxDensityUnits>, IScaleConvertible<MagneticFluxDensity, MagneticFluxDensityUnits>,
        INormalized<MagneticFluxDensityUnits>, INormalizable<MagneticFluxDensity>,
        IDimensionAccessible,
        IValueAccessible<MagneticFluxDensityUnits>,
        IDuplicatable<MagneticFluxDensity>,
        IEquatable<MagneticFluxDensity>,
        IBinaryCompositeTransposable<MagneticFluxDensity, QuotientUnit<MagneticFlux, MagneticFluxUnits, UnitSquared<Length, LengthUnits>, LengthUnits>>,
        IExponentiable<MagneticFluxDensity, MagneticFluxDensityUnits>,
        ILinearUnit<MagneticFluxDensity, MagneticFluxDensityUnits>,
        ICompositeUnit
    {
        private readonly double _T = 0;
        public const char SYMBOL = 'B';

        #region PROPERTIES
        public int Dimension => 1;
        public static Dictionary<MagneticFluxDensityUnits, double> Mapper => new()
        {
            // SI & Metric System
            { MagneticFluxDensityUnits.Tesla, 1.0 },
            { MagneticFluxDensityUnits.Nanotesla, 1e-9 },
            { MagneticFluxDensityUnits.Microtesla, 1e-6 },
            { MagneticFluxDensityUnits.Millitesla, 1e-3 },
            { MagneticFluxDensityUnits.Kilotesla, 1e3 },

            // CGS & Electromagnetic System
            { MagneticFluxDensityUnits.Gauss, 1e-4 },                     // 1 G = 10^-4 T
            { MagneticFluxDensityUnits.Gamma, 1e-9 },                     // 1 gamma = 1 nT = 10^-9 T
            { MagneticFluxDensityUnits.Stattesla, 299.792458 },           // Exact c / 10^6 T (~299.792 T)

            // Atomic & Theoretical Units
            { MagneticFluxDensityUnits.AtomicFluxDensity, 235051.756758 }, // hbar / (e * a_0^2) (~2.35052 x 10^5 T)
            { MagneticFluxDensityUnits.PlanckMagneticField, 5.101e53 }    // sqrt(c^7 / (hbar * G^2 * eps_0)) (~5.101 x 10^53 T)
        };

        public static MagneticFluxDensityUnits BaseScale => MagneticFluxDensityUnits.Tesla;
        public (double Magnitude, MagneticFluxDensityUnits Scale, int ScaleOrdinal) Normalized => new(_T, BaseScale, (int)BaseScale);

        public (double Magnitude, MagneticFluxDensityUnits Scale, int ScaleOrdinal) Original { get; private set; } = (0, BaseScale, (int)BaseScale);
        public (double Magnitude, MagneticFluxDensityUnits Scale, int ScaleOrdinal) Converted { get; private set; } = (0, BaseScale, (int)BaseScale);
        #endregion

        #region CONSTRUCTORS
        public MagneticFluxDensity() { }
        public MagneticFluxDensity(MagneticFluxDensity instance) => this = instance;
        public MagneticFluxDensity(double magnitude)
        {
            Original = (magnitude, BaseScale, (int)BaseScale);
            _T = magnitude;
        }
        public MagneticFluxDensity(double magnitude, MagneticFluxDensityUnits scale)
        {
            Original = (magnitude, scale, (int)scale);
            _T = Methods.Scale(magnitude, scale, Mapper) / Mapper[BaseScale];
        }
        #endregion

        #region METHODS
        public static MagneticFluxDensity Initialize() => new();
        public static MagneticFluxDensity Create(MagneticFluxDensity instance) => new(instance);
        public static MagneticFluxDensity Create(double magnitude) => new(magnitude);
        public static MagneticFluxDensity Create(double magnitude, MagneticFluxDensityUnits scale) => new(magnitude, scale);

        public MagneticFluxDensity Duplicate() => new(this);
        public bool Equals(MagneticFluxDensity other) => _T == other._T;

        public MagneticFluxDensity Convert(MagneticFluxDensityUnits toScale)
        {
            double mag = Methods.Scale(Original.Magnitude, Original.Scale, Mapper) / Mapper[toScale];
            Converted = (mag, toScale, (int)toScale);
            return this;
        }
        public double As(MagneticFluxDensityUnits scale) => Duplicate().Convert(scale).Converted.Magnitude;
        public MagneticFluxDensity Normalize()
        {
            Original = (_T, BaseScale, (int)BaseScale);
            return this;
        }

        public QuotientUnit<MagneticFlux, MagneticFluxUnits, UnitSquared<Length, LengthUnits>, LengthUnits> ToComposite()
            => new QuotientUnit<MagneticFlux, MagneticFluxUnits, UnitSquared<Length, LengthUnits>, LengthUnits>(_T).SetScales(MagneticFluxUnits.Weber, LengthUnits.Meter);
        public static MagneticFluxDensity FromComposite(QuotientUnit<MagneticFlux, MagneticFluxUnits, UnitSquared<Length, LengthUnits>, LengthUnits> comp)
            => comp.Original.Scale1 == MagneticFluxUnits.Weber && comp.Original.Scale2 == LengthUnits.Meter ?
            new(comp.Original.Magnitude) :
            throw new InvalidOperationException(ErrorMsg.COMPOSITE_SCALES_MISMATCH);
        public TernaryQuotientUnit<Denominator<CompositeProduct<Current, Length>>, Force, ForceUnits, Current, CurrentUnits, Length, LengthUnits> ToNewton_PerAmpereMeter()
            => new TernaryQuotientUnit<Denominator<CompositeProduct<Current, Length>>, Force, ForceUnits, Current, CurrentUnits, Length, LengthUnits>(_T)
            .SetScales(ForceUnits.Newton, CurrentUnits.Ampere, LengthUnits.Meter);
        public TernaryQuotientUnit<Numerator<CompositeProduct<ElectricPotential, Time>>, ElectricPotential, ElectricPotentialUnits, Time, TimeUnits, UnitSquared<Length, LengthUnits>, LengthUnits> ToVoltSecond_PerSquareMeter()
            => new TernaryQuotientUnit<Numerator<CompositeProduct<ElectricPotential, Time>>, ElectricPotential, ElectricPotentialUnits, Time, TimeUnits, UnitSquared<Length, LengthUnits>, LengthUnits>(_T)
            .SetScales(ElectricPotentialUnits.Volt, TimeUnits.Second, LengthUnits.Meter);
        public QuaternaryQuotientUnit<
            BinaryComposite<
                Numerator<CompositeProduct<Force, Time>>,
                Denominator<CompositeProduct<ElectricCharge, Length>>>,
            Force, ForceUnits,
            Time, TimeUnits,
            ElectricCharge, ElectricChargeUnits,
            Length, LengthUnits> ToNewtonSecond_PerCoulombMeter()
            => new QuaternaryQuotientUnit<BinaryComposite<Numerator<CompositeProduct<Force, Time>>, Denominator<CompositeProduct<ElectricCharge, Length>>>, Force, ForceUnits, Time, TimeUnits, ElectricCharge, ElectricChargeUnits, Length, LengthUnits>(_T)
            .SetScales(ForceUnits.Newton, TimeUnits.Second, ElectricChargeUnits.Coulomb, LengthUnits.Meter);


        public UnitSquared<MagneticFluxDensity, MagneticFluxDensityUnits> Squared() => this * this;
        public UnitCubed<MagneticFluxDensity, MagneticFluxDensityUnits> Cubed() => this * this * this;
        public HyperUnit<MagneticFluxDensity, MagneticFluxDensityUnits> Pow(int exp)
            => new HyperUnit<MagneticFluxDensity, MagneticFluxDensityUnits>(Math.Pow(_T, exp)).SetDimension(exp);

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
        public static bool operator ==(MagneticFluxDensity l, MagneticFluxDensity r) => l.Equals(r);
        public static bool operator !=(MagneticFluxDensity l, MagneticFluxDensity r) => !l.Equals(r);
        public static bool operator <(MagneticFluxDensity l, MagneticFluxDensity r) => l.Normalized.Magnitude < r.Normalized.Magnitude;
        public static bool operator >(MagneticFluxDensity l, MagneticFluxDensity r) => l.Normalized.Magnitude > r.Normalized.Magnitude;
        public static bool operator <=(MagneticFluxDensity l, MagneticFluxDensity r) => l < r || l == r;
        public static bool operator >=(MagneticFluxDensity l, MagneticFluxDensity r) => l > r || l == r;
        #endregion

        #region SELF ARITHMETIC OPERATORS
        public static MagneticFluxDensity operator +(MagneticFluxDensity l, MagneticFluxDensity r)
            => new(l.Normalized.Magnitude + r.Normalized.Magnitude);
        public static MagneticFluxDensity operator -(MagneticFluxDensity l, MagneticFluxDensity r)
            => new(l.Normalized.Magnitude - r.Normalized.Magnitude);
        public static MagneticFluxDensity operator *(MagneticFluxDensity l, double r)
            => new(l.Normalized.Magnitude * r);
        public static MagneticFluxDensity operator *(double l, MagneticFluxDensity r) => r * l;
        public static MagneticFluxDensity operator /(MagneticFluxDensity l, double r)
            => new(l.Normalized.Magnitude / r);
        #endregion

        #region CROSS-DIMENSIONAL ARITHMETIC OPERATORS
        public static UnitSquared<MagneticFluxDensity, MagneticFluxDensityUnits> operator *(MagneticFluxDensity l, MagneticFluxDensity r)
            => new(l.Normalized.Magnitude * r.Normalized.Magnitude);

        public static double operator /(MagneticFluxDensity l, MagneticFluxDensity r)
            => l.Normalized.Magnitude / r.Normalized.Magnitude;
        #endregion

        #region CROSS-UNIT OPERATORS
        public static QuotientUnit<Force, ForceUnits, ElectricCharge, ElectricChargeUnits> operator *(MagneticFluxDensity l, Velocity r)
            => new QuotientUnit<Force, ForceUnits, ElectricCharge, ElectricChargeUnits>(l.Normalized.Magnitude * r.Normalized.Magnitude).SetScales(ForceUnits.Newton, ElectricChargeUnits.Coulomb);
        public static QuotientUnit<Force, ForceUnits, ElectricCharge, ElectricChargeUnits> operator *(Velocity l, MagneticFluxDensity r) => r * l;
        #endregion
    }

    public struct MagneticFluxStrength :
        IInitializable<MagneticFluxStrength>, IInitializable<MagneticFluxStrength, double>, IInitializable<MagneticFluxStrength, double, MagneticFluxStrengthUnits>,
        IScaleMappable<MagneticFluxStrengthUnits>, IScaleConvertible<MagneticFluxStrength, MagneticFluxStrengthUnits>,
        INormalized<MagneticFluxStrengthUnits>, INormalizable<MagneticFluxStrength>,
        IDimensionAccessible,
        IValueAccessible<MagneticFluxStrengthUnits>,
        IDuplicatable<MagneticFluxStrength>,
        IEquatable<MagneticFluxStrength>,
        IBinaryCompositeTransposable<MagneticFluxStrength, QuotientUnit<Current, CurrentUnits, Length, LengthUnits>>,
        IExponentiable<MagneticFluxStrength, MagneticFluxStrengthUnits>,
        ILinearUnit<MagneticFluxStrength, MagneticFluxStrengthUnits>,
        ICompositeUnit
    {
        private readonly double _Apm = 0;
        public const char SYMBOL = 'H';

        #region PROPERTIES
        public int Dimension => 1;
        public static Dictionary<MagneticFluxStrengthUnits, double> Mapper => new()
        {
            // SI & Metric System
            { MagneticFluxStrengthUnits.Ampere_PerMeter, 1.0 },
            { MagneticFluxStrengthUnits.AmpereTurn_PerMeter, 1.0 },              // 1 At/m = 1 A/m
            { MagneticFluxStrengthUnits.Kiloampere_PerMeter, 1e3 },
            { MagneticFluxStrengthUnits.Milliampere_PerMeter, 1e-3 },

            // CGS & Electromagnetic System
            { MagneticFluxStrengthUnits.Oersted, 79.57747154594767 },             // 1000 / (4 * pi) A/m (~79.5775 A/m)
            { MagneticFluxStrengthUnits.Gilberts_PerCentimeter, 79.57747154594767 },// 1 Gb/cm = 1 Oe (~79.5775 A/m)
            { MagneticFluxStrengthUnits.Statoersted, 2.3855753860477e13 },        // (1000 * c) / (4 * pi * 10^6) A/m (~2.38558 x 10^13 A/m)

            // Imperial & Engineering Units
            { MagneticFluxStrengthUnits.AmpereTurn_PerInch, 39.37007874015748 }, // 1 / 0.0254 A/m (~39.3701 A/m)
            { MagneticFluxStrengthUnits.AmpereTurn_PerFoot, 3.280839895013123 },  // 1 / 0.3048 A/m (~3.28084 A/m)

            // Atomic & Theoretical Units
            { MagneticFluxStrengthUnits.AtomicMagneticFieldStrength, 1.87088647e8 }, // e / (4pi * eps_0 * a_0^2 * c * mu_0) (~1.87089 x 10^8 A/m)
            { MagneticFluxStrengthUnits.PlanckMagneticFieldStrength, 4.0583e59 }    // sqrt(c^9 / (hbar * G^2 * mu_0)) (~4.0583 x 10^59 A/m)
        };

        public static MagneticFluxStrengthUnits BaseScale => MagneticFluxStrengthUnits.Ampere_PerMeter;
        public (double Magnitude, MagneticFluxStrengthUnits Scale, int ScaleOrdinal) Normalized => new(_Apm, BaseScale, (int)BaseScale);

        public (double Magnitude, MagneticFluxStrengthUnits Scale, int ScaleOrdinal) Original { get; private set; } = (0, BaseScale, (int)BaseScale);
        public (double Magnitude, MagneticFluxStrengthUnits Scale, int ScaleOrdinal) Converted { get; private set; } = (0, BaseScale, (int)BaseScale);
        #endregion

        #region CONSTRUCTORS
        public MagneticFluxStrength() { }
        public MagneticFluxStrength(MagneticFluxStrength instance) => this = instance;
        public MagneticFluxStrength(double magnitude)
        {
            Original = (magnitude, BaseScale, (int)BaseScale);
            _Apm = magnitude;
        }
        public MagneticFluxStrength(double magnitude, MagneticFluxStrengthUnits scale)
        {
            Original = (magnitude, scale, (int)scale);
            _Apm = Methods.Scale(magnitude, scale, Mapper) / Mapper[BaseScale];
        }
        #endregion

        #region METHODS
        public static MagneticFluxStrength Initialize() => new();
        public static MagneticFluxStrength Create(MagneticFluxStrength instance) => new(instance);
        public static MagneticFluxStrength Create(double magnitude) => new(magnitude);
        public static MagneticFluxStrength Create(double magnitude, MagneticFluxStrengthUnits scale) => new(magnitude, scale);

        public MagneticFluxStrength Duplicate() => new(this);
        public bool Equals(MagneticFluxStrength other) => _Apm == other._Apm;

        public MagneticFluxStrength Convert(MagneticFluxStrengthUnits toScale)
        {
            double mag = Methods.Scale(Original.Magnitude, Original.Scale, Mapper) / Mapper[toScale];
            Converted = (mag, toScale, (int)toScale);
            return this;
        }
        public double As(MagneticFluxStrengthUnits scale) => Duplicate().Convert(scale).Converted.Magnitude;
        public MagneticFluxStrength Normalize()
        {
            Original = (_Apm, BaseScale, (int)BaseScale);
            return this;
        }

        public QuotientUnit<Current, CurrentUnits, Length, LengthUnits> ToComposite()
            => new QuotientUnit<Current, CurrentUnits, Length, LengthUnits>(_Apm).SetScales(CurrentUnits.Ampere, LengthUnits.Meter);
        public static MagneticFluxStrength FromComposite(QuotientUnit<Current, CurrentUnits, Length, LengthUnits> comp)
            => comp.Original.Scale1 == CurrentUnits.Ampere && comp.Original.Scale2 == LengthUnits.Meter ?
            new(comp.Original.Magnitude) :
            throw new InvalidOperationException(ErrorMsg.COMPOSITE_SCALES_MISMATCH);

        public UnitSquared<MagneticFluxStrength, MagneticFluxStrengthUnits> Squared() => this * this;
        public UnitCubed<MagneticFluxStrength, MagneticFluxStrengthUnits> Cubed() => this * this * this;
        public HyperUnit<MagneticFluxStrength, MagneticFluxStrengthUnits> Pow(int exp)
            => new HyperUnit<MagneticFluxStrength, MagneticFluxStrengthUnits>(Math.Pow(_Apm, exp)).SetDimension(exp);

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
        public static bool operator ==(MagneticFluxStrength l, MagneticFluxStrength r) => l.Equals(r);
        public static bool operator !=(MagneticFluxStrength l, MagneticFluxStrength r) => !l.Equals(r);
        public static bool operator <(MagneticFluxStrength l, MagneticFluxStrength r) => l.Normalized.Magnitude < r.Normalized.Magnitude;
        public static bool operator >(MagneticFluxStrength l, MagneticFluxStrength r) => l.Normalized.Magnitude > r.Normalized.Magnitude;
        public static bool operator <=(MagneticFluxStrength l, MagneticFluxStrength r) => l < r || l == r;
        public static bool operator >=(MagneticFluxStrength l, MagneticFluxStrength r) => l > r || l == r;
        #endregion

        #region SELF ARITHMETIC OPERATORS
        public static MagneticFluxStrength operator +(MagneticFluxStrength l, MagneticFluxStrength r)
            => new(l.Normalized.Magnitude + r.Normalized.Magnitude);
        public static MagneticFluxStrength operator -(MagneticFluxStrength l, MagneticFluxStrength r)
            => new(l.Normalized.Magnitude - r.Normalized.Magnitude);
        public static MagneticFluxStrength operator *(MagneticFluxStrength l, double r)
            => new(l.Normalized.Magnitude * r);
        public static MagneticFluxStrength operator *(double l, MagneticFluxStrength r) => r * l;
        public static MagneticFluxStrength operator /(MagneticFluxStrength l, double r)
            => new(l.Normalized.Magnitude / r);
        #endregion

        #region CROSS-DIMENSIONAL ARITHMETIC OPERATORS
        public static UnitSquared<MagneticFluxStrength, MagneticFluxStrengthUnits> operator *(MagneticFluxStrength l, MagneticFluxStrength r)
            => new(l.Normalized.Magnitude * r.Normalized.Magnitude);

        public static double operator /(MagneticFluxStrength l, MagneticFluxStrength r)
            => l.Normalized.Magnitude / r.Normalized.Magnitude;
        #endregion
    }
}
