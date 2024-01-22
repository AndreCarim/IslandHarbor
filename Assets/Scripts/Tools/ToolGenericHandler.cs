using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tool", menuName = "New Tool")]
public class ToolGenericHandler : ScriptableObject
{
  [SerializeField] private ToolHandler.ToolType toolType;

  [SerializeField] private double damage;
  [SerializeField] private float debounceTime;


  public double getDamage(){
    return damage;
  }
  
  public float getDebounceTime(){
    return debounceTime;
  }

  public ToolHandler.ToolType getToolType(){
    return toolType;
  }
}
