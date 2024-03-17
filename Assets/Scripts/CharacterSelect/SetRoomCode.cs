using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class SetRoomCode : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI code;
    [SerializeField] private Button copyButton; 
    [SerializeField] private GameObject copiedText;
    private bool canCopy = true;
    private int duration = 5;
    private void Start()
    {
        if(TerraNovaManager.Instance != null){
            code.text = TerraNovaManager.Instance.getRelayCode();
        }

        copyButton.onClick.AddListener(() => {
            if(canCopy){
                canCopy = false;
                 GUIUtility.systemCopyBuffer = code.text;
                 StartCoroutine(ActivateAndDeactivate());
            } 
        });

        copiedText.SetActive(false);
    }

    IEnumerator ActivateAndDeactivate()
    {
        // Activate the target object
        copiedText.SetActive(true);
        copyButton.gameObject.SetActive(false);
        // Wait for the specified duration
        yield return new WaitForSeconds(duration);

        // Deactivate the target object
        copiedText.SetActive(false);
        copyButton.gameObject.SetActive(true);
        canCopy = true;
    }
    
}
