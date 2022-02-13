using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace TestUtilities
{
    public static class AssertAdditions
    {
        public static void AllPropertiesAreEqual<T1, T2>(T1 first, T2 second)
        {
            Type t1 = typeof(T1);
            Type t2 = typeof(T2);

            Assert.All(t1.GetMembers(BindingFlags.Public | BindingFlags.Instance), firstMemberInfo => {
                if (firstMemberInfo.MemberType == MemberTypes.Property)
                {
                    var firstPropertyInfo = t1.GetProperty(firstMemberInfo.Name);
                    var secondPropertyInfo = t2.GetProperty(firstMemberInfo.Name);

                    Assert.NotNull(firstPropertyInfo);
                    Assert.NotNull(secondPropertyInfo);

                    Assert.Equal(firstPropertyInfo?.PropertyType, secondPropertyInfo?.PropertyType);

                    object? val1 = firstPropertyInfo!.GetValue(first);
                    object? val2 = secondPropertyInfo!.GetValue(second);

                    Assert.True(val1?.Equals(val2), $"Property {firstMemberInfo.Name} doesn't match");
                }

            });

        }
    }
}
