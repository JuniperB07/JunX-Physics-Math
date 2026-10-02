using JunX.Physics.BaseUnits;
using JunX.Physics.ClassicalMechanics;
using JunX.Physics.Electromagnetism;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace JunX.Physics.Thermodynamics
{
    public struct Entropy :
        IInitializable<Entropy>, IInitializable<Entropy, double>, IInitializable<Entropy, double, EntropyUnits>,
        IScaleMappable<EntropyUnits>, IScaleConvertible<Entropy, EntropyUnits>,
        INormalized<EntropyUnits>, INormalizable<Entropy>,
        IDimensionAccessible,
        IValueAccessible<EntropyUnits>,
        IDuplicatable<Entropy>,
        IEquatable<Entropy>,
        IBinaryCompositeTransposable<Entropy, QuotientUnit<Energy, EnergyUnits, Temperature, TemperatureUnits>>,
        IExponentiable<Entropy, EntropyUnits>,
        ILinearUnit<Entropy, EntropyUnits>,
        ICompositeUnit
    {
        private readonly double _JpK = 0;
        public const char SYMBOL = 'S';

        #region PROPERTIES
        public int Dimension => 1;
        public static Dictionary<EntropyUnits, double> Mapper => new()
        {
            // SI & Metric System
            { EntropyUnits.Joule_PerKelvin, 1.0 },
            { EntropyUnits.Kilojoule_PerKelvin, 1e3 },
            { EntropyUnits.Megajoule_PerKelvin, 1e6 },
            { EntropyUnits.Millijoule_PerKelvin, 1e-3 },

            // Imperial & Thermal Units
            { EntropyUnits.BTU_PerFahrenheit, 1899.100534716 },  // ISO BTU / (5/9 K) (~1899.10 J/K)
            { EntropyUnits.BTU_PerRankine, 1899.100534716 },     // Identical to BTU/°F (~1899.10 J/K)
            { EntropyUnits.Calorie_PerKelvin, 4.184 },            // Thermochemical calorie (4.184 J/K)
            { EntropyUnits.Kilocalorie_PerKelvin, 4184.0 },       // 1000 thermochemical calories (4184 J/K)
            { EntropyUnits.Entropy, 4.184 },                      // Standard "Entropy Unit" (e.u. = 1 cal/K)

            // Statistical Mechanics, Information Theory & Natural Units
            { EntropyUnits.Nat, 1.380649e-23 },                   // Exact k_B = 1.380649e-23 J/K
            { EntropyUnits.Bit, 9.569925912443015e-24 },          // k_B * ln(2) (~9.56993e-24 J/K)
            { EntropyUnits.Hartley, 3.178943825424754e-23 },      // k_B * ln(10) (~3.17894e-23 J/K)
            { EntropyUnits.PlanckEntropy, 1.380649e-23 }          // Equal to k_B in natural Planck units
        };

        public static EntropyUnits BaseScale => EntropyUnits.Joule_PerKelvin;
        public (double Magnitude, EntropyUnits Scale, int ScaleOrdinal) Normalized => new(_JpK, BaseScale, (int)BaseScale);

        public (double Magnitude, EntropyUnits Scale, int ScaleOrdinal) Original { get; private set; } = (0, BaseScale, (int)BaseScale);
        public (double Magnitude, EntropyUnits Scale, int ScaleOrdinal) Converted { get; private set; } = (0, BaseScale, (int)BaseScale);
        #endregion

        #region CONSTRUCTORS
        public Entropy() { }
        public Entropy(Entropy instance) => this = instance;
        public Entropy(double magnitude)
        {
            Original = (magnitude, BaseScale, (int)BaseScale);
            _JpK = magnitude;
        }
        public Entropy(double magnitude, EntropyUnits scale)
        {
            Original = (magnitude, scale, (int)scale);
            _JpK = Methods.Scale(magnitude, scale, Mapper) / Mapper[BaseScale];
        }
        #endregion

        #region METHODS
        public static Entropy Initialize() => new();
        public static Entropy Create(Entropy instance) => new(instance);
        public static Entropy Create(double magnitude) => new(magnitude);
        public static Entropy Create(double magnitude, EntropyUnits scale) => new(magnitude, scale);

        public Entropy Duplicate() => new(this);
        public bool Equals(Entropy other) => _JpK == other._JpK;

        public Entropy Convert(EntropyUnits toScale)
        {
            double mag = Methods.Scale(Original.Magnitude, Original.Scale, Mapper) / Mapper[toScale];
            Converted = (mag, toScale, (int)toScale);
            return this;
        }
        public double As(EntropyUnits scale) => Duplicate().Convert(scale).Converted.Magnitude;
        public Entropy Normalize()
        {
            Original = (_JpK, BaseScale, (int)BaseScale);
            return this;
        }

        public QuotientUnit<Energy, EnergyUnits, Temperature, TemperatureUnits> ToComposite()
            => new QuotientUnit<Energy, EnergyUnits, Temperature, TemperatureUnits>(_JpK).SetScales(EnergyUnits.Joule, TemperatureUnits.Kelvin);
        public static Entropy FromComposite(QuotientUnit<Energy, EnergyUnits, Temperature, TemperatureUnits> comp)
            => comp.Original.Scale1 == EnergyUnits.Joule && comp.Original.Scale2 == TemperatureUnits.Kelvin ?
            new(comp.Original.Magnitude) :
            throw new InvalidOperationException(ErrorMsg.COMPOSITE_SCALES_MISMATCH);

        public UnitSquared<Entropy, EntropyUnits> Squared() => this * this;
        public UnitCubed<Entropy, EntropyUnits> Cubed() => this * this * this;
        public HyperUnit<Entropy, EntropyUnits> Pow(int exp)
            => new HyperUnit<Entropy, EntropyUnits>(Math.Pow(_JpK, exp)).SetDimension(exp);

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
        public static bool operator ==(Entropy l, Entropy r) => l.Equals(r);
        public static bool operator !=(Entropy l, Entropy r) => !l.Equals(r);
        public static bool operator <(Entropy l, Entropy r) => l.Normalized.Magnitude < r.Normalized.Magnitude;
        public static bool operator >(Entropy l, Entropy r) => l.Normalized.Magnitude > r.Normalized.Magnitude;
        public static bool operator <=(Entropy l, Entropy r) => l < r || l == r;
        public static bool operator >=(Entropy l, Entropy r) => l > r || l == r;
        #endregion

        #region SELF ARITHMETIC OPERATORS
        public static Entropy operator +(Entropy l, Entropy r)
            => new(l.Normalized.Magnitude + r.Normalized.Magnitude);
        public static Entropy operator -(Entropy l, Entropy r)
            => new(l.Normalized.Magnitude - r.Normalized.Magnitude);
        public static Entropy operator *(Entropy l, double r)
            => new(l.Normalized.Magnitude * r);
        public static Entropy operator *(double l, Entropy r) => r * l;
        public static Entropy operator /(Entropy l, double r)
            => new(l.Normalized.Magnitude / r);
        #endregion

        #region CROSS-DIMENSIONAL ARITHMETIC OPERATORS
        public static UnitSquared<Entropy, EntropyUnits> operator *(Entropy l, Entropy r)
            => new(l.Normalized.Magnitude * r.Normalized.Magnitude);

        public static double operator /(Entropy l, Entropy r)
            => l.Normalized.Magnitude / r.Normalized.Magnitude;
        #endregion

    }
}
