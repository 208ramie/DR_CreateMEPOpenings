using Autodesk.Revit.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Structure;
using DiRoots_MEPWallOpenings.RevitExternalEventHandlers;
using DiRoots_MEPWallOpenings.Views;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;

namespace DiRoots_MEPWallOpenings.RevitCommands
{
    [Transaction(TransactionMode.Manual)]
    public class CreateOpeningsRCommand : IExternalCommand
    {
        // Define an event and an event handler
        public static CreateMEPOpeningsEventHandler MEPOpeningsEventHandler { get; set; }
        public static ExternalEvent MEPOpeningsEvent { get; set; }





        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get references to the currently open Revit document
            DocHelpers.FillByCommandData(commandData);

            // Create an instance of the event handler class
            MEPOpeningsEventHandler = new CreateMEPOpeningsEventHandler();
            // Create an instance of the external event class and link it to the created instance of the event handler
            MEPOpeningsEvent = ExternalEvent.Create(MEPOpeningsEventHandler);

            // Show the window
            CreateMEPOpeningsView.Instance.Show();

            return Result.Succeeded;
        }
    }
}
