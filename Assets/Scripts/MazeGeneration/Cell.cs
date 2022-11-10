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

    public int[] neighbourCellIndex = new int[] { -1, -1, -1, -1, -1, -1 }; // left, right, bottom, top, below, above
    private Cell[] neighbourCells = new Cell[6];
    public List<Cell> availableNeighbourCells = new List<Cell>();
    public Cell belowNeighbourCell, aboveNeighbourCell;
    public int unvisitedNeighbourCells = 4;
    public int amountOfcellsOnPreviousFloors;

    public void OnCreation(int x, int z, int y)
    {
        //Debug.Log("Index: " + index + " Neighbours: " + neighbourCellIndex[0] + " " + neighbourCellIndex[1] + " x: " + x + "  z:" + z + " y: " + y);
    }

    public void AssignCellNeighbours(int floorSizeX, int floorSizeZ, int mazeSizeY, int previousFloorCellAmount, int x, int z, int y)
    {
        // left neighbour
        if (index == floorSizeZ * x + previousFloorCellAmount)
            neighbourCellIndex[0] = -1;
        else
        {
            neighbourCellIndex[0] = index - 1;
            neighbourCells[0] = MazeManager.cells[index - 1];
            availableNeighbourCells.Add(neighbourCells[0]);

            // Assign right neighbour to the left neighbour
            neighbourCells[0].neighbourCellIndex[1] = index;
            neighbourCells[0].neighbourCells[1] = this;
            neighbourCells[0].availableNeighbourCells.Add(neighbourCells[0].neighbourCells[1]);
        }

        // bottom neighbour
        if (index - floorSizeZ - previousFloorCellAmount < 0)
            neighbourCellIndex[2] = -1;
        else
        {
            neighbourCellIndex[2] = index - floorSizeZ;
            neighbourCells[2] = MazeManager.cells[index - floorSizeZ];
            availableNeighbourCells.Add(neighbourCells[2]);

            // Assign top neighbour to the bottom neighbour
            neighbourCells[2].neighbourCellIndex[3] = index;
            neighbourCells[2].neighbourCells[3] = this;
            neighbourCells[2].availableNeighbourCells.Add(neighbourCells[2].neighbourCells[3]);
        }

        // above neighbour
        //if (isPyramid)
        //{
        //    if ((index - previousFloorCellAmount + 1) % (mazeDimension.y - y) == 0 || index >= Mathf.Ceil((floorSizeX - 1) * (floorSizeZ - 1) + previousFloorCellAmount + mazeDimension.y - (y + 1)))
        //        neighbourCellIndex[4] = -1;
        //    else
        //        neighbourCellIndex[4] = index + floorSizeX * floorSizeZ - x;
        //}
        //else
        //{

        if (index - floorSizeX * floorSizeZ < 0)
            neighbourCellIndex[4] = -1;
        else
        {
            neighbourCellIndex[4] = index - floorSizeX * floorSizeZ;
            neighbourCells[4] = MazeManager.cells[index - floorSizeX * floorSizeZ];
            belowNeighbourCell = neighbourCells[4];

            // Assign top neighbour to the bottom neighbour
            neighbourCells[4].neighbourCellIndex[5] = index;
            neighbourCells[4].neighbourCells[5] = this;
            neighbourCells[4].aboveNeighbourCell = neighbourCells[4].neighbourCells[5];
        }

        //if (index + floorSizeX * floorSizeZ >= floorSizeX * floorSizeZ * mazeSizeY)
        //    neighbourCellIndex[5] = -1;
        //else
        //{
        //    neighbourCellIndex[5] = index + floorSizeX * floorSizeZ;
        //    neighbourCells[5] = MazeManager.cells[index + floorSizeX * floorSizeZ];
        //}
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

            // Remove the current cell from the neighbours list so it doesn't backtrack
            newNeighbourCell.availableNeighbourCells.Remove(this);

            availableNeighbourCells.Remove(newNeighbourCell);
            return newNeighbourCell;
        }
        else
        {
            return null;
        }
    }
}