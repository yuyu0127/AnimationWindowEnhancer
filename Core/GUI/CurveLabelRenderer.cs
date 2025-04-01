using AnimationWindowEnhancer.InternalAPIProxy;
using UnityEditor;
using UnityEngine;

namespace AnimationWindowEnhancer.Core
{
    public class CurveLabelRenderer
    {
        private readonly CurveWrapperProxy _curveWrapper;
        private readonly CurveEditorProxy _curveEditor;
        private readonly GUIContent _labelContent;

        private int _hash;
        private float _minTime;
        private float _maxTime;
        private float _minValue;
        private float _maxValue;
        private Bounds _bounds;

        public CurveLabelRenderer(CurveEditorProxy curveEditor, CurveWrapperProxy curveWrapper)
        {
            _curveEditor = curveEditor;
            _curveWrapper = curveWrapper;
            var binding = _curveWrapper.binding;
            var text = string.IsNullOrEmpty(binding.path)
                ? binding.propertyName
                : $"{binding.path} : {binding.propertyName}";
            _labelContent = new GUIContent(text);
        }

        public void Draw(float currentTime, Rect curveEditorRect)
        {
            if (!AnimationWindowEnhancerPreferences.instance.CurvesShowLabel)
            {
                return;
            }

            var hash = _curveWrapper.curve.GetHashCode();
            if (_hash != hash)
            {
                _hash = hash;
                RefreshCache();
            }

            // Convert drawing area to local GUI area
            var minView = _curveEditor.DrawingToViewTransformPoint(_bounds.min);
            var maxView = _curveEditor.DrawingToViewTransformPoint(_bounds.max);
            var minLocal = HandleUtility.WorldToGUIPoint(minView);
            var maxLocal = HandleUtility.WorldToGUIPoint(maxView);
            // Y Coordinate is flipped
            var rect = new Rect(minLocal.x, maxLocal.y, maxLocal.x - minLocal.x, minLocal.y - maxLocal.y);

            // Calculate label position from the current time and value
            var currentValue = _curveWrapper.curve.Evaluate(currentTime);
            var timeRate = Mathf.InverseLerp(_minTime, _maxTime, currentTime);
            var valueRate = Mathf.InverseLerp(_minValue, _maxValue, currentValue);
            var labelPos = new Vector2(rect.x + rect.width * timeRate, rect.yMax - rect.height * valueRate - 14f);
            var labelWidth = EditorStyles.miniLabel.CalcSize(_labelContent).x;
            var labelRect = new Rect(labelPos.x, labelPos.y, labelWidth, 12f);

            // Clamp by curveEditorRect
            var clampRect = Rect.MinMaxRect(0, 0, curveEditorRect.width - 14, curveEditorRect.height - 14);
            using (new GUI.ClipScope(clampRect))
            {
                // Draw label
                var labelColor = new Color(_curveWrapper.color.r, _curveWrapper.color.g, _curveWrapper.color.b, 0.5f);
                var style = new GUIStyle(EditorStyles.miniLabel)
                {
                    alignment = TextAnchor.MiddleLeft,
                    normal = { textColor = labelColor },
                    active = { textColor = labelColor },
                    hover = { textColor = labelColor },
                    focused = { textColor = labelColor },
                    onNormal = { textColor = labelColor },
                    onActive = { textColor = labelColor },
                    onHover = { textColor = labelColor },
                    onFocused = { textColor = labelColor }
                };
                GUI.Label(labelRect, _labelContent, style);
            }
        }

        private void RefreshCache()
        {
            var curve = _curveWrapper.curve;
            _minTime = curve.keys[0].time;
            _maxTime = curve.keys[^1].time;
            _minValue = float.MaxValue;
            _maxValue = float.MinValue;
            _bounds = _curveWrapper.ComputeBoundsBetweenTime(_minTime, _maxTime);

            var rangeTime = _maxTime - _minTime;
            var resolution = AnimationWindowEnhancerPreferences.instance.CurveResolution;
            var arraySize = Mathf.RoundToInt(rangeTime * _curveWrapper.animationClip.frameRate * resolution) + 1;
            for (var i = 0; i < arraySize; i++)
            {
                var xt = (float) i / (arraySize - 1);
                var time = Mathf.Lerp(_minTime, _maxTime, xt);
                var value = curve.Evaluate(time);
                _minValue = Mathf.Min(_minValue, value);
                _maxValue = Mathf.Max(_maxValue, value);
            }
        }
    }
}
