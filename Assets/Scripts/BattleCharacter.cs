using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class BattleCharacter : MonoBehaviour
{
    public string characterName;
    public bool characterSet = false;
    public bool knockedOut = false;

    public int maxHP;
    public int currentHP;
    public int maxMP;
    public int currentMP;
    public int strength;
    public int defense;
    public int magicStrength;
    public int magicDefense;
    public int speed;
    public int level;

    public Items weapon;

    public string position;

    public string turnState = "waiting";
    public float currentCooldown = 0f;
    public float maxCooldown = 10f;

    public GameObject characterText;
    public TextMeshProUGUI HPText;
    public TextMeshProUGUI MPText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI nameText;

    public GameObject progressDisplay;

    public GameController gameController;

    public bool charged = false;

    public abstract void ReadyForTurn();

    public void SetStats(string name, int[] stats, Items weapon, string position)
    {
        characterName = name;

        maxHP = stats[0];
        currentHP = maxHP;
        maxMP = stats[1];
        currentMP = maxMP;
        currentHP = maxHP;
        strength = stats[2];
        defense = stats[3];
        magicStrength = stats[4];
        magicDefense = stats[5];
        speed = stats[6];
        level = stats[7];

        maxCooldown = 10f - (0.5f * speed);

        this.weapon = weapon;
        this.position = position;

        HPText.text = "HP: " + maxHP + "/" + maxHP;
        MPText.text = "MP: " + maxMP + "/" + maxMP;
        levelText.text = "Lv. " + level;
        nameText.text = characterName;

        characterSet = true;
    }

    public void SetNull()
    {
        characterText.SetActive(false);
    }

    public void ActivateProgressBar()
    {
        progressDisplay.SetActive(true);
    }

    private void Update()
    {
        if (characterSet)
        {
            if (turnState == "waiting")
            {
                UpdateProgress();
            }
        }
    }

    public abstract void UpdateProgress();
}
