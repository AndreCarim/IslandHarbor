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
    private const float HOW_LONG_IS_THE_DAY_IN_MINUTES = 30f; // Change here for how long you want the day to last in minutes.
    private const float NIGHT_START = 22f;//change here to set when the night should start
    private const float NIGHT_END = 7f;//change here to set when the night will end
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

    public override void OnNetworkSpawn()
    {
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

        // Check if the current hour matches the night start (10 pm)
        if (currentHour.Value == Mathf.FloorToInt(NIGHT_START) && isNight.Value == false)
        {
            isNight.Value = true;
            HandleNightStartClientRpc();
        }
        // Check if the current hour matches the night end (7 am)
        else if (currentHour.Value == Mathf.FloorToInt(NIGHT_END) && isNight.Value == true)
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
