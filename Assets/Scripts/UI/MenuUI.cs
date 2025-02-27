using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : UIBase
{
    [SerializeField] SettingUIPanel settingUI = null;
    [SerializeField] Button startGameBtn = null;
    [SerializeField] Button quitGameBtn = null;
    [SerializeField] Button settingBtn = null;
    public override void InitUI()
    {
        base.InitUI();
        if (settingUI != null)
        {
            settingUI.InitUI();
        }
        if (startGameBtn != null)
            startGameBtn.onClick.AddListener(StartPreset);

        if (quitGameBtn != null)
            quitGameBtn.onClick.AddListener(QuitGame);

        if (settingBtn != null)
            settingBtn.onClick.AddListener(ShowSettingUI);
    }

    public override void ShowUI()
    {
        base.ShowUI();
        if (settingUI != null)
        {
            settingUI.HideUI();
        }
        ProcessManager.Instance.StopAllCoroutines();
        SoundManager.Instance.PlayBgmCrossFade("Menu");
    }
    /// <summary>
    /// 显示设置UI（直接盖在菜单上）
    /// </summary>
    public void ShowSettingUI()
    {
        SoundManager.Instance.PlaySfx("Click");
        if (settingUI!=null)
            settingUI.ShowUI();
    }
    public void StartPreset()
    {
        SoundManager.Instance.PlaySfx("Click");
        UIManager.Instance.GetUI<PresetUI>().ShowUI();
        this.HideUI();
    }
    public void QuitGame()
    {
        SoundManager.Instance.PlaySfx("Click");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
