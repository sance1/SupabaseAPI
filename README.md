# SupabaseAPI
SupabaseAPI Program Code


using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;

class User
{
    public string Author { get; set; }
    public string Title { get; set; }
}

class Program
{
    private static readonly string supabaseUrl = "https://rbllwdxyiweszhhaojft.supabase.co";
    private static readonly string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InJibGx3ZHh5aXdlc3poaGFvamZ0Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTI1NDMyNzUsImV4cCI6MjA2ODExOTI3NX0.0zc4BYvHEzqk2W95-lankV3wcmKyOihijx4f0JP5xOo";
    private static readonly string tableEndpoint = "/rest/v1/book";

    static async Task Main(string[] args)
    {
        Console.WriteLine("1. Tambah user");
        Console.WriteLine("2. Lihat semua user");
        Console.Write("Pilih menu (1/2): ");
        var choice = Console.ReadLine();

        if (choice == "1")
        {
            Console.Write("Masukkan nama: ");
            string author = Console.ReadLine();
            Console.Write("Masukkan email: ");
            string title = Console.ReadLine();

            await AddUser(author, title);
        }
        else if (choice == "2")
        {
            await GetUsers();
        }
        else
        {
            Console.WriteLine("Pilihan tidak valid.");
        }
    }

    static async Task AddUser(string author, string title)
    {
        var user = new User { Author = author, Title = title };
        var json = JsonConvert.SerializeObject(user);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var client = new HttpClient();
        client.BaseAddress = new Uri(supabaseUrl);
        client.DefaultRequestHeaders.Add("apikey", supabaseKey);
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {supabaseKey}");
        client.DefaultRequestHeaders.Add("Prefer", "return=representation");

        var response = await client.PostAsync(tableEndpoint, content);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("User berhasil ditambahkan:");
            Console.WriteLine(responseBody);
        }
        else
        {
            Console.WriteLine("Gagal menambahkan user:");
            Console.WriteLine(responseBody);
        }
    }

    static async Task GetUsers()
    {
        using var client = new HttpClient();
        client.BaseAddress = new Uri(supabaseUrl);
        client.DefaultRequestHeaders.Add("apikey", supabaseKey);
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {supabaseKey}");

        var response = await client.GetAsync($"{tableEndpoint}?select=*");
        var responseBody = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var users = JsonConvert.DeserializeObject<List<User>>(responseBody);
            Console.WriteLine("Daftar User:");
            foreach (var user in users)
            {
                Console.WriteLine($"- {user.Author} ({user.Title})");
            }
        }
        else
        {
            Console.WriteLine("Gagal mengambil data:");
            Console.WriteLine(responseBody);
        }
    }
}

