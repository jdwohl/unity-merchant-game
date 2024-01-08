using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillDisplay : MonoBehaviour
{
    public TextMeshProUGUI skillText;
    public string skillName;

    public bool gameControllerSet = false;
    public GameController gameController;

    public void SetGameControllerReference(GameController controller)
    {
        gameController = controller;
        gameControllerSet = true;
    }

    public void SkillSelect()
    {
        if (gameControllerSet)
        {
            gameController.SelectTarget(skillName);
        }
    }
}
