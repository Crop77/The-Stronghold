using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager I { get; private set; }

    [Header("Audio Source")]
    public AudioSource musicSource;

    [Header("Build Phase Music")]
    public AudioClip[] buildPhaseTracks; 

    [Header("Combat Phase Music")]
    public AudioClip[] combatPhaseTracks; 

    [Header("Boss Music")]
    public AudioClip bossTrack;

    [Header("Menu Music")]
    public AudioClip menuTrack;

    [Header("Defeat Music")]
    public AudioClip defeatTrack;

    public float musicVolume = 0.5f;  
    public float fadeDuration = 1.0f; 

    Coroutine _fadeRoutine;



    int _lastBuildIndex = -1;
    int _lastCombatIndex = -1;

    void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
        DontDestroyOnLoad(gameObject);

        if (musicSource == null)
        {
            musicSource = GetComponent<AudioSource>();
        }

        if (musicSource != null)
        {
            musicSource.loop = true; 
            musicSource.playOnAwake = false;
        }
    }

    System.Collections.IEnumerator FadeToClip(AudioClip newClip)
    {
        if (!musicSource.isPlaying || musicSource.clip == null)
        {
            musicSource.clip = newClip;
            musicSource.volume = 0f;
            musicSource.Play();

            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.unscaledDeltaTime;
                float k = Mathf.Clamp01(t / fadeDuration);
                musicSource.volume = Mathf.Lerp(0f, musicVolume, k);
                yield return null;
            }

            musicSource.volume = musicVolume;
            yield break;
        }

        float startVol = musicSource.volume;
        float tOut = 0f;
        while (tOut < fadeDuration)
        {
            tOut += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(tOut / fadeDuration);
            musicSource.volume = Mathf.Lerp(startVol, 0f, k);
            yield return null;
        }

        musicSource.Stop();

        musicSource.clip = newClip;
        musicSource.Play();

        float tIn = 0f;
        while (tIn < fadeDuration)
        {
            tIn += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(tIn / fadeDuration);
            musicSource.volume = Mathf.Lerp(0f, musicVolume, k);
            yield return null;
        }

        musicSource.volume = musicVolume;
    }



    public void PlayBuildPhase()
    {
        if (buildPhaseTracks == null || buildPhaseTracks.Length == 0 || musicSource == null)
            return;

        AudioClip clip = GetRandomClip(buildPhaseTracks, ref _lastBuildIndex);
        PlayClip(clip);
        Debug.Log("[AudioManager] Build phase music: " + clip.name);
    }

    public void PlayCombatPhase()
    {
        if (combatPhaseTracks == null || combatPhaseTracks.Length == 0 || musicSource == null)
            return;

        AudioClip clip = GetRandomClip(combatPhaseTracks, ref _lastCombatIndex);
        PlayClip(clip);
        Debug.Log("[AudioManager] Combat phase music: " + clip.name);
    }

    public void PlayBossTheme()
    {
        if (bossTrack == null || musicSource == null)
            return;

        PlayClip(bossTrack);
        Debug.Log("[AudioManager] Boss theme: " + bossTrack.name);
    }

    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }


    void PlayClip(AudioClip clip)
    {
        if (clip == null || musicSource == null) return;

        if (fadeDuration <= 0f)
        {
            musicSource.clip = clip;
            musicSource.volume = musicVolume;
            musicSource.Play();
            return;
        }

        if (_fadeRoutine != null)
            StopCoroutine(_fadeRoutine);

        _fadeRoutine = StartCoroutine(FadeToClip(clip));
    }


    AudioClip GetRandomClip(AudioClip[] pool, ref int lastIndex)
    {
        if (pool.Length == 1)
        {
            lastIndex = 0;
            return pool[0];
        }

        int index;
        do
        {
            index = Random.Range(0, pool.Length);
        } while (index == lastIndex);

        lastIndex = index;
        return pool[index];
    }

    public void PlayMenuMusic()
    {
        if (musicSource == null || menuTrack == null)
            return;

        PlayClip(menuTrack);
        Debug.Log("[AudioManager] Menu music: " + menuTrack.name);
    }

    public void PlayDefeatMusic()
    {
        if (musicSource == null || defeatTrack == null)
            return;

        PlayClip(defeatTrack);
        Debug.Log("[AudioManager] Defeat music: " + defeatTrack.name);
    }

}
