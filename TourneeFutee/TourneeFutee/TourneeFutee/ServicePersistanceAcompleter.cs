using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace TourneeFutee
{
    /// <summary>
    /// Service de persistance permettant de sauvegarder et charger
    /// des graphes et des tournées dans une base de données MySQL.
    /// </summary>
    public class ServicePersistance
    {
        // ─────────────────────────────────────────────────────────────────────
        // Attributs privés
        // ─────────────────────────────────────────────────────────────────────

        private readonly string _connectionString;

        // TODO : si vous avez besoin de maintenir une connexion ouverte,
        //        ajoutez un attribut MySqlConnection ici.

        // ─────────────────────────────────────────────────────────────────────
        // Constructeur
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Instancie un service de persistance et se connecte automatiquement
        /// à la base de données <paramref name="dbname"/> sur le serveur
        /// à l'adresse IP <paramref name="serverIp"/>.
        /// Les identifiants sont définis par <paramref name="user"/> (utilisateur)
        /// et <paramref name="pwd"/> (mot de passe).
        /// </summary>
        /// <param name="serverIp">Adresse IP du serveur MySQL.</param>
        /// <param name="dbname">Nom de la base de données.</param>
        /// <param name="user">Nom d'utilisateur.</param>
        /// <param name="pwd">Mot de passe.</param>
        /// <exception cref="Exception">Levée si la connexion échoue.</exception>
        public ServicePersistance(string serverIp, string dbname, string user, string pwd)
        {
            // TODO : initialiser et ouvrir la connexion à la base de données
            // Exemple :
            _connectionString = $"server={serverIp};database={dbname};uid={user};pwd={pwd};";

            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
            }

            // TODO : tester la connexion dès la construction
            //        (ouvrir puis fermer une connexion pour valider les paramètres)
        }

        // ─────────────────────────────────────────────────────────────────────
        // Méthodes publiques
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Sauvegarde le graphe <paramref name="g"/> en base de données
        /// (sommets et arcs inclus) et renvoie son identifiant.
        /// </summary>
        /// <param name="g">Le graphe à sauvegarder.</param>
        /// <returns>Identifiant du graphe en base de données (AUTO_INCREMENT).</returns>
        public uint SaveGraph(Graph g)
        {
            // TODO : implémenter la sauvegarde du graphe
            //
            // Ordre recommandé :
            //   1. INSERT dans la table Graphe -> récupérer l'id avec LAST_INSERT_ID()
            //   2. Pour chaque sommet de g : INSERT dans Sommet (valeur + graphe_id)
            //      -> conserver la correspondance sommet C# <-> id BdD
            //   3. Pour chaque arc de la matrice d'adjacence (poids != +inf) :
            //      INSERT dans Arc (sommet_source_id, sommet_dest_id, poids, graphe_id)
            //
            // Exemple pour récupérer l'id généré :
            //   uint id = Convert.ToUInt32(cmd.ExecuteScalar());


            using (var conn = OpenConnection())
            {
                uint graphId;

                var cmd = new MySqlCommand(
           "INSERT INTO Graphe(est_oriente, nb_sommets, no_edge_value) VALUES (@o,@n,@v); SELECT LAST_INSERT_ID();",
           conn);


                cmd.Parameters.AddWithValue("@o", g.Directed ? 1 : 0);
                cmd.Parameters.AddWithValue("@n", g.Order);
                cmd.Parameters.AddWithValue("@v", g.NoEdgeValue);

                graphId = Convert.ToUInt32(cmd.ExecuteScalar());


                var vertices = g.GetVertices();
                Dictionary<string, uint> ids = new Dictionary<string, uint>();



                for (int i = 0; i < vertices.Count; i++)
                {
                    string name = vertices[i];

                    var cmd2 = new MySqlCommand("INSERT INTO Sommet(graphe_id, nom, valeur, indice) VALUES (@g,@n,@v,@i); SELECT LAST_INSERT_ID();", conn);


                    cmd2.Parameters.AddWithValue("@g", graphId);
                    cmd2.Parameters.AddWithValue("@n", name);
                    cmd2.Parameters.AddWithValue("@v", g.GetVertexValue(name));
                    cmd2.Parameters.AddWithValue("@i", i);

                    uint id = Convert.ToUInt32(cmd2.ExecuteScalar());
                    ids[name] = id;

                }

                for (int i = 0; i < vertices.Count; i++)
                {
                    for (int j = 0; j < vertices.Count; j++)
                    {
                        if (i == j) continue;

                        string s = vertices[i];
                        string d = vertices[j];

                        try
                        {
                            float w = g.GetEdgeWeight(s, d);

                            var cmd3 = new MySqlCommand(
                                "INSERT INTO Arc(graphe_id, sommet_source, sommet_dest, poids) VALUES (@g,@s,@d,@p);",
                                conn);

                            cmd3.Parameters.AddWithValue("@g", graphId);
                            cmd3.Parameters.AddWithValue("@s", ids[s]);
                            cmd3.Parameters.AddWithValue("@d", ids[d]);
                            cmd3.Parameters.AddWithValue("@p", w);

                            cmd3.ExecuteNonQuery();
                        }
                        catch
                        {
                            // pas d'arc → on ignore
                        }
                    }
                }

                return graphId;
            }
        }
    
        

        /// <summary>
        /// Charge depuis la base de données le graphe identifié par <paramref name="id"/>
        /// et renvoie une instance de la classe <see cref="Graph"/>.
        /// </summary>
        /// <param name="id">Identifiant du graphe à charger.</param>
        /// <returns>Instance de <see cref="Graph"/> reconstituée.</returns>
        public Graph LoadGraph(uint id)
        {
            // TODO : implémenter le chargement du graphe
            //
            // Ordre recommandé :
            //   1. SELECT dans Graphe WHERE id = @id -> récupérer IsOriented, etc.
            //   2. SELECT dans Sommet WHERE graphe_id = @id -> reconstruire les sommets
            //      (respecter l'ordre d'insertion pour que les indices de la matrice
            //       correspondent à ceux sauvegardés)
            //   3. SELECT dans Arc WHERE graphe_id = @id -> reconstruire la matrice
            //      d'adjacence en utilisant les correspondances sommet_id <-> indice

          
            using (var conn = OpenConnection())
            {
                bool estOriente;
                float noEdgeValue;

                var cmd1 = new MySqlCommand(
                    "SELECT est_oriente, no_edge_value FROM Graphe WHERE id = @id;",
                    conn);

                cmd1.Parameters.AddWithValue("@id", id);

                using (var reader = cmd1.ExecuteReader())
                {
                    if (!reader.Read())
                        throw new Exception("Graphe introuvable");

                    estOriente = Convert.ToBoolean(reader["est_oriente"]);
                    noEdgeValue = Convert.ToSingle(reader["no_edge_value"]);
                }

                Graph graph = new Graph(estOriente, noEdgeValue);

                Dictionary<uint, string> ids = new Dictionary<uint, string>();

                var cmd2 = new MySqlCommand(
                    "SELECT id, nom, valeur FROM Sommet WHERE graphe_id = @g ORDER BY indice;",
                    conn);

                cmd2.Parameters.AddWithValue("@g", id);

                using (var reader = cmd2.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        uint sommetId = Convert.ToUInt32(reader["id"]);
                        string nom = reader["nom"].ToString();
                        float valeur = Convert.ToSingle(reader["valeur"]);

                        graph.AddVertex(nom, valeur);
                        ids[sommetId] = nom;
                    }
                }

                var cmd3 = new MySqlCommand(
                    "SELECT sommet_source, sommet_dest, poids FROM Arc WHERE graphe_id = @graph;",
                    conn);

                cmd3.Parameters.AddWithValue("@graph", id);

                using (var reader = cmd3.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        uint sourceId = Convert.ToUInt32(reader["sommet_source"]);
                        uint destId = Convert.ToUInt32(reader["sommet_dest"]);
                        float poids = Convert.ToSingle(reader["poids"]);

                        if (!estOriente && sourceId > destId)
                            continue;


                        graph.AddEdge(ids[sourceId], ids[destId], poids);
                    }
                }

                return graph;
            }
        
        }

        /// <summary>
        /// Sauvegarde la tournée <paramref name="t"/> (effectuée dans le graphe
        /// identifié par <paramref name="graphId"/>) en base de données
        /// et renvoie son identifiant.
        /// </summary>
        /// <param name="graphId">Identifiant BdD du graphe dans lequel la tournée a été calculée.</param>
        /// <param name="t">La tournée à sauvegarder.</param>
        /// <returns>Identifiant de la tournée en base de données (AUTO_INCREMENT).</returns>
        public uint SaveTour(uint graphId, Tour t)
        {
            // TODO : implémenter la sauvegarde de la tournée
            //
            // Ordre recommandé :
            //   1. INSERT dans Tournee (cout_total, graphe_id) -> récupérer l'id
            //   2. Pour chaque sommet de la séquence (avec son numéro d'ordre) :
            //      INSERT dans EtapeTournee (tournee_id, numero_ordre, sommet_id)
            //
            // Attention : conserver l'ordre des étapes est essentiel pour
            //             pouvoir reconstruire la tournée fidèlement au chargement.

        
            using (var connection = OpenConnection())
            {
                uint tourId;

                //on insére la tournée dans la table Tournee et on récupère l'id généré automatiquement
                var insertTourCommand = new MySqlCommand(
                    "INSERT INTO Tournee(graphe_id, cout_total) VALUES (@graphId, @totalCost); SELECT LAST_INSERT_ID();",
                    connection);

                insertTourCommand.Parameters.AddWithValue("@graphId", graphId);
                insertTourCommand.Parameters.AddWithValue("@totalCost", t.TotalCost);

                tourId = Convert.ToUInt32(insertTourCommand.ExecuteScalar());

                //  on récupére tous les sommets du graphe 
                Dictionary<string, uint> vertexNameToId = new Dictionary<string, uint>();

                var selectVerticesCommand = new MySqlCommand(
                    "SELECT id, nom FROM Sommet WHERE graphe_id = @graphId;",
                    connection);

                selectVerticesCommand.Parameters.AddWithValue("@graphId", graphId);

                using (var reader = selectVerticesCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        uint vertexId = Convert.ToUInt32(reader["id"]);
                        string vertexName = reader["nom"].ToString();
                        vertexNameToId[vertexName] = vertexId;
                    }
                }
                //on insére chaque étape de la tournée dans EtapeTournee et on garde l'ordre grâce à numero_ordre
                for (int i = 0; i < t.Vertices.Count; i++)
                {
                    string vertexName = t.Vertices[i];

                    var insertStepCommand = new MySqlCommand(
                        "INSERT INTO EtapeTournee(tournee_id, numero_ordre, sommet_id) VALUES (@tourId, @orderIndex, @vertexId);",
                        connection);

                    insertStepCommand.Parameters.AddWithValue("@tourId", tourId);
                    insertStepCommand.Parameters.AddWithValue("@orderIndex", i);
                    insertStepCommand.Parameters.AddWithValue("@vertexId", vertexNameToId[vertexName]);

                    insertStepCommand.ExecuteNonQuery();
                }

                return tourId;
            }
        }
        

        /// <summary>
        /// Charge depuis la base de données la tournée identifiée par <paramref name="id"/>
        /// et renvoie une instance de la classe <see cref="Tour"/>.
        /// </summary>
        /// <param name="id">Identifiant de la tournée à charger.</param>
        /// <returns>Instance de <see cref="Tour"/> reconstituée.</returns>
        public Tour LoadTour(uint id)
        {
            // TODO : implémenter le chargement de la tournée
            //
            // Ordre recommandé :
            //   1. SELECT dans Tournee WHERE id = @id -> récupérer cout_total et graphe_id
            //   2. SELECT dans EtapeTournee JOIN Sommet WHERE tournee_id = @id
            //      ORDER BY numero_ordre -> reconstruire la séquence ordonnée de sommets
            //   3. Construire et retourner l'instance Tour


            using (var connection = OpenConnection())
            {
                float totalCost;

                // on récupére le coût total
                var selectTourCommand = new MySqlCommand(
                    "SELECT cout_total FROM Tournee WHERE id = @tourId;",
                    connection);

                selectTourCommand.Parameters.AddWithValue("@tourId", id);

                using (var reader = selectTourCommand.ExecuteReader())
                {
                    if (!reader.Read())
                        throw new Exception("Tournée introuvable");

                    totalCost = Convert.ToSingle(reader["cout_total"]);
                }

                // on récupère la liste ordonée des sommets
                List<string> vertexSequence = new List<string>();

                var selectStepsCommand = new MySqlCommand(
                    "SELECT s.nom FROM EtapeTournee e JOIN Sommet s ON e.sommet_id = s.id WHERE e.tournee_id = @tourId ORDER BY e.numero_ordre;",
                    connection);

                selectStepsCommand.Parameters.AddWithValue("@tourId", id);

                using (var reader = selectStepsCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        vertexSequence.Add(reader["nom"].ToString());
                    }
                }

                //on reconstruit la tournée
                return new Tour(vertexSequence, totalCost);
            }
        }        

        // ─────────────────────────────────────────────────────────────────────
        // Méthodes utilitaires privées (à compléter selon vos besoins)
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Crée et retourne une nouvelle connexion MySQL ouverte.
        /// Encadrez toujours l'appel dans un bloc using pour garantir la fermeture.
        /// </summary>
        private MySqlConnection OpenConnection()
        {
            var conn = new MySqlConnection(_connectionString);
            conn.Open();
            return conn;
        }
    }
}
