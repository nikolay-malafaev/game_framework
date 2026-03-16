using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameFramework.Types
{
    [Serializable]
    [InlineProperty]
    public struct OptionalSerialized<T> : IEquatable<OptionalSerialized<T>>
    {
        [LabelText("")]
        [HorizontalGroup("Optional", Width = 15, MarginRight = 5)]
        [SerializeField]
        private bool _hasValue;

        [HideLabel]
        [HorizontalGroup("Optional")]
        [EnableIf("_hasValue")]
        [SerializeField]
        private T _value;

        public OptionalSerialized(T value) : this(new Optional<T>(value)) { }

        public OptionalSerialized(Optional<T> optional)
        {
            _hasValue = optional.HasValue();
            _value = optional.HasValue() ? optional.Value() : default;
        }
        
        public bool HasValue() => _hasValue;
        
        private Optional<T> ToOptional() => _hasValue ? new Optional<T>(_value) : Optional<T>.None;

        public T Value() => ToOptional().Value();
        
        public T GetValueOrDefault(T defaultValue) => ToOptional().ValueOrDefault(defaultValue);
        
        public static implicit operator Optional<T>(OptionalSerialized<T> serialized) => serialized.ToOptional();
        
        public static implicit operator OptionalSerialized<T>(Optional<T> optional) => new OptionalSerialized<T>(optional);
        
        public static implicit operator OptionalSerialized<T>(T value) => new OptionalSerialized<T>(value);

        public bool Equals(OptionalSerialized<T> other) => ToOptional().Equals(other.ToOptional());
        
        public override bool Equals(object obj) => obj is OptionalSerialized<T> other && Equals(other);
        
        public override int GetHashCode() => ToOptional().GetHashCode();
       
        public override string ToString() => ToOptional().ToString();
    }
}