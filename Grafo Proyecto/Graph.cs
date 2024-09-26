using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace grafo_pruebas_consola
{
    public class Graph
    {
        public List<Node> TheNode = new List<Node>();
        public int[,] Matrix;
        public void AddNode(Node nodo)
        {
            //se puede enviar el parametro nodo y que este sea creado en la parte de botones
            //no es necesario realizar una comparacion ya que siempre se le mandaran valores distintos al crear nodos
            TheNode.Add(nodo);
        }
        //no se porque al iniciar los comparer como nodo no los detecta los otros
        public bool Revisar(Node node1, Node node2, IComparer<char> comp=null) {
            comp=Comparer<char>.Default;
            foreach(var a in node1.Vert) {
                int r=comp.Compare(a.Target.Data,node2.Data);
                if (r == 0) return false;
            }
            return true;
        }

        public int Conect(Node node1, Node node2, int p, IComparer<char> comp = null)
        {
            comp = Comparer<char>.Default;
            foreach (var a in node2.Vert)
            {
                int r = comp.Compare(a.Target.Data, node1.Data);
                if (r == 0) {
                    p = a.Weight;
                } 
            }
            node1.Vert.Add(new Edge(node2, p));
            return p;
        }

        public Queue<Node> BFS()
        {
            var output = new Queue<Node>();
            var tempQueue = new Queue<Node>();
            var current = TheNode[0];
            current.Visited = true;
            tempQueue.Enqueue(current);
            while (tempQueue.Count > 0)
            {
                current = tempQueue.Dequeue();
                output.Enqueue(current);
                for (int i = 0; i < current.Vert.Count; i++)
                {
                    if (!current.Vert[i].Target.Visited)
                    {
                        current.Vert[i].Target.Visited = true;
                        tempQueue.Enqueue(current.Vert[i].Target);
                    }
                }
            }
            foreach (var a in TheNode)
                a.Visited = false;
            return output;
        }

        public Stack<Node> DFS()
        {
            var output = new Stack<Node>();
            var tempQueue = new Stack<Node>();
            var current = TheNode[0];
            current.Visited = true;
            tempQueue.Push(current);
            while (tempQueue.Count > 0)
            {
                current = tempQueue.Pop();
                output.Push(current);
                for (int i = 0; i < current.Vert.Count; i++)
                {
                    Console.WriteLine("pruba " + current.Vert[i].Target.Data);
                    if (!current.Vert[i].Target.Visited)
                    {
                        current.Vert[i].Target.Visited = true;
                        tempQueue.Push(current.Vert[i].Target);
                    }
                }
            }
            foreach (var a in TheNode)
                a.Visited = false;
            return output;
        }

        public void setMat()
        {
            Matrix = new int[TheNode.Count, TheNode.Count];
            for (int i = 0; i < TheNode.Count; i++) {
                var a = TheNode[i];
                for (int j = 0; j < a.Vert.Count; j++) {
                    var b = a.Vert[j];
                    int p=0;
                    for (int h = 0; h < TheNode.Count; h++) {
                        if (b.Target.Data == TheNode[h].Data) { 
                            p = h;
                        }
                    }
                    Matrix[i, p] =b.Weight;
                }
            }
        }
        public int dijkstra(char csrc,char cdest) {
            int src=0,dest=0;
            bool a=false;
            bool b=false;
            for (int i = 0; i < TheNode.Count; i++)
            {
                if (TheNode[i].Data == csrc) { 
                    src = i; 
                    a=true;
                }
                if (TheNode[i].Data == cdest){
                    dest = i;
                    b=true;
                }
            }
            if (a && b)
            {
                int[] dist = new int[TheNode.Count];
                bool[] sptSet = new bool[TheNode.Count];

                for (int i = 0; i < TheNode.Count; i++)
                {
                    dist[i] = int.MaxValue;
                    sptSet[i] = false;
                }
                dist[src] = 0;
                for (int count = 0; count < TheNode.Count - 1; count++)
                {
                    int u = minDistance(dist, sptSet);
                    sptSet[u] = true;
                    for (int v = 0; v < TheNode.Count; v++)
                        if (!sptSet[v] && Matrix[u, v] != 0
                            && dist[u] != int.MaxValue
                            && dist[u] + Matrix[u, v] < dist[v])
                            dist[v] = dist[u] + Matrix[u, v];
                }
                return dist[dest];
            }
            return int.MaxValue;
        }
        int minDistance(int[] dist, bool[] sptSet)
        {
            int min = int.MaxValue, min_index = -1;

            for (int v = 0; v < TheNode.Count; v++)
                if (sptSet[v] == false && dist[v] <= min)
                {
                    min = dist[v];
                    min_index = v;
                }

            return min_index;
        }
    }
}
