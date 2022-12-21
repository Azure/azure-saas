namespace ClientAssertionWithKeyVault.Util;

static public class EncodingExtension
{
    // For details see: https://learn.microsoft.com/en-us/azure/active-directory/develop/msal-net-client-assertions#signed-assertions
    internal static string Base64UrlEncode(this byte[] arg)
    {
        char Base64PadCharacter = '=';
        char Base64Character62 = '+';
        char Base64Character63 = '/';
        char Base64UrlCharacter62 = '-';
        char Base64UrlCharacter63 = '_';

        string str = Convert.ToBase64String(arg);
        str = str.Split(Base64PadCharacter)[0]; // RemoveAccount any trailing padding
        str = str.Replace(Base64Character62, Base64UrlCharacter62); // 62nd char of encoding
        str = str.Replace(Base64Character63, Base64UrlCharacter63); // 63rd char of encoding

        return str;
    }
}
