using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace fahrizaDev.Engine.System.Tag.Editor {
    public class GameplayTagTreeView : TreeView {

        int idCounter = 0;
        private GameplayTag tagContext;

        public GameplayTag context {
            get {
                return tagContext;
            }
        }

        public GameplayTagTreeView(TreeViewState state) : base(state) {
            if (tagContext == null) {
                tagContext = GameplayTag.Empty;
            }
            showBorder = true;
            Reload();
        }

        public GameplayTagTreeView(TreeViewState state, GameplayTag tagContext) : this(state) {
            this.tagContext = tagContext;
            Reload();
        }

        protected override TreeViewItem BuildRoot() {
            List<GameplayTag> tags = new List<GameplayTag>();
            GameplayTagManager.Get.GetAllRootTags(tags);
            var root = new GameplayTagTreeViewItem(idCounter++, -1, "root");
            foreach (GameplayTag child in tags) {
                DFSTag(root, 0, child);
            }
            return root;
        }

        private void DFSTag(GameplayTagTreeViewItem rootItem, int depth, GameplayTag curTag) {
            var newTreeItem = new GameplayTagTreeViewItem(idCounter++, depth, curTag.LeafName);
            newTreeItem.tag = curTag.ToString();
            if (rootItem.isEnabled || rootItem.depth < 0) {
                if (tagContext.IsSubtagOf(curTag)) {
                    SetExpanded(newTreeItem.id, true);
                    newTreeItem.isEnabled = true;
                }
            }
            rootItem.AddChild(newTreeItem);
            List<GameplayTag> childs = new List<GameplayTag>();
            GameplayTagManager.Get.GetChildTags(curTag, childs);
            foreach (GameplayTag child in childs) {
                DFSTag(newTreeItem, depth + 1, child);
            }
        }

        protected override void RowGUI(RowGUIArgs args) {
            var item = (GameplayTagTreeViewItem)args.item;

            UnityEngine.Event evt = UnityEngine.Event.current;
            extraSpaceBeforeIconAndLabel = 18f;

            Rect toggleRect = args.rowRect;
            toggleRect.x += GetContentIndent(args.item);
            toggleRect.width = 16f;

            // Ensure row is selected before using the toggle (usability)
            if (evt.type == EventType.MouseDown && toggleRect.Contains(evt.mousePosition))
                SelectionClick(args.item, false);

            EditorGUI.BeginChangeCheck();
            bool isEnabled = EditorGUI.Toggle(toggleRect, item.isEnabled);
            if (EditorGUI.EndChangeCheck()) {
                if (isEnabled) {
                    RecusiveEnableUp(item);
                    tagContext.Assign(item.tag);
                } else {
                    RecusiveDisableDown(item);
                    tagContext.Assign("");
                }
                ValidateTagItemRecursive(rootItem as GameplayTagTreeViewItem);
            }
            // Text
            base.RowGUI(args);
        }

        private void RecusiveEnableUp(GameplayTagTreeViewItem item) {
            if (item is null || item.isEnabled) {
                return;
            }
            item.isEnabled = true;
            RecusiveEnableUp(item.parent as GameplayTagTreeViewItem);
        }

        private void RecusiveDisableDown(GameplayTagTreeViewItem item) {
            if (item is null || !item.isEnabled) {
                return;
            }
            item.isEnabled = false;
            if (item.children is null) {
                return;
            }
            foreach (var child in item.children) {
                RecusiveDisableDown(child as GameplayTagTreeViewItem);
            }
        }

        private void ValidateTagItemRecursive(GameplayTagTreeViewItem item) {
            if (item is null || !item.isEnabled) {
                return;
            }
            item.isEnabled = GameplayTagManager.Get.RequestGameplayTag(item.tag).IsParentOf(tagContext);
            if (item.children is null) {
                return;
            }
            foreach (var child in item.children) {
                ValidateTagItemRecursive(child as GameplayTagTreeViewItem);
            }
        }
    }
}