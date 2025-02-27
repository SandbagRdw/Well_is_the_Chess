using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SettingUIPanel : UIPanel
{
    [SerializeField] Button backBtn=null;
    [SerializeField] Slider bgmSlider=null;
    [SerializeField] Slider sfxSlider=null;
    public void InitUI()
    {
        if (backBtn != null)
        {
            backBtn.onClick.AddListener(BackToMenu);
        }
        if(bgmSlider != null)
        {
            bgmSlider.value = SoundManager.Instance.BGMVolume;
            bgmSlider.onValueChanged.AddListener((v) => { SoundManager.Instance.SetBGMVolume(v); });
        }
        if (sfxSlider != null)
        {
            sfxSlider.value = SoundManager.Instance.SFXVolume;
            sfxSlider.onValueChanged.AddListener((v) => { SoundManager.Instance.SetSFXVolume(v); });
        }
    }

    void BackToMenu()
    {
        SoundManager.Instance.PlaySfx("Click");
        this.HideUI();
    }
}
