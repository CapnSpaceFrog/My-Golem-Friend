using UnityEngine;

public class HoldableIngredient : Holdable
{
    public StorableIngredient StoredIng;

    public void AddHoldableToPlayerInv()
    {
        if (Player.Inv.AddIngredient(StoredIng, UISlotType.PlayerInv))
        {
            Collider.enabled = false;

            WorldObjectManager.FlagObjectForDestruction(gameObject);
            
            gameObject.SetActive(false);

            transform.parent = null;
        }
    }

    public void DropOnFloor()
    {
        Collider.enabled = true;

        gameObject.SetActive(true);

        transform.parent = null;

        transform.position = Player.PlayerTransform.position + (Player.CharacterBody.transform.forward * 0.75f);

        RB.constraints = RigidbodyConstraints.None;
    }
}
