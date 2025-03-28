using System;
using System.Collections;
using NewgroundsIODotNet.Components.Interfaces;
using NewgroundsIODotNet.DataModels;
using NewgroundsIODotNet.Enums;
using NewgroundsIODotNet.Logging;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace NewgroundsIODotNet.Unity {
    public class UnityCoroutineCommunicator : NgioCommunicator {
        private MonoBehaviour _vessel;
        private Coroutine _heartbeatCoroutine;
        private float _heartbeatDelay;
        private bool _heartbeatOnline;

        public UnityCoroutineCommunicator(
            string appId,
            string encryptionKey,
            MonoBehaviour vessel,
            string appVersion = "0.0.0",
            bool debugMode = false,
            bool preloadMedals = false,
            bool preloadScores = false,
            bool logViewOnInit = true,
            string sessionId = null, string host = null) :
            base(appId, encryptionKey, appVersion, debugMode, preloadMedals, preloadScores,
                logViewOnInit, sessionId, host) {

            _vessel = vessel;
            _host = host ?? "localhost";

#if UNITY_WEBGL
            // WebGL only: Have Communicator automatically take from the URL if null
            if (!string.IsNullOrWhiteSpace(Application.absoluteURL)) {
                _host = host ?? new Uri(Application.absoluteURL).Host;
            }
#endif
        }

        /// <summary>
        /// Sets the MonoBehaviour that will be used to execute Coroutines.
        /// </summary>
        /// <remarks>The Communicator will refuse to send requests without one.</remarks>
        /// <param name="obj"></param>
        public void SetUnityVessel(MonoBehaviour obj) {
            _vessel = obj;
        }

        public override void GetSaveSlotData(SaveSlot slot, Action<string> responseCallback = null) {
            if (_vessel == null) return; // no vessel to carry the request with
            _vessel.StartCoroutine(GetSaveSlotDataCoroutine(slot, responseCallback));
        }

        private IEnumerator GetSaveSlotDataCoroutine(SaveSlot slot, Action<string> responseCallback = null) {
            if (slot.Url == null) yield break;
            using (UnityWebRequest webReq = UnityWebRequest.Get(slot.Url)) {
                yield return webReq.SendWebRequest();

                if (webReq.result != UnityWebRequest.Result.Success) {
                    UnityEngine.Debug.LogError($"NGIO.NET Unity: Trying to get the data from save slot #{slot.Id} failed due to an error with the HTTP client: {webReq.error}");
                    yield break;
                }

                if (webReq.responseCode != 200) {
                    UnityEngine.Debug.LogError($"NGIO.NET Unity: Trying to get the data from save slot #{slot.Id} wasn't successful: HTTP {webReq.responseCode}");
                    yield break;
                }

                responseCallback?.Invoke(webReq.downloadHandler.text);
            }
        }

        public override void OpenUrl(string url) {
            Application.OpenURL(url);
        }

        public override void StartHeartbeat(float seconds = 10) {
            _heartbeatDelay = seconds;
            _heartbeatOnline = true;
            _heartbeatCoroutine = _vessel.StartCoroutine(InternalHeartbeatForever());
        }

        public override void SetHeartbeatSpeed(float newSeconds) {
            _heartbeatDelay = newSeconds;
        }

        public override void StopHeartbeat() {
            if (_heartbeatCoroutine != null) {
                _vessel.StopCoroutine(_heartbeatCoroutine);
            }

            _heartbeatOnline = false;
        }

        public override void SendRequest(INgioComponentRequest[] components, Action<NgioServerResponse> callback,
            Session? forcedSession = null) {
            _vessel.StartCoroutine(SendRequestCoroutine(components, false, forcedSession, callback));
        }

        public override void SendSecureRequest(INgioComponentRequest component, Action<NgioServerResponse> callback) {
            _vessel.StartCoroutine(SendRequestCoroutine(new[] { component }, false, null, callback));
        }

        internal IEnumerator InternalHeartbeatForever() {
            while (_heartbeatOnline) {
                if (_heartbeatPending) {
                    yield return new WaitForEndOfFrame();
                    continue;
                }
                yield return new WaitForSecondsRealtime(_heartbeatDelay);
                HeartBeat();
            }

            yield break;
        }

        public IEnumerator SendRequestCoroutine(INgioComponentRequest[] components, bool forceSecure,
            Session? forceSession = null, Action<NgioServerResponse> callback = null) {

            if (_vessel == null) yield break; // no vessel to carry the request with
            NgioServerRequest newRequest = new NgioServerRequest(this, forceSession) {
                Debug = DebugMode,
                SecurityLevel = SecurityLevel.None,
                ExecutedComponents = components
            };

            if (Secured) {
                newRequest.SecurityLevel = forceSecure ? SecurityLevel.ForceAll : SecurityLevel.OnlyRequired;
            }

            string jsonToSend = JsonConvert.SerializeObject(newRequest, RequestConverter, SecureRequestConverter);

            WWWForm formData = new WWWForm();
            formData.AddField("request", jsonToSend);

            using (UnityWebRequest webReq = UnityWebRequest.Post(NewgroundsGatewayUrl, formData)) {
                yield return webReq.SendWebRequest();
                LastExecution = DateTime.Now;

                if (webReq.result != UnityWebRequest.Result.Success)
                // by this point it's done so it's success or nothing
                {
                    OnServerUnavailable();
                    StopHeartbeat();
                    ConnectionStatus = ConnectionStatus.ServerUnreachable;
                    yield break;
                }

                if (webReq.responseCode != 200) {
                    OnCommunicationError();
                    ConnectionStatus = ConnectionStatus.ServerUnavailable;
                    StopHeartbeat();
                    OnLogMessage($"NGIO.NET Unity: NG Connectivity error, sending request to Newgrounds.io returned {webReq.responseCode}",
                        null, LogSeverity.Critical);
                    yield break;
                }

                NgioServerResponse deserializedResp =
                    JsonConvert.DeserializeObject<NgioServerResponse>(webReq.downloadHandler.text, ServerResponseConverter,
                        ResponseConverter);
                ProcessResponse(deserializedResp);
                callback?.Invoke(deserializedResp);
                yield break;
            }
        }
    }

}