using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMaze : MonoBehaviour
{
    // Maze information
    [SerializeField]
    private int mazeSizeX, mazeSizeY, mazeSizeZ;

    // Cell information
    [SerializeField]
    private GameObject cellPrefab;

    [SerializeField]
    private List<Cell> cells = new List<Cell>();
    private List<Cell> groundCells = new List<Cell>();

    // Start is called before the first frame update
    void Start()
    {
        SpawnMaze();
        DrawMesh();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // This functtion starts the generation of a new maze, the function can be called by a UI button.
    public void SpawnMaze()
    {
        DestroyMaze();
        GameObject Floor;

        for (int y = 0; y < mazeSizeY; y++)
        {
            Floor = new GameObject();
            Floor.name = "Floor" + y;
            Floor.transform.SetParent(transform);

            for (int x = 0; x < mazeSizeX; x++)
            {
                for (int z = 0; z < mazeSizeZ; z++)
                {
                    //GameObject Cell = Instantiate(cellPrefab, new Vector3(x, y, z), transform.rotation, Floor.transform);
                    Cell newCel = new Cell();
                    newCel.groundPosition = new Vector3(x, y, z);

                    for (int i = 0; i < newCel.positions.Length; i++)
                    {
                        newCel.positions[i] = newCel.positions[i] + new Vector3(x, y, z);
                    }

                    // The dividers are the ground and walls of a maze cell
                    newCel.dividers[0] = true;
                    newCel.dividers[1] = true;
                    newCel.dividers[2] = true;
                    newCel.dividers[3] = true;
                    newCel.dividers[4] = true;
                    if (x > 100 && x < 200)
                    {
                        newCel.dividers[0] = false;
                        newCel.dividers[1] = false;
                        newCel.dividers[2] = false;
                        newCel.dividers[3] = false;
                        newCel.dividers[4] = false;
                    }


                    cells.Add(newCel);
                }
            }
        }
    }

    // This function destroys the current maze.
    private void DestroyMaze()
    {

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
