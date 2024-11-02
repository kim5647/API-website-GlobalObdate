using AutoMapper;
using API_website.DataAccess.Postgres.Entities;
using API_website.Core.Models;

namespace API_website.DataAccess.Postgres.Mapper.UserProfile;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserEntities, User>().ReverseMap();
        CreateMap<VideoEntities, Video>().ReverseMap();
    }
}
