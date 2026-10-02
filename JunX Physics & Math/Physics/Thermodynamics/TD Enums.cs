using System;
using System.Collections.Generic;
using System.Text;

namespace JunX.Physics.Thermodynamics
{
    public enum EntropyUnits
    {
        Joule_PerKelvin,
        Kilojoule_PerKelvin,
        Megajoule_PerKelvin,
        Millijoule_PerKelvin,
        BTU_PerFahrenheit,
        BTU_PerRankine,
        Calorie_PerKelvin,
        Kilocalorie_PerKelvin,
        Entropy,
        Nat,
        Bit,
        Hartley,
        PlanckEntropy
    }

    public enum SpecificHeatCapacityUnits
    {
        Joule_PerKilogramKelvin,
        Joule_PerGramKelvin,
        Joule_PerKilogramCelsius,
        Kilojoule_PerKilogramKelvin,
        BTU_PerPoundFahrenheit,
        BTU_PerPoundRankine,
        Calorie_PerGramCelsius,
        Calorie_PerGramKelvin,
        Kilocalorie_PerKilogramKelvin,
        Boltzmann_PerAtomicMass,
        PlanckSpecificHeatCapacity
    }
}
