using fahrizaDev.Editor.Utils;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace fahrizaDev.Engine.System.Tag.Editor { 

    [CustomPropertyDrawer(typeof(GameplayTag))]
    public class GameplayTagPropertyDrawer : PropertyDrawer {

        private GameplayTag tag;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, label);

            tag = PropertyDrawerUtils.GetPropertyInstance<GameplayTag>(property);

            SerializedProperty tagProperty = property.FindPropertyRelative("tag");

            string tagValue = tagProperty.stringValue;

            if (string.IsNullOrEmpty(tagValue)) {
                tagValue = "Empty Tag";
            }

            var icon = EditorGUIUtility.IconContent("d_winbtn_win_close@2x");

            Rect dropdownPosition = position;
            dropdownPosition.width -= EditorStyles.iconButton.fixedWidth;
            Rect removeButtonPosition = position;
            removeButtonPosition.x = removeButtonPosition.xMax - EditorStyles.iconButton.fixedWidth;
            removeButtonPosition.width = EditorStyles.iconButton.fixedWidth;

            bool isDropdown = EditorGUI.DropdownButton(dropdownPosition, new GUIContent(tagValue), FocusType.Keyboard);

            if (isDropdown) {

                GameplayTagWindow window = ScriptableObject.CreateInstance(typeof(GameplayTagWindow)) as GameplayTagWindow;
                window.SetGameplayTagContext(tag);
                window.titleContent = new GUIContent("Add New Tag");
                //window.ShowPopup();

                window.ShowAsDropDown(GUIUtility.GUIToScreenRect(position), new Vector2(position.width, 300));
            }

            if (GUI.Button(removeButtonPosition, icon, EditorStyles.iconButton)) {
                tag.Assign("");
            }

            EditorGUI.EndProperty();
        }
    }
}