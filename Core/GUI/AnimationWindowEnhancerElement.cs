using System.Collections.Generic;
using AnimationWindowEnhancer.InternalAPIProxy;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AnimationWindowEnhancer.Core
{
    /// <summary>
    /// An element that draws curves as an overlay on the AnimationWindow's drawing area
    /// </summary>
    public class AnimationWindowEnhancerElement : IMGUIContainer
    {
        private readonly AnimationWindow _target;
        private readonly Dictionary<DopeLineProxy, DopeLineRenderer> _dopeLineRenderers = new();

        private bool _isStyleDirty = true;
        private float[] _values;
        private AnimationClip _prevClip;

        public AnimationWindowEnhancerElement(AnimationWindow target)
        {
            _target = target;
            pickingMode = PickingMode.Ignore; // Tap pass-through
            onGUIHandler = OnGUI;
        }

        private void OnGUI()
        {
            var clip = _target.animationClip;
            if (clip == null)
            {
                return;
            }

            // Clear cache if the edited AnimationClip has changed
            if (_prevClip != clip)
            {
                _prevClip = clip;
                _dopeLineRenderers.Clear();
            }

            if (_isStyleDirty)
            {
                _isStyleDirty = false;
                MarkDirtyLayout();
            }

            var window = new AnimationWindowProxy(_target);
            if (window.animEditor.state.showCurveEditor)
            {
                OnCurveGUI();
            }
            else
            {
                OnDopeSheetGUI();
            }
        }

        private void OnCurveGUI()
        {
            var window = new AnimationWindowProxy(_target);
            var animEditor = window.animEditor;
            var curveEditor = animEditor.curveEditor;
            var curveRect = curveEditor.rect;

            UpdateStyle(curveRect);
        }

        private void OnDopeSheetGUI()
        {
            var window = new AnimationWindowProxy(_target);
            var animEditor = window.animEditor;
            var dopeSheetEditor = animEditor.dopeSheetEditor;
            var dopeSheetRect = dopeSheetEditor.rect;

            UpdateStyle(dopeSheetRect);

            var animEditorState = animEditor.state;
            var isRoot = true;
            foreach (var dopeLine in animEditorState.dopelines)
            {
                // Skip the first element as it is the root element
                if (isRoot)
                {
                    isRoot = false;
                    continue;
                }

                // Create a new renderer if not cached
                if (!_dopeLineRenderers.TryGetValue(dopeLine, out var dopeLineRenderer))
                {
                    dopeLineRenderer = new DopeLineRenderer(_target.animationClip, dopeLine);
                    _dopeLineRenderers[dopeLine] = dopeLineRenderer;
                }

                // Draw
                var scrollPos = animEditorState.hierarchyState.scrollPos;
                dopeLineRenderer.Draw(dopeSheetEditor, scrollPos);
            }
        }

        /// <summary>
        /// Updates the style to match the drawing area of the DopeSheet
        /// </summary>
        private void UpdateStyle(Rect rect)
        {
            var prevLeft = style.left.value.value;
            var prevTop = style.top.value.value;
            var prevWidth = style.width.value.value;
            var prevHeight = style.height.value.value;

            style.left = rect.x + 1;
            style.top = rect.y + 1;
            style.width = rect.width;
            style.height = rect.height;

            if (Mathf.Approximately(prevLeft, style.left.value.value) &&
                Mathf.Approximately(prevTop, style.top.value.value) &&
                Mathf.Approximately(prevWidth, style.width.value.value) &&
                Mathf.Approximately(prevHeight, style.height.value.value))
            {
                return;
            }

            _isStyleDirty = true;
        }
    }
}
