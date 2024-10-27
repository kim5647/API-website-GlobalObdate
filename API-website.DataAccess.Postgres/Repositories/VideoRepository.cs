using Microsoft.EntityFrameworkCore;
using API_website.Application.Interfaces.Repositories;
using API_website.Core.Models;
using API_website.DataAccess.Postgres.Entities;

namespace API_website.DataAccess.Postgres.Repositories
{
    public class VideoRepository : IVideoRepository
    {
        private readonly DBContext _dbContext;
        public VideoRepository(DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddPathVideo(Video video)
        {
            var videoEntutes = new VideoEntities
            {
                NameVideo = video.NameVideo,
                PathVideo = video.PathVideo,
                UserId = video.UserId,
            };
            await _dbContext.Videos.AddAsync(videoEntutes);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> GetUserId(string username)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) throw new ArgumentException("User not found.");
            return user.Id;
        }
    }
}

