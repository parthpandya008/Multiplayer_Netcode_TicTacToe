using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public class OnClickedOnGridPositionEventArgs : EventArgs
    {
        public int x;
        public int y;
        public PlayerType playerType;
    }
    public class OnGameWinEventArgs : EventArgs
    {
        public LineData line;
        public PlayerType winPlayerType;
    }

    public enum PlayerType
    {
        None,
        Cross,
        Circle
    }

    public enum LineOrientation
    {
        None,
        Horizontal, 
        Vertical,
        DiagonalA, 
        DiagonalB
    }

    public struct LineData
    {
        public List<Vector2Int> gridVector2IntList;
        public Vector2Int centerGridPosition;
        public LineOrientation orientation;
    }

    public static GameManager Instance { get; private set; }

    public event EventHandler<OnClickedOnGridPositionEventArgs> OnClickedOnGridPosition;
    public event EventHandler OnGameStarted;
    public event EventHandler<OnGameWinEventArgs> OnGameWin;
    public event EventHandler OnCurrentPlayerTypeChanged;

    private PlayerType localPlayerType;
    private NetworkVariable<PlayerType> currentPlayerPlayerType = new NetworkVariable<PlayerType>();

    private PlayerType[,] playerTypeArray;

    private List<LineData> linesDataList;

    private void Awake()
    {
        Instance = this;
        playerTypeArray = new PlayerType[3, 3];
        linesDataList = new List<LineData>();

        #region set-LineData
        
        //Horizontal Lines
        LineData hLine1Data = new LineData
        {
           gridVector2IntList = new List<Vector2Int> { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0) },
           centerGridPosition = new Vector2Int(1, 0),
           orientation = LineOrientation.Horizontal           
        };
        linesDataList.Add(hLine1Data);

        LineData hLine2Data = new LineData
        {
            gridVector2IntList = new List<Vector2Int> { new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1) },
            centerGridPosition = new Vector2Int(1, 1),
            orientation = LineOrientation.Horizontal
        };
        linesDataList.Add(hLine2Data);

        LineData hLine3Data = new LineData
        {
            gridVector2IntList = new List<Vector2Int> { new Vector2Int(0, 2), new Vector2Int(1, 2), new Vector2Int(2, 2) },
            centerGridPosition = new Vector2Int(1, 2),
            orientation = LineOrientation.Horizontal
        };
        linesDataList.Add(hLine3Data);

        //Vertical Lines
        LineData vhLine1Data = new LineData
        {
            gridVector2IntList = new List<Vector2Int> { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2) },
            centerGridPosition = new Vector2Int(0, 1),
            orientation = LineOrientation.Vertical
        };
        linesDataList.Add(vhLine1Data);

        LineData vLine2Data = new LineData
        {
            gridVector2IntList = new List<Vector2Int> { new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(1, 2) },
            centerGridPosition = new Vector2Int(1, 1),
            orientation = LineOrientation.Vertical
        };
        linesDataList.Add(vLine2Data);

        LineData vLine3Data = new LineData
        {
            gridVector2IntList = new List<Vector2Int> { new Vector2Int(2, 0), new Vector2Int(2, 1), new Vector2Int(2, 2) },
            centerGridPosition = new Vector2Int(2, 1),
            orientation = LineOrientation.Vertical
        };
         linesDataList.Add(vLine3Data);

        //Diagonal
        LineData dLine1Data = new LineData
        {
            gridVector2IntList = new List<Vector2Int> { new Vector2Int(0, 0), new Vector2Int(1, 1), new Vector2Int(2, 2) },
            centerGridPosition = new Vector2Int(1, 1),
            orientation = LineOrientation.DiagonalA
        };
        linesDataList.Add(dLine1Data);

        LineData dLine2Data = new LineData
        {
            gridVector2IntList = new List<Vector2Int> { new Vector2Int(0, 2), new Vector2Int(1, 1), new Vector2Int(2, 0) },
            centerGridPosition = new Vector2Int(1, 1),
            orientation = LineOrientation.DiagonalB
        };
        linesDataList.Add(dLine2Data);
       
        #endregion
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
        
        if (playerTypeArray[x, y] != PlayerType.None)
        {
            //Already occupied
            return ;
        }
        playerTypeArray[x,y] = playerType;
        
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
        TestWinner();
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

    private bool TestWinnerLine(LineData line)
    {
        PlayerType aPlayerType = playerTypeArray[line.gridVector2IntList[0].x, line.gridVector2IntList[0].y];
        PlayerType bPlayerType = playerTypeArray[line.gridVector2IntList[1].x, line.gridVector2IntList[1].y];
        PlayerType cPlayerType = playerTypeArray[line.gridVector2IntList[2].x, line.gridVector2IntList[2].y];

        return TestWinnerLine(aPlayerType, bPlayerType, cPlayerType);
    }

    private bool TestWinnerLine(PlayerType aPlayerType, PlayerType bPlayerType, PlayerType cPlayerType)
    {
        return (aPlayerType != PlayerType.None && aPlayerType == bPlayerType & aPlayerType == cPlayerType);
    }

    //This method is also server authoritative as it's called from server authoritative method
    private void TestWinner()
    {
        for(int i = 0; i < linesDataList.Count; i++) 
        //foreach (LineData line in linesDataList)
        {
            LineData line = linesDataList[i];
            if (TestWinnerLine(line))
            {
                //WIn
                Debug.Log("Winner!");
                currentPlayerPlayerType.Value = PlayerType.None;
                TriggerOnGameWinRpc(i, playerTypeArray[line.centerGridPosition.x, line.centerGridPosition.y]);
               
                break;
            }
        }

        /*if (TestWinnerLine(playerTypeArray[0, 0], playerTypeArray[1, 0], playerTypeArray[2, 0]))
        {
            //WIn
            Debug.Log("Winner!");
            OnGameWin?.Invoke(this, new OnGameWinEventArgs 
            {
                gridCenterPosition = new Vector2Int(1, 0),
            });
            currentPlayerPlayerType.Value = PlayerType.None;
        }*/
    }

    //Here we are passing lineIndex because we cannot pass the LineData directly as Rpc methods has some limitations
    //Here the linesDataList is comon which is at both the side, client and server
    //To pass the LineData into RPC method we need to do some Network serilzation
    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameWinRpc(int lineIndex, PlayerType winPlayerType)
    {
       LineData line = linesDataList[lineIndex];
        OnGameWin?.Invoke(this, new OnGameWinEventArgs
        {
            line = line,
            winPlayerType = winPlayerType
        });
    }
}
