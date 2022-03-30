using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Recipe
{
    [System.Serializable]
    public class RecipeIngredient
    {
        public IngredientType ReqIng;
        public int ReqIngAmount;
        [HideInInspector]
        public bool IngReqMet;
    }

    public GameObject CraftedObject;
    
    public RecipeIngredient[] RecipeRequirements;

    [HideInInspector]
    public int numberOfReqMet;

    [HideInInspector]
    public RecipeSearchPriority SearchPriority;
}

//TODO: Allow user to take ingredients out of the pot/transfer all to inv/trasnfer all to bench
public class CraftingHandler : MonoBehaviour
{
    public static CraftingHandler Instance;

    [Header("Crafting Variables")]
    //TODO: figure out how we unlock the recipes to the player
    public Recipe[] Recipes;
    public int MaxCauldronCapacity;
    public Transform SpitOutPoint;

    private static RecipeCollection RecipeManager;

    private static Dictionary<IngredientType, int> MixedIngredients;
    private static Inventory CauldronInventory;

    private void Awake()
    {
        Instance = this;

        //Only needs to be the size of the max amount of ingredients
        MixedIngredients = new Dictionary<IngredientType, int>(12);
        RecipeManager = new RecipeCollection();
        CauldronInventory = new Inventory(MaxCauldronCapacity);

        //TODO: Remove once we find a way to give the Player proper ways to unlock recipes
        for (int i = 0; i < Recipes.Length; i++)
        {
            RecipeManager.RecipeUnlocked(Recipes[i]);
        }

        for (int i = 0; i < ((int) IngredientType.None); i++)
        {
            MixedIngredients.Add((IngredientType)i, 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckIfIngredient(other);
    }

    private void CheckIfIngredient(Collider other)
    {
        HoldableIngredient heldIng = other.GetComponent<HoldableIngredient>();

        if (heldIng != null && CanAddToCauldron())
        {
            //is an ingredient
            AddIngToMix(heldIng.StoredIng);
            WorldObjectManager.IngThrownInCauldron(heldIng);
        }
        else
        {
            //TODO: Spit out the ingredient that sets us over capacity
            other.gameObject.transform.position = SpitOutPoint.position;
            //is some other object get it out of the couldron
        }
    }

    private void AddIngToMix(StorableIngredient ingToStore)
    {
        CauldronInventory.AddIngredient(ingToStore, UISlotType.Cauldron);

        MixedIngredients[ingToStore.Type] += 1;

        RecipeManager.SearchPriorityRecipes(MixedIngredients);
    }

    public void RemoveIngFromMix(StorableIngredient ingToRemove)
    {
        //An ingredient can only be removed from the mix if the Player has interacted with the cauldron,
        //otherwise another function adjusts the cauldron inventory
        MixedIngredients[ingToRemove.Type] -= 1;

        UIHandler.Instance.EmptyUISlot(ingToRemove, UISlotType.Cauldron);

        RecipeManager.ReorganizeAfterIngRemoval(MixedIngredients);
    }

    private bool CanAddToCauldron()
    {
        //Start @ 1 to count for the item thrown in
        int totalCount = 1;

        foreach(int ingCount in MixedIngredients.Values)
        {
            totalCount += ingCount;
        }

        //Are we above or at the max ingredient capacity?
        if (totalCount <= MaxCauldronCapacity)
        {
            
            return true;
        }
        else
        {
            Debug.Log("Cauldron is at Max Capacity");
            return false;
        }
    }

    public void RemoveCraftedRecipeIngredients(Recipe recipe)
    {
        foreach (Recipe.RecipeIngredient ing in recipe.RecipeRequirements)
        {
            for (int index = 0; index < ing.ReqIngAmount; index++)
            {
                for (int i = 0; i < CauldronInventory.Ingredients.Length; i++)
                {
                    if (CauldronInventory.Ingredients[i] == null)
                        continue;

                    if (ing.ReqIng == CauldronInventory.Ingredients[i].Type)
                    {
                        RemoveIngFromMix(CauldronInventory.Ingredients[i]);
                    }
                }
            }

            //TODO: this will probably throw an error
            ing.IngReqMet = false;
        }

        GameObject craftedObj = WorldObjectManager.InstantiateCraftedObject(recipe.CraftedObject);
        craftedObj.transform.position = SpitOutPoint.position;

        RecipeManager.ReorganizeAfterIngRemoval(MixedIngredients);
    }

    private void EmptyMixedIngredients()
    {
        for (int i = 0; i < MixedIngredients.Count; i++)
        {
            MixedIngredients[(IngredientType)i] = 0;
        }
    }
}
