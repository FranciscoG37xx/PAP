using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; 

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject carro;
    [SerializeField] private float distance = 5.0f;

    private Vector3 previousPosition;

    void Update()
    {
        //Se o cursor estiver sobre a UI, não faz nada
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        //Se clicou no botão esquerdo do rato, guarda a posição
        if (Input.GetMouseButtonDown(0))
        {
            previousPosition = cam.ScreenToViewportPoint(Input.mousePosition);
        }

        //Se estiver a manter pressionado, roda a câmara
        if (Input.GetMouseButton(0))
        {
            Vector3 direction = previousPosition - cam.ScreenToViewportPoint(Input.mousePosition);
            cam.transform.RotateAround(carro.transform.position, Vector3.up, -direction.x * 180);
            previousPosition = cam.ScreenToViewportPoint(Input.mousePosition);
        }
    }
}

