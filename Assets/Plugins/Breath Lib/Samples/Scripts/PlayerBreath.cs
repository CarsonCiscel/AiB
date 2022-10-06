using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BreathLib;
public class PlayerBreath : MonoBehaviour
{
  //current breath
   [SerializeField] private float breath = 0f;
   [SerializeField] private float maxBreath = 4.0f;

    public BreathBar breathBar;
    //values used to populate breathBar
    public BreathSample breathValues;

     private void Start() {
        //breath = maxBreath;
        breathBar.SetMaxBreath(maxBreath);
    }

    public void UpdateBreath(bool breathSwitch){
        
        if(breathValues.In.Value == 1.0)
        {
            breath += Time.deltaTime;
            Debug.Log("IN " + breath);
        }
        else if(breathValues.Out.Value == 1.0 )
        {
            breath -= Time.deltaTime;
             Debug.Log("OUT " + breath);
        }
       // else if(breathValues.No.Value == 1.0)
       // {
                        
       // }
        breathBar.SetBreath(breath);
      
    }

    

    public float GetBreath()
    {
        return breath;
    }
}
