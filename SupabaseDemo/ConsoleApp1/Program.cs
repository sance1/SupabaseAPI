using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;

class User
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int? Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
}

class Program
{
    private static readonly string supabaseUrl = "https://rbllwdxyiweszhhaojft.supabase.co";
    private static readonly string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InJibGx3ZHh5aXdlc3poaGFvamZ0Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTI1NDMyNzUsImV4cCI6MjA2ODExOTI3NX0.0zc4BYvHEzqk2W95-lankV3wcmKyOihijx4f0JP5xOo";
    private static readonly string tableEndpoint = "/rest/v1/User";

    static async Task Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("\nMenu:");
            Console.WriteLine("1. Tambah user");
            Console.WriteLine("2. Lihat semua user");
            Console.WriteLine("3. Update user");
            Console.WriteLine("4. Delete user");
            Console.WriteLine("0. Keluar");
            Console.Write("Pilih menu: ");
            var choice = Console.ReadLine();

            if (choice == "1")
            {
                Console.Write("Masukkan nama: ");
                string name = Console.ReadLine();
                Console.Write("Masukkan email: ");
                string email = Console.ReadLine();
                await AddUser(name, email);
            }
            else if (choice == "2")
            {
                await GetUsers();
            }
            else if (choice == "3")
            {
                Console.Write("Masukkan ID user yang ingin diupdate: ");
                int id = int.Parse(Console.ReadLine());
                Console.Write("Masukkan email baru: ");
                string newEmail = Console.ReadLine();
                await UpdateUser(id, newEmail);
            }
            else if (choice == "4")
            {
                Console.Write("Masukkan ID user yang ingin dihapus: ");
                int id = int.Parse(Console.ReadLine());
                await DeleteUser(id);
            }
            else if (choice == "0")
            {
                break;
            }
            else
            {
                Console.WriteLine("Pilihan tidak valid.");
            }
        }
    }

    static async Task AddUser(string name, string email)
    {
        var user = new User { Name = name, Email = email };
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
                Console.WriteLine($"- ID: {user.Id}, Nama: {user.Name}, Email: {user.Email}");
            }
        }
        else
        {
            Console.WriteLine("Gagal mengambil data:");
            Console.WriteLine(responseBody);
        }
    }

    static async Task UpdateUser(int id, string newEmail)
    {
        var updateData = new { Email = newEmail };
        var json = JsonConvert.SerializeObject(updateData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var client = new HttpClient();
        client.BaseAddress = new Uri(supabaseUrl);
        client.DefaultRequestHeaders.Add("apikey", supabaseKey);
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {supabaseKey}");
        client.DefaultRequestHeaders.Add("Prefer", "return=representation");

        var response = await client.PatchAsync($"{tableEndpoint}?Id=eq.{id}", content);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("User berhasil diupdate:");
            Console.WriteLine(responseBody);
        }
        else
        {
            Console.WriteLine("Gagal mengupdate user:");
            Console.WriteLine(responseBody);
        }
    }

    static async Task DeleteUser(int id)
    {
        using var client = new HttpClient();
        client.BaseAddress = new Uri(supabaseUrl);
        client.DefaultRequestHeaders.Add("apikey", supabaseKey);
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {supabaseKey}");

        var request = new HttpRequestMessage(HttpMethod.Delete, $"{tableEndpoint}?Id=eq.{id}");
        request.Headers.Add("Prefer", "return=representation");

        var response = await client.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("User berhasil dihapus:");
            Console.WriteLine(responseBody);
        }
        else
        {
            Console.WriteLine("Gagal menghapus user:");
            Console.WriteLine(responseBody);
        }
    }
}
