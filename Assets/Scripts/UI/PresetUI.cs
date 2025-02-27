using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PresetUI : UIBase
{
    [SerializeField] Button startBtn = null;
    [SerializeField] Button backBtn = null;
    [SerializeField] ToggleGroup turnGroup = null;
    [SerializeField] ToggleGroup difficultyGroup = null;

    public override void InitUI()
    {
        base.InitUI();
        if (startBtn != null)
            startBtn.onClick.AddListener(StartWithCurrentSet);
        if (backBtn != null)
            backBtn.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySfx("Click");
                this.HideUI();
                UIManager.Instance.GetUI<MenuUI>().ShowUI();
            });
        Toggle[] toggles = turnGroup.GetComponentsInChildren<Toggle>();
        foreach (Toggle toggle in toggles)
        {
            toggle.onValueChanged.AddListener((b) => { SoundManager.Instance.PlaySfx("Click"); });
        }
        toggles = difficultyGroup.GetComponentsInChildren<Toggle>();
        foreach (Toggle toggle in toggles)
        {
            toggle.onValueChanged.AddListener((b) => { SoundManager.Instance.PlaySfx("Click"); });
        }
        this.HideUI();
    }

    void StartWithCurrentSet()
    {
        SoundManager.Instance.PlaySfx("Click");
        int turn = GetToggleGroupActiveIndex(turnGroup) == 1 ? 1 : -1;//�����ѡ��O������index=1��toggle������1�����䣬���򷵻�-1�����ѡ��X
        int difficulty = Mathf.Max(1, 1 + GetToggleGroupActiveIndex(difficultyGroup));//��max�����ף�������toggle����0-2��+1��Ϊ1-3���Ѷȣ����Ա���Ϊ1�Ѷ�
        ProcessManager.Instance.PresetGame(turn, difficulty);
        this.HideUI();
    }

    public int GetToggleGroupActiveIndex(ToggleGroup group)
    {
        int index = -1;
        if (group == null)
            return index;

        Toggle[] toggles = group.GetComponentsInChildren<Toggle>();
        for (int i = 0; i < toggles.Length; i++)
        {
            if (toggles[i].isOn)
            {
                index = i;
                break;
            }
        }
        return index;
    }
}
