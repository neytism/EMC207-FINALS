using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Vector3 initialPosition;
    public Vector3 initialRotation;
    public float moveSpeed = 10f;
    public float rotationSpeed = 2f;

    public Transform drone;
    
    [SerializeField] private float _slowMotionFactor = 0.5f;
    
    private float xRot;
    private float yRot;

    private bool _canControl = true;

    

    private void Start()
    {
        transform.position = initialPosition;
        transform.rotation = Quaternion.Euler(initialRotation);
    }

    void Update()
    {
        CursorHandler();
        if (!_canControl) return;
        
        // Translation
        Vector3 translation = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("UpDown"), Input.GetAxis("Vertical"));
        translation.Normalize(); // Normalize diagonal movement
        transform.Translate(translation * (moveSpeed * Time.deltaTime));

        // Rotation
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        xRot += mouseX * rotationSpeed;
        yRot += mouseY * rotationSpeed;

        if (yRot > 360 || yRot < -360) yRot = 0;
        if (xRot > 360 || xRot < -360) xRot = 0;
        
        transform.localRotation = Quaternion.Euler(-yRot, xRot, 0f);

        UnnecessaryBullshit();

        SlowMotion();

    }
    
    private void CursorHandler()
    {
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            _canControl = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            _canControl = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void UnnecessaryBullshit()
    {
        drone.rotation = Quaternion.Euler(transform.rotation.x * -1, transform.rotation.y * -1, transform.rotation.z * -1);
    }

    private void SlowMotion()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = _slowMotionFactor;
            UIManager.Instance.ToggleSlowVignette(true);
        }
        
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Time.timeScale = 1f;
            UIManager.Instance.ToggleSlowVignette(false);
        }
        
    }

}
