﻿using FasTnT.Domain.Data.Model.Filters;
using FasTnT.Model.Events.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;

namespace FasTnT.UnitTest.Queries.SimpleEventQuery
{
    [TestClass]
    public class WhenApplyingEventTypeParameter : SimpleEventQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "eventType", Values = new[] { "ObjectEvent" } });
        }

        [TestMethod]
        public void ItShouldCallTheEventFetcherApplyMethodWithSimpleParameterFilter()
        {
            EventFetcher.Verify(x => x.Apply(It.Is<SimpleParameterFilter>(f => f.Field == EpcisField.EventType && f.Values.Any(v => v.ToString() == "ObjectEvent"))), Times.Once);
        }
    }
}
