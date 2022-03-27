using System.Collections;
using System.Collections.Generic;
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

    public int AddedToInv()
    {
        return 1;
    }

    public int RemovedFromInv()
    {
        return 1;
    }

    public void Harvest()
    {
        Harvested = true;
        //Update the mesh filter to the correct mesh for the obj
        meshFilter.mesh = HarvestedMesh;
        Collider.enabled = false;

        if (Player.PlayerInventory.AddIngredient(this))
        {
            return;
        }
        else
        {
            //Display something about the Player's inventory being empty
        }
    }
}
