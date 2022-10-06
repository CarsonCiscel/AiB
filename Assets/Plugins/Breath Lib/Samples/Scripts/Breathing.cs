using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Breathing : MonoBehaviour
{

 PlayerBreath BreathBar;


private bool isPowerUp = false;
private bool isDirectionUp = true;
private float breathChargeTimer = 0.0f;
private float maxBreath = 4.0f;

bool buttonHeldDown; 
bool maxInhale = false; 

   
public void Inhale()
{
        breathChargeTimer += Time.deltaTime;
        gameObject.GetComponent<PlayerBreath>().UpdateBreath(true);

        if(breathChargeTimer >= 4.0f)
        {
            maxInhale = true;  
            Debug.Log("maxInhale is true");
        }
    Debug.Log(breathChargeTimer);
    
}

public void Exhale()
{
    breathChargeTimer -= Time.deltaTime;
    gameObject.GetComponent<PlayerBreath>().UpdateBreath(false);

    //Debug.Log(breathChargeTimer);
}


 void Update() 
{

HoldButton();


if( buttonHeldDown && maxInhale)
{
Exhale();
}

else if(buttonHeldDown && !maxInhale)
{
Inhale();
}

ReleaseButton();

}


public void HoldButton()
{
    if(Input.GetKeyDown("space"))
    buttonHeldDown = true;
}

public void ReleaseButton()
{
    
    if(Input.GetKeyUp("space"))
    {
    buttonHeldDown = false;
    
    }
}


 public bool GetMaxInhale()
    {
        return maxInhale;
    }
/*
void PowerActive()
{
    if(isDirectionUp)
    {
        amtPower += Time.deltaTime * powerSpeed;
        if(amtPower > 100)
        {
            isDirectionUp = false;
            amtPower = 100.0f;
        }
    }
    else
    {
        amtPower -= Time.deltaTime * powerSpeed;
        if(amtPower < 0)
        { 
            isDirectionUp = true;
            amtPower = 0.0f;
        }
        
    }

    imagePowerUp.fillAmount = (0.483f - 0.25f) * amtPower / 100.0f + 0.25f;

}

public void StartPowerUp()
{
    isPowerUp = true;
    amtPower = 0.0f;
    isDirectionUp = true;
   

}


public void EndPowerUp()
{
    isPowerUp = false;
    
}
    */
}
