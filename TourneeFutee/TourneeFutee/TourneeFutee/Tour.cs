namespace TourneeFutee
{
    // Modélise une tournée dans le cadre du problème du voyageur de commerce
    public class Tour
    {
        // TODO : ajouter tous les attributs que vous jugerez pertinents 
        Graph graph;  
        List<(string source, string destination)> segments;
        float cost;
        List<string> vertices;

        // Constructeur 
        public Tour(Graph graph)
        {
            this.graph = graph;
            this.segments = new List<(string source, string destination)>();
            this.cost = 0;
        }

        public Tour(List<string> vertices, float totalCost)
        {
            this.graph = null;
            this.segments = new List<(string source, string destination)>();
            this.vertices = new List<string>(vertices);
            this.cost = totalCost;

            for (int i = 0; i < vertices.Count - 1; i++)
            {
                segments.Add((vertices[i], vertices[i + 1]));
            }
        }

        // propriétés


        // Coût total de la tournée
        public float Cost
        {
            get { return cost; }    
        }

        // Nombre de trajets dans la tournée
        public int NbSegments
        {
            get { return segments.Count; }    
        }

        public float TotalCost
        {
            get { return cost; }
        }

        public List<string> Vertices
        {
            get
            {
                List<string> l = new List<string>();

                if (segments.Count == 0)
                    return l;

                l.Add(segments[0].source);

                foreach (var s in segments)
                {
                    l.Add(s.destination);
                }

                return l;
            }
        }

        // Renvoie vrai si la tournée contient le trajet `source`->`destination`
        public bool ContainsSegment((string source, string destination) segment)
        {
            return segments.Contains(segment);    
        }

        public void AddSegment((string source, string destination) segment, float weight)
        {
            segments.Add(segment);
            cost += weight;
        }

        // Affiche les informations sur la tournée : coût total et trajets
        public void Print()
        {
            Console.WriteLine($"Coût total de la tournée : {cost}");
            Console.WriteLine("Trajets :");
            foreach (var segment in segments)
            {
                Console.WriteLine($"{segment.source} -> {segment.destination}");
            }
            Console.WriteLine();
        }

        // TODO : ajouter toutes les méthodes que vous jugerez pertinentes 

        // Ajoute un segment à la tournée et met à jour le coût total
        public void AddSegment(string source, string destination)
        {
            segments.Add((source, destination));
            cost += graph.GetEdgeWeight(source, destination);
        }

    }
}
