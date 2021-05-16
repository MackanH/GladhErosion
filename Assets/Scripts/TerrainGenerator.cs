using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{

    [Header("Mesh Settings")]
    private int m_dimension = 256;
    [SerializeField]
    private int m_scale = 10;
    [SerializeField][Range(0f, 5f)]
    private float h_scale = 3;
    [SerializeField][Range(0f, 20f)]
    private float rot_speed = 0.2f;

    [Header("Erosion")]
    public int num_iterations = 150000;

    private Vector3[] verts;
    private int[] triangles;

    public float[,] height_map;

    bool animateErosion = false;


    Mesh mesh;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;

    Transform mTerrain;

    public Material material;

    void Start()
    {
        //Application.targetFrameRate = 60;

        GenerateTerrain();
    }

    void Update()
    {
        /* Rotate the object around its Y axis for fun */
        mTerrain.transform.Rotate(0, rot_speed * Time.deltaTime, 0);
    }

    /* Creates a new height map and adds it to the terrains Y component */
    public void GenerateTerrain()
    {
        height_map = FindObjectOfType<HeightmapGenerator>().Generate(m_dimension);
        GenerateMesh();
    }

    public void ErodeTerrain()
    {
        FindObjectOfType<Erosion>().Erode(height_map, m_dimension, num_iterations);
        GenerateMesh();
    }

    /* Generate vertices between (-1, 1) along x- and z-axis and add the
     * y-component from a given heightmap made out of perling noise. */
    void GenerateMesh()
    {
        verts = new Vector3[(m_dimension) * (m_dimension)];
        triangles = new int[(m_dimension- 1) * (m_dimension-1) * 6];

        int itT = 0;

        for (int z = 0; z < m_dimension; z++)
        {
            for (int x = 0; x < m_dimension; x++)
            {
                int itV = z * m_dimension + x;
                /* Introduce vertices between the end vertices */
                Vector3 pctPoint = new Vector3(x / (m_dimension - 1f), 0, z / (m_dimension - 1f));
                
                /* Determine the position for the new vertex */
                Vector3 pointPosition = new Vector3((pctPoint.x * 2) - 1, 0, (pctPoint.z * 2) - 1) * m_scale;

                /* Add the height component to the vertex  */
                pointPosition.y += height_map[x, z] * h_scale;

                /* Add the vertex to the vertex-array */
                verts[itV] = pointPosition;

                /* Triangles */
                if (x != m_dimension - 1 && z != m_dimension - 1)
                {
                    /* x-----x 
                     * | \   |
                     * |  \  |
                     * |   \ |
                     * x-----x */
                    triangles[itT + 0] = itV;
                    triangles[itT + 1] = itV + m_dimension;
                    triangles[itT + 2] = itV + 1;

                    triangles[itT + 3] = itV + 1;
                    triangles[itT + 4] = itV + m_dimension;
                    triangles[itT + 5] = itV + m_dimension + 1;

                    itT += 6;
                }
            }
        }

        if (mesh == null)
        {
            mesh = new Mesh
            {
                name = "Terrain Mesh"
            };
        }
        else
        {
            mesh.Clear();
        }

        /* Allows for generating a mesh large enough to store all vertices */
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        /* Push the vertices and traiangles to a mesh component */
        mesh.vertices = verts;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        /* Assign objects mesh component to all child objects which contains
         * the mesh renderer and filter component (1 in this case) */
        AssignMeshChild();
        meshFilter.sharedMesh = mesh;
        meshRenderer.sharedMaterial = material;
    }

    /* Generates a child object with a shared meshFilter and MeshRenderer
     * component from func GenerateMesh() if it does not exist. */
    void AssignMeshChild()
    {
         mTerrain = transform.Find("mMesh");

        if(mTerrain == null)
        {
            mTerrain = new GameObject("mMesh").transform;
            mTerrain.transform.parent = transform;
            mTerrain.localPosition = Vector3.zero;
            mTerrain.rotation = Quaternion.identity;
        }

        if(!mTerrain.gameObject.GetComponent<MeshFilter>())
        {
            mTerrain.gameObject.AddComponent<MeshFilter>();
        }

        if(!mTerrain.gameObject.GetComponent<MeshRenderer>())
        {
            mTerrain.gameObject.AddComponent<MeshRenderer>();
        }

        meshFilter = mTerrain.gameObject.GetComponent<MeshFilter>();
        meshRenderer = mTerrain.gameObject.GetComponent<MeshRenderer>();
    }
}
