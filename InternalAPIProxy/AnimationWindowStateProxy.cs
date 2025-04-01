using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;

namespace AnimationWindowEnhancer.InternalAPIProxy
{
    public readonly struct AnimationWindowStateProxy : IEquatable<AnimationWindowStateProxy>
    {
        private readonly AnimationWindowState _instance;

        internal AnimationWindowStateProxy(AnimationWindowState animationWindowState)
            => _instance = animationWindowState;

        public bool Equals(AnimationWindowStateProxy other)
            => Equals(_instance, other._instance);

        public override bool Equals(object obj)
            => obj is AnimationWindowStateProxy other && Equals(other);

        public override int GetHashCode()
            => _instance != null ? _instance.GetHashCode() : 0;

        public bool showCurveEditor
            => _instance.showCurveEditor;

        public AnimationWindowHierarchyStateProxy hierarchyState
            => new(_instance.hierarchyState);

        public IEnumerable<DopeLineProxy> dopelines
            => _instance.dopelines.Select(x => new DopeLineProxy(x));

        public float currentTime
            => _instance.currentTime;
    }
}
