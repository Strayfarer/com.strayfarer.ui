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

    public void CalculateVertsAndIndices(out Vertex[] vertices, int noOfVerts, out ushort[] indices, int noOfInds) {

        vertices = new Vertex[noOfVerts];
        indices = new ushort[noOfInds];
        int currentVertIndex = 0;
        int currentIndIndex = 0;
        foreach (var tri in tris) {
            if (currentVertIndex == 0) {
                vertices[currentVertIndex] = tri.vertA;
                indices[currentIndIndex] = (ushort)currentVertIndex;
                currentIndIndex++;
                currentVertIndex++;

                vertices[currentVertIndex] = tri.vertB;
                indices[currentIndIndex] = (ushort)currentVertIndex;
                currentIndIndex++;
                currentVertIndex++;

                vertices[currentVertIndex] = tri.vertC;
                indices[currentIndIndex] = (ushort)currentVertIndex;
                currentIndIndex++;
                currentVertIndex++;
            } else {
                bool foundA = false;
                bool foundB = false;
                bool foundC = false;
                for (int i = 0; i < currentVertIndex; i++) {
                    if (!foundA && vertices[i].position == tri.vertA.position && vertices[i].tint.Equals(tri.vertA.tint)) {
                        foundA = true;
                        indices[currentIndIndex] = (ushort)i;
                    }
                    if (!foundB && vertices[i].position == tri.vertB.position && vertices[i].tint.Equals(tri.vertB.tint)) {
                        foundB = true;
                        indices[currentIndIndex + 1] = (ushort)i;
                    }
                    if (!foundC && vertices[i].position == tri.vertC.position && vertices[i].tint.Equals(tri.vertC.tint)) {
                        foundC = true;
                        indices[currentIndIndex + 2] = (ushort)i;
                    }
                }

                if (!foundA) {
                    vertices[currentVertIndex] = tri.vertA;
                    indices[currentIndIndex] = (ushort)currentVertIndex;
                    currentVertIndex++;
                }
                if (!foundB) {
                    vertices[currentVertIndex] = tri.vertB;
                    indices[currentIndIndex + 1] = (ushort)currentVertIndex;
                    currentVertIndex++;
                }
                if (!foundC) {
                    vertices[currentVertIndex] = tri.vertC;
                    indices[currentIndIndex + 2] = (ushort)currentVertIndex;
                    currentVertIndex++;
                }
                currentIndIndex += 3;
            }
        }
    }
}
