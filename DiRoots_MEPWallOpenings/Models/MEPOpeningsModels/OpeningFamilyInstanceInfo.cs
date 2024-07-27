using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace DiRoots_MEPWallOpenings.Models.MEPOpeningsModels
{
    public class OpeningFamilyInstanceInfo
    {
        public XYZ InsertionPoint { get; set; }
        public FamilySymbol FamilySymbol { get; set; }
        public Wall HostWall { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public Face WallFace => HostWall.ExtractFrontAndBackFacesByDirection().MinBy(f => f.Id); 

        public XYZ VectorToWallCenter => (WallFace as PlanarFace).FaceNormal.Normalize().Negate()  * WallThickness / 2;

        public UV InsertionPointUV => InsertionPoint.ToUV(WallFace);

        private Interval UInterval => Interval.CreateByCenterWidth(InsertionPointUV.U, Width);
        private Interval VInterval => Interval.CreateByCenterWidth(InsertionPointUV.V, Height);
        public IntervalRectangle IntervalRectangle => new IntervalRectangle(UInterval, VInterval);
        
        public XYZ InsertionDirection => HostWall.GetWallLineDirection();
        public double WallThickness => HostWall.Width;


        public OpeningFamilyInstanceInfo(XYZ insertionPoint, FamilySymbol familySymbol, Wall host, double widthWithoutService, double heightWithoutService, double serviceOffsetInMm, bool isMerged)
        {
            HostWall = host;
            InsertionPoint = isMerged ? insertionPoint + VectorToWallCenter : insertionPoint;
            FamilySymbol = familySymbol;

            double processedServiceOffset = serviceOffsetInMm.mmToIU() * 2;
            Width = isMerged ? widthWithoutService : widthWithoutService + processedServiceOffset;
            Height = isMerged ? heightWithoutService : heightWithoutService + processedServiceOffset;


        }
    }
}
