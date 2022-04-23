using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [Tooltip("The highlight color when the tile is placeable.")]
    [SerializeField] Color defaultColor = Color.green;
    [Tooltip("The highlight color when the tile is NOT placeable.")]
    [SerializeField] Color blockedColor = Color.red;
    [Tooltip("The effect that is played when the mouse cursor enters the tile.")]
    [SerializeField] GameObject highlight;
    [Tooltip("The effect that shows the tower attack range.")]
    [SerializeField] GameObject towerRangeDisplay;
    [Tooltip("The Tower script.")]
    [SerializeField] Tower towerPrefab;
    [Tooltip("Can a tower be placed above this tile?")]
    [SerializeField] bool isPlaceable;

    Bank _bank;
    Tower _towerPlaced = null;
    PauseGame _pauseGame;
    TowerSelector _towerSelector; // Contain the info about the tower the player wants to place.
    ParticleSystem _highlightParticles;
    PoolingSystemFinder _poolingSystemFinder;

    bool isMouseAboveThisTile;

    public Tower TowerPlaced { set => _towerPlaced = value; }
    public bool IsPlaceable { get => isPlaceable; set => isPlaceable = value; }

    void Awake()
    {
        _bank = FindObjectOfType<Bank>();
        _pauseGame = FindObjectOfType<PauseGame>();
        _towerSelector = FindObjectOfType<TowerSelector>();
        _highlightParticles = highlight.GetComponent<ParticleSystem>();
        _poolingSystemFinder = FindObjectOfType<PoolingSystemFinder>();
    }

    void Start()
    {
        highlight.SetActive(false);
    }

    void Update()
    {
        if (isMouseAboveThisTile && !_pauseGame.IsGamePaused)
        {
            RemoveTower();
        }
    }

    // Remove the tower from this waypoint.
    void RemoveTower()
    {
        if (!Input.GetMouseButtonDown(2) || _towerPlaced == null) { return; }

        _towerPlaced.DeactivateTower();
    }

    void OnMouseDown()
    {
        // Place a tower above the tile if the tile is placeable and if the game isn't paused
        if (isPlaceable && !_pauseGame.IsGamePaused)
        {
            PurchaseTower();
        }
    }

    // Allows the player to place a tower if he has enough currencies.
    void PurchaseTower()
    {
        // Get the current selected tower from TowerSelector script.
        Tower selectedTower = _towerSelector.SelectedTower;
        PoolingSystem towerPool = _poolingSystemFinder.FindPoolingSystem(selectedTower.gameObject);
        // makes the player pay for the tower creation.
        bool wasTransactionSuccessful;

        // Exit the method if the tower pool is not found.
        if (towerPool == null) { return; }

        wasTransactionSuccessful = _bank.Withdraw(selectedTower.CreationCost);
        if (wasTransactionSuccessful)
        {
            PlaceTower(towerPool);
        }
    }

    // Place a tower above the current tile.
    void PlaceTower(PoolingSystem towerPool)
    {
        GameObject towerObject = towerPool.GetObject();
        _towerPlaced = towerObject.GetComponent<Tower>();

        isPlaceable = false;
        towerObject.transform.position = transform.position;
        towerObject.GetComponent<Tower>().WaypointSpawned = this;
    }

    void OnMouseEnter()
    {
        SetWaypointActive(true);
        SetHighlightColor();
    }

    // Activates/disables the waypoint effects when the mouse cursor enters/exits this tile.
    void SetWaypointActive(bool isActive)
    {
        isMouseAboveThisTile = isActive;
        highlight.SetActive(isActive);
        towerRangeDisplay.SetActive(isActive);
    }

    void OnMouseOver()
    {
        SetHighlightColor();
    }

    // Changes the highlight color based on the "isPlaceable" value.
    void SetHighlightColor()
    {
        var main = _highlightParticles.main;

        if (isPlaceable)
        {
            main.startColor = defaultColor;
        }
        else
        {
            main.startColor = blockedColor;
        }
    }

    void OnMouseExit()
    {
        SetWaypointActive(false);
    }
}
