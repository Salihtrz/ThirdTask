using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThirdTask.Auth.Application.Features.Mediator.Results;
using ThirdTask.Jwt.Dtos;

namespace ThirdTask.Auth.Application.Mapping
{
    public class JwtMapping : Profile
    {
        public JwtMapping() 
        {
            CreateMap<GetCheckAppUserQueryResult, CheckAppUserDto>().ReverseMap();
        }
    }
}
