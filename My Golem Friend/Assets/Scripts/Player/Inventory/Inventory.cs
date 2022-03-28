using System.Collections.Generic;

public class Inventory
{
    public static OverworldIngredient[] Inv;

    public Inventory(int invSize)
    {
        Inv = new OverworldIngredient[invSize];
    }
    
    public bool AddIngredient(OverworldIngredient ingredientToAdd)
    {
        for (int i = 0; i < Inv.Length; i++)
        {
            if (Inv[i] == null)
            {
                Inv[i] = ingredientToAdd;

                InvUISlot UISlot = UIHandler.Instance.GetOpenInvUISlot();

                //This should always be something but incase I somehow fuck up my code i prevent an error from happening
                UISlot.Image.sprite = UIHandler.Instance.GetInvUISprite(ingredientToAdd.IngType);

                return true;
            }
        }

        return false;
    }

    //This is going to get changed as the way we keep manage the player inventory is gonna change
    public bool RemoveItem(OverworldIngredient ingredientToRemove)
    {
        for (int i = 0; i < Inv.Length; i++)
        {
            if (Inv[i] == null)
            {
                //Added item successfully to the inventory
                Inv[i] = ingredientToRemove;
                return true;
            }
        }

        return false;
    }
}
