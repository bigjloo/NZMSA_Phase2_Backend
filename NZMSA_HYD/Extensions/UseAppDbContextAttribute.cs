using System.Reflection;
using HotChocolate.Types;
using HotChocolate.Types.Descriptors;
using NZMSA_HYD.Data;

namespace NZMSA_HYD.GraphQL.Extensions
{
    public class UseAppDbContextAttribute : ObjectFieldDescriptorAttribute
    {
        public override void OnConfigure(
            IDescriptorContext context,
            IObjectFieldDescriptor descriptor,
            MemberInfo member)
        {
            descriptor.UseDbContext<AppDbContext>();
        }
    }
}