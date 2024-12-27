using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AnimationWindowEnhancer.Core
{
    /// <summary>
    /// Class to draw each curve associated with a DopeLine
    /// </summary>
    public class DopeLineCurveRenderer : IDisposable
    {
        private readonly Material _material;
        private readonly AnimationClip _clip;
        private readonly EditorCurveBinding _binding;
        private readonly Gradient _heatmap;

        private float _minTime;
        private float _maxTime;
        private float _minValue;
        private float _maxValue;
        private float[] _leftValues;
        private float[] _rightValues;
        private bool _isConstant;
        private int _hash;

        public DopeLineCurveRenderer(AnimationClip clip, EditorCurveBinding binding)
        {
            _clip = clip;
            _binding = binding;
            _material = new Material(Shader.Find("Hidden/Internal-Colored"));

            var valueName = binding.propertyName.Split('.').Last();
            var preferences = AnimationWindowEnhancerPreferences.instance;
            _heatmap = preferences.CurveHeatmapOverrides.Find(x => x.Name == valueName)?.Heatmap ?? preferences.DefaultCurveHeatmap;
        }

        public void Dispose()
        {
            UnityEngine.Object.DestroyImmediate(_material);
        }

        /// <summary>
        /// Draws the curve within the specified range
        /// </summary>
        public void Draw(Rect dopeLineRect, Rect dopeSheetRect, float dopeLineMinTime, float dopeLineMaxTime)
        {
            var animationCurve = AnimationUtility.GetEditorCurve(_clip, _binding);
            if (animationCurve == null)
            {
                return;
            }

            var hash = animationCurve.GetHashCode();
            if (_hash != hash)
            {
                _hash = hash;
                CacheValues(animationCurve);
            }

            var minTimeRate = Mathf.InverseLerp(dopeLineMinTime, dopeLineMaxTime, _minTime);
            var maxTimeRate = Mathf.InverseLerp(dopeLineMinTime, dopeLineMaxTime, _maxTime);
            var curveRect = new Rect(
                dopeLineRect.x + dopeLineRect.width * minTimeRate,
                dopeLineRect.y,
                dopeLineRect.width * (maxTimeRate - minTimeRate),
                dopeLineRect.height
            );

            GL.PushMatrix();
            GL.Begin(GL.LINE_STRIP);
            _material.SetPass(0);

            var xMin = Mathf.Max(0, curveRect.xMin);
            var xMax = Mathf.Min(dopeSheetRect.width, curveRect.xMax);

            if (_isConstant)
            {
                var y = curveRect.center.y;
                var color = _heatmap.Evaluate(0.5f);

                GL.Color(color);
                GL.Vertex3(xMin, y, 0);
                GL.Vertex3(xMax, y, 0);
            }
            else
            {
                var arraySize = _leftValues.Length;
                var n = arraySize - 1;

                // With n = (arraySize - 1),
                // x = rect.x + (rect.width * i / n)
                // Draw if x > 0,
                var begin = Mathf.Max(0, Mathf.CeilToInt(n * -curveRect.x / curveRect.width));
                // Draw if x < dopeSheetRect.width,
                var end = Mathf.Min(n, Mathf.FloorToInt(n * (dopeSheetRect.width - curveRect.x) / curveRect.width));

                if (begin <= end)
                {
                    // From screen edge to start point
                    if (begin >= 1)
                    {
                        var prevX = curveRect.x + curveRect.width * (begin - 1) / n;
                        var beginX = curveRect.x + curveRect.width * begin / n;
                        var edgeXRate = Mathf.InverseLerp(prevX, beginX, xMin);
                        var edgeValue = Mathf.Lerp(_rightValues[begin - 1], _leftValues[begin], edgeXRate);
                        var edgeYRate = Mathf.InverseLerp(_minValue, _maxValue, edgeValue);
                        var edgeY = Mathf.Lerp(curveRect.yMax, curveRect.yMin + 1, edgeYRate);
                        var edgeColor = _heatmap.Evaluate(edgeYRate);

                        GL.Color(edgeColor);
                        GL.Vertex3(xMin, edgeY, 0);

                        var beginYRate = Mathf.InverseLerp(_minValue, _maxValue, _leftValues[begin]);
                        var beginY = Mathf.Lerp(curveRect.yMax, curveRect.yMin + 1, beginYRate);
                        var beginColor = _heatmap.Evaluate(beginYRate);

                        GL.Color(beginColor);
                        GL.Vertex3(beginX, beginY, 0);
                    }

                    // Intermediate points
                    for (var i = begin; i <= end; i++)
                    {
                        var x = curveRect.x + curveRect.width * i / n;

                        var leftYRate = Mathf.InverseLerp(_minValue, _maxValue, _leftValues[i]);
                        var leftY = Mathf.Lerp(curveRect.yMax, curveRect.yMin + 1, leftYRate);
                        var leftColor = _heatmap.Evaluate(leftYRate);

                        GL.Color(leftColor);
                        GL.Vertex3(x, leftY, 0);

                        var rightYRate = Mathf.InverseLerp(_minValue, _maxValue, _rightValues[i]);
                        var rightY = Mathf.Lerp(curveRect.yMax, curveRect.yMin + 1, rightYRate);
                        var rightColor = _heatmap.Evaluate(rightYRate);

                        GL.Color(rightColor);
                        GL.Vertex3(x, rightY, 0);
                    }

                    // From end point to screen edge
                    if (end < arraySize - 1)
                    {
                        var endX = curveRect.x + curveRect.width * end / n;
                        var endYRate = Mathf.InverseLerp(_minValue, _maxValue, _rightValues[end]);
                        var endY = Mathf.Lerp(curveRect.yMax, curveRect.yMin + 1, endYRate);
                        var endColor = _heatmap.Evaluate(endYRate);

                        GL.Color(endColor);
                        GL.Vertex3(endX, endY, 0);

                        var nextX = curveRect.x + curveRect.width * (end + 1) / n;
                        var edgeXRate = Mathf.InverseLerp(endX, nextX, xMax);
                        var edgeValue = Mathf.Lerp(_rightValues[end], _leftValues[end + 1], edgeXRate);
                        var edgeYRate = Mathf.InverseLerp(_minValue, _maxValue, edgeValue);
                        var edgeY = Mathf.Lerp(curveRect.yMax, curveRect.yMin + 1, edgeYRate);
                        var edgeColor = _heatmap.Evaluate(edgeYRate);

                        GL.Color(edgeColor);
                        GL.Vertex3(xMax, edgeY, 0);
                    }
                }
            }

            GL.End();
            GL.PopMatrix();
        }

        /// <summary>
        /// Updates the cache
        /// </summary>
        private void CacheValues(AnimationCurve animationCurve)
        {
            // Get time range
            var keys = animationCurve.keys;
            _minTime = keys[0].time;
            _maxTime = keys[^1].time;
            var rangeTime = _maxTime - _minTime;

            // Adjust array size assuming value caching for each frame
            var resolution = AnimationWindowEnhancerPreferences.instance.CurveResolution;
            var arraySize = Mathf.RoundToInt(rangeTime * _clip.frameRate * resolution) + 1;
            ArrayUtility.EnsureArraySize(ref _leftValues, arraySize);
            ArrayUtility.EnsureArraySize(ref _rightValues, arraySize);

            // Get values
            EvaluateCurve(_minTime, _maxTime, animationCurve, _leftValues, _rightValues, out _minValue, out _maxValue, out _isConstant);
        }

        /// <summary>
        /// Gets the values from the AnimationCurve and stores them in the array
        /// </summary>
        private static void EvaluateCurve(
            float minTime, float maxTime, AnimationCurve animationCurve,
            float[] leftValues, float[] values, out float minValue, out float maxValue, out bool isConstant)
        {
            minValue = float.MaxValue;
            maxValue = float.MinValue;
            isConstant = true;

            var arraySize = leftValues.Length;
            for (var i = 0; i < arraySize; i++)
            {
                var xt = (float) i / (arraySize - 1);
                var time = Mathf.Lerp(minTime, maxTime, xt);

                // To draw the curve correctly for changes below 1 frame, also get a value with a slight time shift forward
                var eps = 1e-5f;
                var leftValue = animationCurve.Evaluate(time - eps);
                leftValues[i] = leftValue;

                var value = animationCurve.Evaluate(time + eps);
                minValue = Mathf.Min(minValue, value);
                maxValue = Mathf.Max(maxValue, value);
                values[i] = value;

                if (isConstant && !Mathf.Approximately(leftValue, value))
                {
                    isConstant = false;
                }
            }
        }
    }
}
