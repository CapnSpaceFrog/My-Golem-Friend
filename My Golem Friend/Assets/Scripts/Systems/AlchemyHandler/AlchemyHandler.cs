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
    //Can be indeced using the IngredientsType enum
    public Transform[] IngredientLocations;

    public RecipeData[] Recipes;

    private void Awake()
    {

    }

    private void Update()
    {
        
    }
}
