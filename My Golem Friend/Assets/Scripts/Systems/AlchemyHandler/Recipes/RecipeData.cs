using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RecipeIngredient
{
    public IngredientType ReqIng;
    public int ReqIngAmount;
    public bool IngReqMet;
}

public enum RecipeSearchPriority
{
    High,
    Medium,
    Low,
    None
}

[CreateAssetMenu(fileName = "Default Recipe Data", menuName = "Data/Default Recipe Data")]
public class RecipeData : ScriptableObject
{
    public RecipeIngredient[] RecipeRequirements;

    [HideInInspector]
    public int numberOfReqMet;

    

    [HideInInspector]
    public RecipeSearchPriority SearchPriority;
}
