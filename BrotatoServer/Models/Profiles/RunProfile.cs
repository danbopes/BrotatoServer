using AutoMapper;
using BrotatoServer.Models.DB;
using BrotatoServer.Models.JSON;
using Newtonsoft.Json;

namespace BrotatoServer.Models.Profiles;

public class RunProfile : Profile
{
    public RunProfile()
    {
        CreateMap<Run, FullRun>()
            .ForMember(dest => dest.RunData, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<RunInformation>(src.RunInformation)!.RunData))
            .ForMember(dest => dest.CustomData, opt => opt.MapFrom(src => src.CustomData == null ? null : JsonConvert.DeserializeObject<CustomData>(src.CustomData)));
    }
}