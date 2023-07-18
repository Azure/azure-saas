using EntityFrameworkCore.AutoFixture.Sqlite;

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
        _skipLiveTest = (options & AutoDataOptions.SkipLiveTest) == AutoDataOptions.SkipLiveTest;
    }

    public AutoDataNSubstituteAttribute(params Type[] customizations) : this(AutoDataOptions.Default, customizations)
    { }

#pragma warning disable CS8603 // Possible null reference return.
    public override string Skip => LiveUnitTestUtil.SkipIfLiveUnitTest(_skipLiveTest);
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
                        ISpecimenBuilder specimentBuilder = Activator.CreateInstance(customizationType) as ISpecimenBuilder
                            ?? throw new NullReferenceException("SpecimentBuilder cannot be null");

                        fixture.Customizations.Add(specimentBuilder);
                    }
                    else if (typeof(ICustomization).IsAssignableFrom(customizationType))
                    {
                        ICustomization customization = Activator.CreateInstance(customizationType) as ICustomization
                            ?? throw new NullReferenceException("Customization cannot be null");

                        fixture = fixture.Customize(customization);
                    }
                    else
                    {
                        throw new InvalidEnumArgumentException($"{customizationType.FullName} does not implement ICustomization or ISpecimentBuilder");
                    }
                }
            }

            fixture = fixture.Customize(new AutoNSubstituteCustomization() { ConfigureMembers = (options & AutoDataOptions.SkipMembers) != AutoDataOptions.SkipMembers });

            if ((options & AutoDataOptions.UseEFMemoryContext) == AutoDataOptions.UseEFMemoryContext)
            {
                fixture = fixture.Customize(new InMemoryContextCustomization()
                {
                    AutoCreateDatabase = true,
                    OmitDbSets = true
                });

            }
            else if ((options & AutoDataOptions.UseEFSQLiteContext) == AutoDataOptions.UseEFSQLiteContext)
            {
                fixture = fixture.Customize(new SqliteContextCustomization()
                {
                    AutoCreateDatabase = true,
                    AutoOpenConnection = true,
                    OmitDbSets = true
                });
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
    UseEFMemoryContext = 0x04,
    UseEFSQLiteContext = 0x10,

    Default = UseEFSQLiteContext
}
