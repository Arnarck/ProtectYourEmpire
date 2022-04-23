using UnityEngine;
using TMPro;
using UnityEditor.Experimental.SceneManagement;

[ExecuteAlways]
public class CoordinateLabeler : MonoBehaviour
{
    TextMeshPro _label;
    Vector2Int _coordinates;

    void Awake()
    {
        _label = GetComponent<TextMeshPro>();
        // Display coordinates and update the name only when the game starts
        // In play mode, the waypoints shouldn't be moved, so that's why the coordinates and name are updated once
        DisplayCoordinates();
        UpdateName();
        // Deactivates the label in play mode
        if (Application.isPlaying) { _label.enabled = false; }
    }

    void Update()
    {
        // Only updates the info in Scene Mode. Ignore when is in play mode or prefab mode
        if (!Application.isPlaying && PrefabStageUtility.GetPrefabStage(gameObject) == null)
        {
            DisplayCoordinates();
            UpdateName();
        }

        ToggleLabelActiveState();
    }

    // Display the current tile coordinates on the screen
    void DisplayCoordinates()
    {
        _coordinates.x = Mathf.RoundToInt(transform.position.x / UnityEditor.EditorSnapSettings.move.x);
        _coordinates.y = Mathf.RoundToInt(transform.position.z / UnityEditor.EditorSnapSettings.move.z);

        _label.text = _coordinates.ToString();
    }

    // Makes the tile name be equals to its coordinates
    void UpdateName()
    {
        transform.parent.gameObject.name = _coordinates.ToString();
    }

    // Enable or disable the labels
    void ToggleLabelActiveState()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            _label.enabled = !_label.enabled;
        }
    }
}
