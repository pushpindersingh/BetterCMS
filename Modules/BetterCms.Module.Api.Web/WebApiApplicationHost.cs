using System;

using Autofac;

using BetterCms.Module.Api.Filters;
using BetterCms.Module.Api.Operations.Root.Tags.Tag;
using BetterCms.Module.Api.Operations.Root.Version;

using ServiceStack.ServiceInterface.Validation;
using ServiceStack.Text;
using ServiceStack.WebHost.Endpoints;

namespace BetterCms.Module.Api
{
	public class WebApiApplicationHost
		: AppHostBase
	{
        private readonly Func<ILifetimeScope> containerProvider;

        public WebApiApplicationHost(Func<ILifetimeScope> containerProvider)
            : base("Better CMS Web API Host", typeof(VersionService).Assembly)
        {            
            this.containerProvider = containerProvider;
        }

		public override void Configure(Funq.Container container)
		{            
            RequestBinders.Clear();
			
            JsConfig.EmitCamelCaseNames = true;
            JsConfig.IncludeNullValues = true;
		    JsConfig.DateHandler = JsonDateHandler.ISO8601;

            Plugins.Add(new ValidationFeature());
            
            // Add custom request filter
            RequestFilters.Add(GetRequestProcessor.DeserializeJsonFromGet);

            container.Adapter = new AutofacContainerAdapter(containerProvider);
            container.RegisterValidators(typeof(GetTagRequestValidator).Assembly);
		}
	}
}