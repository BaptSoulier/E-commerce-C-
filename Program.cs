using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Program
{
    static void Main()
    {
        // Démarrage du serveur HTTP
        Thread serverThread = new Thread(new ThreadStart(StartServer));
        serverThread.Start();
    }

    static void StartServer()
    {
        // Création d'un écouteur TCP sur l'adresse IP locale (127.0.0.1) et le port 8080
        TcpListener tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8080);
        tcpListener.Start();

        Console.WriteLine("Serveur démarré. Attente des connexions...");

        while (true)
        {
            // Attente de la connexion d'un client
            TcpClient client = tcpListener.AcceptTcpClient();

            // Démarrage d'un nouveau thread pour gérer le client
            Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
            clientThread.Start(client);
        }
    }

    static void HandleClient(object obj)
    {
        // Récupération du client TCP à partir du paramètre d'objet
        TcpClient tcpClient = (TcpClient)obj;

        // Obtention du flux réseau du client
        NetworkStream clientStream = tcpClient.GetStream();

        // Tampon pour stocker les données lues du client
        byte[] buffer = new byte[4096];
        int bytesRead;

        while (true)
        {
            bytesRead = 0;

            try
            {
                // Lecture des données du client dans le tampon
                bytesRead = clientStream.Read(buffer, 0, 4096);
            }
            catch
            {
                // En cas d'erreur, terminer la boucle de gestion du client
                break;
            }

            if (bytesRead == 0)
                // Si aucune donnée n'est lue, le client a fermé la connexion, donc sortir de la boucle
                break;

            // Conversion du tampon en une chaîne représentant la requête HTTP du client
            string request = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine("Requête reçue: " + request);

            // Traitement de la requête (ajouter la logique pour gérer les articles et les utilisateurs)

            // Réponse de base renvoyée au client
            string response = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\nBienvenue.";
            byte[] responseBytes = Encoding.UTF8.GetBytes(response);

            // Écriture de la réponse dans le flux réseau du client
            clientStream.Write(responseBytes, 0, responseBytes.Length);
            clientStream.Flush();
        }

        // Fermeture du client TCP
        tcpClient.Close();
    }
}
