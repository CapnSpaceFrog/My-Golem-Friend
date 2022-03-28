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
    //Holds all the special transform positions for the different ingredients.
    //Can be indexed using the IngredientsType enum
    public Transform[] StoredIngTransform;

    public RecipeData[] CraftableRecipes;

    public static int[] StoredIngredients;

    private void Awake()
    {
        //12 is a magic number that represent the total amount of ingredients
        //Can be indexed using the IngredientsType enum
        StoredIngredients = new int[12];
    }

    private void OnEnable()
    {

    }

    private void Update()
    {
        
    }


}
