using System.Linq;

namespace ASD
{
    using ASD.Graphs;
    using System;
    using System.Collections.Generic;

    public class Lab06 : System.MarshalByRefObject
    {
        /// <summary>
        /// Część I: wyznaczenie najszerszej ścieżki grafu.
        /// </summary>
        /// <param name="G">informacja o przejazdach między punktami; wagi krawędzi są całkowite i nieujemne i oznaczają szerokość trasy między dwoma punktami</param>
        /// <param name="start">informacja o wierzchołku początkowym</param>
        /// <param name="end">informacja o wierzchołku końcowym</param>
        /// <returns>najszersza ścieżka między wierzchołkiem początkowym a końcowym lub pusta lista, jeśli taka ścieżka nie istnieje</returns>
        public List<int> WidePath(DiGraph<int> G, int start, int end)
        {
            int[] szerokosc = new int[G.VertexCount];
            int[] from = new int[G.VertexCount];
            for (int i = 0; i < G.VertexCount; i++)
            {
                szerokosc[i] = 0;
                from[i] = -1;
            }
            szerokosc[start] = int.MaxValue;
            PriorityQueue<int, int> Q = new PriorityQueue<int, int>();
            Q.Insert(start, -szerokosc[start]);
            while (Q.Count != 0)
            {
                int u = Q.Extract();
                foreach (int v in G.OutNeighbors(u))
                {
                    if (szerokosc[v] < Math.Min(G.GetEdgeWeight(u, v), szerokosc[u]))
                    {
                        szerokosc[v] = Math.Min(G.GetEdgeWeight(u, v), szerokosc[u]);
                        from[v] = u;
                        Q.Insert(v, -szerokosc[v]);
                    }
                }
            }
            if (szerokosc[end] == 0)
                return new List<int>();

            List<int> L = new List<int>();
            int x = end;
            while (x != start)
            {
                L.Add(x);
                x = from[x];
            }
            L.Add(x);
            L.Reverse();
            return L;
        }

        /// <summary>
        /// Część II: wyznaczenie najszerszej epidemicznej ścieżki.
        /// </summary>
        /// <param name="G">informacja o przejazdach między punktami; wagi krawędzi są całkowite i nieujemne i oznaczają szerokość trasy między dwoma punktami</param>
        /// <param name="start">informacja o wierzchołku początkowym</param>
        /// <param name="end">informacja o wierzchołku końcowym</param>
        /// <param name="weights">wagi wierzchołków odpowiadające czasom oczekiwania na bramkach wjzadowych do poszczególnych miejsc. Wagi są nieujemne i całkowite</param>
        /// <param name="maxWeight">maksymalna waga krawędzi w grafie</param>
        /// <returns>ścieżka dla której różnica między jej najwęższą krawędzią a sumą wag wierzchołków przez które przechodzi jest maksymalna.</returns>
        public List<int> WeightedWidePath(DiGraph<int> G, int start, int end, int[] weights, int maxWeight)
        {
            //int i = 0; 
            int n = G.VertexCount;
            List<Edge<int>> edges = new List<Edge<int>>();
            List<int> szerokosci = new List<int>();
            foreach (var e in G.BFS().SearchAll())
            {
                if (!szerokosci.Contains(e.weight))
                    szerokosci.Add(e.weight);
                edges.Add(e);
            }
            szerokosci.Sort();
            int[] kk = szerokosci.ToArray();
            DiGraph<int> g = new DiGraph<int>(n);
            bool isEnd = false;
            int[] path = new int[0];
            int max = int.MinValue;
            for (int i = kk.Length - 1; i >= 0; i--)
            {
                foreach (var e in edges)
                {
                    if (e.weight == szerokosci[i] && !g.HasEdge(e.From, e.To))
                    {
                        g.AddEdge(e.From, e.To, weights[e.To]);
                    }
                }
                edges.RemoveAll(e => e.weight == szerokosci[i]);

                var p = Paths.Dijkstra(g, start);
                if (p.Reachable(start, end))
                {
                    isEnd = true;
                    if (kk[i] - p.GetDistance(start, end) > max)
                    {
                        max = kk[i] - p.GetDistance(start, end);
                        path = p.GetPath(start, end);
                    }
                }
            }

            if (!isEnd) return new List<int>();
            return path.ToList();
        }
    }
}