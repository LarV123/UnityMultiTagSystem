using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace fahrizaDev.Editor.Utils {
    public static class PropertyDrawerUtils {

        public static object GetPropertyInstance(SerializedProperty property) {

            string path = property.propertyPath;

            object obj = property.serializedObject.targetObject;
            var type = obj.GetType();

            var fieldNames = path.Split('.');
            for (int i = 0; i < fieldNames.Length; i++) {
                var info = type.GetField(fieldNames[i], BindingFlags.NonPublic | BindingFlags.Public| BindingFlags.Instance);
                if (info == null)
                    break;

                // Recurse down to the next nested object.
                obj = info.GetValue(obj);
                type = info.FieldType;
            }

            return obj;
        }

        public static T GetPropertyInstance<T>(SerializedProperty property) {
            return (T)GetPropertyInstance(property);
        }

    }
}