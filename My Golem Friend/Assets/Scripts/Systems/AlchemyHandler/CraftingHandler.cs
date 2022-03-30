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
    //TODO: Remove once debugging is done
    public string RecipeName;
    public RecipeIngredient[] RecipeRequirements;

    [HideInInspector]
    public int numberOfReqMet;

    [HideInInspector]
    public RecipeSearchPriority SearchPriority;
}

//TODO: Allow user to take ingredients out of the pot/transfer all to inv/trasnfer all to bench
public class CraftingHandler : MonoBehaviour
{
    [Header("Crafting Variables")]
    //TODO: figure out how we unlock the recipes to the player
    public Recipe[] Recipes;
    public int MaxIngCapacity;
    public Transform SpitOutPoint;

    private static RecipeCollection RecipeManager;

    private static Dictionary<IngredientType, int> MixedIngredients;

    private void Awake()
    {
        //Only needs to be the size of the max amount of ingredients
        MixedIngredients = new Dictionary<IngredientType, int>(12);
        RecipeManager = new RecipeCollection();

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

        if (heldIng != null && !CheckMaxCapacity())
        {
            //is an ingredient
            AddIngToMix(heldIng.StoredIng.Type);
            //TODO: how can we tie this in to the WorldObjectManager? This definitly will break something
            WorldObjectManager.IngThrownInCauldron(heldIng);
        }
        else
        {
            //TODO: Spit out the ingredient that sets us over capacity
            other.gameObject.transform.position = SpitOutPoint.position;
            //is some other object get it out of the couldron
        }
    }

    private void AddIngToMix(IngredientType typeToAdd)
    {
        MixedIngredients[typeToAdd] += 1;

        RecipeManager.SearchPriorityRecipes(MixedIngredients);
    }
    
    private bool CheckMaxCapacity()
    {
        int totalCount = 0;

        foreach(int ingCount in MixedIngredients.Values)
        {
            totalCount += ingCount;
        }

        //Are we above or at the max ingredient capacity?
        if (totalCount >= MaxIngCapacity)
        {
            return true;
        }
        else
        {
            return false;
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