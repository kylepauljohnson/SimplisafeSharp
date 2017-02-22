using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = NUnit.Framework.Assert;

namespace SimplisafeSharp.Tests
{
    [TestClass]
    public class SimplisafeSharpTests
    {
        [TestMethod]
        public void Login()
        {
            var simplisafe = new SimplisafeSharp();
            var response = simplisafe.Login("johndoe@email.com", "somepassword");

            Assert.IsNotNull(response);
            Assert.IsNotEmpty(response.SessionId);
            Assert.IsTrue(response.UserId > 0);
        }

        [TestMethod]
        public void GetLocations()
        {
            var simplisafe = new SimplisafeSharp();
            var response = simplisafe.Login("johndoe@email.com", "somepassword");
            var locations = simplisafe.GetLocations(response) ;

            Assert.IsNotNull(locations);
        }

        [TestMethod]
        public void SetStatusHome()
        {
            var simplisafe = new SimplisafeSharp();
            var statusResponse = simplisafe.SetStatus(SimplisafeSharp.StatusType.Home, 0);

            Assert.IsNotNull(statusResponse);
        }

        [TestMethod]
        public void SetStatusAway()
        {
            var simplisafe = new SimplisafeSharp();
            var statusResponse = simplisafe.SetStatus(SimplisafeSharp.StatusType.Away, 0);

            Assert.IsNotNull(statusResponse);
        }

        [TestMethod]
        public void SetStatusOff()
        {
            var simplisafe = new SimplisafeSharp();
            var statusResponse = simplisafe.SetStatus(SimplisafeSharp.StatusType.Off, 0);

            Assert.IsNotNull(statusResponse);
        }
    }
}
