using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MerchantGame/Item")]
public class Items : ScriptableObject
{
    public string keyword;
    // name of the item (e.g. iron sword)

    public int attackPower;
    // strength of the item for basic attacks

    public int magicPower;
    // strength of the item for magic attacks

    public int healingPower;
    // strength of the item for healing magic

    public int defensePower;
    // defensive strength of a shield

    public int weight;
    // determines how much the user's speed is reduced by the item -- only relevant for weapons

    public string type;
    // determines the context for power (attack, healing, etc.)

    public int buyPrice;
    // the price at which the user purchased the item, represented as one number (gold = 100, silver = 10, bronze = 1)
}
