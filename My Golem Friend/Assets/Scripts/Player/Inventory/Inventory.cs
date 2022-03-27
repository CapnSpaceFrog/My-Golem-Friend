using System.Collections.Generic;

public class Inventory
{
    public static int[] PlayerInv;

    private int maxIngCapacity = 5;

    public Inventory(int invSize)
    {
        PlayerInv = new int[invSize];
    }
    
    public bool AddIngredient(OverworldIngredient ingredientToAdd)
    {
        if (PlayerInv[(int)ingredientToAdd.IngType] < maxIngCapacity)
        {
            PlayerInv[(int)ingredientToAdd.IngType] += ingredientToAdd.AddedToInv();
            //Added item successfully to the inventory
            return true;
        }
        else
        {
            //Inv is full, doesn't add item
            return false;
        }
    }

    public bool RemoveItem(OverworldIngredient ingredientToRemove)
    {
        if (PlayerInv[(int)ingredientToRemove.IngType] > 0)
        {
            PlayerInv[(int)ingredientToRemove.IngType] -= ingredientToRemove.AddedToInv();
            //Play had enough ingredients in their inventory
            return true;
        }
        else
        {
            //No ingredients of the type in the Players inventory
            return false;
        }
    }
}
