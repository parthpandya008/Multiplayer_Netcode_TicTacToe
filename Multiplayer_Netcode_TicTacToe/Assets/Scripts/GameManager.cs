using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event EventHandler<OnClickedOnGridPositionEventArgs> OnClickedOnGridPosition;

    public class OnClickedOnGridPositionEventArgs: EventArgs
    {
        public int x;
        public int y;
    }

    private void Awake()
    {
        Instance = this;
    }
    public void ClickedOnGridPosition(int x, int y)
    {
        Debug.Log("ClickedOnGridPosition " + " x" + x + " y" + y);
        OnClickedOnGridPositionEventArgs onClickedOnGridPositionEventArgs = new OnClickedOnGridPositionEventArgs();
        onClickedOnGridPositionEventArgs.x = x;
        onClickedOnGridPositionEventArgs.y = y;
        OnClickedOnGridPosition?.Invoke(this, onClickedOnGridPositionEventArgs);
    }
}
