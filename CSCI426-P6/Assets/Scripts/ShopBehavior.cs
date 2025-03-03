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

    public GameState state;

    // basic pool of power ups
    List<int> powerUps = new List<int> { 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 5, 5 };
    int[] prices = { 4, 2, 4, 2, 8 };
    /* 1 = atkIncrease
     * 2 = atkIncrease COINFLIP
     * 3 = heal
     * 4 = heal COINFLIP
     * 5 = block enemy atk one round */ //we can add more later if we feel we need it

    // current choices' indexes displayed by the shop
    int currChoice1;
    int currChoice2;
    int currChoice3;

    int money;
    float playerAtk;
    float playerHealth;
    float enemyAtk;
    float enemyHealth;
    bool freeRefresh;
    float atkMultiplier = 1.25f;
    bool blockAtk;

    // UI
    [SerializeField] TextMeshProUGUI choice1;
    [SerializeField] TextMeshProUGUI choice2;
    [SerializeField] TextMeshProUGUI choice3;
    [SerializeField] TextMeshProUGUI choice4;
    [SerializeField] TextMeshProUGUI playerHealthText;
    [SerializeField] TextMeshProUGUI enemyHealthText;
    [SerializeField] TextMeshProUGUI dialougeText;
    [SerializeField] TextMeshProUGUI moneyText;
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
    }

    // set up the game 
    IEnumerator SetupGame()
    {
        refreshBtn.onClick.AddListener(RefreshShop);
        choice1Btn.onClick.AddListener(Choice1);
        choice2Btn.onClick.AddListener(Choice2);
        choice3Btn.onClick.AddListener(Choice3);
        attackBtn.onClick.AddListener(Attack);

        money = 10;
        moneyText.text = money.ToString();
        blockAtk = false;
        //idk random vals we can change later
        playerAtk = 50f;
        enemyAtk = 25f;
        enemyHealth = 400f;
        playerHealth = 100f;
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
        if (powerUps.Count < 3)
        {
            powerUps = new List<int> { 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 5, 5 };

        }
        // getting 3 random values from the shop
        if (money > 0 || freeRefresh)
        {
            if (!freeRefresh)
            {
                money--;
                moneyText.text = money.ToString();
            }
            currChoice1 = Random.Range(0, powerUps.Count);
            currChoice2 = Random.Range(0, powerUps.Count);
            while (currChoice2 == currChoice1)
            {
                currChoice2 = Random.Range(0, powerUps.Count);
            }
            currChoice3 = Random.Range(0, powerUps.Count);
            while (currChoice3 == currChoice1 || currChoice3 == currChoice2)
            {
                currChoice3 = Random.Range(0, powerUps.Count);
            }
            freeRefresh = false;
            DisplayShop(currChoice1, choice1);
            DisplayShop(currChoice2, choice2);
            DisplayShop(currChoice3, choice3);
        }

    }

    // displaying shop options
    void DisplayShop(int choice, TextMeshProUGUI choiceText)
    {
        choice1Btn.gameObject.SetActive(true);
        choice2Btn.gameObject.SetActive(true);
        choice3Btn.gameObject.SetActive(true);
        switch (powerUps[choice])
        {
            case 1:
                choiceText.text = "Increase your attack (4 gold)";
                break;
            case 2:
                choiceText.text = "You could increase your attack (2 gold)";
                break;
            case 3:
                choiceText.text = "Heal (4 gold)";
                break;
            case 4:
                choiceText.text = "You could heal (2 gold)";
                break;
            case 5:
                choiceText.text = "Block enemy attack this round (8 gold)";
                break;

        }
    }

    void Choice1()
    {
        if (money >= prices[powerUps[currChoice1] - 1] && state == GameState.PLAYERTURN)
        {
            money -= prices[powerUps[currChoice1] - 1];
            moneyText.text = money.ToString();
            choice1Btn.gameObject.SetActive(false);
            // do behavior correlated to the actual choice here
            PowerUpBehavior(powerUps[currChoice1]);
            powerUps.Remove(currChoice1);
            Debug.Log(money);
        }
    }

    void Choice2()
    {
        if (money >= prices[powerUps[currChoice2] - 1] && state == GameState.PLAYERTURN)
        {
            money -= prices[powerUps[currChoice2] - 1];
            moneyText.text = money.ToString();
            choice2Btn.gameObject.SetActive(false);
            // do behavior correlated to the actual choice here
            PowerUpBehavior(powerUps[currChoice2]);
            powerUps.Remove(currChoice2);
            Debug.Log(money);
        }
    }

    void Choice3()
    {
        if (money >= prices[powerUps[currChoice3] - 1] && state == GameState.PLAYERTURN)
        {
            money -= prices[powerUps[currChoice3] - 1];
            moneyText.text = money.ToString();
            choice3Btn.gameObject.SetActive(false);
            // do behavior correlated to the actual choice here
            PowerUpBehavior(powerUps[currChoice3]);
            powerUps.Remove(currChoice3);
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
        dialougeText.text = "The attack was successful!";

        yield return new WaitForSeconds(1f);

        enemyHealth -= playerAtk;
        enemyHealthText.text = enemyHealth.ToString();


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

        if(blockAtk == true)
        {
            dialougeText.text = "You blocked Andy's attack!";
            yield return new WaitForSeconds(1f);
        }
        else
        {
            dialougeText.text = "Andy just slapped you bro";
            yield return new WaitForSeconds(1f);

            enemyAtk *= atkMultiplier;
            playerHealth -= enemyAtk;
            playerHealthText.text = playerHealth.ToString("F0");
        }

        blockAtk = false;
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
            case 1:
                playerAtk *= atkMultiplier;
                dialougeText.text = "Your attack has been multiplied!";
                Debug.Log("PlayerAtk" + playerAtk);
                break;
            case 2:
                coin = Random.Range(0, 2);
                if (coin == 1)
                {
                    playerAtk *= atkMultiplier;
                    dialougeText.text = "Your attack has been multiplied!";
                    Debug.Log("PlayerAtk" + playerAtk);
                }
                else
                {
                    enemyAtk *= atkMultiplier;
                    dialougeText.text = "Oh no! The enemy's attack has been multiplied!";
                    Debug.Log("EnemyAtk" + enemyAtk);
                }
                break;
            case 3:
                if (playerHealth <= 75) playerHealth += 25;
                else playerHealth = 100;
                dialougeText.text = "Your health has been replenished.";
                Debug.Log("PlayerHealth" + playerHealth);
                break;
            case 4:
                coin = Random.Range(0, 2);
                if (coin == 1)
                {
                    if (playerHealth <= 75) playerHealth += 25;
                    else playerHealth = 100;
                    dialougeText.text = "Your health has been replenished.";
                    Debug.Log("PlayerHealth" + playerHealth);
                }
                else
                {
                    if (enemyHealth <= 350) enemyHealth += 50;
                    else enemyHealth = 400;
                    dialougeText.text = "Oh no! The enemy's health has been replenished. ";
                    Debug.Log("EnemyHealth" + enemyHealth);
                }
                break;
            case 5:
                blockAtk = true;
                break;
        }
    }

}
