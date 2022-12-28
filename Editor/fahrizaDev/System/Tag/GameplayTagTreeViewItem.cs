using fahrizaDev.Engine.System.Tag;
using System.Collections;
using UnityEngine;
using UnityEditor.IMGUI.Controls;

namespace fahrizaDev.Engine.System.Tag.Editor {
    internal class GameplayTagTreeViewItem : TreeViewItem {

        public bool isEnabled;
        public string text = "";
        public string tag;

        public GameplayTagTreeViewItem(int id, int depth, string name) : base(id, depth, name) {

        }
    }
}