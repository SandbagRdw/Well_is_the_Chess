using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
[Serializable]
public struct AudioStruct
{
    public string name;
    public AudioClip clip;
}
public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] Dictionary<string, AudioClip> sfxDict = new Dictionary<string, AudioClip>();
    [SerializeField] AudioStruct[] sfxs;
    [SerializeField] Dictionary<string, AudioClip> bgmDict = new Dictionary<string, AudioClip>();
    [SerializeField] AudioStruct[] bgms;
    [SerializeField] private AudioSource globalSfxAudio = null;
    [SerializeField] private AudioSource globalBgmAudio1 = null;
    [SerializeField] private AudioSource globalBgmAudio2 = null;

    /// <summary>
    /// 最大音量限制，用调节的原始值乘以该值，得到实际值
    /// </summary>
    [SerializeField] float maxVolume = 0.8f;
    public float SFXVolume { get;private set; }
    public float BGMVolume { get; private set; }

    private Coroutine delayReplay;

    [SerializeField] float bgmIntervalTime = 10f;
    [SerializeField] float bgmIntervalRandomness = 5f;

    [SerializeField] float crossfadeTime = 1.5f;
    private int bgmOn = 0;
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        if (globalSfxAudio == null)
            globalSfxAudio = transform.AddComponent<AudioSource>();
        if (globalBgmAudio1 == null)
            globalBgmAudio1 = transform.AddComponent<AudioSource>();
        if (globalBgmAudio2 == null)
            globalBgmAudio2 = transform.AddComponent<AudioSource>();
        SetSFXVolume(1);
        SetBGMVolume(1);

        LoadAllSound();
    }
    void LoadAllSound()
    {
        sfxDict.Clear();
        bgmDict.Clear();
        foreach (AudioStruct s in sfxs)
        {
            sfxDict.Add(s.name, s.clip);
        }
        foreach (AudioStruct s in bgms)
        {
            bgmDict.Add(s.name, s.clip);
        }

    }
    /// <summary>
    /// 简单按照名字播放音效，无堆叠处理，不同时播，直接新的顶掉旧的
    /// </summary>
    /// <param name="sfxName"></param>
    public void PlaySfx(string sfxName)
    {
        if (globalSfxAudio == null)
            return;
        AudioClip clip = sfxDict[sfxName];
        if (clip == null)
        {
            {
                Debug.LogError("Sfx not found:" + sfxName);
                return;
            }
        }
        globalSfxAudio.Stop();
        globalSfxAudio.PlayOneShot(clip);
    }

    public void PlayBgmCrossFade(string bgmName, bool pauseAnothor = true)
    {
        if (globalBgmAudio1 == null)
            return;
        AudioClip clip = bgmDict[bgmName];
        if (clip == null)
        {
            {
                Debug.LogError("Bgm not found:" + bgmName);
                return;
            }
        }
        Debug.Log("Sync time:" + globalBgmAudio1.time + " with " + globalBgmAudio2.time);
        StopAllCoroutines();
        globalBgmAudio1.loop = !pauseAnothor;
        globalBgmAudio2.loop = !pauseAnothor;
        if (bgmOn <= 0)//无人在播，首播直接播
        {
            globalBgmAudio1.clip = clip;
            //globalBgmAudio1.DOFade(bgmVolume, 1f);
            globalBgmAudio1.Play();

            bgmOn = 1;
        }
        else if (bgmOn == 1) //1号在播，渐出后暂停，2号判断若是同名bgm，则继续播放（否则装载clip），渐入
        {
            if (globalBgmAudio1.clip != null && globalBgmAudio1.clip == clip)//正在播要播的，return
            {
                return;
            }
            Debug.Log("Crossfade from " + globalBgmAudio1.clip.name + " to " + clip.name);
            Tween fadeOut = globalBgmAudio1.DOFade(0, crossfadeTime);
            if (pauseAnothor)
                fadeOut.onComplete += () => globalBgmAudio1.Pause();


            globalBgmAudio2.DOFade(BGMVolume, crossfadeTime);
            if (globalBgmAudio2.clip != null && globalBgmAudio2.clip == clip)
            {
                globalBgmAudio2.UnPause();
            }
            else
            {
                globalBgmAudio2.clip = clip;
                globalBgmAudio2.Play();
            }
            bgmOn = 2;
        }
        else if (bgmOn == 2) //2号在播
        {
            if (globalBgmAudio2.clip != null && globalBgmAudio2.clip == clip)//正在播要播的，return
            {
                return;
            }
            Debug.Log("Crossfade from " + globalBgmAudio2.clip.name + " to " + clip.name);
            Tween fadeOut = globalBgmAudio2.DOFade(0, crossfadeTime);
            if (pauseAnothor)
                fadeOut.onComplete += () => globalBgmAudio2.Pause();

            globalBgmAudio1.DOFade(BGMVolume, crossfadeTime);
            if (globalBgmAudio1.clip != null && globalBgmAudio1.clip == clip)
            {
                globalBgmAudio1.UnPause();
            }
            else
            {
                globalBgmAudio1.clip = clip;
                globalBgmAudio1.Play();
            }
            bgmOn = 1;
        }
        if (pauseAnothor)
            delayReplay = StartCoroutine(DelayReplayBgm(clip.length));
    }

    /// <summary>
    /// 双轨同播，1前台1后台，以备后续互切
    /// </summary>
    /// <param name="front"></param>
    /// <param name="back"></param>
    public void PlayBgmWithBg(string front, string back)
    {
        AudioClip frontClip = bgmDict[front];
        AudioClip backClip = bgmDict[back];
        if(globalBgmAudio1.clip == frontClip&& globalBgmAudio2.clip == backClip)
        {
            globalBgmAudio1.UnPause();
            globalBgmAudio1.DOFade(BGMVolume, 0.5f);
            globalBgmAudio2.time = globalBgmAudio1.time;
            return;
        }
        globalBgmAudio1.clip = frontClip;
        globalBgmAudio1.Play();
        globalBgmAudio1.DOFade(BGMVolume, 0.5f);
        bgmOn = 1;
        globalBgmAudio2.clip = backClip;
        globalBgmAudio2.volume = 0;
        globalBgmAudio2.Play();
    }

    IEnumerator DelayReplayBgm(float bgmDuration)
    {
        float delay = bgmIntervalTime + UnityEngine.Random.Range(-1f, 1f) * bgmIntervalRandomness;
        Debug.Log("Start play at " + Time.realtimeSinceStartup + ", will replay with " + delay + " interval.");
        delay += bgmDuration;
        yield return new WaitForSeconds(delay);
        Debug.Log("Start replay at " + Time.realtimeSinceStartup + ".");
        if (bgmOn <= 1)
        {
            globalBgmAudio1.Play();
        }
        else if (bgmOn == 2)
        {
            globalBgmAudio2.Play();
        }
    }

    public void SetSFXVolume(float sfxV)
    {
        SFXVolume = Mathf.Clamp(sfxV , 0, 1);
        if (globalSfxAudio != null)
            globalSfxAudio.volume = SFXVolume * maxVolume;
    }
    public void SetBGMVolume(float bgmV)
    {
        BGMVolume = Mathf.Clamp(bgmV , 0, 1);
        if (globalBgmAudio1 != null)
            globalBgmAudio1.volume = BGMVolume * maxVolume;
        if (globalBgmAudio2 != null)
            globalBgmAudio2.volume = BGMVolume * maxVolume;
    }
}
