using API_website.Core.Models;


namespace API_website.Application.Interfaces.Repositories;
public interface IVideoRepository
{
    Task<Video> GetVideoByNameAsync(string nameVideo, int id);
    Task AddVideo(Video video);
    Task<string> GetPathVideoAsync(string pathVideo, int id);
    string GetPathVideo(string pathVideo, int id);
    Task<List<Video>> GetNameVideo(int id);
    Task DeleteVideoAsync(string nameVideo, int idUser);
}