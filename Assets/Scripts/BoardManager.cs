using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BoardManager : Singleton<BoardManager>
{
    /// <summary>
    /// ���ӵ����ͣ��ո���Ϊ0��XΪ-1��OΪ1
    /// </summary>
    public int[] gridTypes = new int[9];

    /// <summary>
    /// aiҪ������һ����������������
    /// </summary>
    public int LastIndex { get { return lastIndex; } }
    int lastIndex=-1;
    /// <summary>
    /// �����࣬������Ҳ�����
    /// </summary>
    [SerializeField] List<ChessGrid> grids = new();
    /// <summary>
    /// ���ܶѵ��Ķ�����Ч��
    /// </summary>
    List<GameObject> winAnims = new();

    protected override void Awake()
    {
        base.Awake();
        if (grids == null || grids.Count == 0)
        {
            grids.AddRange(transform.GetComponentsInChildren<ChessGrid>());

        }
    }

    /// <summary>
    /// ��ʼ��Ϸ��������Ϸʱ��������,������з��ü�¼�ͱ�����
    /// </summary>
    public void ResetBoard()
    {
        foreach (GameObject go in winAnims)
        {
            PoolManager.Instance.ReturnGameObject(go);
        }
        winAnims.Clear();
        for (int i = 0; i < 9; i++)
        {
            gridTypes[i] = 0;
            grids[i].ResetGrid(i);
        }
        lastIndex = -1;
    }
    /// <summary>
    /// ���������壬�ѵ�ǰ�غϣ���Ӫ�������ӣ�-1��1������ָ����
    /// </summary>
    /// <param name="index">����index</param>
    public void PlayChess(int index)
    {
        if (gridTypes[index] != 0)
        {
            Debug.LogError("BUG: Want to play chess on a played grid(index): "+index);
            return;
        }
        if (ProcessManager.Instance.IsAITurn)
            Debug.Log("AI played a " + (ProcessManager.Instance.CurTurn == 1 ? "O" : "X") + " on " + index);
        else
            Debug.Log("Player played a " + (ProcessManager.Instance.CurTurn == 1 ? "O" : "X") + " on " + index);
        int thisChessType = ProcessManager.Instance.CurTurn;
        if (thisChessType == 1)
        {
            SoundManager.Instance.PlaySfx("PaintO");
        }
        else if (thisChessType == -1)
        {
            SoundManager.Instance.PlaySfx("PaintX");
        }
        gridTypes[index] = thisChessType;
        grids[index].SetGridType(thisChessType);
        lastIndex = index;
        ProcessManager.Instance.NextTurn(CheckGameEnd());
    }
    /// <summary>
    /// ���ȡһ���ո��ӵ�index
    /// </summary>
    /// <returns></returns>
    public int GetRandomAvailableGrid()
    {
        List<int> availableIndex = new List<int>();
        for (int i = 0; i < gridTypes.Length; i++)
        {
            if (gridTypes[i] == 0)
                availableIndex.Add(i);
        }
        if (availableIndex.Count == 0)
            return -1;
        return availableIndex[Random.Range(0, availableIndex.Count)];
    }
    public int CheckGameEnd()
    {
        int win = 0;//�п��ܳ���һ�����ɶ��3������������Է�����if�����3�����
        if (gridTypes[0] != 0 && gridTypes[0] == gridTypes[1] && gridTypes[1] == gridTypes[2])//��1������           
        {
            ShowRowWinAnim(1, false);
            win= gridTypes[0];
        }
        else if (gridTypes[3] != 0 && gridTypes[3] == gridTypes[4] && gridTypes[4] == gridTypes[5])//��2������
        {
            ShowRowWinAnim(4, false);
            win = gridTypes[3];
        }
        else if (gridTypes[6] != 0 && gridTypes[6] == gridTypes[7] && gridTypes[7] == gridTypes[8])//��3������
        {
            ShowRowWinAnim(7, false);
            win = gridTypes[6];
        }

        if (gridTypes[0] != 0 && gridTypes[0] == gridTypes[3] && gridTypes[3] == gridTypes[6])//��1������
        {
            ShowRowWinAnim(3, true);
            win = gridTypes[0];
        }
        else if (gridTypes[1] != 0 && gridTypes[1] == gridTypes[4] && gridTypes[4] == gridTypes[7])//��2������
        {
            ShowRowWinAnim(4, true);
            win = gridTypes[1];
        }
        else if (gridTypes[2] != 0 && gridTypes[2] == gridTypes[5] && gridTypes[5] == gridTypes[8])//��3������
        {
            ShowRowWinAnim(5, true);
            win = gridTypes[2];
        }
        if (gridTypes[0] != 0 && gridTypes[0] == gridTypes[4] && gridTypes[4] == gridTypes[8])//���ϵ���������
        {
            ShowCrossWinAnim(false);
            win = gridTypes[0];
        }
        else if (gridTypes[2] != 0 && gridTypes[2] == gridTypes[4] && gridTypes[4] == gridTypes[6])//���ϵ���������
        {
            ShowCrossWinAnim(true);
            win = gridTypes[2];
        }
        if(win!=0)//��������ͬһ�غϵģ�win���Ը��ǣ�ֻҪ��Ϊ0����Ӯ�ˣ����ؼ���
        {
            SoundManager.Instance.PlaySfx("Win");
            return win;
        }
            

        for(int i = 0; i < gridTypes.Length; i++)
        {
            if (gridTypes[i] == 0)//���пյģ�û����
                return 2;
        }
        return 0;//ƽ��


        //else if (gridTypes[0] != 0 && gridTypes[1] != 0 && gridTypes[2] != 0 && gridTypes[3] != 0 && gridTypes[4] != 0 && gridTypes[5] != 0 && gridTypes[6] != 0 && gridTypes[7] != 0 && gridTypes[8] != 0)
        //{
        //    return 0;//ƽ��
        //}

        //return 2;
    }
    /// <summary>
    /// Ѱ�Ҵ��뷽��û��1����ʤ��λ��
    /// </summary>
    /// <param name="turn"></param>
    /// /// <returns></returns>
    public int FindOneMoveWin(int turn)
    {
        if (gridTypes[0] == 0 && ((gridTypes[1] == turn && gridTypes[2] == turn) || (gridTypes[3] == turn && gridTypes[6] == turn) || (gridTypes[4] == turn && gridTypes[8] == turn)))//����3��
            return 0;
        else if (gridTypes[1] == 0 && ((gridTypes[0] == turn && gridTypes[2] == turn) || (gridTypes[4] == turn && gridTypes[7] == turn)))//��������
            return 1;
        else if (gridTypes[2] == 0 && ((gridTypes[0] == turn && gridTypes[1] == turn) || (gridTypes[5] == turn && gridTypes[8] == turn) || (gridTypes[4] == turn && gridTypes[6] == turn)))
            return 2;
        else if (gridTypes[3] == 0 && ((gridTypes[0] == turn && gridTypes[6] == turn) || (gridTypes[4] == turn && gridTypes[5] == turn)))
            return 3;
        else if (gridTypes[4] == 0 && ((gridTypes[0] == turn && gridTypes[8] == turn) || (gridTypes[2] == turn && gridTypes[6] == turn) 
            || (gridTypes[1] == turn && gridTypes[7] == turn) || (gridTypes[3] == turn && gridTypes[5] == turn)))//�м�������湲4��
            return 4;
        else if (gridTypes[5] == 0 && ((gridTypes[2] == turn && gridTypes[8] == turn) || (gridTypes[3] == turn && gridTypes[4] == turn)))
            return 5;
        else if (gridTypes[6] == 0 && ((gridTypes[0] == turn && gridTypes[3] == turn) || (gridTypes[7] == turn && gridTypes[8] == turn) || (gridTypes[2] == turn && gridTypes[4] == turn)))
            return 6;
        else if (gridTypes[7] == 0 && ((gridTypes[1] == turn && gridTypes[4] == turn) || (gridTypes[6] == turn && gridTypes[8] == turn)))
            return 7;
        else if (gridTypes[8] == 0 && ((gridTypes[2] == turn && gridTypes[5] == turn) || (gridTypes[6] == turn && gridTypes[7] == turn) || (gridTypes[0] == turn && gridTypes[4] == turn)))
            return 8;
        else
            return -1;
    }

    /// <summary>
    /// б�������Ķ���
    /// </summary>
    /// <param name="left">��������Ļ���</param>
    void ShowCrossWinAnim(bool left)
    {
        GameObject go = PoolManager.Instance.GetGameObject("Prefabs/CrossWin");
        if (go==null)
        {
            return;
        }
        go.transform.SetParent(transform.parent);
        go.transform.position = grids[4].transform.position;
              
        if (left)
        {
            go.transform.localEulerAngles = new Vector3(0,0,-90);
        }
        else
        {
            go.transform.localEulerAngles = Vector3.zero;
        }
        go.GetComponent<Animation>()?.Play();
        winAnims.Add(go);
    }
    void ShowRowWinAnim(int index, bool col)
    {
        GameObject go = PoolManager.Instance.GetGameObject("Prefabs/RowWin");
        if (go == null)
        {
            return;
        }
        go.transform.SetParent(transform.parent);
        go.transform.position = grids[index].transform.position;
        if (col)
        {
            go.transform.localEulerAngles = new Vector3(0, 0, -90);
        }
        else
        {
            go.transform.localEulerAngles = Vector3.zero;
        }
        go.GetComponent<Animation>()?.Play();
        winAnims.Add(go);
    }
}
