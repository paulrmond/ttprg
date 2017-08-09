using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCS.Models
{
    public class CodeTesting
    {
        public string test1 { get; set; }
        public string test2 { get; set; }

        public CodeTesting(string test1,string test2)
        {
            this.test1 = test1;
            this.test2 = test2;
        }

        public void testone()
        {
            test1 = "1";
            test2 = "one";
        }   
        
        public void testtwo()
        {
            test1 = "2";
            test2 = "two";
        }  

        public void testthree()
        {
            test1 = "3";
            test2 = "three";
        }
    }
}