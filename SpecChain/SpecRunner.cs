using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Diagnostics;
using System.Linq.Expressions;

namespace SpecChain {
    public class SpecRunner {

        private readonly List<Exception> _Throwns = new List<Exception>();
        private readonly Stopwatch _StopWatch = new Stopwatch();
        private int _passed = 0;
        private int _failed = 0;

        public void RunAllInAssembly(Assembly assembly) {
            _StopWatch.Restart();
            var args = new object[] { };
            foreach (var mInfo in MethodInfoInAssembly(assembly)) {
                try {
                    var func = Expression.Lambda<Action>(mInfo.IsStatic
                                   ? Expression.Call(mInfo)
                                   : Expression.Call(
                                   Expression.New(mInfo.DeclaringType),
                                       mInfo)).Compile();
                    func();
                    Pass();
                }
                catch (Exception e) {
                    _Throwns.Add(e);
                    Fail();
                }
            }
            Console.WriteLine();
            Console.WriteLine();
            ShowResult();
        }

        private IEnumerable<MethodInfo> MethodInfoInAssembly(Assembly assembly) {
            foreach (var type in assembly.GetTypes()) {
                foreach (var m in type.GetMethods()) {
                    if (m.GetCustomAttributes(typeof(SpecAttribute), false).Length > 0) {
                        yield return m;
                    }
                }
            }
        }

        private void Pass() {
            _passed++;
            ShowInColor(ConsoleColor.Green, () => Console.Write("."));
        }
        private void Fail() {
            _failed++;
            ShowInColor(ConsoleColor.Red, () => Console.Write("F"));
        }

        private void ShowInColor(ConsoleColor color, Action action) {
            var backup = Console.ForegroundColor;
            try {
                Console.ForegroundColor = color;
                action();
            }
            finally {
                Console.ForegroundColor = backup;
            }
        }

        public void ShowResult() {
            _StopWatch.Stop();
            if (_Throwns.Count > 0) {
                ShowExceptions();
            }
            Console.WriteLine("Finish in {0} seconds", _StopWatch.ElapsedMilliseconds / 1000.0);
            var color = _failed == 0 ? ConsoleColor.Green : ConsoleColor.Red;
            ShowInColor(color, ()=> Console.WriteLine("{0} examples, {1} failure", _passed + _failed, _failed));
        }

        private void ShowExceptions() {
            Console.WriteLine("Failures:");
            Console.WriteLine();
            int num = 0;
            foreach (var ex in _Throwns) {
                num++;
                var stacktrace = new StackTrace(ex, true);
                var firstFrame = stacktrace.GetFrames().First(f => f.GetMethod().DeclaringType != typeof(Spec));

                var type = firstFrame.GetMethod().DeclaringType;
                var method = firstFrame.GetMethod().Name;
                Console.WriteLine("{0,5}) {1}.{2}()", num, type, method);
                ShowInColor(ConsoleColor.Red, () => Console.WriteLine("       " + ex.Message));
                var file = firstFrame.GetFileName();
                var line = firstFrame.GetFileLineNumber();
                Console.WriteLine("       " + file + ":" + line);
                Console.WriteLine();
            }
        }
    }
}
