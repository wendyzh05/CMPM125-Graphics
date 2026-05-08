using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Time Settings")]
    public float dayLengthSeconds = 60f;
    [Range(0f, 1f)] public float timeOfDay = 0.25f;

    [Header("Skyboxes")]
    public Material daySkybox;
    public Material nightSkybox;

    [Header("Sun")]
    public Light sunLight;
    public float daySunIntensity = 1.2f;
    public float nightSunIntensity = 0.05f;

    [Header("Interior / Night Lights")]
    public Light[] nightLights;

    private bool isNight;

    void Start()
    {
        isNight = timeOfDay > 0.5f;

        foreach (Light light in nightLights)
        {
            if (light != null)
            {
                light.enabled = isNight;
            }
        }

        RenderSettings.skybox = isNight ? nightSkybox : daySkybox;

        UpdateDayNight();
    }

    void Update()
    {
        timeOfDay += Time.deltaTime / dayLengthSeconds;

        if (timeOfDay >= 1f)
        {
            timeOfDay = 0f;
        }

        UpdateDayNight();
    }

    void UpdateDayNight()
    {
        // 0.25 = morning/day, 0.75 = evening/night
        bool shouldBeNight = timeOfDay > 0.5f;

        // Rotate sun across the sky
        if (sunLight != null)
        {
            sunLight.transform.rotation = Quaternion.Euler((timeOfDay * 360f) - 90f, 170f, 0f);
            sunLight.intensity = shouldBeNight ? nightSunIntensity : daySunIntensity;
        }

        // Only switch when day/night changes
        if (shouldBeNight != isNight)
        {
            isNight = shouldBeNight;

            RenderSettings.skybox = isNight ? nightSkybox : daySkybox;

            foreach (Light light in nightLights)
            {
                if (light != null)
                {
                    light.enabled = isNight;
                }
            }

            DynamicGI.UpdateEnvironment();
        }
    }
}
