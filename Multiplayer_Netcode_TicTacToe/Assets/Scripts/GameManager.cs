using System;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    public event EventHandler<OnClickedOnGridPositionEventArgs> OnClickedOnGridPosition;

    public class OnClickedOnGridPositionEventArgs: EventArgs
    {
        public int x;
        public int y;
        public PlayerType playerType;
    }

    public enum PlayerType
    {
        None,
        Cross,
        Circle
    }

    public PlayerType localPlayerType;
    public PlayerType currentPlayerPlayerType;

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log(" OnNetworkSpawn LocalClientId " + NetworkManager.Singleton.LocalClientId);
        if (NetworkManager.Singleton.LocalClientId == 0)
        {
            localPlayerType = PlayerType.Cross;
        }
        else 
        {
            localPlayerType = PlayerType.Circle;
        }

        if (IsServer)
        {
            currentPlayerPlayerType = PlayerType.Cross;
        }
    }

    [Rpc(SendTo.Server)]
    public void ClickedOnGridPositionRpc(int x, int y, PlayerType playerType)
    {
        Debug.Log("ClickedOnGridPosition " + " x" + x + " y" + y);
        if (playerType != currentPlayerPlayerType)
        {
            return;
        }
        
        OnClickedOnGridPositionEventArgs onClickedOnGridPositionEventArgs = new OnClickedOnGridPositionEventArgs();
        onClickedOnGridPositionEventArgs.x = x;
        onClickedOnGridPositionEventArgs.y = y;
        onClickedOnGridPositionEventArgs.playerType = playerType;
        OnClickedOnGridPosition?.Invoke(this, onClickedOnGridPositionEventArgs);

        //Give the turn to next Player Type
        switch (currentPlayerPlayerType)
        {
            case PlayerType.Cross:
                currentPlayerPlayerType = PlayerType.Circle;
                break;
            case PlayerType.Circle:
                currentPlayerPlayerType = PlayerType.Cross;
                break;
        }
    }

    public PlayerType GetLocalPlayerType()
    {
        return localPlayerType;
    }
}
