using System.Web.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;

namespace UnitTest
{
    public static class ControllerTestHelper
    {
        public static void SetFakeControllerContext(Controller controller)
        {
            var httpContext = new Mock<HttpContextBase>();
            var session = new Mock<HttpSessionStateBase>();

            httpContext.Setup(c => c.Session).Returns(session.Object);

            controller.ControllerContext = new ControllerContext(
                httpContext.Object,
                new RouteData(),
                controller
            );
        }
    }
}
