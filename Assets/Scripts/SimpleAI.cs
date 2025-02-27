using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAI : Singleton<SimpleAI>
{
    [SerializeField][Tooltip("简单AI假装思考时间")] float easyAIThinkingTime=0.5f;
    [SerializeField][Tooltip("中等AI假装思考时间")] float mediumAIThinkingTime = 0.75f;
    [SerializeField][Tooltip("困难AI假装思考时间")] float hardAIThinkingTime = 1f;
    int mType = 0;
    /// <summary>
    /// 轮到AI，按难度决定下一手棋
    /// </summary>
    /// <param name="difficulty"></param>
    public void AIPlayChess(int difficulty)
    {
        mType = ProcessManager.Instance.CurTurn;
        switch (difficulty)
        {
            case 1:
                StartCoroutine(EasyPlay());
                break;
            case 2:
                StartCoroutine(MediumPlay());
                break;
            case 3:
                StartCoroutine(HardPlay());
                break;
            default:

                break;
        }
    }
    /// <summary>
    /// 简单ai就是大傻子，思考时间也短
    /// </summary>
    /// <returns></returns>
    IEnumerator EasyPlay()
    {
        yield return new WaitForSeconds(easyAIThinkingTime);
        BoardManager.Instance.PlayChess(BoardManager.Instance.GetRandomAvailableGrid());
    }

    /// <summary>
    /// 中级ai能发现胜利点位，也能阻止玩家胜利
    /// </summary>
    /// <returns></returns>
    IEnumerator MediumPlay()
    {
        yield return new WaitForSeconds(mediumAIThinkingTime);
        if (BoardManager.Instance.LastIndex == -1)//开局空棋盘时，随机1个位置放置
        {
            BoardManager.Instance.PlayChess(BoardManager.Instance.GetRandomAvailableGrid());
            yield break;
        }

        int mPlay = BoardManager.Instance.FindOneMoveWin(mType);//检测自己下在哪一格能胜利
        if (mPlay >= 0)
        {
            BoardManager.Instance.PlayChess(mPlay);
            yield break;
        }

        mPlay = BoardManager.Instance.FindOneMoveWin(mType * (-1)); //检测对手胜利需要下在哪一格，封对手的路
        if (mPlay < 0)
        {
            mPlay = BoardManager.Instance.GetRandomAvailableGrid(); //如果对手无法一步胜利，随机选可用格子
        }
        BoardManager.Instance.PlayChess(mPlay);

    }

    IEnumerator HardPlay()
    {
        yield return new WaitForSeconds(hardAIThinkingTime);
        if (BoardManager.Instance.LastIndex == -1)//开局放中间
        {
            BoardManager.Instance.PlayChess(4);
            yield break;
        }
        int eType = mType * (-1);
        int mPlay = BoardManager.Instance.FindOneMoveWin(mType);//检测自己下在哪一格能胜利
        if (mPlay >= 0)
        {
            BoardManager.Instance.PlayChess(mPlay);
            yield break;
        }

        mPlay = BoardManager.Instance.FindOneMoveWin(eType); //检测对手胜利需要下在哪一格，封对手的路
        if (mPlay >= 0)
        {
            BoardManager.Instance.PlayChess(mPlay);
            yield break;
        }
        int lastIndex= BoardManager.Instance.LastIndex;
        //经检测，这一手没有能使任意一方胜利的格子，要进行推理

        //上一手在中间格子
        if (lastIndex == 4)
        {
            //占领四角
            if (BoardManager.Instance.gridTypes[0] == 0)
            {
                mPlay = 0;
            }
            else if (BoardManager.Instance.gridTypes[2] == 0)
            {
                mPlay = 2;
            }
            else if (BoardManager.Instance.gridTypes[6] == 0)
            {
                mPlay = 6;
            }
            else if (BoardManager.Instance.gridTypes[8] == 0)
            {
                mPlay = 8;
            }
            else 
                mPlay = BoardManager.Instance.GetRandomAvailableGrid(); //中间和四角都没了，随机选1边
        }
        //上一手对手放在四角之一
        else if (lastIndex == 0 || lastIndex == 2 || lastIndex == 6 || lastIndex == 8)
        {
            //中间如果空的，必定先占领中间
            if (BoardManager.Instance.gridTypes[4] == 0)
            {
                mPlay = 4;
            }
            //对手占1角，对角为空，优先占领对角
            else if (BoardManager.Instance.gridTypes[0] == 0 && BoardManager.Instance.gridTypes[8] == eType)
            {
                mPlay = 0;
            }
            else if (BoardManager.Instance.gridTypes[2] == 0 && BoardManager.Instance.gridTypes[6] == eType)
            {
                mPlay = 2;
            }
            else if (BoardManager.Instance.gridTypes[6] == 0 && BoardManager.Instance.gridTypes[2] == eType)
            {
                mPlay = 6;
            }
            else if (BoardManager.Instance.gridTypes[8] == 0 && BoardManager.Instance.gridTypes[0] == eType)
            {
                mPlay = 8;
            }
            //如果两个对角都被对手占领
            else if ((BoardManager.Instance.gridTypes[0] == eType && BoardManager.Instance.gridTypes[8] == eType) 
                || (BoardManager.Instance.gridTypes[2] == eType && BoardManager.Instance.gridTypes[6] == eType))
            {
                //抢1边，此时抢1角必输，对手将3角2边双赢
                if (BoardManager.Instance.gridTypes[1] == 0)
                {
                    mPlay = 1;
                }
                else if (BoardManager.Instance.gridTypes[3] == 0)
                {
                    mPlay = 3;
                }
                else if (BoardManager.Instance.gridTypes[5] == 0)
                {
                    mPlay = 5;
                }
                else if (BoardManager.Instance.gridTypes[7] == 0)
                {
                    mPlay = 7;
                }
            }
            //对手占领角的对角是自己，则自己占左右角
            else if (BoardManager.Instance.gridTypes[0] == 0)
            {
                mPlay = 0;
            }
            else if (BoardManager.Instance.gridTypes[2] == 0)
            {
                mPlay = 2;
            }
            else if (BoardManager.Instance.gridTypes[6] == 0)
            {
                mPlay = 6;
            }
            else if (BoardManager.Instance.gridTypes[8] == 0)
            {
                mPlay = 8;
            }
            else mPlay =  BoardManager.Instance.GetRandomAvailableGrid();//没角了，随便选一边
        }
        //上一手对手放在四边之一
        else if (lastIndex == 1 || lastIndex == 3 || lastIndex == 5 || lastIndex == 7)
        {
            //先占领中间
            if (BoardManager.Instance.gridTypes[4] == 0)
            {
                mPlay = 4;
            }
            //若对手占了2个相邻边，而夹角空着，优先占夹角，防止1子2连
            else if (BoardManager.Instance.gridTypes[0] == 0 && BoardManager.Instance.gridTypes[1] == eType && BoardManager.Instance.gridTypes[3] == eType)
            {
                mPlay = 0;
            }
            else if (BoardManager.Instance.gridTypes[2] == 0 && BoardManager.Instance.gridTypes[1] == eType && BoardManager.Instance.gridTypes[5] == eType)
            {
                mPlay = 2;
            }
            else if (BoardManager.Instance.gridTypes[6] == 0 && BoardManager.Instance.gridTypes[3] == eType && BoardManager.Instance.gridTypes[7] == eType)
            {
                mPlay = 6;
            }
            else if (BoardManager.Instance.gridTypes[8] == 0 && BoardManager.Instance.gridTypes[5] == eType && BoardManager.Instance.gridTypes[7] == eType)
            {
                mPlay = 8;
            }
            //若对手没有两个相临边，占领对手边旁边的角
            else if (BoardManager.Instance.gridTypes[0] == 0 && ((BoardManager.Instance.gridTypes[1] == eType) || (BoardManager.Instance.gridTypes[3] == eType)))
            {
                mPlay = 0;
            }
            else if (BoardManager.Instance.gridTypes[2] == 0 && ((BoardManager.Instance.gridTypes[1] == eType) || (BoardManager.Instance.gridTypes[5] == eType)))
            {
                mPlay = 2;
            }
            else if (BoardManager.Instance.gridTypes[6] == 0 && ((BoardManager.Instance.gridTypes[3] == eType) || (BoardManager.Instance.gridTypes[7] == eType)))
            {
                mPlay = 6;
            }
            else if (BoardManager.Instance.gridTypes[8] == 0 && ((BoardManager.Instance.gridTypes[5] == eType) || (BoardManager.Instance.gridTypes[7] == eType)))
            {
                mPlay = 8;
            }
            //上述情况都不满足时，先占角
            else if (BoardManager.Instance.gridTypes[0] == 0)
            {
                mPlay = 0;
            }
            else if (BoardManager.Instance.gridTypes[2] == 0)
            {
                mPlay = 2;
            }
            else if (BoardManager.Instance.gridTypes[6] == 0)
            {
                mPlay = 6;
            }
            else if (BoardManager.Instance.gridTypes[8] == 0)
            {
                mPlay = 8;
            }
            //再占边
            else if (BoardManager.Instance.gridTypes[1] == 0)
            {
                mPlay = 1;
            }
            else if (BoardManager.Instance.gridTypes[3] == 0)
            {
                mPlay = 3;
            }
            else if (BoardManager.Instance.gridTypes[5] == 0)
            {
                mPlay = 5;
            }
            else if (BoardManager.Instance.gridTypes[7] == 0)
            {
                mPlay = 7;
            }
            else mPlay = BoardManager.Instance.GetRandomAvailableGrid();///保底措施
        }

        BoardManager.Instance.PlayChess(mPlay);
    }


}
