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
        static AnimationWindowEnhancerInitializer()
        {
            EditorApplication.update += Update;
        }

        private static void Update()
        {
            // Get already opened AnimationWindows
            foreach (var animationWindow in AnimationWindowProxy.s_AnimationWindows)
            {
                var root = animationWindow.rootVisualElement;
                if (root.Q<AnimationWindowEnhancerElement>() != null)
                {
                    continue;
                }

                var overlayElement = new AnimationWindowEnhancerElement(animationWindow);
                root.Add(overlayElement);

                // Options
                var optionsElement = new AnimationWindowEnhancerOptionsElement(animationWindow);
                root.Add(optionsElement);
            }
        }
    }
}
