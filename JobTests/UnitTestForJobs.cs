using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JobTests
{
    using System;
    using JobLib;

    [TestClass]
    public class UnitTestForJobs
    {


        [TestMethod]
        public void SingleJobTestMethodWithNoParametes()
        {
            string expectedResult = String.Empty;
            string actualResult = String.Empty;
            string bundleNumberForTesting = String.Empty; // no params

            JobBundles.BundleDict.Add(bundleNumberForTesting, "a=>");

            actualResult = Executor.RunJobs(bundleNumberForTesting);

            Assert.AreEqual(expectedResult, actualResult);

        }


        [TestMethod]
        public void SingleJobTestMethod()
        {
            string expectedResult = "JobName : a";
            string actualResult = String.Empty;
            string bundleNumberForTesting = Guid.NewGuid().ToString();

            JobBundles.BundleDict.Add(bundleNumberForTesting, "a=>");

            actualResult = Executor.RunJobs(bundleNumberForTesting);

            Assert.AreEqual(expectedResult, actualResult);

        }

        [TestMethod]
        public void ThreeJobsTestMethod()
        {
            string expectedResult = "JobName : a JobName : b JobName : c";
            string actualResult = String.Empty;
            string bundleNumberForTesting = Guid.NewGuid().ToString();

            JobBundles.BundleDict.Add(bundleNumberForTesting, "a=>;b=>;c=>");

            actualResult = Executor.RunJobs(bundleNumberForTesting);

            Assert.AreEqual(expectedResult, actualResult);
            JobBundles.BundleDict.Clear();
        }


        [TestMethod]
        public void ThreeJobsWithOneDependencyTestMethod()
        {
            string expectedResult = "JobName : a JobName : c JobName : b";
            string actualResult = String.Empty;
            string bundleNumberForTesting = Guid.NewGuid().ToString();

            JobBundles.BundleDict.Add(bundleNumberForTesting, "a=>;b=>c;c=>");

            actualResult = Executor.RunJobs(bundleNumberForTesting);

            Assert.AreEqual(expectedResult, actualResult);

        }


        [TestMethod]
        public void MultipleDependenciesTestMethod()
        {
            string expectedResult = "JobName : a JobName : f JobName : d JobName : c JobName : b JobName : e";

            string actualResult = String.Empty;
            string bundleNumberForTesting = Guid.NewGuid().ToString();

            JobBundles.BundleDict.Add(bundleNumberForTesting, @"a=>;
                                                               b=>c;
                                                               c=>f;                                           
                                                               d=>a;
                                                               e=>b;
                                                               f=>");

            actualResult = Executor.RunJobs(bundleNumberForTesting);

            Assert.AreEqual(expectedResult, actualResult);

        }



        [TestMethod]
        public void SelfDependencyTestMethod()
        {
            string expectedResult = "Jobs can't depend on itself  !";

            string actualResult = String.Empty;
            string bundleNumberForTesting = Guid.NewGuid().ToString();


            JobBundles.BundleDict.Add(bundleNumberForTesting, @"a=>;
                                                               b=>;
                                                               c=>c");

            actualResult = Executor.RunJobs(bundleNumberForTesting);

            StringAssert.Contains(expectedResult, actualResult);

        }


        [TestMethod]
        public void CycledDependenciesTestMethod()
        {
            string expectedResult = "Cycled dependencies detected: JobName : c JobName : b JobName : f";
            string actualResult = String.Empty;
            string bundleNumberForTesting = Guid.NewGuid().ToString();


            JobBundles.BundleDict.Add(bundleNumberForTesting, @"a =>;
                                                                b =>c;
                                                                c=>f;
                                                                d=>a;
                                                                e =>;
                                                                f =>b");

            actualResult = Executor.RunJobs(bundleNumberForTesting);


            StringAssert.Contains(actualResult, expectedResult);

        }



    }
}
