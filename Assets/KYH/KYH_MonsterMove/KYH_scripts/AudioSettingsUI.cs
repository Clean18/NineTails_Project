using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioSettingsUI : MonoBehaviour
{
    [Header("Mixer Reference")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Sliders")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    private const string BGM_PARAM = "BGM Volume";
    private const string SFX_PARAM = "SFX Volume";

    private const string BGM_PREF = "BGM VolumePref";
    private const string SFX_PREF = "SFX VolumePref";

    private void Start()
    {
        // 1. 저장된 값 불러오기
        float bgm = PlayerPrefs.GetFloat(BGM_PREF, 1f);
        float sfx = PlayerPrefs.GetFloat(SFX_PREF, 1f);

        bgmSlider.value = bgm;
        sfxSlider.value = sfx;

        SetBGMVolume(bgm);
        SetSFXVolume(sfx);

        // 2. 리스너 등록
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetBGMVolume(float value)
    {
        // -80 dB ~ 0 dB 범위로 설정
        float dB = Mathf.Log10(value) * 20f;
        audioMixer.SetFloat(BGM_PARAM, dB);
        PlayerPrefs.SetFloat(BGM_PREF, value);
    }

    public void SetSFXVolume(float value)
    {
        float dB = Mathf.Log10(value) * 20f;
        audioMixer.SetFloat(SFX_PARAM, dB);
        PlayerPrefs.SetFloat(SFX_PREF, value);
    }
}
