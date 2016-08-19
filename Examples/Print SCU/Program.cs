// Copyright (c) 2012-2015 fo-dicom contributors.
// Licensed under the Microsoft Public License (MS-PL).

namespace Print_SCU
{
    using System;
    using System.Drawing;

    using Dicom;
    using Dicom.Imaging;
    using Dicom.Log;
    using System.Collections.Generic;

    internal class Program
    {
        private static void Main(string[] args)
        {
            // Initialize log manager.
            LogManager.SetImplementation(ConsoleLogManager.Instance);

            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            var printJob = new PrintJob("DICOM PRINT JOB")
                               {
                                   RemoteAddress = "localhost",
                                   RemotePort = 104,
                                   CallingAE = "PRINTSCU",
                                   CalledAE = "PRINTSCP"
                               };
             
            printJob.StartFilmBox("STANDARD\\2,2","PORTRAIT", "A4");

            //设置彩色还是黑白打印
            printJob.FilmSession.IsColor = false; //set to true to print in color 
             

            List<String> files = new List<string>();
            files.Add(@"E:\Dicom图片1\123.dcm");
            files.Add(@"E:\Dicom图片1\2.dcm");
            files.Add(@"E:\Dicom图片1\3.dcm");
            int i = 0;
            foreach (String file in files) {
                var dicomImage = new DicomImage(file);
                var bitmap = dicomImage.RenderImage().As<Bitmap>();
                printJob.AddImage(bitmap, i);
                bitmap.Dispose();
                i++;
            }


            printJob.EndFilmBox();

            printJob.Print();

            stopwatch.Stop();
            Console.WriteLine();
            Console.WriteLine(stopwatch.Elapsed);
        }
    }
}
