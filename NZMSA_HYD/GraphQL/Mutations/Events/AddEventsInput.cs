using NZMSA_HYD.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;

namespace NZMSA_HYD.GraphQL.Mutations.Events
{   
    public record EventInput(
        string Name,
        string Description,
        string PhotoURI);

    public record AddEventsInput(
            EventInput[] Events,
            string PublishKey
        );
}
