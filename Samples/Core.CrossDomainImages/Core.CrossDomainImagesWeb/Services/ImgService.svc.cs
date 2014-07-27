using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Net;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.IO;

namespace Core.CrossDomainImagesWeb.Services
{
    [ServiceContract(Namespace = "Core.CrossDomainImagesWeb")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ImgService
    {
        [OperationContract]
        [WebGet]
        public string GetImage(string accessToken, string site, string folder, string file)
        {
            //make the request
            string url = String.Format("{0}_api/web/GetFolderByServerRelativeUrl('{1}')/Files('{2}')/$value", site, folder, file);
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Headers.Add("Authorization", "Bearer" + " " + accessToken);
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                using (var sourceSteam = response.GetResponseStream())
                {
                    using (var newStream = new MemoryStream())
                    {
                        sourceSteam.CopyTo(newStream);
                        byte[] bytes = newStream.ToArray();
                        return "data:image/png;base64, " + Convert.ToBase64String(bytes);
                    }
                }
            }
        }
    }
}
