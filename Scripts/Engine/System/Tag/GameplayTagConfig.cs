using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fahrizaDev.Engine.System.Tag {
    [Serializable, CreateAssetMenu(menuName = "Config/GameplayTag")]
    public class GameplayTagConfig : ScriptableObject {

        //todo
        public List<string> tagList;

    }
}