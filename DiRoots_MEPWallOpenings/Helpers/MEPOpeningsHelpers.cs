using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Structure;
using DiRoots_MEPWallOpenings.Models.MEPOpeningsModels;

namespace DiRoots_MEPWallOpenings.Helpers
{
    /// <summary>
    /// A class containing the core logic for the MEP opening creator plugin in the form of simple static methods
    /// </summary>
    public static class MEPOpeningsHelpers
    {

        /// <summary>
        /// Get the family symbols for the round and rectangular openings
        /// </summary>
        /// <param name="roundFamilySymbolName">The name of the family symbol used for round openings</param>
        /// <param name="rectangleFamilySymbolName">The name of the family symbol used for rectangle openings</param>
        /// <returns>The 2 family symbol if both are found, the symbol that isn't found will be null</returns>
        public static (FamilySymbol roundOpeningFamilySymbol, FamilySymbol rectangleOpeningsFamilySymbol)
            GetOpeningsFamilySymbols(
                string roundFamilySymbolName = "Round Opening", string rectangleFamilySymbolName = "Rectangle Opening")
        {
            // Get the opening families
            List<Element>? allGenericModels = new FilteredElementCollector(DocHelpers.Doc)
                .OfClass(typeof(FamilySymbol))
                .OfCategory(BuiltInCategory.OST_GenericModel)
                .WhereElementIsElementType()
                .ToElements().ToList();

            // Get the openings family symbols
            FamilySymbol? roundOpeningFamilySymbol = allGenericModels
                .FirstOrDefault(e => e.Name.Contains("Round Opening", StringComparison.OrdinalIgnoreCase)) as FamilySymbol;

            FamilySymbol? rectangularOpeningFamilySymbol = allGenericModels
                .FirstOrDefault(e => e.Name.Contains("Rectangle Opening", StringComparison.OrdinalIgnoreCase)) as FamilySymbol;


            return (roundOpeningFamilySymbol, rectangularOpeningFamilySymbol);
        }


        /// <summary>
        /// Lets the user select multiple elements of types (Walls, pipes, ducts) only, filters the walls alone and the ducts and pipes alone
        /// </summary>
        /// <param name="userPrompt">"The prompt that is shown to the user to know what he should do</param>
        /// <returns>A list of the user selected walls and a list of the user selected pipes and ducts in the same list</returns>
        public static (List<Wall> walls, List<MEPCurve> mepCurves) GetSelectedWallsDuctsAndPipes(
            string userPrompt = "Select walls, ducts and pipes")
        {
            // Let the user select, the selection should be walls, ducts and pipes only
            List<Element> selectedElements = SelectionHelpers.SelectElements(e => e is Wall || e is Duct || e is Pipe, "Select walls, ducts and pipes");

            // Filter out the walls from the selection
            List<Wall> selectedWalls = selectedElements.OfType<Wall>().ToList();

            // Filter out the ducts and pipes from the selection
            List<Duct> allDucts = selectedElements.OfType<Duct>().ToList();
            List<Pipe> selectedPipes = selectedElements.OfType<Pipe>().ToList();

            // Merge the ducts and pipes into one list
            List<MEPCurve> allPipesAndDucts = [.. allDucts, .. selectedPipes];

            return (selectedWalls, allPipesAndDucts);
        }

        /// <summary>
        /// Works only for ducts/pipes that are in one of those 2 cases:
        /// 1. The duct is normal to the wall on the xy plane and inclined in the z direction only
        /// 2. The duct has a constant Z value and is not normal to the wall in the XY plane only
        /// </summary>
        /// <param name="mepCurve">The duct/pipe concerned</param>
        /// <param name="intersectionPointWithFirstFace">The intersection of the pipe curve with the first face of the wall</param>
        /// <param name="intersectionPointWithSecondFace">The intersection of the pipe curve with the second face of the wall</param>
        /// <returns></returns>
        public static double GetTotalOpeningHeightForMepCurveInclinedIn2D(MEPCurve mepCurve, XYZ intersectionPointWithFirstFace, XYZ intersectionPointWithSecondFace)
        {

            // Get the delta Z (The height difference between when the duct entered the wall and where it exited it)
            double deltaZ = Math.Abs(intersectionPointWithFirstFace.Z - intersectionPointWithSecondFace.Z);

            // Get the angle between the duct and the z axis
            double verticalAngleBetweenDuctAndZ = mepCurve.GetLocationLine().Direction.AngleTo(XYZ.BasisZ);

            // When the duct is inclined in the z direction, we need to project its height on the z axis
            double projectedDuctHeight = MathHelpers.GetHypotenuseLength(verticalAngleBetweenDuctAndZ, mepCurve.GetHeightOrDiameter());

            // The total opening height is the sum of the delta Z and the projected duct height
            return deltaZ + projectedDuctHeight;
        }

        /// <summary>
        /// Works only for ducts/pipes that are in one of those 2 cases:
        /// 1. The duct is normal to the wall on the xy plane and inclined in the z direction only
        /// 2. The duct has a constant Z value and is not normal to the wall in the XY plane only
        /// </summary>
        /// <param name="mepCurve">The duct/pipe concerned</param>
        /// <param name="wall">The wall the duct/pipe passes through</param>
        /// <param name="intersectionPointWithFirstFace">The intersection of the pipe curve with the first face of the wall</param>
        /// <param name="intersectionPointWithSecondFace">The intersection of the pipe curve with the second face of the wall</param>
        /// <returns></returns>
        public static double GetTotalOpeningWidthForMepCurveInclinedIn2D
            (MEPCurve mepCurve, Wall wall, XYZ intersectionPointWithFirstFace, XYZ intersectionPointWithSecondFace)
        {
            // Get deltaWidth
            // Delta width is the difference between where the duct entered the wall and where it exited it PROJECTED on the wall line direction
            double deltaWidth = GetDeltaWidth(intersectionPointWithFirstFace, intersectionPointWithSecondFace, wall.GetLocationLine().Direction);

            // Project the duct width on the wall line direction
            double projectedDuctWidth = GetProjectedWidthOnWallForDuctInclinedIn2D
                (mepCurve, intersectionPointWithFirstFace, intersectionPointWithSecondFace, wall.GetLocationLine().Direction);
            
            // The total opening width is the sum of deltaWidth and the projected duct width
            return  deltaWidth + projectedDuctWidth;
        }


        /// <summary>
        /// Delta width is the difference between where the duct entered the wall and where it exited it PROJECTED on the wall line direction
        /// </summary>
        /// <param name="intersectionPointWithFirstFace">The intersection point between the duct center line and the first face of the wall</param>
        /// <param name="intersectionPointWithSecondFace">The intersection point between the duct center line and the first face of the wall</param>
        /// <param name="wallDirectionVector">The normalized direction of the wall location line on which the vector will be projected</param>
        /// <returns>The projection length from the point2 to point1 on the wall vector</returns>
        public static double GetDeltaWidth(XYZ intersectionPointWithFirstFace, XYZ intersectionPointWithSecondFace, XYZ wallDirectionVector)
        {
            XYZ? vectorBetweenFirstAndSecondPoints = intersectionPointWithSecondFace - intersectionPointWithFirstFace;
            XYZ? vectorBetweenFirstAndSecondPointsInXY = vectorBetweenFirstAndSecondPoints.ProjectVectorOnXY();
            double deltaWidth = wallDirectionVector.DotProduct(vectorBetweenFirstAndSecondPointsInXY);

            return Math.Abs(deltaWidth); 
        }


        /// <summary>
        /// Projects the duct width on the wall direction vector and returns the projected width
        /// </summary>
        /// <param name="mepCurve">The curve whose width you want to project</param>
        /// <param name="intersectionPointWithFirstFace">The intersection point between the duct center line and the first face of the wall</param>
        /// <param name="intersectionPointWithSecondFace">The intersection point between the duct center line and the second face of the wall</param>
        /// <param name="wallDirectionVector">The normalized direction of the wall location line on which the line will be projected</param>
        /// <returns></returns>
        public static double GetProjectedWidthOnWallForDuctInclinedIn2D
            (MEPCurve mepCurve, XYZ intersectionPointWithFirstFace, XYZ intersectionPointWithSecondFace, XYZ wallDirectionVector)
        {
            XYZ? vectorBetweenFirstAndSecondPoints = intersectionPointWithSecondFace - intersectionPointWithFirstFace;
            XYZ? vectorBetweenFirstAndSecondPointsInXY = vectorBetweenFirstAndSecondPoints.ProjectVectorOnXY();
            double horizontalAngleBetweenWallAndDuct = vectorBetweenFirstAndSecondPointsInXY.AngleTo(wallDirectionVector);
            double projectedDuctWidth = MathHelpers.GetHypotenuseLength(horizontalAngleBetweenWallAndDuct, mepCurve.GetWidthOrDiameter());

            return projectedDuctWidth;
        }

        /// <summary>
        /// <remarks>Must be within a transaction</remarks>
        /// Place opening family instances in the document based on a list of a custom dto class that contains the necessary information
        /// </summary>
        /// <param name="instancesInfo">A custom Dto class carrying the information needed to place an opening instance</param>
        /// <returns>A list of the created family instances</returns>
        public static List<FamilyInstance> CreateOpeningInstances(List<OpeningFamilyInstanceInfo> instancesInfo)
        {
            List<FamilyInstance> createdOpenings = [];
            instancesInfo.ForEach(i =>
            {
                FamilyInstance createdInstance =
                    DocHelpers.Doc.Create.NewFamilyInstance(i.InsertionPoint, i.FamilySymbol, i.InsertionDirection, i.HostWall, StructuralType.NonStructural);

                createdInstance.SetParameter("Width", i.Width);
                createdInstance.SetParameter("Height", i.Height);
                createdInstance.SetParameter("Wall thickness", i.WallThickness);

                createdOpenings.Add(createdInstance);
            });
            return createdOpenings;
        }

    }
}
