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
    
    public void AddNewRecipeToSearch(Recipe recipeUnlocked)
    {
        UnlockedRecipes.Add(recipeUnlocked);

        //Add this recipe to the queue because we may have ingredient 
        PriorityRecipes.Enqueue(recipeUnlocked);

        Debug.Log(recipeUnlocked.ObjectToCraft + ": Recipe added to Priority Queue.");
    }

    public void SearchPriorityRecipes(Dictionary<IngredientType, int> MixedIngredients)
    {
        Recipe[] PriorityQueue = PriorityRecipes.ToArray();

        for (int i = 0; i < PriorityQueue.Length; i++)
        {
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
                            recipe.NumberOfReqMet++;
                            Debug.Log(recipe.ObjectToCraft + $": Requirement of {MixedIngredients[recipe.RecipeRequirements[index].ReqIng]}" +
                                $" met. Recipe has {recipe.NumberOfReqMet} requirements met.");
                        }
                        break;

                    //Medium priorty
                    case 2:
                        if (recipe.RecipeRequirements[index].ReqIngAmount <= 2)
                        {
                            recipe.RecipeRequirements[index].IngReqMet = true;
                            recipe.NumberOfReqMet++;
                            Debug.Log(recipe.ObjectToCraft + $": Requirement of {MixedIngredients[recipe.RecipeRequirements[index].ReqIng]}" +
                                $" met. Recipe has {recipe.NumberOfReqMet} requirements met.");
                        }
                        break;

                    //Case 3 & up is all the same
                    case 3:
                    default:
                        if (recipe.RecipeRequirements[index].ReqIngAmount > 3)
                        {
                            //TODO: If we want ingredient requirements greater than 3, we'll need to do some special behavior here
                            break;
                        }
                        else
                        {
                            recipe.RecipeRequirements[index].IngReqMet = true;
                            recipe.NumberOfReqMet++;
                            Debug.Log(recipe.ObjectToCraft + $": Requirement of {MixedIngredients[recipe.RecipeRequirements[index].ReqIng]}" +
                                $" met. Recipe has {recipe.NumberOfReqMet} requirements met.");
                        }
                        break;
                }
            }
            //have finished iterarting through the recipes requirements

            if (IsRecipeCompleted(recipe))
            {
                Debug.Log("Crafting: " + recipe.ObjectToCraft);
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
            if (recipe.RecipeRequirements[i].IngReqMet == false)
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
                            recipe.NumberOfReqMet++;
                            Debug.Log(recipe.ObjectToCraft + $": Requirement of {MixedIngredients[recipe.RecipeRequirements[index].ReqIng]}" +
                                $" met. Recipe has {recipe.NumberOfReqMet} requirements met.");
                        }
                        break;

                    //Medium priorty
                    case 2:
                        if (recipe.RecipeRequirements[index].ReqIngAmount == 2)
                        {
                            recipe.RecipeRequirements[index].IngReqMet = true;
                            recipe.NumberOfReqMet++;
                            Debug.Log(recipe.ObjectToCraft + $": Requirement of {MixedIngredients[recipe.RecipeRequirements[index].ReqIng]}" +
                                $" met. Recipe has {recipe.NumberOfReqMet} requirements met.");
                        }
                        break;

                    //Case 3 & up is all the same
                    case 3:
                    default:
                        if (recipe.RecipeRequirements[index].ReqIngAmount >= 3)
                        {
                            recipe.RecipeRequirements[index].IngReqMet = true;
                            recipe.NumberOfReqMet++;
                            Debug.Log(recipe.ObjectToCraft + $": Requirement of {MixedIngredients[recipe.RecipeRequirements[index].ReqIng]}" +
                                $" met. Recipe has {recipe.NumberOfReqMet} requirements met.");
                        }
                        break;
                }
            }
            //have finished iterarting through the recipes requirements

            if (IsRecipeCompleted(recipe))
            {
                //Consume the required ingredients out of the pot
                Debug.Log("Crafting: " + recipe.ObjectToCraft);
                CraftingHandler.Instance.RemoveCraftedRecipeIngredients(recipe);
                return;
            }
            else
            {
                UpdateRecipePriority(recipe);
            }
        }
    }

    private void UpdateRecipePriority(Recipe recipe)
    {
        switch (recipe.NumberOfReqMet)
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

    private void ResortRecipesByPriority()
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
                    continue;

                //How many ingredients of the required type for the recipe do we currently have in the pot?
                switch (MixedIngredients[recipe.RecipeRequirements[index].ReqIng])
                {
                    case 0:
                        //If the Ing Req is met, but the ingredient has been totally removed, so revert to false
                        recipe.RecipeRequirements[index].IngReqMet = false;
                        recipe.NumberOfReqMet--;
                        Debug.Log(recipe.ObjectToCraft + $": Recipe Req of type {recipe.RecipeRequirements[index].ReqIng} no longer met.");
                        break;

                    case 1:
                        //Check if the recipe req is equal to one and flag it and move on
                        if (recipe.RecipeRequirements[index].ReqIngAmount > 1)
                        {
                            recipe.RecipeRequirements[index].IngReqMet = false;
                            recipe.NumberOfReqMet--;
                            Debug.Log(recipe.ObjectToCraft + $": Recipe Req of type {recipe.RecipeRequirements[index].ReqIng} no longer met.");
                        }
                        break;

                    //Medium priorty
                    case 2:
                        if (recipe.RecipeRequirements[index].ReqIngAmount > 2)
                        {
                            recipe.RecipeRequirements[index].IngReqMet = false;
                            recipe.NumberOfReqMet--;
                            Debug.Log(recipe.ObjectToCraft + $": Recipe Req of type {recipe.RecipeRequirements[index].ReqIng} no longer met.");
                        }
                        break;

                    //Case 3 & up is all the same
                    case 3:
                    default:
                        Debug.Log(recipe.ObjectToCraft + $": Recipe Req of type {recipe.RecipeRequirements[index].ReqIng} is still met.");
                        //We still have 3 or more ingredients which always satisfies the requirement, so skip
                        continue;
                }
            }

            UpdateRecipePriority(recipe);
        }
        
        ResortRecipesByPriority();
    }
}
