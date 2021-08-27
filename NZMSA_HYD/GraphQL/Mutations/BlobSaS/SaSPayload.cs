using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NZMSA_HYD.GraphQL.Mutations.BlobSaS
{
    public record SaSPayload(
        string Token,
        string Github
        );

}
