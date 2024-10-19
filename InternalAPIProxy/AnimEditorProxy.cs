using System;
using UnityEditor;
using UnityEngine;

namespace AnimationWindowEnhancer.InternalAPIProxy
{
    public readonly struct AnimEditorProxy : IEquatable<AnimEditorProxy>
    {
        private readonly AnimEditor _instance;

        internal AnimEditorProxy(AnimEditor animEditor)
            => _instance = animEditor;

        public bool Equals(AnimEditorProxy other)
            => Equals(_instance, other._instance);

        public override bool Equals(object obj)
            => obj is AnimEditorProxy other && Equals(other);

        public override int GetHashCode()
            => _instance != null ? _instance.GetHashCode() : 0;

        public AnimationWindowStateProxy state
            => new(_instance.state);

        public DopeSheetEditorProxy dopeSheetEditor
            => new(_instance.dopeSheetEditor);

        public CurveEditorProxy curveEditor
            => new(_instance.curveEditor);
    }

    public readonly struct CurveEditorProxy : IEquatable<CurveEditorProxy>
    {
        private readonly CurveEditor _instance;

        internal CurveEditorProxy(CurveEditor animEditor)
            => _instance = animEditor;

        public bool Equals(CurveEditorProxy other)
            => Equals(_instance, other._instance);

        public override bool Equals(object obj)
            => obj is CurveEditorProxy other && Equals(other);

        public override int GetHashCode()
            => _instance != null ? _instance.GetHashCode() : 0;

        public Rect rect
            => _instance.rect;
    }
}
