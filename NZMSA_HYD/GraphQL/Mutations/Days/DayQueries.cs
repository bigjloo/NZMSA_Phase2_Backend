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
using System.Threading;
using System.Threading.Tasks;

namespace NZMSA_HYD.GraphQL.Mutations.Days
{
    [ExtendObjectType(name: "Query")]
    public class DayQueries
    {
        [UseAppDbContext]
        public Day GetDay(String publishKey, [ScopedService] AppDbContext context)
        {
            var day = context.Days.FirstOrDefault(d => d.PublishKey == publishKey);

            return day;
        }

        [UseAppDbContext]
        [Authorize]
        public Day GetToday(ClaimsPrincipal claimsPrincipal, [ScopedService] AppDbContext context)
        {
            var userIdStr = claimsPrincipal.Claims.First(c => c.Type == "userId").Value;
            var user = context.Users.Find(int.Parse(userIdStr));
            var today = context.Days.FirstOrDefault(d => d.UserId == user.Id && d.Date == DateTime.Today);
            return today;
        }
    }

}
