using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeCollection
{
    public static List<RecipeData> UnlockedRecipes;

    private static Queue<RecipeData> PriorityRecipes;

    private static List<RecipeData> RemainingRecipes;

    public RecipeCollection()
    {
        UnlockedRecipes = new List<RecipeData>(2);

        RemainingRecipes = new List<RecipeData>(2);

        PriorityRecipes = new Queue<RecipeData>(2);
    }
    
    public void RecipeUnlocked(RecipeData recipeUnlocked)
    {
        UnlockedRecipes.Add(recipeUnlocked);
    }

    //TODO: Included Incrementation and decrementation in the same function for sake of time
    //This may result in performance loss, but lets find out
    public void SearchPriorityRecipes(Dictionary<IngredientType, int> MixedIngredients)
    {
        RecipeData[] PriorityQueue = PriorityRecipes.ToArray();

        for (int i = 0; i < PriorityQueue.Length; i++)
        {
            //Is there a recipe in this slot? Or is it empty?
            if (PriorityQueue[i] == null)
                continue;

            RecipeData recipe = PriorityQueue[i];

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
                CraftRecipe(recipe);
                //Craft the recipe & clear its requirements
                //Consume the required ingredients out of the pot
                //Return out of here
                return;
            }
            else
            {
                //Recipe wasnt completed, lets add it to eligable recipes however
                //We'll prioritize searching the eligable recipes to speed up the process later
                //This is useful because we can quickly detect if the player is trying to craft a certain recipe than
                //just always iterating through every single recipe

            }

            UpdateRecipePriority(recipe);
        }

        SearchRemainingRecipes(MixedIngredients);
    }

    public bool IsRecipeCompleted(RecipeData recipe)
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
        foreach (RecipeData recipe in RemainingRecipes)
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

            //Update the priority of our recipe
            UpdateRecipePriority(recipe);
        }

        ResortRecipes();

        Debug.Log("Priority Queue has count of: " + PriorityRecipes.Count);
        Debug.Log("Remaining Queue has count of: " + RemainingRecipes.Count);
    }

    private void UpdateRecipePriority(RecipeData recipe)
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

        for (int i = 0; i < UnlockedRecipes.Count; i++)
        {
            if (UnlockedRecipes[i].SearchPriority == (RecipeSearchPriority)i)
            {
                switch (UnlockedRecipes[i].SearchPriority)
                {
                    case RecipeSearchPriority.None:
                        RemainingRecipes.Add(UnlockedRecipes[i]);
                        break;

                    case RecipeSearchPriority.Low:
                        PriorityRecipes.Enqueue(UnlockedRecipes[i]);
                        break;

                    case RecipeSearchPriority.Medium:
                        PriorityRecipes.Enqueue(UnlockedRecipes[i]);
                        break;

                    case RecipeSearchPriority.High:
                        PriorityRecipes.Enqueue(UnlockedRecipes[i]);
                        break;
                }
            }
        }
    }

    private void CraftRecipe(RecipeData recipe)
    {
        Debug.Log("Recipe Crafted: " + recipe);
    }

}
