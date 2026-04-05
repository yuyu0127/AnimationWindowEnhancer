using System.Collections.Generic;
using NUnit.Framework;

namespace AnimationWindowEnhancer.Core.Tests
{
    public class StringUtilityTests
    {
        [Test]
        public void GetLongestCommonPrefix_NullList_ReturnsEmpty()
        {
            Assert.AreEqual("", StringUtility.GetLongestCommonPrefix(null));
        }

        [Test]
        public void GetLongestCommonPrefix_EmptyList_ReturnsEmpty()
        {
            Assert.AreEqual("", StringUtility.GetLongestCommonPrefix(new List<string>()));
        }

        [Test]
        public void GetLongestCommonPrefix_SingleString_ReturnsThatString()
        {
            var result = StringUtility.GetLongestCommonPrefix(new List<string> { "position.x" });
            Assert.AreEqual("position.x", result);
        }

        [Test]
        public void GetLongestCommonPrefix_CommonPrefix_ReturnsPrefix()
        {
            var result = StringUtility.GetLongestCommonPrefix(new List<string>
            {
                "localPosition.x",
                "localPosition.y",
                "localPosition.z"
            });
            Assert.AreEqual("localPosition.", result);
        }

        [Test]
        public void GetLongestCommonPrefix_NoCommonPrefix_ReturnsEmpty()
        {
            var result = StringUtility.GetLongestCommonPrefix(new List<string>
            {
                "alpha",
                "beta",
                "gamma"
            });
            Assert.AreEqual("", result);
        }

        [Test]
        public void GetLongestCommonPrefix_IdenticalStrings_ReturnsFullString()
        {
            var result = StringUtility.GetLongestCommonPrefix(new List<string>
            {
                "position.x",
                "position.x"
            });
            Assert.AreEqual("position.x", result);
        }

        [Test]
        public void GetLongestCommonPrefix_ColorPropertyNames_ReturnsCommonPart()
        {
            var result = StringUtility.GetLongestCommonPrefix(new List<string>
            {
                "m_Color.r",
                "m_Color.g",
                "m_Color.b",
                "m_Color.a"
            });
            Assert.AreEqual("m_Color.", result);
        }
    }
}
