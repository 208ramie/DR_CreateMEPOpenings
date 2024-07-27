using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace DiRoots_MEPWallOpenings.SelectionFilters
{
    public class BaseSelectionFilter : ISelectionFilter
    {
        private readonly Predicate<Element> _elementCondition;

        public BaseSelectionFilter(Predicate<Element> elementCondition)
        {
            _elementCondition = elementCondition;
        }

        public bool AllowElement(Element elem)
        {
            return _elementCondition(elem);
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return true;
        }
    }
}