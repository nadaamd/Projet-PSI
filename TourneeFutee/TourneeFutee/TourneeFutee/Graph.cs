using System.Xml.Linq;

namespace TourneeFutee
{
    public class Graph
    {

        // TODO : ajouter tous les attributs que vous jugerez pertinents 

        Matrix adjacencyMatrix;
        Dictionary<string, int> vertexIndices;
        Dictionary<string, float> vertexValues;
        bool directed;
        float noEdgeValue;
     
        // --- Construction du graphe ---

        // Contruit un graphe (`directed`=true => orienté)
        // La valeur `noEdgeValue` est le poids modélisant l'absence d'un arc (0 par défaut)
        public Graph(bool directed, float noEdgeValue = 0)
        {
            // TODO : implémenter
            this.directed = directed;
            this.noEdgeValue = noEdgeValue;
            adjacencyMatrix = new Matrix(0,0,noEdgeValue);
            vertexIndices = new Dictionary<string, int>();
            vertexValues = new Dictionary<string, float>();

        }


        // --- Propriétés ---

        // Propriété : ordre du graphe
        // Lecture seule
        public int Order
        {
            get { return this.vertexIndices.Count; }   // TODO : implémenter
                                         // pas de set
        }

        // Propriété : graphe orienté ou non
        // Lecture seule
        public bool Directed
        {
            get { return this.directed; }    // TODO : implémenter
                                             // pas de set
        }


        // --- Gestion des sommets ---

        // Ajoute le sommet de nom `name` et de valeur `value` (0 par défaut) dans le graphe
        // Lève une ArgumentException s'il existe déjà un sommet avec le même nom dans le graphe
        public void AddVertex(string name, float value = 0)
        {
            // TODO : implémenter
            if (vertexIndices.ContainsKey(name))
            {
                throw new ArgumentException("Un sommet avec ce nom existe deja.");
            }
            int newIndex=vertexIndices.Count;

            vertexIndices.Add(name, newIndex);
            vertexValues.Add(name, value);

            adjacencyMatrix.AddRow(newIndex);
            adjacencyMatrix.AddColumn(newIndex);
        }


        // Supprime le sommet de nom `name` du graphe (et tous les arcs associés)
        // Lève une ArgumentException si le sommet n'a pas été trouvé dans le graphe
        public void RemoveVertex(string name)
        {
            if (!vertexIndices.ContainsKey(name))
            {
                throw new ArgumentException("Un sommet avec ce nom n'existe pas.");
            }
            int newIndex = vertexIndices[name];

           

            adjacencyMatrix.RemoveColumn(newIndex);
            adjacencyMatrix.RemoveRow(newIndex);

            vertexValues.Remove(name);
            vertexIndices.Remove(name);

            var keys = vertexIndices.Keys.ToList();
            foreach (var k in keys)
            {
                if (vertexIndices[k] > newIndex)
                    vertexIndices[k]--;
            }

        }
        

        // Renvoie la valeur du sommet de nom `name`
        // Lève une ArgumentException si le sommet n'a pas été trouvé dans le graphe
        public float GetVertexValue(string name)
        {
            // TODO : implémenter
            if (!vertexIndices.ContainsKey(name))
            {
                throw new ArgumentException("Le sommet n'a pas été trouvé dans le graphe.");
            }

            return vertexValues[name];
        }

        // Affecte la valeur du sommet de nom `name` à `value`
        // Lève une ArgumentException si le sommet n'a pas été trouvé dans le graphe
        public void SetVertexValue(string name, float value)
        {
            // TODO : implémenter
            if (!vertexIndices.ContainsKey(name))
            {
                throw new ArgumentException("Le sommet n'a pas été trouvé dans le graphe.");
            }

            vertexValues[name] = value ;
        }


        // Renvoie la liste des noms des voisins du sommet de nom `vertexName`
        // (si ce sommet n'a pas de voisins, la liste sera vide)
        // Lève une ArgumentException si le sommet n'a pas été trouvé dans le graphe
        public List<string> GetNeighbors(string vertexName)
        {

            if (!vertexIndices.ContainsKey(vertexName))
            {
                throw new ArgumentException("Le sommet n'a pas été trouvé dans le graphe.");
            }
            List<string> neighborNames = new List<string>();

            int vertexIndex = vertexIndices[vertexName];
         
            foreach(var k in vertexIndices)
            {
                string othername = k.Key;
                int otherindex = k.Value;

                if (adjacencyMatrix.GetValue(vertexIndex, otherindex)!= noEdgeValue)
                {
                    neighborNames.Add(othername);
                }
            }

            return neighborNames;
        }

        // --- Gestion des arcs ---

        /* Ajoute un arc allant du sommet nommé `sourceName` au sommet nommé `destinationName`, avec le poids `weight` (1 par défaut)
         * Si le graphe n'est pas orienté, ajoute aussi l'arc inverse, avec le même poids
         * Lève une ArgumentException dans les cas suivants :
         * - un des sommets n'a pas été trouvé dans le graphe (source et/ou destination)
         * - il existe déjà un arc avec ces extrémités
         */
        public void AddEdge(string sourceName, string destinationName, float weight = 1)
        {
            if(!vertexIndices.ContainsKey(sourceName)|| !vertexIndices.ContainsKey(destinationName)){
                throw new ArgumentException("Un ou les deux sommets n'exsitent pas dans le graphe");
            }
            int srcIndex = vertexIndices[sourceName];
            int dstIndex = vertexIndices[destinationName];

            if (adjacencyMatrix.GetValue(srcIndex, dstIndex)!= noEdgeValue)
            {
                throw new ArgumentException("L'arc existe déjà");
            }
            adjacencyMatrix.SetValue(srcIndex, dstIndex, weight);

            if (!directed)
            {
                adjacencyMatrix.SetValue(dstIndex,srcIndex, weight);
            }
        }

        /* Supprime l'arc allant du sommet nommé `sourceName` au sommet nommé `destinationName` du graphe
         * Si le graphe n'est pas orienté, supprime aussi l'arc inverse
         * Lève une ArgumentException dans les cas suivants :
         * - un des sommets n'a pas été trouvé dans le graphe (source et/ou destination)
         * - l'arc n'existe pas
         */
        public void RemoveEdge(string sourceName, string destinationName)
        {
            // TODO : implémenter


            if (!vertexIndices.ContainsKey(sourceName) || !vertexIndices.ContainsKey(destinationName))
                throw new ArgumentException();

            int a = vertexIndices[sourceName];
            int b = vertexIndices[destinationName];

            if (adjacencyMatrix.GetValue(a, b) == noEdgeValue)
                throw new ArgumentException();

            adjacencyMatrix.SetValue(a, b, noEdgeValue);

            if (!directed)
                adjacencyMatrix.SetValue(b, a, noEdgeValue);
        }

        

        /* Renvoie le poids de l'arc allant du sommet nommé `sourceName` au sommet nommé `destinationName`
         * Si le graphe n'est pas orienté, GetEdgeWeight(A, B) = GetEdgeWeight(B, A) 
         * Lève une ArgumentException dans les cas suivants :
         * - un des sommets n'a pas été trouvé dans le graphe (source et/ou destination)
         * - l'arc n'existe pas
         */
        public float GetEdgeWeight(string sourceName, string destinationName)
        {
            // TODO : implémenter
            if (!vertexIndices.ContainsKey(sourceName) || !vertexIndices.ContainsKey(destinationName))
                throw new ArgumentException();

            int a = vertexIndices[sourceName];
            int b = vertexIndices[destinationName];

            float w = adjacencyMatrix.GetValue(a, b);

            if (w == noEdgeValue)
                throw new ArgumentException();

            return w;

        }

        /* Affecte le poids l'arc allant du sommet nommé `sourceName` au sommet nommé `destinationName` à `weight` 
         * Si le graphe n'est pas orienté, affecte le même poids à l'arc inverse
         * Lève une ArgumentException si un des sommets n'a pas été trouvé dans le graphe (source et/ou destination)
         */
        public void SetEdgeWeight(string sourceName, string destinationName, float weight)
        {
            // TODO : implémenter

            if (!vertexIndices.ContainsKey(sourceName) || !vertexIndices.ContainsKey(destinationName))
                throw new ArgumentException();

            int a = vertexIndices[sourceName];
            int b = vertexIndices[destinationName];

            if (adjacencyMatrix.GetValue(a, b) == noEdgeValue)
                throw new ArgumentException();

            adjacencyMatrix.SetValue(a, b, weight);

            if (!directed)
                adjacencyMatrix.SetValue(b, a, weight);


        }

        // TODO : ajouter toutes les méthodes que vous jugerez pertinentes 

    }


}
