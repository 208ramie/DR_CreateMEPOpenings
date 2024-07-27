namespace DiRoots_MEPWallOpenings.Helpers
{
    public static class ExternalApplicationHelpers
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buttonName"></param>
        /// <param name="ribbonPanel"></param>
        /// <param name="fullClassName"></param>
        /// <param name="toolTip"></param>
        /// <param name="iconPath"></param>
        /// <param name="assemblyPath">Assembly.GetExecutingAssembly()</param>
        public static void CreateButton
        (string buttonName, RibbonPanel ribbonPanel, string fullClassName, string toolTip, string? iconPath, Assembly? executingAssembly = null)
        {
            executingAssembly ??= Assembly.GetExecutingAssembly();

            // Get the assembly path
            string assemblyPath = executingAssembly.Location;

            // Create the push button data (A Push button not assigned to any ribbon panel)
            PushButtonData pushButtonData =
                new PushButtonData(buttonName, buttonName, assemblyPath, fullClassName);

            // Create a push button out of the push button data by adding it into a panel
            PushButton pushButton = (ribbonPanel.AddItem(pushButtonData) as PushButton)!;

            // Set the tooltip
            pushButton.ToolTip = toolTip;


            if (iconPath is not null)
            {
                try
                {
                    // Get the current assembly name to be used in the icon URI
                    string assemblyName = executingAssembly.GetName().Name!;

                    // Create a new bitmap image from the icon path
                    BitmapImage bitmapImage =
                        new BitmapImage(new Uri($"pack://application:,,,/{assemblyName};component/{iconPath}"));

                    // Set the large image of the push button
                    pushButton.LargeImage = bitmapImage;
                }
                catch (Exception)
                {
                }

            }

        }

    }
}