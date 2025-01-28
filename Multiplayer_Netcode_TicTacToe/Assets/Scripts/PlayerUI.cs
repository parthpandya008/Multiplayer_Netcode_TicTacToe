using System;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    private GameObject crossArrowGameObject;
    [SerializeField]
    private GameObject circleArrowGameObject;
    [SerializeField]
    private GameObject crossYouTextGameObject;
    [SerializeField]
    private GameObject circleYouTextGameObject;

    private void Awake()
    {
        crossArrowGameObject.SetActive(false);
        circleArrowGameObject.SetActive(false);
        circleYouTextGameObject.SetActive(false);
        crossYouTextGameObject.SetActive(false);
    }

    private void Start()
    {
        GameManager.Instance.OnGameStarted += OnGameStarted;
        GameManager.Instance.OnCurrentPlayerTypeChanged += OnCurrentPlayerTypeChanged;
    }

    private void OnCurrentPlayerTypeChanged(object sender, EventArgs e)
    {
        UpdateCurrentArrow();
    }

    private void OnGameStarted(object sender, EventArgs e)
    {
        if(GameManager.Instance.GetLocalPlayerType() == GameManager.PlayerType.Cross)
        {
            crossYouTextGameObject.SetActive(true);
        }
        else
        {
            circleYouTextGameObject.SetActive(true);
        }
        UpdateCurrentArrow();
    }

    private void UpdateCurrentArrow()
    {
        if (GameManager.Instance.GetCurrentPlayerType() == GameManager.PlayerType.Cross)
        {
            crossArrowGameObject.SetActive(true) ;
            circleArrowGameObject .SetActive(false) ;
        }
        else
        {
            crossArrowGameObject.SetActive(false);
            circleArrowGameObject.SetActive(true);
        }
    }
}
