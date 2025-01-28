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


    private void Start()
    {
        GameManager.Instance.OnClickedOnGridPosition += OnClickedOnGridPosition;
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