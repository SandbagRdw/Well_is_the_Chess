using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class UIManager : Singleton<UIManager>
{
    Dictionary<string,UIBase> UIDict=new Dictionary<string,UIBase>();
    List<UIBase> UIList=new List<UIBase>();

    [SerializeField] GameObject messageUIPanel;
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);

    }
    public void AddUIBase(UIBase ui)
    {
        if (UIList.Contains(ui))
            return;
        UIList.Add(ui);
        ui.InitUI();
        Debug.Log("Add UIBase:" + ui.GetType());
    }

    public T GetUI<T>()where T : UIBase
    {
        UIBase ui=UIList.Find((ui) => ui.GetType()==typeof(T));
        if (ui == null)
        {
            GameObject go = Instantiate(messageUIPanel);
            go.SetActive(true);           
            ui= go.AddComponent<UIBase>();
        }

        return ui as T;
    }
}
