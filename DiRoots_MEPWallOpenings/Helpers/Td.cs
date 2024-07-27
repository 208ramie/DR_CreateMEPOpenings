namespace DiRoots_MEPWallOpenings.Helpers
{
    /// <summary>
    /// A helper class for the taskdialog
    /// </summary>
    public static class Td
    {
        public static void Show(string body, string title = "RPlugin")
        {
            TaskDialog.Show(title, body);
        }



        public static void ShowError(string mainInstruction, string errorMessage = "", string detailedError = null, string title = "Error")
        {
            TaskDialog td = new TaskDialog(title)
            {
                MainIcon = TaskDialogIcon.TaskDialogIconError,
                TitleAutoPrefix = false,
                MainInstruction = mainInstruction,
                MainContent = errorMessage
            };

            if (!string.IsNullOrEmpty(detailedError))
            {
                td.ExpandedContent = "Detailed Error Information:\n" + detailedError;
            }

            td.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, "OK");

            if (!string.IsNullOrEmpty(detailedError))
            {
                td.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, "Copy Error Details to Clipboard");
            }

            TaskDialogResult result = td.Show();

            if (result == TaskDialogResult.CommandLink2)
            {
                System.Windows.Clipboard.SetText(detailedError);
            }
        }
    }
}