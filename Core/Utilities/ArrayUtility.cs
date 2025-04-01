namespace AnimationWindowEnhancer.Core
{
    public static class ArrayUtility
    {
        /// <summary>
        /// Ensures the array has the specified length
        /// </summary>
        public static void EnsureArraySize<T>(ref T[] array, int size)
        {
            if (array == null || array.Length != size)
            {
                array = new T[size];
            }
        }
    }
}
