using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace fahrizaDev.Engine.System.Tag {
    public class GameplayTagManager : ScriptableObject {

        public const char SEPARATOR = '.';

        private static GameplayTagManager instance = null;

        public static GameplayTagManager Get {
            get {
                if(instance == null) {
                    instance = FindObjectOfType<GameplayTagManager>();
                }
                if (instance == null) {
                    InitializeGameplayTagManager();
                }
                return instance;
            }
        }

        private static void InitializeGameplayTagManager() {
            instance = CreateInstance<GameplayTagManager>();
            instance.LoadGameplayTags();
            instance.hideFlags = HideFlags.DontUnloadUnusedAsset;
        }
#if UNITY_EDITOR
        [MenuItem("fahrizaDev/Reload Gameplay Tag Manager")]
#endif
        public static void ReloadGameplayTagManager() {
            if (instance) {
                Destroy(instance);
                instance = null;
            }
            InitializeGameplayTagManager();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)
#if UNITY_EDITOR
            , InitializeOnLoadMethod, MenuItem("fahrizaDev/Rebuild Tag Data")
#endif
            ]
        public static void RebuildTagData() {
            Get.LoadGameplayTags();
        }

        //mapping of string to gameplay tags and its child
        //index 0 will always be the tag itself
        //index 1-all will be the childs
        private Dictionary<string, List<GameplayTag>> gameplayTags = new Dictionary<string, List<GameplayTag>>();

        //will contain all the root tags
        private List<string> rootTags = new List<string>();

        private void LoadGameplayTags() {
            //todo : use addressables instead
            gameplayTags.Clear();
            rootTags.Clear();
            GameplayTagConfig[] gameplayTagConfigs = Resources.LoadAll<GameplayTagConfig>("/");
            foreach (GameplayTagConfig config in gameplayTagConfigs) {
                foreach (string tagString in config.tagList) {
                    //add first tag
                    RegisterTag(tagString);
                }
            }
            foreach (var tagEntry in gameplayTags) {
                Debug.Log(string.Format("Register {0} to Tag Manager", tagEntry.Key));
            }
        }


        public void GetAllTags(List<GameplayTag> outTags) {
            foreach(var entry in gameplayTags) {
                outTags.Add(entry.Value[0]);
            }
        }

        public void GetAllRootTags(List<GameplayTag> outTags) {
            foreach (var entry in rootTags) {
                outTags.Add(RequestGameplayTag(entry));
            }
        }

        public bool GetChildTags(GameplayTag rootTag, List<GameplayTag> outTags) {
            if (rootTag.IsValid) {
                List<GameplayTag> childTags = new List<GameplayTag>();
                childTags.AddRange(gameplayTags[rootTag.ToString()]);
                childTags.RemoveAt(0);
                outTags.AddRange(childTags);
                return true;
            }
            return false;
        }

        public GameplayTag RequestGameplayTag(string tag) {
            GameplayTag resultTag = GameplayTag.Empty;
            if (!string.IsNullOrEmpty(tag) && gameplayTags.ContainsKey(tag)) {
                resultTag = gameplayTags[tag][0];
            }
            return resultTag;
        }

        private bool RegisterTag(string tagString) {
            if (string.IsNullOrEmpty(tagString)) {
                return false;
            }
            if (gameplayTags.ContainsKey(tagString)) {
                return true;
            }
            GameplayTag newTag = new GameplayTag(tagString);
            List<GameplayTag> tagList = new List<GameplayTag>();
            tagList.Add(newTag);
            gameplayTags.Add(tagString, tagList);
            int lastIndex = tagString.LastIndexOf(SEPARATOR);
            if(lastIndex == -1) {
                rootTags.Add(tagString);
                return true;
            }
            string parentTag = tagString.Remove(lastIndex);
            if (RegisterTag(parentTag)) {
                gameplayTags[parentTag].Add(newTag);
            }
            return true;
        }
    }
}