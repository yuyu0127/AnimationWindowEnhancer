using System;
using UnityEditorInternal;

namespace AnimationWindowEnhancer.InternalAPIProxy
{
    public readonly struct AnimationWindowKeyframeProxy : IEquatable<AnimationWindowKeyframeProxy>
    {
        private readonly AnimationWindowKeyframe _instance;

        internal AnimationWindowKeyframeProxy(AnimationWindowKeyframe keyframe)
            => _instance = keyframe;

        public bool Equals(AnimationWindowKeyframeProxy other)
            => Equals(_instance, other._instance);

        public override bool Equals(object obj)
            => obj is AnimationWindowKeyframeProxy other && Equals(other);

        public override int GetHashCode()
            => _instance != null ? _instance.GetHashCode() : 0;

        internal AnimationWindowKeyframe Instance => _instance;

        public float time
            => _instance.time;
    }
}
