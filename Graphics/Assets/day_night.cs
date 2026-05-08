using UnityEngine;
using TMPro;
using UnityEngine.Rendering;

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

    [Header("UI")]
    public TextMeshProUGUI timeText;

    [Header("Post Processing")]
    public Volume globalVolume;
    public VolumeProfile dayProfile;
    public VolumeProfile nightProfile;

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

        UpdateUI();
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

            if (globalVolume != null)
            {
                globalVolume.profile = isNight ? nightProfile : dayProfile;
            }
            
            DynamicGI.UpdateEnvironment();
            
        }
    }

    void UpdateUI()
{
    float remainingTime;

    if (isNight)
    {
        remainingTime = (1f - timeOfDay) * dayLengthSeconds;
        timeText.text = "Day in: " + Mathf.Ceil(remainingTime) + "s";

        timeText.color = Color.cyan;
    }
    else
    {
        remainingTime = (0.5f - timeOfDay) * dayLengthSeconds;
        timeText.text = "Night in: " + Mathf.Ceil(remainingTime) + "s";

        timeText.color = Color.yellow;
    }
}
}
