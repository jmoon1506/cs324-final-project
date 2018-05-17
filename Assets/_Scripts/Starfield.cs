using UnityEngine;

public class Starfield : MonoBehaviour
{
    public int numStars = 200;
    public float minSize = 0.01f;
    public float maxSize = 0.05f;
    public float parallax = 0.02f;

    public Gradient colorGradient;

    public Texture2D starTexture;

    public float halfHeight;
    public float halfWidth;

    public void CreateStarfield()
    {
        Vector3[] vertices = new Vector3[numStars * 4];
        Vector2[] uvs = new Vector2[vertices.Length];

        //halfWidth = Camera.main.aspect * Camera.main.orthographicSize + parallax * GameManager.singleton.mapWidth;
        //halfHeight = halfWidth / Camera.main.aspect;

        halfHeight = 0.9f * Camera.main.orthographicSize + parallax * GameManager.singleton.mapHeight;
        halfWidth = 0.9f * Camera.main.aspect * Camera.main.orthographicSize + parallax * GameManager.singleton.mapWidth;

        for (int i = 0; i < numStars; i++)
        {
            Vector3 position = new Vector3(Random.Range(-halfWidth, halfWidth), Random.Range(-halfHeight, halfHeight), 0);
            float size = Random.Range(minSize, maxSize) / 2.0f;
            Vector2 v0 = new Vector3(-size, size);
            Vector2 v1 = new Vector3(-size, -size);
            Vector2 v2 = new Vector3(size, -size);
            Vector2 v3 = new Vector3(size, size);
            Quaternion rotation = Quaternion.Euler(0, 0, Random.Range(0.0f, 360.0f));
            vertices[i * 4] = position + rotation * v0;
            vertices[i * 4 + 1] = position + rotation * v1;
            vertices[i * 4 + 2] = position + rotation * v2;
            vertices[i * 4 + 3] = position + rotation * v3;

            uvs[i * 4] = new Vector2(0, 1.0f);
            uvs[i * 4 + 1] = new Vector2(0, 0);
            uvs[i * 4 + 2] = new Vector2(1.0f, 0);
            uvs[i * 4 + 3] = new Vector2(1.0f, 1.0f);
        }

        int[] triangles = new int[vertices.Length / 2 * 3];

        for (int j = 0; j < vertices.Length / 4; j++) {

            triangles[j * 6 + 0] = j * 4 + 0;    //   0_ 3          0 ___ 3
            triangles[j * 6 + 1] = j * 4 + 3;    //   | /            |  /|
            triangles[j * 6 + 2] = j * 4 + 1;    //  1|/            1|/__|2

            triangles[j * 6 + 3] = j * 4 + 3;    //     3
            triangles[j * 6 + 4] = j * 4 + 2;    //    /|
            triangles[j * 6 + 5] = j * 4 + 1;    //  1/_|2
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        MeshFilter mf = GetComponent<MeshFilter>();
        mf.mesh = mesh;
        Material mat = new Material(Shader.Find("Unlit/ParticleOccluded"));
        mat.SetTexture("_MainTex", starTexture);
        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.material = mat;
    }

    /*
    public void CreatePoints() 
    {
        if (GetComponent<ParticleSystem>().maxParticles < stars) {
            GetComponent<ParticleSystem>().maxParticles = stars;
        }
        points = new ParticleSystem.Particle[stars];
        float halfHeight = transform.localPosition.z * Mathf.Tan(Mathf.Deg2Rad * Camera.main.fieldOfView / 2.0f)
            + parallaxMovement * GameManager.singleton.mapHeight;
        float halfWidth = Camera.main.aspect * halfHeight;
        for (int i = 0; i < points.Length; i++) {
            //points[i].position = new Vector3(Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f), 0);
            points[i].position = new Vector3(Random.Range(-halfWidth, halfWidth),
                Random.Range(-halfHeight, halfHeight), 0);
            points[i].startColor = colorGradient.Evaluate(Random.value);
            points[i].startSize = Random.Range(minSize, maxSize);
        }
    }

    void Start()
    {
        if (points == null) {
            CreatePoints();
        }
    }
    */

    void Start()
    {
        //CreateStarfield();
    }

    void Update()
    {
        if (parallax != 0.0f)
        {
            transform.localPosition = new Vector3(parallax * Camera.main.transform.position.x, parallax * Camera.main.transform.position.y, transform.localPosition.z);
        }
    }
}
