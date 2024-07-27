using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI.Events;
using DiRoots_MEPWallOpenings.Models.MEPOpeningsModels;
using DiRoots_MEPWallOpenings.ViewModels;
using DiRoots_MEPWallOpenings.Views;
using OfficeOpenXml.FormulaParsing.Utilities;

namespace DiRoots_MEPWallOpenings.RevitExternalEventHandlers
{
    public class CreateMEPOpeningsEventHandler : IExternalEventHandler
    {




        public static List<OpeningFamilyInstanceInfo> MergeIntersectingOpenings(List<OpeningFamilyInstanceInfo> openings,
            FamilySymbol rectangularOpeningFamilySymbol, double serviceOffsetInMm, double minDistanceForMergeInmm)
        {
            // If the list is null or has 0-1 elements, no merging is needed
            if (openings == null || openings.Count <= 1)
                return openings;

            List<OpeningFamilyInstanceInfo> result = new List<OpeningFamilyInstanceInfo>();

            foreach (var opening in openings)
            {
                bool merged = false;

                // Check if this opening intersects with any in the result list
                for (int i = 0; i < result.Count; i++)
                {
                    if (result[i].IntervalRectangle.Intersects(opening.IntervalRectangle, minDistanceForMergeInmm))
                    {
                        // Merge the intersecting rectangles
                        IntervalRectangle mergedIntervalRectangle = result[i].IntervalRectangle.MergeWith(opening.IntervalRectangle);

                        // Calculate the center point of the merged rectangle
                        UV centerUVPoint = mergedIntervalRectangle.GetCenter();

                        // Convert UV coordinates to XYZ point on the wall face
                        XYZ firstMergedInsterstionPoint = result[i].WallFace.Evaluate(centerUVPoint);

                        // Get the other wall face
                        Face otherWallFace = result[i].HostWall.ExtractFrontAndBackFacesByDirection().First(f => f.Id != result[i].WallFace.Id);

                        XYZ secondMergedInsterstionPoint = otherWallFace.Evaluate(centerUVPoint);

                        XYZ averageMergedInsertionPoint = (firstMergedInsterstionPoint + secondMergedInsterstionPoint) / 2;


                        // Create a new OpeningFamilyInstanceInfo with the merged properties
                        result[i] = new OpeningFamilyInstanceInfo(
                            firstMergedInsterstionPoint,
                            rectangularOpeningFamilySymbol,
                            result[i].HostWall,
                            mergedIntervalRectangle.UInterval.End - mergedIntervalRectangle.UInterval.Start,  // New width
                            mergedIntervalRectangle.VInterval.End - mergedIntervalRectangle.VInterval.Start,  // New height
                            serviceOffsetInMm,
                            true
                        );

                        merged = true;
                        break;  // Exit the loop as we've merged this opening
                    }
                }

                // If the opening didn't intersect with any existing ones, add it to the result list
                if (!merged)
                {
                    result.Add(opening);
                }
            }

            return result;
        }







        public void Execute(UIApplication app)
        {

            #region Some preparation

            // Get reference to the application
            DocHelpers.FillByUiApplication(app);

            // Get reference to the view model
            CreateMEPOpeningsVM vm = CreateMEPOpeningsVM.Instance;


            #endregion


            #region Fields 

            // Opening family symbols
            FamilySymbol? roundOpeningFamilySymbol = null;
            FamilySymbol? rectangularOpeningFamilySymbol = null;
            // MEP curves and walls
            List<Wall> selectedWalls = [];
            List<MEPCurve> allPipesAndDucts = [];


            #endregion



            #region Get the openings families


            (roundOpeningFamilySymbol, rectangularOpeningFamilySymbol) = MEPOpeningsHelpers.GetOpeningsFamilySymbols("Round Opening", "Rectangle Opening");

            // Check if the openings are available
            if (roundOpeningFamilySymbol == null || rectangularOpeningFamilySymbol == null)
            {
                Td.ShowError("Opening families not found");
                return;
            }



            #endregion


            #region User selection

            (selectedWalls, allPipesAndDucts) = MEPOpeningsHelpers.GetSelectedWallsDuctsAndPipes();

            // Make sure the user has selected at least one wall and one duct/pipe
            if (selectedWalls.Count == 0 || allPipesAndDucts.Count == 0)
            {
                Td.ShowError("There must be at least one wall and one duct/pipe selected");
                return;
            }

            #endregion


            using (TransactionGroup transGroup = new TransactionGroup(DocHelpers.Doc))
            {
                transGroup.Start();

                List<OpeningFamilyInstanceInfo> instancesInfo = [];

                foreach (Wall wall in selectedWalls)
                {
                    foreach (MEPCurve mepCurve in allPipesAndDucts)
                    {
                        // Get details about the intersection of the mepCurve with the wall front and back faces
                        var intersectionResult = wall.IntersectWithLine(mepCurve.GetLocationCurve());

                        // If there is no intersection between this MEP curve and the wall, continue to the next MEP curve
                        if (intersectionResult.averagePoint is null)
                            continue;


                        // Accepts round ducts or pipes that are normal to the wall
                        if (mepCurve.IsPipeOrRoundDuct() && mepCurve.IsNormalToWall(wall))
                        {
                            instancesInfo.Add(
                                new OpeningFamilyInstanceInfo(
                                    intersectionResult.averagePoint,
                                    roundOpeningFamilySymbol,
                                    wall,
                                    mepCurve.GetWidthOrDiameter(),
                                    mepCurve.GetHeightOrDiameter(),
                                    vm.ServiceOffsetInmm, 
                                    false));
                        }
                        // Accepts all ducts and pipes as long as they are perpendicular to the wall either horizontally or vertically
                        // The opening will always be rectangular in that case
                        else if (mepCurve.IsNormalToWallButInclinedInZ(wall) || mepCurve.IsHorizontal())
                        {
                            double totalOpeningHeight = MEPOpeningsHelpers.GetTotalOpeningHeightForMepCurveInclinedIn2D
                                (mepCurve, intersectionResult.ptOnFirstFace, intersectionResult.ptOnSecondFace); 


                            double totalOpeningWidth = MEPOpeningsHelpers.GetTotalOpeningWidthForMepCurveInclinedIn2D
                            (mepCurve, wall, intersectionResult.ptOnFirstFace, intersectionResult.ptOnSecondFace); 


                            instancesInfo.Add(
                                new OpeningFamilyInstanceInfo(
                                    intersectionResult.averagePoint,
                                    rectangularOpeningFamilySymbol,
                                    wall,
                                    totalOpeningWidth,
                                    totalOpeningHeight,
                                    vm.ServiceOffsetInmm, 
                                    false));

                        }
                        // Accepts all pipes and ducts (round and rectangular) that are not neither normal horizontally nor vertically to the wall
                        else
                        {
                            // Get the delta Z
                            double deltaZ = Math.Abs(intersectionResult.ptOnFirstFace.Z - intersectionResult.ptOnSecondFace.Z);

                            // Get the delta U
                            double deltaWidth = MEPOpeningsHelpers.GetDeltaWidth(intersectionResult.ptOnFirstFace,
                                intersectionResult.ptOnSecondFace, wall.GetWallLineDirection()); 


                            // Get the length and width of the rectangle drawn by the intersection of the mep curve edges and the wall face
                            var uvDims = mepCurve.GetIntersectionDimensionsOnFace(intersectionResult.firstFace);

                            double totalOpeningHeight = uvDims.deltaV + deltaZ;
                            double totalOpeningWidth = uvDims.deltaU + deltaWidth;


                            instancesInfo.Add(
                                new OpeningFamilyInstanceInfo(
                                    intersectionResult.averagePoint,
                                    rectangularOpeningFamilySymbol,
                                    wall,
                                    totalOpeningWidth,
                                    totalOpeningHeight,
                                    vm.ServiceOffsetInmm, 
                                    false));
                        }

                    }
                }



                List<FamilyInstance> createdOpenings = [];

                // If the user doesn't want to merge the openings, create them directly
                if (!vm.MergeOpenings)
                {
                    DocHelpers.Doc.RunTransaction(() =>
                    {
                        createdOpenings = MEPOpeningsHelpers.CreateOpeningInstances(instancesInfo);
                    }, "Create separate openings");
                }
                // If the user wants to merge the openings, group them by the host wall and merge the intersecting ones
                else
                {
                    // Group the openings by the host wall
                    var openingsByWall = instancesInfo.GroupBy(o => o.HostWall.Id);
                    // Merge the intersecting openings
                    var mergedOpeningsByWall = openingsByWall.Select(group =>
                        MergeIntersectingOpenings(group.ToList(), rectangularOpeningFamilySymbol, vm.ServiceOffsetInmm, vm.MinDisatnceInmm));
                    // Flatten the list of openings
                    var allMergedOpenings = mergedOpeningsByWall.SelectMany(x => x).ToList();

                    DocHelpers.Doc.RunTransaction(() =>
                    {
                        allMergedOpenings.ForEach(i =>
                        {
                            FamilyInstance createdInstance =
                                DocHelpers.Doc.Create.NewFamilyInstance(i.InsertionPoint, i.FamilySymbol, i.InsertionDirection, i.HostWall, StructuralType.NonStructural);

                            createdInstance.SetParameter("Width", i.Width);
                            createdInstance.SetParameter("Height", i.Height);
                            createdInstance.SetParameter("Wall thickness", i.WallThickness);

                            createdOpenings.Add(createdInstance);
                        });
                    }, "Create merged openings");
                }


                transGroup.Assimilate();

                CreateMEPOpeningsView.Instance.Hide();
            }










        }

        public string GetName()
        {
            return "CreateMEPOpeningsEventHandler";
        }
    }
}
