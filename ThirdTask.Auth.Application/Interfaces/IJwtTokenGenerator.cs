using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThirdTask.Auth.Application.Dtos;
using ThirdTask.Auth.Application.Features.Mediator.Results;

namespace ThirdTask.Auth.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        TokenResponseDto GenerateToken(GetCheckAppUserQueryResult result);
    }
}
