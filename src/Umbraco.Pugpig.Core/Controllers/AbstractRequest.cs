using System;
using System.Web;

namespace Umbraco.Pugpig.Core.Controllers
{
    public class AbstractRequest : IAbstractRequest
    {
        public string GetBaseUrl()
        {
            return HttpContext.Current.Request.Url.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped);
        }
    }
}