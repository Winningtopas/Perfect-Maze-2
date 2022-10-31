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
                    if (x != 3 || y == 3)
                    {
                        groundCells.Add(newCel);
                        newCel.ground = true;
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

    // Draw a maze mesh using the informations from the individual cells
    private void DrawMesh()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        List<Vector3> groundVertices = new List<Vector3>();

        for (int i = 0; i < cells.Count; i++)
        {
            if (cells[i].ground)
            {
                Vector3 dimensions = cells[i].groundDimension;
                for (int j = 1; j <= 3; j++) // Use 24 vertices instead of 8 on a cell so the lighting gets rendered correctly.
                {
                    groundVertices.Add(cells[i].groundPosition);
                    groundVertices.Add(cells[i].groundPosition + new Vector3(1f * dimensions.x, 0f * dimensions.y, 0f * dimensions.z));
                    groundVertices.Add(cells[i].groundPosition + new Vector3(1f * dimensions.x, 1f * dimensions.y, 0f * dimensions.z));
                    groundVertices.Add(cells[i].groundPosition + new Vector3(0f * dimensions.x, 1f * dimensions.y, 0f * dimensions.z));

                    groundVertices.Add(cells[i].groundPosition + new Vector3(0f * dimensions.x, 1f * dimensions.y, 1f * dimensions.z));
                    groundVertices.Add(cells[i].groundPosition + new Vector3(1f * dimensions.x, 1f * dimensions.y, 1f * dimensions.z));
                    groundVertices.Add(cells[i].groundPosition + new Vector3(1f * dimensions.x, 0f * dimensions.y, 1f * dimensions.z));
                    groundVertices.Add(cells[i].groundPosition + new Vector3(0f * dimensions.x, 0f * dimensions.y, 1f * dimensions.z));
                }
            }
        }

        mesh.vertices = groundVertices.ToArray();

        List<int> groundTriangles = new List<int>();

        int compensate = 0; // This value is used to compensate for empty vertices. This allows us to have empty cells.

        for (int i = 0; i < cells.Count; i++)
        {
            if (cells[i].ground)
            {
                // Front Quad
                groundTriangles.Add((i - compensate) * 24); // BottomLeftVertex
                groundTriangles.Add((i - compensate) * 24 + 2); // TopLeftVertex
                groundTriangles.Add((i - compensate) * 24 + 1); // BottomRightVertex
                groundTriangles.Add((i - compensate) * 24); // BottomRightVertex
                groundTriangles.Add((i - compensate) * 24 + 3); // TopLeftVertex
                groundTriangles.Add((i - compensate) * 24 + 2); // TopRightVertex

                // Top Quad
                groundTriangles.Add((i - compensate) * 24 + 10); // BottomLeftVertex
                groundTriangles.Add((i - compensate) * 24 + 11); // TopLeftVertex
                groundTriangles.Add((i - compensate) * 24 + 12); // BottomRightVertex
                groundTriangles.Add((i - compensate) * 24 + 10); // BottomRightVertex
                groundTriangles.Add((i - compensate) * 24 + 12); // TopLeftVertex
                groundTriangles.Add((i - compensate) * 24 + 13); // TopRightVertex

                // Right Quad
                groundTriangles.Add((i - compensate) * 24 + 9); // BottomLeftVertex
                groundTriangles.Add((i - compensate) * 24 + 18); // TopLeftVertex
                groundTriangles.Add((i - compensate) * 24 + 5); // BottomRightVertex
                groundTriangles.Add((i - compensate) * 24 + 9); // BottomRightVertex
                groundTriangles.Add((i - compensate) * 24 + 5); // TopLeftVertex
                groundTriangles.Add((i - compensate) * 24 + 6); // TopRightVertex

                // Left Quad
                groundTriangles.Add((i - compensate) * 24 + 8); // BottomLeftVertex
                groundTriangles.Add((i - compensate) * 24 + 7); // TopLeftVertex
                groundTriangles.Add((i - compensate) * 24 + 20); // BottomRightVertex
                groundTriangles.Add((i - compensate) * 24 + 8); // BottomRightVertex
                groundTriangles.Add((i - compensate) * 24 + 20); // TopLeftVertex
                groundTriangles.Add((i - compensate) * 24 + 19); // TopRightVertex

                // Back Quad
                groundTriangles.Add((i - compensate) * 24 + 21); // BottomLeftVertex
                groundTriangles.Add((i - compensate) * 24 + 20); // TopLeftVertex
                groundTriangles.Add((i - compensate) * 24 + 15); // BottomRightVertex
                groundTriangles.Add((i - compensate) * 24 + 21); // BottomRightVertex
                groundTriangles.Add((i - compensate) * 24 + 15); // TopLeftVertex
                groundTriangles.Add((i - compensate) * 24 + 14); // TopRightVertex

                // Bottom Quad
                groundTriangles.Add((i - compensate) * 24 + 16); // BottomLeftVertex
                groundTriangles.Add((i - compensate) * 24 + 22); // TopLeftVertex
                groundTriangles.Add((i - compensate) * 24 + 23); // BottomRightVertex
                groundTriangles.Add((i - compensate) * 24 + 16); // BottomRightVertex
                groundTriangles.Add((i - compensate) * 24 + 17); // TopLeftVertex
                groundTriangles.Add((i - compensate) * 24 + 22); // TopRightVertex
            }
            else
                compensate++;
        }

        mesh.triangles = groundTriangles.ToArray();
        mesh.Optimize();
        mesh.RecalculateNormals();
    }
}
