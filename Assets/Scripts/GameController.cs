using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using System.Linq;

public class GameController : MonoBehaviour
{
    public GameObject merchantView;
    public GameObject battleView;

    public GameObject dialoguePanel;
    public TextMeshProUGUI displayedText;
    public TextMeshProUGUI customerNameText;
    public GameObject talkPanels;
    public Button[] talkButtons = new Button[4];
    public TextMeshProUGUI[] talkChoices = new TextMeshProUGUI[4];
    public GameObject questionPanels;
    public Button[] questionButtons = new Button[3];
    public TextMeshProUGUI[] questionChoices = new TextMeshProUGUI[3];
    public GameObject sellDisplay;
    public GameObject sellInventoryDisplay;
    public GameObject priceDisplay;
    public GameObject supplementalDisplay;
    public GameObject tradeQuestionDisplay;
    public Button[] tradeQuestionButtons = new Button[2];
    public TextMeshProUGUI[] tradeQuestionChoices = new TextMeshProUGUI[2];
    public GameObject idleDisplay;
    public GameObject inventoryDisplay;
    public GameObject itemInventoryDisplay;
    public GameObject infoInventoryDisplay;
    public GameObject intuitionDisplay;
    public GameObject spriteDisplay;
    public Image portrait;
    public GameObject statsDisplay;
    public TextMeshProUGUI statUpdateText;
    public TextMeshProUGUI[] supplementalOptions = new TextMeshProUGUI[3];
    public GameObject sellHealthDisplay;
    public TextMeshProUGUI sellHealthText;
    public TextMeshProUGUI goldDisplayText;
    public TextMeshProUGUI silverDisplayText;
    public TextMeshProUGUI bronzeDisplayText;
    public GameObject buyPriceDisplay;
    public TextMeshProUGUI buyPriceText;
    public GameObject sceneQuestionDisplay;
    public Button[] sceneQuestionButtons = new Button[2];
    public TextMeshProUGUI[] sceneQuestionOptions = new TextMeshProUGUI[2];

    public FadeToBlack fadeToBlack;

    public int[] morality = new int[3];
    // first element is order, second element is neutral, third element is chaos
    public int[] money = new int[3];
    // first element is gold, second element is silver, third element is bronze
    public int intuition;
    public int salesCompleted = 0;

    public Day[] days = new Day[5];

    public Dictionary<string, Information> infoDictionary = new Dictionary<string, Information>();
    public Dictionary<string, Items> itemDictionary = new Dictionary<string, Items>();

    public Information[] allInfo = new Information[10];
    public GameObject infoDisplay;
    public TextMeshProUGUI infoText;
    public Items[] initialItems = new Items[12];
    public LinkedList<Items> inventory;
    public Items[] allItems = new Items[24];
    // used to populate item dictionary
    public GameObject itemDisplay;
    public TextMeshProUGUI itemText;

    public int totalSalePriceProportionDifference = 0;
    // used to calculate rank at the end

    public int currentDay;
    public int currentCustomerIndex;
    public Customer currentCustomer;
    public int dialogueOption;
    public bool advancingEnabled = true;
    public bool toldPracticeInfo = false;

    public LinkedList<Information> playerInfo = new LinkedList<Information>();

    public int buyPriceTotal;
    // combined purchase price of all the items that the user is attempting to sell
    public int[] suggestedPrice = new int[3];
    // price given to customer in gold, silver, and bronze coins
    public int suggestedPriceTotal;
    // suggested price converted to one number for convenience
    public LinkedList<string> itemsToSell = new LinkedList<string>();
    public int sellHealth = 10;
    public int buyDialogueIndex;
    public bool isUpdatingInventory = false;
    public bool isUpdatingPrice = false;
    public bool isCompromising = false;
    public bool isTrading = false;
    public int buyerSuggestion;
    public bool hasCompromised = false;
    // this will be true if the player has tried and failed to compromise
    public bool supplementalDialogue = false;
    // this will be true when the player will press a button to advance during supplemental phase (first time)
    public bool correctSupplementalAnswer = false;
    // this will be true when the player chose the correct supplemental option
    public bool supplementalSaleReady = false;
    // this represents if the player advances to the price phase with the supplemental item being sold
    public bool supplementalPrice = false;
    // this will be true the second time
    public bool triedSupplemental = false;
    // this will be true if the player has tried and failed to sell a supplemental item
    public bool tradeDecisionMade = false;
    public bool tradeCompleted = false;
    public bool saleCompleted = false;

    public double intuitionRandomDouble;

    public double debugRandomDouble;
    public double debugChanceDouble;

    public int sceneCount = 0;
    // counter, increments when scenes end
    public string sceneRoute = "a";
    // changes depending on player's choices
    public Scene currentScene;
    public bool inScene = false;
    public bool dayEnded = false;

    public Button[] battleAllies = new Button[3];
    public Button[] battleEnemies = new Button[3];
    public bool isBattling = false;
    public GameObject allyActionDisplay;
    public Image battlePortrait;
    public GameObject allySkillDisplay;
    public GameObject allySkillList;
    public GameObject skillButton;
    public TextMeshProUGUI skillText;


    public GameObject allyDialogueDisplay;
    public TextMeshProUGUI allyDialogue;
    public GameObject enemyDialogueDisplay;
    public TextMeshProUGUI enemyDialogue;
    public GameObject resultTextDisplay;
    public TextMeshProUGUI resultText;
    public GameObject battleHelpDisplay;
    public TextMeshProUGUI battleHelp;

    public int totalPotions = 0;
    public BattleAlly currentAllySelected;
    public string currentActionSelected;
    public bool tryHealAlly = false;

    public AudioSource merchantAudio;
    public AudioSource suspenseAudio;
    public AudioSource battleAudio;
    public AudioSource happyAudio;
    public AudioSource bossAudio;

    public AudioClip hitClip;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(fadeToBlack.FadeInAndOutBlackSquare(2, "Day " + (currentDay + 1)));
        ResetCustomers();
        SetGameControllerReferenceOnItemDisplay();
        SetGameControllerReferenceOnInfoDisplay();
        SetGameControllerReferenceOnSkillDisplay();
        
        currentCustomer = FindCurrentCustomer();
        customerNameText.text = currentCustomer.customerName;
        PopulateInfoDictionary();
        PopulateItemDictionary();

        inventory = new LinkedList<Items>(initialItems);

        string textToDisplay = "controller text display";
        textToDisplay = currentCustomer.CustomerDialogue(0);

        System.Random rand = new System.Random();
        intuitionRandomDouble = rand.NextDouble();

        displayedText.text = textToDisplay;
        advancingEnabled = false;

        IdleMode();
        //currentScene = days[0].scenes[0];
        //displayedText.text = currentScene.sceneDialogue[0];
        //inScene = true;

        
    }

    // Update is called once per frame
    void Update()
    {
        String textToDisplay = "controller text display";
        bool questionFlag = currentCustomer.questionFlag;

        if (currentCustomer.friendshipFlag)
        {
            currentCustomer.friendshipFlag = false;
            currentCustomer.friendship++;
            StartCoroutine(StatsCoroutine(0));
        }
        else if (currentCustomer.intuitionFlag)
        {
            currentCustomer.intuitionFlag = false;
            intuition++;
            StartCoroutine(StatsCoroutine(1));
        }
        else if (currentCustomer.orderFlag)
        {
            currentCustomer.orderFlag = false;
            morality[0]++;
            StartCoroutine(StatsCoroutine(2));
        }
        else if (currentCustomer.neutralFlag)
        {
            currentCustomer.neutralFlag = false;
            morality[1]++;
            StartCoroutine(StatsCoroutine(2));
        }
        else if (currentCustomer.chaosFlag)
        {
            currentCustomer.chaosFlag = false;
            morality[2]++;
            StartCoroutine(StatsCoroutine(2));
        }
        else if (currentCustomer.infoFlag)
        {
            currentCustomer.infoFlag = false;
            StartCoroutine(StatsCoroutine(3));
            playerInfo.AddLast(currentCustomer.customerInfo);
        }

        if (currentCustomer.specialFlag)
        {
            currentCustomer.specialFlag = false;
            toldPracticeInfo = true;
        }

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.RightArrow)) && currentCustomer.endFlag) {
            currentCustomer.endFlag = false;
            currentCustomer.currentDialogue = 0;
            textToDisplay = currentCustomer.dialogue[currentCustomer.currentDialogue];
            displayedText.text = textToDisplay;
            TalkMode();
        } // add else if for scene end flag raised
        else if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.RightArrow)) && inScene && currentScene.sceneEndFlag)
        {
            currentScene.sceneEndFlag = false;
            SceneManager();
        }
        else if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.RightArrow)) && inScene && currentScene.sceneQuestionFlag)
        {
            SceneQuestionMode();
        } else if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.RightArrow)) && inScene)
        {
            displayedText.text = currentScene.AdvanceSceneDialogue();

            if (currentScene.changeMusicFlag)
            {
                AudioManager(currentScene.musicToPlay);
                currentScene.changeMusicFlag = false;
            }

            if (currentScene.characterTalking[currentScene.sceneDialogueIndex] != 99)
            {
                spriteDisplay.SetActive(true);
                customerNameText.text = currentScene.characters[currentScene.characterTalking[currentScene.sceneDialogueIndex]].customerName;
                portrait.sprite = currentScene.characters[currentScene.characterTalking[currentScene.sceneDialogueIndex]]
                    .portraits[currentScene.characterPortrait[currentScene.sceneDialogueIndex]];
            } else
            {
                spriteDisplay.SetActive(false);
            }
        } else if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.RightArrow)) && advancingEnabled && !questionFlag && !isTrading) {
            textToDisplay = currentCustomer.CustomerDialogue();
            UpdateDialoguePortrait();
            displayedText.text = textToDisplay;
        } else if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.RightArrow)) && advancingEnabled && questionFlag) {
            QuestionMode();
        } else if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.RightArrow)) && supplementalDialogue) {
            if (correctSupplementalAnswer)
            {
                SetBuyDialogue(18);
                UpdateSellPortrait(18);
                spriteDisplay.SetActive(true);
            }
            else
            {
                SetBuyDialogue(17);
                UpdateSellPortrait(17);
                spriteDisplay.SetActive(true);
            }
            // change item list if supplemental answer was incorrect
            correctSupplementalAnswer = false;
            supplementalDialogue = false;
            supplementalPrice = true;
        } else if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.RightArrow)) && supplementalPrice)
        {
            supplementalPrice = false;
            UpdatePrice();
        } else if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.RightArrow)) && saleCompleted)
        {
            saleCompleted = false;
            SaleCompleted();
        }
        else if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.RightArrow)) && isTrading && advancingEnabled && tradeCompleted)
        {
            tradeCompleted = false;
            isTrading = false;
            SaleCompleted();
        }
        else if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.RightArrow)) && isTrading && advancingEnabled && tradeDecisionMade)
        {
            tradeDecisionMade = false;
            tradeCompleted = true;
            SetBuyDialogue(16);
            UpdateSellPortrait(16);
        }
        else if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.RightArrow)) && isTrading && advancingEnabled)
        {
            AdvanceTrade();
        } else if((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.RightArrow)) && isTrading)
        {
            TradeQuestion();
        }
        // the below inputs were used for testing purposes

        //else if (Input.GetKeyDown(KeyCode.LeftArrow))
        //{
        //    SceneManager();
        //}
        //else if (Input.GetKeyDown(KeyCode.RightShift))
        //{
        //    SaleCompleted();
        //}
        //else if (Input.GetKeyDown(KeyCode.UpArrow))
        //{
        //    EndBattle("victory");
        //}
        //else if (Input.GetKeyDown(KeyCode.LeftShift))
        //{
        //    NextDay();
        //}
        //else if (Input.GetKeyDown(KeyCode.DownArrow))
        //{
        //    EndBattle("game over");
        //}

    }

    // ***** MERCHANT METHODS ***** //

    public void IdleMode()
    {
        talkPanels.SetActive(false);
        sellDisplay.SetActive(false);
        inventoryDisplay.SetActive(false);
        intuitionDisplay.SetActive(false);

        currentCustomer.currentDialogue = 0;
        displayedText.text = currentCustomer.dialogue[currentCustomer.currentDialogue];

        UpdateDialoguePortrait();
        spriteDisplay.SetActive(true);
        idleDisplay.SetActive(true);
    }

    public void UpdateDialoguePortrait()
    {
        int currentPortraitIndex = currentCustomer.dialoguePortraits[currentCustomer.currentDialogue];
        Sprite newPortrait = currentCustomer.portraits[currentPortraitIndex];

        this.portrait.sprite = newPortrait;
    }

    IEnumerator StatsCoroutine(int stat)
    {
        string textToDisplay = "coroutine text";
        switch (stat)
        {
            case 0:
                textToDisplay = "Friendship increased!";
                break;
            case 1:
                textToDisplay = "Intuition increased!";
                break;
            case 2:
                textToDisplay = "Morality strengthened.";
                break;
            case 3:
                textToDisplay = "Information gained.";
                break;
            case 4:
                textToDisplay = "Friendship decreased.";
                break;
            case 5:
                textToDisplay = "Sale failed.";
                break;
            case 6:
                textToDisplay = "One week later...";
                break;
            case 7:
                textToDisplay = "Game over -- reset!";
                break;
            default:
                break;
        }

        statUpdateText.text = textToDisplay;

        statsDisplay.SetActive(true);

        yield return new WaitForSeconds(4);

        statsDisplay.SetActive(false);
    }

    // Dialogue phase

    public void TalkMode()
    {
        idleDisplay.SetActive(false);
        spriteDisplay.SetActive(false);
        advancingEnabled = false;

        for (int i = 0; i < talkChoices.Length; i++)
        {
            talkChoices[i].text = currentCustomer.talkOptions[i];
        }

        talkPanels.SetActive(true);
    }

    public void QuestionMode()
    {
        Image button2Image = questionButtons[2].GetComponent<Image>();
        button2Image.color = Color.black;
        // button2Color = Color.green;
        // questionButtons[2].colors = button2Colors;

        advancingEnabled = false;

        for (int i = 0; i < questionChoices.Length; i++)
        {
            questionChoices[i].text = currentCustomer.branchChoices[(3 * currentCustomer.currentDialogueOption) + i];
        }
        
        if(currentCustomer.currentDialogueOption == currentCustomer.lockedIndex && currentCustomer.branchLocked)
        {
            if (!playerInfo.Contains(currentCustomer.requiredInfo))
            {
                questionButtons[2].interactable = false;
            }
            else
            {
                button2Image.color = Color.green;
            }
        }

        spriteDisplay.SetActive(false);
        questionPanels.SetActive(true);
    }

    public void JumpToDialogue(int option)
    {
        dialogueOption = option;

        String textToDisplay = "controller text display";

        int jumpLocation = currentCustomer.talkJumps[option];
        currentCustomer.currentDialogueOption = option;

        talkButtons[option].interactable = false;
        // disabled for testing purposes

        talkPanels.SetActive(false);
        textToDisplay = currentCustomer.CustomerDialogue(jumpLocation);
        displayedText.text = textToDisplay;
        UpdateDialoguePortrait();
        spriteDisplay.SetActive(true);
        advancingEnabled = true;
    }

    public void JumpToBranch(int option)
    {
        String textToDisplay = "controller text display";

        //int talkIndex = (4 * dialogueOption) + option;

        //int jumpLocation = currentCustomer.talkJumps[talkIndex];

        int jumpLocation = currentCustomer.questionJumps[(3*currentCustomer.currentDialogueOption) + option];

        questionButtons[2].interactable = true;

        questionPanels.SetActive(false);
        textToDisplay = currentCustomer.CustomerDialogue(jumpLocation);
        displayedText.text = textToDisplay;
        UpdateDialoguePortrait();
        spriteDisplay.SetActive(true);
        advancingEnabled = true;
    }

    // Inventory (first method used in selling as well)

    public void InventoryMode()
    {
        idleDisplay.SetActive(false);
        spriteDisplay.SetActive(false);

        infoInventoryDisplay.SetActive(false);
        inventoryDisplay.SetActive(true);

        int i = 0;
        foreach (Items item in inventory)
        {
            if (item != null)
            {
                GameObject currentItemDisplay = Instantiate<GameObject>(itemDisplay, itemInventoryDisplay.transform, false);
                currentItemDisplay.transform.position += Vector3.down * (28f * (i + 1));

                ItemDisplay itemScript = currentItemDisplay.GetComponent<ItemDisplay>();
                itemScript.itemText.text = item.keyword;
                itemScript.itemName = item.keyword;
            }
            i++;
        }

        itemInventoryDisplay.SetActive(true);
    }

    public void InfoMode()
    {
        itemInventoryDisplay.SetActive(false);

        PopulateInfoScreen();

        infoInventoryDisplay.SetActive(true);
    }

    public void PopulateInfoScreen()
    {
        int i = 0;
        foreach (Information info in playerInfo)
        {
            if (info != null)
            {
                GameObject currentInfoDisplay = Instantiate<GameObject>(infoDisplay, infoInventoryDisplay.transform, false);
                currentInfoDisplay.transform.position += Vector3.down * (50f * (i + 1));

                InfoDisplay infoScript = currentInfoDisplay.GetComponent<InfoDisplay>();
                infoScript.infoText.text = info.keyword;
                infoScript.keyword = info.keyword.ToLower();
            }
            i++;
        }
    }

    public void SetGameControllerReferenceOnInfoDisplay()
    {
        InfoDisplay infoScript = infoDisplay.GetComponent<InfoDisplay>();
        infoScript.SetGameControllerReference(this);
    }

    public void InfoDescription(string keyword)
    {
        displayedText.text = infoDictionary[keyword].description;
    }

    // Intuition

    public void IntuitionMode()
    {
        spriteDisplay.SetActive(false);
        idleDisplay.SetActive(false);
        intuitionDisplay.SetActive(true);

        if (currentCustomer.customerName == "Corwin")
        {
            displayedText.text = "It seems like Corwin doesn't want to buy anything? He does want something from " +
                "me though. Hmmm...";
        }
        else if (currentCustomer.readability == 0)
        {
            displayedText.text = "Wow, I can't get a read on this girl at all. I wonder what she's thinking...";
        }
        else
        {
            int combinedBuyPriceTotal = itemDictionary[currentCustomer.mainWantedItem].buyPrice + itemDictionary[currentCustomer.supplementalWantedItem].buyPrice;
            // buy price of main and supplemental item
            string supplementalDescription = "";
            switch (currentCustomer.readability)
            {
                case 1:
                    // Leon
                    supplementalDescription = "weapon of some kind";
                    break;
                case 2:
                    // Vera
                    supplementalDescription = "long range weapon";
                    break;
                case 3:
                    // Jack + Sentinel Jack
                    supplementalDescription = currentCustomer.supplementalWantedItem;
                    break;
                default:
                    supplementalDescription = "item";
                    // this case should never happen, but just in case
                    break;
            }

            displayedText.text = "I have a feeling that " + currentCustomer.customerName +
                " would also buy a " + supplementalDescription + " ... I think the most " +
                "that " + currentCustomer.pronoun + " would be willing to pay for both is somewhere around ";

            // next part should display a better average value for real max spending depending on intuition value
            int variance = 6 - intuition;
            int middleOfRange = (int)(((combinedBuyPriceTotal * 2) / 1.35) + ((5 + currentCustomer.friendship) * currentCustomer.agreeability));
            // use formula for calculating real max spending with 5 as a substitute for a random int between 1 and 10
            int projectedRealMaxSpending;

            if (intuitionRandomDouble > .5)
            {
                projectedRealMaxSpending = middleOfRange + variance;
            } else
            {
                projectedRealMaxSpending = middleOfRange - variance;
            }

            if (projectedRealMaxSpending >= 100)
            {
                int intuitionGold = (int)(projectedRealMaxSpending / 100);
                displayedText.text += intuitionGold + " gold";
                projectedRealMaxSpending -= 100 * intuitionGold;
                if (projectedRealMaxSpending > 0) { displayedText.text += ", "; }
            }

            if (projectedRealMaxSpending >= 10)
            {
                int intuitionSilver = (int)(projectedRealMaxSpending / 10);
                displayedText.text += intuitionSilver + " silver";
                projectedRealMaxSpending -= 10 * intuitionSilver;
                if (projectedRealMaxSpending > 0) { displayedText.text += ", "; }
            }

            if (projectedRealMaxSpending >= 1)
            {
                int intuitionBronze = projectedRealMaxSpending;
                displayedText.text += intuitionBronze + " bronze";
            }
            displayedText.text += ".";
        }
    }

    // Sell phase

    public void SellMode()
    {
        idleDisplay.SetActive(false);

        if (!currentCustomer.wantsToTrade)
        {
            sellHealthDisplay.SetActive(true);
            advancingEnabled = false;
            // currentCustomer.GenerateRealMaxSpending();
            sellDisplay.SetActive(true);
            SetBuyDialogue(0);

            PopulateSellInventory();
            UpdateInventory();
        } else
        {
            TradePhase();
        }
    }

    public void UpdateSellPortrait(int index)
    {
        int currentPortraitIndex = currentCustomer.buyPortraits[index];
        Sprite newPortrait = currentCustomer.portraits[currentPortraitIndex];

        this.portrait.sprite = newPortrait;
    }

    public void SetGameControllerReferenceOnItemDisplay()
    {
        ItemDisplay itemScript = itemDisplay.GetComponent<ItemDisplay>();
        itemScript.SetGameControllerReference(this);
    }

    public void PopulateSellInventory()
    {
        int i = 0;
        foreach (Items item in inventory)
        {
            if (item != null)
            {
                GameObject currentItemDisplay = Instantiate<GameObject>(itemDisplay, sellInventoryDisplay.transform, false);
                currentItemDisplay.transform.position += Vector3.down * (28f * (i + 1));

                ItemDisplay itemScript = currentItemDisplay.GetComponent<ItemDisplay>();
                itemScript.itemText.text = item.keyword;
                itemScript.itemName = item.keyword;
            }
            i++;
        }
    }

    public void UpdateInventory()
    {
        spriteDisplay.SetActive(false);
        sellInventoryDisplay.SetActive(true);
        buyPriceDisplay.SetActive(true);
    }

    public void StopUpdatingInventory()
    {
        sellInventoryDisplay.SetActive(false);
        if (!itemsToSell.Contains(currentCustomer.mainWantedItem))
        {
            SetBuyDialogue(1);
            DecrementSellHealth(1);
            UpdateInventory();
        } else if(!currentCustomer.CheckItemsToSell(itemsToSell))
        {
            SetBuyDialogue(2);
            DecrementSellHealth(1);
            UpdateInventory();
        } else if(itemsToSell.Count > 1)
        { // supplemental item is being sold
            SupplementalPhase();
        } else // only main item
        {
            currentCustomer.GenerateRealMaxSpending(buyPriceTotal);
            UpdatePrice();
        }
    }

    public void AddOrRemoveItem(string name, bool add)
    {
        if (add)
        {
            itemsToSell.AddFirst(name.ToLower());
            AddBuyPrice(name.ToLower());
        } else
        {
            itemsToSell.Remove(name.ToLower());
            SubtractBuyPrice(name.ToLower());
        }
    }

    public void AddBuyPrice(string name)
    {
        int itemBuyPrice = itemDictionary[name].buyPrice;
        buyPriceTotal += itemBuyPrice;
        UpdateBuyPriceDisplay();
    }

    public void SubtractBuyPrice(string name)
    {
        int itemBuyPrice = itemDictionary[name].buyPrice;
        buyPriceTotal -= itemBuyPrice;
        if(buyPriceTotal < 0)
        {
            buyPriceTotal = 0;
        }
        UpdateBuyPriceDisplay();
    }

    public void UpdateBuyPriceDisplay()
    {
        int currentBuyPriceTotal = buyPriceTotal;

        string buyPriceString = "Item Buy Price: ";

        if (currentBuyPriceTotal >= 100)
        {
            int currentBuyPriceGold = (int)(currentBuyPriceTotal / 100);
            buyPriceString += currentBuyPriceGold + " gold";
            currentBuyPriceTotal -= 100 * currentBuyPriceGold;
            if (currentBuyPriceTotal > 0) { buyPriceString += ", "; }
        }

        if (currentBuyPriceTotal >= 10)
        {
            int currentBuyPriceSilver = (int)(currentBuyPriceTotal / 10);
            buyPriceString += currentBuyPriceSilver + " silver";
            currentBuyPriceTotal -= 10 * currentBuyPriceSilver;
            if (currentBuyPriceTotal > 0) { buyPriceString += ", "; }
        }

        if (currentBuyPriceTotal >= 1)
        {
            int currentBuyPriceBronze = currentBuyPriceTotal;
            buyPriceString += currentBuyPriceBronze + " bronze";
        }

        buyPriceText.text = buyPriceString;
    }

    public void SupplementalPhase()
    {
        buyPriceDisplay.SetActive(false);
        SetBuyDialogue(10);
        spriteDisplay.SetActive(false);
        supplementalDisplay.SetActive(true);

        supplementalOptions[0].text = currentCustomer.buyDialogue[11];
        supplementalOptions[1].text = currentCustomer.buyDialogue[12];
        supplementalOptions[2].text = currentCustomer.buyDialogue[13];
    }

    public void SupplementalChoice(int option)
    {
        SetBuyDialogue(14 + option);
        if(14 + option == currentCustomer.correctSupplementalIndex)
        {
            correctSupplementalAnswer = true;
            supplementalSaleReady = true;
        } else
        {
            while (itemsToSell.Contains(currentCustomer.supplementalWantedItem))
            {
                AddOrRemoveItem(currentCustomer.supplementalWantedItem, false);
            }
        }
        currentCustomer.GenerateRealMaxSpending(buyPriceTotal);
        supplementalDialogue = true;
        supplementalDisplay.SetActive(false);
        UpdateSellPortrait(14 + option);
        spriteDisplay.SetActive(true);
    }

    public void UpdatePrice()
    {
        SetBuyDialogue(3);
        spriteDisplay.SetActive(false);
        priceDisplay.SetActive(true);
        buyPriceDisplay.SetActive(true);
    }

    public void IncrementSaleSuggestion(int index)
    {
        if (suggestedPrice[index] < 10)
        {
            suggestedPrice[index]++;
        }
        UpdateSuggestedPriceDisplay();
    }

    public void DecrementSaleSuggestion(int index)
    {
        if (suggestedPrice[index] > 0)
        {
            suggestedPrice[index]--;
        }
        UpdateSuggestedPriceDisplay();
    }

    public void UpdateSuggestedPriceDisplay()
    {
        goldDisplayText.text = "Gold: " + suggestedPrice[0];
        silverDisplayText.text = "Silver: " + suggestedPrice[1];
        bronzeDisplayText.text = "Bronze: " + suggestedPrice[2];
    }

    public void SuggestPrice()
    {
        suggestedPriceTotal = (suggestedPrice[0] * 100) + (suggestedPrice[1] * 10) + suggestedPrice[2];
        if (isCompromising)
        {
            SuggestCompromise();
        }
        else if(suggestedPriceTotal > currentCustomer.realMaxSpending)
        { // price was too high
            SetBuyDialogue(5);
            DecrementSellHealth(2);
        } else if (suggestedPriceTotal >= currentCustomer.compromiseInterval)
        { // Compromise phase
            SetBuyDialogue(6);
            CompromisePhase();
        } else
        { // sale complete
            SetBuyDialogue(4);
            UpdateSellPortrait(4);
            sellDisplay.SetActive(false);
            buyPriceDisplay.SetActive(false);
            spriteDisplay.SetActive(true);
            saleCompleted = true;

            if (supplementalSaleReady)
            {
                currentCustomer.suppSaleWentThrough = true;
            }
            currentCustomer.mainSaleWentThrough = true;
            currentCustomer.friendship++;
        }
    }

     public void CompromisePhase()
    {
        buyerSuggestion = suggestedPriceTotal - 4;
        int tempBuyerSuggestion = buyerSuggestion;
        string buyerSuggestionString = "What about... ";
        if(suggestedPriceTotal >= 100)
        {
            int buyerSuggestedGold = (int)(tempBuyerSuggestion / 100);
            buyerSuggestionString += buyerSuggestedGold + " gold";
            tempBuyerSuggestion -= 100 * buyerSuggestedGold;
            if(tempBuyerSuggestion > 0) { buyerSuggestionString += ", "; }
        }

        if (suggestedPriceTotal >= 10)
        {
            int buyerSuggestedSilver = (int)(tempBuyerSuggestion / 10);
            buyerSuggestionString += buyerSuggestedSilver + " silver";
            tempBuyerSuggestion -= 10 * buyerSuggestedSilver;
            if (tempBuyerSuggestion >= 0) { buyerSuggestionString += ", "; }
        }

        if (suggestedPriceTotal >= 1)
        {
            int buyerSuggestedBronze = tempBuyerSuggestion;
            buyerSuggestionString += buyerSuggestedBronze + " bronze";
        }
        buyerSuggestionString += "?";
        displayedText.text = buyerSuggestionString;

        isCompromising = true;
    }

    public void SuggestCompromise()
    {
        if (suggestedPriceTotal >= buyerSuggestion * 1.2)
        {
            suggestedPriceTotal--; // decrement so that buyer asks for less
            SetBuyDialogue(6);
            CompromisePhase();
            DecrementSellHealth(2);
        }
        else if (suggestedPriceTotal <= buyerSuggestion)
        {
            SetBuyDialogue(7);
            UpdateSellPortrait(7);
            spriteDisplay.SetActive(true);
            sellDisplay.SetActive(false);

            saleCompleted = true;
            if (supplementalSaleReady)
            {
                currentCustomer.suppSaleWentThrough = true;
            }
            currentCustomer.mainSaleWentThrough = true;
        }
        else
        { // in between the two intervals
            System.Random rand = new System.Random();
            double randomDouble = rand.NextDouble();
            double buyerInterval = (buyerSuggestion * .2);
            double suggestionDifference = suggestedPriceTotal - buyerSuggestion;

            double chanceOfSuccess = 1 - (suggestionDifference / buyerInterval);
            chanceOfSuccess *= currentCustomer.agreeability;
            // decrease chance of success if customer agreeability is low

            debugRandomDouble = randomDouble;
            debugChanceDouble = chanceOfSuccess;

            if(randomDouble <= chanceOfSuccess)
            {
                SetBuyDialogue(8);
                UpdateSellPortrait(8);
                spriteDisplay.SetActive(true);
                sellDisplay.SetActive(false);
                saleCompleted = true;
                if (supplementalSaleReady)
                {
                    currentCustomer.suppSaleWentThrough = true;
                }
                currentCustomer.mainSaleWentThrough = true;
            } else
            {
                DecrementSellHealth(1);
                suggestedPriceTotal--;
            }
        }
    }
    // // check if size of itemsToSell > 1 (more than main item), call SupplementalSale() if so
    // // otherwise, show text that displays a lower value than the user put along with compromise dialogue

    // (1 - distance from buyer suggestion/10) + (friendship/100) * agreeability = % of sale success

    public void SetBuyDialogue(int index)
    {
        displayedText.text = currentCustomer.buyDialogue[index];
        buyDialogueIndex = index;
    }

    public void TradePhase()
    {
        isTrading = true;
        SetBuyDialogue(buyDialogueIndex);
        UpdateSellPortrait(buyDialogueIndex);
        advancingEnabled = true;
    }

    public void AdvanceTrade()
    {
        buyDialogueIndex++;
        SetBuyDialogue(buyDialogueIndex);
        UpdateSellPortrait(buyDialogueIndex);
        if(buyDialogueIndex == 11)
        {
            advancingEnabled = false;
        }
    }

    public void TradeQuestion()
    {
        for (int i = 0; i < tradeQuestionChoices.Length; i++)
        {
            tradeQuestionChoices[i].text = currentCustomer.buyDialogue[12 + i];
        }
        spriteDisplay.SetActive(false);
        tradeQuestionDisplay.SetActive(true);
    }

    public void TradeDecision(bool decision)
    {
        if (decision)
        {
            SetBuyDialogue(14);
            UpdateSellPortrait(14);
            inventory.Remove(itemDictionary[currentCustomer.mainWantedItem]);
            inventory.AddLast(itemDictionary["iron shield"]);
        } else
        {
            SetBuyDialogue(15);
            UpdateSellPortrait(15);
        }
        tradeQuestionDisplay.SetActive(false);
        spriteDisplay.SetActive(true);
        advancingEnabled = true;
        tradeDecisionMade = true;
    }

    public void DecrementSellHealth(int value)
    {
        sellHealth -= value;

        // want to decrement friendship at the thresholds of 8, 6, 4, 2, and 0 sell health
        if(sellHealth <= 0)
        {
            currentCustomer.friendship--;
            StartCoroutine(StatsCoroutine(5));
            // prioritize saying sale failed over friendship loss (though both occur)
            UpdateSellHealthDisplay();
            SaleCompleted();
            // calling sale completed without going through the normal process should work
        } else if (sellHealth <= 2)
        {
            currentCustomer.friendship--;
            StartCoroutine(StatsCoroutine(4));
            UpdateSellHealthDisplay();
        }
        else if (sellHealth <= 4)
        {
            currentCustomer.friendship--;
            StartCoroutine(StatsCoroutine(4));
            UpdateSellHealthDisplay();
        }
        else if (sellHealth <= 6)
        {
            currentCustomer.friendship--;
            StartCoroutine(StatsCoroutine(4));
            UpdateSellHealthDisplay();
        }
        else if (sellHealth <= 8)
        {
            currentCustomer.friendship--;
            StartCoroutine(StatsCoroutine(4));
            UpdateSellHealthDisplay();
        }

        if (currentCustomer.friendship < 0)
        { // lower bound of 0 on customer friendship
            currentCustomer.friendship = 0;
        }
    }

    public void UpdateSellHealthDisplay()
    {
        sellHealthText.text = "Sell Health: " + sellHealth;
    }

    public void SaleCompleted()
    {
        System.Random rand = new System.Random();
        intuitionRandomDouble = rand.NextDouble();

        isCompromising = false;
        isTrading = false;

        if (currentCustomer.mainSaleWentThrough)
        {
            inventory.Remove(itemDictionary[currentCustomer.mainWantedItem]);
            salesCompleted++;
            totalSalePriceProportionDifference += (currentCustomer.realMaxSpending - suggestedPriceTotal) / currentCustomer.realMaxSpending;
        }
        if (currentCustomer.suppSaleWentThrough)
        {
            inventory.Remove(itemDictionary[currentCustomer.supplementalWantedItem]);
            salesCompleted++;
        }

        for (int i = 0; i < suggestedPrice.Length; i++)
        {
            suggestedPrice[i] = 0;
        }
        UpdateSuggestedPriceDisplay();
        buyPriceTotal = 0;
        UpdateBuyPriceDisplay();

        sellHealth = 10;
        UpdateSellHealthDisplay();
        sellHealthDisplay.SetActive(false);
        supplementalSaleReady = false;

        for (int i = 0; i < talkButtons.Length; i++)
        {
            talkButtons[i].interactable = true;
        }

        for (int i = 0; i < questionButtons.Length; i++)
        {
            questionButtons[i].interactable = true;
        }

        priceDisplay.SetActive(false);
        buyPriceTotal = 0;
        buyDialogueIndex = 0;
        UpdateBuyPriceDisplay();
        itemsToSell.Clear();
        currentCustomerIndex++;
        if(days[currentDay].customers[currentCustomerIndex] != null)
        {
            currentCustomer = FindCurrentCustomer();
            customerNameText.text = currentCustomer.customerName;
            IdleMode();
        } else
        {
            SceneManager();
        }
    }

    public void NextDay()
    {
        Debug.Log("next day");

        dayEnded = false;
        inScene = false;

        if (currentDay < 1)
        {
            StartCoroutine(StatsCoroutine(6));

            if (inventory.Contains(itemDictionary["iron sword"]))
            {
                inventory.Remove(itemDictionary["iron sword"]);
            }

            if(inventory.Contains(itemDictionary["oak stave"]))
            {
                inventory.Remove(itemDictionary["oak stave"]);
            }

            inventory.AddLast(itemDictionary["hp ring"]);
            inventory.AddLast(itemDictionary["mp ring"]);

            currentDay++;

            StartCoroutine(fadeToBlack.FadeInAndOutBlackSquare(2, "Day " + (currentDay + 1)));

            currentCustomerIndex = 0;
            currentCustomer = FindCurrentCustomer();

            sceneCount = 0;
            sceneRoute = "a";
            AudioManager("merchant");
            customerNameText.text = currentCustomer.customerName;
            advancingEnabled = false;
            currentScene = null;
            IdleMode();
        } else
        {
            string song = "merchant";

            spriteDisplay.SetActive(false);
            string ending = "";
            if(sceneRoute == "a")
            {
                ending = "Mercantile Independence";
            } else if (sceneRoute == "aa")
            {
                ending = "Order in the Church";
                song = "battle";
            } else if (sceneRoute == "b")
            {
                ending = "The Lonely Merchant";
                song = "suspense";
            } else if (sceneRoute == "bb")
            {
                ending = "Chaos Incarnate";
                song = "boss";
            }
            displayedText.text = "Thank you for playing! This is the end of the prologue.\n Ending: " + ending;
            AudioManager(song);
            
        }
    }

    // General

    public Customer FindCurrentCustomer()
    {
        Customer customer = days[currentDay].customers[currentCustomerIndex];
        if(customer.name == "customer3")
        {
            customer.friendship = days[0].customers[0].friendship;
            // set day 2 jack initial friendship to jack day 1 friendship
        }
        return customer;
    }

    public void PopulateItemDictionary()
    {
        foreach (Items item in allItems)
        {
            if(item != null)
            {
                itemDictionary.Add(item.keyword.ToLower(), item);
            }
        }
    }

    public void PopulateInfoDictionary()
    {
        foreach (Information info in allInfo)
        {
            if(info != null)
            {
                infoDictionary.Add(info.keyword.ToLower(), info);
            }
        }
    }

    public void ResetCustomers()
    {
        for (int i = 0; i < days[0].customers.Length; i++)
        {
            if(days[0].customers[i] != null)
            {
                days[0].customers[i].ResetCustomer();
            }
        }

        for (int i = 0; i < days[1].customers.Length; i++)
        {
            if (days[1].customers[i] != null)
            {
                days[1].customers[i].ResetCustomer();
            }
        }
    }

    public void AudioManager(string song)
    {
        Debug.Log("change to " + song);
        merchantAudio.gameObject.SetActive(false);
        suspenseAudio.gameObject.SetActive(false);
        battleAudio.gameObject.SetActive(false);
        happyAudio.gameObject.SetActive(false);
        bossAudio.gameObject.SetActive(false);

        switch (song)
        {
            case "merchant":
                merchantAudio.gameObject.SetActive(true);
                break;
            case "suspense":
                suspenseAudio.gameObject.SetActive(true);
                break;
            case "battle":
                battleAudio.gameObject.SetActive(true);
                break;
            case "happy":
                happyAudio.gameObject.SetActive(true);
                break;
            case "boss":
                bossAudio.gameObject.SetActive(true);
                break;
        }
    }

    // ***** SCENE METHODS ***** //

    public void SceneManager()
    {
        if (dayEnded)
        {
            NextDay();
        }
        else if (currentScene != null && currentScene.rightBeforeBattle)
        {
            inScene = false;
            if(currentDay == 0)
            {
                if (!days[0].customers[0].mainSaleWentThrough)
                {
                    sceneRoute = "b";
                }
            } else if (currentDay == 1)
            {
                if (toldPracticeInfo)
                {
                    sceneRoute = "b";
                }
            }

            currentScene = days[currentDay].FindCorrectScene(sceneCount, sceneRoute);
            BattleMode();
        }
        else if (currentScene != null && currentScene.endOfDay)
        {
            dayEnded = true;
            spriteDisplay.SetActive(true);
            idleDisplay.SetActive(false);
            //currentScene = days[currentDay].FindCorrectScene(sceneCount, sceneRoute);
            currentScene.sceneDialogueIndex = 0;
            displayedText.text = currentScene.sceneDialogue[currentScene.sceneDialogueIndex];
            inScene = true;
            //sceneCount++;

            // set portrait and name text for first dialogue
            customerNameText.text = currentScene.characters[currentScene.characterTalking[currentScene.sceneDialogueIndex]].customerName;
            portrait.sprite = currentScene.characters[currentScene.characterTalking[currentScene.sceneDialogueIndex]]
                .portraits[currentScene.characterPortrait[currentScene.sceneDialogueIndex]];
        }
        else
        {
            spriteDisplay.SetActive(true);
            idleDisplay.SetActive(false);
            currentScene = days[currentDay].FindCorrectScene(sceneCount, sceneRoute);
            if (currentScene.endOfDay)
            {
                dayEnded = true;
            }
            currentScene.sceneDialogueIndex = 0;
            displayedText.text = currentScene.sceneDialogue[currentScene.sceneDialogueIndex];
            inScene = true;
            sceneCount++;

            // set portrait and name text for first dialogue
            customerNameText.text = currentScene.characters[currentScene.characterTalking[currentScene.sceneDialogueIndex]].customerName;
            portrait.sprite = currentScene.characters[currentScene.characterTalking[currentScene.sceneDialogueIndex]]
                .portraits[currentScene.characterPortrait[currentScene.sceneDialogueIndex]];
        }
    }

    public void SceneQuestionMode()
    {
        for (int i = 0; i < sceneQuestionOptions.Length; i++)
        {
            sceneQuestionOptions[i].text = currentScene.sceneQuestionOptions[i];
        }

        // check for locked option, then check if player meets the requirements
        if(currentScene.lockedOptionIndex != 99)
        { // if the locked option should exist (will be 0 or 1)
            Image lockedOptionImage = sceneQuestionButtons[currentScene.lockedOptionIndex].GetComponent<Image>();

            int moralityToCheck = 0;
            if(currentScene.statToCheck == "order")
            {
                moralityToCheck = 0;
            } else if (currentScene.statToCheck == "chaos")
            {
                moralityToCheck = 2;
            }

            int valueToCompare = morality[moralityToCheck];
            if(valueToCompare < currentScene.statValue)
            { // player stat is less then requirement, so lock the option
                sceneQuestionButtons[currentScene.lockedOptionIndex].interactable = false;
            } else
            { // player stat is greater than or equal to requirement, so leave the option unlocked
                lockedOptionImage.color = Color.blue;
            }
        }

        spriteDisplay.SetActive(false);
        sceneQuestionDisplay.SetActive(true);
    }

    public void SceneQuestionAnswered(int option)
    {
        // reset status of buttons for next scene
        Image sceneOptionImage0 = sceneQuestionButtons[0].GetComponent<Image>();
        Image sceneOptionImage1 = sceneQuestionButtons[1].GetComponent<Image>();
        sceneOptionImage0.color = Color.black;
        sceneOptionImage1.color = Color.black;

        sceneQuestionButtons[0].interactable = true;
        sceneQuestionButtons[1].interactable = true;

        if(currentScene.lockedOptionIndex != 99)
        {
            if(option == currentScene.lockedOptionIndex)
            { // if the player chose the locked option, change the route
                sceneRoute = currentScene.lockedRouteChange;
            }
        }

        currentScene.sceneQuestionFlag = false;
        sceneQuestionDisplay.SetActive(false);
        spriteDisplay.SetActive(true);
        currentScene.sceneDialogueIndex = currentScene.sceneJumps[option];
        displayedText.text = currentScene.sceneDialogue[currentScene.sceneDialogueIndex];
    }


    // ***** BATTLE METHODS ***** //

    public void BattleMode()
    {
        merchantView.SetActive(false);

        battleView.SetActive(true);
        allyActionDisplay.SetActive(false);
        tryHealAlly = false;
        isBattling = true;

        BattleConstructor(currentDay);
    }

    public void BattleConstructor(int day)
    {
        if (day == 0)
        {
            BattleAlly ally0 = battleAllies[0].GetComponent<BattleAlly>();
            ally0.SetNull();

            BattleAlly ally1 = battleAllies[1].GetComponent<BattleAlly>();
            ally1.SetNull();

            BattleAlly ally2 = battleAllies[2].GetComponent<BattleAlly>();
            int[] ally2Stats = { 52, 0, 8, 8, 3, 5, 7, 1 };
            Items ally2Weapon = itemDictionary["bronze sword"];
            if (days[0].customers[0].mainSaleWentThrough)
            {
                ally2Weapon = itemDictionary["iron sword"];
            }
            // { maxHP, maxMP, strength, defense, magicAttack, magicDefense, speed, level }
            ally2.SetStats("Jack", ally2Stats, ally2Weapon, "backline");
            // set battle dialogue
            ally2.SetBattleDialogue("Take this!", "");
            ally2.ActivateProgressBar();

            BattleEnemy enemy0 = battleEnemies[0].GetComponent<BattleEnemy>();
            int[] enemy0Stats = { 42, 0, 5, 4, 0, 3, 1, 1 };
            enemy0.SetStats("Wolf 1", enemy0Stats, itemDictionary["claw"], "frontline");

            BattleEnemy enemy1 = battleEnemies[1].GetComponent<BattleEnemy>();
            int[] enemy1Stats = { 48, 0, 6, 5, 0, 3, 3, 2 };
            enemy1.SetStats("Wolf 2", enemy1Stats, itemDictionary["claw"], "backline");

            BattleEnemy enemy2 = battleEnemies[2].GetComponent<BattleEnemy>();
            enemy2.SetNull();

            allyDialogue.text = "Jack: Please, tell me what to do! I need help!";
            battleHelp.text = "Click on an ally once their progress bar is full!";
            battleHelpDisplay.SetActive(true);
            enemyDialogueDisplay.SetActive(false);

            if (days[0].customers[0].suppSaleWentThrough)
            {
                totalPotions = 2;
            }

            AudioManager("battle");
        }

        if (day == 1)
        {
            BattleAlly ally0 = battleAllies[0].GetComponent<BattleAlly>();
            int[] ally0Stats = { 80, 0, 10, 10, 1, 2, 5, 4 };
            Items ally0Weapon = itemDictionary["bronze axe"];
            if (days[1].customers[2].mainSaleWentThrough)
            {
                ally0Weapon = itemDictionary["iron axe"];
            }
            ally0.SetStats("Leon", ally0Stats, ally0Weapon, "frontline");
            ally0.SetBattleDialogue("Eat this!", "Haaaaaaa!");
            ally0.ActivateProgressBar();
            string[] ally0Skills = { "Heavy Blow", "", "" };
            ally0.SetSkills(ally0Skills);
            ally0.portrait = days[1].customers[2].portraits[4];
            ally0.characterText.SetActive(true);

            BattleAlly ally1 = battleAllies[1].GetComponent<BattleAlly>();
            int ally1HP = 60;
            if (days[1].customers[0].mainSaleWentThrough)
            {
                ally1HP = 65;
            }
            int[] ally1Stats = { ally1HP, 0, 9, 9, 3, 6, 7, 3 };
            ally1.SetStats("Jack", ally1Stats, itemDictionary["iron sword"], "frontline");
            ally1.SetBattleDialogue("Here I go!", "Time to move faster!");
            ally1.ActivateProgressBar();
            string[] ally1Skills = { "Armor Breaker", "", "" };
            ally1.SetSkills(ally1Skills);
            ally1.portrait = days[1].customers[0].portraits[4];
            ally1.characterText.SetActive(true);

            BattleAlly ally2 = battleAllies[2].GetComponent<BattleAlly>();
            int ally2MP = 20;
            if (days[1].customers[3].mainSaleWentThrough)
            {
                ally2MP = 25;
            }
            int[] ally2Stats = { 50, ally2MP, 7, 6, 6, 9, 8, 4 };
            Items ally2Weapon = itemDictionary["steel bow"];
            if (days[1].customers[3].suppSaleWentThrough)
            {
                ally2Weapon = itemDictionary["iron bow"];
            }
            ally2.SetStats("Vera", ally2Stats, ally2Weapon, "backline");
            ally2.SetBattleDialogue("My aim is true!", "Please... let this work!");
            ally2.ActivateProgressBar();
            string[] ally2Skills = { "Heal", "Healara", "Ward" };
            ally2.SetSkills(ally2Skills);
            ally2.portrait = days[1].customers[3].portraits[4];

            

            BattleEnemy enemy0 = battleEnemies[0].GetComponent<BattleEnemy>();
            int[] enemy0Stats = { 320, 100, 10, 15, 20, 20, -4, 25 };
            enemy0.SetStats("Elise", enemy0Stats, itemDictionary["oak stave"], "frontline");
            string[] eliseBattleDialogue = {"In just a few moments, my spell will inflict instant death on all of you!",
            "Only a little longer... Hee hee!", "Muahahahaha!"};
            enemy0.SetSkillDialogue(eliseBattleDialogue);
            enemy0.knockedOut = false;
            enemy0.turnState = "waiting";

            BattleEnemy enemy1 = battleEnemies[1].GetComponent<BattleEnemy>();
            enemy1.SetNull();

            BattleEnemy enemy2 = battleEnemies[2].GetComponent<BattleEnemy>();
            enemy2.SetNull();

            allyDialogue.text = "Jack: What... is wrong with her?! Please, help us!";
            battleHelp.text = "";
            battleHelpDisplay.SetActive(false);
            enemyDialogue.text = "It begins...";
            enemyDialogueDisplay.SetActive(true);

            totalPotions = 5;
        }
    }

    public BattleCharacter GetBattleScript(GameObject battler)
    {
        BattleCharacter battleScript = battler.GetComponent<BattleCharacter>();
        return battleScript;
    }

    public void DisplayAllyActions(BattleAlly ally)
    {
        if (!tryHealAlly)
        {
            currentAllySelected = ally;
            battlePortrait.sprite = ally.portrait;

            foreach (Transform child in allySkillList.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            allySkillDisplay.SetActive(false);
            allyActionDisplay.SetActive(true);
            battleHelp.text = "What should " + ally.characterName + " do?";
            battleHelpDisplay.SetActive(true);
        }
    }

    public void Charge()
    {
        tryHealAlly = false;
        currentAllySelected.charged = true;
        currentAllySelected.currentCooldown = 0;
        currentAllySelected.turnState = "waiting";
        currentAllySelected.progressBar.color = Color.cyan;

        battleHelpDisplay.SetActive(false);
        allySkillDisplay.SetActive(false);
        allyActionDisplay.SetActive(false);
    }

    public void HealAlly(BattleAlly ally)
    {
        if (tryHealAlly)
        {
            bool success = false;
            int healPower = 0;
            int MPCost = 0;
            switch (currentActionSelected)
            {
                case "potion":
                    if (totalPotions > 0)
                    {
                        healPower = 20;
                        success = true;
                    }
                    break;
                case "Heal":
                    if (currentAllySelected.currentMP >= 5)
                    {
                        healPower = 40;
                        MPCost = 5;
                        success = true;
                    }
                    break;
                case "Healara":
                    if (currentAllySelected.currentMP >= 10)
                    {
                        healPower = 60;
                        MPCost = 10;
                        success = true;
                    }
                    break;
                default:
                    healPower = 10;
                    break;
            }

            if (currentActionSelected == "potion")
            {
                if (success)
                {
                    allySkillDisplay.SetActive(false);
                    allyActionDisplay.SetActive(false);
                    //BattleAlly healer = battleAllies[2].GetComponent<BattleAlly>();
                    //healer.currentMP -= MPCost;
                    //if (healer.currentMP < 0)
                    //{
                    //    healer.currentMP = 0;
                    //}
                    //UpdateBattlerDisplay(healer);
                    totalPotions--;
                    ally.currentHP += healPower;
                    if(ally.currentHP > ally.maxHP)
                    {
                        ally.currentHP = ally.maxHP;
                    }
                    UpdateBattlerDisplay(ally);

                    currentAllySelected.currentCooldown = 0;
                    currentAllySelected.turnState = "waiting";

                    tryHealAlly = false;
                    battleHelpDisplay.SetActive(false);
                    resultText.text = ally.characterName + " restored " + healPower + "HP!";
                    currentAllySelected.progressBar.color = Color.cyan;
                }
                else
                {
                    battleHelp.text = "You have no potions!";
                }
            }
            else
            { // heal or healara
                if (success)
                {
                    allySkillDisplay.SetActive(false);
                    allyActionDisplay.SetActive(false);

                    ally.currentHP += healPower;
                    if (ally.currentHP > ally.maxHP)
                    {
                        ally.currentHP = ally.maxHP;
                    }
                    UpdateBattlerDisplay(ally);

                    currentAllySelected.currentMP -= MPCost;
                    UpdateBattlerDisplay(currentAllySelected);

                    currentAllySelected.currentCooldown = 0;
                    currentAllySelected.turnState = "waiting";

                    tryHealAlly = false;
                    battleHelpDisplay.SetActive(false);
                    resultText.text = ally.characterName + " restored " + healPower + "HP!";
                    currentAllySelected.progressBar.color = Color.cyan;
                }
                else
                {
                    battleHelp.text = "Not enough MP!";
                }
            }
        }
    }

    public void SelectSkill()
    {
        PopulateSkillDisplay(currentAllySelected);
        allySkillDisplay.SetActive(true);
        DisableEnemySelection();
        battleHelp.text = "What skill should " + currentAllySelected.characterName + " use?";
    }

    public void PopulateSkillDisplay(BattleAlly ally)
    {
        for (int i = 0; i < ally.skills.Length; i++)
        {
            if (ally.skills[i] != "")
            {
                GameObject currentSkillDisplay = Instantiate<GameObject>(skillButton, allySkillList.transform, false);
                currentSkillDisplay.transform.position += Vector3.down * (16f * (i + 1));

                SkillDisplay skillScript = currentSkillDisplay.GetComponent<SkillDisplay>();
                skillScript.skillText.text = ally.skills[i];
                skillScript.skillName = ally.skills[i];
            }
        }
    }

    public void SelectTarget(string move)
    {
        // use current ally selected
        currentActionSelected = move;

        if(move == "Ward")
        {
            Ward();
            tryHealAlly = false;
        }
        else if(move == "potion" || move == "Heal" || move == "Healara")
        {
            battleHelp.text = "Which ally will you heal?";
            DisableEnemySelection();
            EnableAllySelection();
            // have to add another purpose to the button (add boolean to detect when allies can be healed)
            tryHealAlly = true;
        } // else if skill selected (don't hide skill display?)
        else if (move == "Armor Breaker" || move == "Heavy Blow")
        {
            battleHelp.text = "Which enemy will you target?";
            DisableAllySelection();
            EnableEnemySelection();
            tryHealAlly = false;
        }
        else
        {
            battleHelp.text = "Which enemy will you target?";
            DisableAllySelection();
            EnableEnemySelection();
            allySkillDisplay.SetActive(false);
            tryHealAlly = false;
        }
    }

    public void DisableAllySelection()
    {
        for (int i = 0; i < battleAllies.Length; i++)
        {
            battleAllies[i].interactable = false;
        }
    }

    public void DisableEnemySelection()
    {
        for (int i = 0; i < battleEnemies.Length; i++)
        {
            battleEnemies[i].interactable = false;
        }
    }

    public void EnableAllySelection()
    {
        for (int i = 0; i < battleAllies.Length; i++)
        {
            BattleAlly currentAlly = battleAllies[i].GetComponent<BattleAlly>();
            if(currentAlly.characterSet && !currentAlly.knockedOut)
            {
                battleAllies[i].interactable = true;
            }
        }
    }

    public void EnableEnemySelection()
    {
        for (int i = 0; i < battleEnemies.Length; i++)
        {
            BattleEnemy currentEnemy = battleEnemies[i].GetComponent<BattleEnemy>();
            if (currentEnemy.characterSet && !currentEnemy.knockedOut)
            {
                battleEnemies[i].interactable = true;
            }
        }
    }

    public void BackToBattle()
    {
        allySkillDisplay.SetActive(false);
        allyActionDisplay.SetActive(false);
        battleHelpDisplay.SetActive(false);
        tryHealAlly = false;
        EnableAllySelection();
        DisableEnemySelection();
        battleHelp.text = "";
    }

    public void HideSkillDisplay()
    {
        allySkillDisplay.SetActive(false);
    }

    public void OnEnemyAttacked(BattleEnemy enemy)
    {
        int attackPower;
        string attackType = "";
        if (currentActionSelected == "attack")
        {
            attackPower = currentAllySelected.strength + currentAllySelected.weapon.attackPower;
            attackType = "physical";
            if (!currentAllySelected.saidAttackDialogue)
            {
                allyDialogue.text = currentAllySelected.characterName + ": " + currentAllySelected.attackDialogue;
                currentAllySelected.saidAttackDialogue = true;
            }
        }
        else if (currentActionSelected == "Armor Breaker")
        {
            // skills

            // lower defense of enemy if defense not already lowered
            if (!enemy.defenseLowered)
            {
                enemy.defenseLowered = true;
                enemy.defense -= 3;
            }

            attackType = "physical";

            attackPower = currentAllySelected.strength + currentAllySelected.weapon.attackPower + 5;
            currentAllySelected.currentHP -= 5;
            if (currentAllySelected.currentHP < 0)
            {
                currentAllySelected.currentHP = 0;
                currentAllySelected.knockedOut = true;
            }
            UpdateBattlerDisplay(currentAllySelected);
        }
        else if (currentActionSelected == "Heavy Blow")
        {
            attackType = "physical";
            attackPower = currentAllySelected.strength + currentAllySelected.weapon.attackPower + 7;

            currentAllySelected.currentHP -= 6;
            if (currentAllySelected.currentHP < 0)
            {
                currentAllySelected.currentHP = 0;
                currentAllySelected.knockedOut = true;
            }
            UpdateBattlerDisplay(currentAllySelected);
        }
        else
        {

            attackPower = 5; // placeholder
        }

        DealDamage(currentAllySelected, enemy, attackPower, attackType);

        Debug.Log("enemy hit");
        StartCoroutine(IsHit(enemy));
        allySkillDisplay.SetActive(false);
        allyActionDisplay.SetActive(false);
        battleHelpDisplay.SetActive(false);
        DisableEnemySelection();
        EnableAllySelection();

        currentAllySelected.currentCooldown = 0;
        currentAllySelected.ready = false;
        currentAllySelected.turnState = "waiting";
        currentAllySelected.progressBar.color = Color.cyan;
        battleHelp.text = "";
    }

    public void EnemyAction(BattleEnemy enemy)
    {
        if (!enemy.knockedOut)
        {
            if (enemy.characterName == "Elise")
            {
                BattleEnemy enemyElise = enemy.GetComponent<BattleEnemy>();
                if (!enemyElise.saidFirstDialogue)
                {
                    enemyDialogue.text = enemyElise.firstSkillDialogue;
                    enemyElise.saidFirstDialogue = true;
                    enemy.currentCooldown = 0;
                    enemy.turnState = "waiting";
                } else if (!enemyElise.saidSecondDialogue)
                {
                    enemyDialogue.text = enemyElise.secondSkillDialogue;
                    enemyElise.saidSecondDialogue = true;
                    enemy.currentCooldown = 0;
                    enemy.turnState = "waiting";
                } else if (enemyElise.saidSecondDialogue)
                {
                    enemyDialogue.text = enemyElise.thirdSkillDialogue;
                    AnnihilationAttack();
                }
                
            }
            else
            { // normal enemy behavior
                enemy.currentCooldown = 0;
                enemy.turnState = "waiting";
                BattleAlly allyToAttack = battleAllies[2].GetComponent<BattleAlly>();
                DealDamage(enemy, allyToAttack, enemy.strength + enemy.weapon.attackPower, "physical");
                StartCoroutine(IsHit(allyToAttack));
            }
        }
    }

    public void DealDamage(BattleCharacter attacker, BattleCharacter defender, int power, string type)
    {
        int damageDone;

        if(type == "physical")
        {
            damageDone = power - defender.defense;
            if(damageDone < 0) { damageDone = 0; }
            if (attacker.charged)
            {
                damageDone *= 2;
                attacker.charged = false;
            }
        }
        else
        {
            damageDone = 3; // placeholder
        }

        defender.currentHP -= damageDone;
        if(defender.currentHP <= 0)
        {
            defender.currentHP = 0;
            defender.knockedOut = true;
            //KnockedOut(defender);
        }

        resultText.text = attacker.characterName + " dealt " + damageDone + " damage to "
            + defender.characterName + "!";

        UpdateBattlerDisplay(defender);

        battleAudio.PlayOneShot(hitClip);

        CheckForBattleEnd();

        //if (typeof(BattleEnemy).IsInstanceOfType(defender))
        //{

        //}
    }

    public void SetGameControllerReferenceOnSkillDisplay()
    {
        SkillDisplay skillDisplay = skillButton.GetComponent<SkillDisplay>();
        skillDisplay.SetGameControllerReference(this);
    }

    public void Ward()
    {
        allySkillDisplay.SetActive(false);
        allyActionDisplay.SetActive(false);
        battleHelpDisplay.SetActive(false);

        for (int i = 0; i < battleAllies.Length; i++)
        {
            BattleAlly currentAlly = battleAllies[i].GetComponent<BattleAlly>();
            currentAlly.warded = true;
        }

        currentAllySelected.currentCooldown = 0;
        currentAllySelected.turnState = "waiting";
        currentAllySelected.progressBar.color = Color.cyan;
        allyDialogue.text = "Vera: Let us hope for protection...";
    }

    public void AnnihilationAttack()
    {
        Debug.Log("annihilation attack");

        int knockedOutAllies = 0;
        for (int i = 0; i < battleAllies.Length; i++)
        {
            BattleAlly currentAlly = battleAllies[i].GetComponent<BattleAlly>();
            if (currentAlly.warded)
            {// if warded, leave at 1 hp
                currentAlly.currentHP = 1;
                UpdateBattlerDisplay(currentAlly);
            } else
            {// if not warded, ally is knocked out
                currentAlly.currentHP = 0;
                currentAlly.knockedOut = true;
                UpdateBattlerDisplay(currentAlly);
                knockedOutAllies++;
                // increment accumulator of knocked out allies
            }
        }

        if(knockedOutAllies == 3)
        {
            EndBattle("game over");
        } else
        {
            EndBattle("withstood the attack");
        }
    }

    public void UpdateBattlerDisplay(BattleCharacter battler)
    {
        battler.HPText.text = "HP: " + battler.currentHP + " / " + battler.maxHP;
        battler.MPText.text = "MP: " + battler.currentMP + " / " + battler.maxMP;
    }

    //public void KnockedOut(BattleCharacter battler)
    //{
    //    // Should disable battler and set disabled color to gray?

    //    Image battlerImage = battler.GetComponent<Image>();
    //    Button battlerButton = battler.GetComponent<Button>();

    //    var colors = battlerButton.colors;
    //    colors.disabledColor = Color.gray;

    //    battlerButton.colors = colors;

    //    Debug.Log("knocked out");
    //}

    public void CheckForBattleEnd()
    {
        bool battleWon = true;
        for (int i = 0; i < battleEnemies.Length; i++)
        {
            BattleEnemy currentEnemy = battleEnemies[i].GetComponent<BattleEnemy>();
            if(currentEnemy.characterSet)
            {
                if (!currentEnemy.knockedOut)
                {
                    battleWon = false;
                }
            }
        }

        bool battleLost = true;
        for (int i = 0; i < battleAllies.Length; i++)
        {
            BattleAlly currentAlly = battleAllies[i].GetComponent<BattleAlly>();
            if (currentAlly.characterSet)
            {
                if (!currentAlly.knockedOut)
                {
                    battleLost = false;
                }
            }
        }

        if (battleWon)
        {
            Debug.Log("victory");
            EndBattle("victory");
        } else if (battleLost)
        {
            EndBattle("game over");
        }
    }

    IEnumerator IsHit(BattleCharacter battler)
    {
        Image battlerImage = battler.GetComponent<Image>();
        battlerImage.color = Color.red;

        Debug.Log("color enumerator");

        yield return new WaitForSeconds(0.1f);

        if(battler.currentHP == 0)
        {
            battlerImage.color = Color.gray;
        } else
        {
            battlerImage.color = Color.black;
        }
    }

    public void ResetBattlers()
    {
        for (int i = 0; i < battleAllies.Length; i++)
        {
            BattleAlly currentAlly = battleAllies[i].GetComponent<BattleAlly>();
            if (currentAlly.characterSet)
            {
                currentAlly.currentCooldown = 0;
                currentAlly.progressBar.color = Color.cyan;
                currentAlly.knockedOut = false;
                currentAlly.turnState = "waiting";
            }
        }

        for (int i = 0; i < battleEnemies.Length; i++)
        {
            BattleEnemy currentEnemy = battleEnemies[i].GetComponent<BattleEnemy>();
            if (currentEnemy.characterSet)
            {
                currentEnemy.currentCooldown = 0;
                if(currentEnemy.characterName == "Elise")
                {
                    currentEnemy.saidFirstDialogue = false;
                    currentEnemy.saidSecondDialogue = false;
                    enemyDialogue.text = "";
                    currentEnemy.knockedOut = false;
                    currentEnemy.turnState = "waiting";
                }
            }
        }
    }

    public void EndBattle(string reason)
    {
        if (reason == "game over") 
        { // if the player lost the battle, restart
            Debug.Log("game over");
            tryHealAlly = false;
            allySkillDisplay.SetActive(false);
            allyActionDisplay.SetActive(false);
            battleHelpDisplay.SetActive(false);
            ResetBattlers();
            StartCoroutine(StatsCoroutine(7));
            BattleMode();
        } else
        { // if the battle ended for another reason, continue the game
            inScene = true;
            tryHealAlly = false;
            Debug.Log("go to next scene");
            ResetBattlers();
            resultText.text = "";
            battleView.SetActive(false);
            merchantView.SetActive(true);
            SceneManager();
        }
    }
}
