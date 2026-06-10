using UnityEngine;
using Zenject;

public class DayNightLightingController : MonoBehaviour
{
    [Header("Ссылки")]
    public Light sunLight;
    public Light moonLight;
    public Material skyboxMaterial;

    [Header("Настройки")]
    public Gradient sunColorGradient;
    public AnimationCurve sunIntensityCurve;
    public Gradient ambientGradient;
    public Gradient fogGradient;
    public Gradient skyColorGradient;
    public Gradient groundColorGradient;

    [Header("Луна")]
    public float moonIntensity = 0.15f;
    public Color moonColor = new Color(0.6f, 0.7f, 0.9f);

    private DayNightService _dayNightService;
    private SignalBus _signalBus;

    [Inject]
    public void Construct(DayNightService dayNightService, SignalBus signalBus)
    {
        _dayNightService = dayNightService;
        _signalBus = signalBus;
        _signalBus.Subscribe<TimeUpdateSignal>(OnTimeUpdated);
    }

    private void OnDestroy()
    {
        _signalBus?.Unsubscribe<TimeUpdateSignal>(OnTimeUpdated);
    }

    public float normalized;

    private void OnTimeUpdated()
    {
        normalized = _dayNightService.GetNormalizedTimeOfDay();

        UpdateSun(normalized);
        UpdateMoon(normalized);
        UpdateEnvironment(normalized);
    }

    private void UpdateSun(float t)
    {
        if (sunLight == null) return;

        // Вращение
        float sunAngle = t * 360f - 90f;           // 0 = рассвет
        sunLight.transform.rotation = Quaternion.Euler(sunAngle, 0, 0);

        sunLight.color = sunColorGradient.Evaluate(t);
        sunLight.intensity = sunIntensityCurve.Evaluate(t) * 1.8f; // подбери под сцену
    }

    private void UpdateMoon(float t)
    {
        if (moonLight == null) return;

        bool isNight = t < 0.25f || t > 0.75f; // примерно ночь

        moonLight.intensity = isNight ? moonIntensity : 0f;
        moonLight.color = moonColor;

        // Луна вращается с оффсетом ~180°
        float moonAngle = (t * 360f - 90f) + 180f;
        moonLight.transform.rotation = Quaternion.Euler(moonAngle, 0, 0);
    }

    private void UpdateEnvironment(float t)
    {
        RenderSettings.ambientLight = ambientGradient.Evaluate(t);
        RenderSettings.ambientIntensity = Mathf.Lerp(0.1f, 1.2f, t); // подбери

        // Fog
        RenderSettings.fogColor = fogGradient.Evaluate(t);

        // Skybox (Procedural)
        if (RenderSettings.skybox != null)
        {
            Material sky = RenderSettings.skybox;

            // Основные параметры Procedural Skybox
            sky.SetColor("_SkyColor", skyColorGradient.Evaluate(t));           // или "_SkyTint"
            sky.SetColor("_GroundColor", groundColorGradient.Evaluate(t));     // горизонт
//            sky.SetFloat("_AtmosphereThickness", Mathf.Lerp(0.5f, 1.2f, t));   // плотность атмосферы
//            sky.SetFloat("_Exposure", Mathf.Lerp(0.8f, 1.5f, t));             // яркость
            sky.SetFloat("_AtmosphereThickness", Mathf.Lerp(0.8f, 1.4f, Mathf.Pow(t, 0.6f))); // толще днём
            sky.SetFloat("_Exposure", Mathf.Lerp(0.6f, 1.8f, t));
            // Солнце в небе
            sky.SetFloat("_SunSize", 0.05f);          // размер солнца
            sky.SetColor("_SunColor", sunColorGradient.Evaluate(t));
        }
    }

    // Для мгновенного обновления при загрузке
    public void ForceUpdate()
    {
        OnTimeUpdated();
    }
}