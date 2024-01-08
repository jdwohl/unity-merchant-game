using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MerchantGame/Customer")]
public class Customer : ScriptableObject
{
    public GameController gameController;
    // reference to the GameController object may be useful

    public string customerName;
    public string pronoun;

    public int friendship;
    // starts at 0, can go up or down depending on player choices
    public double agreeability;
    // double between 0 and 1, different for every customer
    public int readability;
    // int (0 to 3) representing how well the player can get a read on the customer
    public int highMaxSpending;
    public int realMaxSpending;
    // the maximum amount of money a customer is willing to spend on a given day
    // // bronze = 1, silver = 10, gold = 100 for calculation
    public int compromiseInterval;
    // sell prices between this value and realMaxSpending will trigger the compromise phase
    public string mainWantedItem;
    public string supplementalWantedItem;

    public string[] talkOptions = new string[4];
    // when the player clicks talk, these options for conversations will appear
    public string[] branchChoices = new string[12];
    // these are the options the player has when the customer asks a question

    const int MAX_DIALOGUE = 100;
    public string[] dialogue = new string[MAX_DIALOGUE];
    // all of the dialogue options from talking, not sure what the max should be yet
    public int currentDialogue;
    // this will go up as the player advances through text
    public int currentDialogueOption;

    public Information customerInfo;

    public int specialIndex = 99;
    public bool specialFlag = false;
    // used for secret purposes (scene/ending stuff)

    public int[] dialoguePortraits = new int[MAX_DIALOGUE];

    public int[] talkJumps = new int[4];
    public int[] endIndices = new int[12];
    public bool endFlag = false;
    public int[] questionIndices = new int[4];
    // when these indices are reached, the question flag is raised
    public int[] questionJumps = new int[12];
    // when the player answers a question, these will be used to jump to the correct location in the dialogue
    public bool questionFlag = false;
    // when a the customer asks a question, this will become true
    public int[] infoIndices = new int[1];
    public bool infoFlag = false;
    // when the customer gives the player useful information, this will become true
    public bool isTalking = false;
    // when the player is in the talking phase, this will become true
    public bool isBuying = false;
    // when the player is in the selling phase, this will become true
    public bool branchLocked;
    // true if the customer will have a dialogue branch locked behind information
    public int lockedIndex;
    // the talk option number that has its 3rd branch locked (-1 if no locked branch)
    public Information requiredInfo;
    // the information that, if the player has it, will unlock the locked branch

    public int[] friendshipIndices = new int[10];
    // when at these indices, the player will gain/lose friendship depending on value
    public int[] friendshipValues = new int[10];
    // these values are what determine friendship gain/loss at the corresponding indices
    public bool friendshipFlag = false;
    public int[] intuitionIndices = new int[10];
    public bool intuitionFlag = false;
    // this flag will be raised when the player should gain an intuition point
    public int[] orderIndices = new int[10];
    public int[] neutralIndices = new int[10];
    public int[] chaosIndices = new int[10];
    public bool orderFlag = false;
    public bool neutralFlag = false;
    public bool chaosFlag = false;
    // all used for the morality system

    public string[] buyDialogue = new string[20];
    public int correctSupplementalIndex;

    public bool wantsToTrade;
    public bool isTrading = false;

    public bool mainSaleWentThrough = false;
    public bool suppSaleWentThrough = false;
    public int saleDifference;

    public int[] buyPortraits = new int[20];

    public Sprite[] portraits = new Sprite[7];

    public string CustomerDialogue()
    {
        string dialogueToDisplay;

        currentDialogue++;

        dialogueToDisplay = dialogue[currentDialogue];
        CheckCustomerIndices();

        if (dialogueToDisplay != "")
        {
            return dialogueToDisplay;
        }
        else
        {
            return "error";
        }
    }

    private void CheckCustomerIndices()
    {
        for (int i = 0; i < questionIndices.Length; i++)
        {
            if (currentDialogue == questionIndices[i])
            {
                questionFlag = true;
            }
        }

        for (int i = 0; i < friendshipIndices.Length; i++)
        {
            if (currentDialogue == friendshipIndices[i])
            {
                friendshipFlag = true;
            }
        }

        for (int i = 0; i < intuitionIndices.Length; i++)
        {
            if (currentDialogue == intuitionIndices[i])
            {
                intuitionFlag = true;
            }
        }

        for (int i = 0; i < orderIndices.Length; i++)
        {
            if (currentDialogue == orderIndices[i])
            {
                orderFlag = true;
            }
        }

        for (int i = 0; i < neutralIndices.Length; i++)
        {
            if (currentDialogue == neutralIndices[i])
            {
                neutralFlag = true;
            }
        }

        for (int i = 0; i < chaosIndices.Length; i++)
        {
            if (currentDialogue == chaosIndices[i])
            {
                chaosFlag = true;
            }
        }

        for (int i = 0; i < infoIndices.Length; i++)
        {
            if(currentDialogue == infoIndices[i])
            {
                infoFlag = true;
            }
        }

        for (int i = 0; i < endIndices.Length; i++)
        {
            if (currentDialogue == endIndices[i])
            {
                endFlag = true;
            }
        }

        if(currentDialogue == specialIndex)
        {
            specialFlag = true;
        }
    }

    public string CustomerDialogue(int jumpLocation)
    {
        string dialogueToDisplay = "jumped";
        currentDialogue = jumpLocation;

        CheckCustomerIndices();

        dialogueToDisplay = dialogue[currentDialogue];

        questionFlag = false;

        return dialogueToDisplay;
    }

    public string ResetToIdle()
    {
        currentDialogue = 0;
        return dialogue[currentDialogue];
    }

    public bool CheckItemsToSell(LinkedList<string> itemsToSell)
    {
        foreach(string item in itemsToSell)
        {
            if(!(item == mainWantedItem || item == supplementalWantedItem))
            {
                return false;
            }

            //bool itemIsWanted = false;
            //for (int i = 0; i < wantedItems.Length; i++)
            //{
            //    if(item == wantedItems[i])
            //    {
            //        itemIsWanted = true;
            //    }
            //}
            //if (!itemIsWanted)
            //{
            //    return false;
            //}
        }
        return true;
    }

    public void GenerateRealMaxSpending(int buyPriceTotal)
    {
        highMaxSpending = buyPriceTotal * 2;

        System.Random rand = new System.Random();
        int randomNumber = rand.Next(1, 11);

        int adjustment = (int)((randomNumber + friendship) * agreeability);
        realMaxSpending = ((int)(highMaxSpending / 1.35)) + adjustment;
        if(realMaxSpending > highMaxSpending)
        {
            realMaxSpending = highMaxSpending;
        }
        compromiseInterval = (int)(realMaxSpending * .8);
    }

    public void ResetCustomer()
    {
        friendship = 0;
        currentDialogue = 0;
        currentDialogueOption = 0;
        questionFlag = false;
        friendshipFlag = false;
        intuitionFlag = false;
        infoFlag = false;
        orderFlag = false;
        neutralFlag = false;
        chaosFlag = false;
        isTalking = false;
        isBuying = false;
        mainSaleWentThrough = false;
        suppSaleWentThrough = false;
        saleDifference = 0;
    }
}
