
namespace ASD
{
    using ASD.Graphs;
    using System;
    using System.Collections.Generic;

    public class Lab03 : System.MarshalByRefObject
    {
        // Część I
        // Funkcja zwracajaca kwadrat danego grafu.
        // Kwadratem grafu nazywamy graf o takim samym zbiorze wierzchołków jak graf pierwotny, w którym wierzchołki
        // połączone sa krawędzią jeśli w grafie pierwotnym były polączone krawędzia bądź ścieżką złożoną z 2 krawędzi
        // (ale pętli, czyli krawędzi o początku i końcu w tym samym wierzchołku, nie dodajemy!).
        public Graph Square(Graph graph)
        {
            Graph g = new Graph(graph.VertexCount, graph.Representation);

            foreach(Edge e in graph.BFS().SearchAll())
            {
                g.AddEdge(e.From, e.To);
                foreach (int n in graph.OutNeighbors(e.To))
                    if (e.From != n)
                        g.AddEdge(e.From, n);
            }

            return g;
        }

        // Część II
        // Funkcja zwracająca Graf krawędziowy danego grafu.
        // Wierzchołki grafu krawędziwego odpowiadają krawędziom grafu pierwotnego, wierzcholki grafu krawędziwego
        // połączone sa krawędzią jeśli w grafie pierwotnym z krawędzi odpowiadającej pierwszemu z nich można przejść
        // na krawędź odpowiadającą drugiemu z nich przez wspólny wierzchołek.

        // Tablicę names tworzymy i wypełniamy według następującej zasady.
        // Każdemu wierzchołkowi grafu krawędziowego odpowiada element tablicy names (o indeksie równym numerowi wierzchołka)
        // zawierający informację z jakiej krawędzi grafu pierwotnego wierzchołek ten powstał.
        // Np.dla wierzchołka powstałego z krawedzi <0,1> do tablicy zapisujemy krotke (0, 1) - przyda się w dalszych etapach
        public Graph LineGraph(Graph graph, out (int x, int y)[] names)
        {
            int m = 0;
            foreach (var e in graph.DFS().SearchAll())
                if (e.From < e.To)
                    m++;
            names = new (int x, int y)[m];
            Graph g = new Graph(m, graph.Representation);
            int k = 0;
            foreach(var e in graph.DFS().SearchAll())
            {
                if(e.From < e.To)
                {
                    names[k] = (e.From, e.To);
                    for (int i = 0; i < k; i++)
                        if (names[i].x == e.From || names[i].y == e.From || names[i].x == e.To || names[i].y == e.To)
                            g.AddEdge(i, k);
                    k++;
                }
            }
            return g;
        }

        // Część III
        // Funkcja znajdujaca poprawne kolorowanie wierzchołków danego grafu nieskierowanego.
        // Kolorowanie wierzchołków jest poprawne, gdy każde dwa sąsiadujące wierzchołki mają różne kolory
        // Funkcja ma szukać kolorowania według następujacego algorytmu zachłannego:

        // Dla wszystkich wierzchołków v (od 0 do n-1)
        // pokoloruj wierzcholek v kolorem o najmniejszym możliwym numerze(czyli takim, na który nie są pomalowani jego sąsiedzi)
        // Kolory numerujemy począwszy od 0.

        // UWAGA: Podany opis wyznacza kolorowanie jednoznacznie, jakiekolwiek inne kolorowanie, nawet jeśli spełnia formalnie
        // definicję kolorowania poprawnego, na potrzeby tego zadania będzie uznane za błędne.

        // Funkcja zwraca liczbę użytych kolorów (czyli najwyższy numer użytego koloru + 1),
        // a w tablicy colors zapamiętuje kolory poszczególnych wierzchołkow.
        public int VertexColoring(Graph graph, out int[] colors)
        {
            colors = new int[graph.VertexCount];
            int maxcolor = -1;

            bool[,] used = new bool[graph.VertexCount, graph.VertexCount];

            for (int i = 0; i < graph.VertexCount; i++)
                colors[i] = -1;

            for (int i = 0; i < graph.VertexCount; i++)
            {
                int color;
                for (color = 0; color < graph.VertexCount; color++)
                    if (!used[i, color])
                        break;
                colors[i] = color;
                foreach (int n in graph.OutNeighbors(i))
                    used[n, color] = true;
                maxcolor = Math.Max(maxcolor, color);
            }
            return maxcolor + 1;
        }

        // Funkcja znajduje silne kolorowanie krawędzi danego grafu.
        // Silne kolorowanie krawędzi grafu jest poprawne gdy każde dwie krawędzie, które są ze sobą sąsiednie
        // (czyli można przejść z jednej na drugą przez wspólny wierzchołek)
        // albo są połączone inną krawędzią(czyli można przejść z jednej na drugą przez ową inną krawędź), mają różne kolory.

        // Należy zwrocić nowy graf, który będzie miał strukturę identyczną jak zadany graf,
        // ale w wagach krawędzi zostaną zapisane przydzielone kolory.

        // Wskazówka - to bardzo proste.Należy wykorzystać wszystkie poprzednie funkcje.
        // Zastanowić się co możemy powiedzieć o kolorowaniu wierzchołków kwadratu grafu krawędziowego?
        // Jak się to ma do silnego kolorowania krawędzi grafu pierwotnego?
        public int StrongEdgeColoring(Graph graph, out Graph<int> coloredGraph)
        {
            coloredGraph = new Graph<int>(graph.VertexCount, graph.Representation);

            (int x, int y)[] names;
            Graph linearSquared = LineGraph(graph, out names);
            linearSquared = Square(linearSquared);
            
            int[] colors;
            int colorCount = VertexColoring(linearSquared, out colors);

            for (int i = 0; i < names.Length; i++)
                coloredGraph.AddEdge(names[i].x, names[i].y, colors[i]);

            return colorCount;
        }

    }

}
