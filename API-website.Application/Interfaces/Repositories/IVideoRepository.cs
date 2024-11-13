using API_website.Core.Models;


namespace API_website.Application.Interfaces.Repositories;
public interface IVideoRepository
{
    Task<Video> GetVideoByNameAsync(string nameVideo, int id);
    Task AddPathVideo(Video video);
    Task<string> GetPathVideo(string pathVideo);
}