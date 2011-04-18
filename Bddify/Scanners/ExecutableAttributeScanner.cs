using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bddify.Core;

namespace Bddify.Scanners
{
    public class ExecutableAttributeScanner : IScanForSteps
    {
        public IEnumerable<ExecutionStep> Scan(Type scenarioType)
        {
            var methods = scenarioType
                .GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.GetCustomAttributes(typeof(ExecutableAttribute), false).Any())
                .OrderBy(m => ((ExecutableAttribute)m.GetCustomAttributes(typeof(ExecutableAttribute), false)[0]).Order);

            // ToDo: the arguments should be provided
            // ToDO: should use StepText if it is provided
            return methods.Select(m => new ExecutionStep(m, null, NetToString.FromName(m.Name), IsAssertingByAttribute(m)));
        }

        private static bool IsAssertingByAttribute(MethodInfo method)
        {
            var attribute = GetExecutableAttribute(method);
            return attribute.Asserts;
        }

        private static ExecutableAttribute GetExecutableAttribute(MethodInfo method)
        {
            return (ExecutableAttribute)method.GetCustomAttributes(typeof(ExecutableAttribute), false).First();
        }
    }
}