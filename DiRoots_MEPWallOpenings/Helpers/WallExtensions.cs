using Autodesk.Revit.DB;

namespace DiRoots_MEPWallOpenings.Helpers
{
    public static class WallExtensions
    {
        public static Curve GetCurve(this Wall wall)
            => (wall.Location as LocationCurve).Curve;
        

        /// <summary>
        /// Gets the solid of a wall if it has one
        /// If the wall has multiple solids, it returns the first one
        /// </summary>
        /// <param name="wall">The wall you want to return the solid of</param>
        /// <returns>The Solid of the wall</returns>
        public static Solid? GetSolid(this Wall wall)
        {
            Options opts = new();
            opts.View = DocHelpers.Doc.GetDefault3DView();
            opts.ComputeReferences = true;
            return wall.get_Geometry(opts).FirstOrDefault() as Solid;
        }




        public static List<PlanarFace> ExtractFrontAndBackFacesByDirection(this Wall wall)
        {
            XYZ wallDirection = (wall.GetCurve() as Line).Direction.NormalizedAndAbsolute();
            XYZ perpendicularToWallDirection = wallDirection.CrossProduct(XYZ.BasisZ).NormalizedAndAbsolute(); 

            Solid? wallSolid = wall.GetSolid();

            List<PlanarFace> wallPlanarFaces = wallSolid.GetFaces().Cast<PlanarFace>().ToList();

            List<PlanarFace> frontAndBackFaces = wallPlanarFaces
                .Where(f => f.FaceNormal.NormalizedAndAbsolute().IsAlmostEqualTo(perpendicularToWallDirection))
                .ToList();

            return frontAndBackFaces;
        }

        public static List<Face> ExtractFrontAndBackFacesByArea(this Wall wall)
        {
            Solid wallSolid = wall.GetSolid();
            List<Face> wallFaces = wallSolid.GetFaces().OrderByDescending(f => f.Area).Take(2).ToList();
            return wallFaces;
        }


        /// <summary>
        /// Calculates and returns the normalized direction vector of a wall.
        /// If the wall's curve is a straight line (Line), the method returns the normalized direction of the line.
        /// Otherwise, the direction is determined by the vector from the wall's start point to its end point.
        /// </summary>
        /// <param name="wall">The wall object to calculate the direction for.</param>
        /// <returns>A normalized XYZ vector representing the wall's direction.</returns>
        public static XYZ GetWallLineDirection(this Wall wall)
        {
            if (wall.GetCurve() is Line line)
                return line.Direction.Normalize();

            Curve wallCurve = wall.GetCurve();
            XYZ directionVector = (wallCurve.End() - wallCurve.Start()).Normalize();
            return directionVector;
        }

        /// <summary>
        /// Intersects a given wall with a curve and returthe intersection points on the front and back faces of the wall
        /// </summary>
        /// <param name="wall">The wall the curve intersects</param>
        /// <param name="curve">The curve that intersects the wall</param>
        /// <returns>The faces along with the intersection point and an average point for the 2 intersections</returns>
        /// <exception cref="Exception"></exception>
        public static  (PlanarFace firstFace, XYZ? ptOnFirstFace, PlanarFace secondFace, XYZ? ptOnSecondFace, XYZ? averagePoint)
            IntersectWithLine(this Wall wall, Curve curve)
        {
            List<PlanarFace> frontAndBackFaces = wall.ExtractFrontAndBackFacesByDirection();

            if (frontAndBackFaces.Count != 2)
                throw new Exception("Couldn't extract the front and the back face of the walls");

            PlanarFace firstFace = frontAndBackFaces.First();
            PlanarFace secondFace = frontAndBackFaces.Skip(1).First();

            XYZ? pointOnFirstFace = firstFace.IntersectWithCurve(curve);
            XYZ? pointOnSecondFace = secondFace.IntersectWithCurve(curve);

            XYZ averagePoint = null;

            if (pointOnFirstFace is not null && pointOnSecondFace is not null)
                averagePoint = pointOnFirstFace.Average(pointOnSecondFace);


            return (firstFace, pointOnFirstFace, secondFace, pointOnSecondFace, averagePoint);
        }

        /// <summary>
        /// Get a normalized and absolute vector representing the normal direction of a wall
        /// This assumes that the wall is linear, meaning that its curve is a straight line
        /// </summary>
        /// <param name="wall">The wall you want to get the normal to</param>
        /// <returns></returns>
        public static XYZ GetNoramlDirection(this Wall wall)
            => wall.GetLocationLine().Direction.NormalizedAndAbsolute().CrossProduct(XYZ.BasisZ).NormalizedAndAbsolute(); 




    }


}