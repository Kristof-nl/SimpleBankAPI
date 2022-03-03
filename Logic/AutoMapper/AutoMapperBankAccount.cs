using AutoMapper;
using Data.DataObjects;
using Logic.DataTransferObjects.BankAccount;
using Logic.DataTransferObjects.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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