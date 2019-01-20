﻿using FasTnT.Domain;
using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
using FasTnT.UnitTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.UnitTest.Domain.QueryServiceTests
{
    [TestClass]
    public class WhenProcessingAGetVendorVersionRequest : BaseQueryServiceUnitTest
    {
        static string ExpectedVersion = Constants.ProductVersion;

        public GetVendorVersion Request { get; set; }
        public GetVendorVersionResponse Response { get; set; }

        public override void Arrange()
        {
            base.Arrange();

            Request = new GetVendorVersion();
        }

        public override void Act() => Response = QueryService.Process(Request).Result;

        [Assert]
        public void TheResponseShouldNotBeNull() => Assert.IsNotNull(Response);

        [Assert]
        public void TheResponseShouldContainTheSoftwareVersion() => Assert.AreEqual(ExpectedVersion, Response.Version);
    }
}
