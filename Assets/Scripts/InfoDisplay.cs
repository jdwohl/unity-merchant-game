using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoDisplay : MonoBehaviour
{
    public TextMeshProUGUI infoText;
    public GameController gameController;
    public bool gameControllerSet = false;

    public string keyword;
    //public string description;

    public void SetGameControllerReference(GameController controller)
    {
        gameController = controller;
        gameControllerSet = true;
    }

    public void InfoSelect()
    {
        if (gameControllerSet)
        {
            gameController.InfoDescription(this.keyword);
        }
    }
}
