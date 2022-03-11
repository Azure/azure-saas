namespace TestUtilities;

/// <summary>
/// Use this attribute when you want AutoFixture to populate values for both
/// concrete classes and mocks created with NSubstitute
/// 
/// </summary>
/// <example>
/// In the code below, auditor will have values created,
/// IAuditInfo will also have dummy values for every field, 
/// LoggedinUser will have the empty string value.
/// 
/// [Theory, AutoData_NSubstitute_Plus]
/// public void Sample2(Auditor auditor, IAuditInfo info)
/// {
/// 	//This is auto generated.
/// 	Assert.NotNull(info);
///
/// 	//Members of mocked objects have values.
/// 	Assert.NotNull(info.ActingUser);
/// 	Assert.True(info.ActingUser != "");
/// }
/// </example>
public class AutoDataNSubstituteAttribute : AutoDataAttribute
{
    private readonly bool _skipLiveTest = false;

    public AutoDataNSubstituteAttribute(AutoDataOptions options = AutoDataOptions.Default, params Type[] customizations)
        : base(GetFactory(options, customizations))
    {
        this._skipLiveTest = (options & AutoDataOptions.SkipLiveTest) == AutoDataOptions.SkipLiveTest;
    }

    public AutoDataNSubstituteAttribute(params Type[] customizations) : this(AutoDataOptions.Default, customizations)
    { }

#pragma warning disable CS8603 // Possible null reference return.
    public override string Skip => LiveUnitTestUtil.SkipIfLiveUnitTest(this._skipLiveTest);
#pragma warning restore CS8603 // Possible null reference return.

    private static Func<IFixture> GetFactory(AutoDataOptions options, Type[] cusomizations)
    {
        return () =>
        {
            IFixture fixture = new Fixture();

            if (cusomizations != null)
            {
                foreach (Type customizationType in cusomizations)
                {
                    if (typeof(ISpecimenBuilder).IsAssignableFrom(customizationType))
                    {
                        var specimentBuilder = Activator.CreateInstance(customizationType) as ISpecimenBuilder;
                        fixture.Customizations.Add(specimentBuilder);
                    }
                    else if (typeof(ICustomization).IsAssignableFrom(customizationType))
                    {
                        var customization = Activator.CreateInstance(customizationType) as ICustomization;
                        fixture = fixture.Customize(customization);
                    }
                    else
                    {
                        throw new InvalidEnumArgumentException($"{customizationType.FullName} does not implement ICustomization or ISpecimentBuilder");
                    }
                }
            }

            fixture = fixture.Customize(new AutoNSubstituteCustomization() { ConfigureMembers = (options & AutoDataOptions.SkipMembers) != AutoDataOptions.SkipMembers });

            if ((options & AutoDataOptions.SkipEFMemoryContext) != AutoDataOptions.SkipEFMemoryContext)
            {
                fixture = fixture.Customize(new InMemoryContextCustomization());
            }

            return fixture;
        };
    }
}

[Flags]
public enum AutoDataOptions
{
    SkipMembers = 0x01,
    SkipLiveTest = 0x02,
    SkipEFMemoryContext = 0x04,

    Default = 0x00
}
