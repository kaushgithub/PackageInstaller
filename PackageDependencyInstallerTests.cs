using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PackageDependencyInstaller;
using System.Collections.Generic;

namespace PackageInstallerTests
{
    [TestClass]
    public class PackageDependencyInstallerTests
    {
        //Invalid input
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestPackageInstaller_NullInputPassed_ExceptionExpected()
        {
            PackageInstaller testTarget = new PackageInstaller();
            testTarget.PrintDependencies(null);
        }

        //Invalid input
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestPackageInstaller_InvalidInputPassed_ExceptionExpected()
        {
            PackageInstaller testTarget = new PackageInstaller();
            testTarget.PrintDependencies(new string[]{"KittenService"});
        }

        //Valid input 1
        [TestMethod]
        public void TestPackageInstaller_CorrectInputPassed_CorrectDependenciesPrinted()
        {
            PackageInstaller testTarget = new PackageInstaller();
            List<string> testDependencies = new List<string>();
            testDependencies.Add("KittenService:");
            testDependencies.Add("Leetmeme: Cyberportal");
            testDependencies.Add("Cyberportal: Ice");
            string expectedOutPut = "KittenService, Ice, Cyberportal, Leetmeme";
            string actualOutPut = testTarget.PrintDependencies(testDependencies.ToArray());
            Assert.IsNotNull(actualOutPut);
            Assert.IsTrue(string.Compare(expectedOutPut, actualOutPut) == 0);
        }

        //Valid input 2
        [TestMethod]
        public void TestPackageInstaller_CorrectInputPassed1_CorrectDependenciesPrinted()
        {
            PackageInstaller testTarget = new PackageInstaller();
            List<string> testDependencies = new List<string>();
            testDependencies.Add("KittenService:");
            testDependencies.Add("Leetmeme: Cyberportal");
            testDependencies.Add("Cyberportal: Ice");
            testDependencies.Add("CamelCaser: KittenService");
            testDependencies.Add("Fraudstream: Leetmeme");
            testDependencies.Add("Ice:");
            string expectedOutPut = "KittenService, Ice, Cyberportal, Leetmeme, CamelCaser, Fraudstream";
            string actualOutPut = testTarget.PrintDependencies(testDependencies.ToArray());
            Assert.IsNotNull(actualOutPut);
            Assert.IsTrue(string.Compare(expectedOutPut, actualOutPut) == 0);
        }

        //Input having cycles
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestPackageInstaller_InputPassedWithCycles_RejectInput()
        {
            PackageInstaller testTarget = new PackageInstaller();
            List<string> testDependencies = new List<string>();
            testDependencies.Add("KittenService:");
            testDependencies.Add("Leetmeme: Cyberportal");
            testDependencies.Add("Cyberportal: Ice");
            testDependencies.Add("CamelCaser: KittenService");
            testDependencies.Add("Fraudstream:");
            testDependencies.Add("Ice:Leetmeme");
            testTarget.PrintDependencies(testDependencies.ToArray());
        }

        //Input having cycles
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestPackageInstaller_InputPassedWithCycles1_RejectInput()
        {
            PackageInstaller testTarget = new PackageInstaller();
            List<string> testDependencies = new List<string>();
            testDependencies.Add("A:B");
            testDependencies.Add("B:A");
            testTarget.PrintDependencies(testDependencies.ToArray());
        }

    }
}
