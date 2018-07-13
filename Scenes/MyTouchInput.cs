
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MyTouchInput : MonoBehaviour
{

    public LayerMask touchInputMask;
    private List<GameObject> touchlist = new List<GameObject>();
    private GameObject[] touchesOld;
    private RaycastHit hit;


    public float speed = 0.1F;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("hhaa");
        
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            // Get movement of the finger since last frame
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;

            // Move object across XY plane
            transform.Translate(-touchDeltaPosition.x * speed, -touchDeltaPosition.y * speed, 0);

            Debug.Log(Input.GetTouch(0).position);
        }
           
        /*
      if (Input.touchCount > 0)
      {

          touchesOld = new GameObject[touchlist.Count];
          touchlist.CopyTo(touchesOld);
          touchlist.Clear();

          foreach (Touch touch in Input.touches)
          {
              Ray ray = camera.ScreenPointToRay(touch.position);
              RaycastHit hit;
              if (physics.Raycast(ray, out hit, touchInputMask))
              {
                  GameObject recipient = hit.transform.gameobject;
                  if (touch.phase == TouchPhase.began)
                  {
                      recipient.SendMessage("TouchDown", hit.point, SendMessageOptions.DontRequrieReceiver);
                  }
                  if (touch.phase == TouchPhase.Ended)
                  {
                      recipient.SendMessage("TouchUp", hit.point, SendMessageOptions.DontRequrieReceiver);
                  }
                  if (touch.phase == TouchPhase.Stationary || tocuh.phase == TouchPhase.Moved)
                  {
                      recipient.SendMessage("TouchStay", hit.point, SendMessageOptions.DontRequrieReceiver);
                  }
                  if (touch.phase == TouchPhase.Canceled)
                  {
                      recipient.SendMessage("TouchExit", hit.point, SendMessageOptions.DontRequrieReceiver);
                  }

              }
          }

          foreach (GameObject g in touchesOld)
          {
              if (!touchlist.Contains(g))
              {
                  g.SendMessage("OnTouchExit", hit.point, SendMessageOptions.DontRequireReceiver);
              }
          }



      }
          */
    }
}

