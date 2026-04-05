using NUnit.Framework;

namespace AnimationWindowEnhancer.Core.Tests
{
    public class ArrayUtilityTests
    {
        [Test]
        public void EnsureArraySize_NullArray_CreatesNewArray()
        {
            int[] array = null;
            ArrayUtility.EnsureArraySize(ref array, 5);

            Assert.IsNotNull(array);
            Assert.AreEqual(5, array.Length);
        }

        [Test]
        public void EnsureArraySize_DifferentSize_ReplacesArray()
        {
            var array = new int[3];
            ArrayUtility.EnsureArraySize(ref array, 5);

            Assert.AreEqual(5, array.Length);
        }

        [Test]
        public void EnsureArraySize_SameSize_KeepsSameInstance()
        {
            var array = new int[5];
            var original = array;
            ArrayUtility.EnsureArraySize(ref array, 5);

            Assert.AreSame(original, array);
        }

        [Test]
        public void EnsureArraySize_ZeroSize_CreatesEmptyArray()
        {
            int[] array = null;
            ArrayUtility.EnsureArraySize(ref array, 0);

            Assert.IsNotNull(array);
            Assert.AreEqual(0, array.Length);
        }
    }
}
