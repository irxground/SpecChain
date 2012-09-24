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
                }
                catch (AssertFailureException e) {

                }
                catch (Exception e) {

                }
            }
        }

        private IEnumerable<Tuple<object, MethodInfo>> AllExecutePairInAssembly(Assembly assembly) {
            foreach (var type in assembly.GetTypes()) {
                object obj = null;
                foreach (var m in type.GetMethods()) {
                    if (m.GetCustomAttributes(typeof(SpecAttribute), false).Length > 0) {
                        if (m.IsStatic) {
                            yield return Tuple.Create(null as object, m);
                        }
                        else {
                            obj = obj ?? Activator.CreateInstance(type);
                            yield return Tuple.Create(obj, m);
                        }
                    }
                }
            }
        }

        public void Pass() {
            ShowMsgWithColor(ConsoleColor.Green, ".");
        }
        public void Fail() {
            ShowMsgWithColor(ConsoleColor.Red, "F");
        }

        private void ShowMsgWithColor(ConsoleColor color, String message) {
            var backup = Console.ForegroundColor;
            try {
                Console.ForegroundColor = color;
                Console.Write(message);
            } finally {
                Console.ForegroundColor = backup;
            }
        }

        public void ShowResult() {
            Console.WriteLine();
        }
    }
}
