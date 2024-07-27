using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB.Plumbing;
using OfficeOpenXml.Drawing.Chart;

namespace DiRoots_MEPWallOpenings.Helpers
{
    internal static class MEPCurveExtensions
    {
        /// <summary>
        /// Check if the duct is normal to the wall
        /// A duct is normal to a wall when the direction of the duct is the same as the direction of the wall
        /// </summary>
        /// <param name="mepCurve">The duct/Pipe you want to check if it is normal to the wall</param>
        /// <param name="wall"></param>
        /// <returns></returns>
        public static bool IsNormalToWall(this MEPCurve mepCurve, Wall wall)
            => wall.GetNoramlDirection().IsAlmostEqualTo(mepCurve.GetLocationLine().Direction.NormalizedAndAbsolute());



        /// <summary>
        /// True if the duct/pipe is planar on the xy plane, meaning that the duct is horizontal
        /// </summary>
        /// <param name="mepCurve"></param>
        /// <param name="toleranceInMM"></param>
        /// <returns></returns>
        public static bool IsHorizontal(this MEPCurve mepCurve, double toleranceInMM = 1)
        {
            Line ductLine = mepCurve.GetLocationLine();
            double startPointZValueInmm = ductLine.Start().Z.IUTomm();
            double endPointZValueInmm = ductLine.End().Z.IUTomm();
            double toleranceInInternalUnits = toleranceInMM.mmToIU();
            return startPointZValueInmm.IsAlmostEqualTo(endPointZValueInmm, toleranceInInternalUnits);
        }

        /// <summary>
        /// Projects the duct/pipe direction on the xy plane and checks if the projection is normal to the wall
        /// </summary>
        /// <param name="mepCurve"></param>
        /// <param name="wall"></param>
        /// <returns></returns>
        public static bool IsNormalToWallButInclinedInZ(this MEPCurve mepCurve, Wall wall)
        {
            XYZ projectedDuctDirection = mepCurve.GetLocationLine().Direction.ProjectVectorOnXY().NormalizedAndAbsolute();
            XYZ normalToWallDirection = wall.GetNoramlDirection().NormalizedAndAbsolute();
            //$"{projectedDuctDirection.ToCustomString()}\n{normalToWallDirection.ToCustomString()}\n{projectedDuctDirection.IsAlmostEqualTo(normalToWallDirection, 0.01)}".ShowInRevit();
            return projectedDuctDirection.IsAlmostEqualTo(normalToWallDirection, 0.01);
        }


        public static bool IsRoundDuct(this MEPCurve mepCurve)
            => mepCurve is Duct duct && duct.IsRound();

        public static bool IsRectangularDuct(this MEPCurve mepCurve)
            => mepCurve is Duct duct && duct.IsRectangular(); 

        public static bool IsPipeOrRoundDuct(this MEPCurve mepCurve)
            => mepCurve is Pipe || mepCurve.IsRoundDuct();


        /// <summary>
        /// Projects the location line on a Pipe/Circular duct on its surface in the 8 main directions
        /// </summary>
        /// <param name="mepCurve">Optimally you would use a Pipe or a round duct here, if a rectangular duct is used it will be redirected</param>
        /// <returns>The 8 lines mentioned in the description</returns>
        public static List<Line> GetEdges(this MEPCurve mepCurve)
        {
            if (mepCurve.IsRectangularDuct())
            {
                Duct duct = mepCurve as Duct;
                return duct.GetEdges();
            }

            double radius = mepCurve.Diameter / 2.0;

            Line locationLine = mepCurve.GetLocationLine();

            XYZ horizontalPerpendicularDirection = locationLine.Direction.CrossProduct(XYZ.BasisZ).Normalize();

            XYZ verticalPerpendicularDirection = locationLine.Direction.CrossProduct(horizontalPerpendicularDirection).Normalize();

            XYZ diagonalPerpendicularVector = horizontalPerpendicularDirection.GetAverageVector(verticalPerpendicularDirection);

            XYZ otherDiagonalPerpendicularVector = horizontalPerpendicularDirection.Negate().GetAverageVector(verticalPerpendicularDirection);

            Line upperLine = Line.CreateBound(locationLine.Start() + verticalPerpendicularDirection * radius, locationLine.End() + verticalPerpendicularDirection * radius);

            Line lowerLine = Line.CreateBound(locationLine.Start() - verticalPerpendicularDirection * radius, locationLine.End() - verticalPerpendicularDirection * radius);

            Line leftLine = Line.CreateBound(locationLine.Start() + horizontalPerpendicularDirection * radius, locationLine.End() + horizontalPerpendicularDirection * radius);

            Line rightLine = Line.CreateBound(locationLine.Start() - horizontalPerpendicularDirection * radius, locationLine.End() - horizontalPerpendicularDirection * radius);


            Line upperRightLine = Line.CreateBound(locationLine.Start() + diagonalPerpendicularVector * radius, locationLine.End() + diagonalPerpendicularVector * radius);

            Line upperLeftLine = Line.CreateBound(locationLine.Start() + otherDiagonalPerpendicularVector * radius, locationLine.End() + otherDiagonalPerpendicularVector * radius);

            Line lowerRightLine = Line.CreateBound(locationLine.Start() - diagonalPerpendicularVector * radius, locationLine.End() - diagonalPerpendicularVector * radius);

            Line lowerLeftLine = Line.CreateBound(locationLine.Start() - otherDiagonalPerpendicularVector * radius, locationLine.End() - otherDiagonalPerpendicularVector * radius);

            return [leftLine, rightLine, upperLine, lowerLine, upperRightLine, upperLeftLine, lowerRightLine, lowerLeftLine]; 
        }


        /// <summary>
        /// Solves the intersection between the MEP curve edges and a face, and draws a rectangle around the intersection
        /// </summary>
        /// <param name="mepCurve">A pipe or duct</param>
        /// <param name="face">The face the mepCurve intersects</param>
        /// <returns>The length and width of the rectangle formed by the intersection</returns>
        public static (double deltaU, double deltaV) GetIntersectionDimensionsOnFace(this MEPCurve mepCurve, Face face)
        {
            var ductEdges = mepCurve.GetEdges();

            List<XYZ?> intersectionPoints = ductEdges.Select(e => face.IntersectWithCurve(e)).ToList();
            List<UV> uvs = intersectionPoints.Select(ip => ip.ToUV(face)).ToList();
            var uvStats = uvs.GetStatistics();
            double deltaU = uvStats.uSpan;
            double deltaV = uvStats.vSpan;

            return (deltaU, deltaV);
        }

        public static double GetDiameter(this MEPCurve mepCurve)
        {
            if (mepCurve is Pipe pipe)
            {
                return pipe.get_Parameter(BuiltInParameter.RBS_PIPE_OUTER_DIAMETER).AsDouble();
            }

            if (mepCurve.IsRoundDuct())
            {
                return (mepCurve as Duct).Diameter + (mepCurve as Duct).GetInsulationThickness() * 2;
            }
            
            throw new Exception("The mepCurve is not a pipe or a round duct");
        }

        public static double GetWidthOrDiameter(this MEPCurve mepCurve)
            => mepCurve.IsRectangularDuct() ? (mepCurve as Duct).Width + (mepCurve as Duct).GetInsulationThickness() * 2
                : mepCurve.GetDiameter();

        public static double GetHeightOrDiameter(this MEPCurve mepCurve)
            => mepCurve.IsRectangularDuct() ? (mepCurve as Duct).Height + (mepCurve as Duct).GetInsulationThickness() * 2
                : mepCurve.GetDiameter();



    }
}
