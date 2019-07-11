using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Giroscopio : MonoBehaviour
{
    Gyroscope gyro;
    public GameObject cube;
    public float speedMod;
    Vector2 startPos;
    float minSwipeDist = -100f;

    //Raycast
    Ray ray;
    RaycastHit hit;
    GameObject touchedObject;

    //GameManager
    public GameObject gameManager;
    GameSystem GSScript;

    public bool canSwipeStick;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager");
        Input.gyro.enabled = true;
        gyro = Input.gyro;
        GSScript = gameManager.GetComponent<GameSystem>();
        canSwipeStick = true;
    }

    private void MoveAndDestroy(GameObject objectToMove)
    {
        print("Entré al MoveAndDestroy del Giroscopio");
        canSwipeStick = false;
        objectToMove.transform.Translate(-touchedObject.transform.right * 8);
        Destroy(objectToMove, 2f);
        GSScript.StickPicked();
    }

    private void Update()
    {
        transform.LookAt(cube.transform);
        //print(Input.gyro.rotationRate);
        //print(Input.touchCount);
        //print("CAN SWIPE STICK: " + canSwipeStick);

        if (Input.touchCount == 0)
        {
            if (gyro.rotationRate.y > 0.05)
            {
                transform.RotateAround(cube.transform.position, Vector3.up, -Time.deltaTime * speedMod);
            }
            if (gyro.rotationRate.y < -0.05)
            {
                transform.RotateAround(cube.transform.position, Vector3.up, Time.deltaTime * speedMod);
            }
        }
        else if (Input.touchCount == 2 && canSwipeStick)  //&& Input.GetTouch(1).phase == TouchPhase.Began
        {
            bool moveObject = false;

            switch (Input.GetTouch(1).phase)
            {
                case TouchPhase.Began:
                    startPos = Input.GetTouch(1).position;

                    ray = Camera.main.ScreenPointToRay(Input.GetTouch(1).position);
                    Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow, 100f);
                    break;


                case TouchPhase.Ended:
                    float swipeDist = Input.GetTouch(1).position.y - startPos.y;

                    print(swipeDist);

                    if (swipeDist < minSwipeDist)
                    {
                        moveObject = true;
                        print("ME MUEVOOOOOOO");
                    }
                    break;
            }

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.transform.name);
                if (hit.collider != null && moveObject)
                {
                    touchedObject = hit.transform.gameObject;
                    Debug.Log("Touched " + touchedObject.transform.name);

                    if (touchedObject.tag == "Stick" && canSwipeStick)
                    {
                        MoveAndDestroy(touchedObject);
                    }
                    
                }
            }
        }
    }
}
