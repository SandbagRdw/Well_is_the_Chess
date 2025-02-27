using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BoardManager : Singleton<BoardManager>
{
    /// <summary>
    /// 格子的类型，空格子为0，X为-1，O为1
    /// </summary>
    public int[] gridTypes = new int[9];

    /// <summary>
    /// ai要根据上一步棋在哪里决定落点
    /// </summary>
    public int LastIndex { get { return lastIndex; } }
    int lastIndex=-1;
    /// <summary>
    /// 格子类，接收玩家操作用
    /// </summary>
    [SerializeField] List<ChessGrid> grids = new();
    /// <summary>
    /// 可能堆叠的动画特效等
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
    /// 开始游戏和重启游戏时，都调用,清空所有放置记录和表现物
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
    /// 真正的下棋，把当前回合（阵营）的棋子：-1或1，放在指定格
    /// </summary>
    /// <param name="index">格子index</param>
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
    /// 随机取一个空格子的index
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
        int win = 0;//有可能出现一次连成多个3连的情况，所以分三次if，检测3种情况
        if (gridTypes[0] != 0 && gridTypes[0] == gridTypes[1] && gridTypes[1] == gridTypes[2])//第1行三连           
        {
            ShowRowWinAnim(1, false);
            win= gridTypes[0];
        }
        else if (gridTypes[3] != 0 && gridTypes[3] == gridTypes[4] && gridTypes[4] == gridTypes[5])//第2行三连
        {
            ShowRowWinAnim(4, false);
            win = gridTypes[3];
        }
        else if (gridTypes[6] != 0 && gridTypes[6] == gridTypes[7] && gridTypes[7] == gridTypes[8])//第3行三连
        {
            ShowRowWinAnim(7, false);
            win = gridTypes[6];
        }

        if (gridTypes[0] != 0 && gridTypes[0] == gridTypes[3] && gridTypes[3] == gridTypes[6])//第1列三连
        {
            ShowRowWinAnim(3, true);
            win = gridTypes[0];
        }
        else if (gridTypes[1] != 0 && gridTypes[1] == gridTypes[4] && gridTypes[4] == gridTypes[7])//第2列三连
        {
            ShowRowWinAnim(4, true);
            win = gridTypes[1];
        }
        else if (gridTypes[2] != 0 && gridTypes[2] == gridTypes[5] && gridTypes[5] == gridTypes[8])//第3列三连
        {
            ShowRowWinAnim(5, true);
            win = gridTypes[2];
        }
        if (gridTypes[0] != 0 && gridTypes[0] == gridTypes[4] && gridTypes[4] == gridTypes[8])//左上到右下三连
        {
            ShowCrossWinAnim(false);
            win = gridTypes[0];
        }
        else if (gridTypes[2] != 0 && gridTypes[2] == gridTypes[4] && gridTypes[4] == gridTypes[6])//右上到左下三连
        {
            ShowCrossWinAnim(true);
            win = gridTypes[2];
        }
        if(win!=0)//反正都是同一回合的，win可以覆盖，只要不为0就是赢了，返回即可
        {
            SoundManager.Instance.PlaySfx("Win");
            return win;
        }
            

        for(int i = 0; i < gridTypes.Length; i++)
        {
            if (gridTypes[i] == 0)//还有空的，没结束
                return 2;
        }
        return 0;//平局


        //else if (gridTypes[0] != 0 && gridTypes[1] != 0 && gridTypes[2] != 0 && gridTypes[3] != 0 && gridTypes[4] != 0 && gridTypes[5] != 0 && gridTypes[6] != 0 && gridTypes[7] != 0 && gridTypes[8] != 0)
        //{
        //    return 0;//平局
        //}

        //return 2;
    }
    /// <summary>
    /// 寻找传入方有没有1步获胜的位置
    /// </summary>
    /// <param name="turn"></param>
    /// /// <returns></returns>
    public int FindOneMoveWin(int turn)
    {
        if (gridTypes[0] == 0 && ((gridTypes[1] == turn && gridTypes[2] == turn) || (gridTypes[3] == turn && gridTypes[6] == turn) || (gridTypes[4] == turn && gridTypes[8] == turn)))//角有3种
            return 0;
        else if (gridTypes[1] == 0 && ((gridTypes[0] == turn && gridTypes[2] == turn) || (gridTypes[4] == turn && gridTypes[7] == turn)))//边有两种
            return 1;
        else if (gridTypes[2] == 0 && ((gridTypes[0] == turn && gridTypes[1] == turn) || (gridTypes[5] == turn && gridTypes[8] == turn) || (gridTypes[4] == turn && gridTypes[6] == turn)))
            return 2;
        else if (gridTypes[3] == 0 && ((gridTypes[0] == turn && gridTypes[6] == turn) || (gridTypes[4] == turn && gridTypes[5] == turn)))
            return 3;
        else if (gridTypes[4] == 0 && ((gridTypes[0] == turn && gridTypes[8] == turn) || (gridTypes[2] == turn && gridTypes[6] == turn) 
            || (gridTypes[1] == turn && gridTypes[7] == turn) || (gridTypes[3] == turn && gridTypes[5] == turn)))//中间横竖交叉共4种
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
    /// 斜向三连的动画
    /// </summary>
    /// <param name="left">从右往左的划线</param>
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
