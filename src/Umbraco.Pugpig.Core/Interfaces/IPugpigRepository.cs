using System;
using Umbraco.Pugpig.Core.Models;

namespace Umbraco.Pugpig.Core.Interfaces
{
    public interface IPugpigRepository
    {
        Feed CreateEditionList(Guid publicationId);
    }
}
