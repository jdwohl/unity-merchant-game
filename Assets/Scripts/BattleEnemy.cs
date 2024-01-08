using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEnemy : BattleCharacter
{
    public string firstSkillDialogue;
    public string secondSkillDialogue;
    public string thirdSkillDialogue;

    public bool saidFirstDialogue = false;
    public bool saidSecondDialogue = false;

    public bool defenseLowered = false;

    public override void ReadyForTurn()
    {
        gameController.EnemyAction(this);
    }

    public void SetSkillDialogue(string[] skillDialogue)
    {
        firstSkillDialogue = skillDialogue[0];
        secondSkillDialogue = skillDialogue[1];
        thirdSkillDialogue = skillDialogue[2];
    }

    public override void UpdateProgress()
    {
        currentCooldown = currentCooldown + Time.deltaTime;
        float calcCooldown = currentCooldown / maxCooldown;

        if (currentCooldown >= maxCooldown)
        {
            turnState = "ready";

            ReadyForTurn();
        }
    }
}
