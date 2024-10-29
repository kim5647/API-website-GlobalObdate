using Microsoft.EntityFrameworkCore;
using API_website.Application.Interfaces.Repositories;
using API_website.Core.Models;
using API_website.DataAccess.Postgres.Entities;
using AutoMapper;

namespace API_website.DataAccess.Postgres.Repositories
{
    public class VideoRepository : IVideoRepository
    {
        private readonly IMapper _mapper;
        private readonly DBContext _dbContext;
        public VideoRepository(DBContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task AddPathVideo(Video video)
        {
            var videoEntutes = _mapper.Map<VideoEntities>(video);
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

