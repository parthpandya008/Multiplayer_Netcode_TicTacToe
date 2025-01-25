using System;
using Unity.Netcode;
using UnityEngine;

public class GameVisualManager : MonoBehaviour
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
        Transform spawnedTransform = Instantiate(crossPrefab);
        spawnedTransform.GetComponent<NetworkObject>().Spawn(true);
        spawnedTransform.position = GridPosition(e.x, e.y);
    }

    private Vector2 GridPosition(int x, int y)
    {
        Vector2 gridPosition;
        gridPosition = new Vector2(-GRID_SIZE + x * GRID_SIZE, -GRID_SIZE + y * GRID_SIZE);

        return gridPosition;
    }
}
