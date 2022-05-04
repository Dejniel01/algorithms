using System;
using System.Linq;
using System.Collections.Generic;
using ASD.Graphs;

namespace Lab10
{
    public class DeliveryPlanner : MarshalByRefObject
    {

        /// <param name="railway">Graf reprezentujący sieć kolejową</param>
        /// <param name="eggDemand">Zapotrzebowanie na jajka na poszczególnyhc stacjach. Zerowy element tej tablicy zawsze jest 0</param>
        /// <param name="truckCapacity">Pojemność wagonu na jajka</param>
        /// <param name="tankEngineRange">Zasięg parowozu</param>
        /// <param name="isRefuelStation">na danym indeksie true, jeśli na danej stacji można uzupelnić węgiel i wodę</param>
        /// <param name="anySolution">Czy znaleźć jakiekolwiek rozwiązanie (true, etap 1), czy najkrótsze (false, etap 2)</param>
        /// <returns>Informację czy istnieje trasa oraz tablicę reprezentującą kolejne wierzchołki w trasie (pierwszy i ostatni element tablicy musi być 0). W przypadku, gdy zwracany jest false, wartość tego pola nie jest sprawdzana, może być null.</returns>
        public (bool routeExists, int[] route) PlanDelivery(Graph<int> railway, int[] eggDemand, int truckCapacity, int tankEngineRange, bool[] isRefuelStation, bool anySolution)
        {
            Stack<int> path = new Stack<int>();
            List<int> foundPath = null;
            bool[] visited = new bool[railway.VertexCount];
            int curTruck = truckCapacity;
            int curTank = tankEngineRange;
            int curVisited = 1;
            int curLength = 0;
            int maxLength = int.MaxValue;

            path.Push(0);

            FindPath(0);

            return (foundPath != null, foundPath?.ToArray());

            void FindPath(int v)
            {
                
                if ((anySolution && foundPath != null) || curTank < 0 || curTruck < 0 || curLength >= maxLength)
                    return;

                if (isRefuelStation[v])
                    curTank = tankEngineRange;

                if (curVisited == railway.VertexCount && v == 0)
                {
                    if (curLength < maxLength)
                    {
                        maxLength = curLength;
                        foundPath = path.ToList();
                    }
                    return;
                }

                if (v == 0)
                    curTruck = truckCapacity;

                foreach (var u in railway.OutNeighbors(v))
                {
                    if (!visited[u])
                    {
                        int prevTruck = curTruck;
                        int prevTank = curTank;
                        int tempLength = railway.GetEdgeWeight(v, u);

                        curTruck -= eggDemand[u];
                        curTank -= tempLength;
                        curLength += tempLength;
                        if (u != 0)
                        {
                            visited[u] = true;
                            curVisited++;
                        }
                        path.Push(u);

                        FindPath(u);

                        curTruck = prevTruck;
                        curTank = prevTank;
                        curLength -= tempLength;
                        if (u != 0)
                        {
                            visited[u] = false;
                            curVisited--;
                        }
                        path.Pop();
                    }
                }
            }
        }
    }
}
