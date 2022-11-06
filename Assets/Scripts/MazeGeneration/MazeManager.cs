using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeManager : MonoBehaviour
{
    // Maze information
    public static List<Cell> cells = new List<Cell>();
    public List<Cell> unvisitedCells = new List<Cell>();

    [SerializeField]
    private Vector3 mazeDimension;
    private int currentFloor;
    private List<int> cellAmountByFloor = new List<int>();
    private Cell currentCell, nextCell, endCell;
    private bool isGenerating;

    // Start/End Cells

    private List<int> startCellTriangles = new List<int>();

    // Different generation modes
    [SerializeField]
    private bool isPyramid;

    // User interface
    [SerializeField]
    private AdjustSliderValue mazeWidthSliderText, mazeHeightSliderText, mazeLengthSliderText, playerSpeedSliderText;

    // Camera information
    [SerializeField]
    private Transform topDownCamera, frontCamera, firstPersonCamera;
    private Vector3 firstPersonCameraOffset = new Vector3(.5f, .5f, .5f);
    private Vector3 firstPersonCameraStartPosition;
    private List<Vector3> firstPersonCameraPositions = new List<Vector3>();
    private bool reachedMazeEnd;
    private int playerSpeed = 5;

    // Animations

    [SerializeField]
    private Animator[] curtainAnimators;
    private float curtainOpenTime;

    // Start is called before the first frame update
    void Start()
    {
        // All the animators have the same animations, so I only need to check one animator for the clip lengths
        AnimationClip[] clips = curtainAnimators[0].runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            switch (clip.name)
            {
                case "curtainsOpen":
                    curtainOpenTime = clip.length;
                    break;
            }
        }
    }

    public void SpawnMaze()
    {
        if (!isGenerating)
        {
            StopAllCoroutines();
            StartCoroutine(ResetMaze());
        }
        isGenerating = true;
    }

    public IEnumerator ResetMaze()
    {
        currentFloor = 0;

        curtainAnimators[0].Play("curtainsClose");
        curtainAnimators[1].Play("curtainsClose");
        curtainAnimators[2].Play("curtainsBigClose");

        // the open and close animation have the same time, since it's the same animation but reversed
        yield return new WaitForSeconds(curtainOpenTime);

        DestroyMaze();
        if (isPyramid)
        {
            mazeWidthSliderText.sliderValue = 10;
            mazeHeightSliderText.sliderValue = 10;
            mazeLengthSliderText.sliderValue = 10;
        }
        mazeDimension.x = mazeWidthSliderText.sliderValue;
        mazeDimension.y = mazeHeightSliderText.sliderValue;
        mazeDimension.z = mazeLengthSliderText.sliderValue;
        playerSpeed = playerSpeedSliderText.sliderValue;

        SpawnFloorGrid();
        PositionCameras();
    }
    public void PyramidToggle()
    {
        isPyramid = !isPyramid;
    }

    // This function starts the generation of a new maze, the function can be called by a UI button.
    private void SpawnFloorGrid()
    {
        int floorCellAmount = 0;
        Vector3 sizeModifiers = Vector3.zero;
        Vector3 floorSize = mazeDimension;

        // Give the pyramid a height depending on the base
        if (isPyramid)
        {
            if (mazeDimension.x < mazeDimension.z)
                mazeDimension.y = mazeDimension.x;
            else
                mazeDimension.y = mazeDimension.z;
        }

        for (int i = 0; i < floorSize.y; i++)
        {
            if (isPyramid)
                sizeModifiers = new Vector3(i, 0f, i);
            floorSize = mazeDimension - sizeModifiers;
        }

        sizeModifiers = Vector3.zero;
        floorSize = mazeDimension;

        for (int y = 0; y < floorSize.y; y++)
        {
            if (isPyramid)
                sizeModifiers = new Vector3(currentFloor, 0f, currentFloor);

            floorSize = mazeDimension - sizeModifiers;

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

                    AssignCellNeighboursIndices(newCel, (int)floorSize.x, (int)floorSize.z, x, z, y);

                    newCel.OnCreation(x, z, y);
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

    private void AssignCellNeighboursIndices(Cell newCel, int floorSizeX, int floorSizeZ, int x, int z, int y)
    {
        int previousFloorCellAmount = 0;
        for (int i = y; i > 0; i--)
        {
            if (y > 0)
                previousFloorCellAmount += cellAmountByFloor[y - i];
        }

        // left neighbour
        if (newCel.index == floorSizeZ * x + previousFloorCellAmount)
            newCel.neighbourCellIndex[0] = -1;
        else
            newCel.neighbourCellIndex[0] = newCel.index - 1;

        // right neighbour
        if (newCel.index == floorSizeZ * (x + 1) - 1 + previousFloorCellAmount || newCel.index + 1 + previousFloorCellAmount == floorSizeX * floorSizeZ * (int)mazeDimension.y)
            newCel.neighbourCellIndex[1] = -1;
        else
            newCel.neighbourCellIndex[1] = newCel.index + 1;

        // top neighbour
        if (newCel.index >= previousFloorCellAmount + floorSizeZ * floorSizeX - floorSizeZ)
            newCel.neighbourCellIndex[2] = -1;
        else
            newCel.neighbourCellIndex[2] = newCel.index + floorSizeZ;

        // bottom neighbour
        if (newCel.index - floorSizeZ - previousFloorCellAmount < 0)
            newCel.neighbourCellIndex[3] = -1;
        else
            newCel.neighbourCellIndex[3] = newCel.index - floorSizeZ;

        // above neighbour
        if (isPyramid)
        {
            if ((newCel.index - previousFloorCellAmount + 1) % (mazeDimension.y - y) == 0 || newCel.index >= Mathf.Ceil((floorSizeX - 1) * (floorSizeZ - 1) + previousFloorCellAmount + mazeDimension.y - (y + 1)))
                newCel.neighbourCellIndex[4] = -1;
            else
                newCel.neighbourCellIndex[4] = newCel.index + floorSizeX * floorSizeZ - x;
        }
        else
        {
            if (newCel.index + floorSizeX * floorSizeZ >= floorSizeX * floorSizeZ * mazeDimension.y)
                newCel.neighbourCellIndex[4] = -1;
            else
                newCel.neighbourCellIndex[4] = newCel.index + floorSizeX * floorSizeZ;
        }
    }

    private void AssignCellNeighbourCells()
    {
        for (int i = 0; i < cells.Count; i++)
        {
            for (int j = 0; j < 4; j++) // Only do this for the left, right, top and bottom neighbour, not the above neighbour
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
        bool generateEndCell = false;
        bool generatedFloorEndCell = false;

        for (int y = 0; y < mazeDimension.y; y++)
        {
            //startCellIndex = Random.Range(0, cellAmountByFloor[y]) + visitedCellAmount; // A random cell on the floor

            if (y == 0)
                startCellIndex = Random.Range(0, (int)mazeDimension.x); // Makes an entrance on the side of the maze
            else
                startCellIndex = endCell.neighbourCellIndex[4]; // Make the cell above the endcell the startcell of the next floor

            currentCell = cells[startCellIndex];
            currentCell.isStartCell = true;
            currentCell.visited = true; // Set the first cell on visited, otherwise the maze can become one big loop

            if (y == 0)
                firstPersonCameraStartPosition = currentCell.positions[0] + firstPersonCameraOffset;

            firstPersonCameraPositions.Add(currentCell.positions[0] + firstPersonCameraOffset);

            visitedCellAmount += cellAmountByFloor[y];

            while (true)
            {
                nextCell = currentCell.GetRandomCellNeighbour();

                if (nextCell != null && !nextCell.visited) // If there is a neighbouring cell that's unvisited
                {
                    if (!generatedFloorEndCell)// Only add camera positions when the endcell hasn't been generated yet
                        firstPersonCameraPositions.Add(nextCell.positions[0] + firstPersonCameraOffset);

                    nextCell.visited = true;

                    if (currentCell.unvisitedNeighbourCells > 0)
                        unvisitedCells.Add(currentCell);

                    RemoveWalls(currentCell, nextCell);

                    currentCell = nextCell;
                }
                else if (unvisitedCells.Count > 0)
                {
                    generateEndCell = true;
                    if (generateEndCell && !generatedFloorEndCell)
                    {
                        generatedFloorEndCell = true;
                        endCell = currentCell;
                        currentCell.isEndCell = true;
                    }

                    currentCell = unvisitedCells[unvisitedCells.Count - 1];
                    unvisitedCells.RemoveAt(unvisitedCells.Count - 1);
                }
                else // If every cell has been visited
                {
                    generateEndCell = false;
                    generatedFloorEndCell = false;

                    // Draw the mesh if all the floors have gotten a maze layout
                    if (y == mazeDimension.y - 1)
                        StartCoroutine(FinishMazeGeneration());

                    break;
                }
            }
            if (y == mazeDimension.y - 1)
                yield break;
        }
        yield return null;
    }

    private void DestroyMaze()
    {
        cells.Clear();
        startCellTriangles.Clear();
        firstPersonCameraPositions.Clear();

        GetComponent<MeshFilter>().mesh = null;
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
        int number = 0;
        for (int i = 0; i < cells.Count; i++)
        {
            for (int j = 0; j < cells[i].dividers.Length; j++)
            {
                if (cells[i].dividers[j])
                {
                    if (cells[i].isStartCell && j == 0 && cells[i].positions[0].y != 0) { } // if the startcell is not on the ground, leave it empty so the fp camera can move through it.
                    else
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
                            number += 8;
                        }
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
                    if (cells[i].isStartCell && j == 0 && cells[i].positions[0].y != 0) // if the startcell is not on the ground, leave it empty so the fp camera can move through it.
                    { }
                    else
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
        }
        return newTrianglesCoordinates;
    }


    private Vector2[] CreateUVs(Vector3[] vertices)
    {
        Vector2[] newUvs = new Vector2[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            newUvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

        return newUvs;
    }

    // Draw a maze mesh using the informations from the individual cells
    private void DrawMesh()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        List<Vector3> verticesList = CreateVertices();
        Vector3[] vertices = verticesList.ToArray();
        mesh.vertices = vertices;

        List<int> trianglesList = CreateTriangles();
        int[] triangles = trianglesList.ToArray();
        mesh.triangles = triangles;

        Vector2[] uvs = CreateUVs(vertices);
        mesh.uv = uvs;

        mesh.subMeshCount = 2; //mesh counter
        mesh.SetTriangles(triangles, 0);

        mesh.Optimize();
        mesh.RecalculateNormals();
    }

    private void PositionCameras()
    {
        float x = mazeDimension.x / 2f;
        float y = mazeDimension.y / 2f;
        float z = mazeDimension.z / 2f;
        float largestBetweenXAndZ = mazeDimension.x;
        float largestBetweenZAndY = mazeDimension.y;

        if (x > z)
            largestBetweenXAndZ = mazeDimension.x;
        else
            largestBetweenXAndZ = mazeDimension.z;

        if (z > y)
            largestBetweenZAndY = mazeDimension.z;
        else
            largestBetweenZAndY = mazeDimension.y;

        topDownCamera.transform.position = new Vector3(x, mazeDimension.y + largestBetweenXAndZ, z);
        frontCamera.transform.position = new Vector3(mazeDimension.x + largestBetweenZAndY, y, z);
        firstPersonCamera.transform.position = firstPersonCameraStartPosition;
    }

    private IEnumerator FinishMazeGeneration()
    {
        DrawMesh();

        curtainAnimators[0].Play("curtainsOpen");
        curtainAnimators[1].Play("curtainsOpen");
        curtainAnimators[2].Play("curtainsBigOpen");

        yield return new WaitForSeconds(curtainOpenTime);

        isGenerating = false;
        StartCoroutine(PositionFirstPersonCamera());
    }

    private IEnumerator PositionFirstPersonCamera()
    {
        float positionTime = 1.1f - playerSpeed * 0.1f;
        float rotateTime = .15f;
        Vector3 startPosition = Vector3.zero;
        Vector3 lookDirection = Vector3.zero;

        Quaternion startLookOrientation = new Quaternion();
        Quaternion lookOrientation = new Quaternion();

        for (int i = 0; i < firstPersonCameraPositions.Count; i++)
        {
            startPosition = firstPersonCamera.position;

            float t = 0f;
            while (t < positionTime)
            {
                firstPersonCamera.position = Vector3.Lerp(startPosition, firstPersonCameraPositions[i], t / positionTime);
                t += Time.deltaTime;
                yield return null;
                firstPersonCamera.position = firstPersonCameraPositions[i];
            }
            if (t >= positionTime)
            {
                if (i + 1 < firstPersonCameraPositions.Count)
                    lookDirection = firstPersonCameraPositions[i + 1] - firstPersonCamera.position;
                else
                    lookOrientation = startLookOrientation;

                lookOrientation = Quaternion.LookRotation(lookDirection);

                if (lookOrientation != startLookOrientation)
                {
                    StartCoroutine(RotateFirstPersonCamera(lookOrientation, rotateTime));
                    yield return new WaitForSeconds(rotateTime);
                }
                startLookOrientation = lookOrientation;
            }
        }
        reachedMazeEnd = true;
    }

    IEnumerator RotateFirstPersonCamera(Quaternion endValue, float duration)
    {
        float time = 0;
        Quaternion startValue = firstPersonCamera.rotation;

        while (time < duration)
        {
            firstPersonCamera.rotation = Quaternion.Lerp(startValue, endValue, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        firstPersonCamera.rotation = endValue;
    }
}
