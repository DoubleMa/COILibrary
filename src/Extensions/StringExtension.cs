using Mafi;
using Mafi.Localization;
using System.Text.RegularExpressions;

namespace COILib.Extensions;

public static class StringExtension {

    public static string CleanString(this string str) => Regex.Replace(str, "[^a-zA-Z0-9]", "");

    public static string CapitalizeAllFirstChar(this string str) => str.Split(' ').MapArray(e => e.CapitalizeFirstChar()).JoinStrings(" ");

    public static LocStrFormatted CapitalizeFirstChar(this LocStrFormatted loc) => loc.Value.CapitalizeFirstChar().AsLoc();

    public static LocStrFormatted CapitalizeFirstChar(this LocStr loc) => loc.AsFormatted.CapitalizeFirstChar();

    public static LocStrFormatted CapitalizeAllFirstChar(this LocStrFormatted loc) => loc.Value.CapitalizeAllFirstChar().AsLoc();

    public static LocStrFormatted CapitalizeAllFirstChar(this LocStr loc) => loc.AsFormatted.CapitalizeAllFirstChar();
}