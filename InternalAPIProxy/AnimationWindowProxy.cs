using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace AnimationWindowEnhancer.InternalAPIProxy
{
    public readonly struct AnimationWindowProxy : IEquatable<AnimationWindowProxy>
    {
        private static readonly FieldInfo _animationWindowsField
            = typeof(AnimationWindow).GetField("s_AnimationWindows", BindingFlags.NonPublic | BindingFlags.Static);

        private readonly AnimationWindow _instance;

        public AnimationWindowProxy(AnimationWindow animationWindow)
            => _instance = animationWindow;

        public bool Equals(AnimationWindowProxy other)
            => Equals(_instance, other._instance);

        public override bool Equals(object obj)
            => obj is AnimationWindowProxy other && Equals(other);

        public override int GetHashCode()
            => _instance != null ? _instance.GetHashCode() : 0;

        public AnimEditorProxy animEditor
            => new(_instance.animEditor);

        public static List<AnimationWindow> s_AnimationWindows
            => (List<AnimationWindow>) _animationWindowsField.GetValue(null);
    }
}
