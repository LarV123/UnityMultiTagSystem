using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

using fahrizaDev.Editor.Utils;

namespace fahrizaDev.Engine.System.Tag.Editor {
    [CustomPropertyDrawer(typeof(GameplayTagContainer))]
    public class GameplayTagContainerPropertyDrawer : PropertyDrawer {

        private List<string> pendingRemoveList = new List<string>();

        private const float BORDER_PADDING = 5;
        private const int MAX_TAG_SHOWN = 5;

        private Vector2 scrollPosition;

        private float TAG_HEIGHT = EditorGUIUtility.singleLineHeight + BORDER_PADDING * 1.5f;

        private GUIStyle tagStyle;

        GameplayTagContainer tagContainer;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {

            if (tagContainer == null) {
                tagContainer = PropertyDrawerUtils.GetPropertyInstance<GameplayTagContainer>(property);
            }

            float buttonHeight = 2 * BORDER_PADDING + EditorGUIUtility.singleLineHeight;

            int tagShown = Mathf.Clamp(tagContainer.tags.Count, 0, MAX_TAG_SHOWN);

            if (tagContainer.tags.Count > 0) {
                return tagShown * TAG_HEIGHT + buttonHeight + 2 * EditorGUIUtility.singleLineHeight + 2.5f * BORDER_PADDING;
            }

            return EditorGUIUtility.singleLineHeight + BORDER_PADDING * 2.5f + buttonHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

            tagStyle = new GUIStyle(EditorStyles.helpBox);
            tagStyle.fontSize = 12;

            if (tagContainer == null) {
                tagContainer = PropertyDrawerUtils.GetPropertyInstance<GameplayTagContainer>(property);
            }

            EditorGUI.BeginProperty(position, label, property);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            int tagCount = tagContainer.tags.Count;

            GUI.Box(position, "", EditorStyles.helpBox);

            position = new Rect(position.x + BORDER_PADDING, position.y + BORDER_PADDING, position.width - BORDER_PADDING * 2, position.height - BORDER_PADDING * 2);

            // Draw label
            EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            position.y += EditorGUIUtility.singleLineHeight;

            if (tagCount > 0) {
                var curTagsRect = position;
                curTagsRect.height = EditorGUIUtility.singleLineHeight;
                GUI.Label(curTagsRect, "Current Tags :", EditorStyles.label);

                position.y = position.y + EditorGUIUtility.singleLineHeight;

                Rect tagViewRect = new Rect(position.x, position.y, position.width, TAG_HEIGHT * tagCount);
                Rect tagContainerRect = new Rect(0, 0, position.width, TAG_HEIGHT * tagCount);
                if (tagCount > MAX_TAG_SHOWN) {
                    tagViewRect.height = TAG_HEIGHT * MAX_TAG_SHOWN;
                    tagContainerRect.width -= GUI.skin.verticalScrollbar.fixedWidth;
                    scrollPosition = GUI.BeginScrollView(tagViewRect, scrollPosition, tagContainerRect);
                } else if( tagCount > 0){
                    GUI.BeginGroup(tagViewRect);
                }
                var groupPos = tagContainerRect;
                groupPos.x = 0;
                groupPos.y = 0;
                foreach (string tag in tagContainer.tags) {
                    bool isNotValid = !GameplayTagManager.Get.RequestGameplayTag(tag).IsValid;

                    if (isNotValid) {
                        DrawUnregisteredTag(ref groupPos, tag);
                    } else {
                        DrawRegisteredTag(ref groupPos, tag);
                    }
                }
                if(tagCount > MAX_TAG_SHOWN) {
                    GUI.EndScrollView();
                } else if (tagCount > 0) {
                    GUI.EndGroup();
                }
                position.y += tagViewRect.height;
            }

            position.y += BORDER_PADDING;
            position.height = EditorGUIUtility.singleLineHeight;

            GUIContent buttonContent = new GUIContent("Add Tag", "Add new tag to container");

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();

            if (GUI.Button(position, buttonContent)) {
                GameplayTagContainerWindow window = ScriptableObject.CreateInstance(typeof(GameplayTagContainerWindow)) as GameplayTagContainerWindow;
                window.SetGameplayTagContainerContext(tagContainer);
                window.titleContent = new GUIContent("Add New Tag");
                //window.ShowPopup();
                
                window.ShowAsDropDown(GUIUtility.GUIToScreenRect(position), new Vector2(position.width, 300));
                //GameplayTagWindow.ShowModalWindow(tagContainer);
            }

            foreach (string deleteTag in pendingRemoveList) {
                tagContainer.RemoveAllGameplayTag(deleteTag);
            }
            pendingRemoveList.Clear();
        }

        private void DrawRegisteredTag(ref Rect position, string tag) {

            //content to draw
            GUIContent boxContent = new GUIContent(tag, null, "Registered tag");
            var icon = EditorGUIUtility.IconContent("d_winbtn_win_close@2x");

            //draw box
            Rect boxPosition = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight + BORDER_PADDING);
            GUI.Box(boxPosition, boxContent, tagStyle);

            //draw button
            Rect buttonPosition = new Rect(boxPosition.x + boxPosition.width - EditorStyles.iconButton.fixedWidth - BORDER_PADDING,
                boxPosition.y + boxPosition.height / 2 - EditorStyles.iconButton.fixedHeight / 2,
                EditorStyles.iconButton.fixedWidth,
                EditorStyles.iconButton.fixedHeight);
            if (GUI.Button(buttonPosition, icon, EditorStyles.iconButton)) {
                pendingRemoveList.Add(tag);
            }

            position.y = position.y + EditorGUIUtility.singleLineHeight + BORDER_PADDING * 1.5f;
        }

        private void DrawUnregisteredTag(ref Rect position, string tag) {

            //content to draw
            GUIContent errorIcon = EditorGUIUtility.IconContent("Error");
            GUIContent boxContent = new GUIContent(tag, errorIcon.image, "Unregistered tag");
            var icon = EditorGUIUtility.IconContent("d_winbtn_win_close@2x");

            //draw box
            Rect boxPosition = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight + BORDER_PADDING);
            Color oldColor = GUI.backgroundColor;
            GUI.backgroundColor = oldColor + Color.red;
            GUI.Box(boxPosition, boxContent, tagStyle);
            GUI.backgroundColor = oldColor;

            //draw button
            Rect buttonPosition = new Rect(boxPosition.x + boxPosition.width - EditorStyles.iconButton.fixedWidth - BORDER_PADDING,
                boxPosition.y + boxPosition.height / 2 - EditorStyles.iconButton.fixedHeight / 2,
                EditorStyles.iconButton.fixedWidth,
                EditorStyles.iconButton.fixedHeight);
            if (GUI.Button(buttonPosition, icon, EditorStyles.iconButton)) {
                pendingRemoveList.Add(tag);
            }

            position.y = position.y + EditorGUIUtility.singleLineHeight + BORDER_PADDING * 1.5f;
        }
    }
}