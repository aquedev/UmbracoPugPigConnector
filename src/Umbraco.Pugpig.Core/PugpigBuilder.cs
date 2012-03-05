using Umbraco.Framework.DependencyManagement;
using Umbraco.Pugpig.Core.Controllers;
using Umbraco.Pugpig.Core.Interfaces;
using Umbraco.Pugpig.Core.Repositories;

namespace Umbraco.Pugpig.Core
{
    public class PugpigBuilder : IDependencyDemandBuilder 
    {
        public void Build(IContainerBuilder containerBuilder, IBuilderContext context)
        {
            containerBuilder.For(typeof(PugpigRepository)).KnownAs<IPugpigRepository>();
            containerBuilder.For(typeof(AbstractRequest)).KnownAs<IAbstractRequest>();
           // containerBuilder.Build();
        }
    }
}