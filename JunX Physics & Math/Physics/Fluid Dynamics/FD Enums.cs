using System;
using System.Collections.Generic;
using System.Text;

namespace JunX.Physics.FluidDynamics
{
    public enum DynamicViscosityUnits
    {
        PascalSecond,
        MillipascalSecond,
        MicropascalSecond,
        KilopascalSecond,
        Piose,
        Centipoise,
        Millipoise,
        Micropoise,
        PoundForceSecond_PerSquareFoot,
        PoundForceSecond_PerSquareInch,
        Pound_PerFootHour,
        Pound_PerFootSecond,
        Slug_PerFootSecond,
        KilogramForceSecond_PerSquareMeter,
        Gram_PerCentimeterSecond,
        AtomicDynamicViscosity,
        PlanckDynamicViscosity
    }

    public enum KinematicViscosityUnits
    {
        SquareMeter_PerSecond,
        SquareMillimeter_PerSecond,
        SquareCentimeter_PerSecond,
        Stokes,
        Centistokes,
        Millistokes,
        Microstokes,
        SquareFoot_PerSecond,
        SquareFoot_PerHour,
        SquareInch_PerSecond,
        AtomicKinematicViscosity,
        PlanckKinematicViscosity
    }

    public enum DensityUnits
    {
        Kilogram_PerCubicMeter,
        Gram_PerCubicCentimeter,
        Milligram_PerCubicMeter,
        Kilogram_PerLiter,
        Gram_PerMilliliter,
        Gram_PerLiter,
        MetricTon_PerCubicMeter,
        Milligram_PerCubicCentimeter,
        Pound_PerCubicFoot,
        Pound_PerCubicInch,
        Pound_PerGallon_US,
        Pound_PerGallon_Imperial,
        Slug_PerCubicFoot,
        Ounce_PerCubicInch,
        Ounce_PerFluidOunce,
        LongTon_PerCubicYard,
        ShordTon_PerCubicYard,
        AtomicDensity,
        PlanckDensity
    }

    public enum SpecificWeightUnits
    {
        Newton_PerCubicMeter,
        Kilonewton_PerCubicMeter,
        Meganewton_PerCubicMeter,
        Millinewton_PerCubicMeter,
        Dyne_PerCubicCentimeter,
        KilogramForce_PerCubicMeter,
        GramForce_PerCubicCentimeter,
        KilogramForce_PerLiter,
        PoundForce_PerCubicFoot,
        PoundForce_PerCubicInch,
        PoundForce_PerGallon,
        OunceForce_perCubicInch,
        OunceForce_PerCubicFoot,
        Kip_PerCubicFoot,
        LongTonForce_PerCubicYard,
        ShortTonForce_PerCubicYard,
        AtomicSpecificWeight,
        PlanckSpecificWeight
    }

    public enum VolumetricFlowRateUnits
    {
        CubicMeter_PerSecond,
        CubicMeter_PerHour,
        Liter_PerSecond,
        Liter_PerMinute,
        Liter_PerHour,
        Milliliter_PerSecond,
        Milliliter_PerMinute,
        CubicFoot_PerSecond,
        CubicFoot_PerMinute,
        CubicInch_PerSecond,
        Gallon_PerMinute_US,
        Gallon_PerHour,
        Gallon_PerDay,
        Gallon_PerMinute_Imperial,
        MillionGallons_PerDay,
        Barrel_PerDay,
        AcreFoot_PerDay,
        MinersInch,
        AtomicVolumetricFlowRate,
        PlanckVolumetricFlowRate
    }

    public enum MassFlowRateUnits
    {
        Kilogram_PerSecond,
        Kilogram_PerHour,
        Kilogram_PerMinute,
        Gram_PerSecond,
        Gram_PerMinute,
        Gram_PerHour,
        Milligram_PerSecond,
        MetricTon_PerHour,
        MetricTon_PerDay,
        Pound_PerSecond,
        Pound_PerMinute,
        Pound_PerHour,
        Ounce_PerSecond,
        Ounce_PerMinute,
        Slug_PerMinute,
        LongTon_PerHour,
        ShortTon_PerHour,
        ShortTon_PerDay,
        AtomicMassFlowRate,
        PlanckMassFlowRate
    }

    public enum SurfaceTensionUnits
    {
        Newton_PerMeter,
        Millinewton_PerMeter,
        Joule_PerSquareMeter,
        Millijoule_PerSquareMeter,
        Micronewton_PerMeter,
        Dyne_PerCentimeter,
        Erg_PerSquareCentimeter,
        KilogramForce_PerMeter,
        GramForce_PerCentimeter,
        PoundForce_PerInch,
        PoundForce_PerFoot,
        OunceForce_PerInch,
        Poundal_PerInch,
        AtomicSurfaceTension,
        PlanckSurfaceTension
    }
}
