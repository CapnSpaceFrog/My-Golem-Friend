using UnityEngine;

public class HoldableIngredient : Holdable
{
    public StorableIngredient StoredIng;

    public void AddHoldableToPlayerInv()
    {
        if (Player.Inv.AddIngredient(StoredIng, UISlotType.PlayerInv))
        {
            WorldObjectManager.FlagObjectForDestruction(gameObject);

            gameObject.SetActive(false);

            transform.parent = null;
            FPInteract.HeldObject = null;
        }
    }

    public void DropOnFloor()
    {
        Collider.enabled = true;

        gameObject.SetActive(true);

        transform.parent = null;

        RB.constraints = RigidbodyConstraints.None;

        transform.position = new Vector3(Player.PlayerTransform.position.x,
            Player.PlayerTransform.position.y,
            FPCameraController.MainCamera.transform.position.z + 0.5f);
    }
}
