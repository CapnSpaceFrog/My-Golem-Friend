[System.Serializable]
public class StorableIngredient
{
    public IngredientType Type;
}

public class Inventory
{
    public static StorableIngredient[] Ingredients;

    public Inventory(int invSize)
    {
        Ingredients = new StorableIngredient[invSize];
    }

    public static StorableIngredient CreateStorableIng(Ingredient ingBase)
    {
        StorableIngredient newIng = new StorableIngredient();
        newIng.Type = ingBase.IngType;
        return newIng;
    }

    public static StorableIngredient CreateStorableIng(IngredientType type)
    {
        StorableIngredient newIng = new StorableIngredient();
        newIng.Type = type;
        return newIng;
    }

    public bool AddIngredient(StorableIngredient ing)
    {
        for (int i = 0; i < Ingredients.Length; i++)
        {
            if (Ingredients[i] == null)
            {
                Ingredients[i] = ing;
                UIHandler.Instance.FillInvUISlot(Ingredients[i]);
                return true;
            }
        }

        return false;
    }

    public bool RemoveIngredient(StorableIngredient ing)
    {
        for (int i = 0; i < Ingredients.Length; i++)
        {
            if (Ingredients[i] == ing)
            {
                Ingredients[i] = null;
                UIHandler.Instance.EmptyInvUISlot(ing);
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
                AlchemyHandler.Instance.AddIngToStorage(Ingredients[i].Type);
                RemoveIngredient(Ingredients[i]);
            }
        }

        WorldObjectManager.PurgeFlaggedObjects();
    }
}
