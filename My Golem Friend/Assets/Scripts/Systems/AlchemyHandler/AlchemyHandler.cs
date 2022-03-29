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
    Ingredient_Max
}

public class AlchemyHandler : MonoBehaviour
{
    public static AlchemyHandler Instance;
    //Holds all the special transform positions for the different ingredients.
    //Can be indexed using the IngredientsType enum
    public GameObject[] IngredientStorageDisplays;

    public RecipeData[] CraftableRecipes;

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
        if (AlchemyHandler.StoredIngredients[(int)type] == 0)
        {
            IngredientStorageDisplays[(int)type].SetActive(true);
        }

        AlchemyHandler.StoredIngredients[(int)type] += 1;

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
