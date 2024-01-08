using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemDisplay : MonoBehaviour
{
    public TextMeshProUGUI itemText;
    public string itemName;
    public bool addedToSellList = false;
    public bool gameControllerSet = false;
    public GameController gameController;

    public void SetGameControllerReference(GameController controller)
    {
        this.gameController = controller;
        this.gameControllerSet = true;
    }

    public void ToggleSelect()
    {
        if (gameControllerSet)
        {
            addedToSellList = !addedToSellList;
            gameController.AddOrRemoveItem(itemName, addedToSellList);
        }
    }
}
