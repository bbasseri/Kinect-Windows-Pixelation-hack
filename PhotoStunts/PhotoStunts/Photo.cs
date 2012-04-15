using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Text.RegularExpressions;
//using System.Windows.Controls;
using System.Net;
using System.IO;
using System.Drawing;
using System.Collections;


namespace PhotoStunts
{
    public class Photo
    {

        public static void Main(String[] args)
        {
            Photo p = new Photo();
            //p.getImage("https://api.500px.com/v1/photos?feature=editors&page=2&consumer_key=HJFMg2LvexMhIKrDs2qaVUxNem27kAxhdfiAYJMW");
            p.getImageOAuth();
        }

        public Photo()
        {
            Console.WriteLine("Calling constructor");
        }


        public void getImageOAuth() {
            string consumerKey = "HJFMg2LvexMhIKrDs2qaVUxNem27kAxhdfiAYJMW";
            string consumerSecret = "mPdJjtpeyXvRaUQaAaHP7KJMOWWAh8NfSf3T5NuO";
            Uri uri = new Uri("https://api.500px.com/v1/users");
            Uri requestToken = new Uri("https://api.500px.com/v1/oauth/request_token");
            Uri accessToken = new Uri("https://api.500px.com/v1/oauth/access_token");
            Uri authorizeURL = new Uri("https://api.500px.com/v1/oauth/authorize");

            OAuthBase oAuth = new OAuthBase();
            string nonce = oAuth.GenerateNonce();
            string timeStamp = oAuth.GenerateTimeStamp();
            string o1 = "";
            string o2 = "";
            string sig = oAuth.GenerateSignature(uri,
                consumerKey, consumerSecret,
                string.Empty, string.Empty,
                "GET", timeStamp, nonce,
                OAuthBase.SignatureTypes.HMACSHA1, out o1, out o2);

            sig = HttpUtility.UrlEncode(sig);

            StringBuilder sb = new StringBuilder(uri.ToString());
            sb.AppendFormat("?oauth_consumer_key={0}&", consumerKey);
            sb.AppendFormat("oauth_nonce={0}&", nonce);
            sb.AppendFormat("oauth_timestamp={0}&", timeStamp);
            sb.AppendFormat("oauth_signature_method={0}&", "HMAC-SHA1");
            sb.AppendFormat("oauth_version={0}&", "1.0");
            sb.AppendFormat("oauth_signature={0}", sig);

            System.Diagnostics.Debug.WriteLine(sb.ToString());
        }

        //Given a URL return a collection with the images at that location
        public ArrayList getImage(string _URL)
        {
            ArrayList allImages = new ArrayList();
            try
            {
                // Open a connection
                System.Net.HttpWebRequest httpWebRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(_URL);

                httpWebRequest.AllowWriteStreamBuffering = true;

                // You can also specify additional header values like the user agent or the referer: (Optional)
                httpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)";
                httpWebRequest.Referer = "http://www.google.com/";

                // set timeout for 20 seconds (Optional)
                httpWebRequest.Timeout = 20000;

                // Request response:
                System.Net.WebResponse webResponse = httpWebRequest.GetResponse();


                //Get all the images from the response
                //System.IO.Stream webStream = webResponse.GetResponseStream();
                //---------------------------
                // Obtain a 'Stream' object associated with the response object.
                Stream receiveStream = webResponse.GetResponseStream();

                Encoding encode = System.Text.Encoding.GetEncoding("utf-8");

                // Pipe the stream to a higher level stream reader with the required encoding format. 
                StreamReader readStream = new StreamReader(receiveStream, encode);

                Console.WriteLine("\nResponse stream received");
                Char[] read = new Char[256];

                // Read 256 charcters at a time.    
                int count = readStream.Read(read, 0, 256);
                String concatt = "";
                Console.WriteLine("HTML...\r\n");

                while (count > 0)
                {
                    // Dump the 256 characters on a string and display the string onto the console.
                    String str = new String(read, 0, count);
                    concatt = concatt + str;
                    Console.Write(str);
                    count = readStream.Read(read, 0, 256);
                }

                Console.WriteLine("");
                // Release the resources of stream object.
                readStream.Close();
                receiveStream.Close();
                //---------------------------

                //MatchCollection matches = Regex.Matches(concatt, "\\\"image_url\\\":[https?|ftp|gopher|telnet|file|notes|ms-help]://[^\\\",}][jpg|jpeg|JPG|JPEG|bmp|BMP|GIF|gif|PNG|png|TIFF|tiff|WMF|wmf|EMF|emf]");
                MatchCollection matches = Regex.Matches(concatt, "\\\"image_url\\\":\\\"https?://[^\\\",}]*");
                HttpWebRequest req = null;
                WebResponse resp = null;
                Stream respStream = null;
                Image tmpImage = null;
                int i = 0;
                foreach (Match m in matches) {
                    String url = m.Value.Substring(13);
                    req = (HttpWebRequest)HttpWebRequest.Create(url);
                    resp = req.GetResponse();
                    respStream = resp.GetResponseStream();

                    // convert webstream to image
                    tmpImage = Image.FromStream(respStream);
                    //Save image to file system
                    //tmpImage.Save("C:\\del" + i +".jpeg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    //Add the images to the array
                    allImages.Add(tmpImage);
                    // Cleanup
                    resp.Close();
                    respStream.Close();
                    i++;
                }
               
            }
            catch (Exception _Exception)
            {
                // Error
                Console.WriteLine("Exception caught", _Exception.ToString());
                return null;
            }

            return allImages;

        }

    }
   
}