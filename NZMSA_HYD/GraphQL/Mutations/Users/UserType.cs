using HotChocolate;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using NZMSA_HYD.Data;
using NZMSA_HYD.GraphQL.Mutations.Days;
using NZMSA_HYD.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NZMSA_HYD.GraphQL.Mutations.Users
{
    public class UserType : ObjectType<User>
    {
        protected override void Configure(IObjectTypeDescriptor<User> descriptor)
        {
            descriptor.Field(u => u.Id).Type<NonNullType<IdType>>();
            descriptor.Field(u => u.Name).Type<NonNullType<StringType>>();
            descriptor.Field(u => u.Github).Type<StringType>();
            descriptor.Field(u => u.ImageURI).Type<StringType>();

            descriptor.Field(u => u.Days)
                .ResolveWith<Resolvers>(r => r.GetDays(default!, default!, default))
                .UseDbContext<AppDbContext>()
                .Type<ListType<DayType>>();

        }

        private class Resolvers
        {
            public async Task<IEnumerable<Day>> GetDays(User user, [ScopedService] AppDbContext context, CancellationToken cancellationToken)
            {
                return await context.Days.Where(d => d.UserId == user.Id).ToArrayAsync(cancellationToken);
            }
        }
    }
}
