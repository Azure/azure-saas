namespace TestUtilities;

/// <summary>
/// Use this attribute when you want to provide some of the values for a Theory that has AutoDataNSubstituteAttribute
/// It will pass the values you provide, and use AutoFixture/NSubstitute to generate the rest of them.
/// </summary>
public class InlineAutoDataNSubstituteAttribute : InlineAutoDataAttribute
{
    public InlineAutoDataNSubstituteAttribute(params object[] values) : base(new AutoDataNSubstituteAttribute(), values)
    {
    }
}