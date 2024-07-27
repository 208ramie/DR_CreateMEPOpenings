using Autodesk.Revit.DB;

namespace DiRoots_MEPWallOpenings.Helpers
{
    public static class LineExtensions
    {
        #region Get start and end

        public static XYZ Start(this Line line) => line.GetEndPoint(0);
        public static XYZ Start(this Curve curve) => curve.GetEndPoint(0);

        public static XYZ End(this Line line) => line.GetEndPoint(1);
        public static XYZ End(this Curve curve) => curve.GetEndPoint(1);

        #endregion
        
    }
}