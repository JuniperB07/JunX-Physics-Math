using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace JunX.Mathematics.Geometry
{
    public struct Length :
        IInitializable<Length>, IInitializable<Length, double>, IInitializable<Length, double, LengthUnits>,
        IScaleMappable<LengthUnits>, IScaleConvertible<Length, LengthUnits>,
        INormalized<LengthUnits>, INormalizable<Length>,
        IDimensionAccessible,
        IValueAccessible<LengthUnits>,
        IDuplicatable<Length>,
        IEquatable<Length>,
        IValidatable,
        ILinearUnit<Length, LengthUnits>
    {
        private readonly double _m;

        #region PROPERTIES
        public static Dictionary<LengthUnits, double> Mapper => new()
        {
            { LengthUnits.PlanckLength, 1.616255e-35 },     // ℓₚ = √(ħG / c³)
            { LengthUnits.Yoctometer, 1e-24 },               // ym
            { LengthUnits.Zeptometer, 1e-21 },               // zm
            { LengthUnits.Attometer, 1e-18 },                // am
            { LengthUnits.Femtometer, 1e-15 },               // fm (fermi)
            { LengthUnits.Picometer, 1e-12 },                // pm
            { LengthUnits.Angstrom, 1e-10 },                 // Å (0.1 nm)
            { LengthUnits.Nanometer, 1e-9 },                 // nm
            { LengthUnits.Micrometer, 1e-6 },                // µm (micron)

            { LengthUnits.Millimeter, 1e-3 },                // mm
            { LengthUnits.Centimeter, 1e-2 },                // cm
            { LengthUnits.Meter, 1.0 },                      // m (SI Base Unit)
            { LengthUnits.Kilometer, 1000.0 },               // km

            { LengthUnits.Inch, 0.0254 },                    // in (2.54 cm)
            { LengthUnits.Foot, 0.3048 },                    // ft (12 in)
            { LengthUnits.Yard, 0.9144 },                    // yd (3 ft)
            { LengthUnits.Fathom, 1.8288 },                  // fathom (2 yd / 6 ft)
            { LengthUnits.Mile, 1609.344 },                  // mi (5280 ft)
            { LengthUnits.NauticalMile, 1852.0 },            // nmi (exact international definition)
            { LengthUnits.League, 4828.032 },                // 3 statute miles

            { LengthUnits.LunarDistance, 3.844e8 },          // LD (mean Earth-Moon distance ~384,400 km)
            { LengthUnits.AstronomicalUnit, 149597870700.0 },// AU (exact IAU standard definition)
            { LengthUnits.LightSecond, 299792458.0 },        // distance light travels in 1s (c)
            { LengthUnits.LightYear, 9.4607304725808e15 },   // ly (IAU standard: c * 365.25 Julian days)
            { LengthUnits.Parsec, 3.08567758149137e16 },     // pc (exact IAU resolution B2 definition: 648000/π AU)
            { LengthUnits.Megaparsec, 3.08567758149137e22 }, // Mpc (1,000,000 pc)
            { LengthUnits.Gigaparsec, 3.08567758149137e25 }  // Gpc (1,000,000,000 pc)

        };

        public static LengthUnits BaseScale => LengthUnits.Meter;
        public (double Magnitude, LengthUnits Scale, int ScaleOrdinal) Normalized => (_m, BaseScale, (int)BaseScale);

        public int Dimension => 1;

        public (double Magnitude, LengthUnits Scale, int ScaleOrdinal) Original { get; private set; }
        public (double Magnitude, LengthUnits Scale, int ScaleOrdinal) Converted { get; private set; }
        #endregion

        #region CONSTRUCTORS
        public Length(double magnitude, LengthUnits scale)
        {
            Original = (magnitude, scale, (int)scale);
            Converted = (0, BaseScale, (int)BaseScale);

            _m = Methods.Scale(magnitude, scale, Mapper) / Mapper[BaseScale];
        }
        public Length(double magnitude)
        {
            Original = (magnitude, BaseScale, (int)BaseScale);
            Converted = (0, BaseScale, (int)BaseScale);
            _m = magnitude;
        }
        public Length(Length l)
        {
            this = l;
        }
        public Length()
        {
            Original = (0, BaseScale, (int)BaseScale);
            Converted = Original;
            _m = 0;
        }
        #endregion

        #region METHODS
        public static Length Create(double magnitude, LengthUnits scale) => new(magnitude, scale);
        public static Length Create(double magnitude) => new(magnitude);
        public static Length Create(Length length) => new(length);
        public static Length Initialize() => new();

        public Length Normalize()
        {
            if (Original.Scale == BaseScale)
                return this;

            double mag = Methods.Scale(Original.Magnitude, Original.Scale, Mapper) / Mapper[BaseScale];
            Original = (mag, BaseScale, (int)BaseScale);
            return this;
        }

        public Length Convert(LengthUnits toScale)
        {
            double res = Methods.Scale(Original.Magnitude, Original.Scale, Mapper) / Mapper[toScale];
            Converted = (res, toScale, (int)toScale);
            return this;
        }
        public double As(LengthUnits scale) => Duplicate().Convert(scale).Converted.Magnitude;

        public Length Duplicate() => new(this);
        public bool Equals(Length l) => Normalized.Magnitude == l.Normalized.Magnitude;
        public bool IsValid() => Original.Magnitude >= 0;
        #endregion

        #region OVERRIDES
        [Obsolete]
        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return base.Equals(obj);
        }
        [Obsolete]
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region CONDITIONAL OPERATORS
        public static bool operator ==(Length l, Length r) => l.Equals(r);
        public static bool operator !=(Length l, Length r) => !l.Equals(r);
        public static bool operator <(Length l, Length r)
            => l.Normalized.Magnitude < r.Normalized.Magnitude;
        public static bool operator >(Length l, Length r)
            => l.Normalized.Magnitude > r.Normalized.Magnitude;
        public static bool operator <=(Length l, Length r) => l < r || l == r;
        public static bool operator >=(Length l, Length r) => l > r || l == r;
        #endregion

        #region SELF ARITHMETIC OPERATORS
        public static Length operator +(Length l, Length r)
            => new(l.Normalized.Magnitude + r.Normalized.Magnitude);
        public static Length operator -(Length l, Length r) 
            => new(l.Normalized.Magnitude -  r.Normalized.Magnitude);
        public static Length operator *(Length l, double r)
            => new(l.Normalized.Magnitude * r);
        public static Length operator *(double l, Length r) => r * l;
        public static Length operator /(Length l, double r)
            => new(l.Normalized.Magnitude / r);
        #endregion

        #region CROSS-DIMENSIONAL OPERATORS
        public static double operator /(Length l, Length r)
            => l.Normalized.Magnitude / r.Normalized.Magnitude;
        #endregion
    }

    public struct Area :
        IInitializable<Area>, IInitializable<Area, double>, IInitializable<Area, double, AreaUnits>,
        IScaleMappable<AreaUnits>, IScaleConvertible<Area, AreaUnits>,
        INormalized<AreaUnits>, INormalizable<Area>,
        IDimensionAccessible,
        IValueAccessible<AreaUnits>,
        IDuplicatable<Area>,
        IEquatable<Area>,
        IValidatable,
        ILinearUnit<Area, AreaUnits>
    {
        private readonly double _m2;

        #region PROPERTIES
        public int Dimension => 2;
        public static Dictionary<AreaUnits, double> Mapper => new()
        {
            { AreaUnits.SquarePlanckArea, 2.61223e-70 },      // ℓₚ² = ħG / c³ (~2.612 × 10⁻⁷⁰ m²)
            { AreaUnits.SquareAttometer, 1e-36 },              // am²
            { AreaUnits.SquareFemtometer, 1e-30 },             // fm²
            { AreaUnits.Barn, 1e-28 },                         // b (100 fm², standard nuclear cross-section unit)
            { AreaUnits.SquarePicometer, 1e-24 },              // pm²
            { AreaUnits.SquareNanometer, 1e-18 },              // nm²
            { AreaUnits.SquareMicrometer, 1e-12 },             // µm²

            { AreaUnits.SquareMillimeter, 1e-6 },              // mm²
            { AreaUnits.SquareCentimeter, 1e-4 },              // cm²
            { AreaUnits.SquareMeter, 1.0 },                    // m² (SI Base Unit)
            { AreaUnits.Are, 100.0 },                          // a (100 m²)
            { AreaUnits.Decare, 1000.0 },                      // daa (1,000 m² / 10 ares)
            { AreaUnits.Hectare, 10000.0 },                    // ha (10,000 m² / 100 ares)
            { AreaUnits.SquareKilometer, 1e6 },                // km²

            { AreaUnits.CircularMil, 5.06707479097497e-10 },   // cmil (area of circle with d = 1 mil / 0.001 in)
            { AreaUnits.SquareInch, 0.00064516 },              // in² (exact: 0.0254 m)²
            { AreaUnits.SquareFoot, 0.09290304 },              // ft² (exact: 0.3048 m)²
            { AreaUnits.SquareYard, 0.83612736 },              // yd² (exact: 0.9144 m)²
            { AreaUnits.SquareAcre, 4046.8564224 },            // ac (exact: 4,840 yd²)
            { AreaUnits.SquareMile, 2589988.110336 },          // mi² (exact: 1,760 yd)²

            { AreaUnits.SquareLightSecond, 8.98755178736817e16 }, // (299,792,458 m)²
            { AreaUnits.SquareAstronomicalUnit, 2.237952291797e22 }, // AU² (149,597,870,700 m)²
            { AreaUnits.SquareLightYear, 8.9505431526364e31 },    // ly² (~9.4607 × 10¹⁵ m)²
            { AreaUnits.SquareParsec, 9.521406180373e32 }         // pc² (~3.0857 × 10¹⁶ m)²
        };

        public static AreaUnits BaseScale => AreaUnits.SquareMeter;
        public (double Magnitude, AreaUnits Scale, int ScaleOrdinal) Normalized => (_m2, BaseScale, (int)BaseScale);

        public (double Magnitude, AreaUnits Scale, int ScaleOrdinal) Original { get; private set; }
        public (double Magnitude, AreaUnits Scale, int ScaleOrdinal) Converted { get; private set; }
        #endregion

        #region CONSTRUCTORS
        public Area()
        {
            Original = (0, BaseScale, (int)BaseScale);
            Converted = (0, BaseScale, (int)BaseScale);
            _m2 = 0;
        }
        public Area(Area instance)
        {
            this = instance;
        }
        public Area(double magnitude)
        {
            Original = (magnitude, BaseScale, (int)BaseScale);
            Converted = (0, BaseScale, (int)BaseScale);
            _m2 = magnitude;
        }
        public Area(double magnitude, AreaUnits scale)
        {
            Original = (magnitude, scale, (int)scale);
            Converted = (0, BaseScale, (int)BaseScale);
            _m2 = Methods.Scale(magnitude, scale, Mapper) / Mapper[BaseScale];
        }
        #endregion

        #region METHODS
        public static Area Initialize() => new();
        public static Area Create(Area instance) => new(instance);
        public static Area Create(double magnitude) => new(magnitude);
        public static Area Create(double magnitude, AreaUnits scale) => new(magnitude, scale);

        public Area Normalize()
        {
            if (Original.Scale == BaseScale)
                return this;

            Original = (Normalized.Magnitude, Normalized.Scale, Normalized.ScaleOrdinal);
            return this;
        }

        public Area Convert(AreaUnits toScale)
        {
            if(toScale == Original.Scale)
            {
                Converted = (Original.Magnitude, Original.Scale, Original.ScaleOrdinal);
                return this;
            }

            double mag = Methods.Scale(Original.Magnitude, Original.Scale, Mapper) / Mapper[toScale];
            Converted = (mag, toScale, (int)toScale);
            return this;
        }
        public double As(AreaUnits scale) => Duplicate().Convert(scale).Converted.Magnitude;

        public Area Duplicate() => new(this);
        public bool Equals(Area other) => Normalized.Magnitude == other.Normalized.Magnitude;
        public bool IsValid() => Original.Magnitude >= 0;
        #endregion
    }
}
