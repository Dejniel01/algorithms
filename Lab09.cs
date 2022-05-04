
using System.Collections.Generic;
using System.Linq;
using ASD.Graphs;

/// <summary>
/// Klasa rozszerzająca klasę Graph o rozwiązania problemów największej kliki i izomorfizmu grafów metodą pełnego przeglądu (backtracking)
/// </summary>
public static class Lab10GraphExtender
{
    /// <summary>
    /// Wyznacza największą klikę w grafie i jej rozmiar metodą pełnego przeglądu (backtracking)
    /// </summary>
    /// <param name="g">Badany graf</param>
    /// <param name="clique">Wierzchołki znalezionej największej kliki - parametr wyjściowy</param>
    /// <returns>Rozmiar największej kliki</returns>
    /// <remarks>
    /// Nie wolno modyfikować badanego grafu.
    /// </remarks>
    public static int MaxClique(this Graph g, out int[] clique)
    {
        List<int> S = new List<int>();
        List<int> bestS = new List<int>();

        MaxCliqueRec(0);

        clique = bestS.ToArray();
        return bestS.Count;

        void MaxCliqueRec(int k)
        {
            List<int> C = new List<int>();
            for (int i = k; i < g.VertexCount; i++)
            {
                int j;
                for (j = 0; j < S.Count; j++)
                    if (!g.HasEdge(S[j], i))
                        break;
                if (j == S.Count)
                    C.Add(i);
            }

            if (C.Count + S.Count <= bestS.Count)
                return;
            else if (S.Count > bestS.Count)
                bestS = S.ToList();

            foreach (int m in C)
            {
                S.Add(m);
                MaxCliqueRec(m + 1);
                S.Remove(m);
            }
        }
    }

    /// <summary>
    /// Bada izomorfizm grafów metodą pełnego przeglądu (backtracking)
    /// </summary>
    /// <param name="g">Pierwszy badany graf</param>
    /// <param name="h">Drugi badany graf</param>
    /// <param name="map">Mapowanie wierzchołków grafu h na wierzchołki grafu g (jeśli grafy nie są izomorficzne to null) - parametr wyjściowy</param>
    /// <returns>Informacja, czy grafy g i h są izomorficzne</returns>
    /// <remarks>
    /// 1) Uwzględniamy wagi krawędzi
    /// 3) Nie wolno modyfikować badanych grafów.
    /// </remarks>
    public static bool IsomorphismTest(this Graph<int> g, Graph<int> h, out int[] map)
    {
        var tmp = g;
        g = h;
        h = tmp;
        map = null;
        if (g.VertexCount != h.VertexCount) return false;

        var used = new bool[g.VertexCount];
        var permutation = new int[g.VertexCount];
        int[] temp = null;

        GenerateMap(0);

        if (temp != null)
        {
            map = temp.ToArray();
            return true;
        }
        return false;

        void GenerateMap(int k)
        {
            if (k == g.VertexCount)
            {
                temp = permutation.ToArray();
                return;
            }
            for (int m = 0; m < g.VertexCount; m++)
                if (!used[m] && g.OutNeighbors(k).Count() == h.OutNeighbors(m).Count())
                {
                    bool git = true;
                    foreach (var v in g.OutNeighbors(k))
                    {
                        if (v < k)
                        {
                            if (!h.HasEdge(m, permutation[v]) || g.GetEdgeWeight(k, v) != h.GetEdgeWeight(m, permutation[v]))
                            {
                                git = false;
                                break;
                            }
                        }
                    }

                    if (git)
                        for (int v = 0; v < k; v++)
                            if (g.HasEdge(v, k) && (!h.HasEdge(permutation[v], m) || g.GetEdgeWeight(v, k) != h.GetEdgeWeight(permutation[v], m)))
                            {
                                git = false;
                                break;
                            }

                    if (!git) continue;

                    used[m] = true;
                    permutation[k] = m;
                    GenerateMap(k + 1);
                    used[m] = false;
                }
        }
    }



}

