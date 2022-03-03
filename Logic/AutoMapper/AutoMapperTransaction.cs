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
    public class AutoMapperTransaction : Profile
    {
        public AutoMapperTransaction()
        {
            CreateMap<Transaction, TransactionDto>().ReverseMap();
        }
        
    }
}
