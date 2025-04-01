using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace AnimationWindowEnhancer.Core
{
    /// <summary>
    /// Class to draw the settings for AnimationWindowEnhancer
    /// </summary>
    [CustomEditor(typeof(AnimationWindowEnhancerPreferences))]
    public class AnimationWindowEnhancerPreferencesEditor : Editor
    {
        private const float ResetButtonWidth = 54f;

        private ReorderableList _heatmapOverridesList;
        private bool _foldoutHeatmap;

        private void OnEnable()
        {
            var heatmapOverridesProperty = serializedObject.FindProperty(nameof(AnimationWindowEnhancerPreferences.CurveHeatmapOverrides));
            _heatmapOverridesList = new ReorderableList(serializedObject, heatmapOverridesProperty, true, true, true, true)
            {
                drawHeaderCallback = rect =>
                {
                    GUI.Label(rect, new GUIContent("Heatmap Overrides", "Overrides the heatmap for curves by property name"));

                    var buttonRect = new Rect(rect.x + rect.width - ResetButtonWidth + 7, rect.y, ResetButtonWidth - 7, rect.height);
                    if (GUI.Button(buttonRect, "Reset", EditorStyles.miniButton))
                    {
                        heatmapOverridesProperty.ClearArray();
                        foreach (var entry in AnimationWindowEnhancerPreferences.InitialCurveHeatmapSettings)
                        {
                            heatmapOverridesProperty.InsertArrayElementAtIndex(heatmapOverridesProperty.arraySize);
                            var element = heatmapOverridesProperty.GetArrayElementAtIndex(heatmapOverridesProperty.arraySize - 1);
                            element.FindPropertyRelative(nameof(AnimationWindowEnhancerPreferences.HeatmapOverrideEntry.Name)).stringValue = entry.Name;
                            element.FindPropertyRelative(nameof(AnimationWindowEnhancerPreferences.HeatmapOverrideEntry.Heatmap)).gradientValue = entry.Heatmap;
                        }
                        serializedObject.ApplyModifiedProperties();
                    }
                },
                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var element = _heatmapOverridesList.serializedProperty.GetArrayElementAtIndex(index);
                    rect.height = EditorGUIUtility.singleLineHeight;
                    rect.y += 1;

                    var nameProperty = element.FindPropertyRelative(nameof(AnimationWindowEnhancerPreferences.HeatmapOverrideEntry.Name));
                    var colorProperty = element.FindPropertyRelative(nameof(AnimationWindowEnhancerPreferences.HeatmapOverrideEntry.Heatmap));

                    var nameWidth = EditorGUIUtility.labelWidth - 22f;
                    var nameRect = new Rect(rect.x, rect.y, nameWidth, rect.height);
                    var colorRect = new Rect(rect.x + nameWidth + 3, rect.y, rect.width - nameWidth - 3, rect.height);

                    EditorGUI.PropertyField(nameRect, nameProperty, GUIContent.none);
                    EditorGUI.PropertyField(colorRect, colorProperty, GUIContent.none);
                },
                elementHeight = EditorGUIUtility.singleLineHeight,
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            using (new EditorGUILayout.VerticalScope(EditorStyles.inspectorFullWidthMargins))
            {
                // Curve
                GUILayout.Label("Curve", EditorStyles.boldLabel);

                // - Default Heatmap
                using (new EditorGUILayout.HorizontalScope())
                {
                    var defaultHeatmapProperty = serializedObject.FindProperty(nameof(AnimationWindowEnhancerPreferences.DefaultCurveHeatmap));
                    EditorGUILayout.PropertyField(defaultHeatmapProperty, new GUIContent("Default Heatmap", "The heatmap to use for curves that don't have an override"));
                    if (GUILayout.Button("Reset", GUILayout.Width(ResetButtonWidth)))
                    {
                        defaultHeatmapProperty.gradientValue = AnimationWindowEnhancerPreferences.InitialDefaultCurveHeatmap;
                    }
                }

                // - Heatmap Overrides
                _heatmapOverridesList.DoLayoutList();

                // - Resolution
                using (new EditorGUILayout.HorizontalScope())
                {
                    var curveResolutionProperty = serializedObject.FindProperty(nameof(AnimationWindowEnhancerPreferences.CurveResolution));
                    EditorGUILayout.PropertyField(curveResolutionProperty, new GUIContent("Resolution", "The number of points per frame to draw the curve"));
                    curveResolutionProperty.intValue = Mathf.Max(1, curveResolutionProperty.intValue);
                    if (GUILayout.Button("Reset", GUILayout.Width(ResetButtonWidth)))
                    {
                        curveResolutionProperty.intValue = AnimationWindowEnhancerPreferences.InitialCurveResolution;
                    }
                }

                EditorGUILayout.Space();

                // Label
                GUILayout.Label("Label", EditorStyles.boldLabel);

                // - Color
                using (new EditorGUILayout.HorizontalScope())
                {
                    var labelColorProperty = serializedObject.FindProperty(nameof(AnimationWindowEnhancerPreferences.LabelColor));
                    EditorGUILayout.PropertyField(labelColorProperty, new GUIContent("Color", "The color of the label on each dope line"));
                    if (GUILayout.Button("Reset", GUILayout.Width(ResetButtonWidth)))
                    {
                        labelColorProperty.colorValue = AnimationWindowEnhancerPreferences.InitialLabelColor;
                    }
                }
                // - Font Size
                using (new EditorGUILayout.HorizontalScope())
                {
                    var labelFontSizeProperty = serializedObject.FindProperty(nameof(AnimationWindowEnhancerPreferences.LabelFontSize));
                    EditorGUILayout.PropertyField(labelFontSizeProperty, new GUIContent("Font Size", "The font size of the label on each dope line"));
                    labelFontSizeProperty.intValue = Mathf.Max(1, labelFontSizeProperty.intValue);
                    if (GUILayout.Button("Reset", GUILayout.Width(ResetButtonWidth)))
                    {
                        labelFontSizeProperty.intValue = AnimationWindowEnhancerPreferences.InitialLabelFontSize;
                    }
                }

                EditorGUILayout.Space();

                // Others
                GUILayout.Label("Others", EditorStyles.boldLabel);

                // - Color Band Height
                using (new EditorGUILayout.HorizontalScope())
                {
                    var colorBandHeightProperty = serializedObject.FindProperty(nameof(AnimationWindowEnhancerPreferences.ColorBandHeight));
                    EditorGUILayout.PropertyField(colorBandHeightProperty, new GUIContent("Color Band Height", "The height of the color band on dope lines of color properties"));
                    colorBandHeightProperty.intValue = Mathf.Max(0, colorBandHeightProperty.intValue);
                    if (GUILayout.Button("Reset", GUILayout.Width(ResetButtonWidth)))
                    {
                        colorBandHeightProperty.intValue = AnimationWindowEnhancerPreferences.InitialColorBandHeight;
                    }
                }

                // - Parent Dope Line Color
                using (new EditorGUILayout.HorizontalScope())
                {
                    var parentDopeLineColorProperty = serializedObject.FindProperty(nameof(AnimationWindowEnhancerPreferences.ParentDopeLineColor));
                    EditorGUILayout.PropertyField(parentDopeLineColorProperty, new GUIContent("Parent Dope Line Color", "The color of the dope line which has children"));
                    if (GUILayout.Button("Reset", GUILayout.Width(ResetButtonWidth)))
                    {
                        parentDopeLineColorProperty.colorValue = AnimationWindowEnhancerPreferences.InitialParentDopeLineColor;
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
