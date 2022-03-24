using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlchemyHandler : MonoBehaviour
{
    enum IngredientType
    {

    }
    //Holds all the special transform positions for the different ingredients.
    //Imagine a table that has a shit load of ingredients
    public Transform[] ingredientLocations;


    /*
     * Need to be able to throw a bunch of ingredients into this thing and it needs to be able to see what ingredients have been
     * put into it, and if too many are thrown in or an invalid recipe was attempted, it spits the ingredients back out
     * to the Player
     */
}
