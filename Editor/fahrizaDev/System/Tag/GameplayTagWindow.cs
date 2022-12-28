using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace fahrizaDev.Engine.System.Tag.Editor {
    public class GameplayTagWindow : EditorWindow {

        [SerializeField]
        private TreeViewState treeViewState;

        private GameplayTagTreeView treeView;

        private GameplayTag context;

        private void OnEnable() {
            if (treeViewState == null) {
                treeViewState = new TreeViewState();
            }
            if (treeView == null) {
                treeView = new GameplayTagTreeView(treeViewState);
            }
        }

        public void SetGameplayTagContext(GameplayTag context) {
            if (treeViewState == null) {
                treeViewState = new TreeViewState();
            }
            treeView = new GameplayTagTreeView(treeViewState, context);
            this.context = context;
        }

        private void OnGUI() {
            GUI.Box(new Rect(0, 0, position.width, position.height), "", EditorStyles.helpBox);
            treeView.OnGUI(new Rect(0, 0, position.width, position.height));
        }

        [MenuItem("fahrizaDev/Tag Window")]
        static void OpenTagWindow() {
            var window = GetWindow<GameplayTagWindow>();
            window.titleContent = new GUIContent("Gameplay Tag");
            window.Show();
        }

        public static void ShowModalWindow(GameplayTag tag) {
            GameplayTagWindow window = ScriptableObject.CreateInstance(typeof(GameplayTagWindow)) as GameplayTagWindow;
            window.SetGameplayTagContext(tag);
            window.titleContent = new GUIContent("Add New Tag");
            window.ShowModalUtility();
        }
    }
}