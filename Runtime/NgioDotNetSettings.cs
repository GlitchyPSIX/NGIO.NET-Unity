using UnityEngine;
namespace NewgroundsIODotNet.Unity {
    public class NgioDotNetSettings : ScriptableObject {

        [SerializeField]
        public string m_AppId;
        [SerializeField]
        public string m_EncryptionKey;
        [SerializeField]
        public string m_AppVersion;
        [SerializeField]
        public bool m_DebugMode;
        [SerializeField]
        public bool m_PreloadMedals;
        [SerializeField]
        public MedalSpriteCollection m_MedalSprites;
        [SerializeField]
        public bool m_PreloadScores;
        [SerializeField]
        public bool m_LogViewOnInit;

        public string AppId => m_AppId;
        public string EncryptionKey => m_EncryptionKey;
        public string AppVersion => m_AppVersion;
        public bool DebugMode => m_DebugMode;
        public bool PreloadMedals => m_PreloadMedals;
        public bool PreloadScores => m_PreloadScores;
        public bool LogViewOnInit => m_LogViewOnInit;
        public MedalSpriteCollection MedalSprites => m_MedalSprites;

    }
}