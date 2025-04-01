using System;
using UnityEditor;
using UnityEditorInternal;

namespace AnimationWindowEnhancer.InternalAPIProxy
{
    public readonly struct AnimationWindowCurveProxy : IEquatable<AnimationWindowCurveProxy>
    {
        private readonly AnimationWindowCurve _instance;

        internal AnimationWindowCurveProxy(AnimationWindowCurve curve)
            => _instance = curve;

        public bool Equals(AnimationWindowCurveProxy other)
            => Equals(_instance, other._instance);

        public override bool Equals(object obj)
            => obj is AnimationWindowCurveProxy other && Equals(other);

        public override int GetHashCode()
            => _instance != null ? _instance.GetHashCode() : 0;

        public EditorCurveBinding binding
            => _instance.binding;
    }
}
