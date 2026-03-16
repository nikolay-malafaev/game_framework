using System;
using System.Collections.Generic;

namespace GameFramework.Types
{
    public readonly struct Optional<T> : IEquatable<Optional<T>>
    {
        private readonly T _value;
        private readonly bool _hasValue;
        public static Optional<T> None => default;

        public Optional(T value)
        {
            if (value is UnityEngine.Object obj)
            {
                _hasValue = obj != null;
            }
            else
            {
                _hasValue = value != null;
            }

            _value = value;
        }

        public bool HasValue() => _hasValue;

        public T Value()
        {
            return HasValue() ? _value : throw new InvalidOperationException("Optional has no value.");
        }
        
        public T ValueOrDefault(T defaultValue) => HasValue() ? _value : defaultValue;

        public bool Equals(Optional<T> other)
        {
            if (!HasValue() && !other.HasValue()) return true;
            if (HasValue() != other.HasValue()) return false;
            return EqualityComparer<T>.Default.Equals(_value, other._value);
        }
        
        public static implicit operator Optional<T>(T value) => new Optional<T>(value);
        
        public static explicit operator T(Optional<T> optional) => optional.Value();

        public override bool Equals(object obj) => obj is Optional<T> other && Equals(other);

        public override int GetHashCode() => HasValue() ? EqualityComparer<T>.Default.GetHashCode(_value) : 0;

        public override string ToString() => HasValue() ? _value.ToString() : "None";
    }
}