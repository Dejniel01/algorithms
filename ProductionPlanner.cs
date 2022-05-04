using System;
using System.Linq;
using ASD.Graphs;

namespace ASD
{
    public class ProductionPlanner : MarshalByRefObject
    {
        /// <summary>
        /// Flaga pozwalająca na włączenie wypisywania szczegółów skonstruowanego planu na konsolę.
        /// Wartość <code>true</code> spoeoduje wypisanie planu.
        /// </summary>
        public bool ShowDebug { get; } = false;

        /// <summary>
        /// Część 1. zadania - zaplanowanie produkcji telewizorów dla pojedynczego kontrahenta.
        /// </summary>
        /// <remarks>
        /// Do przeprowadzenia testów wyznaczających maksymalną produkcję i zysk wymagane jest jedynie zwrócenie obiektu <see cref="PlanData"/>.
        /// Testy weryfikujące plan wymagają przypisania tablicy z planem do parametru wyjściowego <see cref="weeklyPlan"/>.
        /// </remarks>
        /// <param name="production">
        /// Tablica obiektów zawierających informacje o produkcji fabryki w kolejnych tygodniach.
        /// Wartości pola <see cref="PlanData.Quantity"/> oznaczają limit produkcji w danym tygodniu,
        /// a pola <see cref="PlanData.Value"/> - koszt produkcji jednej sztuki.
        /// </param>
        /// <param name="sales">
        /// Tablica obiektów zawierających informacje o sprzedaży w kolejnych tygodniach.
        /// Wartości pola <see cref="PlanData.Quantity"/> oznaczają maksymalną sprzedaż w danym tygodniu,
        /// a pola <see cref="PlanData.Value"/> - cenę sprzedaży jednej sztuki.
        /// </param>
        /// <param name="storageInfo">
        /// Obiekt zawierający informacje o magazynie.
        /// Wartość pola <see cref="PlanData.Quantity"/> oznacza pojemność magazynu,
        /// a pola <see cref="PlanData.Value"/> - koszt przechowania jednego telewizora w magazynie przez jeden tydzień.
        /// </param>
        /// <param name="weeklyPlan">
        /// Parametr wyjściowy, przez który powinien zostać zwrócony szczegółowy plan sprzedaży.
        /// </param>
        /// <returns>
        /// Obiekt <see cref="PlanData"/> opisujący wyznaczony plan.
        /// W polu <see cref="PlanData.Quantity"/> powinna znaleźć się maksymalna liczba wyprodukowanych telewizorów,
        /// a w polu <see cref="PlanData.Value"/> - wyznaczony maksymalny zysk fabryki.
        /// </returns>
        public PlanData CreateSimplePlan(PlanData[] production, PlanData[] sales, PlanData storageInfo,
            out SimpleWeeklyPlan[] weeklyPlan)
        {
            const int size_const = 4;
            NetworkWithCosts<int, double> n = new NetworkWithCosts<int, double>(size_const * production.Length + 2);
            int s = size_const * production.Length;
            int t = size_const * production.Length + 1;

            for (int i = 0; i < production.Length; i++)
            {
                n.AddEdge(size_const * i, size_const * i + 1, sales[i].Quantity, 0.0);
                n.AddEdge(size_const * i, size_const * i + 2, storageInfo.Quantity, 0.0);
                n.AddEdge(size_const * i + 2, size_const * i + 3, storageInfo.Quantity, 0.0);
                n.AddEdge(s, size_const * i, production[i].Quantity, production[i].Value);
                n.AddEdge(size_const * i + 1, t, sales[i].Quantity, -sales[i].Value);
                if (i > 0)
                {
                    n.AddEdge(size_const * (i - 1) + 3, size_const * i + 1, storageInfo.Quantity, storageInfo.Value);
                    n.AddEdge(size_const * (i - 1) + 3, size_const * i + 2, storageInfo.Quantity, storageInfo.Value);
                }
            }

            var (flowValue, flowCost, f) = Flows.MinCostMaxFlow(n, s, t);

            weeklyPlan = new SimpleWeeklyPlan[production.Length];
            for (int i = 0; i < production.Length; i++)
            {
                weeklyPlan[i].UnitsProduced = f.HasEdge(s, size_const * i) ? f.GetEdgeWeight(s, size_const * i) : 0;
                weeklyPlan[i].UnitsSold = f.HasEdge(size_const * i + 1, t) ? f.GetEdgeWeight(size_const * i + 1, t) : 0;
                weeklyPlan[i].UnitsStored = f.HasEdge(size_const * i + 2, size_const * i + 3) ? f.GetEdgeWeight(size_const * i + 2, size_const * i + 3) : 0;
            }

            return new PlanData
            {
                Value = -flowCost,
                Quantity = flowValue
            };
        }

        /// <summary>
        /// Część 2. zadania - zaplanowanie produkcji telewizorów dla wielu kontrahentów.
        /// </summary>
        /// <remarks>
        /// Do przeprowadzenia testów wyznaczających produkcję dającą maksymalny zysk wymagane jest jedynie zwrócenie obiektu <see cref="PlanData"/>.
        /// Testy weryfikujące plan wymagają przypisania tablicy z planem do parametru wyjściowego <see cref="weeklyPlan"/>.
        /// </remarks>
        /// <param name="production">
        /// Tablica obiektów zawierających informacje o produkcji fabryki w kolejnych tygodniach.
        /// Wartość pola <see cref="PlanData.Quantity"/> oznacza limit produkcji w danym tygodniu,
        /// a pola <see cref="PlanData.Value"/> - koszt produkcji jednej sztuki.
        /// </param>
        /// <param name="sales">
        /// Dwuwymiarowa tablica obiektów zawierających informacje o sprzedaży w kolejnych tygodniach.
        /// Pierwszy wymiar tablicy jest równy liczbie kontrahentów, zaś drugi - liczbie tygodni w planie.
        /// Wartości pola <see cref="PlanData.Quantity"/> oznaczają maksymalną sprzedaż w danym tygodniu,
        /// a pola <see cref="PlanData.Value"/> - cenę sprzedaży jednej sztuki.
        /// Każdy wiersz tablicy odpowiada jednemu kontrachentowi.
        /// </param>
        /// <param name="storageInfo">
        /// Obiekt zawierający informacje o magazynie.
        /// Wartość pola <see cref="PlanData.Quantity"/> oznacza pojemność magazynu,
        /// a pola <see cref="PlanData.Value"/> - koszt przechowania jednego telewizora w magazynie przez jeden tydzień.
        /// </param>
        /// <param name="weeklyPlan">
        /// Parametr wyjściowy, przez który powinien zostać zwrócony szczegółowy plan sprzedaży.
        /// </param>
        /// <returns>
        /// Obiekt <see cref="PlanData"/> opisujący wyznaczony plan.
        /// W polu <see cref="PlanData.Quantity"/> powinna znaleźć się optymalna liczba wyprodukowanych telewizorów,
        /// a w polu <see cref="PlanData.Value"/> - wyznaczony maksymalny zysk fabryki.
        /// </returns>
        public PlanData CreateComplexPlan(PlanData[] production, PlanData[,] sales, PlanData storageInfo,
            out WeeklyPlan[] weeklyPlan)
        {
            const int size_const = 5;
            int weekDivider = size_const + sales.GetLength(0);
            NetworkWithCosts<int, double> n = new NetworkWithCosts<int, double>(weekDivider * production.Length + 2);
            int s = weekDivider * production.Length;
            int t = weekDivider * production.Length + 1;

            for (int i = 0; i < production.Length; i++)
            {
                n.AddEdge(s, weekDivider * i, production[i].Quantity, 0);
                n.AddEdge(weekDivider * i, t, production[i].Quantity, 0);
                n.AddEdge(weekDivider * i, weekDivider * i + 1, production[i].Quantity, production[i].Value);
                n.AddEdge(weekDivider * i + 1, weekDivider * i + 4, production[i].Quantity + storageInfo.Quantity, 0);
                n.AddEdge(weekDivider * i + 1, weekDivider * i + 2, storageInfo.Quantity, 0);
                n.AddEdge(weekDivider * i + 2, weekDivider * i + 3, storageInfo.Quantity, 0);
                if (i > 0)
                {
                    n.AddEdge(weekDivider * (i - 1) + 3, weekDivider * i + 4, storageInfo.Quantity, storageInfo.Value);
                    n.AddEdge(weekDivider * (i - 1) + 3, weekDivider * i + 2, storageInfo.Quantity, storageInfo.Value);
                }
                for (int j = 0; j < sales.GetLength(0); j++)
                {
                    n.AddEdge(weekDivider * i + 4, weekDivider * i + 4 + j + 1, sales[j, i].Quantity, 0);
                    n.AddEdge(weekDivider * i + 4 + j + 1, t, sales[j, i].Quantity, -sales[j, i].Value);

                }
            }

            var (flowValue, flowCost, f) = Flows.MinCostMaxFlow(n, s, t);

            int productionCount = 0;
            weeklyPlan = new WeeklyPlan[production.Length];

            for (int i = 0; i < production.Length; i++)
            {
                if (f.HasEdge(weekDivider * i, weekDivider * i + 1))
                {
                    productionCount += f.GetEdgeWeight(weekDivider * i, weekDivider * i + 1);
                    weeklyPlan[i].UnitsProduced = f.GetEdgeWeight(weekDivider * i, weekDivider * i + 1);
                }
                else
                    weeklyPlan[i].UnitsProduced = 0;

                weeklyPlan[i].UnitsStored = f.HasEdge(weekDivider * i + 2, weekDivider * i + 3) ?
                    f.GetEdgeWeight(weekDivider * i + 2, weekDivider * i + 3) : 0;

                weeklyPlan[i].UnitsSold = new int[sales.GetLength(0)];
                for (int j = 0; j < sales.GetLength(0); j++)
                    weeklyPlan[i].UnitsSold[j] = f.HasEdge(weekDivider * i + 4 + j + 1, t) ?
                        f.GetEdgeWeight(weekDivider * i + 4 + j + 1, t) : 0;
            }


            return new PlanData
            {
                Value = -flowCost,
                Quantity = productionCount,
            };
        }
    }
}