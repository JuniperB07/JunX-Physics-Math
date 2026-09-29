using JunX.Physics.BaseUnits;
using JunX.Physics.Kinematics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;
using System.Text;

namespace JunX.Mathematics.Geometry
{
    /// <summary>
    /// Represents a one-dimensional spatial length unit structure supporting scale conversions, normalization, geometric derivations, and higher-order dimensional exponentiation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="Length"/> implements foundational linear contracts including <see cref="ILinearUnit{TSelf, TEnum}"/>, <see cref="INormalizable{TSelf}"/>, 
    /// and <see cref="IExponentiable{TArea, TVolume, THyper}"/> to encapsulate spatial measurements ranging across subatomic, macroscopic, and cosmological scales.
    /// </para>
    /// <para>
    /// It normalizes values relative to the SI base unit (<see cref="LengthUnits.Meter"/>) and provides evaluation pipelines for unit conversions, validation, 
    /// equality assertions, geometric derivations (such as delta displacements, radius, and diameter calculations), and dimensional operations into <see cref="Area"/>, <see cref="Volume"/>, and hyper-dimensional structures.
    /// </para>
    /// </remarks>
    public struct Length :
        IInitializable<Length>, IInitializable<Length, double>, IInitializable<Length, double, LengthUnits>,
        IScaleMappable<LengthUnits>, IScaleConvertible<Length, LengthUnits>,
        INormalized<LengthUnits>, INormalizable<Length>,
        IDimensionAccessible,
        IValueAccessible<LengthUnits>,
        IDuplicatable<Length>,
        IEquatable<Length>,
        IValidatable,
        IExponentiable<Area, Volume, HyperUnit<Length, LengthUnits>>,
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

        public Area Squared() => this * this;
        public Volume Cubed() => this * this * this;
        public HyperUnit<Length, LengthUnits> Pow(int exp)
            => new HyperUnit<Length, LengthUnits>(Math.Pow(Normalized.Magnitude, exp)).SetDimension(Dimension * exp);

        public static Length Abs(Length a)
        {
            if (a.Normalized.Magnitude >= 0)
                return new(a.Normalized.Magnitude);

            return new(a.Normalized.Magnitude * -1);
        }


        public HyperUnit<Length, LengthUnits> ToHyperUnit()
            => new HyperUnit<Length, LengthUnits>(Original.Magnitude, Original.Scale).SetDimension(Dimension);

        public Length Duplicate() => new(this);
        public bool Equals(Length l) => Normalized.Magnitude == l.Normalized.Magnitude;
        public bool IsValid() => Original.Magnitude >= 0;
        #endregion

        #region DERIVATIONS
        public static Length Derive(Velocity velocity, Time time) => velocity * time;
        public static Length Derive(Acceleration acceleration, UnitSquared<Time, TimeUnits> timeSquared) => acceleration * timeSquared;

        public static Length DeltaX(Length initial, Length final) => final - initial;
        public static Length Diameter(Length radius) => radius * 2.0;
        public static Length Radius(Length diameter) => diameter / 2.0;
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
        public static Area operator *(Length l, Length r)
            => new(l.Normalized.Magnitude * r.Normalized.Magnitude);
        public static Volume operator *(Length l, Area r)
            => new(l.Normalized.Magnitude * r.Normalized.Magnitude);
        public static HyperUnit<Length, LengthUnits> operator *(Length l, Volume r)
            => new HyperUnit<Length, LengthUnits>(l.Normalized.Magnitude * r.Normalized.Magnitude).SetDimension(l.Dimension + r.Dimension);

        public static double operator /(Length l, Length r)
            => l.Normalized.Magnitude / r.Normalized.Magnitude;
        #endregion

        #region CROSS-UNIT ARITHMETIC OPERATORS
        public static Velocity operator /(Length l, Time r)
            => new(l.Normalized.Magnitude / r.Normalized.Magnitude);

        public static Acceleration operator /(Length l, UnitSquared<Time, TimeUnits> r)
            => new(l.Normalized.Magnitude / r.Normalized.Magnitude);
        public static Acceleration operator /(UnitSquared<Velocity, VelocityUnits> l, Length r)
            => new(l.Normalized.Magnitude / r.Normalized.Magnitude);

        public static Time operator /(Length l, Velocity r)
            => new(l.Normalized.Magnitude / r.Normalized.Magnitude);

        public static UnitSquared<Time, TimeUnits> operator /(Length l, Acceleration r)
            => new(l.Normalized.Magnitude / r.Normalized.Magnitude);
        public static UnitSquared<Velocity, VelocityUnits> operator *(Length l, Acceleration r)
            => new(l.Normalized.Magnitude * r.Normalized.Magnitude);

        public static Velocity operator *(Length l, AngularVelocity r)
            => new(l.Normalized.Magnitude * r.Normalized.Magnitude);
        #endregion
    }

    /// <summary>
    /// Represents a two-dimensional spatial area unit structure supporting scale conversions, normalization, geometric derivations, and cross-dimensional operations.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="Area"/> implements core spatial contracts including <see cref="IScaleConvertible{TSelf, TEnum}"/>, <see cref="INormalizable{TSelf}"/>, 
    /// <see cref="ISquareRootable{TRoot}"/>, and <see cref="IExponentiable{THyper}"/> to manage planar space measurements across subatomic, macroscopic, and astronomical scales.
    /// </para>
    /// <para>
    /// It normalizes values relative to the SI base unit (<see cref="AreaUnits.SquareMeter"/>) and provides static evaluation factories for 
    /// geometric planar surfaces (e.g., polygons, circles, ellipses) and three-dimensional surface area derivations (e.g., spheres, cones, cylinders, tori). 
    /// Additionally, it handles algebraic dimensional reduction to <see cref="Length"/> via square roots and dimensional scaling into higher-order structures like <see cref="Volume"/>.
    /// </para>
    /// </remarks>
    public struct Area :
        IInitializable<Area>, IInitializable<Area, double>, IInitializable<Area, double, AreaUnits>,
        IScaleMappable<AreaUnits>, IScaleConvertible<Area, AreaUnits>,
        INormalized<AreaUnits>, INormalizable<Area>,
        IDimensionAccessible,
        IValueAccessible<AreaUnits>,
        IDuplicatable<Area>,
        IEquatable<Area>,
        IValidatable,
        IExponentiable<HyperUnit<Length, LengthUnits>>,
        ISquareRootable<Length>,
        IDimensionalUnit
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

        public HyperUnit<Length, LengthUnits> Squared() => this * this;
        public HyperUnit<Length, LengthUnits> Cubed() => (this * this) * ToUnitSquared();
        public HyperUnit<Length, LengthUnits> Pow(int exp)
            => new HyperUnit<Length, LengthUnits>(Math.Pow(Normalized.Magnitude, exp)).SetDimension(Dimension * exp);

        public Length Sqrt() => new(Math.Sqrt(Normalized.Magnitude));
        public static Area Abs(Area a)
        {
            if (a.Normalized.Magnitude >= 0)
                return new(a.Normalized.Magnitude);

            return new(a.Normalized.Magnitude * -1);
        }

        public UnitSquared<Length, LengthUnits> ToUnitSquared() => new(Normalized.Magnitude, LengthUnits.Meter);

        public static Area FromUnitSquared(UnitSquared<Length, LengthUnits> unit)
            => new(unit.Normalized.Magnitude, BaseScale);

        public Area Duplicate() => new(this);
        public bool Equals(Area other) => Normalized.Magnitude == other.Normalized.Magnitude;
        public bool IsValid() => Original.Magnitude >= 0;

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
        public static Area Square(Length side) => side.Squared();
        public static Area Rectangle(Length length, Length width) => length * width;
        public static Area Triangle(Length Base, Length height) => 0.5 * Base * height;
        public static Area Triangle(Length sideA, Length sideB, Length sideC)
        {
            Length s = (sideA + sideB + sideC) / 2.0;
            HyperUnit<Length, LengthUnits> rad = s * (s - sideA) * (s - sideB) * (s - sideC);
            return FromUnitSquared(rad.Sqrt().ToUnitSquared());
        }
        public static Area EquilateralTriangle(Length side)
            => (Math.Sqrt(3.0) / 4.0) * side.Squared();
        public static Area Parallelogram(Length Base, Length height) => Base * height;
        public static Area Trapezoid(Length sideA, Length sideB, Length height)
            => ((sideA + sideB) / 2.0) * height;
        public static Area Rhombus(Length diagonal1, Length diagonal2)
            => 0.5 * diagonal1 * diagonal2;
        public static Area Circle(Length radius) => Math.PI * radius.Squared();
        public static Area CircularSector(Length radius, Angle theta)
            => 0.5 * radius.Squared() * theta.Normalized.Magnitude;
        public static Area Ellipse(Length semiMinorAxis, Length semiMajorAxis)
            => Math.PI * semiMajorAxis * semiMinorAxis;
        public static Area RegularPolygon(double sideCount, Length side)
            => (sideCount * side.Squared()) / (4.0 * Math.Tan(Math.PI / sideCount));
        public static Area IrregularPolygon(Length[] sideX, Length[] sideY)
        {
            if (sideX.Length != sideY.Length)
                throw new InvalidOperationException(ErrorMsg.ARRAY_LENGTH_MISMATCH);

            if (sideX.Length < 3)
                throw new InvalidOperationException(ErrorMsg.Insufficient_Array_Length(3));

            Area sumUp = new();
            Area sumDown = new();
            for(int i = 0; i<sideX.Length; i++)
            {
                if (i == sideX.Length - 1)
                {
                    sumDown += sideX[i] * sideY[0];
                    sumUp += sideX[0] * sideY[i];
                    break;
                }

                sumDown += sideX[i] * sideY[i + 1];
                sumUp += sideX[i + 1] * sideY[i];
            }

            return 0.5 * Abs(sumDown - sumUp);
        }

        public static Area Sphere(Length radius) => 4.0 * Math.PI * radius.Squared();
        public static Area Cube(Length side) => 6.0 * side.Squared();
        public static Area RectangularPrism(Length length, Length width, Length height)
            => 2.0 * ((length * width) + (length * height) + (width * height));
        public static Area Cylinder_Total(Length radius, Length height)
            => 2.0 * Math.PI * radius * (radius + height);
        public static Area Cylinder_Lateral(Length radius, Length height) => 2.0 * Math.PI * radius * height;
        public static Area Cone_Total(Length radius, Length height)
            => Math.PI * radius * (radius + (radius.Squared() + height.Squared()).Sqrt());
        public static Area Cone_Lateral(Length radius, Length height)
            => Math.PI * radius * (radius.Squared() + height.Squared()).Sqrt();
        public static Area Torus(Length majorRadius, Length minorRadius)
            => 4.0 * Math.Pow(Math.PI, 2) * majorRadius * minorRadius;
        #endregion

        #region CONDITIONAL OPERATORS
        public static bool operator ==(Area l, Area r) => l.Equals(r);
        public static bool operator !=(Area l, Area r) => !l.Equals(r);
        public static bool operator <(Area l, Area r) => l.Normalized.Magnitude < r.Normalized.Magnitude;
        public static bool operator >(Area l, Area r) => l.Normalized.Magnitude > r.Normalized.Magnitude;
        public static bool operator <=(Area l, Area r) => l < r || l == r;
        public static bool operator >=(Area l, Area r) => l > r || l == r;
        #endregion

        #region SELF ARITHMETIC OPERATORS
        public static Area operator +(Area l, Area r)
            => new(l.Normalized.Magnitude + r.Normalized.Magnitude);
        public static Area operator -(Area l, Area r)
            => new(l.Normalized.Magnitude - r.Normalized.Magnitude);
        public static Area operator *(Area l, double r)
            => new(l.Normalized.Magnitude * r);
        public static Area operator *(double l, Area r) => r * l;
        public static Area operator /(Area l, double r)
            => new(l.Normalized.Magnitude / r);
        #endregion

        #region CROSS DIMENSIONAL ARITHMETIC OPERATORS
        public static HyperUnit<Length, LengthUnits> operator *(Area l, Area r)
            => new HyperUnit<Length, LengthUnits>(l.Normalized.Magnitude * r.Normalized.Magnitude).SetDimension(l.Dimension + r.Dimension);
        public static Volume operator *(Area l, Length r)
            => new(l.Normalized.Magnitude * r.Normalized.Magnitude);
        public static HyperUnit<Length, LengthUnits> operator *(Area l, Volume r)
            => new HyperUnit<Length, LengthUnits>(l.Normalized.Magnitude * r.Normalized.Magnitude).SetDimension(l.Dimension + r.Dimension);

        public static double operator /(Area l, Area r)
            => l.Normalized.Magnitude / r.Normalized.Magnitude;
        public static Length operator /(Area l, Length r)
            => new(l.Normalized.Magnitude / r.Normalized.Magnitude);
        #endregion
    }

    /// <summary>
    /// Represents a three-dimensional spatial volume unit structure supporting scale conversions, normalization, geometric derivations, and cross-dimensional operations.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="Volume"/> implements foundational volumetric contracts including <see cref="IScaleConvertible{TSelf, TEnum}"/>, <see cref="INormalizable{TSelf}"/>, 
    /// <see cref="ICubeRootable{TRoot}"/>, and <see cref="IExponentiable{THyper}"/> to manage 3D spatial capacity across subatomic, culinary, imperial, metric, and astronomical scales.
    /// </para>
    /// <para>
    /// It normalizes values relative to the SI base unit (<see cref="VolumeUnits.CubicMeter"/>) and provides static evaluation factories for 3D geometric shapes 
    /// (e.g., spheres, cylinders, cones, pyramids, ellipsoids, tori, and frustums). Furthermore, it handles dimensional reduction to <see cref="Area"/> and <see cref="Length"/> 
    /// via division or cube roots, as well as higher-order dimensional scaling via hyper-unit exponentiation.
    /// </para>
    /// </remarks>
    public struct Volume :
        IInitializable<Volume>, IInitializable<Volume, double>, IInitializable<Volume, double, VolumeUnits>,
        IScaleMappable<VolumeUnits>, IScaleConvertible<Volume, VolumeUnits>,
        INormalized<VolumeUnits>, INormalizable<Volume>,
        IDimensionAccessible,
        IValueAccessible<VolumeUnits>,
        IDuplicatable<Volume>,
        IEquatable<Volume>,
        IValidatable,
        IExponentiable<HyperUnit<Length, LengthUnits>>,
        ICubeRootable<Length>,
        IDimensionalUnit
    {
        private readonly double _m3;

        #region PROPERTIES
        public int Dimension => 3;
        public static Dictionary<VolumeUnits, double> Mapper => new()
        {
            { VolumeUnits.CubicMillimeter, 1e-9 },
            { VolumeUnits.CubicCentimeter, 1e-6 },
            { VolumeUnits.Milliliter, 1e-6 },
            { VolumeUnits.Liter, 1e-3 },
            { VolumeUnits.CubicMeter, 1.0 },
            { VolumeUnits.CubicKilometer, 1e9 },
            { VolumeUnits.Drop, 0.05 / 1_000_000.0 },
            { VolumeUnits.Minim, 0.000061611519921875 / 1000.0 },
            { VolumeUnits.FluidDram, 0.0036966911953125 / 1000.0 },
            { VolumeUnits.Teaspoon, 0.00492892159375 / 1000.0 },
            { VolumeUnits.Tablespoon, 0.01478676478125 / 1000.0 },
            { VolumeUnits.FluidOunce, 0.0295735295625 / 1000.0 },
            { VolumeUnits.Shot, 0.04436029434375 / 1000.0 },
            { VolumeUnits.Gill, 0.11829411825 / 1000.0 },
            { VolumeUnits.Cup, 0.2365882365 / 1000.0 },
            { VolumeUnits.Pint, 0.473176473 / 1000.0 },
            { VolumeUnits.Quart, 0.946352946 / 1000.0 },
            { VolumeUnits.Gallon, 3.785411784 / 1000.0 },
            { VolumeUnits.DryPint, 0.5506104713575 / 1000.0 },
            { VolumeUnits.DryQuart, 1.101220942715 / 1000.0 },
            { VolumeUnits.Peck, 8.80976754172 / 1000.0 },
            { VolumeUnits.Bushel, 35.23907016688 / 1000.0 },
            { VolumeUnits.CubicInch, 0.000016387064 },
            { VolumeUnits.CubicFoot, 0.028316846592 },
            { VolumeUnits.CubicYard, 0.764554857984 },
            { VolumeUnits.PlanckVolume, 4.2217e-105 },
            { VolumeUnits.CubicLightSecond, 2.694400241737398e25 },
            { VolumeUnits.CubicAstronomicalUnit, 3.347928976238074e33 },
            { VolumeUnits.CubicLightYear, 8.467324086940843e47 },
            { VolumeUnits.CubicParsec, 2.937998947230494e50 }
        };

        public static VolumeUnits BaseScale => VolumeUnits.CubicMeter;
        public (double Magnitude, VolumeUnits Scale, int ScaleOrdinal) Normalized => (_m3, BaseScale, (int)BaseScale);

        public (double Magnitude, VolumeUnits Scale, int ScaleOrdinal) Original { get; private set; }
        public (double Magnitude, VolumeUnits Scale, int ScaleOrdinal) Converted { get; private set; }
        #endregion

        #region CONSTRUCTORS
        public Volume()
        {
            Original = (0, BaseScale, (int)BaseScale);
            Converted = (0, BaseScale, (int)BaseScale);
            _m3 = 0;
        }
        public Volume(Volume instance)
        {
            this = instance;
        }
        public Volume(double magnitude)
        {
            Original = (magnitude, BaseScale, (int)BaseScale);
            Converted = (0, BaseScale, (int)BaseScale);
            _m3 = magnitude;
        }
        public Volume(double magnitude, VolumeUnits scale)
        {
            Original = (magnitude, scale, (int)scale);
            Converted = (0, BaseScale, (int)BaseScale);
            _m3 = Methods.Scale(magnitude, scale, Mapper) / Mapper[BaseScale];
        }
        #endregion

        #region METHODS
        public static Volume Initialize() => new();
        public static Volume Create(Volume instance) => new Volume(instance);
        public static Volume Create(double magnitude) => new(magnitude);
        public static Volume Create(double magnitude, VolumeUnits scale) => new(magnitude, scale);

        public Volume Duplicate() => new(this);
        public bool Equals(Volume other) => Normalized.Magnitude == other.Normalized.Magnitude;
        public bool IsValid() => Original.Magnitude >= 0;

        public Volume Normalize()
        {
            if (Original.Scale == BaseScale)
                return this;

            Original = (Normalized.Magnitude, Normalized.Scale, Normalized.ScaleOrdinal);
            return this;
        }
        public Volume Convert(VolumeUnits toScale)
        {
            if (Original.Scale == toScale)
            {
                Converted = (Original.Magnitude, Original.Scale, Original.ScaleOrdinal);
                return this;
            }

            double mag = Methods.Scale(Original.Magnitude, Original.Scale, Mapper) / Mapper[toScale];
            Converted = (mag, toScale, (int)toScale);
            return this;
        }
        public double As(VolumeUnits scale) => Duplicate().Convert(scale).Converted.Magnitude;

        public HyperUnit<Length, LengthUnits> Squared() => ToUnitCubed() * ToUnitCubed();
        public HyperUnit<Length, LengthUnits> Cubed() => ToUnitCubed() * ToUnitCubed() * ToUnitCubed();
        public HyperUnit<Length, LengthUnits> Pow(int exp)
            => new HyperUnit<Length, LengthUnits>(Math.Pow(Normalized.Magnitude, exp)).SetDimension(Dimension * exp);

        public Length CubeRt() => new(Radical.Root(Normalized.Magnitude, 3));
        public static Volume Abs(Volume a)
        {
            if (a.Normalized.Magnitude >= 0)
                return new(a.Normalized.Magnitude);

            return new(a.Normalized.Magnitude * -1);
        }


        public UnitCubed<Length, LengthUnits> ToUnitCubed() => new(Normalized.Magnitude, LengthUnits.Meter);

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
        public static Volume Cube(Length side) => side.Cubed();
        public static Volume RectangularVolume(Length length, Length width, Length height)
            => length * width * height;
        public static Volume GeneralPrism(Area Base, Length height) => Base * height;
        public static Volume Sphere(Length radius)
            => (4.0 / 3.0) * Math.PI * radius.Cubed();
        public static Volume RightCircularCylinder(Length radius, Length height)
            => Math.PI * radius.Squared() * height;
        public static Volume RightCircularCone(Length radius, Length height)
            => (1.0 / 3.0) * Math.PI * radius.Squared() * height;
        public static Volume Pyramid(Area Base, Length height)
            => (1.0 / 3.0) * Base * height;
        public static Volume RegularTetrahedron(Length edge)
            => edge.Cubed() / (6.0 * Math.Sqrt(2));
        public static Volume Ellipsoid(Length semiA, Length semiB, Length semiC)
            => (4.0 / 3.0) * Math.PI * semiA * semiB * semiC;
        public static Volume Torus(Length majorRadius, Length minorRadius)
            => 2.0 * Math.Pow(Math.PI, 2) * majorRadius * minorRadius.Squared();
        public static Volume Cone_Frustum(Length radius1, Length radius2, Length height)
            => (1.0 / 3.0) * Math.PI * height * (radius1.Squared() + radius2.Squared() + (radius1 * radius2));
        public static Volume Pyramid_Frustum(Area Base1, Area Base2, Length height)
            => (1.0 / 3.0) * height * (Base1 + Base2 + Area.FromUnitSquared((Base1 * Base2).Sqrt().ToUnitSquared()));
        public static Volume SphericalCap(Length baseRadius, Length height)
            => (1.0 / 6.0) * Math.PI * height * ((3 * baseRadius.Squared()) + height.Squared());
        #endregion

        #region CONDITIONAL OPERATORS
        public static bool operator ==(Volume l, Volume r) => l.Equals(r);
        public static bool operator !=(Volume l, Volume r) => !(l == r);
        public static bool operator <(Volume l, Volume r)
            => l.Normalized.Magnitude < r.Normalized.Magnitude;
        public static bool operator >(Volume l, Volume r)
            => l.Normalized.Magnitude > r.Normalized.Magnitude;
        public static bool operator <=(Volume l, Volume r) => l < r || l == r;
        public static bool operator >=(Volume l, Volume r) => l > r || l == r;
        #endregion

        #region SELF ARITHMETIC OPERATORS
        public static Volume operator +(Volume l, Volume r)
            => new(l.Normalized.Magnitude + r.Normalized.Magnitude);
        public static Volume operator -(Volume l, Volume r)
            => new(l.Normalized.Magnitude - r.Normalized.Magnitude);
        public static Volume operator *(Volume l, double r)
            => new(l.Normalized.Magnitude * r);
        public static Volume operator *(double l, Volume r) => r * l;
        public static Volume operator /(Volume l, double r)
            => new(l.Normalized.Magnitude / r);
        #endregion

        #region CROSS DIMENSIONAL ARITHMETIC OPERATORS
        public static HyperUnit<Length, LengthUnits> operator *(Volume l, Length r)
            => new HyperUnit<Length, LengthUnits>(l.Normalized.Magnitude * r.Normalized.Magnitude).SetDimension(l.Dimension + r.Dimension);
        public static HyperUnit<Length, LengthUnits> operator *(Volume l, Area r)
            => new HyperUnit<Length, LengthUnits>(l.Normalized.Magnitude * r.Normalized.Magnitude).SetDimension(l.Dimension + r.Dimension);
        public static HyperUnit<Length, LengthUnits> operator *(Volume l, Volume r)
            => new HyperUnit<Length, LengthUnits>(l.Normalized.Magnitude * r.Normalized.Magnitude).SetDimension(l.Dimension + r.Dimension);

        public static Area operator /(Volume l, Length r)
            => new(l.Normalized.Magnitude / r.Normalized.Magnitude);
        public static Length operator /(Volume l, Area r)
            => new(l.Normalized.Magnitude / r.Normalized.Magnitude);
        public static double operator /(Volume l, Volume r)
            => l.Normalized.Magnitude / r.Normalized.Magnitude;
        #endregion
    }

    /// <summary>
    /// Represents a dimensionless angular measurement structure supporting unit conversions, normalization, and trigonometric angular operations.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="Angle"/> implements core unit abstractions including <see cref="IScaleConvertible{TSelf, TEnum}"/>, <see cref="INormalizable{TSelf}"/>, 
    /// and <see cref="IDimensionAccessible"/> to represent angular rotation and arc measures across angular units (degrees and radians).
    /// </para>
    /// <para>
    /// It maintains a spatial dimension of 0 and normalizes internal values relative to the SI base unit (<see cref="AngleUnits.Radians"/>). 
    /// The structure facilitates scale conversions, equality checks, magnitude comparison operators, and basic additive/subtractive angular arithmetic.
    /// </para>
    /// </remarks>
    public struct Angle :
        IInitializable<Angle>, IInitializable<Angle, double>, IInitializable<Angle, double, AngleUnits>,
        IScaleConvertible<Angle, AngleUnits>,
        INormalized<AngleUnits>, INormalizable<Angle>,
        IDimensionAccessible,
        IValueAccessible<AngleUnits>,
        IDuplicatable<Angle>,
        IEquatable<Angle>
    {
        private readonly double _rad;
        public const char SYMBOL = 'θ';

        #region PROPERTIES
        public int Dimension => 0;

        public static AngleUnits BaseScale => AngleUnits.Radians;
        public (double Magnitude, AngleUnits Scale, int ScaleOrdinal) Normalized => (_rad, BaseScale, (int)BaseScale);

        public (double Magnitude, AngleUnits Scale, int ScaleOrdinal) Original { get; private set; }
        public (double Magnitude, AngleUnits Scale, int ScaleOrdinal) Converted { get; private set; }
        #endregion

        #region CONSTRUCTORS
        public Angle()
        {
            Original = (0, BaseScale, (int)BaseScale);
            Converted = (0, BaseScale, (int)BaseScale);
            _rad = 0;
        }
        public Angle(Angle instance)
        {
            this = instance;
        }
        public Angle(double magnitude)
        {
            Original = (magnitude, BaseScale, (int)BaseScale);
            Converted = (0, BaseScale, (int)BaseScale);
            _rad = magnitude;
        }
        public Angle(double magnitude, AngleUnits scale)
        {
            Original = (magnitude, scale, (int)scale);
            Converted = (0, BaseScale, (int)BaseScale);

            if (scale == AngleUnits.Radians)
                _rad = magnitude;
            else
                _rad = magnitude * (Math.PI / 180.0);
        }
        #endregion

        #region METHODS
        public static Angle Initialize() => new();
        public static Angle Create(Angle instance) => new(instance);
        public static Angle Create(double magnitude) => new(magnitude);
        public static Angle Create(double magnitude, AngleUnits scale) => new(magnitude, scale);

        public Angle Duplicate() => new(this);
        public bool Equals(Angle other) => Normalized.Magnitude == other.Normalized.Magnitude;

        public Angle Convert(AngleUnits toScale)
        {
            if(Original.Scale == toScale) // Radians-Radians & Degree-Degree
            {
                Converted = (Original.Magnitude, Original.Scale, Original.ScaleOrdinal);
                return this;
            }

            double mag;
            if (Original.Scale == AngleUnits.Radians && toScale == AngleUnits.Degree) // Radians - Degree
                mag = Original.Magnitude * (Math.PI / 180.0);
            else // Degree - Radians
                mag = Original.Magnitude * (180.0 / Math.PI);

            Converted = (mag, toScale, (int)toScale);
            return this;
        }
        public double As(AngleUnits scale) => Duplicate().Convert(scale).Converted.Magnitude;
        public Angle Normalize()
        {
            Original = (Normalized.Magnitude, Normalized.Scale, Normalized.ScaleOrdinal);
            return this;
        }

        public static Angle Abs(Angle a)
        {
            if (a.Normalized.Magnitude >= 0)
                return new(a.Normalized.Magnitude);

            return new(a.Normalized.Magnitude * -1);
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
        public static Angle Delta(Angle initial, Angle final) => final - initial;
        #endregion

        #region CONDITIONAL OPERATORS
        public static bool operator ==(Angle l, Angle r) => l.Equals(r);
        public static bool operator !=(Angle l, Angle r) => !l.Equals(r);
        public static bool operator <(Angle l, Angle r)
            => l.Normalized.Magnitude < r.Normalized.Magnitude;
        public static bool operator >(Angle l, Angle r)
            => l.Normalized.Magnitude > r.Normalized.Magnitude;
        public static bool operator <=(Angle l, Angle r) => l < r || l == r;
        public static bool operator >=(Angle l, Angle r) => l > r || l == r;
        #endregion

        #region ARITHMETIC OPERATORS
        public static Angle operator +(Angle l, Angle r)
            => new(l.Normalized.Magnitude + r.Normalized.Magnitude);
        public static Angle operator -(Angle l, Angle r)
            => new(l.Normalized.Magnitude - r.Normalized.Magnitude);
        #endregion

        #region CROSS-UNIT OPERATORS
        public static AngularVelocity operator /(Angle l, Time r)
            => new(l.Normalized.Magnitude / r.Normalized.Magnitude);
        public static Time operator /(Angle l, AngularVelocity r)
            => new(l.Normalized.Magnitude / r.Normalized.Magnitude);
        #endregion
    }
}
