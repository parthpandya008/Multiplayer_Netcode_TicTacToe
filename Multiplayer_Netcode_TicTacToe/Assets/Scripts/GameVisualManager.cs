using System;
using Unity.Netcode;
using UnityEngine;

public class GameVisualManager : NetworkBehaviour
{
    private const float GRID_SIZE = 3.1f;

    [SerializeField]
    private Transform crossPrefab;
    [SerializeField]
    private Transform circlePrefab;
    [SerializeField]
    private Transform lineCompletePrefab;


    private void Start()
    {
        GameManager.Instance.OnClickedOnGridPosition += OnClickedOnGridPosition;
        GameManager.Instance.OnGameWin += OnGameWin;
    }

    private void OnGameWin(object sender, GameManager.OnGameWinEventArgs e)
    {
       if(!NetworkManager.Singleton.IsServer)
        {
            return;
        }
        float eularZ = 0;

        switch(e.line.orientation)
        {
            case GameManager.LineOrientation.Horizontal:
                eularZ = 0;
                break;
            case GameManager.LineOrientation.Vertical:
                eularZ = 90;
                break;
            case GameManager.LineOrientation.DiagonalA:
                eularZ = 45; 
                break;
            case GameManager.LineOrientation.DiagonalB:
                eularZ = - 45;
                break;
        }

        Quaternion lineQuaternion = Quaternion.Euler(0,0, eularZ);
        Transform lineCompleteTransform = Instantiate(lineCompletePrefab, 
            GridPosition (e.line.centerGridPosition.x, e.line.centerGridPosition.y), lineQuaternion);

        //This is how we spawn the object on the netwrok for all the client
        lineCompleteTransform.GetComponent<NetworkObject>().Spawn(true);
    }

    private void OnClickedOnGridPosition(object sender, GameManager.OnClickedOnGridPositionEventArgs e)
    {
        Debug.Log("OnClickedOnGridPosition");
        SpawnObjectRpc(e.x, e.y, e.playerType);
    }

    [Rpc(SendTo.Server)]
    private void SpawnObjectRpc(int x, int y, GameManager.PlayerType playerType)
    {
        Debug.Log("RPC SpawnObject playerType " + playerType);
        Transform prefab = null;
        
        switch(playerType)
        {
            case GameManager.PlayerType.Cross:
                prefab = crossPrefab;
                break;
            case GameManager.PlayerType.Circle:
                prefab = circlePrefab;
                break;
        }

        Transform spawnedTransform = Instantiate(prefab, GridPosition(x, y), Quaternion.identity);
        //This is how we spawn the object on the server, so it gets spawned on all the client
        spawnedTransform.GetComponent<NetworkObject>().Spawn(true);
        //spawnedTransform.position = GridPosition(x, y);
    }

    private Vector2 GridPosition(int x, int y)
    {
        Vector2 gridPosition;
        gridPosition = new Vector2(-GRID_SIZE + x * GRID_SIZE, -GRID_SIZE + y * GRID_SIZE);

        return gridPosition;
    }
}