using Autodesk.Revit.DB;

namespace DiRoots_MEPWallOpenings.Helpers
{
    public static class ElementExtensions
    {

        /// <summary>
        /// Retrieves the location curve of an element
        /// </summary>
        /// <param name="element">The Curve based element you want to get the location for</param>
        /// <returns>Curve if it is a curve based element</returns>
        /// <exception cref="Exception">If it isn't a point based element it throws an exception</exception>
        public static Curve GetLocationCurve(this Element element)
        {
            // Assume the element's location is a location point
            LocationCurve? location = element.Location as LocationCurve;

            // If it is not a location curve, the value will be null, in this case throw an exception
            if (location is null)
                throw new Exception("This is not a curve based element\nLocation curve couldn't be retrieved");

            // If it is a location curve just return it
            return location.Curve;
        }

        /// <summary>
        /// Retrieves the location line of an element
        /// </summary>
        /// <param name="element">The Line based element you want to get teh location for </param>
        /// <returns>Line if it is a Line based element</returns>
        /// <exception cref="Exception">If the element is not a line based element (Curve based or line based)</exception>
        public static Line GetLocationLine(this Element element)
        {
            Curve locationCurve = element.GetLocationCurve();

            if (locationCurve is Line line)
                return line;

            throw new Exception("The element's location is not a line");
        }

        /// <summary>
        /// Get the first solid from any element
        /// </summary>
        /// <param name="element">The element you want to extract the solid from</param>
        /// <returns>The First solid of the element</returns>
        public static Solid? GetSolid(this Element element)
        {
            Options options = new Options
            {
                ComputeReferences = true,
                DetailLevel = ViewDetailLevel.Fine
            };

            GeometryElement geometryElement = element.get_Geometry(options);
            return geometryElement.FirstOrDefault() as Solid;
        }

        /// <summary>
        /// Sets a specific parameter by its name
        /// </summary>
        /// <typeparam name="T">The type of the value assigned to the parameter</typeparam>
        /// <param name="element">The element you want to change its parameter</param>
        /// <param name="parameterName">The name of the parameter</param>
        /// <param name="newValue">The new value for the parameter</param>
        /// <returns>True if the setting process worked, false if it didn't</returns>
        public static bool SetParameter<T>(this Element element, string parameterName, T newValue)
        {
            Parameter parameter = element.LookupParameter(parameterName);
            if (parameter == null) 
                return false;

            return newValue switch
            {
                int intValue => parameter.Set(intValue),
                double doubleValue => parameter.Set(doubleValue),
                string stringValue => parameter.Set(stringValue),
                ElementId elementId => parameter.Set(elementId),
                _ => false
            };
        }
        
    }
}