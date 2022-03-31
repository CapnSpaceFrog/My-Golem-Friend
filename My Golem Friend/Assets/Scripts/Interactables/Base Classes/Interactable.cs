using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractableType
{
    OverworldIngredient,
    StoredIngredient,
    IngredientStorageTable,
    CraftingStation,
    Recipe,
    Holdable,
    Golem
}

public class Interactable : MonoBehaviour
{
    public InteractableType InterObjType;

    protected Collider Collider;

    public virtual void Awake()
    {
        Collider = GetComponent<Collider>();
    }
}