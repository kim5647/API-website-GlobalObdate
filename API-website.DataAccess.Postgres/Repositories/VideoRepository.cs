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
        public async Task<string> GetPathVideo(string pathVideo)
        {
            var video = await _dbContext.Videos.FirstAsync(v => v.NameVideo == pathVideo);
            return video.PathVideo;
        }
        public async Task AddPathVideo(Video video)
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


        //public async Task<IEnumerable<Video>> GetAllAsync()
        //{
        //    var videos = await _dbContext.Videos
        //    .Select(v => new Video // Изменено на `Video`
        //    {
        //        Id = v.Id,
        //        NameVideo = v.NameVideo,
        //        PathVideo = v.PathVideo,
        //        UserId = v.UserId
        //    }).ToListAsync();
        //    return _mapper.Map<Video>(videos);
        //}


    }
}

