using System;
using Umbraco.Framework.DependencyManagement.Autofac;

[assembly: WebActivator.PreApplicationStartMethod(
    typeof($rootnamespace$.App_Start.MySuperPackage), "PreStart")]

namespace $rootnamespace$.App_Start {
    public static class MySuperPackage {
        public static void PreStart() {
            // Add your start logic here
			 var builder = new AutofacContainerBuilder();
          builder.For(typeof(PugpigRepository)).KnownAs<IPugpigRepository>();
		  builder.Build();
        }
    }
}