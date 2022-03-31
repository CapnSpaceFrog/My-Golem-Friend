using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldObjectManager : MonoBehaviour
{
    public static WorldObjectManager Instance { get; private set; }

    private static List<HoldableIngredient> InstantiatedIngredients;
    private static List<GameObject> FlaggedForDestruction;

    [Header("Ingredient Prefab Variables")]
    public GameObject[] HoldableIngPrefab;

    private void Awake()
    {
        Instance = this;

        FlaggedForDestruction = new List<GameObject>(5);
        InstantiatedIngredients = new List<HoldableIngredient>(8);
    }

    //Spawns ingredients
    public void InstantiateHoldableIngredient(StorableIngredient storedIng)
    {
        //Are we trying to instantiate an object that already exists?
        if (!CheckForExistingIngObject(storedIng))
        {
            GameObject Obj = Instantiate(HoldableIngPrefab[(int)storedIng.Type]);

            HoldableIngredient heldIng = Obj.GetComponent<HoldableIngredient>();

            InstantiatedIngredients.Add(heldIng);

            if (FPInteract.HeldObject == null)
            {
                heldIng.AddToHand();
            }
            else
            {
                heldIng.DropOnFloor();
            }
        }
    }

    public bool CheckForExistingIngObject(StorableIngredient storedIng)
    {
        foreach (HoldableIngredient ing in InstantiatedIngredients)
        {
            if (ing.StoredIng == storedIng)
            {
                ing.gameObject.SetActive(true);

                if (FPInteract.HeldObject == null)
                {
                    ing.AddToHand();
                }
                else
                {
                    ing.DropOnFloor();
                }

                ClearObjectFromDestruction(ing.gameObject);
                return true;
            }
        }

        return false;
    }

    //Spawns Crafted Item
    public static GameObject InstantiateCraftedObject(GameObject objToSpawn)
    {
        GameObject craftedObj = Instantiate(objToSpawn);

        return craftedObj;
    }

    public static void FlagObjectForDestruction(GameObject objToDestroy)
    {
        FlaggedForDestruction.Add(objToDestroy);
    }

    public static void ClearObjectFromDestruction(GameObject objToClear)
    {
        FlaggedForDestruction.Remove(objToClear);
    }

    public static void PurgeFlaggedObjects()
    {
        for (int i = 0; i < FlaggedForDestruction.Count; i++)
        {
            Destroy(FlaggedForDestruction[i]);

            FlaggedForDestruction.Remove(FlaggedForDestruction[i]);
        }
    }

    public static void IngThrownInCauldron(HoldableIngredient heldIng)
    {
        for (int i = 0; i < InstantiatedIngredients.Count; i++)
        {
            if (InstantiatedIngredients[i] == heldIng)
            {
                InstantiatedIngredients.Remove(heldIng);
                Destroy(heldIng.gameObject);
            }
        }
    }
}
