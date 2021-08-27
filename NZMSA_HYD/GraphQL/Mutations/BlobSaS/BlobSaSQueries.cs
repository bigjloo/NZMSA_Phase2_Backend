using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NZMSA_HYD.BlobSaSBuilder;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using NZMSA_HYD.Data;
using HotChocolate;
using NZMSA_HYD.GraphQL.Extensions;

namespace NZMSA_HYD.GraphQL.Mutations.BlobSaS
{
    [ExtendObjectType(name: "Query")]

    public class BlobSaSQueries
    {
        // Returns users github info + SaSToken
        [UseAppDbContext]
        [Authorize]
        public SaSPayload GetAccountSaSToken(ClaimsPrincipal claimsPrincipal, [ScopedService] AppDbContext context)
        {
            var userIdStr = claimsPrincipal.Claims.First(c => c.Type == "userId").Value;
            var userId = int.Parse(userIdStr);
            var github = context.Users.Find(userId).Github;

            var sasBuilder = new BlobStorageService();
            var token = sasBuilder.GetAccountSasToken();

            return new SaSPayload(token, github);
        }
    }
}
