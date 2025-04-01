using AnimationWindowEnhancer.InternalAPIProxy;
using UnityEditor;
using UnityEngine.UIElements;

namespace AnimationWindowEnhancer.Core
{
    public class AnimationWindowEnhancerOptionsElement : IMGUIContainer
    {
        public AnimationWindowEnhancerOptionsElement(AnimationWindow animationWindow)
        {
            style.position = Position.Absolute;
            style.width = 64;
            style.right = 12;
            style.bottom = 12;

            // Add toggles to switch visibility of curves and labels
            var dopesheetCurveToggle = CreateToggle("Curve", AnimationWindowEnhancerPreferences.instance.DopesheetShowCurve,
                v => AnimationWindowEnhancerPreferences.instance.DopesheetShowCurve = v);
            Add(dopesheetCurveToggle);

            var dopesheetLabelToggle = CreateToggle("Label", AnimationWindowEnhancerPreferences.instance.DopesheetShowLabel,
                v => AnimationWindowEnhancerPreferences.instance.DopesheetShowLabel = v);
            Add(dopesheetLabelToggle);

            var curvesLabelToggle = CreateToggle("Label", AnimationWindowEnhancerPreferences.instance.CurvesShowLabel,
                v => AnimationWindowEnhancerPreferences.instance.CurvesShowLabel = v);
            Add(curvesLabelToggle);

            // Polling the AnimationWindow state and updating the toggle visibility
            onGUIHandler = () =>
            {
                var window = new AnimationWindowProxy(animationWindow);
                var showCurveEditor = window.animEditor.state.showCurveEditor;

                dopesheetCurveToggle.style.display = new StyleEnum<DisplayStyle>(showCurveEditor ? DisplayStyle.None : DisplayStyle.Flex);
                dopesheetLabelToggle.style.display = new StyleEnum<DisplayStyle>(showCurveEditor ? DisplayStyle.None : DisplayStyle.Flex);
                curvesLabelToggle.style.display = new StyleEnum<DisplayStyle>(showCurveEditor ? DisplayStyle.Flex : DisplayStyle.None);
            };
        }

        private static Toggle CreateToggle(string text, bool value, System.Action<bool> onValueChanged)
        {
            var toggle = new Toggle(text);
            toggle.SetValueWithoutNotify(value);
            toggle.RegisterValueChangedCallback(e => onValueChanged(e.newValue));
            var label = toggle.Q<Label>();
            label.style.minWidth = 40f;
            return toggle;
        }
    }
}
