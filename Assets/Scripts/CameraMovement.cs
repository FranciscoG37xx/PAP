using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject carro;
    [SerializeField] private float distance = 5.0f;
  
    private Vector3 previousPosition;

    void Start()
    {
        
    }

    
    void Update()
    {
   
      if(Input.GetMouseButtonDown(0))
      {
        previousPosition = cam.ScreenToViewportPoint(Input.mousePosition);
      }
      if (Input.GetMouseButton(0))
      {
        Vector3 direction = previousPosition - cam.ScreenToViewportPoint(Input.mousePosition);

        //cam.transform.RotateAround( carro.transform.position, Vector3.right, direction.y * 180);
        cam.transform.RotateAround( carro.transform.position, Vector3.up, -direction.x * 180 );

        previousPosition = cam.ScreenToViewportPoint(Input.mousePosition);
      }
    }
}
