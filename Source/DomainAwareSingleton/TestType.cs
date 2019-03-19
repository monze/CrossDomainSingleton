using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DomainAwareSingleton
{
    public sealed class TestType1 : MarshalByRefObject
    {
        private int count = 1;

        public TestType1()
        {
            //MessageBox.Show("Creating instance in AppDomain " + AppDomain.CurrentDomain.FriendlyName);
        }

        public void Test()
        {
            string msg = string.Format("Invoked in AppDomain {0} for the {1} time(s)." , AppDomain.CurrentDomain.FriendlyName, count);
            MessageBox.Show(msg);
            count++;
        }

        public void SilentTest()
        {
            count++;
        }

        public string DomainName
        {
            get { return AppDomain.CurrentDomain.FriendlyName; }
        }

        public int Count
        {
            get { return this.count; }
        }
    }

    public sealed class TestType2 : MarshalByRefObject
    {
        private int count = 1;
        public TestType2()
        {
            //MessageBox.Show("Creating instance in AppDomain " + AppDomain.CurrentDomain.FriendlyName);
        }

        public void Test()
        {
            string msg = string.Format("Invoked in AppDomain {0} for the {1} time(s).", AppDomain.CurrentDomain.FriendlyName, count);
            MessageBox.Show(msg);
            count++;
        }

        public void SilentTest()
        {
            count++;
        }

        public string DomainName
        {
            get { return AppDomain.CurrentDomain.FriendlyName; }
        }

        public int Count
        {
            get { return this.count; }
        }
    }
}
