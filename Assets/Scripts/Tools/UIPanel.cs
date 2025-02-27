using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIPanel : MonoBehaviour
{
    public virtual void ShowUI()
    {
        gameObject.SetActive(true);
    }

    public virtual void HideUI()
    {
        gameObject.SetActive(false);

    }
}
