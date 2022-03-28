using System;
using UnityEngine;

public class FPInteract : MonoBehaviour
{
    public static Holdable HeldObject = null;

    [Header("Interaction Variables")]
    public LayerMask InteractablesMask;
    public float InteractRayDistance;
    public float InputInteractCooldown;
    private float inputCooldownTimer;

    public static event Action OnStorageTableInteract;

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

        bool hitObj = Physics.Raycast(FPCameraController.MainCamera.ScreenToWorldPoint(midPoint), 
            FPCameraController.MainCamera.transform.forward, out RaycastHit hit, InteractRayDistance, InteractablesMask);

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
            //Functionality is completed, just need to set Meshes for harvested versions
            case InteractableType.OverworldIng:
                OverworldIngredient ing = hitObj.gameObject.GetComponent<OverworldIngredient>();

                Debug.Log("Overworld Ingredient Tocuhed: " + ing.IngType);
                OnOverworldIngInteract(ing);
                break;

            case InteractableType.IngStorage:
                StoredIngredient storedIng = hitObj.gameObject.GetComponent<StoredIngredient>();

                Debug.Log("Stored Ingredient Tocuhed: " + storedIng);
                OnStoredIngInteract(storedIng);
                break;

                //Functionality is completed
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

    //TODO: OnOverworldIngInteract Update
    private void OnOverworldIngInteract(OverworldIngredient overworldIng)
    {
        if (overworldIng.Harvested)
        {
            //This part needs to be removed and when the obj is harvested it is disabled automatically
            print(overworldIng.IngType + " has already been harvested");
        }
        else
        {
            overworldIng.Harvest();
        }
    }

    private void OnStoredIngInteract(StoredIngredient storedIng)
    {
        
    }

    private void OnHoldableInteract(Holdable storageIngTable)
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(FPCameraController.MainCamera.ScreenToWorldPoint(new Vector2(Screen.width / 2, Screen.height / 2)), 
            FPCameraController.MainCamera.transform.forward * InteractRayDistance);
    }
}