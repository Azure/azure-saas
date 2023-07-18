using System.Reflection.Metadata.Ecma335;

namespace TestUtilities;

public static class AssertAdditions
{
    /// <summary>
    /// Makes sure all members of one object are equal to similarly named members
    /// of another object.  Second object can have more members but needs to 
    /// contain all of the first objects members unless explicitely skipped.
    /// </summary>
    /// <typeparam name="T1">First object type</typeparam>
    /// <typeparam name="T2">Second object type</typeparam>
    /// <param name="first">First object</param>
    /// <param name="second">Second object</param>
    /// <param name="skip">List of parameters to skip</param>
    public static void AllPropertiesAreEqual<T1, T2>(T1 first, T2 second, params string[] skip)
    {
        Type t1 = typeof(T1);
        Type t2 = typeof(T2);

        Assert.All(t1.GetMembers(BindingFlags.Public | BindingFlags.Instance), firstMemberInfo =>
        {
            if (firstMemberInfo.MemberType == MemberTypes.Property)
            {
                if (skip.Contains(firstMemberInfo.Name))
                {
                    return;
                }

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
