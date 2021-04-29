using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{

    [Header("Mesh Settings")]
    [Range(64,256)]
    public int mDim = 256;
    public float mScale = 10f;
    [Range(0,15)]
    public float hScale = 10f;
    public Material material;
    [Range(0,20)]
    public float rotSpeed = 0.2f;

    [Header("Erosion settings")]
    public int noCycles = 50;


    Vector3[] verts;
    int[] triangles;

    float[] hMap;


    Mesh mesh;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;

    Transform mTerrain;


    // Start is called before the first frame update
    void Start()
    {
        QualitySettings.vSyncCount = 1;

        GenerateTerrain();
    }

    // Update is called once per frame
    void Update()
    {
        GenerateTerrain();
    }

    void GenerateTerrain()
    {
        hMap = FindObjectOfType<HeightmapGenerator>().Generate(mDim);
        GenerateMesh();

        /* Rotate the object around its Y axis */
        mTerrain.transform.Rotate(0, rotSpeed * Time.deltaTime, 0);
    }

    /* Generate vertices between (-1, 1) along x- and z-axis and add the
     * y-component from a given heightmap made out of perling noise. */
    void GenerateMesh()
    {
        verts = new Vector3[(mDim + 1) * (mDim + 1)];
        triangles = new int[mDim * mDim * 6];

        int itT = 0;
        int itV = 0;

        for (int i = 0, z = 0; z <= mDim; z++)
        {
            for (int x = 0; x <= mDim; x++)
            {
                /* Introduce vertices between the end vertices */
                Vector3 pctPoint = new Vector3(x / (mDim - 1f), 0, z / (mDim - 1f));
                
                /* Determine the position for the new vertex */
                Vector3 pointPosition = new Vector3((pctPoint.x * 2) - 1, 0, (pctPoint.z * 2) - 1) * mScale;

                /* Add the height component to the vertex  */
                pointPosition.y += hMap[i] * hScale;

                /* Add the vertex to the vertex-array */
                verts[i] = pointPosition;
                
                i++;

                /* Triangles */
                if (x < mDim && z < mDim)
                {
                    /* x-----x 
                     * | \   |
                     * |  \  |
                     * |   \ |
                     * x-----x */
                    triangles[itT + 0] = itV + 0;
                    triangles[itT + 1] = itV + mDim + 1;
                    triangles[itT + 2] = itV + 1;
                    triangles[itT + 3] = itV + 1;
                    triangles[itT + 4] = itV + mDim + 1;
                    triangles[itT + 5] = itV + mDim + 2;

                    itV += 1;
                    itT += 6;
                }
            }
            itV += 1;
        }

        if (mesh == null)
        {
            mesh = new Mesh();
            mesh.name = "Terrain Mesh";
        }
        else
        {
            mesh.Clear();
        }

        /* Allows for generating a mesh large enough to store all vertices */
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        mesh.vertices = verts;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        AssignMeshChild();
        meshFilter.sharedMesh = mesh;
        meshRenderer.sharedMaterial = material;
    }

    /* Generates a child object with a shared meshFilter and
     * MeshRenderer component from func GenerateMesh() if it
     * does not exist. CREDIT to Sebaistian Lague: https://bit.ly/20os8QF */
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
