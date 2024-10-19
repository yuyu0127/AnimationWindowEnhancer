using AnimationWindowEnhancer.InternalAPIProxy;
using UnityEditor;
using UnityEngine;

namespace AnimationWindowEnhancer.Core
{
    public class CurveLabelRenderer
    {
        private readonly CurveWrapperProxy _curveWrapper;
        private readonly CurveEditorProxy _curveEditor;

        private int _hash;
        private float _minTime;
        private float _maxTime;
        private float _minValue;
        private float _maxValue;

        public CurveLabelRenderer(CurveEditorProxy curveEditor, CurveWrapperProxy curveWrapper)
        {
            _curveEditor = curveEditor;
            _curveWrapper = curveWrapper;
        }

        public void Draw(float currentTime)
        {
            var hash = _curveWrapper.curve.GetHashCode();
            if (_hash != hash)
            {
                _hash = hash;
                RefreshCache();
            }

            // Convert drawing area to local GUI area
            var minDrawing = _curveWrapper.bounds.min;
            var maxDrawing = _curveWrapper.bounds.max;
            var minView = _curveEditor.DrawingToViewTransformPoint(minDrawing);
            var maxView = _curveEditor.DrawingToViewTransformPoint(maxDrawing);
            var minLocal = HandleUtility.WorldToGUIPoint(minView);
            var maxLocal = HandleUtility.WorldToGUIPoint(maxView);

            // Y Coordinate is flipped
            var rect = new Rect(minLocal.x, maxLocal.y, maxLocal.x - minLocal.x, minLocal.y - maxLocal.y);

            // Calculate label position from the current time and value
            var currentValue = _curveWrapper.curve.Evaluate(currentTime);
            var timeRate = Mathf.InverseLerp(_minTime, _maxTime, currentTime);
            var valueRate = Mathf.InverseLerp(_minValue, _maxValue, currentValue);
            var labelPos = new Vector2(rect.x + rect.width * timeRate, rect.yMax - rect.height * valueRate);

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

            var binding = _curveWrapper.binding;
            var text = string.IsNullOrEmpty(binding.path)
                ? binding.propertyName
                : $"{binding.path} : {binding.propertyName}";
            GUI.Label(new Rect(labelPos.x, labelPos.y - 14, 800, 12), text, style);
        }

        private void RefreshCache()
        {
            var curve = _curveWrapper.curve;
            _minTime = curve.keys[0].time;
            _maxTime = curve.keys[^1].time;
            _minValue = float.MaxValue;
            _maxValue = float.MinValue;

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
