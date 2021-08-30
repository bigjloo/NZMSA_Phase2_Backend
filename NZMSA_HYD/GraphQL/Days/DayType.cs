using HotChocolate;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using NZMSA_HYD.Data;
using NZMSA_HYD.GraphQL.Mutations.Events;
using NZMSA_HYD.GraphQL.Mutations.Users;
using NZMSA_HYD.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NZMSA_HYD.GraphQL.Mutations.Days
{
    public class DayType : ObjectType<Day>
    {
        protected override void Configure(IObjectTypeDescriptor<Day> descriptor)
        {
            descriptor.Field(d => d.Id).Type<NonNullType<IdType>>();
            descriptor.Field(d => d.Date).Type<NonNullType<DateTimeType>>();
            descriptor.Field(d => d.PublishKey).Type<StringType>();

            descriptor.Field(d => d.User)
                .ResolveWith<Resolvers>(r => r.GetUser(default!, default!, default))
                .UseDbContext<AppDbContext>()
                .Type<NonNullType<UserType>>();

            descriptor.Field(d => d.Events)
                .ResolveWith<Resolvers>(r => r.GetEvents(default!, default!, default))
                .UseDbContext<AppDbContext>()
                .Type<ListType<EventType>>();
        }

        private class Resolvers
        {
            public async Task<User> GetUser(Day day, [ScopedService] AppDbContext context, CancellationToken cancellationToken)
            {
                return await context.Users.FindAsync(new object[] { day.UserId }, cancellationToken);
            }

            public async Task<IEnumerable<Event>> GetEvents(Day day, [ScopedService] AppDbContext context, CancellationToken cancellationToken)
            {
                return await context.Events.Where(e => e.DayId == day.Id).ToArrayAsync(cancellationToken);
            }
        }
    }
}
