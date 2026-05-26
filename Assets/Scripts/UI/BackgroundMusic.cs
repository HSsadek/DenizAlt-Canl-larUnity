using UnityEngine;

/// <summary>
/// Arka plan müziği yöneticisi.
/// Sahneye boş bir GameObject ekle, bu scripti ata, müzik dosyasını sürükle.
/// </summary>
public class BackgroundMusic : MonoBehaviour
{
    [Header("Müzik Ayarları")]
    [Tooltip("Arka plan müzik dosyasını buraya sürükle")]
    public AudioClip musicClip;

    [Range(0f, 1f)]
    [Tooltip("Müzik ses seviyesi (0 = sessiz, 1 = tam ses)")]
    public float volume = 0.3f;

    [Tooltip("Müzik döngüde çalsın mı?")]
    public bool loop = true;

    private AudioSource audioSource;

    // Sahneler arası yok edilmesin
    private static BackgroundMusic instance;

    void Awake()
    {
        // Tekil örnek — sahneler arası korunsun
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        // AudioSource oluştur
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = musicClip;
        audioSource.volume = volume;
        audioSource.loop = loop;
        audioSource.playOnAwake = false;
        audioSource.priority = 256; // Düşük öncelik (efekt sesleri öne çıksın)
    }

    void Start()
    {
        if (musicClip != null)
        {
            audioSource.Play();
            Debug.Log("[BackgroundMusic] Müzik çalıyor: " + musicClip.name);
        }
        else
        {
            Debug.LogWarning("[BackgroundMusic] Müzik dosyası atanmadı! Inspector'dan 'Music Clip' alanına sürükle.");
        }
    }

    /// <summary>
    /// Balık bulunduğunda müziği biraz kıs (bilgi sesi öne çıksın)
    /// </summary>
    public void DuckVolume()
    {
        if (audioSource != null)
            audioSource.volume = volume * 0.3f;
    }

    /// <summary>
    /// Normal ses seviyesine dön
    /// </summary>
    public void RestoreVolume()
    {
        if (audioSource != null)
            audioSource.volume = volume;
    }

    public static BackgroundMusic Instance => instance;
}
