using JunX.Mathematics.Geometry;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices.Marshalling;
using System.Text;

namespace JunX.Physics.BaseUnits
{
    public struct Time :
        IInitializable<Time>, IInitializable<Time, double>, IInitializable<Time, double, TimeUnits>,
        IScaleMappable<TimeUnits>, IScaleConvertible<Time, TimeUnits>,
        INormalized<TimeUnits>, INormalizable<Time>,
        IDimensionAccessible,
        IValueAccessible<TimeUnits>,
        IDuplicatable<Time>,
        IEquatable<Time>,
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

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}
