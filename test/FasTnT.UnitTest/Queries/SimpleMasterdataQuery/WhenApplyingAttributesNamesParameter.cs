﻿using FasTnT.Domain.Data.Model.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FasTnT.UnitTest.Queries.SimpleMasterdataQuery
{
    [TestClass]
    public class WhenApplyingAttributesNamesParameter : SimpleMasterdataQueryTestBase
    {
        public override void Given()
        {
            base.Given();

            Parameters.Add(new Model.Queries.QueryParameter { Name = "attributeNames", Values = new[] { "attName" } });
        }

        [TestMethod]
        public void ItShouldNotCallAnyApplyMethod()
        {
            MasterdataFetcher.Verify(x => x.Apply(It.IsAny<LimitFilter>()), Times.Never);
            MasterdataFetcher.Verify(x => x.Apply(It.IsAny<MasterdataTypeFilter>()), Times.Never);
            MasterdataFetcher.Verify(x => x.Apply(It.IsAny<MasterdataExistsAttibuteFilter>()), Times.Never);
            MasterdataFetcher.Verify(x => x.Apply(It.IsAny<MasterdataDescendentNameFilter>()), Times.Never);
            MasterdataFetcher.Verify(x => x.Apply(It.IsAny<MasterdataNameFilter>()), Times.Never);
        }
    }
}
