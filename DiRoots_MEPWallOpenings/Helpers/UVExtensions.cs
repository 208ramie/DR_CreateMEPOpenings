using Autodesk.Revit.DB;

namespace DiRoots_MEPWallOpenings.Helpers
{
    public static class UVExtensions
    {

        /// <summary>
        /// Gets statistics about a list of UVs, like the minU, maxU, minV, maxV, uSpan, vSpan
        /// </summary>
        /// <param name="uvs">A list of UVs</param>
        /// <returns>minU, maxU, minV, maxV, uSpan, vSpan</returns>
        public static (double minU, double maxU, double minV, double maxV, double uSpan, double vSpan) GetStatistics(
            this IEnumerable<UV> uvs)
        {
            List<double> allUs = uvs.Select(uv => uv.U).ToList();
            List<double> allVs = uvs.Select(uv => uv.V).ToList();
            double minU = allUs.Min();
            double maxU = allUs.Max();
            double minV = allVs.Min();
            double maxV = allVs.Max();
            double uSpan = Math.Abs(maxU - minU);
            double vSpan = Math.Abs(maxV - minV);
            return (minU, maxU, minV, maxV, uSpan, vSpan);
        }

    }
}