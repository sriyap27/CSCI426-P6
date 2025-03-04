using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

// Create game states
public enum GameState { START, PLAYERTURN, ENEMYTURN, WIN, LOSE }

public class ShopBehavior : MonoBehaviour
{
    // references to player and enemy
    public GameObject player;
    public GameObject enemy;
    SpriteRenderer enemySprite;

    public GameState state;

    // basic pool of power ups
    List<int> powerUps = new List<int> { 0, 0, 1, 1, 2, 2, 3, 3, 4, 5, 6, 7};
    int[] prices = { 3, 3, 4, 4, 3, 3, 3, 4 };
    /* 0 = heal
     * 1 = sacrifice hp to deal damage
     * 2 = deal damage (vanilla)
     * 3 = damage over time
     * 4 = block CS
     * 5 = block architecture
     * 6 = block games
     * 7 = cleanse
     */ //we can add more later if we feel we need it

    // current choices' indexes displayed by the shop
    int currChoice1;
    int currChoice2;
    int currChoice3;

    int money;
    int playerHealth;
    int enemyHealth;
    bool freeRefresh;
    int blockType;
    int enemyType;
    int dotCounter;
    int debuffType = 0;
    int numRerolls;

    public int moneyDebuff = 1;
    public int rerollDebuff = 3;
    public int atkDebuff = 10;
    

    // UI
    [SerializeField] TextMeshProUGUI choice1;
    [SerializeField] TextMeshProUGUI choice2;
    [SerializeField] TextMeshProUGUI choice3;
    [SerializeField] TextMeshProUGUI choice4;
    [SerializeField] TextMeshProUGUI playerHealthText;
    [SerializeField] TextMeshProUGUI enemyHealthText;
    [SerializeField] TextMeshProUGUI dialougeText;
    [SerializeField] TextMeshProUGUI moneyText;
    [SerializeField] TextMeshProUGUI enemyTypeText;
    [SerializeField] TextMeshProUGUI debuffText;
    [SerializeField] Button choice1Btn;
    [SerializeField] Button choice2Btn;
    [SerializeField] Button choice3Btn;
    [SerializeField] Button attackBtn;
    [SerializeField] Button refreshBtn;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // start game
        state = GameState.START;
        StartCoroutine(SetupGame());
    }

    // Update is called once per frame
    void Update()
    {
        // track player health
        playerHealthText.text = playerHealth.ToString("F0");
        enemyHealthText.text = enemyHealth.ToString("F0");

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        switch(debuffType)
        {
            case 0:
                debuffText.text = "No debuff";
                break;
            case 1:
                debuffText.text = "Income debuff";
                break;
            case 2:
                debuffText.text = "Attack debuff";
                break;
            case 3:
                debuffText.text = "Reroll debuff";
                break;
        }

        switch(enemyType)
        {
            case 1:
                enemyTypeText.text = "Andy Type: CS";
                break;
            case 2:
                enemyTypeText.text = "Andy Type: Architecture";
                break;
            case 3:
                enemyTypeText.text = "Andy Type: Games";
                break;
        }
    }

    // set up the game 
    IEnumerator SetupGame()
    {
        refreshBtn.onClick.AddListener(RefreshShop);
        choice1Btn.onClick.AddListener(Choice1);
        choice2Btn.onClick.AddListener(Choice2);
        choice3Btn.onClick.AddListener(Choice3);
        attackBtn.onClick.AddListener(Attack);
        enemySprite = enemy.GetComponent<SpriteRenderer>();

        money = 10;
        moneyText.text = money.ToString();
        //idk random vals we can change later
        enemyHealth = 400;
        playerHealth = 100;
        enemyType = 1;
        blockType = 0; // indicates no block being used
        //making sure money is not subtracted for auto refresh
        freeRefresh = true;

        dialougeText.text = "A wild Andy Nealen appears...";

        yield return new WaitForSeconds(2f);

        state = GameState.PLAYERTURN;
        PlayerTurn();

    }

    // start player turn
    void PlayerTurn()
    {
        freeRefresh = true;
        RefreshShop();
        dialougeText.text = "Choose an action: ";
    }


    // refresh the shop
    void RefreshShop()
    {
        // repopulating if we somehow run out
        if (freeRefresh)
        {
            powerUps = new List<int> { 0, 0, 1, 1, 2, 2, 3, 3, 4, 5, 6, 7 };
            enemyType = Random.Range(1, 4);
            numRerolls = rerollDebuff;
            if (enemyType == 1)
            {
                dialougeText.text = "Andy's attack will be CS type";
                enemySprite.color = Color.green;
            }
            else if (enemyType == 2)
            {
                dialougeText.text = "Andy's attack will be Architecture type";
                enemySprite.color = Color.red;
            }
            else
            {
                dialougeText.text = "Andy's attack will be Games type";
                enemySprite.color = Color.blue;
            }

        }
        // getting 3 random values from the shop
        int currChoice1Idx;
        int currChoice2Idx;
        int currChoice3Idx;
        if ((money > 0 || freeRefresh) && (debuffType != 3 ||numRerolls > 0) && powerUps.Count >= 3)
        {
            if (!freeRefresh)
            {
                money--;
                moneyText.text = money.ToString();
            }
            currChoice1Idx = Random.Range(0, powerUps.Count);
            currChoice2Idx = Random.Range(0, powerUps.Count);
            while (currChoice2Idx == currChoice1Idx)
            {
                currChoice2Idx = Random.Range(0, powerUps.Count);
            }
            currChoice3Idx = Random.Range(0, powerUps.Count);
            while (currChoice3Idx == currChoice1Idx || currChoice3Idx == currChoice2Idx)
            {
                currChoice3Idx = Random.Range(0, powerUps.Count);
            }
            currChoice1 = powerUps[currChoice1Idx];
            currChoice2 = powerUps[currChoice2Idx];
            currChoice3 = powerUps[currChoice3Idx];
            powerUps.Remove(currChoice1);
            powerUps.Remove(currChoice2);
            powerUps.Remove(currChoice3);
            freeRefresh = false;
            DisplayShop(currChoice1, choice1);
            DisplayShop(currChoice2, choice2);
            DisplayShop(currChoice3, choice3);
            numRerolls--;
        }

    }

    // displaying shop options
    void DisplayShop(int choice, TextMeshProUGUI choiceText)
    {
        choice1Btn.gameObject.SetActive(true);
        choice2Btn.gameObject.SetActive(true);
        choice3Btn.gameObject.SetActive(true);
        switch (choice)
        {
            case 0:
                choiceText.text = "Heal (3 gold)";
                break;
            case 1:
                choiceText.text = "Sacrifice HP to deal damage (3 gold)";
                break;
            case 2:
                choiceText.text = "Deal damage (4 gold)";
                break;
            case 3:
                choiceText.text = "Deal damage over 3 rounds (4 gold)";
                break;
            case 4:
                choiceText.text = "Block CS attack (3 gold)";
                break;
            case 5:
                choiceText.text = "Block Architecture attack (3 gold)";
                break;
            case 6:
                choiceText.text = "Block Games attack (3 gold)";
                break;
            case 7:
                choiceText.text = "Cleanse all debuffs";
                break;

        }
    }

    void Choice1()
    {
        if (money >= prices[currChoice1] && state == GameState.PLAYERTURN)
        {
            money -= prices[currChoice1];
            moneyText.text = money.ToString();
            choice1Btn.gameObject.SetActive(false);
            // do behavior correlated to the actual choice here
            PowerUpBehavior(currChoice1);
            Debug.Log(money);
        }
    }

    void Choice2()
    {
        if (money >= prices[currChoice2] && state == GameState.PLAYERTURN)
        {
            money -= prices[currChoice2];
            moneyText.text = money.ToString();
            choice2Btn.gameObject.SetActive(false);
            // do behavior correlated to the actual choice here
            PowerUpBehavior(currChoice2);
            Debug.Log(money);
        }
    }

    void Choice3()
    {
        if (money >= prices[currChoice3] && state == GameState.PLAYERTURN)
        {
            money -= prices[currChoice3];
            moneyText.text = money.ToString();
            choice3Btn.gameObject.SetActive(false);
            // do behavior correlated to the actual choice here
            PowerUpBehavior(currChoice3);
            Debug.Log(money);
        }
    }

    void Attack()
    {
        // only execute if it's the player's turn
        if (state != GameState.PLAYERTURN)
            return;

        StartCoroutine(PlayerAttack());
    }

    // attack routine
    IEnumerator PlayerAttack()
    {
        if (dotCounter > 0)
        {
            dialougeText.text = "You dealt damage to Andy!";
            enemyHealth -= 30;
            if (debuffType == 2)
            {
                enemyHealth += atkDebuff;
            }
            dotCounter--;

            yield return new WaitForSeconds(1f);

            enemyHealthText.text = enemyHealth.ToString();


            
        }
        // check if enemy is alive 
        if (enemyHealth <= 0)
        {
            state = GameState.WIN;
            EndGame();
        }

        else
        {
            state = GameState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }

        yield return new WaitForSeconds(2f);

    }

    // enemy attacks player
    IEnumerator EnemyTurn()
    {
        dialougeText.text = "It's Andy's turn to attack now!";

        yield return new WaitForSeconds(1f);

        if(blockType == enemyType)
        {
            dialougeText.text = "You blocked Andy's attack!";
            yield return new WaitForSeconds(1f);
        }
        else if (blockType != 0)
        {
            dialougeText.text = "Andy applied his debuff";
            yield return new WaitForSeconds(1f);

            //enemyAtk *= atkMultiplier;
            //playerHealth -= enemyAtk;
            debuffType = enemyType;
            playerHealthText.text = playerHealth.ToString("F0");
        }
        else
        {
            dialougeText.text = "Andy just slapped you bruh";
            yield return new WaitForSeconds(1f);
            debuffType = enemyType;
            playerHealth -= 30;
            playerHealthText.text = playerHealth.ToString("F0");
        }

        //blockAtk = false;
        blockType = 0;
        yield return new WaitForSeconds(1f);

        // check player health

        if (playerHealth <= 0)
        {
            state = GameState.LOSE;
            EndGame();
        }

        else
        {
            money += 10;
            if (debuffType == 1)
            {
                money -= moneyDebuff;
            }
            moneyText.text = money.ToString();
            state = GameState.PLAYERTURN;
            PlayerTurn();
        }

    }

    void EndGame()
    {
        if (state == GameState.WIN)
        {
            dialougeText.text = "Andy Nealen has finally met his demise!";
        }

        else
        {
            dialougeText.text = "Looks like you'll be getting a 0 this week....";
        }
    }

    void PowerUpBehavior(int powerUP)
    {
        int coin;
        switch (powerUP)
        {
            case 0:
                if (playerHealth <= 75) playerHealth += 25;
                else playerHealth = 100;
                dialougeText.text = "Your health has been replenished.";
                Debug.Log("PlayerHealth" + playerHealth);
                break;
            case 1:
                playerHealth -= 10;
                enemyHealth -= 75;
                if (debuffType == 2)
                    enemyHealth += atkDebuff;
                dialougeText.text = "You used your own HP to greatly damage Andy.";
                break;
            case 2:
                enemyHealth -= 50;
                if (debuffType == 2)
                enemyHealth += atkDebuff;
                dialougeText.text = "You damaged Andy!";
                break;
            case 3:
                dotCounter = 3;
                dialougeText.text = "Deal damage at the beginning of the next 3 rounds!";
                break;
            case 4:
                blockType = 1;
                dialougeText.text = "You will fully block CS attacks";
                break;
            case 5:
                blockType = 2;
                dialougeText.text = "You will fully block Architecture attacks";
                break;
            case 6:
                blockType = 3;
                dialougeText.text = "You will fully block Games attacks";
                break;
            case 7:
                debuffType = 0;
                dialougeText.text = "Cleansed debuff";
                break;
        }
    }

}
