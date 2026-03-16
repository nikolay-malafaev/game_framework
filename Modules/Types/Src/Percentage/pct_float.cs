using System;

namespace GameFramework.Types
{
    /// <summary>
    /// Represents a percentage floating-point value in the range [0, 100].
    /// Automatically clamps values, replaces NaN and Infinity with zero,
    /// and supports safe arithmetic operations.
    /// </summary>ы
    public readonly struct pct_float : IEquatable<pct_float>
    {
        private readonly float m_value;

        public pct_float(float v)
        {
            if (float.IsNaN(v) || float.IsInfinity(v))
            {
                v = 0.0f;
            }

            m_value = ClampPercent(v);
        }

        public float Value => m_value;

        public static implicit operator float(pct_float v)
        {
            return v.m_value;
        }

        public static implicit operator pct_float(float v)
        {
            return new pct_float(v);
        }

        public static pct_float operator +(pct_float a, float b)
        {
            return new pct_float(a.m_value + b);
        }

        public static pct_float operator -(pct_float a, float b)
        {
            return new pct_float(a.m_value - b);
        }

        public static pct_float operator *(pct_float a, float b)
        {
            return new pct_float(a.m_value * b);
        }

        public static pct_float operator /(pct_float a, float b)
        {
            if (b == 0f)
            {
                return new pct_float(0.0f);
            }

            return new pct_float(a.m_value / b);
        }

        public bool Equals(pct_float other)
        {
            return m_value.Equals(other.m_value);
        }

        public override bool Equals(object obj)
        {
            return obj is pct_float other && Equals(other);
        }

        public override int GetHashCode()
        {
            return m_value.GetHashCode();
        }

        public static bool operator ==(pct_float left, pct_float right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(pct_float left, pct_float right)
        {
            return !left.Equals(right);
        }

        private static float ClampPercent(float v)
        {
            if (v < 0f) return 0f;
            if (v > 100f) return 100f;
            return v;
        }
    }
}
