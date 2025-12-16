#nullable enable
using System.Collections.Generic;
using UnityEngine.UIElements;

public class TriUtil {
    List<Tri> tris;

    public TriUtil(List<Tri> listOfTris) {
        tris = listOfTris;
    }

    public void SetListOfTris(List<Tri> listOfTris) {
        tris = listOfTris;
    }

    public void CalculateVertsAndIndices(out Vertex[] vertices, out ushort[] indices) {
        var verts = new List<Vertex>();
        var inds = new List<ushort>();
        foreach (var tri in tris) {
            if (!verts.Contains(tri.vertA)) {
                verts.Add(tri.vertA);
            }

            if (!verts.Contains(tri.vertB)) {
                verts.Add(tri.vertB);
            }

            if (!verts.Contains(tri.vertC)) {
                verts.Add(tri.vertC);
            }

            inds.Add((ushort)verts.IndexOf(tri.vertA));
            inds.Add((ushort)verts.IndexOf(tri.vertB));
            inds.Add((ushort)verts.IndexOf(tri.vertC));
        }

        vertices = verts.ToArray();
        indices = inds.ToArray();
    }
}
