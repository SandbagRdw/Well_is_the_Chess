using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessManager : Singleton<ProcessManager>
{
    /// <summary>
    /// ��ǰ���ıߵĻغϣ�-1ΪX�ж���1ΪO�ж�,0Ϊ��Ϸ����
    /// </summary>
    public int CurTurn { get { return cur; } }
    /// <summary>
    /// ����ǰ���ķ���X��O����-1ΪX��1ΪO
    /// </summary>
    private int cur;

    /// <summary>
    /// ����õ�ʲô�壬-1ΪX��1ΪO
    /// </summary>
    public int PlayerType { get { return playerType; } }

    private int playerType;
    /// <summary>
    /// ai�Ѷȣ�1Ϊ�򵥣�2Ϊ�еȣ�3Ϊ����
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
    /// ��ʽ��ʼ��Ϸ
    /// </summary>
    public void StartGame()
    {
        BoardManager.Instance.ResetBoard();//����հ���
        SoundManager.Instance.PlayBgmCrossFade("Game1",false);
        cur = -1;//X����
        boardUI.BoardcastNextTurn(cur);
        isAI = playerType != cur;//������û��ѡX����playerType��Ϊ-1������AI�Ļغ�
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
    /// ����������ӵĽ���������¸��غϣ���������֣�
    /// </summary>
    /// <param name="condition"></param>
    public void NextTurn(int condition)
    {
        if (condition == 2)//��û����
        {
            cur *= -1;//����������
            boardUI.BoardcastNextTurn(cur);
            isAI = !isAI;//�����
            if (isAI)
                SimpleAI.Instance.AIPlayChess(difficulty);
            return;
        }
        else if (condition == 1) //O��ʤ
        {
            boardUI.BoardcastNextTurn(2);
        }
        else if (condition == -1)//X��ʤ 
        {
            boardUI.BoardcastNextTurn(-2);
        }
        else//ƽ��
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
