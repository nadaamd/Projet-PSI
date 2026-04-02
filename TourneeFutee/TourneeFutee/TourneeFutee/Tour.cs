namespace TourneeFutee
{
    // Modélise une tournée dans le cadre du problème du voyageur de commerce
    public class Tour
    {
        // TODO : ajouter tous les attributs que vous jugerez pertinents 
        Graph graph;  
        List<(string source, string destination)> segments;
        float cost;

        // Constructeur 
        public Tour(Graph graph)
        {
            this.graph = graph;
            this.segments = new List<(string source, string destination)>();
            this.cost = 0;
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

    }
}
