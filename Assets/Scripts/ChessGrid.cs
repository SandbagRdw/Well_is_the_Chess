using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChessGrid : MonoBehaviour
{
    [SerializeField] List<GameObject> chessIcons = new();
    [SerializeField] int mType = 0;
    [SerializeField] int mIndex = 0;
    Button mBtn;

    private void Awake()
    {
        mBtn = GetComponent<Button>();
        if (mBtn != null)
            mBtn.onClick.AddListener(PlayerInputOnGrid);
    }
    public void ResetGrid(int index)
    {
        mType = 0;
        mIndex = index;
        mBtn.interactable = true;
        foreach (GameObject icon in chessIcons)
        {
            PoolManager.Instance.ReturnGameObject(icon);
        }
        chessIcons.Clear();
    }

    public void SetGridType(int type)
    {
        mType = type;
        mBtn.interactable = false;
        GameObject go=null;
        if (type == 1)
        {
            go=PoolManager.Instance.GetGameObject("ChessO");
        }else if (type == -1)
        {
            go = PoolManager.Instance.GetGameObject("ChessX");
        }
        if (go!=null)
        {
            chessIcons.Add(go);
            go.transform.SetParent(transform, false);
            go.transform.localPosition= Vector3.zero;
            go.GetComponent<Animation>()?.Play();
        }
        
    }

    void PlayerInputOnGrid()
    {
        if (ProcessManager.Instance.IsAITurn || mType != 0)
            return;
        BoardManager.Instance.PlayChess(mIndex);
        
    }
    private void OnMouseDown()
    {
        Debug.Log("Click grid index:" + mIndex);
        
    }
    private void OnMouseEnter()
    {
        Debug.Log("Enter grid index:" + mIndex);
    }
}
