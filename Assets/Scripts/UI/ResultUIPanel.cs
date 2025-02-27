using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultUIPanel : UIPanel
{
    [SerializeField] Button quickRestartBtn = null;
    [SerializeField] Button toPresetBtn = null;
    [SerializeField] Button backToMenuBtn = null;

    public void InitUI()
    {
        if (quickRestartBtn != null)
        {
            quickRestartBtn.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySfx("Click");
                ProcessManager.Instance.StartGame();
                this.HideUI();
            });
        }

        if (toPresetBtn != null)
        {
            toPresetBtn.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySfx("Click");
                UIManager.Instance.GetUI<BoardUI>().HideUI();
                UIManager.Instance.GetUI<PresetUI>().ShowUI();
            });
        }

        if (backToMenuBtn != null)
        {
            backToMenuBtn.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySfx("Click");
                UIManager.Instance.GetUI<BoardUI>().HideUI();
                UIManager.Instance.GetUI<MenuUI>().ShowUI();
            });
        }
    }
    public override void ShowUI()
    {
        base.ShowUI();
        transform.localPosition= new Vector3(800f, transform.localPosition.y, transform.localPosition.z);
        Tween tween = transform.DOLocalMoveX(0, 0.8f);
    }
    //public override void HideUI()
    //{
    //    Tween tween = transform.DOMoveX(800, 1.2f);
    //    //tween.onComplete += () => { base.HideUI(); };
    //}
    public void AnimHideUI()
    {
        Tween tween = transform.DOMoveX(800, 1.2f);
        tween.onComplete += () => { this.HideUI(); };
    }
}
