using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace DiRoots_MEPWallOpenings.Helpers
{
    public static class SolidExtensions
    {
        public static List<Face> GetFaces(this Solid solid)
        {
            FaceArray faceArray = solid.Faces;
            return faceArray.Cast<Face>().ToList();
        }

        public static List<Edge> GetEdges(this Solid? solid)
        {
            EdgeArray edgeArray = solid.Edges;
            return edgeArray.Cast<Edge>().ToList();
        }

    }
}
