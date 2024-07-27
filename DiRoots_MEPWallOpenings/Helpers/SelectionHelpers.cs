using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using DiRoots_MEPWallOpenings.SelectionFilters;

namespace DiRoots_MEPWallOpenings.Helpers
{
    public class SelectionHelpers
    {
        /// <summary>
        /// Prompts the user to select elements that satisfy a specific condition. 
        /// </summary>
        /// <param name="elementCondition"></param>
        /// <param name="prompt"></param>
        /// <param name="classType"></param>
        /// <returns></returns>
        public static List<Element> SelectElements(Predicate<Element> elementCondition, string prompt = "")
        {
            // Create a selection filter with this predicate
            BaseSelectionFilter selectionFilter = new(elementCondition);
            // Pick the objects you want
            IList<Reference>? references = DocHelpers.UiDoc.Selection.PickObjects(ObjectType.Element, selectionFilter, prompt);
            // Convert the references into elements
            List<Element> selectedElements = references.Select(r => DocHelpers.Doc.GetElement(r.ElementId)).ToList();
            return selectedElements;
        }
    }
}