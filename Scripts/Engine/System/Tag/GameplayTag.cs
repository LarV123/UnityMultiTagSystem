using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fahrizaDev.Engine.System.Tag {
    [Serializable]
    public class GameplayTag {

        public static GameplayTag Empty {
            get {
                return emptyTag;
            }
        }
        private static GameplayTag emptyTag = new GameplayTag("");

        [SerializeField]
        private string tag;
        [SerializeField]
        private int depth;
        [SerializeField]
        private string tagParent;

        public bool IsValid {
            get {
                if (this == GameplayTag.Empty) {
                    return false;
                }
                return GameplayTagManager.Get.RequestGameplayTag(tag) == this;
            }
        }

        public GameplayTag Parent {
            get {
                return GameplayTagManager.Get.RequestGameplayTag(tagParent);
            }
        }

        public string LeafName {
            get {
                int lastIndex = tag.LastIndexOf('.');
                if (lastIndex < 0) {
                    return tag;
                }
                return tag.Substring(lastIndex + 1);
            }
        }

        public bool FindCommonParent(GameplayTag other, out GameplayTag parentTag) {
            if(other.Equals(this)) {
                parentTag = this;
                return true;
            }
            if(other.depth == this.depth) {
                parentTag = GameplayTag.Empty;
                return false;
            }
            if(other.depth < this.depth) {
                return Parent.FindCommonParent(other, out parentTag);
            }
            if (other.depth > this.depth) {
                return other.FindCommonParent(this, out parentTag);
            }
            parentTag = GameplayTag.Empty;
            return false;
        }

        public bool IsSubtagOf(GameplayTag other) {
            if(other.Equals(this)) {
                return true;
            }
            if (other.depth >= this.depth) {
                return false;
            }
            if (other.depth < this.depth) {
                return Parent.IsSubtagOf(other);
            }
            return false;
        }

        public bool IsParentOf(GameplayTag other) {
            return other.IsSubtagOf(this);
        }

        public GameplayTag(string tag) {
            this.tag = tag;
            int lastIndex = tag.LastIndexOf('.');
            depth = tag.Split('.').Length;
            tagParent = "";
            if (lastIndex >= 0) {
                tagParent = tag.Remove(lastIndex);
            }
        }

        public void Assign(string tag) {
            this.tag = tag;
            int lastIndex = tag.LastIndexOf('.');
            depth = tag.Split('.').Length;
            tagParent = "";
            if (lastIndex >= 0) {
                tagParent = tag.Remove(lastIndex);
            }
        }

        public override string ToString() {
            return tag;
        }

        public override bool Equals(object obj) {
            if(obj is null) {
                return false;
            }
            if(obj is GameplayTag) {
                GameplayTag other = (GameplayTag)obj;
                return other.tag == tag;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode() {
            return HashCode.Combine(tag, depth);
        }

        public static bool operator ==(GameplayTag lhs, GameplayTag rhs) {
            return Equals(lhs, rhs);
        }

        public static bool operator !=(GameplayTag lhs, GameplayTag rhs) {
            return !Equals(lhs, rhs);
        }
    }
}