using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.Helpers;

public class AutomapperProfiles : Profile
{
    public AutomapperProfiles()
    {
        CreateMap<AppUser, MemberDto>()
            .ForMember(destination => destination.MainPhotoUrl,
                options => 
                    options.MapFrom(user => user.Photos.FirstOrDefault(photo => photo.IsMain).Url))
            .ForMember(destination => destination.Age,
                options => options.MapFrom(user => user.DateOfBirth.CalculateAge()));
                
        CreateMap<Photo, PhotoDto>();
    }
}
