using System.Net;
using System.Text;

namespace Puzzle_Solver;

public class WebServer
{
    private static readonly HttpListener _httpListener = new();

    public static void RunHTTP()
    {
        _httpListener.Prefixes.Add(string.Format("http://*:{0}/", 80));
        _httpListener.Start();
        Console.WriteLine("Web server running");
        new Thread(ListenThread).Start();
    }

    private static void ListenThread()
    {
        while (_httpListener.IsListening)
            try
            {
                var context = _httpListener.GetContext();
                var request = context.Request;
                var response = context.Response;

                if (request.HttpMethod == "GET")
                {
                    var str = request.Url.ToString().Split('/')[3];
                    var returnStr = "Failed";

                    //Request has to be http://<ip>/--puzzle--<puzzle_id>
                    if (!string.IsNullOrEmpty(str) && str.StartsWith("--puzzle--"))
                    {
                        try
                        {
                            str = str.Replace("--puzzle--", String.Empty);
                            var imageBuffer = new WebClient().DownloadData(
                                $"http://ubistatic-a.akamaihd.net/0098/captcha/generated/{str}-PuzzleWithMissingPiece.rttex");
                            returnStr = $"{Module.Detect(imageBuffer, str, out _, out _, out _)}";
                        }
                        catch
                        {
                            //Ignore
                        }
                    }
                    
                    var buffer = Encoding.UTF8.GetBytes(returnStr);
                    response.ContentLength64 = buffer.Length;
                    var output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    output.Close();
                    response.Close();
                }
            }
            catch
            {
                //Ignore
            }
    }
}