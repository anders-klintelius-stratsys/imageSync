using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExifLib;
using System.IO;


namespace ImageTest
{
    class Program
    {
        static string toPath = @Properties.Settings.Default.toPath;
        static string fromPath = @Properties.Settings.Default.fromPath;

        static int numFiles = 0;
        static int failedFiles = 0;
        static int unknownFiles = 0;
        static int copiedFiles = 0;
        static void Main(string[] args)
        {
            foreach(string fPath in fromPath.Split(char.Parse(";")))
            { 
            string[] fileEntries = Directory.GetFiles(fPath);
            foreach (string fileName in fileEntries)
            {
                try
                {
                    getImage(fileName);
                    numFiles += 1;
                }
                catch (Exception ex)
                {
                    failedFiles += 1;
                }
            }
            }
            Console.WriteLine(numFiles.ToString() + " har synkats");
            Console.WriteLine(copiedFiles.ToString() + " nya bilder har kopierats");
            Console.WriteLine(failedFiles.ToString() + " failade");
            Console.WriteLine(unknownFiles.ToString() + " hade okänt filformat");
            Console.ReadLine();
        }
        static void getImage(string path)
        {
            if(Path.GetExtension(path).ToLower()==".jpg")
            {
                using (ExifReader reader = new ExifReader(@path))
            {
                // Extract the tag data using the ExifTags enumeration
                DateTime datePictureTaken;
                if (reader.GetTagValue<DateTime>(ExifTags.DateTimeDigitized,
                                                out datePictureTaken))
                {
                    // Do whatever is required with the extracted information
                    copyImage(path, datePictureTaken);
                }
            }
                
            }
            else if(Path.GetExtension(path) == ".mp4")
            {
                string dateTaken = Path.GetFileName(path).Split(char.Parse("_"))[0];
                if (dateTaken.Length != 8)
                {
                    dateTaken = Path.GetFileName(path).Split(char.Parse("_"))[1];
                }
                dateTaken = dateTaken.Insert(4, "-").Insert(7, "-");
                copyImage(path, DateTime.Parse(dateTaken));
            }
            else
            {
                unknownFiles += 1;
            }
        }
        static void copyImage(string path, DateTime dateTaken)
        {
            string tempPath = toPath + dateTaken.Year.ToString()+"\\" + DateTime.Parse(dateTaken.AddMonths(1).ToShortDateString().Remove(8, 2)+"01").AddDays(-1).ToShortDateString()+" mobilbilder";
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }
            string fileName = Path.GetFileName(path);
            if(!File.Exists(tempPath + "\\" + fileName))
            {
            copiedFiles += 1;
            File.Copy(path, tempPath+"\\"+fileName);
            Console.WriteLine(fileName +" copied");
            }
        }
    }
}
