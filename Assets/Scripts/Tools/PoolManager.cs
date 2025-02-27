using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
[Serializable]
public struct PoolStruct
{
	public GameObject prefab;
	public int preloadAmount;
}
public class PoolManager : Singleton<PoolManager>
{
    [SerializeField] List<PoolStruct> preloadPrefabs=new List<PoolStruct>();
    public void PreloadAll()
    {
		if (preloadPrefabs.Count > 0)
		{
			foreach(PoolStruct pool in preloadPrefabs)
			{
				PreloadTargetGameObject(pool.prefab, pool.preloadAmount);
			}
		}
    }
    /// <summary>
    /// 归还物体到Pool下，隐藏之
    /// </summary>
    /// <param name="gameObject"></param>
    public void ReturnGameObject(GameObject go)
    {
		go.SetActive(false);
		go.transform.SetParent(transform);

    }
    /// <summary>
    /// 从pool里拿一个已回收的物体，或者创建一个新的（均需要重设父级！）
    /// </summary>
    /// <param name="name">预制体名</param>
    /// <returns></returns>
    public GameObject GetGameObject(string name)
    {
        GameObject targetGameObject;
        Transform targetChild = transform.Find(name);
        if (targetChild)//若能找到指定名称的就拿来用
        {
            targetGameObject = targetChild.gameObject;
            targetGameObject.SetActive(true);
            
        }
        else
        {
			PoolStruct pool = preloadPrefabs.Find(s => s.prefab.name == name);
			if (pool.prefab!=null)
			{
				targetGameObject = PreloadTargetGameObject(pool.prefab, pool.preloadAmount);
				targetGameObject.SetActive(true);
			}
			else
			{
				targetGameObject = Instantiate(Resources.Load(name) as GameObject, transform);
				targetGameObject.name.Replace("(Clone)", "");//将创建的预制体的Clone后缀删除，变为原名，这样后续能用名字查找
			}
            
        }
        return targetGameObject;
    }

	/// <summary>
	/// 预加载一些对象备用
	/// </summary>
	/// <param name="target">目标物体</param>
	/// <param name="number">数量</param>
	/// <returns></returns>
	public GameObject PreloadTargetGameObject(GameObject target,int number)
	{
		GameObject tempGameObject=null;
		for(int i = 0; i < number; i++)
		{
			tempGameObject= Instantiate(target, transform);
			tempGameObject.name.Replace("(Clone)", "");//将创建的预制体的Clone后缀删除，变为原名，这样后续能用名字查找
			tempGameObject.SetActive(false);
		}
		return tempGameObject;
	}
}
