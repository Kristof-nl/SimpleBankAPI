using AutoMapper;
using Data.DataObjects;
using Logic.DataTransferObjects.BankAccount;


namespace Logic.AutoMapper
{
    public class AutoMapperBankAccount : Profile
    {
        public AutoMapperBankAccount()
        {
            CreateMap<BankAccount, BankAccountDto>().ReverseMap();
            CreateMap<BankAccount, CreateBankAccountDto>().ReverseMap();
            CreateMap<CreateBankAccountDto, BankAccountDto>().ReverseMap();
            CreateMap<ShortBankAccountDto, BankAccountDto>().ReverseMap();
            CreateMap<BankAccount, ShortBankAccountDto>().ReverseMap();
        }
    }
}