using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace JunX.Mathematics
{
    public static class Radical
    {
        public static double Root(double radicand, int index)
            => radicand < 0 ?
            throw new ArgumentOutOfRangeException(nameof(radicand), radicand, "Negative radicand is not allowed.")
            : Math.Pow(radicand, 1.0 / index);
    }

    public class MathOperators
    {
        public static Str Absolute<Str, En>(Str unit, En scale)
            where En: Enum
            where Str: struct, ILinearUnit<Str, En>
        {
            if (unit.Original.Magnitude >= 0)
                return Str.Create(unit.Original.Magnitude, scale);

            return Str.Create(unit.Original.Magnitude * -1.0, scale);
        }
    }
}
