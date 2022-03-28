using System;
using UnityEngine;

public class FPInteract : MonoBehaviour
{
    public LayerMask InteractablesMask;

    //This should be removed and the global static reference to the main camera from FPCameraController should be used instead
    public Camera m_Cam;

    public float InteractRayDistance;

    private float inputCooldownTimer;
    public float InputInteractCooldown;

    public static event Action OnStorageTableInteract;

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
        if (inputCooldownTimer > InputInteractCooldown)
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

        bool hitObj = Physics.Raycast(m_Cam.ScreenToWorldPoint(midPoint), m_Cam.transform.forward, out RaycastHit hit, InteractRayDistance, InteractablesMask);

        if (hitObj)
        {
            InvokeHandler(hit.transform.gameObject.GetComponent<Interactable>());
        }
    }

    //Checks what type of object the player touched and passes execution to the correct function
    private void InvokeHandler(Interactable hitObj)
    {
        switch (hitObj.InterObjType)
        {
            case InteractableType.OverworldIng:
                OverworldIngredient ing = hitObj.gameObject.GetComponent<OverworldIngredient>();

                Debug.Log("Overworld Ingredient Tocuhed: " + ing.IngType);
                OnOverworldIngInteract(ing);
                break;

            case InteractableType.IngStorage:
                IngStorage storedIng = hitObj.gameObject.GetComponent<IngStorage>();

                Debug.Log("Stored Ingredient Tocuhed: " + storedIng);
                OnStoredIngInteract(storedIng);
                break;

            case InteractableType.StorageTable:
                OnStorageTableInteract?.Invoke();

                Debug.Log("Storage Table Tocuhed.");
                break;

            case InteractableType.Holdable:
                Holdable heldObj = hitObj.gameObject.GetComponent<Holdable>();

                Debug.Log("Holdable Object Tocuhed: " + heldObj);
                OnHoldableInteract(heldObj);
                break;
        }
    }

    private void OnOverworldIngInteract(OverworldIngredient overworldIng)
    {
        if (overworldIng.Harvested)
        {
            //Display something that this ingredient has already been harvested, or disable the object
            print(overworldIng.IngType + " has already been harvested");
        }
        else
        {
            overworldIng.Harvest();
        }
    }

    private void OnStoredIngInteract(IngStorage storedIng)
    {
        
    }

    private void OnHoldableInteract(Holdable storageIngTable)
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(m_Cam.ScreenToWorldPoint(new Vector2(Screen.width / 2, Screen.height / 2)), m_Cam.transform.forward * InteractRayDistance);
    }
}