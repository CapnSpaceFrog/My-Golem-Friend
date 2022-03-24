using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPCameraController : MonoBehaviour
{
    [SerializeField]
    private float vertClamp;

    private Transform m_CamTransform;

    public Transform characterBody;

    public static Transform CharacterBodyTransform;

    private float vertRotation;
    private float horRotation;

    public void Awake()
    {
        m_CamTransform = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;

        CharacterBodyTransform = characterBody;
    }

    public void Update()
    {
        RotateCamera();
    }

    private void RotateCamera()
    {
        vertRotation += -InputHandler.MouseDelta.y;
        horRotation += InputHandler.MouseDelta.x;

        vertRotation = Mathf.Clamp(vertRotation, -vertClamp, vertClamp);

        m_CamTransform.rotation = Quaternion.Euler(vertRotation, horRotation, 0);

        Vector3 bodyRotation = new Vector3(0, m_CamTransform.transform.localEulerAngles.y, 0);

        characterBody.localEulerAngles = bodyRotation;
    }
}

