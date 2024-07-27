using Autodesk.Revit.DB;

namespace DiRoots_MEPWallOpenings.Helpers
{
    public static class FaceExtensions
    {
        /// <summary>
        /// Gets the point that results from the intersection of a face with a curve
        /// </summary>
        /// <param name="face">The face to check</param>
        /// <param name="curve">The curve to check</param>
        /// <returns>A point if they do intersect, or none if they don't</returns>
        public static XYZ? IntersectWithCurve(this Face face, Curve curve)
        {
            IntersectionResultArray intersectionResultArray;
            SetComparisonResult intersectionResult = face.Intersect(curve, out intersectionResultArray);

            return intersectionResult == SetComparisonResult.Overlap ?
                intersectionResultArray.get_Item(0).XYZPoint
                : null;
        }

    }
}