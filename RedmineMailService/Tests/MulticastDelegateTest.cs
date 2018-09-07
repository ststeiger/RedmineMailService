using System;
using System.Collections.Generic;
using System.Text;

namespace RedmineMailService.Tests
{


    // https://stackoverflow.com/questions/563549/difference-between-events-and-delegates-and-its-respective-applications
    // The keyword event is a scope modifier for multicast delegates. 
    // Practical differences between this and just declaring a multicast delegate are as follows:
    //   - You can use event in an interface.
    //   - Invocation access to the multicast delegate is limited to the declaring class. 
    //     The behaviour is as though the delegate were private for invocation.
    //     For the purposes of assignment, access is as specified by an explicit access modifier 
    //     (eg public event).

    public class TestClass
    {
        public delegate string GetName_t();

        public event GetName_t OnGetName;


        // string fooo = TestClass.Coalesce<string>();
        // string fooo = TestClass.Coalesce(null, "def", "abc");
        public static T Coalesce<T>(params T[] arguments)
        {
            for (int i = 0; i < arguments.Length; ++i)
            {
                if (!System.Collections.Generic.EqualityComparer<T>.Default.Equals(arguments[i], default(T)))
                    return arguments[i];
            }

            return default(T);
        }


        public string lala()
        {
            // return this.OnGetName();

            System.MulticastDelegate m = (System.MulticastDelegate)this.OnGetName;
            System.Delegate[] dlist = m.GetInvocationList();
            string[] results = new string[dlist.Length];

            for (int i = 0; i < dlist.Length; ++i)
            {
                object[] p = { /*put your parameters here*/ };
                results[i] = System.Convert.ToString(dlist[i].DynamicInvoke(p));
            }

            // return string.Join(",", results);
            return Coalesce(results);
        }

        public string lalaSimple(System.Type t, string name)
        {
            System.MulticastDelegate m = (System.MulticastDelegate)this.OnGetName;
            System.Delegate[] dlist = m.GetInvocationList();
            
            for (int i = 0; i < dlist.Length; ++i)
            {
                // object[] p = { t, name };
                object[] p = { /*put your parameters here*/ };
                string ret = System.Convert.ToString(dlist[i].DynamicInvoke(p));
                if (!string.IsNullOrEmpty(ret) && ret.Trim() != string.Empty)
                    return ret;
            }

            // return string.Join(",", results);
            // return Coalesce(results);
            throw new System.ArgumentException("Cannot find dependency with this arguments.");
        }

    }

    public class Foobar
    {
        protected string m_foo = "foo";
        protected string m_bar = "bar";

        public string foo()
        {
            System.Console.WriteLine(m_foo);
            return m_foo;
        }

        public string bar()
        {
            System.Console.WriteLine(m_bar);
            return m_bar;
        }

    }


    // https://stackoverflow.com/questions/563549/difference-between-events-and-delegates-and-its-respective-applications
    class MulticastDelegateTest
    {

        
        static void Test()
        {
            TestClass tc = new TestClass();
            var fb = new Foobar();
            tc.OnGetName += fb.foo;
            tc.OnGetName += fb.bar;
            // tc.OnGetName();

            System.Console.WriteLine("lala: " + tc.lala());
        }

    }

}
