namespace GameFramework.Types
{
    /// <summary>
    /// Represents an integer percentage value in the range [0, 100].
    /// Automatically clamps values and provides safe arithmetic operations.
    /// </summary>
    public readonly struct pct_int : System.IEquatable<pct_int>
    {
        private readonly int m_value;

        public pct_int(int v)
        {
            m_value = Clamp(v);
        }

        public static implicit operator int(pct_int v)
        {
            return v.m_value;
        }

        public static implicit operator pct_int(int v)
        {
            return new pct_int(v);
        }

        public static pct_int operator +(pct_int a, int b)
        {
            return new pct_int(a.m_value + b);
        }

        public static pct_int operator -(pct_int a, int b)
        {
            return new pct_int(a.m_value - b);
        }

        public static pct_int operator *(pct_int a, int b)
        {
            return new pct_int(a.m_value * b);
        }

        public static pct_int operator /(pct_int a, int b)
        {
            if (b == 0)
            {
                return new pct_int(0);
            }

            return new pct_int(a.m_value / b);
        }

        public bool Equals(pct_int other) => m_value == other.m_value;
        public override bool Equals(object obj) => obj is pct_int other && Equals(other);
        public override int GetHashCode() => m_value;

        private static int Clamp(int v)
        {
            if (v < 0) return 0;
            if (v > 100) return 100;
            return v;
        }

        public override string ToString()
        {
            return $"{m_value}%";
        }
    }
}