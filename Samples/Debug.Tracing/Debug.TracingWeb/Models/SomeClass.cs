using Debug.Tracing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Debug.TracingWeb.Models
{
    public class SomeClass
    {
        public void SomeMethod1(string x, string y)
        {
            using (new TraceUtil().TraceMethod(x,y))
            {
                //do some logic here
                SomeMethod2(x);
            }
        }

        public void SomeMethod2(string u)
        {
            using (new TraceUtil().TraceMethod(u))
            {
                //do some logic here
            }
        }
    }
}