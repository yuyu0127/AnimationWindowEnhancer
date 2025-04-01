using UnityEngine;

namespace AnimationWindowEnhancer.Core
{
    public static class GradientUtility
    {
        /// <summary>
        /// Generates a gradient from the start and end colors
        /// </summary>
        public static Gradient FromBeginEnd(Color begin, Color end)
        {
            return new Gradient
            {
                colorKeys = new[]
                {
                    new GradientColorKey(begin, 0),
                    new GradientColorKey(end, 1),
                },
                alphaKeys = new[]
                {
                    new GradientAlphaKey(begin.a, 0),
                    new GradientAlphaKey(end.a, 1),
                },
                mode = GradientMode.PerceptualBlend
            };
        }
    }
}
