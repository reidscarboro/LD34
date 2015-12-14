using UnityEngine;
using System.Collections;

public class TreeController : MonoBehaviour {

    public MeshFilter mf;
    public PolygonCollider2D polygon;
    public TreeOutlineController treeOutlineController;

    public GameObject treeOutline;
    public GameObject treeTexture;
    public GameObject treePlaceholder;

	public void updatePolygon() {
        mf.mesh.Clear();

        Mesh newMesh = createMesh(polygon.GetPath(0));

        mf.mesh = newMesh;
        treeOutlineController.setMesh(newMesh);
    }

    public void enable() {
        treeOutline.SetActive(true);
        treeTexture.SetActive(true);
        treePlaceholder.SetActive(false);
        updatePolygon();
    }

    public Vector2[] getVerticies() {
        return polygon.GetPath(0);
    }

    public void setVertices(Vector2[] vertices) {
        polygon.SetPath(0, vertices);
    }

    public Mesh createMesh(Vector2[] nodePositions) {

        Triangulator tr = new Triangulator(nodePositions);
        int[] indices = tr.Triangulate();

        // Create the Vector3 vertices
        Vector3[] vertices = new Vector3[nodePositions.Length];
        for (int i = 0; i < vertices.Length; i++) {
            vertices[i] = new Vector3(nodePositions[i].x, nodePositions[i].y, 0);
        }

        // Create the mesh
        Mesh msh = new Mesh();
        msh.vertices = vertices;
        msh.triangles = indices;
        msh.RecalculateNormals();
        msh.RecalculateBounds();

        return msh;
    }
}
