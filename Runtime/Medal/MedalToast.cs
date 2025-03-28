using System.Collections;
using System.Collections.Generic;
using NewgroundsIODotNet.DataModels;
using NewgroundsIODotNet.Unity;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class MedalToast : MonoBehaviour {
    [SerializeField]
    private TextMeshProUGUI TextPoints;
    [SerializeField]
    private TextMeshProUGUI TextMedalName;
    [SerializeField]
    private Image MedalPicture;
    [SerializeField]
    private string PointsTemplate = "{0}pts";

    [SerializeField] private AudioSource AudioEntrance;
    [SerializeField] private Animator MyAnimator;

    [SerializeField] private AudioSource AudioExit;

    private MedalSpriteCollection _coll;

    public void DisplayMedalUnlock(Medal medal) {
        TextPoints.text = string.Format(PointsTemplate, medal.Value);
        TextMedalName.text = medal.Name;
        if (_coll != null) {
            MedalPicture.sprite = _coll.GetMedalSprite(medal);
        }
        MyAnimator.Play("Unlock");
    }
    public void PlayAudioIn(AnimationEvent evt) {
        AudioEntrance.Play();
    }

    public void PlayAudioExit(AnimationEvent evt) {
        AudioExit.Play();
    }
}
