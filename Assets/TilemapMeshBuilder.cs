using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering;

[ExecuteAlways]
[RequireComponent(typeof(Tilemap))]
public class TilemapMeshBuilder : MonoBehaviour
{
    [Header("Build")]
    [SerializeField] private bool generateOnEnable = true;
    [SerializeField] private bool regenerateOnValidate = false;
    [SerializeField] private string childName = "NavMeshSourceMesh";

    [Header("Components")]
    [SerializeField] private bool createMeshRenderer = true;
    [SerializeField] private bool createMeshCollider = true;

    [Header("Renderer")]
    [SerializeField] private bool rendererVisible = true;
    [SerializeField] private Material debugMaterial;

    [Header("Surface")]
    [SerializeField] private float worldYOffset = 0.01f;

    [Header("Tile Filter")]
    [SerializeField] private bool useTileFilter = true;
    [SerializeField] private List<TileBase> allowedTiles = new List<TileBase>();
    [SerializeField] private bool clearListBeforeCollect = true;

    [Header("Options")]
    [SerializeField] private bool skipEmptyBoundsLog = false;

    private Tilemap tilemap;
    private GameObject meshChild;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;
    private Mesh generatedMesh;

    private struct CellRect
    {
        public int xMin;
        public int yMin;
        public int width;
        public int height;

        public int xMax => xMin + width;
        public int yMax => yMin + height;

        public CellRect(int xMin, int yMin, int width, int height)
        {
            this.xMin = xMin;
            this.yMin = yMin;
            this.width = width;
            this.height = height;
        }
    }
    private void Awake()
    {
        CacheRefs();
        EnsureChild();
    }

    private void OnEnable()
    {
        CacheRefs();
        EnsureChild();

        if (generateOnEnable)
            GenerateMesh();
    }

    private void CacheRefs()
    {
        if (tilemap == null)
            tilemap = GetComponent<Tilemap>();
    }

    [ContextMenu("Generate Mesh")]
    public void GenerateMesh()
    {
        CacheRefs();

        if (tilemap == null)
        {
            Debug.LogError("[TilemapNavMeshMeshBuilder] Tilemap not found.", this);
            return;
        }

        EnsureChild();

        BoundsInt bounds = tilemap.cellBounds;

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        int vi = 0;
        int includedTileCount = 0;
        int rectCount = 0;

        bool[,] walkable = BuildWalkableMap(bounds);


        List<CellRect> rects = BuildMergedRects(walkable, bounds);
        rectCount = rects.Count;

        foreach (CellRect rect in rects)
        {
            includedTileCount += rect.width * rect.height;
            AppendQuadFromRect(rect, vertices, triangles, ref vi);
        }

        if (generatedMesh == null)
        {
            generatedMesh = new Mesh
            {
                name = $"{name}_NavMeshMesh"
            };
        }
        else
        {
            generatedMesh.Clear();
        }

        if (vertices.Count == 0)
        {
            meshFilter.sharedMesh = null;

            if (meshCollider != null)
                meshCollider.sharedMesh = null;

            if (!skipEmptyBoundsLog)
                Debug.LogWarning("[TilemapNavMeshMeshBuilder] No allowed tiles found to build mesh.", this);

            return;
        }

        generatedMesh.SetVertices(vertices);
        generatedMesh.SetTriangles(triangles, 0);
        generatedMesh.RecalculateNormals();
        generatedMesh.RecalculateBounds();

        meshFilter.sharedMesh = generatedMesh;

        if (createMeshCollider && meshCollider != null)
        {
            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = generatedMesh;
        }

        ApplyRendererState();

        Debug.Log(
            $"[TilemapNavMeshMeshBuilder] Mesh generated. IncludedTiles={includedTileCount}, Quads={rectCount}, Vertices={generatedMesh.vertexCount}, Tris={generatedMesh.triangles.Length / 3}",
            this
        );
    }
    private bool IsWalkableCell(Vector3Int cell)
    {
        TileBase tile = tilemap.GetTile(cell);

        if (tile == null)
            return false;

        if (!useTileFilter)
            return true;

        return allowedTiles.Contains(tile);
    }
    private bool[,] BuildWalkableMap(BoundsInt bounds)
    {
        int width = bounds.size.x;
        int height = bounds.size.y;

        bool[,] walkable = new bool[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int cell = new Vector3Int(bounds.xMin + x, bounds.yMin + y, 0);
                walkable[x, y] = IsWalkableCell(cell);
            }
        }

        return walkable;
    }
    private void AppendQuadFromRect(CellRect rect, List<Vector3> vertices, List<int> triangles, ref int vi)
    {
        Vector3Int c00 = new Vector3Int(rect.xMin, rect.yMin, 0);
        Vector3Int c10 = new Vector3Int(rect.xMax, rect.yMin, 0);
        Vector3Int c11 = new Vector3Int(rect.xMax, rect.yMax, 0);
        Vector3Int c01 = new Vector3Int(rect.xMin, rect.yMax, 0);

        Vector3 w0 = tilemap.CellToWorld(c00);
        Vector3 w1 = tilemap.CellToWorld(c10);
        Vector3 w2 = tilemap.CellToWorld(c11);
        Vector3 w3 = tilemap.CellToWorld(c01);

        Vector3 yOffset = Vector3.up * worldYOffset;
        w0 += yOffset;
        w1 += yOffset;
        w2 += yOffset;
        w3 += yOffset;

        Vector3 l0 = meshChild.transform.InverseTransformPoint(w0);
        Vector3 l1 = meshChild.transform.InverseTransformPoint(w1);
        Vector3 l2 = meshChild.transform.InverseTransformPoint(w2);
        Vector3 l3 = meshChild.transform.InverseTransformPoint(w3);

        vertices.Add(l0);
        vertices.Add(l1);
        vertices.Add(l2);
        vertices.Add(l3);

        Vector3 n = Vector3.Cross(w1 - w0, w2 - w0);

        if (Vector3.Dot(n, Vector3.up) >= 0f)
        {
            triangles.Add(vi + 0);
            triangles.Add(vi + 1);
            triangles.Add(vi + 2);

            triangles.Add(vi + 0);
            triangles.Add(vi + 2);
            triangles.Add(vi + 3);
        }
        else
        {
            triangles.Add(vi + 0);
            triangles.Add(vi + 2);
            triangles.Add(vi + 1);

            triangles.Add(vi + 0);
            triangles.Add(vi + 3);
            triangles.Add(vi + 2);
        }

        vi += 4;
    }
    private List<CellRect> BuildMergedRects(bool[,] walkable, BoundsInt bounds)
    {
        int width = bounds.size.x;
        int height = bounds.size.y;

        bool[,] visited = new bool[width, height];
        List<CellRect> rects = new List<CellRect>();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (!walkable[x, y] || visited[x, y])
                    continue;

                int rectWidth = 1;
                while (x + rectWidth < width && walkable[x + rectWidth, y] && !visited[x + rectWidth, y])
                {
                    rectWidth++;
                }

                int rectHeight = 1;
                bool canExtend = true;

                while (y + rectHeight < height && canExtend)
                {
                    for (int checkX = x; checkX < x + rectWidth; checkX++)
                    {
                        if (!walkable[checkX, y + rectHeight] || visited[checkX, y + rectHeight])
                        {
                            canExtend = false;
                            break;
                        }
                    }

                    if (canExtend)
                        rectHeight++;
                }

                for (int markY = y; markY < y + rectHeight; markY++)
                {
                    for (int markX = x; markX < x + rectWidth; markX++)
                    {
                        visited[markX, markY] = true;
                    }
                }

                rects.Add(new CellRect(
                    bounds.xMin + x,
                    bounds.yMin + y,
                    rectWidth,
                    rectHeight
                ));
            }
        }

        return rects;
    }

    [ContextMenu("Clear Mesh")]
    public void ClearMesh()
    {
        if (meshFilter != null)
            meshFilter.sharedMesh = null;

        if (meshCollider != null)
            meshCollider.sharedMesh = null;

        if (generatedMesh != null)
        {
            if (Application.isPlaying)
                Destroy(generatedMesh);
            else
                DestroyImmediate(generatedMesh);

            generatedMesh = null;
        }
    }
    [ContextMenu("Collect Used Tiles")]
    public void CollectUsedTiles()
    {
        CacheRefs();

        if (tilemap == null)
        {
            Debug.LogError("[TilemapNavMeshMeshBuilder] Tilemap not found.", this);
            return;
        }

        if (clearListBeforeCollect)
            allowedTiles.Clear();

        HashSet<TileBase> uniqueTiles = new HashSet<TileBase>(allowedTiles);
        BoundsInt bounds = tilemap.cellBounds;

        int addedCount = 0;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);
                TileBase tile = tilemap.GetTile(cell);

                if (tile == null)
                    continue;

                if (uniqueTiles.Add(tile))
                {
                    allowedTiles.Add(tile);
                    addedCount++;
                }
            }
        }

        Debug.Log(
            $"[TilemapNavMeshMeshBuilder] CollectUsedTiles complete. Added={addedCount}, Total={allowedTiles.Count}",
            this
        );
    }
    private void EnsureChild()
    {
        if (meshChild == null)
        {
            Transform found = transform.Find(childName);
            if (found != null)
            {
                meshChild = found.gameObject;
            }
            else
            {
                meshChild = new GameObject(childName);
                meshChild.transform.SetParent(transform, false);
                meshChild.transform.localPosition = Vector3.zero;
                meshChild.transform.localRotation = Quaternion.identity;
                meshChild.transform.localScale = Vector3.one;
            }
        }

        if (!meshChild.TryGetComponent(out meshFilter))
            meshFilter = meshChild.AddComponent<MeshFilter>();

        if (createMeshRenderer)
        {
            if (!meshChild.TryGetComponent(out meshRenderer))
                meshRenderer = meshChild.AddComponent<MeshRenderer>();

            if (debugMaterial == null)
            {
                Shader shader = Shader.Find("Standard");
                if (shader != null)
                {
                    debugMaterial = new Material(shader);
                    debugMaterial.color = new Color(0f, 1f, 0f, 0.25f);
                }
            }

            if (debugMaterial != null)
                meshRenderer.sharedMaterial = debugMaterial;

            meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
            meshRenderer.receiveShadows = false;
        }
        else
        {
            meshRenderer = meshChild.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                if (Application.isPlaying)
                    Destroy(meshRenderer);
                else
                    DestroyImmediate(meshRenderer);

                meshRenderer = null;
            }
        }

        if (createMeshCollider)
        {
            if (!meshChild.TryGetComponent(out meshCollider))
                meshCollider = meshChild.AddComponent<MeshCollider>();
        }
        else
        {
            meshCollider = meshChild.GetComponent<MeshCollider>();
            if (meshCollider != null)
            {
                if (Application.isPlaying)
                    Destroy(meshCollider);
                else
                    DestroyImmediate(meshCollider);

                meshCollider = null;
            }
        }
    }

    private void ApplyRendererState()
    {
        if (meshRenderer != null)
            meshRenderer.enabled = rendererVisible;
    }
}
