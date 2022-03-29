using UnityEngine;

public class OverworldIngredient : Ingredient
{
    MeshFilter meshFilter;
    public Mesh HarvestedMesh;

    [HideInInspector]
    public bool Harvested;

    public override void Awake()
    {
        base.Awake();

        meshFilter = GetComponent<MeshFilter>();
    }

    public void Harvest()
    {
        StorableIngredient storedIng = Inventory.CreateStorableIng(this);

        if (Player.Inv.AddIngredient(storedIng))
        {
            Harvested = true;
            meshFilter.mesh = HarvestedMesh;
            Collider.enabled = false;
            return;
        }
        else
        {
            //Display something about the Player's inventory being full
            Debug.Log("Player Inventory is full!");
        }
    }
}
