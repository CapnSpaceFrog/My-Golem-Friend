using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractableType
{
    OverworldIng,
    IngStorage,
    StorageTable,
    Holdable
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