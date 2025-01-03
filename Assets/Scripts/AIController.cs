using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIController : MonoBehaviour
{
    [SerializeField]
    private Material hitMaterial;  // Matériau pour une case touchée
    [SerializeField]
    private Material missMaterial; // Matériau pour une case manquée

    private HashSet<string> firedPositions = new HashSet<string>(); // Ensemble des positions déjà tirées
    private Queue<string> targetedPositions = new Queue<string>(); // Positions prioritaires à cibler

    private int gridHeight = 10;
    private int gridWidth = 10;

    public void InitializeAIShips(GridController gridController, List<BoatData> boatsData)
    {
        // Code inchangé pour placer les bateaux
        Debug.Log("Initializing AI Ships");
        Debug.Log("Boats data count: " + boatsData.Count);

        foreach (var boatData in boatsData)
        {
            bool placed = false;
            int maxAttempts = 100; // Limit the number of attempts
            int attempts = 0;

            List<string> allPositions = new List<string>();
            for (char row = 'A'; row < 'A' + gridHeight; row++)
            {
                for (int col = 1; col <= gridWidth; col++)
                {
                    allPositions.Add($"{row}{col}");
                }
            }

            while (!placed && attempts < maxAttempts)
            {
                attempts++;
                int randomIndex = Random.Range(0, allPositions.Count);
                string randomStartPos = allPositions[randomIndex];
                allPositions.RemoveAt(randomIndex);

                bool randomIsHorizontal = Random.Range(0, 2) == 0;
                GameObject boatInstance = Instantiate(boatData.boatPrefab);
                foreach (Renderer renderer in boatInstance.GetComponentsInChildren<Renderer>())
                {
                    renderer.enabled = false;
                }

                Boat boatComponent = boatInstance.GetComponent<Boat>();
                if (boatComponent != null)
                {
                    boatComponent.InitializeBoat(boatData);
                    placed = gridController.PlaceBoat(boatComponent, randomStartPos, randomIsHorizontal);

                    if (!placed) Destroy(boatInstance);
                }
                else
                {
                    Debug.LogError($"Boat prefab {boatData.boatPrefab.name} does not have a Boat component!");
                    Destroy(boatInstance);
                }
            }

            if (!placed)
            {
                Debug.LogError($"Failed to place boat {boatData.boatName} after {maxAttempts} attempts.");
            }
        }
    }

    public void TakeTurn(GridController playerGrid, System.Action onTurnEnd)
    {
        string targetPosition;

        if (targetedPositions.Count > 0)
        {
            targetPosition = targetedPositions.Dequeue();
        }
        else
        {
            do
            {
                targetPosition = GetRandomPositionCode();
            } while (firedPositions.Contains(targetPosition));
        }

        firedPositions.Add(targetPosition);

        if (playerGrid.Shoot(targetPosition))
        {
            playerGrid.grid.SetCellMaterial(targetPosition, hitMaterial);

            if (playerGrid.cellToBoat.TryGetValue(targetPosition, out Boat boat))
            {
                if (boat.IsSunk())
                {
                    Debug.Log($"Le bateau {boat.GetName()} est coulé !");
                    List<string> sunkBoatPositions = playerGrid.GetBoatCells(boat);

                    foreach (string pos in sunkBoatPositions)
                    {
                        RemoveFromQueueAndNeighbors(pos, playerGrid);
                    }
                }
                else
                {
                    foreach (string neighbor in GetNeighborPositions(targetPosition))
                    {
                        if (!firedPositions.Contains(neighbor) && !targetedPositions.Contains(neighbor))
                        {
                            targetedPositions.Enqueue(neighbor);
                        }
                    }
                }
            }
        }
        else
        {
            playerGrid.grid.SetCellMaterial(targetPosition, missMaterial);
        }

        onTurnEnd.Invoke();
    }

    private void RemoveFromQueueAndNeighbors(string position, GridController playerGrid)
    {
        targetedPositions = new Queue<string>(targetedPositions.Where(pos => pos != position));

        foreach (string neighbor in GetNeighborPositions(position))
        {
            if (!playerGrid.cellToBoat.ContainsKey(neighbor)) // Ne retire que les voisins inoccupés
            {
                targetedPositions = new Queue<string>(targetedPositions.Where(pos => pos != neighbor));
            }
        }
    }

    private List<string> GetNeighborPositions(string position)
    {
        List<string> neighbors = new List<string>();

        char row = position[0];
        int col = int.Parse(position.Substring(1));

        if (row > 'A') neighbors.Add($"{(char)(row - 1)}{col}");
        if (row < 'A' + gridHeight - 1) neighbors.Add($"{(char)(row + 1)}{col}");
        if (col > 1) neighbors.Add($"{row}{col - 1}");
        if (col < gridWidth) neighbors.Add($"{row}{col + 1}");

        return neighbors;
    }

    private string GetRandomPositionCode()
    {
        char row = (char)Random.Range(65, 65 + gridHeight);
        int col = Random.Range(1, gridWidth + 1);
        return $"{row}{col}";
    }

    public void setMaterail(Material hit, Material miss)
    {
        hitMaterial = hit;
        missMaterial = miss;
    }

    public void ResetAI()
    {
        firedPositions.Clear();
        targetedPositions.Clear();
    }
}
