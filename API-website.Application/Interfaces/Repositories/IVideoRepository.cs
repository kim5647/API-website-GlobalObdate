using API_website.Core.Models;


namespace API_website.Application.Interfaces.Repositories;
public interface IVideoRepository
{
    Task AddPathVideo(Video video);
    Task<int> GetUserId(string userId);
}