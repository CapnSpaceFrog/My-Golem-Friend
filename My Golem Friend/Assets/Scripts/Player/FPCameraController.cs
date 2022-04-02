using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPCameraController : MonoBehaviour
{
    [SerializeField]
    private float vertClamp;

    public static Camera MainCamera { get; private set; }

    public Transform characterBody;

    public static Transform CharacterBodyTransform;

    private float vertRotation;
    private float horRotation;

    public void Awake()
    {
        MainCamera = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;

        CharacterBodyTransform = characterBody;
    }

    public void Update()
    {
        
    }

    private void LateUpdate()
    {
        RotateCamera();
    }

    private void RotateCamera()
    {
        vertRotation += -InputHandler.MouseDelta.y;
        horRotation += InputHandler.MouseDelta.x;

        vertRotation = Mathf.Clamp(vertRotation, -vertClamp, vertClamp);

        MainCamera.transform.rotation = Quaternion.Euler(vertRotation, horRotation, 0);

        Vector3 bodyRotation = new Vector3(0, MainCamera.transform.localEulerAngles.y, 0);

        characterBody.localEulerAngles = bodyRotation;
    }
}

