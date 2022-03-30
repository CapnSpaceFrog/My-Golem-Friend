using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IngredientType
{
    Lavender,
    Honeycomb,
    BellFlower,
    Toadstool,
    MossyGem,
    Glowstone,
    FlowerOfDeath,
    Ectoplasm,
    AgedBlood,
    WaterLily,
    ReptileScale,
    LivingClay,
    None
}

public class IngredientTableManager : MonoBehaviour
{
    public static IngredientTableManager Instance;
    //Holds all the special transform positions for the different ingredients.
    //Can be indexed using the IngredientsType enum
    public GameObject[] IngredientStorageDisplays;

    public static int[] StoredIngredients;

    private void Awake()
    {
        Instance = this;
        //12 is a magic number that represent the total amount of ingredients
        //Can be indexed using the IngredientsType enum
        StoredIngredients = new int[12];
    }

    private void OnEnable()
    {

    }

    public void AddIngToStorage(IngredientType type)
    {
        if (IngredientTableManager.StoredIngredients[(int)type] == 0)
        {
            IngredientStorageDisplays[(int)type].SetActive(true);
        }

        IngredientTableManager.StoredIngredients[(int)type] += 1;

        UIHandler.Instance.UpdateStoredIngCounters();
    }

    public void RemoveIngFromStorage(IngredientType type)
    {
        if (StoredIngredients[(int)type] - 1 == 0)
        {
            StoredIngredients[(int)type]--;

            IngredientStorageDisplays[(int)type].SetActive(false);
        }
        else
        {
            StoredIngredients[(int)type]--;
        }

        UIHandler.Instance.UpdateStoredIngCounters();
    }
}
