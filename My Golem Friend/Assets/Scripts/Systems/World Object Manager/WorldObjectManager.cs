using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObjectManager : MonoBehaviour
{
    public static List<Holdable> InsantiatedObjects;

    [Header("Ingredient Prefab Variables")]
    public GameObject[] HoldableIngredients;

    private void Awake()
    {
        //15 for default size, hopefully player never maxes this out so the engine doesnt keep resizing it
        InsantiatedObjects = new List<Holdable>(15);


    }

    public static void InsantiateObject(IngredientType type)
    {
        Debug.Log("Instantiated Object of type: " + type);
    }

    public static void InsantiateObject()
    {

    }
}
