namespace GameFramework.Types
{
    /// <summary>
    /// Represents a normalized floating-point value in the range [0, 1].
    /// Automatically clamps values, replaces NaN and Infinity with zero,
    /// and provides safe arithmetic operations.
    /// </summary>
    public readonly struct n_float : System.IEquatable<n_float>
    {
        private readonly float m_value;

        public n_float(float v)
        {
            if (float.IsNaN(v) || float.IsInfinity(v))
            {
                v = 0.0f;
            }

            m_value = Clamp01(v);
        }

        public static implicit operator float(n_float v)
        {
            return v.m_value;
        }

        public static implicit operator n_float(float v)
        {
            return new n_float(v);
        }

        public static n_float operator +(n_float a, float b)
        {
            return new n_float(a.m_value + b);
        }

        public static n_float operator -(n_float a, float b)
        {
            return new n_float(a.m_value - b);
        }

        public static n_float operator *(n_float a, float b)
        {
            return new n_float(a.m_value * b);
        }

        public static n_float operator /(n_float a, float b)
        {
            if (b == 0f)
            {
                return new n_float(0.0f);
            }

            return new n_float(a.m_value / b);
        }

        public bool Equals(n_float other) => m_value.Equals(other.m_value);
        public override bool Equals(object obj) => obj is n_float other && Equals(other);
        public override int GetHashCode() => m_value.GetHashCode();

        private static float Clamp01(float v)
        {
            if (v < 0f) return 0f;
            if (v > 1f) return 1f;
            return v;
        }
    }
}