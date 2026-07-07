using UnityEngine;

[CreateAssetMenu(fileName = "DayNightSO", menuName = "Scriptable Objects/DayNightSO")]
public class DayNightSO : ScriptableObject
{
    public bool UpdateMinutes;
    public bool UpdateHours;
    public bool UpdateDays;
    public int StartHourByCreatedWorld;
    public int HoursInDay = 24;
    public int MinutesInHour = 60;
    public float RealSecondsInGameMinutes = 1f;
    public int HourStartNight = 1;
    public int HourStartSunny = 12;
}