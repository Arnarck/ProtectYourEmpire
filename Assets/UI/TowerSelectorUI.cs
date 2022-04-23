using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerSelectorUI : MonoBehaviour
{
    [Header("Images")]
    [Tooltip("The image that visually represents the currently selected tower.")]
    [SerializeField] Image selectedTowerDisplay;
    [Tooltip("The sprite that visually represents the tower with which the player starts the game.")]
    [SerializeField] Sprite defaultSelectedTower;

    [Header("Towers")]
    [Tooltip("The tower representing the Bolt Ballista.")]
    [SerializeField] Tower boltBallista;
    [Tooltip("The tower representing the Fire Ballista.")]
    [SerializeField] Tower fireBallista;
    [Tooltip("The tower representing the Ice Ballista.")]
    [SerializeField] Tower iceBallista;
    [Tooltip("The tower representing the Shock Ballista.")]
    [SerializeField] Tower shockBallista;

    [Header("Price Display")]
    [Tooltip("The text representing the cost of building the Bolt Ballista.")]
    [SerializeField] TextMeshProUGUI boltCost;
    [Tooltip("The text representing the cost of building the Fire Ballista.")]
    [SerializeField] TextMeshProUGUI fireCost;
    [Tooltip("The text representing the cost of building the Ice Ballista.")]
    [SerializeField] TextMeshProUGUI iceCost;
    [Tooltip("The text representing the cost of building the Shock Ballista.")]
    [SerializeField] TextMeshProUGUI shockCost;
    [Tooltip("The text representing the cost of restoring castle health.")]
    [SerializeField] TextMeshProUGUI castleRestorationCost;

    Castle _castle;

    // Change the sprite of the selected tower every time the player changes the selected tower.
    public Sprite SelectedTowerDisplay { set => selectedTowerDisplay.sprite = value; }

    void Awake()
    {
        _castle = FindObjectOfType<Castle>();
    }

    void Start()
    {
        selectedTowerDisplay.sprite = defaultSelectedTower;
    }

    // Updates Tower Selector Menu data every time it is activated.
    public void UpdateInformation()
    {
        boltCost.text = $"${boltBallista.CreationCost}";
        fireCost.text = $"${fireBallista.CreationCost}";
        iceCost.text = $"${iceBallista.CreationCost}";
        shockCost.text = $"${shockBallista.CreationCost}";
        castleRestorationCost.text = $"${_castle.RestorationCost}";
    }

    // Change the sprite of the selected tower every time the player changes the selected tower.
    public void DisplaySelectedTower(Sprite towerDisplay)
    {
        selectedTowerDisplay.sprite = towerDisplay;
    }
}
