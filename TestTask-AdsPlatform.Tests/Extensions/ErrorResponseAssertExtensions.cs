using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTask_AdsPlatform.Result;

namespace TestTask_AdsPlatform.Tests.Extensions
{
    public static class ErrorResponseAssertExtensions
    {
        public static void ShouldBeEquivalentTo(this ErrorResponse actual, ErrorResponse expected)
        {
            Assert.NotNull(actual);
            Assert.Equal(expected.ErrorCode, actual.ErrorCode);
            Assert.Equal(expected.Message, actual.Message);
        }
    }
}
