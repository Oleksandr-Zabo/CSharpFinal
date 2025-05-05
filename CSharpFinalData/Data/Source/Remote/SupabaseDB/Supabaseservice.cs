using Supabase.Gotrue;
using Client = Supabase.Client;
using static Supabase.Postgrest.Constants;

namespace CSharpFinalData.Data.Source.Remote.SupabaseDB;

/// <summary>
/// Provides services to interact with Supabase.
/// </summary>
/// <remarks>
/// This class handles initialization and user session management for the Supabase client.
/// It is designed to work with a specific Supabase project's URL and API key.
/// </remarks>
public class SupabaseService
{
    
    private const string SupabaseUrl = "https://blsbwhilzmlhlhxywfpl.supabase.co";
    private const string SupabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImJsc2J3aGlsem1saGxoeHl3ZnBsIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDU5NDg0MTYsImV4cCI6MjA2MTUyNDQxNn0.QN24DqtBr6wFqHNyAYw-XOoHGxbxx0fneOoDJxFsDHo";
    
    private readonly Supabase.Client _client;
        
    public User? SupabaseUser { get; set; } = null;
        
    public bool IsLoggedIn { get; set; } = false;
        
    public SupabaseService()
    {
        try
        {
            var options = new Supabase.SupabaseOptions
            {
                AutoConnectRealtime = true,
            };
            _client = new Client(SupabaseUrl, SupabaseKey, options);
        }
        catch (Exception ex)
        {
            throw new Exception($"SupabaseService() raise Exception: {ex.Message}");
        }
    }
        
    public async Task InitServiceAsync()
    {
        try
        {
            await _client.InitializeAsync()!;
        }
        catch (Exception ex)
        {
            throw new Exception($"InitServiceAsync() raise Exception: {ex.Message}");
        }
    }
}