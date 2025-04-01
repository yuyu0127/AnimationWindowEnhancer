using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AnimationWindowEnhancer.InternalAPIProxy
{
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

        public IEnumerable<CurveWrapperProxy> animationCurves
            => _instance.animationCurves.Select(x => new CurveWrapperProxy(x));

        public Vector3 DrawingToViewTransformPoint(Vector3 point)
            => _instance.DrawingToViewTransformPoint(point);
    }
}
