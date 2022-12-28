using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace fahrizaDev.Engine.System.Tag {
    [Serializable]
    public class GameplayTagContainer : IEnumerable<GameplayTag> {

        [SerializeField, HideInInspector]
        public List<string> tags;

        private int iteratorIndex = 0;

        public GameplayTagContainer() {
            tags = new List<string>();
        }

        public GameplayTagContainer(GameplayTagContainer other) {
            tags = new List<string>();
            tags.AddRange(other.tags);
        }

        public GameplayTagContainer(string tag) {
            tags = new List<string>();
            AddGameplayTag(tag);
        }

        public bool Contains(string otherTagStr) {
            GameplayTag otherTag = GameplayTagManager.Get.RequestGameplayTag(otherTagStr);
            return Contains(otherTag);
        }

        public bool Contains(GameplayTag otherTag) {
            if (!otherTag.IsValid) {
                return false;
            }
            foreach (GameplayTag tag in this) {
                if (tag.FindCommonParent(otherTag, out GameplayTag parentTag)) {
                    if (parentTag == otherTag) {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool ContainsAll(GameplayTagContainer container) {
            bool result = true;
            foreach (GameplayTag otherTag in container) {
                if (!Contains(otherTag)) {
                    result = false;
                    break;
                }
            }
            return result;
        }

        public bool ContainsAny(GameplayTagContainer container) {
            bool result = false;
            foreach(GameplayTag otherTag in container) {
                if (Contains(otherTag)) {
                    result = true;
                    break;
                }
            }
            return result;
        }

        public bool Matches(string otherTagStr) {
            GameplayTag otherTag = GameplayTagManager.Get.RequestGameplayTag(otherTagStr);
            return Matches(otherTag);
        }

        public bool Matches(GameplayTag otherTag) {
            if (!otherTag.IsValid) {
                return false;
            }
            foreach (GameplayTag tag in this) {
                if (tag == otherTag) {
                    return true;
                }
            }
            return false;
        }

        public bool MatchesAll(GameplayTagContainer container) {
            bool result = true;
            foreach(GameplayTag otherTag in container) {
                if (!Matches(otherTag)) {
                    result = false;
                    break;
                }
            }
            return result;
        }

        public bool MatchesAny(GameplayTagContainer container) {
            bool result = false;
            foreach (GameplayTag otherTag in container) {
                if (Matches(otherTag)) {
                    result = true;
                    break;
                }
            }
            return result;
        }

        public void AddGameplayTag(string otherTagStr) {
            GameplayTag otherTag = GameplayTagManager.Get.RequestGameplayTag(otherTagStr);
            if (!otherTag.IsValid) {
                return;
            }
            string foundSubtag = tags.Find((tagStr) => {
                GameplayTag tag = GameplayTagManager.Get.RequestGameplayTag(tagStr);
                if (!tag.IsValid) {
                    return false;
                }
                if (tag.IsSubtagOf(otherTag)) {
                    return true;
                }
                return false;
            });
            if (!string.IsNullOrEmpty(foundSubtag)) {
                return;
            }
            tags.RemoveAll((tagStr) => {
                GameplayTag tag = GameplayTagManager.Get.RequestGameplayTag(tagStr);
                if (!tag.IsValid) {
                    return false;
                }
                if (tag.IsParentOf(otherTag)) {
                    return true;
                }
                return false;
            });
            tags.Add(otherTagStr);
        }

        public void RemoveGameplayTag(string otherTagStr) {
            GameplayTag otherTag = GameplayTagManager.Get.RequestGameplayTag(otherTagStr);
            if (!otherTag.IsValid) {
                tags.Remove(otherTagStr);
                return;
            }
            tags.RemoveAll((tagStr) => {
                GameplayTag tag = GameplayTagManager.Get.RequestGameplayTag(tagStr);
                if(!tag.IsValid) {
                    return false;
                }
                if (tag.IsSubtagOf(otherTag)) {
                    return true;
                }
                return false;
            });
            if (otherTag.Parent.IsValid) {
                AddGameplayTag(otherTag.Parent.ToString());
            }
        }

        public void RemoveAllGameplayTag(string otherTagStr) {
            GameplayTag otherTag = GameplayTagManager.Get.RequestGameplayTag(otherTagStr);
            if (!otherTag.IsValid) {
                tags.Remove(otherTagStr);
                return;
            }
            tags.RemoveAll((tagStr) => {
                GameplayTag tag = GameplayTagManager.Get.RequestGameplayTag(tagStr);
                if (!tag.IsValid) {
                    return false;
                }
                if (tag.IsSubtagOf(otherTag)) {
                    return true;
                }
                return false;
            });
        }

        private IEnumerable<GameplayTag> Tags() {
            foreach(string tagStr in tags){
                GameplayTag tag = GameplayTagManager.Get.RequestGameplayTag(tagStr);
                if (tag.IsValid) {
                    yield return tag;
                }
            }
        }

        public IEnumerator<GameplayTag> GetEnumerator() {
            return Tags().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return Tags().GetEnumerator();
        }

    }
}