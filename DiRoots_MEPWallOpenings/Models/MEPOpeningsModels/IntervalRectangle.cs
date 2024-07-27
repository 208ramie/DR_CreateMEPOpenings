using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Autodesk.Revit.DB;

namespace DiRoots_MEPWallOpenings.Models.MEPOpeningsModels
{
    public class IntervalRectangle
    {
        public Interval UInterval { get; set; }
        public Interval VInterval { get; set; }

        public double Width => UInterval.Span;
        public double Height => VInterval.Span;

        public IntervalRectangle(Interval uInterval, Interval vInterval)
        {
            UInterval = uInterval;
            VInterval = vInterval;
        }

        public IntervalRectangle MergeWith(IntervalRectangle otherRectangle)
        {
            Interval mergedUInterval = UInterval.MergeWith(otherRectangle.UInterval);
            Interval mergedVInterval = VInterval.MergeWith(otherRectangle.VInterval);

            return new IntervalRectangle(mergedUInterval, mergedVInterval);
        }

        public bool Intersects(IntervalRectangle otherRectangle, double toleranceInmm)
        {
            return UInterval.Intersects(otherRectangle.UInterval, toleranceInmm) && VInterval.Intersects(otherRectangle.VInterval, toleranceInmm);
        }

        public UV GetCenter()
        {
            double uCenter = (UInterval.Start + UInterval.End) / 2;
            double vCenter = (VInterval.Start + VInterval.End) / 2;

            return new UV(uCenter, vCenter);
        }
    }
}
