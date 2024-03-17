using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class OptionsHandler : MonoBehaviour
{
    [SerializeField] private GameObject optionMenu;
    [SerializeField] private Button openOptionsButton;
    [SerializeField] private Button closeOptionsButton;

    //mouseSensitivity
    [SerializeField] private Slider sliderMouseSensitivity;
    [SerializeField] private TextMeshProUGUI sliderNumber;
    [SerializeField] private CameraInput cameraInput;
   
    //mouseSensitivity

    //FPS
    [SerializeField] private Button fpsButton;
    [SerializeField] private GameObject fpsText;
    //FPS

    

    private void Start() {
        openOptionsButton.onClick.AddListener(show);
        closeOptionsButton.onClick.AddListener(hide);
        fpsButton.onClick.AddListener(fpsButtonClicked);
        

        sliderMouseSensitivity.maxValue = 100f;
        sliderMouseSensitivity.minValue = 1f;

        sliderMouseSensitivity.onValueChanged.AddListener(adjustMouseSensitivity);
    }

    private void adjustMouseSensitivity(float value){
       

        cameraInput.setMouseSensitivity(value);
        sliderNumber.text = (value / 10).ToString("0.0");
    }

    private void fpsButtonClicked(){
        if(fpsText.activeSelf){
            //is already open, close
            fpsText.SetActive(false);
        }else{
            fpsText.SetActive(true);
        }
    }


    private void show(){
        optionMenu.SetActive(true); 

        sliderMouseSensitivity.value = cameraInput.getMouseSensitivy();
    }

    private void hide(){
        optionMenu.SetActive(false);
    }
}
