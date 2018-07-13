using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NewBehaviourScript : MonoBehaviour {

    public Color defaultColour;
    public Color selectedColour;
    private Material mat;

  /*

    void Start()
    {
        mat = GameObject.GetComponent<Renderer>().material.color;

    }
    */
    void TouchUp()
    {
        mat.color= defaultColour;

    }
    void TouchDown()
    {
        mat.color = selectedColour; 

    }
    void TouchExit()
    {
        mat.color = defaultColour;

    }
    void TouchStay()
    {
        mat.color = selectedColour;

    }

}
