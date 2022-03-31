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
        Harvested = true;
        meshFilter.mesh = HarvestedMesh;
        Collider.enabled = false;
    }
}
