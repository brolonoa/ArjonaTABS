using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
   public const string VolumeKey = "VolumenGeneral";

   public static SoundManager Instance { get; private set; }

   [Header("Music")]
   [SerializeField] private AudioClip menuMusic;
   [SerializeField] private AudioClip gameMusic;
   [SerializeField] private string menuSceneName = "MenuPrincipal";
   [SerializeField] [Range(0, 1)] private float musicVolume = 0.6f;
   [SerializeField] private float fadeTime = 1.5f;

   [Header("Combat")]
   [SerializeField] private AudioClip combatStartSound;
   [SerializeField] [Range(0, 1)] private float sfxVolume = 1f;

   private AudioSource[] musicSources;
   private AudioSource sfxSource;
   private readonly float[] fades = new float[2];
   private int activeSource;
   private GameManager subscribedGameManager;
   private Coroutine fadeCoroutine;

   [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
   private static void CreateInstance()
   {
      AudioListener.volume = PlayerPrefs.GetFloat(VolumeKey, 1f);

      if (Instance != null)
      {
         return;
      }

      SoundManager prefab = Resources.Load<SoundManager>("SoundManager");
      if (prefab != null)
      {
         Instantiate(prefab).name = "SoundManager";
      }
   }

   private void Awake()
   {
      if (Instance != null && Instance != this)
      {
         Destroy(gameObject);
         return;
      }

      Instance = this;
      DontDestroyOnLoad(gameObject);

      musicSources = new AudioSource[2];
      for (int i = 0; i < musicSources.Length; i++)
      {
         musicSources[i] = gameObject.AddComponent<AudioSource>();
         musicSources[i].loop = true;
         musicSources[i].playOnAwake = false;
         musicSources[i].volume = 0;
      }

      sfxSource = gameObject.AddComponent<AudioSource>();
      sfxSource.playOnAwake = false;
   }

   private void OnEnable()
   {
      SceneManager.sceneLoaded += OnSceneLoaded;
   }

   private void OnDisable()
   {
      SceneManager.sceneLoaded -= OnSceneLoaded;
      UnsubscribeCombat();
   }

   private void Start()
   {
      OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
   }

   private void Update()
   {
      for (int i = 0; i < musicSources.Length; i++)
      {
         musicSources[i].volume = fades[i] * musicVolume;
      }
   }

   private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
   {
      if (mode != LoadSceneMode.Single)
      {
         return;
      }

      PlayMusic(scene.name == menuSceneName ? menuMusic : gameMusic);
      SubscribeCombat();
   }

   private void SubscribeCombat()
   {
      UnsubscribeCombat();

      if (GameManager.Instance != null)
      {
         subscribedGameManager = GameManager.Instance;
         subscribedGameManager.OnCombatStart += PlayCombatStart;
      }
   }

   private void UnsubscribeCombat()
   {
      if (subscribedGameManager != null)
      {
         subscribedGameManager.OnCombatStart -= PlayCombatStart;
      }

      subscribedGameManager = null;
   }

   public void PlayMusic(AudioClip clip)
   {
      if (clip == null)
      {
         return;
      }

      if (musicSources[activeSource].clip == clip && musicSources[activeSource].isPlaying)
      {
         return;
      }

      activeSource = 1 - activeSource;
      musicSources[activeSource].clip = clip;
      musicSources[activeSource].Play();

      if (fadeCoroutine != null)
      {
         StopCoroutine(fadeCoroutine);
      }

      fadeCoroutine = StartCoroutine(CrossFade());
   }

   public void PlaySfx(AudioClip clip)
   {
      if (clip != null)
      {
         sfxSource.PlayOneShot(clip, sfxVolume);
      }
   }

   private void PlayCombatStart()
   {
      PlaySfx(combatStartSound);
   }

   private IEnumerator CrossFade()
   {
      int fadingOut = 1 - activeSource;
      float startIn = fades[activeSource];
      float startOut = fades[fadingOut];

      for (float t = 0; t < fadeTime; t += Time.unscaledDeltaTime)
      {
         float k = t / fadeTime;
         fades[activeSource] = Mathf.Lerp(startIn, 1, k);
         fades[fadingOut] = Mathf.Lerp(startOut, 0, k);
         yield return null;
      }

      fades[activeSource] = 1;
      fades[fadingOut] = 0;
      musicSources[fadingOut].Stop();
      fadeCoroutine = null;
   }
}
