using System.Collections.Generic;

public class Inventory
{
    public static Ingredient[] Ingredients;

    public Inventory(int invSize)
    {
        Ingredients = new Ingredient[invSize];
    }
    
    public bool AddIngredient(Ingredient ingredient)
    {
        for (int i = 0; i < Ingredients.Length; i++)
        {
            if (Ingredients[i] == null)
            {
                Ingredients[i] = ingredient;
                return true;
            }
        }

        return false;
    }

    public bool RemoveIngredient(Ingredient ingredient)
    {
        for (int i = 0; i < Ingredients.Length; i++)
        {
            if (Ingredients[i] == ingredient)
            {
                Ingredients[i] = null;
                return true;
            }
        }

        return false;
    }

    public void StoreIngredientsToTable()
    {
        for (int i = 0; i < Ingredients.Length; i++)
        {
            if (Ingredients[i] != null)
            {
                AlchemyHandler.Instance.AddIngToStorage(Ingredients[i].IngType);
                UIHandler.Instance.EmptyInvUISlot(Ingredients[i]);
                RemoveIngredient(Ingredients[i]);
            }
        }

        UIHandler.Instance.UpdateStoredIngCounters();
    }
}
