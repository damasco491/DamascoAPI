﻿using HotChocolate.Data.Filters;
using HotChocolate.Data.Filters.Expressions;

namespace API.GraphQL
{
    public class CustomFilteringConvention : FilterConvention
    {
        protected override void Configure(IFilterConventionDescriptor descriptor)
        {
            descriptor.AddDefaults();
            descriptor.AddProviderExtension(
                new QueryableFilterProviderExtension(
                    x => x.AddFieldHandler<QueryableStringInvariantEqualsHandler>()));
        }
    }
}
