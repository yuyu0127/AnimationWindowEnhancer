using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;

namespace AnimationWindowEnhancer.InternalAPIProxy
{
    public readonly struct DopeLineProxy : IEquatable<DopeLineProxy>
    {
        private readonly DopeLine _instance;

        internal DopeLineProxy(DopeLine dopeLine)
            => _instance = dopeLine;

        public bool Equals(DopeLineProxy other)
            => Equals(_instance, other._instance);

        public override bool Equals(object obj)
            => obj is DopeLineProxy other && Equals(other);

        public override int GetHashCode()
            => _instance != null ? _instance.GetHashCode() : 0;

        internal DopeLine Instance
            => _instance;

        public Type valueType
            => _instance.valueType;

        public bool hasChildren
            => _instance.hasChildren;

        public IEnumerable<AnimationWindowKeyframeProxy> keys
            => _instance.keys.Select(x => new AnimationWindowKeyframeProxy(x));

        public IEnumerable<AnimationWindowCurveProxy> curves
            => _instance.curves.Select(x => new AnimationWindowCurveProxy(x));
    }
}
