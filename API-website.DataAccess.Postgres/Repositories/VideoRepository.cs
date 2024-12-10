using Microsoft.EntityFrameworkCore;
using API_website.Application.Interfaces.Repositories;
using API_website.Core.Models;
using API_website.DataAccess.Postgres.Entities;
using API_website.DataAccess.Postgres.Mapper.UserProfile;
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
        public string GetPathVideo(string pathVideo, int id)
        {
            var video = _dbContext.Videos
                .Where(v => v.NameVideo == pathVideo && v.UserId == id)
                .SingleOrDefault();

            if (video == null)
            {
                throw new InvalidOperationException($"Video with Path='{pathVideo}' and Id={id} not found.");
            }

            return video.PathVideo;
        }
        public async Task<string> GetPathVideoAsync(string pathVideo, int id)
        {
            var video = await _dbContext.Videos
                .Where(v => v.NameVideo == pathVideo && v.UserId == id)
                .SingleOrDefaultAsync();

            if (video == null)
            {
                throw new InvalidOperationException($"Video with Path='{pathVideo}' and Id={id} not found.");
            }

            return video.PathVideo;
        }
        public async Task AddVideo(Video video)
        {
            var videoEntutes = _mapper.Map<VideoEntities>(video);
            await _dbContext.Videos.AddAsync(videoEntutes);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<Video> GetVideoByNameAsync(string nameVideo, int id)
        {
            var videos = await _dbContext.Videos
                .Where(v => v.NameVideo.Contains(nameVideo))
                .FirstOrDefaultAsync(v => v.UserId == id);

            return _mapper.Map<Video>(videos);
        }

        public async Task<List<Video>> GetNameVideo(int id)
        {
            List<VideoEntities> values = await _dbContext.Videos.Where(v => v.UserId == id).ToListAsync();

            return _mapper.Map<List<Video>>(values);
        }
        public async Task DeleteVideoAsync(string videoName, int idUser)
        {
            var videoEntity = await _dbContext.Videos
                .FirstOrDefaultAsync(v => v.NameVideo == videoName && v.UserId == idUser);

            if (videoEntity == null)
            {
                throw new Exception($"Video '{videoName}' not found for user {idUser}.");
            }

            _dbContext.Videos.Remove(videoEntity);

            await _dbContext.SaveChangesAsync();
        }


    }
}

