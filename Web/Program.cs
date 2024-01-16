using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class User
{
    public int Id { get; set; }
    public string? Username { get; set; }
}

public class Article
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
}

public class UserRepository
{
    private static List<User> users = new List<User>();
    private static int userIdCounter = 1;

    public List<User> GetAllUsers()
    {
        return users;
    }

    public User GetUserById(int userId)
    {
        return users.Find(u => u.Id == userId);
    }

    public User CreateUser(User user)
    {
        user.Id = userIdCounter++;
        users.Add(user);
        return user;
    }

    // Ajout des méthodes pour les opérations de mise à jour et de suppression
    public void UpdateUser(User updatedUser)
    {
        var existingUser = users.FirstOrDefault(u => u.Id == updatedUser.Id);
        if (existingUser != null)
        {
            existingUser.Username = updatedUser.Username;
            // Mettez à jour d'autres propriétés au besoin
        }
    }

    public void DeleteUser(int userId)
    {
        var userToRemove = users.FirstOrDefault(u => u.Id == userId);
        if (userToRemove != null)
        {
            users.Remove(userToRemove);
        }
    }
}

public class ArticleRepository
{
    private static List<Article> articles = new List<Article>();
    private static int articleIdCounter = 1;

    public List<Article> GetAllArticles()
    {
        return articles;
    }

    public Article GetArticleById(int articleId)
    {
        return articles.Find(a => a.Id == articleId);
    }

    public Article CreateArticle(Article article)
    {
        article.Id = articleIdCounter++;
        articles.Add(article);
        return article;
    }

    // Ajout des méthodes pour les opérations de mise à jour et de suppression
    public void UpdateArticle(Article updatedArticle)
    {
        var existingArticle = articles.FirstOrDefault(a => a.Id == updatedArticle.Id);
        if (existingArticle != null)
        {
            existingArticle.Title = updatedArticle.Title;
            existingArticle.Content = updatedArticle.Content;
            // Mettez à jour d'autres propriétés au besoin
        }
    }

    public void DeleteArticle(int articleId)
    {
        var articleToRemove = articles.FirstOrDefault(a => a.Id == articleId);
        if (articleToRemove != null)
        {
            articles.Remove(articleToRemove);
        }
    }
}

public class SimpleHttpServer
{
    private readonly HttpListener listener = new HttpListener();
    private readonly UserRepository userRepository = new UserRepository();
    private readonly ArticleRepository articleRepository = new ArticleRepository();

    public SimpleHttpServer(string[] prefixes)
    {
        if (!HttpListener.IsSupported)
        {
            throw new NotSupportedException("HTTP Listener is not supported on this platform.");
        }

        foreach (var prefix in prefixes)
        {
            listener.Prefixes.Add(prefix);
        }
    }

    public void Start()
    {
        listener.Start();
        Console.WriteLine("Server started. Listening for requests...");

        Task.Run(() => HandleRequests());
    }

    public void Stop()
    {
        listener.Stop();
        listener.Close();
    }

    private async Task HandleRequests()
    {
        while (listener.IsListening)
        {
            var context = await listener.GetContextAsync();
            ProcessRequest(context);
        }
    }

    private void ProcessRequest(HttpListenerContext context)
    {
        var request = context.Request;
        var response = context.Response;

        var path = request.Url.AbsolutePath.ToLower();

        switch (request.HttpMethod)
        {
            case "GET":
                HandleGetRequest(path, response);
                break;
            case "POST":
                HandlePostRequest(path, request.InputStream, response);
                break;
            case "PUT":
                HandlePutRequest(path, request.InputStream, response);
                break;
            case "DELETE":
                HandleDeleteRequest(path, response);
                break;
            default:
                response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                break;
        }

        response.Close();
    }

    private void HandleGetRequest(string path, HttpListenerResponse response)
    {
        switch (path)
        {
            case "/api/users":
                SendJsonResponse(response, userRepository.GetAllUsers());
                break;
            case "/api/articles":
                SendJsonResponse(response, articleRepository.GetAllArticles());
                break;
            default:
                if (path.StartsWith("/api/users/"))
                {
                    GetSingleUser(path, response);
                }
                else if (path.StartsWith("/api/articles/"))
                {
                    GetSingleArticle(path, response);
                }
                else
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                }
                break;
        }
    }

    private void GetSingleUser(string path, HttpListenerResponse response)
    {
        var userIdStr = path.Split('/').Last();
        if (int.TryParse(userIdStr, out int userId))
        {
            var user = userRepository.GetUserById(userId);
            if (user != null)
            {
                SendJsonResponse(response, user);
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
            }
        }
        else
        {
            response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
    }

    private void GetSingleArticle(string path, HttpListenerResponse response)
    {
        var articleIdStr = path.Split('/').Last();
        if (int.TryParse(articleIdStr, out int articleId))
        {
            var article = articleRepository.GetArticleById(articleId);
            if (article != null)
            {
                SendJsonResponse(response, article);
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
            }
        }
        else
        {
            response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
    }

    private void HandlePostRequest(string path, Stream inputStream, HttpListenerResponse response)
    {
        switch (path)
        {
            case "/api/users":
                CreateUser(inputStream, response);
                break;
            case "/api/articles":
                CreateArticle(inputStream, response);
                break;
            default:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                break;
        }
    }

    private void HandlePutRequest(string path, Stream inputStream, HttpListenerResponse response)
    {
        switch (path)
        {
            case "/api/users":
                UpdateUser(inputStream, response);
                break;
            case "/api/articles":
                UpdateArticle(inputStream, response);
                break;
            default:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                break;
        }
    }

    private void HandleDeleteRequest(string path, HttpListenerResponse response)
    {
        switch (path)
        {
            case "/api/users":
                DeleteUser(path, response);
                break;
            case "/api/articles":
                DeleteArticle(path, response);
                break;
            default:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                break;
        }
    }

    private void CreateUser(Stream inputStream, HttpListenerResponse response)
    {
        using (var reader = new StreamReader(inputStream))
        {
            var json = reader.ReadToEnd();
            var newUser = JsonConvert.DeserializeObject<User>(json);

            if (newUser != null)
            {
                var createdUser = userRepository.CreateUser(newUser);
                SendJsonResponse(response, createdUser, HttpStatusCode.Created);
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
        }
    }

    private void CreateArticle(Stream inputStream, HttpListenerResponse response)
    {
        using (var reader = new StreamReader(inputStream))
        {
            var json = reader.ReadToEnd();
            var newArticle = JsonConvert.DeserializeObject<Article>(json);

            if (newArticle != null)
            {
                var createdArticle = articleRepository.CreateArticle(newArticle);
                SendJsonResponse(response, createdArticle, HttpStatusCode.Created);
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
        }
    }

    // Ajout des méthodes pour les opérations de mise à jour et de suppression
    private void UpdateUser(Stream inputStream, HttpListenerResponse response)
    {
        using (var reader = new StreamReader(inputStream))
        {
            var json = reader.ReadToEnd();
            var updatedUser = JsonConvert.DeserializeObject<User>(json);

            if (updatedUser != null)
            {
                userRepository.UpdateUser(updatedUser);
                response.StatusCode = (int)HttpStatusCode.OK;
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
        }
    }

    private void UpdateArticle(Stream inputStream, HttpListenerResponse response)
    {
        using (var reader = new StreamReader(inputStream))
        {
            var json = reader.ReadToEnd();
            var updatedArticle = JsonConvert.DeserializeObject<Article>(json);

            if (updatedArticle != null)
            {
                articleRepository.UpdateArticle(updatedArticle);
                response.StatusCode = (int)HttpStatusCode.OK;
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
        }
    }

    private void DeleteUser(string path, HttpListenerResponse response)
    {
        var userIdStr = path.Split('/').Last();
        if (int.TryParse(userIdStr, out int userId))
        {
            userRepository.DeleteUser(userId);
            response.StatusCode = (int)HttpStatusCode.NoContent;
        }
        else
        {
            response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
    }

    private void DeleteArticle(string path, HttpListenerResponse response)
    {
        var articleIdStr = path.Split('/').Last();
        if (int.TryParse(articleIdStr, out int articleId))
        {
            articleRepository.DeleteArticle(articleId);
            response.StatusCode = (int)HttpStatusCode.NoContent;
        }
        else
        {
            response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
    }

    private void SendJsonResponse<T>(HttpListenerResponse response, T data, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        response.StatusCode = (int)statusCode;
        response.ContentType = "application/json";

        var json = JsonConvert.SerializeObject(data);
        var buffer = Encoding.UTF8.GetBytes(json);

        response.ContentLength64 = buffer.Length;

        using (var output = response.OutputStream)
        {
            output.Write(buffer, 0, buffer.Length);
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        var prefixes = new[] { "http://localhost:8080/" };
        var server = new SimpleHttpServer(prefixes);

        server.Start();

        Console.WriteLine("Press Enter to stop the server.");
        Console.ReadLine();

        server.Stop();
        Console.WriteLine("Server stopped.");
    }
}

