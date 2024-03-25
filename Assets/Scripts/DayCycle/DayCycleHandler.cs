using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.Netcode;

public class DayCycleHandler : NetworkBehaviour
{
    // Set the starting hour as a variable
    private const float STARTING_HOUR = 7f; // change here to set the starting time
    private const float HOW_LONG_IS_THE_DAY_IN_MINUTES = 5f; // Change here for how long you want the day to last in minutes.
    private const float SUNRISE_START = 6f;
    private const float SUNSET_START = 18f;
    private const float NIGHT_START = 20f;//change here to set when the night should start
    private NetworkVariable<bool> isNight = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private float secondsPerDay;
    private float elapsedTime = 0f;
    private NetworkVariable<int> currentHour = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<int> currentMinute = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<int> currentSecond = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<int> currentDay = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

   [SerializeField] private GameObject anouncementObject;
   [SerializeField] private TextMeshProUGUI anouncementText;
    private float howLongTextShow = 3f; // Duration for which the object will be shown in seconds

    
    #region daynight color change
    //colors
    private Color nightFogColor;
    private Color nightAmbientColor;
    private Color nightDirectionLightColor;
    private Color nightSkyboxColor;

    private Color dayFogColor;
    private Color dayAmbientColor;
    private Color dayDirectionLightColor;
    private Color daySkyboxColor;

    private Color sunsetFogColor;
    private Color sunsetAmbientColor;
    private Color sunsetDirectionLightColor;
    private Color sunsetSkyboxColor;

    private Color sunriseFogColor;
    private Color sunriseAmbientColor;
    private Color sunriseDirectionLightColor;
    private Color sunriseSkyboxColor;

    //colors

    [Header("Enviromental Assets")]
    [SerializeField] private Light directionalLight;
    [SerializeField] private Material skyboxMaterial;

    [SerializeField] private float rotationSpeed = .5f;


    // Calculate the total duration of each phase
    float dayDuration;
    float sunsetDuration;
    float sunriseDuration;
    float nightDuration;

    public int transitionPercentage = 80;
    // Declare timeElapsed variable outside the method
    private float timeElapsedNight = 0f;
    #endregion

    public override void OnNetworkSpawn()
    {
        if(IsOwner){
            nightFogColor = new Color(0.05f, 0.05f, 0.1f, 1f); // Dark blue
            nightAmbientColor = new Color(0.05f, 0.05f, 0.1f, 1f); // Dark blue
            nightDirectionLightColor = Color.blue; // Blue
            nightSkyboxColor = new Color(0.05f, 0.05f, 0.1f, 1f); // Dark blue

            dayFogColor = new Color(0.6f, 0.8f, 1f, 1f); // Light blue
            dayAmbientColor = new Color(0.6f, 0.8f, 1f, 1f); // Light blue
            dayDirectionLightColor = Color.white; // White
            daySkyboxColor = new Color(0.6f, 0.8f, 1f, 1f); // Light blue

            sunsetFogColor = new Color(1f, 0.5f, 0.2f, 1f); // Orange
            sunsetAmbientColor = new Color(1f, 0.5f, 0.2f, 1f); // Orange
            sunsetDirectionLightColor = new Color(1f, 0.5f, 0.2f, 1f); // Orange
            sunsetSkyboxColor = new Color(1f, 0.5f, 0.2f, 1f); // Orange

            sunriseFogColor = new Color(1f, 0.5f, 0.2f, 1f); // Orange
            sunriseAmbientColor = new Color(1f, 0.5f, 0.2f, 1f); // Orange
            sunriseDirectionLightColor = new Color(1f, 0.5f, 0.2f, 1f); // Orange
            sunriseSkyboxColor = new Color(1f, 0.5f, 0.2f, 1f); // Orange

            sunriseDuration = STARTING_HOUR - SUNRISE_START;
            sunsetDuration = NIGHT_START - SUNSET_START;
            dayDuration = SUNSET_START - STARTING_HOUR;
            nightDuration = getPhaseDuration(SUNRISE_START, NIGHT_START);

        }

        if (!IsServer) return;
        currentDay.Value = 1;

        secondsPerDay = HOW_LONG_IS_THE_DAY_IN_MINUTES * 60f;
    }

    void Update()
    {
        if (!IsServer) return;

        elapsedTime += Time.deltaTime;
        if (elapsedTime >= secondsPerDay)
        {
            currentDay.Value ++;
            elapsedTime -= secondsPerDay;
            handleDayStartClientRpc(currentDay.Value);
        }


        UpdateTimeServerRpc();

         // Update environmental assets based on the time of day
        UpdateDayNightCycle();
        RotateSkybox();
    }

    // It needs to be ServerRpc otherwise we would get an error, since ServerRpc are only 
    // replicated on the server instance of this script, so the clients won't even have this. 
    [ServerRpc]
    private void UpdateTimeServerRpc()
    {
        // Check if the DayCycleHandler component is enabled
        if (!enabled || !gameObject)
        {
            Debug.LogWarning("DayCycleHandler component is not enabled or not available at the scene");
            return;
        }

        // Check if any of the required variables are null
        if (currentHour == null || currentMinute == null || currentSecond == null)
        {
            // Handle the null case
            Debug.LogWarning("One or more network variables is null in UpdateTime.");
            return;
        }

        // Calculate normalized time
        float normalizedTime = elapsedTime / secondsPerDay;
        // Calculate current hour based on starting hour
        currentHour.Value = Mathf.FloorToInt((normalizedTime * 24f + STARTING_HOUR) % 24f);
        // Calculate remaining minutes after calculating hours
        float remainingMinutes = (normalizedTime * 24f - Mathf.FloorToInt(normalizedTime * 24f)) * 60f;
        // Ensure minutes do not exceed 60 and reset appropriately
        currentMinute.Value = Mathf.FloorToInt(remainingMinutes) % 60;
        // Calculate current second
        currentSecond.Value = Mathf.FloorToInt((remainingMinutes - currentMinute.Value) * 60f);

        // Check if the current hour is within a tolerance of NIGHT_START (10 pm)
        if (Mathf.Abs(currentHour.Value - NIGHT_START) < 0.1f && isNight.Value == false)
        {
            isNight.Value = true;
            HandleNightStartClientRpc();
        }
        // Check if the current hour is within a tolerance of NIGHT_END (7 am)
        else if (Mathf.Abs(currentHour.Value - SUNRISE_START) < 0.1f && isNight.Value == true)
        {
            isNight.Value = false;
            HandleNightEndClientRpc();
        }
    }

    [ClientRpc]
    private void HandleNightStartClientRpc()
    {
        //if(!IsOwner)return;

        anouncementText.text = "Night Started";
        StartCoroutine(ShowObjectCoroutine());
        // This method will be called when the clock hits NIGHT_START
    }

    [ClientRpc]
    private void HandleNightEndClientRpc()
    {
        // This method will be called when the clock hits NIGHT_END 
    }
    
    [ClientRpc]
    private void handleDayStartClientRpc(int day){
        //if(!IsOwner)return;

        anouncementText.text = "Day " + day.ToString();
        StartCoroutine(ShowObjectCoroutine());
    }
    

    private void UpdateDayNightCycle()
    {
        float t = 0f;

        if (directionalLight && skyboxMaterial)
        {

            // Calculate current time in minutes
            float currentHourValue = currentHour.Value + currentMinute.Value / 60f;

            // Calculate the transition time within each phase
            float transitionTime = 0f;
            float phaseDuration = 0f;

            if (currentHourValue >= NIGHT_START || currentHourValue < SUNRISE_START)
            {
                // Night phase
                // Debug.Log("Current Phase: Night");

                phaseDuration = nightDuration;
                float transitionStart = phaseDuration * (80f / 100f); // Calculate the time at which the transition should start

                // Calculate the time elapsed since NIGHT_START
                timeElapsedNight = currentHourValue >= NIGHT_START ? currentHourValue - NIGHT_START : (24f - NIGHT_START) + currentHourValue;

                // Ensure timeElapsedNight doesn't exceed phase duration
                timeElapsedNight = Mathf.Clamp(timeElapsedNight, 0f, phaseDuration);

                // Calculate the fraction of the phase duration that has elapsed
                float phaseFraction = Mathf.Clamp01(timeElapsedNight / phaseDuration);

                // If the current time is before the transition start time, t is 0
                // If the current time is after the transition start time, smoothly increase t from 0 to 1 until the end of the phase
                if (timeElapsedNight < transitionStart)
                {
                    t = 0f; // Transition hasn't started yet
                }
                else
                {
                    // Calculate the fraction of the remaining time covered by the transition
                    float transitionFraction = Mathf.Clamp01((timeElapsedNight - transitionStart) / (phaseDuration - transitionStart));

                    // Smoothly interpolate t from 0 to 1 during the transition period
                    t = Mathf.SmoothStep(0f, 1f, transitionFraction); // Transition is in progress
                }

                // Interpolate colors and update environmental assets only if transition has started and phaseFraction is less than or equal to 1
                if (timeElapsedNight >= transitionStart && phaseFraction <= 1f)
                {
                    InterpolateColorsAndSetAssets(nightFogColor, sunriseFogColor, nightAmbientColor, sunriseAmbientColor, nightDirectionLightColor, sunriseDirectionLightColor, nightSkyboxColor, sunriseSkyboxColor, t);
                }

                //Debug.Log($"phaseDuration: {phaseDuration}, timeElapsed: {timeElapsedNight}");
            }
            else if (currentHourValue >= SUNSET_START && currentHourValue < NIGHT_START)
            {
                // Sunset phase
                //Debug.Log("Current Phase: Sunset");

                phaseDuration = sunsetDuration;
                transitionTime = phaseDuration * (transitionPercentage / 100f);

                // Calculate time elapsed since SUNSET_START
                float timeElapsed = currentHourValue - SUNSET_START;
                if (timeElapsed < 0f) // Adjust for time wrapping around from 24 to 0
                {
                    timeElapsed += 24f;
                }

                t = Mathf.Clamp01((timeElapsed - transitionTime) / (phaseDuration - transitionTime));

                // Interpolate colors and update environmental assets
                InterpolateColorsAndSetAssets(sunsetFogColor, nightFogColor, sunsetAmbientColor, nightAmbientColor, sunsetDirectionLightColor, nightDirectionLightColor, sunsetSkyboxColor, nightSkyboxColor, t);
            }
            else if (currentHourValue >= STARTING_HOUR && currentHourValue < SUNSET_START)
            {
                // Day phase
                //Debug.Log("Current Phase: Day");

                phaseDuration = dayDuration;
                transitionTime = phaseDuration * (transitionPercentage / 100f);

                // Calculate time elapsed since STARTING_HOUR
                float timeElapsed = currentHourValue - STARTING_HOUR;
                if (timeElapsed < 0f) // Adjust for time wrapping around from 24 to 0
                {
                    timeElapsed += 24f;
                }

                t = Mathf.Clamp01((timeElapsed - transitionTime) / (phaseDuration - transitionTime));

                // Interpolate colors and update environmental assets
                InterpolateColorsAndSetAssets(dayFogColor, sunsetFogColor, dayAmbientColor, sunsetAmbientColor, dayDirectionLightColor, sunsetDirectionLightColor, daySkyboxColor, sunsetSkyboxColor, t);
            }
            else if (currentHourValue >= SUNRISE_START && currentHourValue < STARTING_HOUR)
            {
                // Sunrise phase
               // Debug.Log("Current Phase: Sunrise");

                phaseDuration = sunriseDuration;
                transitionTime = phaseDuration * (transitionPercentage / 100f);

                // Calculate time elapsed since SUNRISE_START
                float timeElapsed = currentHourValue - SUNRISE_START;
                if (timeElapsed < 0f) // Adjust for time wrapping around from 24 to 0
                {
                    timeElapsed += 24f;
                }

                t = Mathf.Clamp01((timeElapsed - transitionTime) / (phaseDuration - transitionTime));

                // Interpolate colors and update environmental assets
                InterpolateColorsAndSetAssets(sunriseFogColor, dayFogColor, sunriseAmbientColor, dayAmbientColor, sunriseDirectionLightColor, dayDirectionLightColor, sunriseSkyboxColor, daySkyboxColor, t);
            }
        }
       // Debug.Log(t);
    } 



    // Method to interpolate colors and update assets
    private void InterpolateColorsAndSetAssets(Color startFogColor, Color endFogColor, Color startAmbientColor, Color endAmbientColor, Color startDirectionLightColor, Color endDirectionLightColor, Color startSkyboxColor, Color endSkyboxColor, float t)
    {
        Color fogColor = Color.Lerp(startFogColor, endFogColor, t);
        Color ambientColor = Color.Lerp(startAmbientColor, endAmbientColor, t);
        Color directionLightColor = Color.Lerp(startDirectionLightColor, endDirectionLightColor, t);
        Color skyboxColor = Color.Lerp(startSkyboxColor, endSkyboxColor, t);

        RenderSettings.fogColor = fogColor;
        RenderSettings.ambientLight = ambientColor;
        directionalLight.color = directionLightColor;
        skyboxMaterial.SetColor("_Tint", skyboxColor);
    }

   

    private float getPhaseDuration(float finalTime, float initialTime)
    {
        float result = finalTime - initialTime;
        if (result < 0)
        {
            result += 24f; // Adding 24 hours to convert negative result to positive
        }
        return result;
    }



    private void RotateSkybox()
    {
        if(skyboxMaterial){
            float currentRotation = skyboxMaterial.GetFloat("_Rotation");
            float newRotation = currentRotation + rotationSpeed * Time.deltaTime;
            newRotation = Mathf.Repeat(newRotation, 360f);
            skyboxMaterial.SetFloat("_Rotation", newRotation);
        } 
    }

    private string GetAmPm(int hour)
    {
        if (hour >= 12)
            return "PM";
        else
            return "AM";
    }


    IEnumerator ShowObjectCoroutine()
    {
        // Activate the object
        anouncementObject.SetActive(true);

        // Wait for the specified duration
        yield return new WaitForSeconds(howLongTextShow);

        // Deactivate the object
        anouncementObject.SetActive(false);
    }

   

    private string FormatTime(int hour, int minute)
    {
        return string.Format("{0:D2}:{1:D2}", hour % 12 == 0 ? 12 : hour % 12, minute);
    }

    public string getTime()
    {
        return FormatTime(currentHour.Value, currentMinute.Value) + " " + GetAmPm(currentHour.Value);
    }

    public int getCurrentDay(){
        return currentDay.Value;
    }

    public bool getIsNight(){
        return isNight.Value;
    }

    
}