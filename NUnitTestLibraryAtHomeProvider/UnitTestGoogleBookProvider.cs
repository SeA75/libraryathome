using LibraryAtHomeProvider;
using LibraryAtHomeRepositoryDriver;
using Moq;
using NUnit.Framework;
using RestSharp;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace NUnitTestLibraryAtHomeProvider
{
    public class UnitTestsGoogleBookProvider
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test_FetchBookInfoNull_ThrowArgumentNullException()
        {
            IBooksProvider provider = new GoogleBookProvider(new RequestManager(new RestClient()));            

            Assert.Throws<ArgumentNullException>(() => provider.FetchBookInfo(null));
        }

        [Test]
        public void Test_FetchBookInfoBookEmpty()
        {
            IBooksProvider provider = new GoogleBookProvider(new RequestManager(new RestClient()));

            Assert.Throws<ArgumentNullException>(() => provider.FetchBookInfo(new PocoBook()));
        }

        [Test]
        public void Test_FetchBookInfoBookProviderReturnNull_ReturnEmptyList()
        {
            // Create the mock
            var reqManagerMock = new Mock<IRestRequestManager>();

            // Configure the mock to do something
            reqManagerMock.Setup(x => x.ExecuteGet()).Returns<IRestResponseManager>(null);

            IBooksProvider provider = new GoogleBookProvider(reqManagerMock.Object);

            var res = provider.FetchBookInfo(new PocoBook("test", "myTitle", new Collection<string>(), "", "test"));

            Assert.IsTrue(!res.Any());
        }


        [Test]
        public void Test_ExecuteGetReturnNull_ReturnEmptyResponse()
        {
            // Create the mock
            var restClientMock = new Mock<RestClient>();

            // Configure the mock to do something
            restClientMock.Setup(x => x.Execute(It.IsAny<IRestRequest>(), Method.GET)).Returns<IRestResponse>(null);
            IRestRequestManager mngr = new RequestManager(restClientMock.Object);
            var res = mngr.ExecuteGet();

            Assert.IsTrue(ResponseManager.IsNullOrEmpty(res as ResponseManager));
        }

        [Test]
        public void Test_ExecuteGetRequestNull_ThrowException()
        {
            Assert.Throws<ArgumentNullException>(() => new RequestManager(null));
        }

       


    }
}