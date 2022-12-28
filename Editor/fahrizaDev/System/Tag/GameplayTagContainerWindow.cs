using fahrizaDev.Engine.System.Tag;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace fahrizaDev.Engine.System.Tag.Editor {
    public class GameplayTagContainerWindow : EditorWindow {

        [SerializeField]
        private TreeViewState treeViewState;

        private GameplayTagContainerTreeView treeView;

        private GameplayTagContainer context;

        private List<string> pendingRemoveTag = new List<string>();

        private void OnEnable() {
            if (treeViewState == null) {
                treeViewState = new TreeViewState();
            }
            if(treeView == null) {
                treeView = new GameplayTagContainerTreeView(treeViewState);
            }
        }

        public void SetGameplayTagContainerContext(GameplayTagContainer context) {
            if (treeViewState == null) {
                treeViewState = new TreeViewState();
            }
            treeView = new GameplayTagContainerTreeView(treeViewState, context);
            this.context = context;
        }

        private void OnGUI() {
            GUI.Box(new Rect(0, 0, position.width, position.height), "", EditorStyles.helpBox);
            float tagViewHeight = Mathf.Max(position.height * 0.2f, 100.0f);
            GUI.BeginScrollView(new Rect(0, 0, position.width, tagViewHeight), Vector2.one * 10, new Rect(0, 0, position.width, tagViewHeight));
            var icon = EditorGUIUtility.IconContent("d_winbtn_win_close@2x");
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            float accumulateSize = 0;
            var oldBackgroundColor = GUI.backgroundColor;
            var darkerBackgroundColor = oldBackgroundColor - 2 * Color.grey;
            GUI.backgroundColor = darkerBackgroundColor;
            foreach (string tag in context.tags) {
                GUIContent content = new GUIContent(tag);
                GUIStyle style = new GUIStyle(EditorStyles.label);
                style.alignment = TextAnchor.MiddleCenter;
                float labelWidth = style.CalcSize(content).x;
                accumulateSize += labelWidth + EditorStyles.iconButton.fixedWidth + EditorStyles.helpBox.CalcSize(new GUIContent("")).x;
                if (accumulateSize > position.width - GUI.skin.verticalScrollbar.fixedWidth) {
                    EditorGUILayout.EndHorizontal();
                    accumulateSize = 0;
                    EditorGUILayout.BeginHorizontal();
                }
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.Width(labelWidth + EditorStyles.iconButton.fixedWidth));
                GUILayout.Label(content, style);
                GUI.backgroundColor = oldBackgroundColor;
                if (GUILayout.Button(icon, EditorStyles.iconButton)) {
                    pendingRemoveTag.Add(tag);
                }
                GUI.backgroundColor = darkerBackgroundColor;
                EditorGUILayout.EndHorizontal();
            }
            GUI.backgroundColor = oldBackgroundColor;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            GUI.EndScrollView();
            treeView.OnGUI(new Rect(0, tagViewHeight, position.width, position.height-tagViewHeight));
            foreach(string removeTag in pendingRemoveTag) {
                context.RemoveAllGameplayTag(removeTag);
            }
            pendingRemoveTag.Clear();
        }

        [MenuItem("fahrizaDev/Tag Window")]
        static void OpenTagWindow() {
            var window = GetWindow<GameplayTagContainerWindow>();
            window.titleContent = new GUIContent("Gameplay Tag");
            window.Show();
        }

        public static void ShowModalWindow(GameplayTagContainer container) {
            GameplayTagContainerWindow window = ScriptableObject.CreateInstance(typeof(GameplayTagContainerWindow)) as GameplayTagContainerWindow;
            window.SetGameplayTagContainerContext(container);
            window.titleContent = new GUIContent("Add New Tag");
            window.ShowModalUtility();
        }
    }
}