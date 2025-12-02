using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class FreeCameraMovment : MonoBehaviour
{
    public float Sensitivity = 1.0f;
    public float Speed = 1.0f;

    private Coroutine myCoroutine;

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            myCoroutine = StartCoroutine(UpdateCamera());
        }
        else if (Input.GetMouseButtonUp(1))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            StopCoroutine(myCoroutine);
        }
    }

    private IEnumerator UpdateCamera()
    {
        while (true)
        {
            RotateCam();
            MoveCam();
            yield return null;
        }

    }

    private void RotateCam()
    {
        Vector3 mousInput = new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0);
        transform.Rotate(mousInput * Sensitivity * Time.deltaTime * 50);
        Vector3 eulerRot = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(eulerRot.x, eulerRot.y, 0);
    }

    private void MoveCam()
    {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        transform.Translate(input * Speed * Time.deltaTime);
    }
}
