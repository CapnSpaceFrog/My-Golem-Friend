using System;
using UnityEngine;

public class FPInteract : MonoBehaviour
{
    [Header("Interaction Variables")]
    public LayerMask InteractablesMask;
    public float InteractRayDistance;
    public float InputInteractCooldown;
    private float inputCooldownTimer;

    public static event Action OnStorageTableInteract;

    [Header("Holdable Variables")]
    public static Holdable HeldObject = null;

    public void OnEnable()
    {
        InputHandler.OnInteractInput += InteractInputCheck;
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

    private void InteractInputCheck(InteractInput inputType)
    {
        if (CheckInputTimer())
        {
            switch (inputType)
            {
                case InteractInput.E:
                    InteractButtonInput(CastInteractRay());
                    break;

                case InteractInput.LeftClick:
                    if (HeldObject != null)
                    {
                        HeldObject.Throw();
                    }
                    else
                    {
                        LeftClickInput(CastInteractRay());
                    }
                    break;

                case InteractInput.RightClick:
                    if (HeldObject != null)
                    {
                        HeldObject.Drop();
                    }
                    break;
            }
        }
    }

    private Interactable CastInteractRay()
    {
        Vector2 midPoint = new Vector2(Screen.width / 2, Screen.height / 2);

        bool hitObj = Physics.Raycast(FPCameraController.MainCamera.ScreenToWorldPoint(midPoint),
            FPCameraController.MainCamera.transform.forward, out RaycastHit hit, InteractRayDistance, InteractablesMask);

        if (hitObj)
        {
            return hit.transform.gameObject.GetComponent<Interactable>();
        }
        else
        {
            return null;
        }
    }

    private void InteractButtonInput(Interactable hitObj)
    {
        if (hitObj == null)
        {
            return;
        }

        switch (hitObj.InterObjType)
        {
            case InteractableType.OverworldIngredient:
                OverworldIngredient ing = hitObj.gameObject.GetComponent<OverworldIngredient>();

                if (!ing.Harvested)
                {
                    ing.Harvest();
                }

                break;

            case InteractableType.StoredIngredient:
                //Add the Item to the Player's inventory, but not their hand
                IngredientType ingType = hitObj.gameObject.GetComponent<Ingredient>().IngType;

                IngredientTableManager.Instance.RemoveIngFromStorage(ingType);

                StorableIngredient newIng = Inventory.CreateStorableIng(ingType);

                Player.Inv.AddIngredient(newIng);
                break;

            case InteractableType.IngredientStorageTable:
                OnStorageTableInteract?.Invoke();
                break;

            case InteractableType.Holdable:
                //Pressing E on a holdable object adds it to the Player's inventory
                Holdable heldObj = hitObj.gameObject.GetComponent<Holdable>();

                if (heldObj.HoldableType == HoldableType.Ingredient)
                {
                    HoldableIngredient heldIng = hitObj.gameObject.GetComponent<HoldableIngredient>();

                    Player.Inv.AddIngredient(heldIng.StoredIng); 
                    WorldObjectManager.FlagObjectForDestruction(hitObj.gameObject);
                    hitObj.gameObject.SetActive(false);
                }
                
                break;
        }
    }

    private void LeftClickInput(Interactable hitObj)
    {
        if (hitObj == null)
        {
            return;
        }

        switch (hitObj.InterObjType)
        {
            case InteractableType.StoredIngredient:
                //Left clicking the Ing Storage adds the item to the players hand
                IngredientType ingType = hitObj.gameObject.GetComponent<Ingredient>().IngType;

                IngredientTableManager.Instance.RemoveIngFromStorage(ingType);

                WorldObjectManager.Instance.InstantiateHoldableIngredient(Inventory.CreateStorableIng(ingType));
                break;

            case InteractableType.Holdable:
                //Left clicking a holdable object that's on the ground puts it in the Player's hand
                Holdable heldObj = hitObj.gameObject.GetComponent<Holdable>();

                heldObj.PickedUp();
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(FPCameraController.MainCamera.ScreenToWorldPoint(new Vector2(Screen.width / 2, Screen.height / 2)),
            FPCameraController.MainCamera.transform.forward * InteractRayDistance);
    }
}