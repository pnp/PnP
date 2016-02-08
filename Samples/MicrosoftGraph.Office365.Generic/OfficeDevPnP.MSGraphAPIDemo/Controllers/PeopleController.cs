using OfficeDevPnP.MSGraphAPIDemo.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OfficeDevPnP.MSGraphAPIDemo.Controllers
{
    public class PeopleController : Controller
    {
        public ActionResult GetPersonaPhoto(String upn, Int32 width = 0, Int32 height = 0)
        {
            Stream result = null;
            String contentType = "image/png";

            var sourceStream = UsersGroupsHelper.GetUserPhoto(upn);

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

            return base.File(result, contentType);
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