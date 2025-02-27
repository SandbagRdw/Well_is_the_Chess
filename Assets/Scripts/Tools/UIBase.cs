using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIBase : MonoBehaviour
{
    CanvasGroup canvasGroup;
    protected void Awake()
    {
        UIManager.Instance.AddUIBase(this);
    }
    public virtual void InitUI()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public virtual void ShowUI()
    {
        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);
        if (canvasGroup != null)// && canvasGroup.alpha == 0
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }
    public virtual void HideUI()
    {
        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(false);
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
}
