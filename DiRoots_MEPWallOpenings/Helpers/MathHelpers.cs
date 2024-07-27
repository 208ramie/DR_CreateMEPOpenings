namespace DiRoots_MEPWallOpenings.Helpers
{
    public static class MathHelpers
    {
        /// <summary>
        /// Get the length of the hypotenuse of a right triangle given the angle and the opposite side length
        /// </summary>
        /// <param name="angleInRad">Any of 2 acute angle</param>
        /// <param name="oppositeSideLength">The length of the side opposite to the angle</param>
        /// <returns>The side length of the hypotenuse</returns>
        public static double GetHypotenuseLength(double angleInRad, double oppositeSideLength)
            => oppositeSideLength / Math.Sin(angleInRad);
    }
}