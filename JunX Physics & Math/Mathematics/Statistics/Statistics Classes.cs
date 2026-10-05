using System;
using System.Collections.Generic;
using System.Text;

namespace JunX.Mathematics.Statistics
{
    public class StatisticsSolver
    {
        public static Str RootMeanSquare<Str, En>(Str[] elements, En scale)
            where En: Enum
            where Str: struct, ILinearUnit<Str, En>
        {
            Str sum = Str.Initialize();
            for(int i = 0; i<elements.Length; i++)
            {
                double mag = sum.Normalized.Magnitude + elements[i].Normalized.Magnitude;
                sum = Str.Create(Math.Pow(mag, 2));
            }

            double resMag = (1.0 / elements.Length) * sum.Normalized.Magnitude;
            Str res = Str.Create(Math.Sqrt(resMag));
            return res;
        }
        public static Str RootMeanSquare<Str, En>(Str peak, En scale)
            where En : Enum
            where Str : struct, ILinearUnit<Str, En>
            => Str.Create(peak.Normalized.Magnitude / Math.Sqrt(2.0));
    }
}
