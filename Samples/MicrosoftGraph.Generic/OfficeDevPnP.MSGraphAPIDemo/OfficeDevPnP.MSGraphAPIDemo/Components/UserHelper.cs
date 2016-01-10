using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Components
{
    public class UserHelper
    {
        public static Stream GetUserPhoto(String upn)
        {
            String contentType = "image/png";

            var result = MicrosoftGraphHelper.MakeGetRequestForStream(
                String.Format("{0}users/{1}/photo/$value",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri, upn),
                contentType);

            return (result);
        }
    }
}