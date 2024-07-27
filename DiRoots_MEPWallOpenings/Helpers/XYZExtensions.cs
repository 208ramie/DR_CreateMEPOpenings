using System.Runtime.CompilerServices;
using Autodesk.Revit.DB;

namespace DiRoots_MEPWallOpenings.Helpers
{
    public static class XYZExtensions
    {


        /// <summary>
        /// Ensure a better vector comparison where direction is not important
        /// First the vector is normalized then we take the abs values of it's X Y Z values
        /// </summary>
        /// <param name="xyz">The vector you want to normalize and absolute</param>
        /// <returns>A normalized absolute vector</returns>
        public static XYZ NormalizedAndAbsolute(this XYZ xyz)
        {
            XYZ normalizedXYZ = xyz.Normalize();
            XYZ normalizedAbsoluteXYZ = new XYZ(Math.Abs(normalizedXYZ.X), Math.Abs(normalizedXYZ.Y), Math.Abs(normalizedXYZ.Z));
            return normalizedAbsoluteXYZ;
        }
        

        public static XYZ Average(this XYZ firstPoint, XYZ secondPoint)
        {
            double newX = (firstPoint.X + secondPoint.X) / 2;
            double newY = (firstPoint.Y + secondPoint.Y) / 2;
            double newZ = (firstPoint.Z + secondPoint.Z) / 2;
            return new XYZ(newX, newY, newZ);
        }
        

        /// <summary>
        /// Projects a vector on the XY plane by removing the z component of the vector. 
        /// </summary>
        /// <param name="vector">The vector you want to project on the XY</param>
        /// <returns>The projected vector on the XY</returns>
        public static XYZ ProjectVectorOnXY(this XYZ vector)
            => new XYZ(vector.X, vector.Y, 0);
        

        /// <summary>
        /// Projects a point onto a face and gets the UV coordinates of the point on that face
        /// </summary>
        /// <param name="point">The point you want to project on the face</param>
        /// <param name="face">The face you want to project the point onto</param>
        /// <returns>The UV coordinates of projecting the point on the face</returns>
        public static UV ToUV(this XYZ point, Face face)
            => face.Project(point).UVPoint;

        
        public static XYZ GetAverageVector(this XYZ firstVector, XYZ secondVector)
        {
            var normalizedFirstVector = firstVector.Normalize();
            var normalizedSecondVector = secondVector.Normalize();

            var tweenedVector = (normalizedFirstVector + normalizedSecondVector).Normalize(); 
            return tweenedVector;
        }
        
    }
}