using System;
using NewgroundsIODotNet.DataModels;
using NewgroundsIODotNet.Logging;
using NewgroundsIODotNet.Unity;
using UnityEngine;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;

[AddComponentMenu("Scripts/NGIO.NET Tank Engine")]
public class NGIONet : MonoBehaviour {
    private UnityCoroutineCommunicator _communicator;


    private static NGIONet _vessel;
    public static NGIONet Engine => _vessel;

    public UnityCoroutineCommunicator Comms => _communicator;


    /// <summary>
    /// Fired when a Medal is unlocked.
    /// </summary>
    [Space(20)]
    [SerializeField]
    private UnityEvent<Medal> MedalUnlocked;

    [SerializeField]
    private NgioDotNetSettings NgioSettings;

    // Start is called before the first frame update
    void Awake() {
        // This WILL restart the Communicator 
        // Nuclear option. Should only be put in a preloading scene.
        if (_vessel != null && _vessel != this) {
            Destroy(this.gameObject);
        }
        else {
            _vessel = this;
            DontDestroyOnLoad(this);
        }

        // TODO: Store SessionID in Playerprefs
        NgioDotNetSettings setts = NgioSettings;
        Debug.Log(NgioSettings);
        _communicator = new UnityCoroutineCommunicator(setts.AppId, setts.EncryptionKey, this,
            setts.AppVersion, setts.DebugMode, setts.PreloadMedals, setts.PreloadScores,
            setts.LogViewOnInit);

        _communicator.MedalUnlocked += (sender, medal) => MedalUnlocked?.Invoke(medal);

        _communicator.LogMessageReceived += (sender, info) => {
            switch (info.Severity) {
                case LogSeverity.Info:
                    Debug.Log(info.Message);
                    break;
                case LogSeverity.Warning:
                    Debug.LogWarning(info.Message);
                    break;
                case LogSeverity.Error:
                    Debug.LogError(info.Message);
                    break;
                case LogSeverity.Critical:
                    Debug.LogError($"[CRITICAL] info.Message");
                    break;
            }
        };
    }
}
