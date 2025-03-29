using System;
using System.Collections.Generic;
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

        string existingSessionId = PlayerPrefs.GetString("__ngio_session_id", null);

#if UNITY_WEBGL
        if (!string.IsNullOrWhiteSpace(Application.absoluteURL)) {
            Dictionary<string, string> qparams = GetQueryParams(new Uri(Application.absoluteURL));
            if (qparams.TryGetValue("ngio_session_id", out string sessId)) {
                existingSessionId = sessId;
            }
        }
#endif
        _communicator = new UnityCoroutineCommunicator(setts.AppId, setts.EncryptionKey, this,
            setts.AppVersion, setts.DebugMode, setts.PreloadMedals, setts.PreloadScores,
            setts.LogViewOnInit, existingSessionId);

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

        _communicator.Ready += (sender, args) => {
            // save to playerprefs
            if (!_communicator.LoginSkipped
                && _communicator.Session != null
                && _communicator.Session.Value.RememberSession) {
                PlayerPrefs.SetString("__ngio_session_id", _communicator.Session.Value.Id);
            }
        };
    }

    private Dictionary<string, string> GetQueryParams(Uri uri) {
        if (uri == null) return null;

        Dictionary<string, string> paramsDict = new();
        string query = uri.Query.TrimStart('?');
        foreach (string queryParam in query.Split("&")) {
            string[] qParams = queryParam.Split("=");
            paramsDict.Add(qParams[0], qParams[1] ?? null);
        }

        return paramsDict;
    }
}
