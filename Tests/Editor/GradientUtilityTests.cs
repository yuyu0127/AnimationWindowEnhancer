using NUnit.Framework;
using UnityEngine;

namespace AnimationWindowEnhancer.Core.Tests
{
    public class GradientUtilityTests
    {
        [Test]
        public void FromBeginEnd_SetsCorrectColorKeys()
        {
            var begin = new Color(1, 0, 0, 1);
            var end = new Color(0, 0, 1, 1);

            var gradient = GradientUtility.FromBeginEnd(begin, end);
            var colorKeys = gradient.colorKeys;

            Assert.AreEqual(2, colorKeys.Length);
            Assert.AreEqual(0f, colorKeys[0].time);
            Assert.AreEqual(1f, colorKeys[1].time);
            Assert.AreEqual(begin, (Color)colorKeys[0].color);
            Assert.AreEqual(end, (Color)colorKeys[1].color);
        }

        [Test]
        public void FromBeginEnd_SetsCorrectAlphaKeys()
        {
            var begin = new Color(1, 0, 0, 0.25f);
            var end = new Color(0, 0, 1, 0.75f);

            var gradient = GradientUtility.FromBeginEnd(begin, end);
            var alphaKeys = gradient.alphaKeys;

            Assert.AreEqual(2, alphaKeys.Length);
            Assert.AreEqual(0f, alphaKeys[0].time);
            Assert.AreEqual(1f, alphaKeys[1].time);
            Assert.AreEqual(begin.a, alphaKeys[0].alpha, 0.001f);
            Assert.AreEqual(end.a, alphaKeys[1].alpha, 0.001f);
        }

        [Test]
        public void FromBeginEnd_EvaluateAtMidpoint_ReturnsInterpolatedColor()
        {
            var begin = Color.black;
            var end = Color.white;

            var gradient = GradientUtility.FromBeginEnd(begin, end);
            var mid = gradient.Evaluate(0.5f);

            // Perceptual blend may not be exact 0.5 linear, but should be somewhere in between
            Assert.Greater(mid.r, 0f);
            Assert.Less(mid.r, 1f);
        }

        [Test]
        public void FromBeginEnd_EvaluateAtBoundaries_ReturnsExactColors()
        {
            var begin = new Color(1, 0, 0, 1);
            var end = new Color(0, 0, 1, 1);

            var gradient = GradientUtility.FromBeginEnd(begin, end);

            var evalBegin = gradient.Evaluate(0f);
            Assert.AreEqual(begin.r, evalBegin.r, 0.001f);
            Assert.AreEqual(begin.g, evalBegin.g, 0.001f);
            Assert.AreEqual(begin.b, evalBegin.b, 0.001f);

            var evalEnd = gradient.Evaluate(1f);
            Assert.AreEqual(end.r, evalEnd.r, 0.001f);
            Assert.AreEqual(end.g, evalEnd.g, 0.001f);
            Assert.AreEqual(end.b, evalEnd.b, 0.001f);
        }
    }
}
