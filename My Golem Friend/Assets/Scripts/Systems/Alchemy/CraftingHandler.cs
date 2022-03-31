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
            //TODO: Clean up this spit out process and enable an objs colliders n such
            other.gameObject.transform.position = SpitOutPoint.position;
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
        Debug.Log($"Removed {ingToRemove.Type}. {MixedIngredients[ingToRemove.Type]} left.");

        CauldronInventory.RemoveIngredient(ingToRemove, UISlotType.Cauldron);

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
            int ingAmountToRemove = ing.ReqIngAmount;

            for (int i = 0; ingAmountToRemove > 0; i++)
            {
                if (CauldronInventory.Ingredients[i] == null)
                    continue;

                if (ing.ReqIng == CauldronInventory.Ingredients[i].Type)
                {
                    MixedIngredients[CauldronInventory.Ingredients[i].Type] -= 1;

                    Debug.Log($"Removed {CauldronInventory.Ingredients[i].Type}. {MixedIngredients[CauldronInventory.Ingredients[i].Type]} left.");
                    CauldronInventory.RemoveIngredient(CauldronInventory.Ingredients[i], UISlotType.Cauldron);
                    ingAmountToRemove--;
                }
            }

            ing.IngReqMet = false;
            recipe.numberOfReqMet = 0;
        }

        GameObject craftedObj = WorldObjectManager.InstantiateCraftedObject(recipe.CraftedObject);
        craftedObj.transform.position = SpitOutPoint.position;
    }

    public void FindRecipeToUnlock(GameObject unlockedObj)
    {
        foreach (Recipe recipe in Recipes)
        {
            if (recipe.CraftedObject == unlockedObj)
            {
                RecipeManager.AddNewRecipeToSearch(recipe);
                RecipeManager.SearchPriorityRecipes(MixedIngredients);
                return;
                //TODO: Add the recipe to the proper UI panel
            }
        }
    }

    private void EmptyMixedIngredients()
    {
        for (int i = 0; i < MixedIngredients.Count; i++)
        {
            MixedIngredients[(IngredientType)i] = 0;
        }
    }
}
