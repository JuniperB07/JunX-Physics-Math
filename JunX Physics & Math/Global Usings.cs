#region BINARY COMPOSITE UNITS
global using FreeSpacePermeability = JunX.QuotientUnit<JunX.Physics.ClassicalMechanics.Force, JunX.Physics.ClassicalMechanics.ForceUnits, JunX.UnitSquared<JunX.Physics.BaseUnits.Current, JunX.Physics.BaseUnits.CurrentUnits>, JunX.Physics.BaseUnits.CurrentUnits>;
global using ElectricField = JunX.QuotientUnit<JunX.Physics.Electromagnetism.ElectricPotential, JunX.Physics.Electromagnetism.ElectricPotentialUnits, JunX.Mathematics.Geometry.Length, JunX.Mathematics.Geometry.LengthUnits>;
global using EnergyDensity = JunX.QuotientUnit<JunX.Physics.ClassicalMechanics.Energy, JunX.Physics.ClassicalMechanics.EnergyUnits, JunX.UnitCubed<JunX.Mathematics.Geometry.Length, JunX.Mathematics.Geometry.LengthUnits>, JunX.Mathematics.Geometry.LengthUnits>;
#endregion

#region INTERMEDIARY UNIT : (C^2)(V^2) / N(m^2)^2
global using CoulombSquaredVoltageSquared_PerNewtonSquareMeterSquared = JunX.QuaternaryQuotientUnit<
    JunX.BinaryComposite<
        JunX.Numerator<
            JunX.CompositeProduct<
                JunX.UnitSquared<
                    JunX.Physics.Electromagnetism.ElectricCharge,
                    JunX.Physics.Electromagnetism.ElectricChargeUnits>,
                JunX.UnitSquared<
                    JunX.Physics.Electromagnetism.ElectricPotential,
                    JunX.Physics.Electromagnetism.ElectricPotentialUnits>>>,
        JunX.Denominator<
            JunX.CompositeProduct<
                JunX.Physics.ClassicalMechanics.Force,
                JunX.UnitSquared<
                    JunX.UnitSquared<
                        JunX.Mathematics.Geometry.Length,
                        JunX.Mathematics.Geometry.LengthUnits>,
                    JunX.Mathematics.Geometry.LengthUnits>>>>,
    JunX.UnitSquared<
        JunX.Physics.Electromagnetism.ElectricCharge,
        JunX.Physics.Electromagnetism.ElectricChargeUnits>,
    JunX.Physics.Electromagnetism.ElectricChargeUnits,
    JunX.Physics.ClassicalMechanics.Force,
    JunX.Physics.ClassicalMechanics.ForceUnits,
    JunX.UnitSquared<
        JunX.UnitSquared<
            JunX.Mathematics.Geometry.Length,
            JunX.Mathematics.Geometry.LengthUnits>,
        JunX.Mathematics.Geometry.LengthUnits>,
    JunX.Mathematics.Geometry.LengthUnits,
    JunX.UnitSquared<
        JunX.Physics.Electromagnetism.ElectricPotential,
        JunX.Physics.Electromagnetism.ElectricPotentialUnits>,
    JunX.Physics.Electromagnetism.ElectricPotentialUnits>;
#endregion

#region INTERMEDIARY UNIT: (N^2)(m^2) / N(m^2)^2
global using NewtonSquaredMeterSquared_PerNewtonSquareMeterSquared = JunX.QuaternaryQuotientUnit<
    JunX.BinaryComposite<
        JunX.Numerator<
            JunX.CompositeProduct<
                JunX.UnitSquared<
                    JunX.Physics.ClassicalMechanics.Force,
                    JunX.Physics.ClassicalMechanics.ForceUnits>,
                JunX.UnitSquared<
                    JunX.Mathematics.Geometry.Length,
                    JunX.Mathematics.Geometry.LengthUnits>>>,
        JunX.Denominator<
            JunX.CompositeProduct<
                JunX.Physics.ClassicalMechanics.Force,
                JunX.UnitSquared<
                    JunX.UnitSquared<
                        JunX.Mathematics.Geometry.Length,
                        JunX.Mathematics.Geometry.LengthUnits>,
                    JunX.Mathematics.Geometry.LengthUnits>>>>,
    JunX.UnitSquared<
        JunX.Physics.ClassicalMechanics.Force,
        JunX.Physics.ClassicalMechanics.ForceUnits>,
    JunX.Physics.ClassicalMechanics.ForceUnits,
    JunX.UnitSquared<
        JunX.Mathematics.Geometry.Length,
        JunX.Mathematics.Geometry.LengthUnits>,
    JunX.Mathematics.Geometry.LengthUnits,
    JunX.Physics.ClassicalMechanics.Force,
    JunX.Physics.ClassicalMechanics.ForceUnits,
    JunX.UnitSquared<
        JunX.UnitSquared<
            JunX.Mathematics.Geometry.Length,
            JunX.Mathematics.Geometry.LengthUnits>,
        JunX.Mathematics.Geometry.LengthUnits>,
    JunX.Mathematics.Geometry.LengthUnits>;
#endregion

#region INTERMEDIARY UNIT: Tesla^2
global using TeslaSquared = JunX.TernaryQuotientUnit<
    JunX.SquaredComposite<
        JunX.Denominator<
            JunX.CompositeProduct<
                JunX.Physics.BaseUnits.Current,
                JunX.Mathematics.Geometry.Length>>>,
    JunX.UnitSquared<
        JunX.Physics.ClassicalMechanics.Force,
        JunX.Physics.ClassicalMechanics.ForceUnits>,
    JunX.Physics.ClassicalMechanics.ForceUnits,
    JunX.UnitSquared<
        JunX.Physics.BaseUnits.Current,
        JunX.Physics.BaseUnits.CurrentUnits>,
    JunX.Physics.BaseUnits.CurrentUnits,
    JunX.UnitSquared<
        JunX.Mathematics.Geometry.Length,
        JunX.Mathematics.Geometry.LengthUnits>,
    JunX.Mathematics.Geometry.LengthUnits>;
#endregion