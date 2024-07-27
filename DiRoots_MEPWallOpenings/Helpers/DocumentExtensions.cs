using Autodesk.Revit.DB;

namespace DiRoots_MEPWallOpenings.Helpers
{
    public static class DocumentExtensions
    {
        public static View3D GetDefault3DView(this Document doc)
            => new FilteredElementCollector(doc)
                .OfClass(typeof(View3D))
                .ToElements()
                .Cast<View3D>()
                .First(v => v.Name.Equals("{3D}") && !v.IsTemplate);
        public static void RunTransaction(
            this Document document, Action doAction, string transactionName = "Default transaction name")
        {
            using (var transaction = new Transaction(document, transactionName))
            {
                transaction.Start();
                doAction.Invoke();
                transaction.Commit();
            }
        }
        
    }
}