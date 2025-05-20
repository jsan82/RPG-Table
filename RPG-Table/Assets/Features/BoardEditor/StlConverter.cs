using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

public class StlConverter : MonoBehaviour
{
    private Material material;

    // Start is called before the first frame update
    void Start()
    {
        //Convert("input model path", "output model path"); //comment if not testing
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Convert(string filePath, string exportPath)
    {
        Mesh mesh = FileIsAscii(filePath) ? LoadAsciiStl(filePath) : LoadBinaryStl(filePath);

        if (mesh != null)
        {
            GameObject obj = new GameObject(Path.GetFileNameWithoutExtension(filePath));
            MeshFilter mf = obj.AddComponent<MeshFilter>();
            MeshRenderer mr = obj.AddComponent<MeshRenderer>();

            mf.mesh = mesh;
            mr.material = material != null ? material : new Material(Shader.Find("Standard"));

            ExportMeshToObj(mesh, exportPath);
            UnityEngine.Object.DestroyImmediate(obj);
            Debug.Log($"OBJ exported to: {exportPath}");
        }
    }

    private bool FileIsAscii(string path)
    {
        using (StreamReader reader = new StreamReader(path))
        {
            string header = reader.ReadLine();
            return header != null && header.Trim().StartsWith("solid", StringComparison.OrdinalIgnoreCase);
        }
    }

    private Mesh LoadBinaryStl(string path)
    {
        using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
        {
            reader.ReadBytes(80);
            uint triangleCount = reader.ReadUInt32();

            Vector3[] vertices = new Vector3[triangleCount * 3];
            int[] triangles = new int[triangleCount * 3];

            for (int i = 0; i < triangleCount; i++)
            {
                reader.ReadSingle(); reader.ReadSingle(); reader.ReadSingle();

                for (int v = 0; v < 3; v++)
                {
                    float x = reader.ReadSingle();
                    float y = reader.ReadSingle();
                    float z = reader.ReadSingle();
                    vertices[i * 3 + v] = new Vector3(x, z, y);
                    triangles[i * 3 + v] = i * 3 + v;
                }

                reader.ReadUInt16();
            }

            return CreateMesh(vertices, triangles);
        }
    }

    private Mesh LoadAsciiStl(string path)
    {
        var vertices = new List<Vector3>();
        var triangles = new List<int>();
        var vertexDict = new Dictionary<Vector3, int>();

        using (var reader = new StreamReader(path))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim();
                if (line.StartsWith("vertex"))
                {
                    Vector3 vertex = ParseVector(line.Substring(6));
                    vertices.Add(vertex);
                    triangles.Add(vertices.Count - 1);
                    //if (!vertexDict.ContainsKey(vertex))
                    //{
                    //    vertexDict[vertex] = vertices.Count;
                    //    vertices.Add(vertex);
                    //}
                    //triangles.Add(vertexDict[vertex]);
                }
            }
        }

        return CreateMesh(vertices.ToArray(), triangles.ToArray());
    }

    private Vector3 ParseVector(string input)
    {
        var parts = input.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        return new Vector3(
            float.Parse(parts[0], CultureInfo.InvariantCulture),
            float.Parse(parts[1], CultureInfo.InvariantCulture),
            float.Parse(parts[2], CultureInfo.InvariantCulture)
        );
    }

    private Mesh CreateMesh(Vector3[] vertices, int[] triangles)
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        // no textures and no UV so no more recalculating I guess
        return mesh;
    }

    private void ExportMeshToObj(Mesh mesh, string path)
    {
        using (StreamWriter writer = new StreamWriter(path))
        {
            writer.WriteLine("# Exported from Unity STL importer");

            foreach (Vector3 v in mesh.vertices)
                writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "v {0} {1} {2}", v.x, v.y, v.z));
            //writer.WriteLine(string.Format(CultureInfo.InvariantCulture, $"v {v.x} {v.y} {v.z}"));

            foreach (Vector3 n in mesh.normals)
                writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "vn {0} {1} {2}", n.x, n.y, n.z));
            //writer.WriteLine(string.Format(CultureInfo.InvariantCulture, $"vn {n.x} {n.y} {n.z}"));

            for (int i = 0; i < mesh.triangles.Length; i += 3)
            {
                int i1 = mesh.triangles[i] + 1;
                int i2 = mesh.triangles[i + 1] + 1;
                int i3 = mesh.triangles[i + 2] + 1;
                writer.WriteLine($"f {i1}//{i1} {i2}//{i2} {i3}//{i3}");
            }
        }
    }
}