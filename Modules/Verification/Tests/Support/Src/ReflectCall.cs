using System;
using System.Linq;
using System.Reflection;

namespace GameFramework.Verification.Tests
{
    public static class ReflectUtils
    {
        public static object GetInstance(
            string typeName,
            string assemblyName = null,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
        {
            var asm = assemblyName == null
                ? AppDomain.CurrentDomain
                    .GetAssemblies()
                    .FirstOrDefault(a => a.GetType(typeName, false) != null)
                : AppDomain.CurrentDomain
                    .GetAssemblies()
                    .FirstOrDefault(a => a.GetName().Name == assemblyName);

            if (asm == null) throw new Exception("Assembly not found");
            var type = asm.GetType(typeName, true);
            var ctor = type.GetConstructor(flags, null, Type.EmptyTypes, null);
            if (ctor == null) throw new Exception("No suitable constructor");
            return ctor.Invoke(null);
        }

        public static FieldInfo GetField(object obj, string fieldName,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
        {
            return obj.GetType().GetField(fieldName, flags);
        }

        public static void SetFieldValue(object obj, string fieldName, object value,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
        {
            var field = obj.GetType().GetField(fieldName, flags);
            if (field == null) throw new Exception("Field not found");
            field.SetValue(obj, value);
        }

        public static object GetFieldValue(object obj, string fieldName,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
        {
            var field = obj.GetType().GetField(fieldName, flags);
            if (field == null) throw new Exception("Field not found");
            return field.GetValue(obj);
        }
    }
}