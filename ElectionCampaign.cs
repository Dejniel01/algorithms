using System;
using System.Collections.Generic;
using System.Linq;
using ASD.Graphs;

namespace ASD
{
    public class Lab08 : MarshalByRefObject
    {
        /// <summary>
        /// Znajduje cykl rozpoczynający się w stolicy, który dla wybranych miast,
        /// przez które przechodzi ma największą sumę liczby ludności w tych wybranych
        /// miastach oraz minimalny koszt.
        /// </summary>
        /// <param name="cities">
        /// Graf miast i połączeń między nimi.
        /// Waga krawędzi jest kosztem przejechania między dwoma miastami.
        /// Koszty transportu między miastami są nieujemne.
        /// </param>
        /// <param name="citiesPopulation">Liczba ludności miast</param>
        /// <param name="meetingCosts">
        /// Koszt spotkania w każdym z miast.
        /// Dla części pierwszej koszt spotkania dla każdego miasta wynosi 0.
        /// Dla części drugiej koszty są nieujemne.
        /// </param>
        /// <param name="budget">Budżet do wykorzystania przez kandydata.</param>
        /// <param name="capitalCity">Numer miasta będącego stolicą, z której startuje kandydat.</param>
        /// <param name="path">
        /// Tablica dwuelementowych krotek opisująca ciąg miast, które powinen odwiedzić kandydat.
        /// Pierwszy element krotki to numer miasta do odwiedzenia, a drugi element decyduje czy
        /// w danym mieście będzie organizowane spotkanie wyborcze.
        /// 
        /// Pierwszym miastem na tej liście zawsze będzie stolica (w której można, ale nie trzeba
        /// organizować spotkania).
        /// 
        /// Zakładamy, że po odwiedzeniu ostatniego miasta na liście kandydat wraca do stolicy
        /// (na co musi mu starczyć budżetu i połączenie między tymi miastami musi istnieć).
        /// 
        /// Jeżeli kandydat nie wyjeżdża ze stolicy (stolica jest jedynym miastem, które odwiedzi),
        /// to lista `path` powinna zawierać jedynie jeden element: stolicę (wraz z informacją
        /// czy będzie tam spotkanie czy nie). Nie są wtedy ponoszone żadne koszty podróży.
        /// 
        /// W pierwszym etapie drugi element krotki powinien być zawsze równy `true`.
        /// </param>
        /// <returns>
        /// Liczba mieszkańców, z którymi spotka się kandydat.
        /// </returns>
        public int ComputeElectionCampaignPath(Graph<int> cities, int[] citiesPopulation,
            double[] meetingCosts, double budget, int capitalCity, out (int, bool)[] path)
        {
            path = new (int, bool)[1];
            path[0] = (capitalCity, false);
            int maxCount = -1;
            double maxCost = 0;

            List<int> cycle = new List<int>();
            List<int> maxCycle = null;
            double cost = 0;
            int count = 0;
            bool[] visited = new bool[cities.VertexCount];
            bool[] org = new bool[cities.VertexCount];
            bool[] maxOrg = new bool[cities.VertexCount];

            cycle.Add(capitalCity);
            visited[capitalCity] = true;

            if (meetingCosts[capitalCity] != 0)
                GenerateCycle();

            org[capitalCity] = true;
            cost = meetingCosts[capitalCity];
            count = citiesPopulation[capitalCity];

            GenerateCycle();


            if (maxCycle != null)
            {
                path = new (int, bool)[maxCycle.Count];
                for (int i = 0; i < maxCycle.Count; i++)
                    path[i] = (maxCycle[i], maxOrg[maxCycle[i]]);
            }

            return maxCount;

            void GenerateCycle()
            {
                var next = cities.OutNeighbors(cycle.Last());

                if (cost > budget)
                {
                    return;
                }

                if ((count > maxCount || (count == maxCount && cost < maxCost)) && (cycle.Last() == capitalCity ||
                    (cities.HasEdge(cycle.Last(), capitalCity) && cost + cities.GetEdgeWeight(cycle.Last(), capitalCity) <= budget)))
                {
                    maxCost = cost;
                    maxCount = count;
                    maxCycle = cycle.ToList();
                    maxOrg = org.ToArray();
                }

                foreach (var v in next)
                {
                    if (!visited[v])
                    {
                        cost += cities.GetEdgeWeight(cycle.Last(), v);
                        cycle.Add(v);
                        visited[v] = true;

                        if (meetingCosts[v] != 0)
                            GenerateCycle();

                        org[v] = true;
                        cost += meetingCosts[v];
                        count += citiesPopulation[v];

                        GenerateCycle();

                        cycle.Remove(v);
                        cost -= cities.GetEdgeWeight(cycle.Last(), v);
                        cost -= meetingCosts[v];
                        count -= citiesPopulation[v];
                        visited[v] = false;
                        org[v] = false;
                    }
                }
            }

        }
    }
}


/*
   public int ComputeElectionCampaignPath(Graph<int> cities, int[] citiesPopulation,
            double[] meetingCosts, double budget, int capitalCity, out (int, bool)[] path)
        {
            path = new (int, bool)[1];
            path[0] = (capitalCity, true);
            int maxCount = citiesPopulation[capitalCity];
            double maxCost = 0;

            List<int> cycle = new List<int>();
            List<int> maxCycle = null;
            double cost = 0;
            int count = 0;
            bool[] visited = new bool[cities.VertexCount];

            cycle.Add(capitalCity);
            visited[capitalCity] = true;
            count += citiesPopulation[capitalCity];

            GenerateCycle();

            if (maxCycle != null)
            {
                path = new (int, bool)[maxCycle.Count];
                for (int i = 0; i < maxCycle.Count; i++)
                    path[i] = (maxCycle[i], true);
            }

            return maxCount;

            void GenerateCycle()
            {
                var next = cities.OutNeighbors(cycle.Last());

                if(cost > budget)
                {
                    return;
                }

                if(count > maxCount && cities.HasEdge(cycle.Last(), capitalCity) && cost + cities.GetEdgeWeight(cycle.Last(), capitalCity) <= budget)
                {
                    maxCost = cost;
                    maxCount = count;
                    maxCycle = cycle.ToList();
                }

                if (count == maxCount && cost < maxCost && cities.HasEdge(cycle.Last(), capitalCity) && cost + cities.GetEdgeWeight(cycle.Last(), capitalCity) <= budget)
                {
                    maxCost = cost;
                    maxCount = count;
                    maxCycle = cycle.ToList();
                }

                foreach(var v in next)
                {
                    if(!visited[v])
                    {
                        cost += cities.GetEdgeWeight(cycle.Last(), v);
                        count += citiesPopulation[v];
                        cycle.Add(v);
                        visited[v] = true;
                        
                        GenerateCycle();

                        cost -= cities.GetEdgeWeight(cycle[cycle.Count - 2], cycle.Last());
                        count -= citiesPopulation[cycle.Last()];
                        visited[cycle.Last()] = false;
                        cycle.RemoveAt(cycle.Count - 1);
                    }
                }
            }

        }
 */
