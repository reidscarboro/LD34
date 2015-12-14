using UnityEngine;
using System.Collections;

public class TreeOutlineController : MonoBehaviour {

    public MeshFilter mf;

    public void setMesh(Mesh mesh) {
        mf.mesh = mesh;
    }
}
