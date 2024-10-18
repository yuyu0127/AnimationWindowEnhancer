using System;
using System.Reflection;
using UnityEditorInternal;
using UnityEngine;

namespace AnimationWindowEnhancer.InternalAPIProxy
{
    public readonly struct DopeSheetEditorProxy : IEquatable<DopeSheetEditorProxy>
    {
        private static readonly MethodInfo _getKeyframeRectMethod
            = typeof(DopeSheetEditor).GetMethod("GetKeyframeRect", BindingFlags.NonPublic | BindingFlags.Instance);

        private readonly DopeSheetEditor _instance;

        internal DopeSheetEditorProxy(DopeSheetEditor dopeSheetEditor)
            => _instance = dopeSheetEditor;

        public bool Equals(DopeSheetEditorProxy other)
            => Equals(_instance, other._instance);

        public override bool Equals(object obj)
            => obj is DopeSheetEditorProxy other && Equals(other);

        public override int GetHashCode()
            => _instance != null ? _instance.GetHashCode() : 0;

        public Rect rect
            => _instance.rect;

        public Rect GetKeyframeRect(in DopeLineProxy dopeLine, in AnimationWindowKeyframeProxy keyframe)
            => (Rect) _getKeyframeRectMethod.Invoke(_instance, new object[] { dopeLine.Instance, keyframe.Instance });
    }
}
