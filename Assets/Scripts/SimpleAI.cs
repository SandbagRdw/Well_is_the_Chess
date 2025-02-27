using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAI : Singleton<SimpleAI>
{
    [SerializeField][Tooltip("��AI��װ˼��ʱ��")] float easyAIThinkingTime=0.5f;
    [SerializeField][Tooltip("�е�AI��װ˼��ʱ��")] float mediumAIThinkingTime = 0.75f;
    [SerializeField][Tooltip("����AI��װ˼��ʱ��")] float hardAIThinkingTime = 1f;
    int mType = 0;
    /// <summary>
    /// �ֵ�AI�����ѶȾ�����һ����
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
    /// ��ai���Ǵ�ɵ�ӣ�˼��ʱ��Ҳ��
    /// </summary>
    /// <returns></returns>
    IEnumerator EasyPlay()
    {
        yield return new WaitForSeconds(easyAIThinkingTime);
        BoardManager.Instance.PlayChess(BoardManager.Instance.GetRandomAvailableGrid());
    }

    /// <summary>
    /// �м�ai�ܷ���ʤ����λ��Ҳ����ֹ���ʤ��
    /// </summary>
    /// <returns></returns>
    IEnumerator MediumPlay()
    {
        yield return new WaitForSeconds(mediumAIThinkingTime);
        if (BoardManager.Instance.LastIndex == -1)//���ֿ�����ʱ�����1��λ�÷���
        {
            BoardManager.Instance.PlayChess(BoardManager.Instance.GetRandomAvailableGrid());
            yield break;
        }

        int mPlay = BoardManager.Instance.FindOneMoveWin(mType);//����Լ�������һ����ʤ��
        if (mPlay >= 0)
        {
            BoardManager.Instance.PlayChess(mPlay);
            yield break;
        }

        mPlay = BoardManager.Instance.FindOneMoveWin(mType * (-1)); //������ʤ����Ҫ������һ�񣬷���ֵ�·
        if (mPlay < 0)
        {
            mPlay = BoardManager.Instance.GetRandomAvailableGrid(); //��������޷�һ��ʤ�������ѡ���ø���
        }
        BoardManager.Instance.PlayChess(mPlay);

    }

    IEnumerator HardPlay()
    {
        yield return new WaitForSeconds(hardAIThinkingTime);
        if (BoardManager.Instance.LastIndex == -1)//���ַ��м�
        {
            BoardManager.Instance.PlayChess(4);
            yield break;
        }
        int eType = mType * (-1);
        int mPlay = BoardManager.Instance.FindOneMoveWin(mType);//����Լ�������һ����ʤ��
        if (mPlay >= 0)
        {
            BoardManager.Instance.PlayChess(mPlay);
            yield break;
        }

        mPlay = BoardManager.Instance.FindOneMoveWin(eType); //������ʤ����Ҫ������һ�񣬷���ֵ�·
        if (mPlay >= 0)
        {
            BoardManager.Instance.PlayChess(mPlay);
            yield break;
        }
        int lastIndex= BoardManager.Instance.LastIndex;
        //����⣬��һ��û����ʹ����һ��ʤ���ĸ��ӣ�Ҫ��������

        //��һ�����м����
        if (lastIndex == 4)
        {
            //ռ���Ľ�
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
                mPlay = BoardManager.Instance.GetRandomAvailableGrid(); //�м���ĽǶ�û�ˣ����ѡ1��
        }
        //��һ�ֶ��ַ����Ľ�֮һ
        else if (lastIndex == 0 || lastIndex == 2 || lastIndex == 6 || lastIndex == 8)
        {
            //�м�����յģ��ض���ռ���м�
            if (BoardManager.Instance.gridTypes[4] == 0)
            {
                mPlay = 4;
            }
            //����ռ1�ǣ��Խ�Ϊ�գ�����ռ��Խ�
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
            //��������ԽǶ�������ռ��
            else if ((BoardManager.Instance.gridTypes[0] == eType && BoardManager.Instance.gridTypes[8] == eType) 
                || (BoardManager.Instance.gridTypes[2] == eType && BoardManager.Instance.gridTypes[6] == eType))
            {
                //��1�ߣ���ʱ��1�Ǳ��䣬���ֽ�3��2��˫Ӯ
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
            //����ռ��ǵĶԽ����Լ������Լ�ռ���ҽ�
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
            else mPlay =  BoardManager.Instance.GetRandomAvailableGrid();//û���ˣ����ѡһ��
        }
        //��һ�ֶ��ַ����ı�֮һ
        else if (lastIndex == 1 || lastIndex == 3 || lastIndex == 5 || lastIndex == 7)
        {
            //��ռ���м�
            if (BoardManager.Instance.gridTypes[4] == 0)
            {
                mPlay = 4;
            }
            //������ռ��2�����ڱߣ����нǿ��ţ�����ռ�нǣ���ֹ1��2��
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
            //������û���������ٱߣ�ռ����ֱ��ԱߵĽ�
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
            //���������������ʱ����ռ��
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
            //��ռ��
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
            else mPlay = BoardManager.Instance.GetRandomAvailableGrid();///���״�ʩ
        }

        BoardManager.Instance.PlayChess(mPlay);
    }


}
