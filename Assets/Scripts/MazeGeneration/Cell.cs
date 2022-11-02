using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    // Cell information
    public Vector3[] positions = new Vector3[] { new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, .9f), new Vector3(0.9f, 0f, 0f) };
    public Vector3[] dimensions = new Vector3[] { new Vector3(1f, .1f, 1f), new Vector3(1f, 1f, .1f), new Vector3(.1f, 1f, 1f), new Vector3(1f, 1f, .1f), new Vector3(.1f, 1f, 1f) };

    public bool[] dividers = new bool[] { true, true, true, true, true }; // the ground is the first divider
    public bool isStartCell, isEndCell;
    public bool visited;
    public int index;

    // Neighbour cell informatioon

    public int[] neighbourCellIndex = new int[] { -1, -1, -1, -1 };
    public List<Cell> availableNeighbourCells = new List<Cell>();
    public int unvisitedNeighbourCells = 4;
    public int amountOfcellsOnPreviousFloors;

    public void OnCreation()
    {
        //Debug.Log("Index: " + index + " Neighbours: " + neighbourCellIndex[0] + " " + neighbourCellIndex[1] + " " + neighbourCellIndex[2] + " " + neighbourCellIndex[3]);
    }

    public Cell GetRandomCellNeighbour()
    {
        for (int i = 0; i < availableNeighbourCells.Count; i++)
        {
            if (availableNeighbourCells[i].visited)
            {
                unvisitedNeighbourCells--;
                availableNeighbourCells.Remove(availableNeighbourCells[i]);
            }
        }

        if (availableNeighbourCells.Count > 0)
        {
            int r = Random.Range(0, availableNeighbourCells.Count);
            unvisitedNeighbourCells--;

            Cell newNeighbourCell = availableNeighbourCells[r];
            availableNeighbourCells.Remove(newNeighbourCell);
            return newNeighbourCell;
        }
        else
        {
            return null;
        }
    }
}