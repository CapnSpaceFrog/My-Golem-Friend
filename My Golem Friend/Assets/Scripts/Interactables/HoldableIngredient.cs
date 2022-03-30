public class HoldableIngredient : Holdable
{
    public StorableIngredient StoredIng;

    public void AddToInventory()
    {
        Player.Inv.AddIngredient(StoredIng, UISlotType.PlayerInv);
        WorldObjectManager.FlagObjectForDestruction(gameObject);

        gameObject.SetActive(false);

        gameObject.transform.parent = null;
    }
}
