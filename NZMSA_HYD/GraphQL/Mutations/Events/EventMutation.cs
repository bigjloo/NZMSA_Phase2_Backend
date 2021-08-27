using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Data;
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

namespace NZMSA_HYD.GraphQL.Mutations.Events
{
    [ExtendObjectType(name: "Mutation")]
    public class EventMutation
    {
        // Replaces current day event's with new events 
        [UseAppDbContext]
        [Authorize]
        public async Task<Day> AddEventsAsync(ClaimsPrincipal claimsPrincipal, AddEventsInput input, [ScopedService] AppDbContext context, CancellationToken cancellationToken)
        {
            var userIdStr = claimsPrincipal.Claims.First(c => c.Type == "userId").Value;
            var userId = int.Parse(userIdStr);
            var today = context.Days.FirstOrDefault(d => d.UserId == userId && d.Date == DateTime.Today);

            // If no record for today, create a new Day and save
            if (today == null)
            {
                today = new Day
                {
                    Date = DateTime.Today,
                    UserId = userId,
                };
                context.Days.Add(today);
                await context.SaveChangesAsync(cancellationToken);
            } else
            {
                // Deletes all events for the day
                var eventsForDay = context.Events.Where(e => e.DayId == today.Id);
                foreach (var e in eventsForDay)
                {
                    context.Events.Remove(e);
                    await context.SaveChangesAsync(cancellationToken);
                }
            }

            today.PublishKey = input.PublishKey;

            var count = 0;

            // Replace with new Events
            foreach (var e in input.Events)
            {
                var newEvent = new Event
                {
                    Name = e.Name,
                    Description = e.Description,
                    PhotoURI = e.PhotoURI,
                    Order = count++,
                    DayId = today.Id
                };
                context.Events.Add(newEvent);
                await context.SaveChangesAsync(cancellationToken);
            }

            return today;
        }
    }
}
