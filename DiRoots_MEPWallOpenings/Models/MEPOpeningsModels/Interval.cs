using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiRoots_MEPWallOpenings.Models.MEPOpeningsModels
{
    public class Interval
    {
        public double Start { get; set; }
        public double End { get; set; }
        public double Span { get; set; }


        private Interval(double start, double end)
        {
            Start = start;
            End = end;
        }

        public static Interval CreateByStartEnd(double start, double end)
        {
            double smallerValue = Math.Min(start, end);
            double biggerValue = Math.Max(start, end);

            return new Interval(smallerValue, biggerValue);
        }

        public static Interval CreateByCenterWidth(double center, double width)
        {
            width = Math.Abs(width);

            return new Interval(center - width / 2, center + width / 2);
        }



        /// <summary>
        /// Determines if this interval intersects with another interval, including a tolerance.
        /// </summary>
        /// <param name="otherInterval">The other interval to check against.</param>
        /// <param name="tolerance">The tolerance distance for considering intervals as intersecting.</param>
        /// <returns>True if the intervals intersect or are within the tolerance; otherwise, false.</returns>
        public bool Intersects(Interval otherInterval, double toleranceInmm)
        {
            double toleranceInIU = toleranceInmm.mmToIU();
            // Check if one interval starts before the other ends (plus tolerance) and vice versa
            return this.Start <= otherInterval.End + toleranceInIU && this.End + toleranceInIU >= otherInterval.Start;
        }

        public Interval MergeWith(Interval otherInterval)
        {
            
            double createdStart = Math.Min(this.Start, otherInterval.Start);
            double createdEnd = Math.Max(this.End, otherInterval.End);

            return Interval.CreateByStartEnd(createdStart, createdEnd); 
        }

    }
}
