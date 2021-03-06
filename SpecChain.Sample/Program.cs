﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecChain.Sample {
    class Program {
        static void Main(string[] args) {
            Spec.RunAllInAssembly(typeof (Program));
        }

        [Spec]
        public void AlwaysPass() {
            Spec.Assert(true);
        }

        [Spec]
        public static void StaticMethodAlsoOK() {
            Spec.Assert(true);
        }

        [Spec]
        public void AlwaysFail() {
            Spec.Assert(false);
        }
    }
}
