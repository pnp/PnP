using BusinessApps.O365ProjectsApp.Infrastructure.MicrosoftGraph;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BusinessApps.O365ProjectsApp.WebApp.Controllers
{
    [Authorize]
    public class PersonaController : Controller
    {
        public ActionResult GetPhoto(String upn, Int32 width = 0, Int32 height = 0)
        {
            Stream result = null;
            String contentType = "image/png";

            var sourceStream = GetUserPhoto(upn);

            if (sourceStream != null && width != 0 && height != 0)
            {
                Image sourceImage = Image.FromStream(sourceStream);
                Image resultImage = ScaleImage(sourceImage, width, height);

                result = new MemoryStream();
                resultImage.Save(result, ImageFormat.Png);
                result.Position = 0;
            }
            else
            {
                result = sourceStream;
            }

            if (result != null)
            {
                return base.File(result, contentType);
            }
            else
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.NoContent);
            }
        }

        /// <summary>
        /// This method retrieves the photo of a single user from Azure AD
        /// </summary>
        /// <param name="upn">The UPN of the user</param>
        /// <returns>The user's photo retrieved from Azure AD</returns>
        private static Stream GetUserPhoto(String upn)
        {
            String contentType = "image/png";

            var result = MicrosoftGraphHelper.MakeGetRequestForStream(
                String.Format("{0}users/{1}/photo/$value",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri, upn),
                contentType);

            return (result);
        }

        private Image ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }
    }
}