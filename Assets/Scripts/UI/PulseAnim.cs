using UnityEngine;

/// <summary>
/// Basit nabız (pulse) animasyonu. UI elemanlarına canlılık katar.
/// </summary>
public class PulseAnim : MonoBehaviour
{
    public float speed = 1.5f;
    public float minScale = 0.9f;
    public float maxScale = 1.1f;
    private Vector3 baseScale;

    void Start() => baseScale = transform.localScale;

    void Update()
    {
        float t = (Mathf.Sin(Time.time * speed) + 1f) * 0.5f;
        float s = Mathf.Lerp(minScale, maxScale, t);
        transform.localScale = baseScale * s;
    }
}
