using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecChain.Sample {
    class Program {
        static void Main(string[] args) {
            Spec.RunAllInAssembly(typeof (Program));
        }

        [Spec]
        public void Test() {
            Spec.Assert(true);

            Spec.Assert(false);
        }
    }
}
