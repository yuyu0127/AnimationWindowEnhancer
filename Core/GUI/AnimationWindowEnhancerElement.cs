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
        private readonly Dictionary<DopeLineProxy, DopeLineDrawer> _dopeLineDrawers = new();

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
                _dopeLineDrawers.Clear();
            }

            var targetProxy = new AnimationWindowProxy(_target);
            var animEditor = targetProxy.animEditor;
            var dopeSheetEditor = animEditor.dopeSheetEditor;
            var animEditorState = animEditor.state;
            var dopeSheetRect = dopeSheetEditor.rect;
            var scrollPos = animEditorState.hierarchyState.scrollPos;

            if (_isStyleDirty)
            {
                _isStyleDirty = false;
                MarkDirtyLayout();
            }

            UpdateStyle(dopeSheetRect);

            var isRoot = true;
            foreach (var dopeLine in animEditorState.dopelines)
            {
                // Skip the first element as it is the root element
                if (isRoot)
                {
                    isRoot = false;
                    continue;
                }

                // Create a new drawer if not cached
                if (!_dopeLineDrawers.TryGetValue(dopeLine, out var dopeLineDrawer))
                {
                    dopeLineDrawer = new DopeLineDrawer(clip, dopeLine);
                    _dopeLineDrawers[dopeLine] = dopeLineDrawer;
                }

                // Draw
                dopeLineDrawer.Draw(dopeSheetRect, dopeSheetEditor, scrollPos);
            }
        }

        /// <summary>
        /// Updates the style to match the drawing area of the DopeSheet
        /// </summary>
        private void UpdateStyle(Rect dopeSheetRect)
        {
            var prevLeft = style.left.value.value;
            var prevTop = style.top.value.value;
            var prevWidth = style.width.value.value;
            var prevHeight = style.height.value.value;

            style.left = dopeSheetRect.x + 1;
            style.top = dopeSheetRect.y + 1;
            style.width = dopeSheetRect.width;
            style.height = dopeSheetRect.height;

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
