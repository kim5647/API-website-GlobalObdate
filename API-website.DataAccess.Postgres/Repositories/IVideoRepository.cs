using API_website.DataAccess.Postgres.Entities;

//namespace API_website.Application.Interfaces.Repository;
namespace API_website.DataAccess.Postgres.Repositories;
public interface IVideoRepository
{
    Task AddPathVideo(Video video);
    Task<int> GetUserId(string userId);
}