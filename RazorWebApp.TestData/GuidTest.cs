using System;

namespace RazorWebApp.TestData
{
    public static class GuidTest
    {
        public static bool Check(string AValue) 
        {
            var LValue = AValue.Replace("\"", "");
            return Guid.TryParse(LValue, out var _);
        }
    }
}
