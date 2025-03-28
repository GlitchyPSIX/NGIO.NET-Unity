using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using NewgroundsIODotNet.DataModels;
using UnityEngine;

namespace NewgroundsIODotNet.Unity {
    [CreateAssetMenu(menuName = "Newgrounds/Medal Sprite Collection")]
    public class MedalSpriteCollection : ScriptableObject {
        [SerializeField] public Sprite FallbackSprite;
        [SerializeField] public MedalSpriteDef[] MedalSpriteDefinitions = Array.Empty<MedalSpriteDef>();
        private bool _populated;

        private Dictionary<int, Sprite> _medalSprites = new ();

        public IReadOnlyDictionary<int, Sprite> MedalSprites {
            get {
                return _medalSprites;
            }
        }

        private void PopulateMap() {
            _medalSprites = new();
            foreach (MedalSpriteDef definition in MedalSpriteDefinitions) {
                _medalSprites.Add(definition.MedalId, definition.MedalSprite);
            }
        }

        /// <summary>
        /// Gets a medal's sprite from the collection.
        /// </summary>
        /// <param name="medal">Medal to get the sprite for</param>
        /// <returns>The medal's assigned sprite. Fallback Sprite if null.</returns>
        [CanBeNull]
        public Sprite GetMedalSprite(Medal medal) {
            if (_populated) PopulateMap();
            Sprite outSprite;
            if (!_medalSprites.TryGetValue(medal.Id, out outSprite)) {
                return FallbackSprite;
            }
            else {
                return outSprite != null ? outSprite : FallbackSprite;
            }
        }
    }
}
