using System;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    public event EventHandler<OnClickedOnGridPositionEventArgs> OnClickedOnGridPosition;
    public event EventHandler OnGameStarted;
    public event EventHandler OnCurrentPlayerTypeChanged;

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

    private PlayerType localPlayerType;
    private NetworkVariable<PlayerType> currentPlayerPlayerType = new NetworkVariable<PlayerType>();

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

        //We just need to set this for server only, as the spawn logic is run on server only
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
        }
        currentPlayerPlayerType.OnValueChanged += OnCurrentPlayerTypeNetworkVariableChanged;
    }

    private void OnCurrentPlayerTypeNetworkVariableChanged(PlayerType oldValue, PlayerType newValue)
    {
        OnCurrentPlayerTypeChanged?.Invoke(this, EventArgs.Empty);
    }

    private void OnClientConnectedCallback(ulong obj)
    {
        if (NetworkManager.Singleton.ConnectedClientsList.Count == 2)
        {
            currentPlayerPlayerType.Value = PlayerType.Cross;
            TriggerOnGameStartedRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameStartedRpc()
    {
        OnGameStarted?.Invoke(this, EventArgs.Empty);
    }

    [Rpc(SendTo.Server)]
    public void ClickedOnGridPositionRpc(int x, int y, PlayerType playerType)
    {
        Debug.Log("ClickedOnGridPosition " + " x" + x + " y" + y);
        if (playerType != currentPlayerPlayerType.Value)
        {
            return;
        }
        
        OnClickedOnGridPositionEventArgs onClickedOnGridPositionEventArgs = new OnClickedOnGridPositionEventArgs();
        onClickedOnGridPositionEventArgs.x = x;
        onClickedOnGridPositionEventArgs.y = y;
        onClickedOnGridPositionEventArgs.playerType = playerType;
        OnClickedOnGridPosition?.Invoke(this, onClickedOnGridPositionEventArgs);

        //Give the turn to next Player Type
        switch (currentPlayerPlayerType.Value)
        {
            case PlayerType.Cross:
                currentPlayerPlayerType.Value = PlayerType.Circle;
                break;
            case PlayerType.Circle:
                currentPlayerPlayerType.Value = PlayerType.Cross;
                break;
        }
       // TriggerOnCurrentPlayerTypeChangedRpc();
    }

    /*[Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnCurrentPlayerTypeChangedRpc()
    {
        OnCurrentPlayerTypeChanged?.Invoke(this, EventArgs.Empty);
    }*/

    public PlayerType GetLocalPlayerType()
    {
        return localPlayerType;
    }

    public PlayerType GetCurrentPlayerType()
    {
        return currentPlayerPlayerType.Value;
    }
}
