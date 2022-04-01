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

    public string RecipeName;

    public GameObject ObjectToCraft;
    
    public RecipeIngredient[] RecipeRequirements;

    [HideInInspector]
    public int NumberOfReqMet;
    [HideInInspector]
    public RecipeSearchPriority SearchPriority;
}

public class CraftingHandler : MonoBehaviour
{
    public static CraftingHandler Instance;

    public static Inventory CauldronInventory { get; private set; }

    [Header("Crafting Variables")]
    public Recipe[] Recipes;
    public int MaxCauldronCapacity;
    public Transform SpitOutPoint;

    private static RecipeCollection RecipeManager;
    
    private static Dictionary<IngredientType, int> MixedIngredients;

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
            recipe.NumberOfReqMet = 0;
        }

        GameObject craftedObj = WorldObjectManager.InstantiateCraftedObject(recipe.ObjectToCraft);
        craftedObj.transform.position = SpitOutPoint.position;

        EmptyCauldronToTable();
    }

    public void FindRecipeToUnlock(string recipeToUnlock)
    {
        foreach (Recipe recipe in Recipes)
        {
            if (recipe.RecipeName == recipeToUnlock)
            {
                RecipeManager.AddNewRecipeToSearch(recipe);
                UIHandler.Instance.UpdateUnlockedRecipeUI(recipeToUnlock);
                RecipeManager.SearchPriorityRecipes(MixedIngredients);
                return;
            }
        }
    }

    public void EmptyCauldronToTable()
    {
        for (int i = 0; i < CauldronInventory.Ingredients.Length; i++)
        {
            if (CauldronInventory.Ingredients[i] == null)
                continue;

            StorableIngredient storedIng = CauldronInventory.Ingredients[i];

            MixedIngredients[storedIng.Type] -= 1;
            Debug.Log($"Removed {storedIng.Type}. {MixedIngredients[storedIng.Type]} left.");
        }

        CauldronInventory.StoreIngredientsToTable(UISlotType.Cauldron);

        RecipeManager.ReorganizeAfterIngRemoval(MixedIngredients);
    }

    public void EmptyIngToPlayerInv()
    {
        for (int i = 0; i < CauldronInventory.Ingredients.Length; i++)
        {
            if (CauldronInventory.Ingredients[i] == null)
                continue;

            StorableIngredient storedIng = CauldronInventory.Ingredients[i];

            if (Player.Inv.AddIngredient(storedIng, UISlotType.PlayerInv))
            {
                CauldronInventory.RemoveIngredient(storedIng, UISlotType.Cauldron);
            }
            else
            {
                Debug.Log("Did not add item because player inventory is full.");
                break;
            }
        }

        Debug.Log("Stopped emptying cauldorn to player inv.");
    }
}