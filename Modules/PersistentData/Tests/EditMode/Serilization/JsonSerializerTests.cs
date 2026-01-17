using System;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Newtonsoft.Json;

namespace GameFramework.PersistentData.Tests
{
    public class JsonSerializerTests
    {
        private JsonSerializer _serializer;

        [SetUp]
        public void SetUp()
        {
            _serializer = new GameFramework.PersistentData.JsonSerializer();
        }

        [Test]
        public void Serialize_UsesUtf8WithoutBom()
        {
            byte[] bytes = _serializer.Serialize(new SampleData { MyValue = 1, Text = "test" });

            // BOM for UTF-8: EF BB BF
            Assert.That(bytes.Length, Is.GreaterThan(3));
            Assert.That(bytes[0], Is.Not.EqualTo(0xEF));
            Assert.That(bytes[1], Is.Not.EqualTo(0xBB));
            Assert.That(bytes[2], Is.Not.EqualTo(0xBF));
        }

        [Test]
        public void Serialize_UsesCamelCase_IgnoresNulls_And_WritesEnumsAsStrings()
        {
            var data = new SampleData
            {
                MyValue = 5,
                Text = null, // должно быть проигнорировано (NullValueHandling.Ignore)
                EnumValue = ExampleEnum.SecondValue
            };

            byte[] bytes = _serializer.Serialize(data);
            string json = Encoding.UTF8.GetString(bytes);

            // camelCase
            StringAssert.Contains("\"myValue\":5", json);
            StringAssert.Contains("\"enumValue\":\"SecondValue\"", json);

            // null поле не должно сериализоваться
            Assert.That(json.Contains("text"), Is.False, $"JSON unexpectedly contains 'text': {json}");
            Assert.That(json.Contains("Text"), Is.False, $"JSON unexpectedly contains 'Text': {json}");
        }

        [Test]
        public void Deserialize_IgnoresUnknownMembers()
        {
            // unknownField должен быть проигнорирован (MissingMemberHandling.Ignore)
            const string json = "{\"myValue\":7,\"unknownField\":123,\"enumValue\":\"FirstValue\"}";
            byte[] bytes = Encoding.UTF8.GetBytes(json);

            SampleData obj = _serializer.Deserialize<SampleData>(bytes);

            Assert.That(obj, Is.Not.Null);
            Assert.That(obj.MyValue, Is.EqualTo(7));
            Assert.That(obj.EnumValue, Is.EqualTo(ExampleEnum.FirstValue));
        }

        [Test]
        public void TypeNameHandlingAuto_RoundTripsDerivedType_WhenDeclaredAsBase()
        {
            BaseType value = new DerivedType { DerivedValue = "hello" };

            byte[] bytes = _serializer.Serialize<BaseType>(value);
            BaseType deserialized = _serializer.Deserialize<BaseType>(bytes);

            Assert.That(deserialized, Is.TypeOf<DerivedType>());
            Assert.That(((DerivedType)deserialized).DerivedValue, Is.EqualTo("hello"));
        }

        [Test]
        public void Serialize_DoesNotThrow_OnReferenceLoop()
        {
            var node = new Node { Name = "root" };
            node.Next = node; // цикл

            Assert.DoesNotThrow(() => _serializer.Serialize(node));

            byte[] bytes = _serializer.Serialize(node);
            string json = Encoding.UTF8.GetString(bytes);

            StringAssert.Contains("\"name\":\"root\"", json);
            Assert.That(json.Length, Is.LessThan(10_000), "JSON looks unexpectedly huge (possible loop).");
        }

        [Test]
        public void DateTime_Roundtrip_PreservesUtcKind()
        {
            var wrapper = new DateWrapper
            {
                Value = new DateTime(2020, 1, 2, 3, 4, 5, DateTimeKind.Utc)
            };

            byte[] bytes = _serializer.Serialize(wrapper);
            DateWrapper back = _serializer.Deserialize<DateWrapper>(bytes);

            Assert.That(back, Is.Not.Null);
            Assert.That(back.Value, Is.EqualTo(wrapper.Value));
            Assert.That(back.Value.Kind, Is.EqualTo(DateTimeKind.Utc));
        }

        [Test]
        public void Deserialize_EmptyBytes_ReturnsNull_ForReferenceType()
        {
            SampleData obj = _serializer.Deserialize<SampleData>(ReadOnlyMemory<byte>.Empty);
            Assert.That(obj, Is.Null);
        }

        [Test]
        public void Deserialize_EmptyBytes_Throws_ForNonNullableValueType()
        {
            Assert.Throws<JsonSerializationException>(() => _serializer.Deserialize<int>(ReadOnlyMemory<byte>.Empty));
        }

        [Test]
        public void RoundTrip_UnicodeText_Works()
        {
            var data = new SampleData { MyValue = 1, Text = "Hello🙂" };

            byte[] bytes = _serializer.Serialize(data);
            SampleData back = _serializer.Deserialize<SampleData>(bytes);

            Assert.That(back, Is.Not.Null);
            Assert.That(back.Text, Is.EqualTo("Hello🙂"));
        }

        [Test]
        public void CustomSettings_AreUsed()
        {
            var custom = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver()
            };

            var serializer = new JsonSerializer(custom);

            byte[] bytes = serializer.Serialize(new SampleData { MyValue = 10, EnumValue = ExampleEnum.FirstValue });
            string json = Encoding.UTF8.GetString(bytes);

            Assert.That(json.Contains("\n") || json.Contains("\r"), Is.True, $"Expected indented JSON, got: {json}");

            StringAssert.Contains("\"MyValue\":", json);
        }

        // ===== Test models =====

        private enum ExampleEnum
        {
            FirstValue,
            SecondValue
        }

        [Serializable]
        private sealed class SampleData
        {
            public int MyValue { get; set; }
            public string Text { get; set; }
            public ExampleEnum EnumValue { get; set; }
        }

        private abstract class BaseType
        {
        }

        private sealed class DerivedType : BaseType
        {
            public string DerivedValue { get; set; }
        }

        private sealed class Node
        {
            public string Name { get; set; }
            public Node Next { get; set; }
        }

        private sealed class DateWrapper
        {
            public DateTime Value { get; set; }
        }
    }
}
