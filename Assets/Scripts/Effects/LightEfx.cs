using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public class LightEfx : SimpleEfx
{
    [SerializeField]Transform lightTrans;
    [SerializeField] Image lightImage;
    float localScaleF = 1f;
    float scaleUpSpeed = 1f;
    float localPositionY = 0f;
    float moveDownSpeed = 3f;
    float fadeInTime;
    float fadeOutTime;
    [SerializeField]bool keepSmall=false;
    protected override void OnPlayEffect()
    {
        base.OnPlayEffect();
        if(lightTrans==null)
            lightTrans=transform.GetChild(0);

        if (lightTrans == null)
            return;
        if (lightImage == null)
            lightImage = lightTrans.GetComponent<Image>();     

        if (keepSmall)
        {
            localScaleF = 0.5f;
            localPositionY = 150f;

            EfxDuration *= Random.Range(0.6f, 1.2f);
            scaleUpSpeed = Random.Range(0.3f, 0.6f) * 1f;
            moveDownSpeed = Random.Range(0, 0.2f) * 3f;
        }
        else
        {
            localScaleF = Random.Range(0.5f, 1f);
            localPositionY = Random.Range(-1f, 1f) * 100f;

            EfxDuration *= Random.Range(0.8f, 1.6f);
            scaleUpSpeed = Random.Range(0.4f, 1.2f) * 1f;
            moveDownSpeed = Random.Range(0, 2f) * 3f;
        }
        lightTrans.localScale = Vector3.one * localScaleF;
        lightTrans.localPosition = new Vector3(0, localPositionY, 0);

        lightImage.DOFade(0, 0);
        fadeInTime = Random.Range(0, 0.2f) * EfxDuration;
        fadeOutTime = Random.Range(0, 0.5f) * EfxDuration;
        lightImage.DOFade(1, fadeInTime);
        StartCoroutine(DelayFadeOut());
    }
    protected override void OnUpdateEffect(float deltaTime)
    {
        base.OnUpdateEffect(deltaTime);
        if (lightTrans==null)
        {
            return;
        }
        localScaleF += scaleUpSpeed * deltaTime;
        lightTrans.localScale = Vector3.one * localScaleF;
        localPositionY -=moveDownSpeed * deltaTime;
        lightTrans.localPosition = new Vector3(0, localPositionY, 0);
    }
    public void KeepSmall(bool b)
    {
        keepSmall = b;
    }

    IEnumerator DelayFadeOut()
    {
        yield return new WaitForSeconds(EfxDuration-fadeOutTime);
        lightImage.DOFade(0, fadeOutTime);
    }
}
