using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThirdTask.Jwt.Dtos;

namespace ThirdTask.Jwt.Interfaces
{
    public interface IJwtTokenGenerator
    {
        TokenResponseDto GenerateToken(CheckAppUserDto result);
    }
}
