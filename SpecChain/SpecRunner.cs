using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SpecChain {
    public class SpecRunner {
        public void RunAllInAssembly(Assembly assembly) {
            var args = new object[] { };
            foreach (var pair in AllExecutePairInAssembly(assembly)) {
                try {
                    var obj = pair.Item1;
                    var mInfo = pair.Item2;
                    mInfo.Invoke(obj, args);
                    Pass();
                }
                catch (AssertFailureException) {
                    Fail();
                }
                catch (Exception) {
                    Fail();
                }
            }
        }

        private IEnumerable<Tuple<object, MethodInfo>> AllExecutePairInAssembly(Assembly assembly) {
            foreach (var type in assembly.GetTypes()) {
                foreach (var m in type.GetMethods()) {
                    if (m.GetCustomAttributes(typeof(SpecAttribute), false).Length > 0) {
                        yield return Tuple.Create(m.IsStatic ? null : Activator.CreateInstance(type), m);
                    }
                }
            }
        }

        private void Pass() {
            ShowMsgWithColor(ConsoleColor.Green, ".");
        }
        private void Fail() {
            ShowMsgWithColor(ConsoleColor.Red, "F");
        }

        private void ShowMsgWithColor(ConsoleColor color, String message) {
            var backup = Console.ForegroundColor;
            try {
                Console.ForegroundColor = color;
                Console.Write(message);
            }
            finally {
                Console.ForegroundColor = backup;
            }
        }

        public void ShowResult() {
            Console.WriteLine();
        }
    }
}
