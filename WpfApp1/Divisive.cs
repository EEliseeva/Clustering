using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    class Divisive
    {
        public static List<List<(int, int)>> Clusters;
        public static List<double> Costs = new List<double>();

        public static double CalcDistancePoints((double, double) center, (int, int) point)
        {
            double sqrDistance = 0;
            sqrDistance += Math.Pow((center.Item1 - point.Item1), 2) + Math.Pow((center.Item2 - point.Item2), 2);
            return sqrDistance;
        }

        public static int[] SetClustering(List<(int, int)> data, List<(double, double)> centers)
        {
            double minDist;
            double distance;
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

        public static List<(double, double)> UpdateClustering(List<(double, double)> centers, List<(int, int)> data)
        {
            int[] clustering = SetClustering(data, centers);
            int dataLen = clustering.Length;
            List<(double, double)> newCenters = new List<(double, double)>();
            int[] clusterCounts = new int[centers.Count];
            int[] sumX = new int[centers.Count];
            int[] sumY = new int[centers.Count];
            for (int i = 0; i < dataLen; i++)
            {
                clusterCounts[clustering[i]]++;
                sumX[clustering[i]] += data[i].Item2;
                sumY[clustering[i]] += data[i].Item1;
            }

            for (int i = 0; i < centers.Count; i++)
            {
                (double, double) newCenter;
                if (clusterCounts[i] > 1)
                {
                    newCenter.Item2 = sumX[i] / clusterCounts[i];
                    newCenter.Item1 = sumY[i] / clusterCounts[i];
                    newCenters.Add(newCenter);
                }
                else if (clusterCounts[i] == 1)
                {
                    newCenters.Add(centers[i]);
                }
            }

            return newCenters;
        }

        public static bool SameCenters(List<(double, double)> prevCenters, List<(double, double)> newCenters)
        {
            if (prevCenters.Count != newCenters.Count) return false;
            for (int i = 0; i < prevCenters.Count; i++)
            {
                if (prevCenters[i] != newCenters[i]) return false;
            }
            return true;
        }

        private static List<(double, double)> ChooseCenters(List<(int, int)> cluster)
        {
            List<(double, double)> centers = new List<(double, double)>();
            double maxDistance = -1;
            int[] bestComb = new int[2];
            for (int i = 0; i < cluster.Count; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    double distance = CalcDistancePoints(cluster[i], cluster[j]);
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        bestComb[0] = i;
                        bestComb[1] = j;
                    }
                }
            }
            centers.Add(cluster[bestComb[0]]);
            centers.Add(cluster[bestComb[1]]);
            return centers;
        }

        public static void DivideClusters(List<(int, int)> cluster)
        {
            List<(double, double)> centers;

            centers = ChooseCenters(cluster);

            List<(double, double)> prevCenters = new List<(double, double)>();
            for (int i = 0; i < centers.Count; i++)
            {
                prevCenters.Add(centers[i]);
            }
            var updatedCenters = UpdateClustering(centers, cluster);
            while (!SameCenters(prevCenters, updatedCenters))
            {
                updatedCenters = UpdateClustering(centers, cluster);
                prevCenters.Clear();
                foreach(var center in updatedCenters) 
                {
                    prevCenters.Add(center);
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
            Console.WriteLine("!!!" + firstNewCluster.Count + " " + secondNewCluster.Count);
            int index = Clusters.IndexOf(cluster);
            Costs.RemoveAt(index);
            Clusters.Remove(cluster);
            (int, int) firstNewClusterCenter = FindCentroid(firstNewCluster);
            (int, int) secondNewClusterCenter = FindCentroid(secondNewCluster);
            if (firstNewClusterCenter.Item1 != -1)
            {
                Clusters.Add(firstNewCluster);
                Costs.Add(CalcClusterCost(firstNewCluster, firstNewClusterCenter));
            }
            if (secondNewClusterCenter.Item1 != -1)
            {
                Clusters.Add(secondNewCluster);
                Costs.Add(CalcClusterCost(secondNewCluster, secondNewClusterCenter));
            }

        }

        public static double CalcClusterCost(List<(int, int)> cluster, (int, int) center)
        {
            double cost = 0;
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
            if (cluster.Count > 0)
            {
                centroid.Item2 = (int)(sumX / cluster.Count);
                centroid.Item1 = (int)(sumY / cluster.Count);
            }
            else
            {
                centroid.Item1 = -1;
                centroid.Item2 = -1;
            }
            return centroid;
        }

        public static void Start(List<(int, int)> data)
        {
            Clusters = new List<List<(int, int)>>(data.Count);
            Random r = new Random();
            double maxCost = -1;
            int maxIndex = -1;
            List<(int, int)> firstCluster = new List<(int, int)>();
            foreach (var point in data)
            {
                firstCluster.Add(point);
            }
            Clusters.Add(firstCluster);
            Costs.Add(CalcClusterCost(firstCluster, FindCentroid(firstCluster)));
            while (Clusters.Count < data.Count)
            {
                for (int i = 0; i < Clusters.Count; i++)
                {
                    if (Costs[i] > maxCost && Clusters[i].Count > 1)
                    {
                        maxIndex = i;
                        maxCost = Costs[i];
                    }
                }
                if (maxCost == 0) break;
                Console.WriteLine(Clusters[maxIndex].Count + " " + maxCost);
                DivideClusters(Clusters[maxIndex]);
                maxCost = -1;
                maxIndex = -1;
            }
            Console.WriteLine();
        }
    }
}
