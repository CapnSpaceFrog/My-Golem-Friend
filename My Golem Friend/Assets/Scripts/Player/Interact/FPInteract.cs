using System;
using UnityEngine;

public class FPInteract : MonoBehaviour
{
    [Header("Interaction Variables")]
    public LayerMask InteractablesMask;
    public float InteractRayDistance;
    public float InputInteractCooldown;
    private float inputCooldownTimer;

    public static event Action OnIngStorageTableInteract;
    public static event Action OnCraftingStationInteract;
    public static event Action OnGolemInteract;

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
                    Interactable interObj = CastInteractRay();
                    if (interObj == null)
                    {
                        if (HeldObject != null && HeldObject.HoldableType == HoldableType.Ingredient)
                        {
                            HoldableIngredient heldIng = HeldObject.GetComponent<HoldableIngredient>();

                            heldIng.AddHoldableToPlayerInv();
                            FPInteract.HeldObject = null;
                        }
                    }
                    else
                    {
                        InteractButtonInput(interObj);
                    }
                    break;

                case InteractInput.LeftClick:
                    //TODO: Implement Dialogue Ability with golem here
                    if (HeldObject != null)
                    {
                        HeldObject.ThrowFromHand();
                    }
                    else
                    {
                        LeftClickInput(CastInteractRay());
                    }
                    break;

                case InteractInput.RightClick:
                    if (HeldObject != null)
                    {
                        HeldObject.DropFromHand();
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
            return;

        switch (hitObj.InterObjType)
        {
            case InteractableType.OverworldIngredient:
                OverworldIngredient ing = hitObj.gameObject.GetComponent<OverworldIngredient>();

                StorableIngredient storedIng = Inventory.CreateStorableIng(ing.IngType);

                if (Player.Inv.AddIngredient(storedIng, UISlotType.PlayerInv))
                {
                    ing.Harvest();
                }

                break;

            case InteractableType.StoredIngredient:
                //Add the Item to the Player's inventory, but not their hand
                IngredientType ingType = hitObj.gameObject.GetComponent<Ingredient>().IngType;

                IngredientTableManager.Instance.RemoveIngFromStorage(ingType);

                StorableIngredient newIng = Inventory.CreateStorableIng(ingType);

                Player.Inv.AddIngredient(newIng, UISlotType.PlayerInv);
                break;

            case InteractableType.IngredientStorageTable:
                OnIngStorageTableInteract?.Invoke();
                break;

            case InteractableType.CraftingStation:
                OnCraftingStationInteract?.Invoke();
                break;

            case InteractableType.Recipe:
                RecipePickup recipe = hitObj.GetComponent<RecipePickup>();

                CraftingHandler.Instance.FindRecipeToUnlock(recipe.RecipeToUnlock);
                Destroy(recipe.gameObject);
                break;

            case InteractableType.Holdable:
                //Pressing E on a holdable object adds it to the Player's inventory
                Holdable heldObj = hitObj.gameObject.GetComponent<Holdable>();

                switch (heldObj.HoldableType)
                {
                    case HoldableType.Ingredient:
                        HoldableIngredient heldIng = hitObj.gameObject.GetComponent<HoldableIngredient>();

                        heldIng.AddHoldableToPlayerInv();
                        break;

                    case HoldableType.GolemPart:
                        heldObj.AddToHand();
                        break;

                    case HoldableType.QuestItem:
                        heldObj.AddToHand();
                        break;
                }
                break;

            case InteractableType.Golem:

                if (HeldObject == null)
                    return;

                OnGolemInteract?.Invoke();
                break;
        }
    }

    private void LeftClickInput(Interactable hitObj)
    {
        if (hitObj == null)
            return;

        switch (hitObj.InterObjType)
        {
            case InteractableType.OverworldIngredient:
                OverworldIngredient ing = hitObj.gameObject.GetComponent<OverworldIngredient>();

                WorldObjectManager.Instance.InstantiateHoldableIngredient(Inventory.CreateStorableIng(ing.IngType));
                ing.Harvest();

                break;

            case InteractableType.StoredIngredient:
                //Left clicking the Ing Storage adds the item to the players hand
                IngredientType ingType = hitObj.gameObject.GetComponent<Ingredient>().IngType;

                IngredientTableManager.Instance.RemoveIngFromStorage(ingType);

                WorldObjectManager.Instance.InstantiateHoldableIngredient(Inventory.CreateStorableIng(ingType));
                break;

            case InteractableType.Recipe:
                RecipePickup recipe = hitObj.GetComponent<RecipePickup>();

                CraftingHandler.Instance.FindRecipeToUnlock(recipe.RecipeToUnlock);
                Destroy(recipe.gameObject);
                break;

            case InteractableType.Holdable:
                //Left clicking a holdable object that's on the ground puts it in the Player's hand
                Holdable heldObj = hitObj.gameObject.GetComponent<Holdable>();

                heldObj.AddToHand();
                break;
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawRay(FPCameraController.MainCamera.ScreenToWorldPoint(new Vector2(Screen.width / 2, Screen.height / 2)),
    //        FPCameraController.MainCamera.transform.forward * InteractRayDistance);
    //}
}