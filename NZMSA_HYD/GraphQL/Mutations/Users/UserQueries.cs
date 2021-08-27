using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Types;
using NZMSA_HYD.Data;
using NZMSA_HYD.GraphQL.Extensions;
using NZMSA_HYD.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NZMSA_HYD.GraphQL.Mutations.Users
{
    [ExtendObjectType(name: "Query")]
    public class UserQueries
    {

        [UseAppDbContext]
        [Authorize]
        public User GetSelf(ClaimsPrincipal claimsPrincipal, [ScopedService] AppDbContext context)
        {
            var userIdStr = claimsPrincipal.Claims.First(c => c.Type == "userId").Value;
            var user = context.Users.Find(int.Parse(userIdStr));
            return user;
        }


    }
}
