using System;
using System.Collections.Generic;
using System.Text;

namespace JunX.Physics.Electromagnetism
{
    public enum ElectricChargeUnits
    {
        Coulomb,
        Millicoulomb,
        Microcoulomb,
        Nanocoulomb,
        Picocoulomb,
        Kilocoulomb,
        Statcoulomb,
        Abcoulomb,
        Ampere_Hour,
        Milliampere_Hour,
        Ampere_Second,
        Ampere_Minute,
        ElementaryCharge,
        Faraday,
        AtomicCharge,
        PlanckCharge
    }

    public enum ElectricPotentialUnits
    {
        Volt,
        Microvolt,
        Millivolt,
        Kilovolt,
        Megavolt,
        Gigavolt,
        Teravolt,
        Statvolt,
        Abvolt,
        AtomicPotential,
        PlanckVoltage
    }

    public enum ElectricResistanceUnits
    {
        Ohm,
        Micohm,
        Milliohm,
        Kilohm,
        Megohm,
        Gigohm,
        Terohm,
        Statohm,
        Abohm,
        AtomicResistance,
        PlanckImpedance
    }

    public enum ElectricConductanceUnits
    {
        Siemens,
        Microsiemens,
        Millisiemens,
        Kilosiemens,
        Megasiemens,
        Mho,
        Statsiemens,
        Absiemens,
        AtomicConductance,
        PlanckAdmittance
    }

    public enum CapacitanceUnits
    {
        Farad,
        Millifarad,
        Microfarad,
        Nanofarad,
        Picofarad,
        Femtofarad,
        Kilofarad,
        Statfarad,
        Abfarad,
        Centimeter,
        Ampere_Second_PerVolt,
        AtomicCapacitance,
        PlanckCapacitance
    }

    public enum MagneticFluxUnits
    {
        Weber,
        Microweber,
        Milliweber,
        Kiloweber,
        Megaweber,
        Maxwell,
        LineOfForce,
        Kilomaxwell,
        Statweber,
        AtomicMagneticFlux,
        PlanckMagneticFlux
    }

    public enum InductanceUnits
    {
        Henry,
        Nanohenry,
        Microhenry,
        Millihenry,
        Kilohenry,
        Abhenry,
        Stathenry,
        Centimeter,
        AtomicInductance,
        PlanckInductance
    }

    public enum MagneticFluxDensityUnits
    {
        Tesla,
        Nanotesla,
        Microtesla,
        Millitesla,
        Kilotesla,
        Gauss,
        Gamma,
        Stattesla,
        AtomicFluxDensity,
        PlanckMagneticField
    }

    public enum MagneticFluxStrengthUnits
    {
        Ampere_PerMeter,
        AmpereTurn_PerMeter,
        Kiloampere_PerMeter,
        Milliampere_PerMeter,
        Oersted,
        Gilberts_PerCentimeter,
        Statoersted,
        AmpereTurn_PerInch,
        AmpereTurn_PerFoot,
        AtomicMagneticFieldStrength,
        PlanckMagneticFieldStrength
    }
}
