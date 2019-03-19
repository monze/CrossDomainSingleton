using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DomainAwareSingleton
{
   
    public class SingletonTest
    {
        [Fact]
        public void TopDownDomainTest()
        {
            TestType1 testType1 = Singleton<TestType1>.Instance;
            Assert.Equal("DomainAwareSingleton.xUnitTest", testType1.DomainName);
            Assert.Equal(1, testType1.Count);

            testType1.SilentTest();
            testType1.SilentTest();

            Assert.Equal(3, testType1.Count);

            // In unit test, the default appDomain is location in C:/Program.../visualStudio...TESTPLATFORM
            // so we need to used the information from the current appDomain.
            Evidence securityInfo = AppDomain.CurrentDomain.Evidence;
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;

            AppDomain subAppDomain = AppDomain.CreateDomain("TopDownFirstSubAppDomain", securityInfo, setup);

            // Remember to set the navigation name.
            subAppDomain.SetData(AppDomainHelper.KeyParentAppDomainFrindlyName, AppDomain.CurrentDomain.FriendlyName);
            subAppDomain.DoCallBack(TopDownFirstSubAppDomain);

            Assert.Equal(8, testType1.Count);
        }

        private static void TopDownFirstSubAppDomain()
        {
            TestType1 testType1 = Singleton<TestType1>.Instance;
            Assert.Equal("TopDownFirstSubAppDomain", AppDomain.CurrentDomain.FriendlyName);
            Assert.Equal(3, testType1.Count);

            testType1.SilentTest();
            testType1.SilentTest();

            Evidence securityInfo = AppDomain.CurrentDomain.Evidence;
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;

            AppDomain subAppDomain = AppDomain.CreateDomain("TopDownSecondSubAppDomain", securityInfo, setup);

            // Remember to set the navigation name.
            subAppDomain.SetData(AppDomainHelper.KeyParentAppDomainFrindlyName, AppDomain.CurrentDomain.FriendlyName);
            subAppDomain.DoCallBack(TopDownSecondSubAppDomain);

            Assert.Equal(7, testType1.Count);
            testType1.SilentTest();
        }

        private static void TopDownSecondSubAppDomain()
        {
            TestType1 testType1 = Singleton<TestType1>.Instance;
            Assert.Equal("TopDownSecondSubAppDomain", AppDomain.CurrentDomain.FriendlyName);
            Assert.Equal(5, testType1.Count);

            testType1.SilentTest();
            testType1.SilentTest();
        }


        [Fact]
        public void BottomUpDomainTest()
        {
            // In unit test, the default appDomain is location in C:/Program.../visualStudio...TESTPLATFORM
            // so we need to used the information from the current appDomain.
            Evidence securityInfo = AppDomain.CurrentDomain.Evidence;
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;

            AppDomain subAppDomain = AppDomain.CreateDomain("BottomUpFirstSubAppDomain", securityInfo, setup);

            // Remember to set the navigation name.
            subAppDomain.SetData(AppDomainHelper.KeyParentAppDomainFrindlyName, AppDomain.CurrentDomain.FriendlyName);
            subAppDomain.DoCallBack(BottomUpFirstSubAppDomain);

            TestType1 testType1 = Singleton<TestType1>.Instance;
            Assert.Equal(5, testType1.Count);
            testType1.SilentTest();
            testType1.SilentTest();

            Assert.Equal("DomainAwareSingleton.xUnitTest", testType1.DomainName);
            Assert.Equal(7, testType1.Count);
        }

        private static void BottomUpFirstSubAppDomain()
        {
            Evidence securityInfo = AppDomain.CurrentDomain.Evidence;
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;

            AppDomain subAppDomain = AppDomain.CreateDomain("BottomUpSecondSubAppDomain", securityInfo, setup);

            // Remember to set the navigation name.
            subAppDomain.SetData(AppDomainHelper.KeyParentAppDomainFrindlyName, AppDomain.CurrentDomain.FriendlyName);
            subAppDomain.DoCallBack(BottomUpSecondSubAppDomain);            

            TestType1 testType1 = Singleton<TestType1>.Instance;
            Assert.Equal("BottomUpFirstSubAppDomain", AppDomain.CurrentDomain.FriendlyName);
            Assert.Equal(2, testType1.Count);

            testType1.SilentTest();
            testType1.SilentTest();

            Assert.Equal(4, testType1.Count);
            testType1.SilentTest();
        }

        private static void BottomUpSecondSubAppDomain()
        {
            TestType1 testType1 = Singleton<TestType1>.Instance;
            Assert.Equal("BottomUpSecondSubAppDomain", AppDomain.CurrentDomain.FriendlyName);
            Assert.Equal(1, testType1.Count);

            testType1.SilentTest();
        }
    }
}
