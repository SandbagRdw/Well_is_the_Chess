using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEfx : MonoBehaviour
{
	public float EfxDuration = 1f;
	[SerializeField] float tick = 0f;
	public bool IsLoop = false;
    void Start()
    {
		OnPlayEffect();
    }
	protected virtual void OnPlayEffect()
	{
        tick = 0f;
    }

    // Update is called once per frame
    void Update()
    {
		OnUpdateEffect(Time.deltaTime); 
    }

	protected virtual void OnUpdateEffect(float deltaTime)
	{
        tick += deltaTime;
        if (tick >= EfxDuration && !IsLoop)
        {
            PoolManager.Instance.ReturnGameObject(gameObject);
        }
    }
}
