using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class FanMeshGenerator : MonoBehaviour
{
    public int segments = 30;             // 삼각형 개수
    public float angle = 90f;             // 부채꼴 각도
    public float radius = 1f;             // 반지름
    public Color color = new Color(1, 0, 0, 0.4f); // 반투명 빨간색

    private Mesh mesh;
    private Material mat;

    void Awake()
    {
        // 메시 및 머티리얼 한 번만 설정
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        mat = new Material(Shader.Find("Sprites/Default")); // ✅ Sprite 렌더링용 쉐이더
        mat.color = color;
        GetComponent<MeshRenderer>().material = mat;

        var renderer = GetComponent<MeshRenderer>();
        renderer.sortingLayerName = "Default"; // 필요시 "UI"나 "Player" 등으로 바꿔도 됨
        renderer.sortingOrder = 10;
    }

    void Start()
    {
        GenerateMesh();
    }

    void OnEnable()
    {
        // SetActive(true)시 메시 재적용 보장
        if (mesh == null || mesh.vertexCount == 0)
        {
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
            GenerateMesh();
        }
    }

    public void UpdateMesh(float newAngle, float newRadius)
    {
        angle = newAngle;
        radius = newRadius;
        GenerateMesh();
    }

    void GenerateMesh()
    {
        mesh.Clear(); // ✅ 기존 메시 초기화

        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];

        vertices[0] = Vector3.zero;
        float angleStep = angle / segments;
        float startAngle = -angle / 2f;

        for (int i = 0; i <= segments; i++)
        {
            float rad = Mathf.Deg2Rad * (startAngle + i * angleStep);
            vertices[i + 1] = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;
        }

        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3 + 0] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}
