using UnityEngine;

public class TowerSelector : MonoBehaviour
{
    [Tooltip("The tower that will initially be selected for the player.")]
    [SerializeField] Tower defaultTowerPrefab;

    Tower _selectedTower;

    // When the player opens the "Tower Selector" menu and click on a tower (fire, ice, shock, common),
    // the "_selectedTower" receive a reference to that tower the player clicked.
    // The "_selectedTower" is used in the "Waypoint" script when the player clicks on a placeable tile to create a tower
    public Tower SelectedTower { get => _selectedTower; set => _selectedTower = value; }

    void Awake()
    {
        _selectedTower = defaultTowerPrefab;
    }
}
