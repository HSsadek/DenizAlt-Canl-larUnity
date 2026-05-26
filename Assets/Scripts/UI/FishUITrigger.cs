using UnityEngine;
using Vuforia;
using System.Collections;

/// <summary>
/// Vuforia ObserverBehaviour ile doğrudan çalışır.
/// Tracking durumuna göre UI bilgi kartını ve sesi kontrol eder.
/// </summary>
public class FishUITrigger : MonoBehaviour
{
    [Header("Balık Bilgileri")]
    public string fishName = "🐟 Mavi Yüzgeçli Orkinos";

    [TextArea(3, 10)]
    public string fishDescription =
        "🏎️ Okyanusun en hızlı yüzücüsü!\n" +
        "Saatte 80 km hızla yüzer — bir araba kadar hızlı!\n\n" +
        "📏 2 metre boyunda olabilir — belki senden bile uzun!\n\n" +
        "🌊 Atlantik, Pasifik ve Hint Okyanuslarında yaşar.\n\n" +
        "💡 Biliyor muydun?\n" +
        "Vücut ısısını kendisi ayarlayabilen nadir balıklardan biridir!";

    [Header("Ses (Opsiyonel)")]
    public AudioSource fishAudio;

    private ObserverBehaviour observer;
    private bool vuforiaReady = false;
    private bool isCurrentlyShowing = false;

    void Start()
    {
        // ObserverBehaviour'u bul
        observer = GetComponent<ObserverBehaviour>();
        if (observer == null)
            observer = GetComponentInParent<ObserverBehaviour>();

        if (observer != null)
        {
            observer.OnTargetStatusChanged += OnTargetStatusChanged;
            Debug.Log("[FishUITrigger] Observer'a bağlandı: " + observer.TargetName);
        }

        // AudioSource atanmamışsa otomatik bul
        if (fishAudio == null)
            fishAudio = GetComponentInChildren<AudioSource>(true);

        StartCoroutine(WaitForReady());
    }

    IEnumerator WaitForReady()
    {
        yield return new WaitForSeconds(4f);
        vuforiaReady = true;

        if (observer != null && observer.TargetStatus.Status == Status.TRACKED && !isCurrentlyShowing)
            ShowInfo();
    }

    void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus targetStatus)
    {
        if (!vuforiaReady) return;
        if (UIManager.Instance == null) return;

        bool isTracked = (targetStatus.Status == Status.TRACKED);

        if (isTracked && !isCurrentlyShowing)
            ShowInfo();
        else if (!isTracked && isCurrentlyShowing)
            HideInfo();
    }

    void ShowInfo()
    {
        isCurrentlyShowing = true;
        UIManager.Instance.ShowFishInfo(fishName, fishDescription);

        // Arka plan müziğini kıs (bilgi sesi öne çıksın)
        if (BackgroundMusic.Instance != null)
            BackgroundMusic.Instance.DuckVolume();

        // Ses çal - obje aktif olduktan sonra
        StartCoroutine(PlayAudioDelayed());
    }

    void HideInfo()
    {
        isCurrentlyShowing = false;
        UIManager.Instance.HideFishInfo();

        // Sesi durdur
        if (fishAudio != null && fishAudio.isPlaying)
            fishAudio.Stop();

        // Arka plan müziğini normale döndür
        if (BackgroundMusic.Instance != null)
            BackgroundMusic.Instance.RestoreVolume();
    }

    IEnumerator PlayAudioDelayed()
    {
        // Objenin aktif olmasını bekle (1 frame)
        yield return null;
        yield return null;

        if (fishAudio != null && fishAudio.gameObject.activeInHierarchy)
        {
            fishAudio.Play();
            Debug.Log("[FishUITrigger] Ses çalınıyor!");
        }
    }

    void OnDestroy()
    {
        if (observer != null)
            observer.OnTargetStatusChanged -= OnTargetStatusChanged;
    }
}
