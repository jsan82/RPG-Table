using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

/// <summary>
/// Converts STL files (ASCII or binary) to Unity Meshes and exports them as OBJ files.
/// </summary>
public class StlConverter : MonoBehaviour
{
    /// <summary>
    /// Optional material to assign to the created GameObject when converting.
    /// </summary>
    private Material material;

/*    void Start() //comment if not testing
    {
        Convert("input model path", "output model path"); 
    }*/

    /// <summary>
    /// Converts an STL file into a Unity Mesh and exports it as an OBJ file.
    /// </summary>
    /// <param name="filePath">Path to the input STL file.</param>
    /// <param name="exportPath">Path to save the output OBJ file.</param>
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

    /// <summary>
    /// Checks whether an STL file is ASCII or binary.
    /// </summary>
    /// <param name="path">Path to the STL file.</param>
    /// <returns>True if ASCII, false if binary.</returns>
    private bool FileIsAscii(string path)
    {
        using (StreamReader reader = new StreamReader(path))
        {
            string header = reader.ReadLine();
            return header != null && header.Trim().StartsWith("solid", StringComparison.OrdinalIgnoreCase);
        }
    }

    /// <summary>
    /// Loads a binary STL file and converts it to a Unity Mesh.
    /// </summary>
    /// <param name="path">Path to the binary STL file.</param>
    /// <returns>Converted Unity Mesh.</returns>
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

    /// <summary>
    /// Loads an ASCII STL file and converts it to a Unity Mesh.
    /// </summary>
    /// <param name="path">Path to the ASCII STL file.</param>
    /// <returns>Converted Unity Mesh.</returns>
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
                }
            }
        }

        return CreateMesh(vertices.ToArray(), triangles.ToArray());
    }

    /// <summary>
    /// Parses a line into a Vector3.
    /// </summary>
    /// <param name="input">Space-separated string of 3 float values.</param>
    /// <returns>Parsed Vector3.</returns>
    private Vector3 ParseVector(string input)
    {
        var parts = input.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        return new Vector3(
            float.Parse(parts[0], CultureInfo.InvariantCulture),
            float.Parse(parts[1], CultureInfo.InvariantCulture),
            float.Parse(parts[2], CultureInfo.InvariantCulture)
        );
    }

    /// <summary>
    /// Creates a Unity Mesh from vertex and triangle data.
    /// </summary>
    /// <param name="vertices">Array of vertex positions.</param>
    /// <param name="triangles">Array of triangle indices.</param>
    /// <returns>Generated Mesh.</returns>
    private Mesh CreateMesh(Vector3[] vertices, int[] triangles)
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    /// <summary>
    /// Exports a Unity Mesh to an OBJ file.
    /// </summary>
    /// <param name="mesh">Mesh to export.</param>
    /// <param name="path">Destination file path for the OBJ.</param>
    private void ExportMeshToObj(Mesh mesh, string path)
    {
        using (StreamWriter writer = new StreamWriter(path))
        {
            writer.WriteLine("# Exported from Unity STL importer");

            foreach (Vector3 v in mesh.vertices)
                writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "v {0} {1} {2}", v.x, v.y, v.z));

            foreach (Vector3 n in mesh.normals)
                writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "vn {0} {1} {2}", n.x, n.y, n.z));

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