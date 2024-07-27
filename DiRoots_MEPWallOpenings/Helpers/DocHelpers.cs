using Autodesk.Revit.DB;

namespace DiRoots_MEPWallOpenings.Helpers
{
    public class DocHelpers
    {
        public static ExternalCommandData CommandData { get; private set; }
        public static UIDocument UiDoc { get; private set; }
        public static Document Doc { get; private set; }

        public static void FillByCommandData(ExternalCommandData commandData)
        {
            CommandData = commandData;
            UiDoc = commandData.Application.ActiveUIDocument;
            Doc = UiDoc.Document;
        }

        public static void FillByUiApplication(UIApplication uiApp)
        {
            UiDoc = uiApp.ActiveUIDocument;
            Doc = UiDoc.Document;
        }

    }
}
