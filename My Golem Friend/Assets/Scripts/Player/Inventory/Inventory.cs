using System.Collections.Generic;

public class Inventory
{
    public static OverworldIngredient[] Ingredients;

    public Inventory(int invSize)
    {
        Ingredients = new OverworldIngredient[invSize];
    }
    
    public bool AddIngredient(OverworldIngredient ingredientToAdd)
    {
        for (int i = 0; i < Ingredients.Length; i++)
        {
            if (Ingredients[i] == null)
            {
                Ingredients[i] = ingredientToAdd;
                UIHandler.Instance.FillInvUISlot(ingredientToAdd.IngType);

                return true;
            }
        }

        return false;
    }

    //This is going to get changed as the way we keep manage the player inventory is gonna change
    public bool RemoveItem(OverworldIngredient ingredientToRemove)
    {
        for (int i = 0; i < Ingredients.Length; i++)
        {
            if (Ingredients[i] == ingredientToRemove)
            {
                UIHandler.Instance.EmptyInvUISlot(ingredientToRemove.IngType);
                Ingredients[i] = null;
                return true;
            }
        }

        return false;
    }

    public void StoreAllIngredients()
    {
        //Store all of the ingredients
        for (int i = 0; i < Ingredients.Length; i++)
        {
            if (Ingredients[i] != null)
            {
                AlchemyHandler.StoredIngredients[(int)Ingredients[i].IngType] += 1;
                RemoveItem(Ingredients[i]);
            }
        }
    }

    public bool CheckIfEmpty()
    {
        for (int i = 0; i < Ingredients.Length; i++)
        {
            if (Ingredients[i] != null)
            {
                return false;
            }
        }

        return true;
    }
}
