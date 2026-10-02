using JunX.Physics.BaseUnits;
using JunX.Physics.ClassicalMechanics;
using JunX.Physics.Kinematics;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata;
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
}
