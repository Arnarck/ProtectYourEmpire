using TMPro;
using UnityEngine;

public class Bank : MonoBehaviour
{
    [Header("Currencies")]
    [Tooltip("The audio that plays when the player receive currencies.")]
    [SerializeField] AudioClip ReceivingCurrencies;
    [Tooltip("The text that displays the player's amount of currencies.")]
    [SerializeField] TextMeshProUGUI currenciesDisplay;
    [Tooltip("The amount of currencies the player will start the game.")]
    [SerializeField] int initialCurrencies = 20;
    [Tooltip("The maximum amount of currencies the player can have.")]
    [SerializeField] int maxCurrencies = 1000;

    AudioSource _audioSource;
    int _currentCurrencies;

    public int CurrentCurrencies { get => _currentCurrencies; }

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _currentCurrencies = initialCurrencies > maxCurrencies ? maxCurrencies : initialCurrencies;
    }

    void Start()
    {
        DisplayCurrencies();
    }

    // Displays the player's updated amount of currencies
    void DisplayCurrencies()
    {
        currenciesDisplay.text = $"${_currentCurrencies}";
    }

    // Increase the player's amount of currencies
    public void Deposit(int amount)
    {
        // prevents coins from being deposited if the player already has the maximum amount of coins
        if (_currentCurrencies >= maxCurrencies) { return; }

        // protects against negative numbers being passed by parameter
        if (amount < 0)
        {
            amount = Mathf.Abs(amount);
        }

        _audioSource.PlayOneShot(ReceivingCurrencies);
        _currentCurrencies += amount;

        // restricts the number of current currencies to never be higher than the maximum limit
        if (_currentCurrencies > maxCurrencies)
        {
            _currentCurrencies = maxCurrencies;
        }

        DisplayCurrencies();
    }

    // Decreases the amount of currencies. Return true if the transaction be successful, and false if not
    public bool Withdraw(int amount)
    {
        // Cancel the transaction if the player doesn't have enough coins
        if (_currentCurrencies < amount) { return false; }

        // protects against negative numbers being passed by parameter
        if (amount < 0)
        {
            amount = Mathf.Abs(amount);
        }

        _currentCurrencies -= amount;
        DisplayCurrencies();
        return true;
    }
}
