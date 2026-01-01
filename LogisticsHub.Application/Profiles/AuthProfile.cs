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
    public class AuthProfile:Profile
    {
        public AuthProfile()
        {
            CreateMap<RegisterModel, ApplicationUser>().ForMember(dest => dest.FullName,
                opt => opt.MapFrom(src => src.UserName));
        }
    }
}
