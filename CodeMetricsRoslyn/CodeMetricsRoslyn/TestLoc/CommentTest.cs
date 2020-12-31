using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMetricsRoslyn
{
    public class CommentTest
    {
        /*asldkaksdkalsd
         * asdasd
         * 
         * asdasdasdasd
         * asdasdasd
         * 
         * 
         * a
         * sdasdasd*/ public void Method2() { }   //lkjlakjsdljoasd;fkk;l;

        public void Method()
        {
            Method2();
            if (4 < 3)
            {
                var a = "qwe";
            }
            else
            {
                var b = "asd";
            }

            if (1 < 2) { var q = "sdf"; } else { var w = "dfg"; }
            var str = "strdpoz;lsdfk";
            /*
             
             var r= 3;

            var s = new Test();
             
             
             */
        }
    }
}
