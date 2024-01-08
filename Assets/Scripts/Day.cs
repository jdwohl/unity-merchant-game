using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MerchantGame/Day")]
public class Day : ScriptableObject
{
    public const int MAX_CUSTOMERS = 5;
    public Customer[] customers = new Customer[MAX_CUSTOMERS];
    // customers for the day will be placed in here
    public int dayNumber;

    public Scene[] scenes = new Scene[10];

    public Scene FindCorrectScene(int sceneIndex, string sceneRoute)
    {
        for (int i = 0; i < scenes.Length; i++)
        {
            if (scenes[i] != null)
            {
                if (scenes[i].sceneIndex == sceneIndex && scenes[i].sceneRoute == sceneRoute)
                {
                    return scenes[i];
                }
            }
        }

        return scenes[1];
        // failsafe
    }
}
