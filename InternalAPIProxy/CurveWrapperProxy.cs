using System;
using UnityEditor;
using UnityEngine;

namespace AnimationWindowEnhancer.InternalAPIProxy
{
    public readonly struct CurveWrapperProxy : IEquatable<CurveWrapperProxy>
    {
        private readonly CurveWrapper _instance;

        internal CurveWrapperProxy(CurveWrapper curveWrapper)
            => _instance = curveWrapper;

        public bool Equals(CurveWrapperProxy other)
            => Equals(_instance, other._instance);

        public override bool Equals(object obj)
            => obj is CurveWrapperProxy other && Equals(other);

        public override int GetHashCode()
            => _instance != null ? _instance.GetHashCode() : 0;

        public AnimationCurve curve
            => _instance.curve;

        public EditorCurveBinding binding
            => _instance.binding;

        public Bounds ComputeBoundsBetweenTime(float start, float end)
            => _instance.ComputeBoundsBetweenTime(start, end);

        public AnimationClip animationClip
            => _instance.animationClip;

        public Color color
            => _instance.color;
    }
}
