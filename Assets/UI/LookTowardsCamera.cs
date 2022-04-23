using UnityEngine;

public class LookTowardsCamera : MonoBehaviour
{
    PauseGame _pauseGame;
    Transform _cameraCoordinates;

    void Awake()
    {
        _cameraCoordinates = Camera.main.transform;
        _pauseGame = FindObjectOfType<PauseGame>();
    }

    void Update()
    {
        if (!_pauseGame.IsGamePaused)
        {
            AlignWithCameraRotation();
        }
    }

    // Makes the object have the same rotation as the camera.
    void AlignWithCameraRotation()
    {
        transform.rotation = _cameraCoordinates.rotation;
    }
}
