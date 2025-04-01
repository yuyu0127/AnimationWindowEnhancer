using System;
using UnityEditorInternal;
using UnityEngine;

namespace AnimationWindowEnhancer.InternalAPIProxy
{
    public readonly struct AnimationWindowHierarchyStateProxy : IEquatable<AnimationWindowHierarchyStateProxy>
    {
        private readonly AnimationWindowHierarchyState _instance;

        internal AnimationWindowHierarchyStateProxy(AnimationWindowHierarchyState hierarchyState)
            => _instance = hierarchyState;

        public bool Equals(AnimationWindowHierarchyStateProxy other)
            => Equals(_instance, other._instance);

        public override bool Equals(object obj)
            => obj is AnimationWindowHierarchyStateProxy other && Equals(other);

        public override int GetHashCode()
            => _instance != null ? _instance.GetHashCode() : 0;

        public Vector2 scrollPos
            => _instance.scrollPos;
    }
}
