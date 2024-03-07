using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
    [SerializeField] private int clientIndex;


    private void show(){
        gameObject.SetActive(true);
    }

    private void hide(){
        gameObject.SetActive(false);
    }
}
