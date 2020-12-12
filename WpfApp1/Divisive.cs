using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    class Divisive
    {
        public static int Size = 501;//10001;
        public static int Offset = 50; //100
        public static int NumOfPoints = 10000; //40000
        public static int RandPoints = 20;

        public static List<List<(int, int)>> Clusters;
        public static List<int> Costs = new List<int>();

        public static List<(int, int)> CreateData()
        {
            int[,] map = new int[Size + 1, Size + 1];
            List<(int, int)> data = new List<(int, int)>(NumOfPoints + RandPoints);
            List<(int, int)> initPoints = new List<(int, int)>(RandPoints);
            Random r = new Random();
            int x, y;
            for (int i = 0; i < RandPoints; i++)
            {
                x = r.Next(Size + 1);
                y = r.Next(Size + 1);
                while (map[y, x] == 1)
                {
                    x = r.Next(Size + 1);
                    y = r.Next(Size + 1);
                }
                map[y, x] = 1;
                (int, int) point;
                point.Item1 = y;
                point.Item2 = x;
                initPoints.Add(point);
                data.Add(point);
            }
            int X_offset, Y_offset, rndInd;
            for (int i = 0; i < NumOfPoints; i++)
            {
                rndInd = r.Next(RandPoints);
                y = initPoints[rndInd].Item1;
                x = initPoints[rndInd].Item2;
                X_offset = r.Next(-Offset, Offset);
                Y_offset = r.Next(-Offset, Offset);
                while (y + Y_offset < 0 || y + Y_offset >= Size || x + X_offset < 0 || x + X_offset >= Size)
                {
                    X_offset = r.Next(-Offset, Offset);
                    Y_offset = r.Next(-Offset, Offset);
                }
                (int, int) point;
                point.Item1 = y + Y_offset;
                point.Item2 = x + X_offset;
                data.Add(point);
            }
            return data;
        }

        public static int CalcDistancePoints((int, int) center, (int, int) point)
        {
            int sqrDistance = 0;
            sqrDistance += (int)(Math.Pow((center.Item1 - point.Item1), 2) + Math.Pow((center.Item2 - point.Item2), 2));
            return sqrDistance;
        }

        public static int[] SetClustering(List<(int, int)> data, List<(int, int)> centers)
        {
            int minDist;
            int distance;
            int minIndex = 0;
            int index = 0;
            int[] clustering = new int[data.Count];

            foreach (var point in data)
            {
                minDist = CalcDistancePoints(centers[0], point);
                for (int i = 0; i < centers.Count; i++)
                {
                    distance = CalcDistancePoints(centers[i], point);
                    if (distance < minDist)
                    {
                        minDist = distance;
                        minIndex = i;
                    }
                }
                clustering[index++] = minIndex;
                minIndex = 0;
            }
            return clustering;
        }

        public static List<(int, int)> UpdateClustering(List<(int, int)> centers, List<(int, int)> data)
        {
            int[] newClustering;
            int[] clustering = SetClustering(data, centers);
            int dataLen = clustering.Length;
            List<(int, int)> newCenters = new List<(int, int)>();
            int[] clasterCounts = new int[centers.Count];
            int[] sumX = new int[centers.Count];
            int[] sumY = new int[centers.Count];
            for (int i = 0; i < dataLen; i++)
            {
                clasterCounts[clustering[i]]++;
                sumX[clustering[i]] += data[i].Item2;
                sumY[clustering[i]] += data[i].Item1;
            }
            for (int i = 0; i < centers.Count; i++)
            {
                (int, int) newCenter;
                if (clasterCounts[i] > 0)
                {
                    newCenter.Item2 = (int)(sumX[i] / clasterCounts[i]);
                    newCenter.Item1 = (int)(sumY[i] / clasterCounts[i]);
                    newCenters.Add(newCenter);
                }
                else
                {
                    newCenters.Add(centers[i]);
                }
            }
            newClustering = SetClustering(data, newCenters);
            return newCenters;
        }

        public static bool SameCenters(List<(int, int)> prevCenters, List<(int, int)> newCenters)
        {
            for (int i = 0; i < prevCenters.Count; i++)
            {
                if (prevCenters[i] != newCenters[i]) return false;
            }
            return true;
        }

        public static void DivideClusters(List<(int, int)> cluster)
        {
            List<(int, int)> centers = new List<(int, int)>();
            Random r = new Random();
            int index;
            for (int i = 0; i < 2; i++)
            {
                index = r.Next(cluster.Count);
                (int, int) center;
                center.Item1 = cluster[index].Item1;
                center.Item2 = cluster[index].Item2;
                centers.Add(center);
            }
            List<(int, int)> prevCenters = new List<(int, int)>();
            for (int i = 0; i < centers.Count; i++)
            {
                prevCenters.Add(centers[i]);
            }
            var updatedCenters = UpdateClustering(centers, cluster);
            while (!SameCenters(prevCenters, updatedCenters))
            {
                updatedCenters = UpdateClustering(centers, cluster);
                prevCenters.Clear();
                for (int i = 0; i < centers.Count; i++)
                {
                    prevCenters.Add(updatedCenters[i]);
                }
            }
            int[] clustering = SetClustering(cluster, updatedCenters);
            List<(int, int)> firstNewCluster = new List<(int, int)>();
            List<(int, int)> secondNewCluster = new List<(int, int)>();
            for (int i = 0; i < cluster.Count; i++)
            {
                if (clustering[i] == 0)
                {
                    firstNewCluster.Add(cluster[i]);
                }
                else secondNewCluster.Add(cluster[i]);
            }
            index = Clusters.IndexOf(cluster);
            Costs.RemoveAt(index);
            Clusters.Remove(cluster);
            Clusters.Add(firstNewCluster);
            Clusters.Add(secondNewCluster);
            (int, int) firstNewClusterCenter = FindCentroid(firstNewCluster);
            (int, int) secondNewClusterCenter = FindCentroid(secondNewCluster);
            Costs.Add(CalcClusterCost(firstNewCluster, firstNewClusterCenter));
            Costs.Add(CalcClusterCost(secondNewCluster, secondNewClusterCenter));

        }

        public static int CalcClusterCost(List<(int, int)> cluster, (int, int) center)
        {
            int cost = 0;
            foreach(var point in cluster)
            {
                cost += CalcDistancePoints(point, center);
            }
            cost /= cluster.Count;
            return cost;
        }

        public static (int, int) FindCentroid(List<(int, int)> cluster)
        {
            (int, int) centroid;
            int sumX = 0;
            int sumY = 0;
            for (int i = 0; i < cluster.Count; i++)
            {
                sumX += cluster[i].Item2;
                sumY += cluster[i].Item1;
            }
            centroid.Item2 = (int)(sumX / cluster.Count);
            centroid.Item1 = (int)(sumY / cluster.Count);
            return centroid;
        }

        public static List<List<(int, int)>> Start()
        {
            Clusters = new List<List<(int, int)>>(NumOfPoints + RandPoints);
            Random r = new Random();
            List<(int, int)> data = CreateData();
            int numberOfClust = 5;
            int maxCost = -1;
            int maxIndex = -1;
            List<(int, int)> firstCluster = new List<(int, int)>();
            foreach (var point in data)
            {
                firstCluster.Add(point);
            }
            Clusters.Add(firstCluster);
            Costs.Add(CalcClusterCost(firstCluster, FindCentroid(firstCluster)));
            while (Clusters.Count < numberOfClust)
            {
                for (int i = 0; i < Clusters.Count; i++)
                {
                    if (Costs[i] > maxCost)
                    {
                        maxIndex = i;
                        maxCost = Costs[i];
                    }
                }
                DivideClusters(Clusters[maxIndex]);
                Console.WriteLine(maxCost);
                maxCost = -1;
                maxIndex = -1;
            }
            return Clusters;
        }
    }
}
