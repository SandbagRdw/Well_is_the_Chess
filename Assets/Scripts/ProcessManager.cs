using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessManager : Singleton<ProcessManager>
{
    /// <summary>
    /// 当前是哪边的回合，-1为X行动，1为O行动,0为游戏结束
    /// </summary>
    public int CurTurn { get { return cur; } }
    /// <summary>
    /// 处理当前是哪方（X或O），-1为X，1为O
    /// </summary>
    private int cur;

    /// <summary>
    /// 玩家用的什么棋，-1为X，1为O
    /// </summary>
    public int PlayerType { get { return playerType; } }

    private int playerType;
    /// <summary>
    /// ai难度，1为简单，2为中等，3为困难
    /// </summary>
    private int difficulty=0;

    public bool IsAITurn { get { return isAI; } }

    private bool isAI;

    BoardUI boardUI;

    Coroutine lightRainCoroutine=null;
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        UIManager.Instance.GetUI<MenuUI>().ShowUI();
    }
    /// <summary>
    /// 正式开始游戏
    /// </summary>
    public void StartGame()
    {
        BoardManager.Instance.ResetBoard();//先清空版面
        SoundManager.Instance.PlayBgmCrossFade("Game1",false);
        cur = -1;//X先行
        boardUI.BoardcastNextTurn(cur);
        isAI = playerType != cur;//如果玩家没有选X，即playerType不为-1，则是AI的回合
        if (isAI)
            SimpleAI.Instance.AIPlayChess(difficulty);
        lightRainCoroutine = StartCoroutine(LightRain());
    }

    public void PresetGame(int playerSide,int AIDifficulty)
    {
        boardUI = UIManager.Instance.GetUI<BoardUI>();
        if (!boardUI)
        {
            Debug.LogError("Can`t find board object!");
            return;
        }
        boardUI.ShowUI();
        SoundManager.Instance.PlayBgmWithBg("Game1","Game2");
        playerType = playerSide;
        difficulty = AIDifficulty;
        StartGame();
    }

    /// <summary>
    /// 根据这次落子的结果，处理下个回合（或结束本局）
    /// </summary>
    /// <param name="condition"></param>
    public void NextTurn(int condition)
    {
        if (condition == 2)//还没结束
        {
            cur *= -1;//换棋子类型
            boardUI.BoardcastNextTurn(cur);
            isAI = !isAI;//换玩家
            if (isAI)
                SimpleAI.Instance.AIPlayChess(difficulty);
            return;
        }
        else if (condition == 1) //O获胜
        {
            boardUI.BoardcastNextTurn(2);
        }
        else if (condition == -1)//X获胜 
        {
            boardUI.BoardcastNextTurn(-2);
        }
        else//平局
        {
            boardUI.BoardcastNextTurn(0);
        }
        cur = 0;
        SoundManager.Instance.PlayBgmCrossFade("Game2", false);
    }

    IEnumerator LightRain()
    {
        float nextTime;
        while (cur!=0)
        {
            nextTime = Random.Range(0.2f, 1.5f);
            yield return new WaitForSeconds(nextTime);
            GameObject go=PoolManager.Instance.GetGameObject("Prefabs/LightEfx");
            go.transform.SetParent(UIManager.Instance.transform.Find("UIEfxRoot"), false);
            float range = Random.Range(-920f, 920f);
            go.transform.localPosition = new Vector3(range, 640f, 0);
            go.GetComponent<LightEfx>()?.KeepSmall(Mathf.Abs(range) < 320f);
        }
    }
}
