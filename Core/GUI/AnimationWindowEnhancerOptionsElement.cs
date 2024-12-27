using UnityEngine.UIElements;

namespace AnimationWindowEnhancer.Core
{
    public class AnimationWindowEnhancerOptionsElement : VisualElement
    {
        public AnimationWindowEnhancerOptionsElement()
        {
            style.position = Position.Absolute;
            style.width = 64;
            style.right = 12;
            style.bottom = 12;

            var curveToggle = CreateToggle("Curve", AnimationWindowEnhancerPrefs.ShowCurve, v => AnimationWindowEnhancerPrefs.ShowCurve = v);
            Add(curveToggle);

            var labelToggle = CreateToggle("Label", AnimationWindowEnhancerPrefs.ShowLabel, v => AnimationWindowEnhancerPrefs.ShowLabel = v);
            Add(labelToggle);
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
