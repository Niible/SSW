using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class CustomSound
{
    public string name;
    public AudioClip[] clips;

    [Range(0f, 1f)] public float volume = 1.0f;
    [Range(0f, 1.5f)] public float pitch = 1.0f;
    public Vector2 randomVolumeRange = new Vector2(1.0f, 1.0f);
    public Vector2 randomPitchRange = new Vector2(1.0f, 1.0f);

    private AudioSource _source;

    public void SetSource(AudioSource source)
    {
        _source = source;
        var randomClip = Random.Range(0, clips.Length - 1);
        _source.clip = clips[randomClip];
    }

    public void Play()
    {
        if (clips.Length > 1)
        {
            var randomClip = Random.Range(0, clips.Length - 1);
            _source.clip = clips[randomClip];
        }

        _source.volume = volume * Random.Range(randomVolumeRange.x, randomVolumeRange.y);
        _source.pitch = pitch * Random.Range(randomPitchRange.x, randomPitchRange.y);
        _source.Play();
    }
}

public class AudioManager : MonoBehaviour
{
    // Make it a singleton class that can be accessible everywhere

    public static AudioManager Instance { get; private set; }

    [SerializeField] private CustomSound[] sounds;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one AudioManger in scene");
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        for (var i = 0; i < sounds.Length; i++)
        {
            var go = new GameObject("CustomSound_" + i + "_" + sounds[i].name);
            go.transform.SetParent(transform);
            sounds[i].SetSource(go.AddComponent<AudioSource>());
        }
    }

    public void PlayCustomSound(string soundName)
    {
        foreach (var t in sounds)
        {
            if (t.name != soundName) continue;
            t.Play();
            return;
        }

        Debug.LogWarning("AudioManager: CustomSound name not found in list: " + soundName);
    }
}