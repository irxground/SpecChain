using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace SpecChain {
    public class Spec {


        public static void RunAllInAssembly<T>() {
            RunAllInAssembly(typeof (T));
        }

        public static void RunAllInAssembly(Type type) {
            RunAllInAssembly(type.Assembly);
        }

        public static void RunAllInAssembly(Assembly assembly) {
            var args = new object[] {};
            foreach (var type in assembly.GetTypes()) {
                object obj = null;
                foreach (var m in type.GetMethods()) {
                    if (m.GetCustomAttributes(typeof(SpecAttribute), false).Length > 0) {
                        if (m.IsStatic) {
                            m.Invoke(null, args);
                        } else {
                            obj = obj ?? Activator.CreateInstance(type);
                            m.Invoke(obj, args);
                        }
                    }
                }
            }
        }
    }
}
