using System.Collections.Generic;

public class Inventory<T>
{
    public static List<T> PlayerInv;

    public Inventory(int invSize)
    {
        PlayerInv = new List<T>(invSize);
    }

    public bool AddItem(T itemToAdd)
    {
        for (int i = 0; i < PlayerInv.Count; i++)
        {
            if (PlayerInv[i] == null)
            {
                PlayerInv.Insert(i, itemToAdd);
                return true;
            }
        }

        return false;
    }

    public bool RemoveItem(T itemToRemove)
    {
        for (int i = 0; i < PlayerInv.Count; i++)
        {
            if (PlayerInv[i].Equals(itemToRemove))
            {
                PlayerInv.RemoveAt(i);
                return true;
            }
        }

        return false;
    }
}
