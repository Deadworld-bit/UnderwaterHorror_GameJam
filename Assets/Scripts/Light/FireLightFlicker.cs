using UnityEngine;

[RequireComponent(typeof(Light))]
public class FireLightFlicker : MonoBehaviour
{
    [Header("Flicker Settings")]
    [SerializeField] private float minIntensity = 0.5f;
    [SerializeField] private float maxIntensity = 1.5f;
    [SerializeField] private float flickerSpeed = 1.0f;

    private Light lightComponent;
    private float randomSeed;

    void Start()
    {
        lightComponent = GetComponent<Light>();
        randomSeed = Random.Range(0.0f, 100.0f);
    }

    void Update()
    {
        float noise = Mathf.PerlinNoise(randomSeed + Time.time * flickerSpeed, 0.0f);
        lightComponent.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
    }
}