using System;
using UnityEditor;
using UnityEngine;

namespace AnimationWindowEnhancer.Core
{
    /// <summary>
    /// Class to draw a gradient for DopeLines representing color
    /// </summary>
    public class DopeLineGradientRenderer : IDisposable
    {
        private readonly AnimationClip _clip;
        private readonly EditorCurveBinding _bindingR;
        private readonly EditorCurveBinding _bindingG;
        private readonly EditorCurveBinding _bindingB;
        private readonly EditorCurveBinding _bindingA;
        private readonly Material _material;

        private Color[] _leftColors;
        private Color[] _rightColors;
        private bool _isConstant;
        private int _hash;

        public DopeLineGradientRenderer(AnimationClip clip, EditorCurveBinding bindingR, EditorCurveBinding bindingG, EditorCurveBinding bindingB, EditorCurveBinding bindingA)
        {
            _clip = clip;
            _bindingR = bindingR;
            _bindingG = bindingG;
            _bindingB = bindingB;
            _bindingA = bindingA;
            _material = new Material(Shader.Find("Hidden/Internal-Colored"));
        }

        public void Dispose()
        {
            UnityEngine.Object.DestroyImmediate(_material);
        }

        /// <summary>
        /// Draws the gradient within the specified range
        /// </summary>
        public void Draw(Rect dopeLineRect, Rect dopeSheetRect)
        {
            // Get the curves for R, G, B, A
            var animationCurveR = AnimationUtility.GetEditorCurve(_clip, _bindingR);
            var animationCurveG = AnimationUtility.GetEditorCurve(_clip, _bindingG);
            var animationCurveB = AnimationUtility.GetEditorCurve(_clip, _bindingB);
            var animationCurveA = AnimationUtility.GetEditorCurve(_clip, _bindingA);

            // Update cache if the content has changed
            var hash = HashCode.Combine(animationCurveR, animationCurveG, animationCurveB, animationCurveA);
            if (_hash != hash)
            {
                _hash = hash;
                CacheValues(animationCurveR, animationCurveG, animationCurveB, animationCurveA);
            }

            GL.PushMatrix();
            GL.Begin(GL.QUADS);
            _material.SetPass(0);

            var xMin = Mathf.Max(0, dopeLineRect.xMin);
            var xMax = Mathf.Min(dopeSheetRect.width, dopeLineRect.xMax);

            var height = AnimationWindowEnhancerPreferences.instance.ColorBandHeight;
            var yMin = dopeLineRect.yMax - height;
            var yMax = dopeLineRect.yMax;

            if (_isConstant)
            {
                var color = _rightColors[0];

                GL.Color(color);

                GL.Vertex3(xMin, yMin, 0);
                GL.Vertex3(xMin, yMax, 0);
                GL.Vertex3(xMax, yMax, 0);
                GL.Vertex3(xMax, yMin, 0);
            }
            else
            {
                var arraySize = _leftColors.Length;
                var n = arraySize - 1;

                // With n = (arraySize - 1),
                // x = rect.x + (rect.width * i / n)
                // Draw if x > 0,
                var begin = Mathf.Max(0, Mathf.CeilToInt(n * -dopeLineRect.x / dopeLineRect.width));
                // Draw if x < dopeSheetRect.width,
                var end = Mathf.Min(n, Mathf.FloorToInt(n * (dopeSheetRect.width - dopeLineRect.x) / dopeLineRect.width));

                if (begin <= end)
                {
                    // From screen edge to start point
                    if (begin >= 1)
                    {
                        var prevX = dopeLineRect.x + dopeLineRect.width * (begin - 1) / n;
                        var beginX = dopeLineRect.x + dopeLineRect.width * begin / n;
                        var edgeXRate = Mathf.InverseLerp(prevX, beginX, xMin);

                        var color0 = Color.Lerp(_rightColors[begin - 1], _leftColors[begin], edgeXRate);
                        var color1 = _leftColors[begin];

                        // Bottom left vertex
                        GL.Color(color0);
                        GL.Vertex3(xMin, yMin, 0);

                        // Top left vertex
                        GL.Color(color0);
                        GL.Vertex3(xMin, yMax, 0);

                        // Top right vertex
                        GL.Color(color1);
                        GL.Vertex3(beginX, yMax, 0);

                        // Bottom right vertex
                        GL.Color(color1);
                        GL.Vertex3(beginX, yMin, 0);
                    }

                    for (var i = begin; i <= end - 1; i++)
                    {
                        var x0 = dopeLineRect.x + dopeLineRect.width * i / n;
                        var x1 = dopeLineRect.x + dopeLineRect.width * (i + 1) / n;

                        var color0 = _rightColors[i];
                        var color1 = _leftColors[i + 1];

                        // Bottom left vertex
                        GL.Color(color0);
                        GL.Vertex3(x0, yMin, 0);

                        // Top left vertex
                        GL.Color(color0);
                        GL.Vertex3(x0, yMax, 0);

                        // Top right vertex
                        GL.Color(color1);
                        GL.Vertex3(x1, yMax, 0);

                        // Bottom right vertex
                        GL.Color(color1);
                        GL.Vertex3(x1, yMin, 0);
                    }

                    // From end point to screen edge
                    if (end < arraySize - 1)
                    {
                        var endX = dopeLineRect.x + dopeLineRect.width * end / n;
                        var nextX = dopeLineRect.x + dopeLineRect.width * (end + 1) / n;

                        var color0 = _rightColors[end];
                        var color1 = Color.Lerp(_rightColors[end], _leftColors[end + 1], Mathf.InverseLerp(endX, nextX, xMax));

                        // Bottom left vertex
                        GL.Color(color0);
                        GL.Vertex3(endX, yMin, 0);

                        // Top left vertex
                        GL.Color(color0);
                        GL.Vertex3(endX, yMax, 0);

                        // Top right vertex
                        GL.Color(color1);
                        GL.Vertex3(xMax, yMax, 0);

                        // Bottom right vertex
                        GL.Color(color1);
                        GL.Vertex3(xMax, yMin, 0);
                    }
                }
            }

            GL.End();
            GL.PopMatrix();
        }

        private void CacheValues(AnimationCurve animationCurveR, AnimationCurve animationCurveG, AnimationCurve animationCurveB, AnimationCurve animationCurveA)
        {
            // Get time range
            var keysR = animationCurveR.keys;
            var keysG = animationCurveG.keys;
            var keysB = animationCurveB.keys;
            var keysA = animationCurveA.keys;
            var minTime = Mathf.Min(keysR[0].time, keysG[0].time, keysB[0].time, keysA[0].time);
            var maxTime = Mathf.Max(keysR[^1].time, keysG[^1].time, keysB[^1].time, keysA[^1].time);
            var rangeTime = maxTime - minTime;

            // Adjust array size assuming value caching for each frame
            var resolution = AnimationWindowEnhancerPreferences.instance.CurveResolution;
            var arraySize = Mathf.RoundToInt(rangeTime * _clip.frameRate * resolution) + 1;
            ArrayUtility.EnsureArraySize(ref _leftColors, arraySize);
            ArrayUtility.EnsureArraySize(ref _rightColors, arraySize);

            // Get values
            EvaluateCurves(minTime, maxTime, animationCurveR, animationCurveG, animationCurveB, animationCurveA, _leftColors, _rightColors, out _isConstant);
        }

        /// <summary>
        /// Gets the values from the AnimationCurve and stores them in the array
        /// </summary>
        private static void EvaluateCurves(
            float minTime, float maxTime,
            AnimationCurve animationCurveR, AnimationCurve animationCurveG, AnimationCurve animationCurveB, AnimationCurve animationCurveA,
            Color[] leftColors, Color[] rightColors, out bool isConstant)
        {
            isConstant = true;

            var arraySize = leftColors.Length;
            for (var i = 0; i < arraySize; i++)
            {
                var xt = (float) i / (arraySize - 1);
                var time = Mathf.Lerp(minTime, maxTime, xt);

                // To draw the gradient correctly for changes below 1 frame, also get a value with a slight time shift forward
                var eps = 1e-5f;
                var leftColor = new Color(
                    animationCurveR.Evaluate(time - eps),
                    animationCurveG.Evaluate(time - eps),
                    animationCurveB.Evaluate(time - eps),
                    animationCurveA.Evaluate(time - eps)
                );
                leftColors[i] = leftColor;

                var rightColor = new Color(
                    animationCurveR.Evaluate(time + eps),
                    animationCurveG.Evaluate(time + eps),
                    animationCurveB.Evaluate(time + eps),
                    animationCurveA.Evaluate(time + eps)
                );
                rightColors[i] = rightColor;

                if (isConstant && leftColor != rightColor)
                {
                    isConstant = false;
                }
            }
        }
    }
}
