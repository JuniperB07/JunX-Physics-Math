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
}
