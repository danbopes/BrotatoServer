using AutoMapper;
using BrotatoServer.Models.DB;
using BrotatoServer.Models.Views;

namespace BrotatoServer.Models.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserSettings, UserSettingsViewModel>()
            .ReverseMap();
    }
}