using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NZMSA_HYD.GraphQL.Mutations.Users
{
    public record LoginInput(
        string Code);
}
