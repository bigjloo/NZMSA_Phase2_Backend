using HotChocolate;
using HotChocolate.Types;
using NZMSA_HYD.Data;
using NZMSA_HYD.GraphQL.Mutations.Days;
using NZMSA_HYD.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NZMSA_HYD.GraphQL.Mutations.Events
{
    public class EventType : ObjectType<Event>
    {
        protected override void Configure(IObjectTypeDescriptor<Event> descriptor)
        {
            descriptor.Field(e => e.Id).Type<NonNullType<IdType>>();
            descriptor.Field(e => e.Name).Type<NonNullType<StringType>>();
            descriptor.Field(e => e.Description).Type<NonNullType<StringType>>();
            descriptor.Field(e => e.Order).Type<NonNullType<IntType>>();
            descriptor.Field(e => e.PhotoURI).Type<StringType>();

            descriptor.Field(e => e.Day)
                .ResolveWith<Resolvers>(r => r.GetDay(default!, default!, default))
                .UseDbContext<AppDbContext>()
                .Type<NonNullType<DayType>>();
        }

        private class Resolvers
        {
            public async Task<Day> GetDay(Event e, [ScopedService] AppDbContext context, CancellationToken cancellationToken)
            {
                return await context.Days.FindAsync(new object[] { e.DayId }, cancellationToken);
            }
        }
    }
}
