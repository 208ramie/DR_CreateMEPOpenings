using DiRoots_MEPWallOpenings.RevitCommands;

namespace DiRoots_MEPWallOpenings.ExternalApplication
{
    public class ExternalApp : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            // Tab => Panel => Button
            // Define the tab name
            const string tabName = "Test";
            // Create a tab
            application.CreateRibbonTab(tabName);
            // Create panel inside the tab
            RibbonPanel testPanel = application.CreateRibbonPanel(tabName, "TestPanel");

            // Create a button inside the panel
            ExternalApplicationHelpers.CreateButton("Create Openings", testPanel, typeof(CreateOpeningsRCommand).FullName!,
                "Description", "Resources/CreateDuctOpenings.ico");

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}