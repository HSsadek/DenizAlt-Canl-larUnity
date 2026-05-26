using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class FishARController : MonoBehaviour
{
    private ARTrackedImageManager trackedImageManager;
    private AudioSource infoAudio;

    [Header("Balık Bilgileri")]
    [SerializeField] private string fishName = "Mavi Yüzgeçli Orkinos";
    [SerializeField][TextArea] private string fishDescription =
        "Mavi yüzgeçli orkinos, okyanusların en hızlı yüzen balıklarından biridir. " +
        "80 km/s hıza ulaşabilir ve 2 metre boyunda olabilir. " +
        "Atlantik, Pasifik ve Hint Okyanuslarında yaşar.";

    void Awake()
    {
        trackedImageManager = GetComponent<ARTrackedImageManager>();
        infoAudio = GetComponentInChildren<AudioSource>();
    }

    void OnEnable() => trackedImageManager.trackedImagesChanged += OnChanged;
    void OnDisable() => trackedImageManager.trackedImagesChanged -= OnChanged;

    void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var newImage in eventArgs.added)
        {
            PlayFishAudio();
            // UI'da balık bilgisini göster
            if (UIManager.Instance != null)
                UIManager.Instance.ShowFishInfo(fishName, fishDescription);
        }

        foreach (var updatedImage in eventArgs.updated)
        {
            if (updatedImage.trackingState == TrackingState.Tracking)
            {
                if (!infoAudio.isPlaying) PlayFishAudio();
                if (UIManager.Instance != null)
                    UIManager.Instance.ShowFishInfo(fishName, fishDescription);
            }
            else
            {
                infoAudio.Pause();
                // Balık kaybolunca UI'yı gizle
                if (UIManager.Instance != null)
                    UIManager.Instance.HideFishInfo();
            }
        }
    }

    void PlayFishAudio()
    {
        if (infoAudio != null)
        {
            Debug.Log("Balık tespit edildi - ses çalınıyor!");
            infoAudio.Play();
        }
        else
        {
            Debug.LogError("Hata: AudioSource bulunamadı!");
        }
    }
}