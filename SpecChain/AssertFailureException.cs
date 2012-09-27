using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecChain {
    class AssertFailureException : Exception {
        public AssertFailureException() : base("Assertに失敗しました。"){
            
        }
    }
}
