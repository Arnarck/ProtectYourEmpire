using System.Collections;
using TMPro;
using UnityEngine;

public class MovingText : MonoBehaviour
{
    [Header("Text Movement")]
    [Tooltip("The direction and speed the text will move.")]
    [SerializeField] Vector3 throwDirection;
    [Tooltip("Randomize the direction the text will move. Values vary between '-throwDirection' and 'throwDirection'.")]
    [SerializeField] bool isRandomizingX, isRandomizingY, isRandomizingZ;

    [Header("Lifetime")]
    [Tooltip("How long the text will live.")]
    [SerializeField] float lifeTime;
    [Tooltip("How fast the text alpha channel value will decrease.")]
    [SerializeField] float fadeOutSpeed = 3f;

    TextMeshPro _text;
    Color _defaultColor;
    PauseGame _pauseGame;
    RectTransform _rectTransform;

    float _xThrow, _yThrow, _zThrow;

    void Awake()
    {
        _text = GetComponent<TextMeshPro>();
        _pauseGame = FindObjectOfType<PauseGame>();
        _rectTransform = GetComponent<RectTransform>();

        _defaultColor = _text.color;
    }

    void OnEnable()
    {
        ResetStatus();
        StartCoroutine(LifetimeCountdown());
    }

    void ResetStatus()
    {
        _text.color = _defaultColor;

        _xThrow = isRandomizingX ? Random.Range(-throwDirection.x, throwDirection.x) : throwDirection.x;
        _yThrow = isRandomizingY ? Random.Range(-throwDirection.y, throwDirection.y) : throwDirection.y;
        _zThrow = isRandomizingZ ? Random.Range(-throwDirection.z, throwDirection.z) : throwDirection.z;
    }

    void Update()
    {
        if (!_pauseGame.IsGamePaused)
        {
            ProcessMovement();
        }
    }

    IEnumerator LifetimeCountdown()
    {
        yield return new WaitForSeconds(lifeTime);
        StartCoroutine(FadeOut());
    }

    // Move the damage text.
    void ProcessMovement()
    {
        Vector3 translationThisFrame = new Vector3(_xThrow, _yThrow, _zThrow) * Time.deltaTime;
        _rectTransform.Translate(translationThisFrame, Space.World);
    }

    // Reduces the alpha channel value of the damage text over time.
    IEnumerator FadeOut()
    {
        float alphaValue = _text.color.a;

        while (alphaValue > 0f)
        {
            float alphaChangeThisFrame = Time.deltaTime * fadeOutSpeed;
            alphaValue -= alphaChangeThisFrame;
            _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, alphaValue);

            yield return new WaitForEndOfFrame();
        }

        gameObject.SetActive(false);
    }
}
