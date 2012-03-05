using Umbraco.Framework;
using Umbraco.Pugpig.Core.Models;

namespace Umbraco.Pugpig.Core.Interfaces
{
    public interface IPugpigRepository
    {
        Feed CreateEditionList(string publicationName);
    }
}
