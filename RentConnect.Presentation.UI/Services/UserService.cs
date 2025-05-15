using RentConnect.API.RentConnect.Domain.Models;

namespace RentConnect.Presentation.UI.Services;

public class UserService
{
    private readonly HttpClient _httpClient;

    public UserService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<ApplicationUser>>("api/Users");
    }

    public async Task<ApplicationUser> GetUserByIdAsync(string id)
    {
        return await _httpClient.GetFromJsonAsync<ApplicationUser>($"api/Users/{id}");
    }

    public async Task CreateUserAsync(ApplicationUser user)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Users", user);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateUserAsync(string id, ApplicationUser user)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/Users/{id}", user);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteUserAsync(string id)
    {
        var response = await _httpClient.DeleteAsync($"api/Users/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task ForgotPasswordAsync(string email)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Users/forgot-password", new { Email = email });
        response.EnsureSuccessStatusCode();
    }

    public async Task ResetPasswordAsync(string email, string password, string code)
    {
        var payload = new
        {
            Email = email,
            Password = password,
            Code = code
        };

        var response = await _httpClient.PostAsJsonAsync("api/Users/reset-password", payload);
        response.EnsureSuccessStatusCode();
    }


}