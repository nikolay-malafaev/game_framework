namespace GameFramework.Math
{
    public static class Range
    {
        public static float Convert(float originalStart, float originalEnd, float newStart, float newEnd,
            float value)
        {
            if (originalEnd - originalStart == 0)
            {
                return 0;
            }

            float scale = (newEnd - newStart) / (originalEnd - originalStart);
            return newStart + (value - originalStart) * scale;
        }

        public static int Convert(int originalStart, int originalEnd, int newStart, int newEnd, int value)
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