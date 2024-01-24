using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tool", menuName = "New Tool")]
public class ToolGenericHandler : ScriptableObject
{
  [SerializeField] private ToolEnum.ToolType toolType;

  [SerializeField] private double damage;
  [SerializeField] private float debounceTime;

  //sprite


  public double getDamage(){
    return damage;
  }
  
  public float getDebounceTime(){
    return debounceTime;
  }

  public ToolEnum.ToolType getToolType(){
    return toolType;
  }
}
