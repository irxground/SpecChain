using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading;
using System.Diagnostics;

namespace SpecChain {
    public class Spec {

        private static SpecRunner _Runner = null;

        private static SpecRunner Runner { get { return _Runner ?? ThrowNoRunner(); } }
        private static SpecRunner ThrowNoRunner() {
            throw new InvalidOperationException("Please run spec.");
        }

        #region Runner

        public static void RunAllInAssembly<T>() {
            RunAllInAssembly(typeof(T));
        }

        public static void RunAllInAssembly(Type type) {
            RunAllInAssembly(type.Assembly);
        }

        public static void RunAllInAssembly(Assembly assembly) {
            var runner = new SpecRunner();
            try {
                if (Interlocked.CompareExchange(ref _Runner, runner, null) != null) {
                    Console.Error.WriteLine("Don't run test parallel.");
                    Environment.Exit(1);
                }
                runner.RunAllInAssembly(assembly);
            }
            finally {
                if (Interlocked.Exchange(ref _Runner, null) != runner) {
                    Console.Error.WriteLine("Invalid State");
                    Environment.Exit(1);
                }
            }
            runner.ShowResult();
        }

        #endregion

        [DebuggerStepThrough]
        public static void Assert(bool result) {
            if (result == false) {
                throw new AssertFailureException();
            }
        }
    }
}
