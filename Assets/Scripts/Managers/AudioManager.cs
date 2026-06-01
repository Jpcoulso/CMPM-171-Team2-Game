using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public enum MusicType
    {
        CombatMusic,
        BossMusic,
        ArmoryMusic,
        DefaultMusic,
    }
    public enum SFXType
    {
        SFX1,
        SFX2,
    }

    [System.Serializable]
    public class MusicSound
    {
        [Range(0f, 1f)]
        public float volume = 1f;
        public bool loop = true;
        public MusicType type;
        public AudioClip clip;
    }
    [System.Serializable]
    public class SFXSound
    {
        [Range(0f, 1f)]
        public float volume = 1f;
        public bool loop = false;
        public SFXType type;
        public AudioClip[] clips;
    }

    [Header("Music")]
    public List<MusicSound> musicSounds;
    [Header("SFX")]
    public List<SFXSound> sfxSounds;
    [Header("Fade Transition")]
    public float fadeDuration = 3f;

    private Dictionary<MusicType, MusicSound> musicSoundDict = new();
    private Dictionary<SFXType, SFXSound> sfxSoundDict = new();
    private AudioSource musicSource;
    private AudioSource sfxSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialize sound dictionary
        foreach (MusicSound s in musicSounds) { musicSoundDict[s.type] = s; }
        foreach (SFXSound s in sfxSounds) { sfxSoundDict[s.type] = s; }

        // Create audio sources
        musicSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();
    }
    void Start()
    {
        // Play initial music if needed
        PlayMusic(MusicType.DefaultMusic);
    }

    public void PlayMusic(MusicType type, float volumeOverride = -1f)
    {
        if (!musicSoundDict.TryGetValue(type, out MusicSound s)) return;
        musicSource.clip = s.clip;
        musicSource.volume = volumeOverride >= 0 ? volumeOverride : s.volume;
        musicSource.loop = s.loop;    // Loops by default, can be overridden when playing
        musicSource.Play();
    }
    public void PlaySFX(SFXType type, float volumeOverride = -1f)
    {
        if (!sfxSoundDict.TryGetValue(type, out SFXSound s)) return;
        if (s.clips == null || s.clips.Length == 0)
        {
            Debug.LogWarning("No clips assigned for SFX type: " + type);
            return;
        }
        AudioClip clip = s.clips[Random.Range(0, s.clips.Length)];
        sfxSource.PlayOneShot(clip, volumeOverride >= 0 ? volumeOverride : s.volume);
    }
    public void FadeToMusic(MusicType type)
    {
        if (!musicSoundDict.TryGetValue(type, out MusicSound s)) return;
        StartCoroutine(FadeTransition(musicSource, s.clip, s.volume, s.loop));
    }
    private IEnumerator FadeTransition(AudioSource source, AudioClip newClip, float targetVolume, bool loop)
    {
        // Fade out
        float startVolume = source.volume;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            source.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }
        source.Stop();

        // Swap and fade in
        source.clip = newClip;
        source.loop = loop;
        source.volume = 0f;
        source.Play();
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            source.volume = Mathf.Lerp(0f, targetVolume, t / fadeDuration);
            yield return null;
        }
        source.volume = targetVolume;
    }
}