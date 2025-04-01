using System;
using System.Linq;
using AnimationWindowEnhancer.InternalAPIProxy;
using UnityEditor;
using UnityEngine;

namespace AnimationWindowEnhancer.Core
{
    /// <summary>
    /// Class to draw each DopeLine
    /// </summary>
    public class DopeLineRenderer : IDisposable
    {
        private readonly DopeLineProxy _dopeLine;
        private readonly DopeLineCurveRenderer[] _curveRenderers;
        private readonly DopeLineGradientRenderer _gradientRenderer;
        private readonly GUIContent _labelContent;
        private readonly Lazy<GUIStyle> _labelStyle = new(() =>
        {
            var preferences = AnimationWindowEnhancerPreferences.instance;
            var color = preferences.LabelColor;
            return new GUIStyle(EditorStyles.miniLabel)
            {
                fontSize = preferences.LabelFontSize,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = color },
                hover = { textColor = color },
                active = { textColor = color },
                focused = { textColor = color },
                onNormal = { textColor = color },
                onHover = { textColor = color },
                onActive = { textColor = color },
                onFocused = { textColor = color },
            };
        });

        public DopeLineRenderer(AnimationClip clip, DopeLineProxy dopeLine)
        {
            _dopeLine = dopeLine;

            var curves = _dopeLine.curves.ToArray();

            // If there are 4 curves and each property name ends with .r, .g, .b, .a, draw a gradient
            var isColorProperty =
                curves.Length == 4 &&
                curves[0].binding.propertyName.EndsWith(".r") &&
                curves[1].binding.propertyName.EndsWith(".g") &&
                curves[2].binding.propertyName.EndsWith(".b") &&
                curves[3].binding.propertyName.EndsWith(".a");

            if (isColorProperty)
            {
                _gradientRenderer = new DopeLineGradientRenderer(clip, curves[0].binding, curves[1].binding, curves[2].binding, curves[3].binding);
            }
            else
            {
                _curveRenderers = new DopeLineCurveRenderer[curves.Length];
                for (var i = 0; i < curves.Length; i++)
                {
                    _curveRenderers[i] = new DopeLineCurveRenderer(clip, curves[i].binding);
                }
            }

            // Use the common part of all property names as the label
            var propertyNames = curves.Select(x => x.binding.propertyName).ToList();
            var commonPropertyName = StringUtility.GetLongestCommonPrefix(propertyNames).TrimEnd('.');
            var objectName = curves.First().binding.path.Split('/').Last();
            var labelText = !string.IsNullOrEmpty(objectName)
                ? objectName + " : " + commonPropertyName
                : commonPropertyName;
            _labelContent = new GUIContent(labelText);
        }

        public void Dispose()
        {
            _gradientRenderer?.Dispose();
            foreach (var curveRenderer in _curveRenderers)
            {
                curveRenderer?.Dispose();
            }
        }

        public void Draw(DopeSheetEditorProxy dopeSheetEditor, Vector2 scrollPos)
        {
            if (!AnimationWindowEnhancerPreferences.instance.DopesheetShowLabel &&
                !AnimationWindowEnhancerPreferences.instance.DopesheetShowCurve)
            {
                return;
            }

            // Get the drawing area
            GetVisibleRange(_dopeLine, dopeSheetEditor, scrollPos, out var minTime, out var maxTime, out var dopeLineRect);
            if (dopeLineRect.width <= 0)
            {
                return;
            }

            // Do not draw if the Y-axis is out of range
            if (dopeLineRect.yMax < 0)
            {
                return;
            }
            var dopeSheetRect = dopeSheetEditor.rect;
            if (dopeSheetRect.height < dopeLineRect.yMax)
            {
                return;
            }

            if (_dopeLine.hasChildren)
            {
                var maskRect = new Rect(0, dopeLineRect.y, dopeSheetRect.width, dopeLineRect.height);
                var parentDopeLineColor = AnimationWindowEnhancerPreferences.instance.ParentDopeLineColor;
                EditorGUI.DrawRect(maskRect, parentDopeLineColor);
            }

            // Draw label
            if (AnimationWindowEnhancerPreferences.instance.DopesheetShowLabel)
            {
                DrawLabel(dopeLineRect, dopeSheetRect);
            }

            if (AnimationWindowEnhancerPreferences.instance.DopesheetShowCurve)
            {
                // Draw gradient
                if (_gradientRenderer != null)
                {
                    _gradientRenderer.Draw(dopeLineRect, dopeSheetRect);
                }
                // Draw curves
                else
                {
                    foreach (var curveRenderer in _curveRenderers)
                    {
                        curveRenderer?.Draw(dopeLineRect, dopeSheetRect, minTime, maxTime);
                    }
                }
            }
        }

        /// <summary>
        /// Draw the label at a suitable position
        /// </summary>
        private void DrawLabel(Rect rect, Rect dopeSheetRect)
        {
            var labelCenter = rect.center.x;
            var labelWidth = _labelStyle.Value.CalcSize(_labelContent).x;
            var labelLeft = labelCenter - labelWidth / 2;
            var labelRight = labelCenter + labelWidth / 2;
            if (labelLeft < 0)
            {
                labelRight -= labelLeft;
                labelLeft = 0;
            }
            if (labelRight > dopeSheetRect.width)
            {
                labelLeft -= labelRight - dopeSheetRect.width;
                labelRight = dopeSheetRect.width;
            }
            var labelRect = new Rect(labelLeft, rect.y, labelRight - labelLeft, rect.height);
            EditorGUI.LabelField(labelRect, _labelContent, _labelStyle.Value);
        }

        /// <summary>
        /// Get the area to draw the curve
        /// </summary>
        private static void GetVisibleRange(
            DopeLineProxy dopeLine, DopeSheetEditorProxy dopeSheetEditor, Vector2 scrollPos,
            out float minTime, out float maxTime, out Rect curveRect)
        {
            var firstKey = dopeLine.keys.First();
            var firstKeyRect = dopeSheetEditor.GetKeyframeRect(dopeLine, firstKey);

            var lastKey = dopeLine.keys.Last();
            var lastKeyRect = dopeSheetEditor.GetKeyframeRect(dopeLine, lastKey);

            minTime = firstKey.time;
            maxTime = lastKey.time;
            curveRect = new Rect(
                firstKeyRect.x + 5,
                firstKeyRect.y - scrollPos.y,
                lastKeyRect.x - firstKeyRect.x + 1,
                firstKeyRect.height - 2
            );
        }
    }
}
