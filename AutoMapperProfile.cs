using AutoMapper;
using CampusFlow.Mvc.Models;
using CampusFlow.Mvc.ViewModels;

namespace CampusFlow.Mvc;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // Convert User to UserViewModel
        CreateMap<User, UserViewModel>();

        // Convert CreateUserViewModel to User
        CreateMap<CreateUserViewModel, User>()
            .ForMember(dest => dest.PasswordHash,
                opt => opt.Ignore());

        // Convert User to EditUserViewModel
        CreateMap<User, EditUserViewModel>();

        // Update only allowed fields in an existing User
        CreateMap<EditUserViewModel, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserName,
                opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.Email,
                opt => opt.MapFrom(src => src.Email));
    }
}
