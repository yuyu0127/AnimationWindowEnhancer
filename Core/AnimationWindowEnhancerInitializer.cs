using System.Collections.Generic;
using AnimationWindowEnhancer.InternalAPIProxy;
using UnityEditor;
using UnityEngine.UIElements;

namespace AnimationWindowEnhancer.Core
{
    /// <summary>
    /// Adds an element to draw an overlay above the AnimationWindow when the editor starts
    /// </summary>
    [InitializeOnLoad]
    public static class AnimationWindowEnhancerInitializer
    {
        private static readonly HashSet<AnimationWindow> _initializedWindows = new();

        static AnimationWindowEnhancerInitializer()
        {
            EditorApplication.update += Update;
        }

        private static void Update()
        {
            var animationWindows = AnimationWindowProxy.s_AnimationWindows;
            if (animationWindows == null)
            {
                return;
            }

            // Skip if the window count hasn't changed
            if (animationWindows.Count == _initializedWindows.Count)
            {
                return;
            }

            // Remove closed windows from tracking
            _initializedWindows.RemoveWhere(w => !animationWindows.Contains(w));

            // Add elements to new windows
            foreach (var animationWindow in animationWindows)
            {
                if (_initializedWindows.Contains(animationWindow))
                {
                    continue;
                }

                _initializedWindows.Add(animationWindow);

                var root = animationWindow.rootVisualElement;

                var overlayElement = new AnimationWindowEnhancerElement(animationWindow);
                root.Add(overlayElement);

                // Options
                var optionsElement = new AnimationWindowEnhancerOptionsElement(animationWindow);
                root.Add(optionsElement);
            }
        }
    }
}
