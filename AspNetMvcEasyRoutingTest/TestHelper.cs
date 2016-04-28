using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetMvcEasyRoutingTest
{
    public static class TestExtensions
    {
        /// <summary>
        /// Test an action that should have an exception of a specified type. It the exception is thrown, the assertion is a succes, otherwise, it fails.
        /// </summary>
        /// <param name="expression">The action that must raise an error</param>
        public static void Thrown<T>(Action expression) where T : Exception
        {
            try
            {
                expression.Invoke();
            }
            catch (T)
            {
                return;//This is what we want.
            }
            catch (Exception nonExpectedException)
            {
                Assert.Fail("Non excepted exception thrown.\n" + nonExpectedException.GetType() + Environment.NewLine + nonExpectedException.Message);
            }
            Assert.Fail("Expected exception has not been thrown.");

        }

        /// <summary>
        /// Test an action that should have an exception of a specified type. It the exception is thrown, the assertion is a succes, otherwise, it fails.
        /// 
        /// Allow to do validation on the exception, for example, on properties
        /// </summary>
        /// <param name="expression">The action that must raise an error</param>
        /// <returns>The exception</returns>
        public static T ThrownAndReturn<T>(Action expression) where T : Exception
        {
            try
            {
                expression.Invoke();
            }
            catch (T r)
            {
                return r;//This is what we want.
            }
            catch (Exception nonExpectedException)
            {
                Assert.Fail("Non excepted exception thrown.\n" + nonExpectedException.GetType() + Environment.NewLine + nonExpectedException.Message);
            }
            Assert.Fail("Expected exception has not been thrown.");
            return null;
        }
    }
}
