using System;
using System.Collections.Generic;
using System.Text;

namespace JunX.Physics.ClassicalMechanics
{
    public enum ForceUnits
    {
        Newton,
        Kilonewton,
        Meganewton,
        Dyne,
        PoundForce,
        Poundal,
        Kip,
        OunceForce,
        ShortTonForce,
        LongTonForce,
        KilogramForce,
        GramForce,
        TonneForce,
        PlanckForce,
        AtomicForce
    }

    public enum EnergyUnits
    {
        Joule,
        Kilojoule,
        Megajoule,
        Gigajoule,
        Erg,
        WattHour,
        KilowattHour,
        MegawattHour,
        GigawattHour,
        Calorie,
        Kilocalorie,
        BritishThermalUnit,
        Therm,
        Quad,
        FootPoundForce,
        FootPoundal,
        HorsepowerHour,
        Electronvolt,
        Kiloelectronvolt,
        Megaelectronvolt,
        Gigaelectronvolt,
        Teraelectronvolt,
        Rydberg,
        Hartree,
        WaveNumber,
        OilTonne,
        CoalTonne,
        OilBarrel,
        TNT_Tonne,
        AtomicMassUnit,
        PlanckEnergy
    }

    public enum PowerUnits
    {
        Watt,
        Milliwatt,
        Kilowatt,
        Megawatt,
        Gigawatt,
        Terawatt,
        ErgPerSecond,

        Horsepower_Mechanical,
        Horsepower_Metric,
        Horsepower_Electrical,
        Horsepower_Boiler,
        Horsepower_Air,

        FootPoundForcePerSecond,
        FootPooundForcePerMinute,
        BTU_PerHour,
        TonRefrigeration,
        
        CaloriePerSecond,
        KilocaloriePerHour,
        
        PlanckPower,
        HartreePerAtomicUnitTime
    }

    public enum PressureUnits
    {
        Pascal,
        Hectopascal,
        Kilopascal,
        Megapascal,
        Gigapascal,
        Bar,
        Millibar,
        Barye,
        Atmosphere_Standard,
        Atmosphere_Technical,
        Torr,
        Mercury_Millimeter,
        Mercury_Inch,
        Water_Centimeter,
        Water_Millimeter,
        Water_Inch,
        Water_Foot,
        PoundPerSquareInch,
        PoundPerSquareFoot,
        KipPerSquareInch,
        OuncePerSquareInch,
        PoundalPerSquareFoot,
        LongTonPerSquareInch,
        ShortTonPerSquareInch,
        PlanckPressure,
        AtomicPressure
    }

    public enum MomentumUnits
    {
        NewtonPerSecond,
        Gram_Centimeter_PerSecond,
        DyneSecond,
        Tonne_Meter_PerSecond,
        Pound_Foot_PerSecond,
        Pound_ForceS_econd,
        Poundal_Second,
        Slug_Foot_PerSecond,
        Ounce_Inch_PerSecond,
        Electronvolt_Per_C,
        KiloElectronvolt_Per_C,
        MegaElectronvolt_Per_C,
        GigaElectronvolt_Per_C,
        TeraElectronvolt_Per_C,
        SolarMass_AstronomicalUnit_PerYear,
        PlanckMomentum,
        AtomicMomentum
    }

    public enum AngularMomentumUnits
    {
        Joule_Second,
        Newton_Meter_Second,
        Gram_CentimeterSquared_PerSecond,
        Erg_Second,
        Pound_SquareFoot_PerSecond,
        PoundForce_Foot_Second,
        Slug_SquareFoot_PerSecond,
        Poundal_Foot_Second,
        AtomicAngularMomentum,
        Electronvolt_Second,
        SolarMass_AstronomicalUnitSquared_PerYear,
        GeometrizedMassSquared
    }

    public enum TorqueUnits
    {
        Newton_Meter,
        Millinewton_Meter,
        Kilonewton_Meter,
        Meganewton_Meter,
        Dyne_Centimeter,
        KilogramForce_Meter,
        GramForce_Centimeter,
        PoundForce_Foot,
        PoundForce_Inch,
        OunceForce_Inch,
        Kip_Foot,
        Kip_Inch,
        Poundal_Foot,
        PlanckTorque,
        AtomicTorque
    }
}
