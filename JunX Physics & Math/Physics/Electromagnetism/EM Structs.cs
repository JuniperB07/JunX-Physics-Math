using JunX.Physics.BaseUnits;
using JunX.Physics.ClassicalMechanics;
using JunX.Physics.Kinematics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
}
