using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AnimationWindowEnhancer.Core
{
    /// <summary>
    /// ScriptableObject to store settings for AnimationWindowEnhancer
    /// </summary>
    [FilePath("AnimationWindowEnhancerPreferences.asset", FilePathAttribute.Location.PreferencesFolder)]
    public class AnimationWindowEnhancerPreferences : ScriptableSingleton<AnimationWindowEnhancerPreferences>
    {
        // Initial values
        public static List<HeatmapOverrideEntry> InitialCurveHeatmapSettings => new()
        {
            new HeatmapOverrideEntry { Name = "x", Heatmap = GradientUtility.FromBeginEnd(new Color(1, 0.1f, 0.1f, 0.25f), new Color(1, 0.1f, 0.1f, 0.75f)) },
            new HeatmapOverrideEntry { Name = "r", Heatmap = GradientUtility.FromBeginEnd(new Color(1, 0.1f, 0.1f, 0.25f), new Color(1, 0.1f, 0.1f, 0.75f)) },
            new HeatmapOverrideEntry { Name = "y", Heatmap = GradientUtility.FromBeginEnd(new Color(0.1f, 0.75f, 0.1f, 0.25f), new Color(0.1f, 0.75f, 0.1f, 0.75f)) },
            new HeatmapOverrideEntry { Name = "g", Heatmap = GradientUtility.FromBeginEnd(new Color(0.1f, 0.75f, 0.1f, 0.25f), new Color(0.1f, 0.75f, 0.1f, 0.75f)) },
            new HeatmapOverrideEntry { Name = "z", Heatmap = GradientUtility.FromBeginEnd(new Color(0.1f, 0.75f, 1, 0.25f), new Color(0.1f, 0.75f, 1, 0.75f)) },
            new HeatmapOverrideEntry { Name = "b", Heatmap = GradientUtility.FromBeginEnd(new Color(0.1f, 0.75f, 1, 0.25f), new Color(0.1f, 0.75f, 1, 0.75f)) },
            new HeatmapOverrideEntry { Name = "w", Heatmap = GradientUtility.FromBeginEnd(new Color(1, 0.75f, 0.1f, 0.25f), new Color(1, 0.75f, 0.1f, 0.75f)) },
            new HeatmapOverrideEntry { Name = "a", Heatmap = GradientUtility.FromBeginEnd(new Color(1, 0.75f, 0.1f, 0.25f), new Color(1, 0.75f, 0.1f, 0.75f)) },
        };

        public static Gradient InitialDefaultCurveHeatmap => GradientUtility.FromBeginEnd(new Color(1, 1, 0.2f, 0.25f), new Color(1, 1, 0.2f, 0.75f));
        public const int InitialCurveResolution = 1;

        public static readonly Color InitialLabelColor = new Color(0.47f, 0.67f, 0.98f, 0.5f);
        public const int InitialLabelFontSize = 9;

        public const int InitialColorBandHeight = 3;
        public static readonly Color InitialParentDopeLineColor = new Color(0, 0, 0, 0.2f);

        // Settings values
        public Gradient DefaultCurveHeatmap = InitialDefaultCurveHeatmap;
        public List<HeatmapOverrideEntry> CurveHeatmapOverrides = InitialCurveHeatmapSettings;
        public int CurveResolution = InitialCurveResolution;

        public Color LabelColor = InitialLabelColor;
        public int LabelFontSize = InitialLabelFontSize;

        public int ColorBandHeight = InitialColorBandHeight;
        public Color ParentDopeLineColor = InitialParentDopeLineColor;

        public bool DopesheetShowCurve = true;
        public bool DopesheetShowLabel = true;
        public bool CurvesShowLabel = true;

        public void Save()
        {
            Save(true);
        }

        [Serializable]
        public class HeatmapOverrideEntry
        {
            public string Name;
            public Gradient Heatmap;
        }
    }
}
