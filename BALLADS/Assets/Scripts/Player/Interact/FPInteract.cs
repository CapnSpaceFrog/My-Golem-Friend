using System;
using UnityEngine;

public class FPInteract : MonoBehaviour
{
    public LayerMask interactMask;

    //Remember to private this when done debugging
    public Camera m_Cam;

    public float interRayDistance;

    private float inputCooldownTimer;
    public float inputCooldown;

    public static event Action<string[]> OnHoldableHit;

    public static Holdable HeldObject = null;

    public void Awake()
    {
        m_Cam = Camera.main;
    }

    public void OnEnable()
    {
        InputHandler.OnInteractInput += InteractInputCheck;
        InputHandler.OnPlaceInput += OnPlaceInputCheck;
        InputHandler.OnThrowInput += OnThrowInputCheck;
    }

    public void Update()
    {
        inputCooldownTimer += Time.deltaTime;
    }

    private bool CheckInputTimer()
    {
        if (inputCooldownTimer > inputCooldown)
        {
            inputCooldownTimer = 0;
            return true;
        }

        return false;
    }

    private void InteractInputCheck()
    {
        if (CheckInputTimer())
        {
            CastInteractRay();
        }
    }

    private void OnPlaceInputCheck()
    {
        if (HeldObject != null && CheckInputTimer())
        {
            //Are we looking at an object's place space?

            //Aren't looking at an object's place space, so drop it
            HeldObject.Drop();
        }
    }

    private void OnThrowInputCheck()
    {
        if (HeldObject != null && CheckInputTimer())
        {
            HeldObject.Throw();
        }
    }

    private void CastInteractRay()
    {
        Vector2 midPoint = new Vector2(Screen.width / 2, Screen.height / 2);

        bool hitObj = Physics.Raycast(m_Cam.ScreenToWorldPoint(midPoint), m_Cam.transform.forward, out RaycastHit hit, interRayDistance, interactMask);

        if (hitObj)
        {
            InvokeHandler(hit.transform.gameObject.GetComponent<Interactable>());
        }
    }

    //Invokes the proper event according to the objects type.
    //Event is received by a particular handler to handle the object's behavior accordingly
    private void InvokeHandler(Interactable hitObj)
    {
        switch (hitObj.InterObjType)
        {
            case InteractableType.Holdable:
                Holdable heldObj = hitObj.gameObject.GetComponent<Holdable>();

                OnHoldableInteract(heldObj);
                break;

            default:
                print("ERROR: Didn't get an object type in the switch statement in FPInteract.");
                break;
        }
    }

    private void OnHoldableInteract(Holdable obj)
    {
        if (obj.Exhausted)
        {
            //No need to display dialogue again, just pick up the object
            obj.PickedUp();
        }
        else
        {
            

            
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(m_Cam.ScreenToWorldPoint(new Vector2(Screen.width / 2, Screen.height / 2)), m_Cam.transform.forward * interRayDistance);
    }
}