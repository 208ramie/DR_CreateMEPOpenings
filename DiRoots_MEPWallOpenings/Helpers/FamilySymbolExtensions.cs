using Autodesk.Revit.DB;

namespace DiRoots_MEPWallOpenings.Helpers
{
    public static class FamilySymbolExtensions
    {
        public static FamilySymbol ActivateIfNotActive(this FamilySymbol familySymbol)
        {
            if (!familySymbol.IsActive)
                familySymbol.Activate();

            return familySymbol;
        }
    }
}