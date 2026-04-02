namespace TourneeFutee
{
    // Résout le problème de voyageur de commerce défini par le graphe `graph`
    // en utilisant l'algorithme de Little
    public class Little
    {
        // TODO : ajouter tous les attributs que vous jugerez pertinents 
        Graph graph;
        Matrix costMatrix;
        List<string> cities;

        // Instancie le planificateur en spécifiant le graphe modélisant un problème de voyageur de commerce
        public Little(Graph graph)
        {
            this.graph = graph;
            this.cities = graph.GetVertices();

            int n = cities.Count;
            costMatrix = new Matrix(n, n, float.PositiveInfinity);
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i != j)
                    {
                        try
                        {
                            float weight = graph.GetEdgeWeight(cities[i], cities[j]);
                            costMatrix.SetValue(i, j, weight);
                        }
                        catch (ArgumentException)
                        {
                            // Si il n'y a pas d'arête entre cities[i] et cities[j], on laisse le coût à l'infini
                        }
                    }
                }
            }
        }

        // Trouve la tournée optimale dans le graphe `this.graph`
        // (c'est à dire le cycle hamiltonien de plus faible coût)
        public Tour ComputeOptimalTour()
        {
            // TODO : implémenter
           Tour bestTour=new Tour(graph);
            float minCost=float.PositiveInfinity;
            List<(string, string)> bestSegments= new List<(string, string)>();

            List<int> path=new List<int> { 0};
            bool[] visited=new bool[cities.Count];
            visited[0]=true;

            ComputeOptimalTourRecursive(path, visited,0, ref minCost, ref bestSegments);

            foreach(var seg in bestSegments) 
            {
                float weight = graph.GetEdgeWeight(seg.Item1, seg.Item2);
                bestTour.AddSegment((seg.Item1, seg.Item2), weight);
            }
            return bestTour;
        }

        // --- Méthodes utilitaires réalisant des étapes de l'algorithme de Little


        // Réduit la matrice `m` et revoie la valeur totale de la réduction
        // Après appel à cette méthode, la matrice `m` est *modifiée*.
        public static float ReduceMatrix(Matrix m)
        {
            // TODO : implémenter
            return 0.0f;
        }

        // Renvoie le regret de valeur maximale dans la matrice de coûts `m` sous la forme d'un tuple `(int i, int j, float value)`
        // où `i`, `j`, et `value` contiennent respectivement la ligne, la colonne et la valeur du regret maximale
        public static (int i, int j, float value) GetMaxRegret(Matrix m)
        {
            // TODO : implémenter
            return (0, 0, 0.0f);

        }

        /* Renvoie vrai si le segment `segment` est un trajet parasite, c'est-à-dire s'il ferme prématurément la tournée incluant les trajets contenus dans `includedSegments`
         * Une tournée est incomplète si elle visite un nombre de villes inférieur à `nbCities`
         */
        public static bool IsForbiddenSegment((string source, string destination) segment, List<(string source, string destination)> includedSegments, int nbCities)
        {

            // TODO : implémenter
            return false;   
        }

        // TODO : ajouter toutes les méthodes que vous jugerez pertinentes 
        private void ComputeOptimalTourRecursive(List<int> path, bool[] visited, float currentCost, ref float minCost, ref List<(string, string)> bestSegments)
        {
            if (path.Count == cities.Count)
            {
                float returnCost = costMatrix.GetValue(path[path.Count - 1], 0);

                if (returnCost == float.PositiveInfinity)
                { return; }

                float totalCost = currentCost + returnCost;

                if (totalCost < minCost)
                {
                    minCost = totalCost;
                    bestSegments = new List<(string, string)>();

                    for (int i = 0; i < path.Count - 1; i++)
                    {
                        bestSegments.Add((cities[path[i]], cities[path[i + 1]]));
                    }
                    bestSegments.Add((cities[path[path.Count - 1]], cities[0]));
                }
                return;
            }

            if (currentCost >= minCost)
            { return; }

            for (int i = 0; i < cities.Count; i++)
            {
                if (!visited[i])
                {
                    float cost = costMatrix.GetValue(path[path.Count - 1], i);
                    if (cost == float.PositiveInfinity)
                    { continue; }

                    visited[i] = true;
                    path.Add(i);

                    ComputeOptimalTourRecursive(path, visited, currentCost + cost, ref minCost, ref bestSegments);

                    path.RemoveAt(path.Count - 1);
                    visited[i] = false;
                }
            }

        }
    }
}
