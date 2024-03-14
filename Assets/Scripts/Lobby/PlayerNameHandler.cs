using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class PlayerNameHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentPlayerName;
    [SerializeField] private TMP_InputField changeNameInput;
    [SerializeField] private Button changeNameButton;
    [SerializeField] private Button randomNameButton;


    private int minimumNameCount = 2;
    private int maximumNameCount = 25;



    private void Start() {
        changeNameButton.onClick.AddListener(changeNameButtonClicked);
        randomNameButton.onClick.AddListener(() => {
            string newName = TerraNovaManager.Instance.chooseRandomName();
            changeName(newName);
        });

        currentPlayerName.text = TerraNovaManager.Instance.getPlayerName();
    }

    private void changeNameButtonClicked(){
        if(changeNameInput.text.Length >= minimumNameCount && changeNameInput.text.Length <= maximumNameCount){
            changeName(changeNameInput.text);
        }
    }

    private void changeName(string newName){
        currentPlayerName.text = newName;

        changeNameInput.text = "";

        TerraNovaManager.Instance.setPlayerName(newName);
    }


    


}
