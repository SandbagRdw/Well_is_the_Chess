using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
public class BoardUI : UIBase
{
    [SerializeField] TMP_Text text = null;
    [SerializeField] Image label = null;
    [SerializeField]ResultUIPanel resultPanel = null;
    [SerializeField] Button backBtn = null;
    public override void InitUI()
    {
        base.InitUI();
        if(resultPanel!=null) 
            resultPanel.InitUI();
        if (backBtn != null)
            backBtn.onClick.AddListener(() => {
                SoundManager.Instance.PlaySfx("Click");
                this.HideUI();
                UIManager.Instance.GetUI<MenuUI>().ShowUI();
            });
        this.HideUI();        
    }
    public override void ShowUI()
    {
        base.ShowUI();
        if (resultPanel != null)
            resultPanel.HideUI();
        transform.localScale = Vector3.zero;
        Tween tween=transform.DOScale(1, 0.6f);
        tween.onComplete+=()=> backBtn.gameObject.SetActive(true);
    }

    public override void HideUI()
    {
        if(backBtn != null)
            backBtn.gameObject.SetActive(false);
        if (resultPanel != null)
            resultPanel.HideUI();
        //transform.localScale = Vector3.one;
        Tween tween= transform.DOScale(0, 0.4f);
        tween.onComplete += () => { base.HideUI(); };
    }

    public void BoardcastNextTurn(int turn)
    {
        switch (turn)
        {
            case 0:
                text.text = "平局!!!!!";
                break;
            case 1:
                text.text = "O方回合！";
                break;
            case -1:
                text.text = "X方回合！";
                break;
            case 2:
                text.text = "O方胜利！";
                break;
            case -2:
                text.text = "X方胜利！";
                break;
            default:
                text.text = "";
                break;
        }
        if (turn == 0 || turn == 2 || turn == -2)
            if (resultPanel != null)
                resultPanel.ShowUI();
    }
}
