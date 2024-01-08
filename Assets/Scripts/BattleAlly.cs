using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleAlly : BattleCharacter
{
    public Sprite portrait;
    public Image progressBar;

    public string[] skills = new string[3];

    public bool saidAttackDialogue;
    public bool saidSkillDialogue;
    public string attackDialogue;
    public string skillDialogue;

    public bool warded = false;

    public Button allyButton;

    public bool ready = false;

    public override void ReadyForTurn()
    {
        allyButton.interactable = true;
    }

    public void SetBattleDialogue(string attackDialogue, string skillDialogue)
    {
        this.attackDialogue = attackDialogue;
        this.skillDialogue = skillDialogue;
    }

    public void SetSkills(string[] skills)
    {
        this.skills = skills;
    }

    public override void UpdateProgress()
    {
        allyButton.interactable = false;
        currentCooldown = currentCooldown + Time.deltaTime;
        float calcCooldown = currentCooldown / maxCooldown;

        progressBar.transform.localScale = new Vector3(Mathf.Clamp(calcCooldown, 0, 1), progressBar.transform.localScale.y, progressBar.transform.localScale.z);

        if(currentCooldown >= maxCooldown)
        {
            turnState = "ready";
            ready = true;
            progressBar.color = Color.green;

            ReadyForTurn();
        }
    }
}
