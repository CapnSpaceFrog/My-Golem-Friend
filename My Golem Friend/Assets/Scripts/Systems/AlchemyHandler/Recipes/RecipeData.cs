using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AlchemyRecipe
{
    public IngredientType ReqIng;
    public int ReqIngAmount;
}

[CreateAssetMenu(fileName = "Default Recipe Data", menuName = "Data/Default Recipe Data")]
public class RecipeData : ScriptableObject
{
    public AlchemyRecipe[] Recipe;
}
