using AutoMapper;
using LogisticsHub.Application.DTOs;
using LogisticsHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Application.Profiles
{
    public class StoreProfile:Profile
    {
        public StoreProfile()
        {
            CreateMap<Store, CreateStoreDto>().ReverseMap();
        }
    }
}
