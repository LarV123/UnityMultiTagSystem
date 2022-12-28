using UnityEngine;

namespace fahrizaDev.Engine.System.Tag {
    public static class GameplayTagUtils {
        
        public static GameplayTagContainer GetTags(this GameObject gameObject) {
            GameplayTagContainerComponent tagComponent = gameObject.GetComponent<GameplayTagContainerComponent>();
            if (tagComponent) {
                return tagComponent.TagContainer;
            }
            return new GameplayTagContainer();
        }
        public static GameplayTagContainer GetTags(this Component gameObject) {
            GameplayTagContainerComponent tagComponent = gameObject.GetComponent<GameplayTagContainerComponent>();
            if (tagComponent) {
                return tagComponent.TagContainer;
            }
            return new GameplayTagContainer();
        }
    }
}