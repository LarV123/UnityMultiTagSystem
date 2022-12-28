using System.Collections;
using UnityEngine;

namespace fahrizaDev.Engine.System.Tag {
    public class GameplayTagContainerComponent : MonoBehaviour {

        public GameplayTagContainer TagContainer => new GameplayTagContainer(tagContainer);

        [SerializeField]
        private GameplayTagContainer tagContainer;

    }
}