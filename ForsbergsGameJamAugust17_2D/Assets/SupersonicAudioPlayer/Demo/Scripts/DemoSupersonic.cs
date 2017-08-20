using Supersonic;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DemoSupersonic : MonoBehaviour
{
    #region Fields/Properties

    [Header("Tracks"), Space(10)]
    public Track PasswordTrack;
    public Track BossTrack;

    [Space(10)]

    [Header("Temporary Tracks"), Space(10)]    
    public Track IntroTrack;

    [Space(10)]

    [Header("Pause Tracks"), Space(10)]
    public Track PauseTrack;

    [Space(10)]

    [Header("Sound Effects"), Space(10)]
    public SoundEffect LaserSoundEffect;
    public SoundEffect JumpSoundEffect;

    private List<Button> _allButtons;
    private Text _pauseGameButtonText;
    private Text _pauseGameSilentlyButtonText;

    #endregion
    #region Events

    private void Awake()
    {
        _allButtons = GetComponentsInChildren<Button>().ToList();
        
        _pauseGameButtonText = GameObject.Find("PauseGame").GetComponent<Button>().GetComponentInChildren<Text>();        
        _pauseGameSilentlyButtonText = GameObject.Find("PauseGameSilently").GetComponentInChildren<Button>().GetComponentInChildren<Text>();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            PauseGameSilently();
        }
    }

    private void Start()
	{
        Supersonic.AudioPlayer.Instance.PlayTrack(PasswordTrack, true, false);
    }

    #endregion
    #region Tracks

    public void PlayBossTrack()
    {
        AudioPlayer.Instance.PlayTrack(BossTrack, true, false);
    }

    public void PlayPasswordTrack()
    {
        AudioPlayer.Instance.PlayTrack(PasswordTrack, true, false);
    }

    #endregion
    #region Temporary Tracks

    public void PlayIntroTrack()
    {
        AudioPlayer.Instance.PlayTemporaryTrack(IntroTrack, false, destroySoundEffects: true, muteSoundEffects: false, destroySoundEffectsWhenDone: true);
    }

    #endregion
    #region Pause Tracks

    public void PauseGameSilently()
    {
        Time.timeScale = Convert.ToInt16(!Convert.ToBoolean(Time.timeScale));

        if (Time.timeScale == 0)
        {
            AudioPlayer.Instance.PlayPauseSilently();

            _pauseGameSilentlyButtonText.text = "Resume Game";            
            //ToggleButtons(false);
        }
        else
        {
            AudioPlayer.Instance.StopPauseSilently();

            _pauseGameSilentlyButtonText.text = "Pause Game Silently";            
            //ToggleButtons(true);
        }
    }

    public void PauseGame()
    {
        Time.timeScale = Convert.ToInt16(!Convert.ToBoolean(Time.timeScale));

        if (Time.timeScale == 0)
        {
            AudioPlayer.Instance.PlayPauseTrack(PauseTrack);

            _pauseGameButtonText.text = "Resume Game";
            ToggleButtons(false);
        }
        else
        {
            AudioPlayer.Instance.StopPauseTrack();

            _pauseGameButtonText.text = "Pause Game Track";
            ToggleButtons(true);
        }
    }

    #endregion    
    #region Sound Effects

    public void PlayLaserSoundEffect()
    {
        AudioPlayer.Instance.PlaySoundEffect3D(LaserSoundEffect, Vector3.zero);
    }

    public void PlayJumpSoundEffect()
    {
        AudioPlayer.Instance.PlaySoundEffect2D(JumpSoundEffect);
    }

    public void DestroySoundEffects()
    {
        AudioPlayer.Instance.DestroySoundEffects();
    }

    #endregion

    #region Helpers

    private void ToggleButtons(bool enabled)
    {
        foreach (var button in _allButtons.Where(x => x.name != EventSystem.current.currentSelectedGameObject.name && x.tag != "SoundEffect"))
        {
            button.interactable = enabled;
        }
    }

    #endregion
}
