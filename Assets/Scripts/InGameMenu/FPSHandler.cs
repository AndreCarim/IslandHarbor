using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class FPSHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fpsText;
    private float deltaTime = 0.0f;

    private void Update()
    {
        if(gameObject.activeSelf)
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = $"FPS: {Mathf.Ceil(fps)}";
    }
}
