﻿using FasTnT.Domain.Data.Model.Filters;
using FasTnT.Model.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FasTnT.UnitTest.Queries.SimpleEventQuery
{
    [TestClass]
    public class WhenApplyingGE_ILMDParameterWithNumericValue : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "GE_ILMD_namespace#ilmd", Values = new[] { "15" } });
        }

        [TestMethod]
        public void ItShouldCallTheEventFetcherApplyMethodWithSourceDestinationFilter()
        {
            EventFetcher.Verify(x => x.Apply(It.Is<ComparisonCustomFieldFilter>(f => f.IsInner == false && f.Comparator == FilterComparator.GreaterOrEqual && f.Field.Namespace == "namespace" && f.Field.Name == "ilmd" && f.Field.Type == FieldType.Ilmd && (double)f.Value == 15)), Times.Once);
        }
    }
}
