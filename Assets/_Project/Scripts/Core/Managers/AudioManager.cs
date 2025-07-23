// AudioManager.cs
using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource ambientSource;

    [Header("Audio Clips")]
    public AudioClip backgroundMusic;
    public AudioClip[] customerChatter;
    public AudioClip[] coffeeMachineSound;
    public AudioClip orderCompleteSound;
    public AudioClip cashRegisterSound;
    public AudioClip footstepSound;
    public AudioClip newOrderSound;
    public AudioClip orderTakenSound;
    public AudioClip orderFailedSound;
    public AudioClip[] happyCustomerSounds;
    public AudioClip[] angryCustomerSounds;
    public AudioClip brewCompleteSound;

    public static AudioManager Instance { get; private set; }

    private Dictionary<string, AudioClip> audioClips;
    private Dictionary<string, Vector3> equipmentPositions;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeAudioSystem()
    {
        audioClips = new Dictionary<string, AudioClip>();
        equipmentPositions = new Dictionary<string, Vector3>();

        // Register audio clips
        audioClips["background_music"] = backgroundMusic;
        audioClips["order_complete"] = orderCompleteSound;
        audioClips["cash_register"] = cashRegisterSound;
        audioClips["footstep"] = footstepSound;

        // Start background music
        PlayBackgroundMusic();

        // Start ambient sounds
        PlayAmbientSounds();
    }

    void PlayBackgroundMusic()
    {
        if(backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.volume = 0.3f;
            musicSource.Play();
        }
    }

    void PlayAmbientSounds()
    {
        // Coffee shop ambient sounds
        ambientSource.volume = 0.2f;
        InvokeRepeating(nameof(PlayRandomChatter), 5f, Random.Range(15f, 30f));
        InvokeRepeating(nameof(PlayCoffeeMachineSound), 10f, Random.Range(20f, 45f));
    }

    void PlayRandomChatter()
    {
        if(customerChatter.Length > 0)
        {
            AudioClip chatter = customerChatter[Random.Range(0, customerChatter.Length)];
            PlaySFX(chatter, 0.1f);
        }
    }

    void PlayCoffeeMachineSound()
    {
        if(coffeeMachineSound.Length > 0)
        {
            AudioClip machineSound = coffeeMachineSound[Random.Range(0, coffeeMachineSound.Length)];
            PlaySFX(machineSound, 0.3f);
        }
    }

    public void PlaySFX(string clipName, float volume = 1f)
    {
        if(audioClips.ContainsKey(clipName))
        {
            PlaySFX(audioClips[clipName], volume);
        }
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if(clip != null)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }

    public void PlaySFXAtPosition(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if(clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, position, volume);
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }

    // Game-specific audio methods
    public void PlayNewOrderSound()
    {
        if (newOrderSound != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(newOrderSound);
        }
    }

    public void PlayOrderTakenSound()
    {
        if (orderTakenSound != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(orderTakenSound);
        }
    }

    public void PlayOrderCompleteSound()
    {
        if (orderCompleteSound != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(orderCompleteSound);
        }
    }

    public void PlayOrderFailedSound()
    {
        if (orderFailedSound != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(orderFailedSound);
        }
    }

    public void PlayHappyCustomerSound()
    {
        if (happyCustomerSounds.Length > 0 && sfxSource != null)
        {
            AudioClip randomClip = happyCustomerSounds[Random.Range(0, happyCustomerSounds.Length)];
            sfxSource.PlayOneShot(randomClip);
        }
    }

    public void PlayAngryCustomerSound()
    {
        if (angryCustomerSounds.Length > 0 && sfxSource != null)
        {
            AudioClip randomClip = angryCustomerSounds[Random.Range(0, angryCustomerSounds.Length)];
            sfxSource.PlayOneShot(randomClip);
        }
    }

    public void PlayBrewCompleteSound()
    {
        if (brewCompleteSound != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(brewCompleteSound);
        }
    }

    // Asset Manager Integration
    public void RegisterEquipmentPosition(string equipmentName, Vector3 position)
    {
        equipmentPositions[equipmentName] = position;
        Debug.Log($"Registered equipment audio position: {equipmentName} at {position}");
    }

    public void PlayEquipmentSound(string equipmentName, AudioClip clip)
    {
        if (equipmentPositions.ContainsKey(equipmentName))
        {
            Vector3 position = equipmentPositions[equipmentName];
            PlaySpatialSound(clip, position);
        }
        else
        {
            // Fallback to regular SFX
            if (sfxSource != null)
            {
                sfxSource.PlayOneShot(clip);
            }
        }
    }

    void PlaySpatialSound(AudioClip clip, Vector3 worldPosition)
    {
        if (clip != null)
        {
            GameObject tempAudioSource = new GameObject("TempAudio_" + clip.name);
            tempAudioSource.transform.position = worldPosition;

            AudioSource spatialSource = tempAudioSource.AddComponent<AudioSource>();
            spatialSource.clip = clip;
            spatialSource.volume = sfxSource.volume;
            spatialSource.spatialBlend = 1f; // 3D sound
            spatialSource.rolloffMode = AudioRolloffMode.Linear;
            spatialSource.maxDistance = 10f;
            spatialSource.Play();

            // Destroy after clip finishes
            Destroy(tempAudioSource, clip.length + 0.1f);
        }
    }
}
