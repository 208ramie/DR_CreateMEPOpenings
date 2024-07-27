using Autodesk.Revit.DB;

namespace DiRoots_MEPWallOpenings.Helpers
{
    public static class DoubleExtensions
    {
        public static double MToIU(this double numberInMeters)
            => UnitUtils.ConvertToInternalUnits(numberInMeters, UnitTypeId.Meters);

        public static double mmToIU(this double numberInMillimeters)
            => UnitUtils.ConvertToInternalUnits(numberInMillimeters, UnitTypeId.Millimeters);

        public static double IUTomm(this double numberInInternalUnits)
            => UnitUtils.ConvertFromInternalUnits(numberInInternalUnits, UnitTypeId.Millimeters);
        
        public static bool IsAlmostEqualTo(this double firstNumber, double secondNumber, double tolerance)
            => Math.Abs(firstNumber - secondNumber) < tolerance;

    }
}