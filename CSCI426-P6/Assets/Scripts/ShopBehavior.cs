using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ShopBehavior : MonoBehaviour
{
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
    int playerHealth;
    float enemyAtk;
    int enemyHealth;
    bool freeRefresh;
    float atkMultiplier = 1.25f;
    bool blockAtk;

    // UI
    [SerializeField] TextMeshProUGUI choice1;
    [SerializeField] TextMeshProUGUI choice2;
    [SerializeField] TextMeshProUGUI choice3;
    [SerializeField] Button choice1Btn;
    [SerializeField] Button choice2Btn;
    [SerializeField] Button choice3Btn;
    [SerializeField] Button refreshBtn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        refreshBtn.onClick.AddListener(RefreshShop);
        choice1Btn.onClick.AddListener(Choice1);
        choice2Btn.onClick.AddListener(Choice2);
        choice3Btn.onClick.AddListener(Choice3);
        money = 10;
        blockAtk = false;
        //idk random vals we can change later
        playerAtk = 50f;
        enemyAtk = 25f;
        enemyHealth = 300;
        playerHealth = 100;
        //making sure money is not subtracted for auto refresh
        freeRefresh = true;
        RefreshShop();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
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
        if (money >= prices[powerUps[currChoice1]-1])
        {
            money -= prices[powerUps[currChoice1]-1];
            choice1Btn.gameObject.SetActive(false);
            // do behavior correlated to the actual choice here
            PowerUpBehavior(powerUps[currChoice1]);
            Debug.Log(money);
        }
    }

    void Choice2()
    {
        if (money >= prices[powerUps[currChoice2] - 1])
        {
            money -= prices[powerUps[currChoice2] - 1];
            choice2Btn.gameObject.SetActive(false);
            // do behavior correlated to the actual choice here
            PowerUpBehavior(powerUps[currChoice2]);
            Debug.Log(money);
        }
    }

    void Choice3()
    {
        if (money >= prices[powerUps[currChoice3] - 1])
        {
            money -= prices[powerUps[currChoice3] - 1];
            choice3Btn.gameObject.SetActive(false);
            // do behavior correlated to the actual choice here
            PowerUpBehavior(powerUps[currChoice3]);
            Debug.Log(money);
        }
    }

    void PowerUpBehavior(int powerUP)
    {
        int coin;
        switch (powerUP)
        {
            case 1:
                playerAtk *= atkMultiplier;
                Debug.Log("PlayerAtk" + playerAtk);
                break;
            case 2:
                coin = Random.Range(0, 2);
                if (coin == 1)
                {
                    playerAtk *= atkMultiplier;
                    Debug.Log("PlayerAtk" + playerAtk);
                }
                else
                {
                    enemyAtk *= atkMultiplier;
                    Debug.Log("EnemyAtk" + enemyAtk);
                }
                break;
            case 3:
                playerHealth += 25;
                Debug.Log("PlayerHealth" + playerHealth);
                break;
            case 4:
                coin = Random.Range(0, 2);
                if (coin == 1)
                {
                    playerHealth += 25;
                    Debug.Log("PlayerHealth" + playerHealth);
                }
                else
                {
                    enemyHealth += 50;
                    Debug.Log("EnemyHealth" + enemyHealth);
                }
                break;
            case 5:
                blockAtk = true;
                break;
        }
    }

}
