using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NewgroundsIODotNet.Unity {
    [Serializable]
    public struct MedalSpriteDef {
        [SerializeField] public int MedalId;
        [SerializeField] public Sprite MedalSprite;
    }
}