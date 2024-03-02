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
    private const float STARTING_HOUR = 7f; // It will start at 7 am
    private const float HOW_LONG_IS_THE_DAY_IN_MINUTES = .5f; // Change here for how long you want the day to last in minutes.
    private float secondsPerDay;
    private float elapsedTime = 0f;
    private NetworkVariable<int> currentHour = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<int> currentMinute = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<int> currentSecond = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<int> currentDay = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

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
            Debug.LogWarning("DayCycleHandler component is not enabled.");
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
    }

    private string GetAmPm(int hour)
    {
        if (hour >= 12)
            return "PM";
        else
            return "AM";
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
}
