using System.Diagnostics;
using Yolov8Net;
using Image = System.Drawing.Image;

namespace Puzzle_Solver;

public static class Module
{
    public static IPredictor Predictor;
    public static string Detect(byte[] buffer, string id, out Image image, out Prediction prediction, out long Time)
    {
        Stream stream = new MemoryStream(RTTEX.RTTEXConverter.RTTEXUnpack(buffer).Result);
        image = Image.FromStream(stream);
        stream.Close();
        
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        var predictions = Predictor.Predict(image);
        
        stopwatch.Stop();
        
        Console.WriteLine($"Preditcted {predictions.Length} detects in {stopwatch.ElapsedMilliseconds}ms");

        if (predictions.Length != 0)
        {
            try
            {
                prediction = predictions.Where(c => c.Score >= 0.6).First();
                if (prediction != null)
                {
                    var value = Math.Max(prediction.Rectangle.X, 0) / 512;
                    Time = stopwatch.ElapsedMilliseconds;
                    return value.ToString();
                }
            }
            catch
            {

            }
        }

        prediction = null;

        Time = stopwatch.ElapsedMilliseconds;

        return "Failed";
    }

    public static void Initialize()
    {
        Predictor = YoloV5Predictor.Create("model.onnx", new[] { "puzzle" }, false);
    }
}