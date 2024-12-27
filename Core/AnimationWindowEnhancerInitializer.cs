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
                var optionsElement = new AnimationWindowEnhancerOptionsElement();
                root.Add(optionsElement);
            }
        }
    }

    public static class AnimationWindowEnhancerPrefs
    {
        private const string ShowCurveKey = "AnimationWindowEnhancer.ShowCurve";
        private const string ShowLabelKey = "AnimationWindowEnhancer.ShowLabel";

        public static bool ShowCurve
        {
            get => EditorPrefs.GetBool(ShowCurveKey, true);
            set => EditorPrefs.SetBool(ShowCurveKey, value);
        }

        public static bool ShowLabel
        {
            get => EditorPrefs.GetBool(ShowLabelKey, true);
            set => EditorPrefs.SetBool(ShowLabelKey, value);
        }
    }
}
