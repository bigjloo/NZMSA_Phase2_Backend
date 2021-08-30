using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NZMSA_HYD.Model;

namespace NZMSA_HYD.GraphQL.Mutations.Users
{
    public record LoginPayload(
        User User,
        string Jwt);
}
