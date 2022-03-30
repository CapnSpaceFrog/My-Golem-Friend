using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RecipeSearchPriority
{
    None,
    Low,
    Medium,
    High,
    maxPriorities
}

public class RecipeCollection
{
    public static List<Recipe> UnlockedRecipes;

    private static Queue<Recipe> PriorityRecipes;

    private static List<Recipe> RemainingRecipes;

    public RecipeCollection()
    {
        UnlockedRecipes = new List<Recipe>(2);

        RemainingRecipes = new List<Recipe>(2);

        PriorityRecipes = new Queue<Recipe>(2);
    }
    
    public void RecipeUnlocked(Recipe recipeUnlocked)
    {
        UnlockedRecipes.Add(recipeUnlocked);

        //Add the newly unlocked recipe to the pool of recipes
        RemainingRecipes.Add(recipeUnlocked);
    }

    //TODO: Included Incrementation and decrementation in the same function for sake of time
    //This may result in performance loss, but lets find out
    public void SearchPriorityRecipes(Dictionary<IngredientType, int> MixedIngredients)
    {
        Recipe[] PriorityQueue = PriorityRecipes.ToArray();

        for (int i = 0; i < PriorityQueue.Length; i++)
        {
            //Is there a recipe in this slot? Or is it empty?
            if (PriorityQueue[i] == null)
                continue;

            Recipe recipe = PriorityQueue[i];

            for (int index = 0; index < recipe.RecipeRequirements.Length; index++)
            {
                //This ing requiurement is already met, no need to check
                if (recipe.RecipeRequirements[index].IngReqMet)
                    continue;

                //How many ingredients of the required type for the recipe do we currently have in the pot?
                switch (MixedIngredients[recipe.RecipeRequirements[index].ReqIng])
                {
                    case 0:
                        //if we have none of the ingredient type, skip ahead
                        continue;

                    case 1:
                        //Check if the recipe req is equal to one and flag it and move on
                        if (recipe.RecipeRequirements[index].ReqIngAmount == 1)
                        {
                            recipe.RecipeRequirements[index].IngReqMet = true;
                            recipe.numberOfReqMet++;
                            break;
                        }
                        break;

                    //Medium priorty
                    case 2:
                        if (recipe.RecipeRequirements[index].ReqIngAmount == 2)
                        {
                            recipe.RecipeRequirements[index].IngReqMet = true;
                            recipe.numberOfReqMet++;
                            break;
                        }
                        break;

                    //Case 3 & up is all the same
                    case 3:
                    default:
                        if (recipe.RecipeRequirements[index].ReqIngAmount == 3)
                        {
                            recipe.RecipeRequirements[index].IngReqMet = true;
                            recipe.numberOfReqMet++;
                            break;
                        }
                        break;
                }
            }
            //have finished iterarting through the recipes requirements

            if (IsRecipeCompleted(recipe))
            {
                //Consume the required ingredients out of the pot
                CraftingHandler.Instance.RemoveCraftedRecipeIngredients(recipe);
                return;
            }
            else
            {
                UpdateRecipePriority(recipe);
            }
        }

        SearchRemainingRecipes(MixedIngredients);
    }

    public bool IsRecipeCompleted(Recipe recipe)
    {
        for (int i = 0; i < recipe.RecipeRequirements.Length; i++)
        {
            if (!recipe.RecipeRequirements[i].IngReqMet)
                return false;
        }

        return true;
    }

    public void SearchRemainingRecipes(Dictionary<IngredientType, int> MixedIngredients)
    {
        foreach (Recipe recipe in RemainingRecipes)
        {
            //Is there a recipe in this slot? Or is it empty?
            if (recipe == null)
                continue;

            for (int index = 0; index < recipe.RecipeRequirements.Length; index++)
            {
                //This ing requiurement is already met, no need to check
                if (recipe.RecipeRequirements[index].IngReqMet)
                    continue;

                //How many ingredients of the required type for the recipe do we currently have in the pot?
                switch (MixedIngredients[recipe.RecipeRequirements[index].ReqIng])
                {
                    case 0:
                        //if we have none of the ingredient type, skip ahead
                        continue;

                    case 1:
                        //Check if the recipe req is equal to one and flag it and move on
                        if (recipe.RecipeRequirements[index].ReqIngAmount == 1)
                        {
                            recipe.RecipeRequirements[index].IngReqMet = true;
                            recipe.numberOfReqMet++;
                        }
                        break;

                    //Medium priorty
                    case 2:
                        if (recipe.RecipeRequirements[index].ReqIngAmount == 2)
                        {
                            recipe.RecipeRequirements[index].IngReqMet = true;
                            recipe.numberOfReqMet++;
                        }
                        break;

                    //Case 3 & up is all the same
                    case 3:
                    default:
                        if (recipe.RecipeRequirements[index].ReqIngAmount >= 3)
                        {
                            recipe.RecipeRequirements[index].IngReqMet = true;
                            recipe.numberOfReqMet++;
                            break;
                        }
                        break;
                }
            }
            //have finished iterarting through the recipes requirements

            //Update the priority of our recipe
            UpdateRecipePriority(recipe);
        }

        ResortRecipes();

        Debug.Log("Priority Queue has count of: " + PriorityRecipes.Count);
        Debug.Log("Remaining Queue has count of: " + RemainingRecipes.Count);
    }

    private void UpdateRecipePriority(Recipe recipe)
    {
        switch (recipe.numberOfReqMet)
        {
            case 0:
                recipe.SearchPriority = RecipeSearchPriority.None;
                break;

            case 1:
                recipe.SearchPriority = RecipeSearchPriority.Low;
                break;

            case 2:
            case 3:
                recipe.SearchPriority = RecipeSearchPriority.Medium;
                break;

            case 4:
            default:
                recipe.SearchPriority = RecipeSearchPriority.High;
                break;
        }
    }

    private void ResortRecipes()
    {
        PriorityRecipes.Clear();
        RemainingRecipes.Clear();

        foreach (Recipe toSortRecipe in UnlockedRecipes)
        {
            for (int i = 0; i < (int)RecipeSearchPriority.maxPriorities; i++)
            {
                if (toSortRecipe.SearchPriority == (RecipeSearchPriority)i)
                {
                    switch (toSortRecipe.SearchPriority)
                    {
                        case RecipeSearchPriority.None:
                            RemainingRecipes.Add(toSortRecipe);
                            break;

                        case RecipeSearchPriority.Low:
                            PriorityRecipes.Enqueue(toSortRecipe);
                            break;

                        case RecipeSearchPriority.Medium:
                            PriorityRecipes.Enqueue(toSortRecipe);
                            break;

                        case RecipeSearchPriority.High:
                            PriorityRecipes.Enqueue(toSortRecipe);
                            break;
                    }
                }
            }
        }
    }

    public void ReorganizeAfterIngRemoval(Dictionary<IngredientType, int> MixedIngredients)
    {
        foreach (Recipe recipe in UnlockedRecipes)
        {
            for (int index = 0; index < recipe.RecipeRequirements.Length; index++)
            {
                //We only count ing reqs that are met, so if it isn't we don't need to check it
                if (recipe.RecipeRequirements[index].IngReqMet == false)
                    break;

                //How many ingredients of the required type for the recipe do we currently have in the pot?
                switch (MixedIngredients[recipe.RecipeRequirements[index].ReqIng])
                {
                    case 0:
                        //If the Ing Req is met, but the ingredient has been totally removed, so revert to false
                        recipe.RecipeRequirements[index].IngReqMet = false;
                        recipe.numberOfReqMet--;
                        break;

                    case 1:
                        //Check if the recipe req is equal to one and flag it and move on
                        if (recipe.RecipeRequirements[index].ReqIngAmount >= 2)
                        {
                            recipe.RecipeRequirements[index].IngReqMet = false;
                            recipe.numberOfReqMet--;
                        }
                        break;

                    //Medium priorty
                    case 2:
                        if (recipe.RecipeRequirements[index].ReqIngAmount >= 3)
                        {
                            recipe.RecipeRequirements[index].IngReqMet = false;
                            recipe.numberOfReqMet--;
                        }
                        break;

                    //Case 3 & up is all the same
                    case 3:
                    default:
                        //We still have 3 or more ingredients which always satisfies the requirement, so skip
                        continue;
                }
            }

            UpdateRecipePriority(recipe);
        }
        
        ResortRecipes();
    }
}
