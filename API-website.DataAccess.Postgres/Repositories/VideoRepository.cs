public class VideoRepository : IVideoRepository
{
    private readonly DBContext _dbContext;
    public VideoRepository(DBContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task AddPathVideo(Video video)
    {
        await _dbContext.Videos.AddAsync(video);
        await _dbContext.SaveChangesAsync();
    }
} 