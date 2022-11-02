using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeManager : MonoBehaviour
{
    // Maze information
    [SerializeField]
    private Vector3 mazeDimension;
    private int currentFloor;
    [SerializeField]
    private List<int> cellAmountByFloor = new List<int>();
    private Dictionary<int, Vector3> floorDimensions = new Dictionary<int, Vector3>();

    public static List<Cell> cells = new List<Cell>();
    [SerializeField]
    public List<Cell> unvisitedCells = new List<Cell>();
    private Cell previousCell, currentCell, nextCell, endCell;

    // Different generation modes
    [SerializeField]
    private bool isPyramid;

    // Start is called before the first frame update
    void Start()
    {
        SpawnFloorGrid();
    }

    public void SpawnMaze()
    {
        DestroyMaze();
        SpawnFloorGrid();
    }

    // This functtion starts the generation of a new maze, the function can be called by a UI button.
    public void SpawnFloorGrid()
    {
        GameObject Floor;
        int floorCellAmount = 0;
        Vector3 sizeModifiers = Vector3.zero;
        Vector3 floorSize = mazeDimension;

        for (int y = 0; y < floorSize.y; y++)
        {
            Floor = new GameObject();
            Floor.name = "Floor" + y;
            Floor.transform.SetParent(transform);

            if (isPyramid)
                sizeModifiers = new Vector3(currentFloor, 0f, currentFloor);

            floorSize = mazeDimension - sizeModifiers;
            floorDimensions.Add(currentFloor, floorSize);

            for (int x = 0; x < floorSize.x; x++)
            {
                for (int z = 0; z < floorSize.z; z++)
                {
                    Cell newCel = new Cell();

                    for (int i = 0; i < newCel.positions.Length; i++) // Sets the positon for the ground and all walls
                    {
                        newCel.positions[i] = newCel.positions[i] + new Vector3(x, y, z);
                    }

                    cells.Add(newCel);
                    newCel.index = cells.IndexOf(newCel);

                    AssignCellNeighboursIndices(newCel, (int)floorSize.x, (int)floorSize.z, x);

                    newCel.OnCreation();
                    floorCellAmount++;
                }
            }

            // Add the amount of cells of a floor to a list, so cells can easily be found back
            cellAmountByFloor.Add(floorCellAmount);
            floorCellAmount = 0;
            currentFloor++;
        }

        AssignCellNeighbourCells();
        StartCoroutine(GenerateMaze());
    }

    private void AssignCellNeighboursIndices(Cell newCel, int floorSizeX, int floorSizeZ, int x)
    {
        // left neighbour
        if (newCel.index == floorSizeX * x)
            newCel.neighbourCellIndex[0] = -1;
        else
            newCel.neighbourCellIndex[0] = newCel.index - 1;

        // right neighbour
        if (newCel.index - ((int)mazeDimension.x * (x + 1) - 1) == 0)
            newCel.neighbourCellIndex[1] = -1;
        else
            newCel.neighbourCellIndex[1] = newCel.index + 1;

        // top neighbour
        if (newCel.index + (int)mazeDimension.x >= floorSizeX * floorSizeX)
            newCel.neighbourCellIndex[2] = -1;
        else
            newCel.neighbourCellIndex[2] = newCel.index + (int)mazeDimension.x;

        // bottom neighbour
        if (newCel.index - (int)mazeDimension.x < 0)
            newCel.neighbourCellIndex[3] = -1;
        else
            newCel.neighbourCellIndex[3] = newCel.index - (int)mazeDimension.x;
    }

    private void AssignCellNeighbourCells()
    {
        for (int i = 0; i < cells.Count; i++)
        {
            for (int j = 0; j < cells[i].neighbourCellIndex.Length; j++)
            {
                if (cells[i].neighbourCellIndex[j] != -1)
                {
                    cells[i].availableNeighbourCells.Add(cells[cells[i].neighbourCellIndex[j]]);
                }
            }
        }
    }

    private IEnumerator GenerateMaze()
    {
        int startCellIndex = 0;
        int visitedCellAmount = 0;

        for (int y = 0; y < mazeDimension.y; y++)
        {
            startCellIndex = Random.Range(0, cellAmountByFloor[y]) + visitedCellAmount;

            currentCell = cells[startCellIndex];
            currentCell.isStartCell = true;
            visitedCellAmount += cellAmountByFloor[y];

            while (true)
            {
                nextCell = currentCell.GetRandomCellNeighbour();
                if (nextCell != null && !nextCell.visited) // If there is a neighbouring cell that's unvisited
                {
                    nextCell.visited = true;

                    if (currentCell.unvisitedNeighbourCells > 0)
                        unvisitedCells.Add(currentCell);

                    RemoveWalls(currentCell, nextCell);
                    currentCell = nextCell;
                }
                else if (unvisitedCells.Count > 0)
                {
                    currentCell = unvisitedCells[unvisitedCells.Count - 1];
                    unvisitedCells.RemoveAt(unvisitedCells.Count - 1);
                }
                else // if every cell has been visited
                {
                    endCell = currentCell;
                    currentCell.isEndCell = true;
                    DrawMesh();
                    break;
                }
            }
            if (y == mazeDimension.y - 1)
                yield break;
        }
        yield return null;
    }

    // This function destroys the current maze.
    private void DestroyMaze()
    {

    }

    private void RemoveWalls(Cell a, Cell b)
    {
        Vector3 positionDifference = a.positions[0] - b.positions[0];

        if (positionDifference.x == 1)
        {
            a.dividers[2] = false;
            b.dividers[4] = false;
        }
        else if (positionDifference.x == -1)
        {
            a.dividers[4] = false;
            b.dividers[2] = false;
        }

        if (positionDifference.z == 1)
        {
            a.dividers[1] = false;
            b.dividers[3] = false;
        }
        else if (positionDifference.z == -1)
        {
            a.dividers[3] = false;
            b.dividers[1] = false;
        }
    }

    private List<Vector3> CreateVertices()
    {
        List<Vector3> newVertices = new List<Vector3>();

        for (int i = 0; i < cells.Count; i++)
        {
            for (int j = 0; j < cells[i].dividers.Length; j++)
            {
                if (cells[i].dividers[j])
                {
                    Vector3 dimensions = cells[i].dimensions[j];
                    for (int k = 1; k <= 3; k++) // Use 24 vertices instead of 8 on a cell so the lighting gets rendered correctly.
                    {
                        newVertices.Add(cells[i].positions[j]);
                        newVertices.Add(cells[i].positions[j] + new Vector3(1f * dimensions.x, 0f * dimensions.y, 0f * dimensions.z));
                        newVertices.Add(cells[i].positions[j] + new Vector3(1f * dimensions.x, 1f * dimensions.y, 0f * dimensions.z));
                        newVertices.Add(cells[i].positions[j] + new Vector3(0f * dimensions.x, 1f * dimensions.y, 0f * dimensions.z));

                        newVertices.Add(cells[i].positions[j] + new Vector3(0f * dimensions.x, 1f * dimensions.y, 1f * dimensions.z));
                        newVertices.Add(cells[i].positions[j] + new Vector3(1f * dimensions.x, 1f * dimensions.y, 1f * dimensions.z));
                        newVertices.Add(cells[i].positions[j] + new Vector3(1f * dimensions.x, 0f * dimensions.y, 1f * dimensions.z));
                        newVertices.Add(cells[i].positions[j] + new Vector3(0f * dimensions.x, 0f * dimensions.y, 1f * dimensions.z));
                    }
                }
            }
        }
        return newVertices;
    }

    private List<int> CreateTriangles()
    {
        List<int> newTrianglesCoordinates = new List<int>();

        int lastIndex = 0;
        for (int i = 0; i < cells.Count; i++)
        {
            for (int j = 0; j < cells[i].dividers.Length; j++)
            {
                if (cells[i].dividers[j])
                {
                    newTrianglesCoordinates.Add(lastIndex); // BottomLeftVertex
                    newTrianglesCoordinates.Add(lastIndex + 2); // TopLeftVertex
                    newTrianglesCoordinates.Add(lastIndex + 1); // BottomRightVertex
                    newTrianglesCoordinates.Add(lastIndex); // BottomRightVertex
                    newTrianglesCoordinates.Add(lastIndex + 3); // TopLeftVertex
                    newTrianglesCoordinates.Add(lastIndex + 2); // TopRightVertex

                    // Top Quad
                    newTrianglesCoordinates.Add(lastIndex + 10); // BottomLeftVertex
                    newTrianglesCoordinates.Add(lastIndex + 11); // TopLeftVertex
                    newTrianglesCoordinates.Add(lastIndex + 12); // BottomRightVertex
                    newTrianglesCoordinates.Add(lastIndex + 10); // BottomRightVertex
                    newTrianglesCoordinates.Add(lastIndex + 12); // TopLeftVertex
                    newTrianglesCoordinates.Add(lastIndex + 13); // TopRightVertex

                    // Right Quad
                    newTrianglesCoordinates.Add(lastIndex + 9); // BottomLeftVertex
                    newTrianglesCoordinates.Add(lastIndex + 18); // TopLeftVertex
                    newTrianglesCoordinates.Add(lastIndex + 5); // BottomRightVertex
                    newTrianglesCoordinates.Add(lastIndex + 9); // BottomRightVertex
                    newTrianglesCoordinates.Add(lastIndex + 5); // TopLeftVertex
                    newTrianglesCoordinates.Add(lastIndex + 6); // TopRightVertex

                    // Left Quad
                    newTrianglesCoordinates.Add(lastIndex + 8); // BottomLeftVertex
                    newTrianglesCoordinates.Add(lastIndex + 7); // TopLeftVertex
                    newTrianglesCoordinates.Add(lastIndex + 20); // BottomRightVertex
                    newTrianglesCoordinates.Add(lastIndex + 8); // BottomRightVertex
                    newTrianglesCoordinates.Add(lastIndex + 20); // TopLeftVertex
                    newTrianglesCoordinates.Add(lastIndex + 19); // TopRightVertex

                    // Back Quad
                    newTrianglesCoordinates.Add(lastIndex + 21); // BottomLeftVertex
                    newTrianglesCoordinates.Add(lastIndex + 20); // TopLeftVertex
                    newTrianglesCoordinates.Add(lastIndex + 15); // BottomRightVertex
                    newTrianglesCoordinates.Add(lastIndex + 21); // BottomRightVertex
                    newTrianglesCoordinates.Add(lastIndex + 15); // TopLeftVertex
                    newTrianglesCoordinates.Add(lastIndex + 14); // TopRightVertex

                    // Bottom Quad
                    newTrianglesCoordinates.Add(lastIndex + 16); // BottomLeftVertex
                    newTrianglesCoordinates.Add(lastIndex + 22); // TopLeftVertex
                    newTrianglesCoordinates.Add(lastIndex + 23); // BottomRightVertex
                    newTrianglesCoordinates.Add(lastIndex + 16); // BottomRightVertex
                    newTrianglesCoordinates.Add(lastIndex + 17); // TopLeftVertex
                    newTrianglesCoordinates.Add(lastIndex + 22); // TopRightVertex

                    lastIndex += 24;
                }
            }
        }
        return newTrianglesCoordinates;
    }

    // Draw a maze mesh using the informations from the individual cells
    private void DrawMesh()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        List<Vector3> groundVertices = CreateVertices();
        mesh.vertices = groundVertices.ToArray();

        List<int> triangles = CreateTriangles();
        mesh.triangles = triangles.ToArray();

        mesh.Optimize();
        mesh.RecalculateNormals();
    }
}