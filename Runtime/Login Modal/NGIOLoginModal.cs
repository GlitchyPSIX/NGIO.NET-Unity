using System;
using System.Collections;
using System.Collections.Generic;
using NewgroundsIODotNet.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class NGIOLoginModal : MonoBehaviour {
    [SerializeField] private GameObject[] PhaseGameObjects;
    private bool _canCommunicate = false;
    /* phases:
       0 - connecting
       1 - asking for login
       2 - logging in
       3 - ready
       4 - error
     */
    [SerializeField] private UnityEvent OnAllReady;
    [SerializeField] private TextMeshProUGUI StatusText;
    [SerializeField] private TextMeshProUGUI PlayerNameText;
    [SerializeField] private GameObject LogoutObject;
    [SerializeField] private bool ShowLogoutButton = true;
    private bool ForceSkip = false;

    public void LogIn() {
        if (!_canCommunicate) return;
        NGIONet.Engine.Comms.LogIn();
        NGIONet.Engine.Comms.SetHeartbeatSpeed(2);
        SwitchToPhase(2);
    }

    public void TryAgain() {
        if (!_canCommunicate) return;
        NGIONet.Engine.Comms.ResetConnectionState();
        NGIONet.Engine.Comms.Initialize();
        SwitchToPhase(0);
    }

    public void SkipOverride() {
        ForceSkip = false;
        SkipLogin();
        SetPlayerNameStatus();
    }

    public void SkipLogin() {
        if (!_canCommunicate) return;
        NGIONet.Engine.Comms.SkipLogin();
        NGIONet.Engine.Comms.SetHeartbeatSpeed(10);

    }

    public void CancelLogin() {
        if (!_canCommunicate) return;
        NGIONet.Engine.Comms.CancelLogin();
        NGIONet.Engine.Comms.SetHeartbeatSpeed(10);
        SwitchToPhase(1);
    }

    public void LogOut() {
        if (!_canCommunicate) return;
        if (NGIONet.Engine.Comms.HasUser) {
            NGIONet.Engine.Comms.LogOut();
        }
        else {
            NGIONet.Engine.Comms.ResetConnectionState();
            NGIONet.Engine.Comms.Initialize();
            SwitchToPhase(0);
        }

    }

    private void SwitchToPhase(int phase) {
        for (int index = 0; index < PhaseGameObjects.Length; index++) {
            GameObject phaseGameObject = PhaseGameObjects[index];
            phaseGameObject.SetActive(index == phase);
        }
    }

    private void SetErrorStatus() {
        StatusText.text = NGIONet.Engine.Comms.ConnectionStatus.ToString();
        SwitchToPhase(4);
    }

    private void SetPlayerNameStatus() {
        SwitchToPhase(3);
        PlayerNameText.text = NGIONet.Engine.Comms.CurrentUser?.Name ?? "Guest";
        LogoutObject.SetActive(ShowLogoutButton && !(ForceSkip));
    }

    private void StatusChange(object sender, ConnectionStatus status) {
        switch (status) {
            case ConnectionStatus.LoginRequired:
            case ConnectionStatus.UserLoggedOut:
                SwitchToPhase(NGIONet.Engine.Comms.LoginPageOpen ? 2 : 1);
                break;
            case ConnectionStatus.Ready:
                OnAllReady?.Invoke();
                SetPlayerNameStatus();
                break;
            case ConnectionStatus.LoginCancelled:
                SwitchToPhase(1);
                break;
            case ConnectionStatus.ServerUnreachable:
            case ConnectionStatus.LoginFailed:
            case ConnectionStatus.ServerUnavailable:
                SetErrorStatus();
                break;
        }
    }

    void Start() {
        SwitchToPhase(0);

    }

    void OnDestroy() {
        _canCommunicate = false;
        NGIONet.Engine.Comms.ConnectionStatusChange -= StatusChange;
    }

    void Update() {
        if (_canCommunicate) return; // do nothing if we can access ngio.net

        if (NGIONet.Engine == null) return;
        _canCommunicate = NGIONet.Engine.Comms != null;

        if (!_canCommunicate) return; // if by this point we can communicate (was false)
        NGIONet.Engine.Comms.Initialize();
        NGIONet.Engine.Comms.StopHeartbeat();
        NGIONet.Engine.Comms.StartHeartbeat();
        NGIONet.Engine.Comms.ConnectionStatusChange += StatusChange;
    }
}
