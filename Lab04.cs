using System;
using ASD.Graphs;
using ASD;
using System.Collections.Generic;

namespace ASD
{

    public class Lab04 : System.MarshalByRefObject
    {
        /// <summary>
        /// Etap 1 - szukanie trasy z miasta start_v do miasta end_v, startując w dniu day
        /// </summary>
        /// <param name="g">Ważony graf skierowany będący mapą</param>
        /// <param name="start_v">Indeks wierzchołka odpowiadającego miastu startowemu</param>
        /// <param name="end_v">Indeks wierzchołka odpowiadającego miastu docelowemu</param>
        /// <param name="day">Dzień startu (w tym dniu należy wyruszyć z miasta startowego)</param>
        /// <param name="days_number">Liczba dni uwzględnionych w rozkładzie (tzn. wagi krawędzi są z przedziału [0, days_number-1])</param>
        /// <returns>(result, route) - result ma wartość true gdy podróż jest możliwa, wpp. false, 
        /// route to tablica z indeksami kolejno odwiedzanych miast (pierwszy indeks to indeks miasta startowego, ostatni to indeks miasta docelowego),
        /// jeżeli result == false to route ustawiamy na null</returns>
        public (bool result, int[] route) Lab04_FindRoute(DiGraph<int> g, int start_v, int end_v, int day, int days_number)
        {
            DiGraph rg = new DiGraph(g.VertexCount * days_number, g.Representation);

            foreach (var e in g.DFS().SearchAll())
                rg.AddEdge(e.From*days_number + Decrement(e.weight, days_number), e.To*days_number + e.weight);

            int[] from = new int[g.VertexCount * days_number];
            for (int i = 0; i < g.VertexCount * days_number; i++)
                from[i] = -1;

            int last = -1;

            foreach (var e in rg.DFS().SearchFrom(start_v * days_number + Decrement(day, days_number)))
            {
                if(from[e.To] == -1)
                    from[e.To] = e.From;
                if (e.To / days_number == end_v)
                {
                    last = e.To;
                    break;
                }
            }

            if(last == -1)
                return (false, null);

            List<int> l = new List<int>();

            while (last != start_v * days_number + Decrement(day, days_number))
            {
                l.Add(last / days_number);
                last = from[last];
            }
            l.Add(last / days_number);
            l.Reverse();

            return (true, l.ToArray());
        }

        private int Increment(int day, int days_number)
        {
            return (day + 1) % days_number;
        }
        private int Decrement(int day, int days_number)
        {
            return (day + days_number - 1) % days_number;
        }
        /// <summary>
        /// Etap 2 - szukanie trasy z jednego z miast z tablicy start_v do jednego z miast z tablicy end_v (startować można w dowolnym dniu)
        /// </summary>
        /// <param name="g">Ważony graf skierowany będący mapą</param>
        /// <param name="start_v">Tablica z indeksami wierzchołków startowych (trasę trzeba zacząć w jednym z nich)</param>
        /// <param name="end_v">Tablica z indeksami wierzchołków docelowych (trasę trzeba zakończyć w jednym z nich)</param>
        /// <param name="days_number">Liczba dni uwzględnionych w rozkładzie (tzn. wagi krawędzi są z przedziału [0, days_number-1])</param>
        /// <returns>(result, route) - result ma wartość true gdy podróż jest możliwa, wpp. false, 
        /// route to tablica z indeksami kolejno odwiedzanych miast (pierwszy indeks to indeks miasta startowego, ostatni to indeks miasta docelowego),
        /// jeżeli result == false to route ustawiamy na null</returns>
        public (bool result, int[] route) Lab04_FindRouteSets(DiGraph<int> g, int[] start_v, int[] end_v, int days_number)
        {
            DiGraph rg = new DiGraph(g.VertexCount * days_number + 2, g.Representation);

            foreach (var e in g.DFS().SearchAll())
                rg.AddEdge(e.From * days_number + Decrement(e.weight, days_number), e.To * days_number + e.weight);

            int[] from = new int[g.VertexCount * days_number + 2];
            for (int i = 0; i < g.VertexCount * days_number + 2; i++)
                from[i] = -1;

            int last = -1;

            int fake_start = g.VertexCount * days_number;
            int fake_end = g.VertexCount * days_number + 1;

            foreach (var s in start_v)
                for (int i = 0; i < days_number; i++)
                    rg.AddEdge(fake_start, s * days_number + i);

            foreach (var s in end_v)
                for (int i = 0; i < days_number; i++)
                    rg.AddEdge(s * days_number + i, fake_end);

            foreach (var e in rg.DFS().SearchFrom(fake_start))
            {
                if (from[e.To] == -1)
                    from[e.To] = e.From;
                if (e.To == fake_end)
                {
                    last = e.To;
                    break;
                }
            }

            if (last == -1)
                return (false, null);

            List<int> l = new List<int>();

            last = from[last];

            while (last != fake_start)
            {
                l.Add(last / days_number);
                last = from[last];
            }
            l.Reverse();

            return (true, l.ToArray());
        }
    }
}
