[System.Serializable]
public class StorableIngredient
{
    public IngredientType Type;
}

public class Inventory
{
    public StorableIngredient[] Ingredients;

    public Inventory(int invSize)
    {
        Ingredients = new StorableIngredient[invSize];
    }

    public static StorableIngredient CreateStorableIng(IngredientType ingType)
    {
        StorableIngredient newIng = new StorableIngredient();
        newIng.Type = ingType;
        return newIng;
    }

    public bool AddIngredient(StorableIngredient ing, UISlotType slotType)
    {
        for (int i = 0; i < Ingredients.Length; i++)
        {
            if (Ingredients[i] == null)
            {
                Ingredients[i] = ing;
                UIHandler.Instance.FillUISlot(Ingredients[i], slotType);
                return true;
            }
        }

        return false;
    }

    public bool RemoveIngredient(StorableIngredient ing, UISlotType slotType)
    {
        for (int i = 0; i < Ingredients.Length; i++)
        {
            if (Ingredients[i] == ing)
            {
                Ingredients[i] = null;
                UIHandler.Instance.EmptyUISlot(ing, slotType);
                return true;
            }
        }

        return false;
    }

    public void StoreIngredientsToTable(UISlotType slotType)
    {
        for (int i = 0; i < Ingredients.Length; i++)
        {
            if (Ingredients[i] != null)
            {
                IngredientTableManager.Instance.AddIngToStorage(Ingredients[i].Type);
                RemoveIngredient(Ingredients[i], slotType);
            }
        }
    }
}
