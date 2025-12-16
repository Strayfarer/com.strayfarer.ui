#nullable enable
using UnityEngine.UIElements;

public class Tri {
    internal Vertex vertA;
    internal Vertex vertB;
    internal Vertex vertC;

    public Tri(Vertex vertA, Vertex vertB, Vertex vertC) {
        this.vertA = vertA;
        this.vertB = vertB;
        this.vertC = vertC;
    }
}
