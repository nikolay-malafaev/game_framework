namespace GameFramework.Utils
{
    public static class MathUtils
    {
        public static float ConvertRange(float originalStart, float originalEnd, float newStart, float newEnd,
            float value)
        {
            if (originalEnd - originalStart == 0)
            {
                return 0;
            }

            float scale = (newEnd - newStart) / (originalEnd - originalStart);
            return newStart + (value - originalStart) * scale;
        }

        public static int ConvertRange(int originalStart, int originalEnd, int newStart, int newEnd, int value)
        {
            if (originalEnd == originalStart)
            {
                return newStart;
            }

            float scale = (float)(newEnd - newStart) / (originalEnd - originalStart);
            float result = newStart + (value - originalStart) * scale;

            return (int)result;
        }
    }
}