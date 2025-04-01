using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AnimationWindowEnhancer.Core
{
    /// <summary>
    /// Class that provides the settings screen for AnimationWindowEnhancer
    /// </summary>
    public class AnimationWindowEnhancerPreferencesProvider : SettingsProvider
    {
        private const string Path = "Preferences/Animation Window Enhancer";

        private Editor _editor;

        private AnimationWindowEnhancerPreferencesProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null)
            : base(path, scopes, keywords)
        {
        }

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            var keywords = new HashSet<string>(new[] { "Animation", "Window", "Enhancer" });
            return new AnimationWindowEnhancerPreferencesProvider(Path, SettingsScope.User, keywords);
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            var preferences = AnimationWindowEnhancerPreferences.instance;
            preferences.hideFlags = HideFlags.HideAndDontSave & ~HideFlags.NotEditable;
            Editor.CreateCachedEditor(preferences, null, ref _editor);
        }

        public override void OnGUI(string searchContext)
        {
            using (var ccs = new EditorGUI.ChangeCheckScope())
            {
                _editor.OnInspectorGUI();
                if (ccs.changed)
                {
                    AnimationWindowEnhancerPreferences.instance.Save();
                }
            }
        }
    }
}
