using UnityEngine;

public class OverworldIngredient : Interactable
{
    MeshFilter meshFilter;
    public Mesh HarvestedMesh;

    [HideInInspector]
    public bool Harvested;

    public IngredientType IngType;

    public override void Awake()
    {
        base.Awake();

        meshFilter = GetComponent<MeshFilter>();
    }

    public void Harvest()
    {
        Harvested = true;
        //Update the mesh filter to the correct mesh for the obj
        meshFilter.mesh = HarvestedMesh;
        Collider.enabled = false;

        if (Player.Inv.AddIngredient(this))
        {
            return;
        }
        else
        {
            //Display something about the Player's inventory being full
            Debug.Log("Player Inventory is full!");
        }
    }
}
