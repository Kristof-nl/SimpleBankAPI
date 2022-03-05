using AutoMapper;
using Data.DataObjects;
using Logic.DataTransferObjects.Transaction;
using Logic.DataTransferObjects.User;

namespace Logic.AutoMapper
{
    public class AutoMapperUser : Profile
    {
        public AutoMapperUser()
        {
            CreateMap<ApiUser, UserDto>().ReverseMap();
        }
        
    }
}
