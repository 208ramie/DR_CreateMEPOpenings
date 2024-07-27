using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using DiRoots_MEPWallOpenings.RevitCommands;

namespace DiRoots_MEPWallOpenings.Helpers
{
    public static class DuctExtensions
    {
        public static bool IsRound(this Duct duct)
        => duct.DuctType.Shape == ConnectorProfileType.Round;
        
        public static bool IsRectangular(this Duct duct)
        => duct.DuctType.Shape == ConnectorProfileType.Rectangular;

        
        /// <summary>
        /// Get the 4 main edges of a rectangular duct, these edges are the longest edges of the duct
        /// </summary>
        /// <param name="rectangularDuct"></param>
        /// <returns>The 4 edges as lines</returns>
        public static List<Line> GetEdges(this Duct rectangularDuct)
        {
            if (!rectangularDuct.IsRectangular())
            {
                MEPCurve mepCurve = rectangularDuct;
                mepCurve.GetEdges();
            }
            return rectangularDuct.GetSolid()
                .GetEdges().Select(e => e.AsCurve())
                .OrderByDescending(c => c.Length).Take(4)
                .Cast<Line>().ToList();
        }


        public static double GetInsulationThickness(this Duct duct)
            => duct.get_Parameter(BuiltInParameter.DUCT_INSULATION_THICKNESS).AsDouble();
    }
}
