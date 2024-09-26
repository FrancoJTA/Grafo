using System.Collections.Generic;

namespace grafo_pruebas_consola
{
    public class Node
    {
        public char Data { get; set; }
        public List<Edge> Vert { get; set; }
        public bool Visited;
        public Node(char data)
        {
            this.Data = data;
            Vert = new List<Edge>();
        }
    }
    public class Edge
    {
        public Node Target { get; set; }
        public int Weight { get; set; }

        public Edge(Node target, int weight)
        {
            Target = target;
            Weight = weight;
        }
    }
}
