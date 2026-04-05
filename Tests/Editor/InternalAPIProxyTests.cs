using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace AnimationWindowEnhancer.InternalAPIProxy.Tests
{
    /// <summary>
    /// Tests that verify the Unity internal APIs accessed via reflection are still available.
    /// If any of these tests fail, the corresponding Unity version has changed its internal API
    /// and the proxy code needs to be updated.
    /// </summary>
    public class InternalAPIProxyTests
    {
        // Helper to resolve internal Unity types
        private static Type FindType(string fullTypeName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(fullTypeName);
                if (type != null)
                    return type;
            }
            return null;
        }

        // Helper to find a member (property or field) by name
        private static bool HasMember(Type type, string memberName)
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            return type.GetProperty(memberName, flags) != null || type.GetField(memberName, flags) != null;
        }

        // ---- Type existence ----

        [TestCase("UnityEditor.AnimationWindow")]
        [TestCase("UnityEditor.AnimEditor")]
        [TestCase("UnityEditorInternal.DopeSheetEditor")]
        [TestCase("UnityEditorInternal.DopeLine")]
        [TestCase("UnityEditorInternal.AnimationWindowKeyframe")]
        [TestCase("UnityEditorInternal.AnimationWindowCurve")]
        [TestCase("UnityEditor.CurveEditor")]
        [TestCase("UnityEditor.CurveWrapper")]
        public void InternalType_Exists(string typeName)
        {
            var type = FindType(typeName);
            Assert.IsNotNull(type, $"Type '{typeName}' not found. Unity internal API may have changed.");
        }

        // AnimationWindowState and AnimationWindowHierarchyState may be in UnityEditor or UnityEditorInternal
        [TestCase("AnimationWindowState")]
        [TestCase("AnimationWindowHierarchyState")]
        public void InternalType_Exists_AnyNamespace(string shortName)
        {
            var type = FindType($"UnityEditor.{shortName}") ?? FindType($"UnityEditorInternal.{shortName}");
            Assert.IsNotNull(type, $"Type '{shortName}' not found in UnityEditor or UnityEditorInternal.");
        }

        // ---- Reflection-based access (most fragile) ----

        [Test]
        public void AnimationWindow_Has_s_AnimationWindows_Field()
        {
            var type = FindType("UnityEditor.AnimationWindow");
            Assert.IsNotNull(type);

            var field = type.GetField("s_AnimationWindows", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNotNull(field, "AnimationWindow.s_AnimationWindows field not found.");
        }

        [Test]
        public void DopeSheetEditor_Has_GetKeyframeRect_Method()
        {
            var type = FindType("UnityEditorInternal.DopeSheetEditor");
            Assert.IsNotNull(type);

            var method = type.GetMethod("GetKeyframeRect", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(method, "DopeSheetEditor.GetKeyframeRect method not found.");
        }

        // ---- AnimationWindow ----

        [Test]
        public void AnimationWindow_Has_animEditor_Member()
        {
            var type = FindType("UnityEditor.AnimationWindow");
            Assert.IsNotNull(type);
            Assert.IsTrue(HasMember(type, "animEditor"), "AnimationWindow.animEditor not found.");
        }

        // ---- AnimEditor ----

        [TestCase("state")]
        [TestCase("dopeSheetEditor")]
        [TestCase("curveEditor")]
        public void AnimEditor_Has_Member(string memberName)
        {
            var type = FindType("UnityEditor.AnimEditor");
            Assert.IsNotNull(type);
            Assert.IsTrue(HasMember(type, memberName), $"AnimEditor.{memberName} not found.");
        }

        // ---- AnimationWindowState ----

        [TestCase("showCurveEditor")]
        [TestCase("hierarchyState")]
        [TestCase("dopelines")]
        [TestCase("currentTime")]
        public void AnimationWindowState_Has_Member(string memberName)
        {
            var type = FindType("UnityEditor.AnimationWindowState") ?? FindType("UnityEditorInternal.AnimationWindowState");
            Assert.IsNotNull(type, "AnimationWindowState type not found.");
            Assert.IsTrue(HasMember(type, memberName), $"AnimationWindowState.{memberName} not found.");
        }

        // ---- DopeLine ----

        [TestCase("valueType")]
        [TestCase("hasChildren")]
        [TestCase("keys")]
        [TestCase("curves")]
        public void DopeLine_Has_Member(string memberName)
        {
            var type = FindType("UnityEditorInternal.DopeLine");
            Assert.IsNotNull(type);
            Assert.IsTrue(HasMember(type, memberName), $"DopeLine.{memberName} not found.");
        }

        // ---- AnimationWindowKeyframe ----

        [Test]
        public void AnimationWindowKeyframe_Has_time_Member()
        {
            var type = FindType("UnityEditorInternal.AnimationWindowKeyframe");
            Assert.IsNotNull(type);
            Assert.IsTrue(HasMember(type, "time"), "AnimationWindowKeyframe.time not found.");
        }

        // ---- AnimationWindowCurve ----

        [Test]
        public void AnimationWindowCurve_Has_binding_Member()
        {
            var type = FindType("UnityEditorInternal.AnimationWindowCurve");
            Assert.IsNotNull(type);
            Assert.IsTrue(HasMember(type, "binding"), "AnimationWindowCurve.binding not found.");
        }

        // ---- AnimationWindowHierarchyState ----

        [Test]
        public void AnimationWindowHierarchyState_Has_scrollPos_Member()
        {
            var type = FindType("UnityEditor.AnimationWindowHierarchyState") ?? FindType("UnityEditorInternal.AnimationWindowHierarchyState");
            Assert.IsNotNull(type, "AnimationWindowHierarchyState type not found.");
            Assert.IsTrue(HasMember(type, "scrollPos"), "AnimationWindowHierarchyState.scrollPos not found.");
        }

        // ---- CurveEditor ----

        [TestCase("rect")]
        [TestCase("animationCurves")]
        public void CurveEditor_Has_Member(string memberName)
        {
            var type = FindType("UnityEditor.CurveEditor");
            Assert.IsNotNull(type);
            Assert.IsTrue(HasMember(type, memberName), $"CurveEditor.{memberName} not found.");
        }

        [Test]
        public void CurveEditor_Has_DrawingToViewTransformPoint_Method()
        {
            var type = FindType("UnityEditor.CurveEditor");
            Assert.IsNotNull(type);

            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(m => m.Name == "DrawingToViewTransformPoint")
                .ToArray();
            Assert.IsTrue(methods.Length > 0, "CurveEditor.DrawingToViewTransformPoint method not found.");

            // Verify the Vector3 overload exists (used by CurveEditorProxy)
            var vector3Overload = methods.FirstOrDefault(m =>
            {
                var p = m.GetParameters();
                return p.Length == 1 && p[0].ParameterType == typeof(Vector3);
            });
            Assert.IsNotNull(vector3Overload, "CurveEditor.DrawingToViewTransformPoint(Vector3) overload not found.");
        }

        // ---- CurveWrapper ----

        [TestCase("curve")]
        [TestCase("binding")]
        [TestCase("color")]
        public void CurveWrapper_Has_Member(string memberName)
        {
            var type = FindType("UnityEditor.CurveWrapper");
            Assert.IsNotNull(type);
            Assert.IsTrue(HasMember(type, memberName), $"CurveWrapper.{memberName} not found.");
        }

        [Test]
        public void CurveWrapper_Has_ComputeBoundsBetweenTime_Method()
        {
            var type = FindType("UnityEditor.CurveWrapper");
            Assert.IsNotNull(type);

            var method = type.GetMethod("ComputeBoundsBetweenTime", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(method, "CurveWrapper.ComputeBoundsBetweenTime method not found.");
        }

        // ---- DopeSheetEditor ----

        [Test]
        public void DopeSheetEditor_Has_rect_Member()
        {
            var type = FindType("UnityEditorInternal.DopeSheetEditor");
            Assert.IsNotNull(type);
            Assert.IsTrue(HasMember(type, "rect"), "DopeSheetEditor.rect not found.");
        }
    }
}
