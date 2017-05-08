using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ionic.Zip;

namespace BundleZip
{
    class Program
    {

		public static string getTimeStamp ()
		{
			return DateTime.Now.ToString ("MM/dd/yyyy HH:mm:ss.fffffff ");
		}

		public static void printStatementToConsole (string result)
		{
			Console.WriteLine (getTimeStamp () + " " + result);
		}

		public static string getFileNameWithoutPrefix(string result) {
			int splitLoc = result.IndexOf("_") + 1;
			string tmpfilename = result.Substring(splitLoc);
			printStatementToConsole(result+":"+tmpfilename);
			return tmpfilename;
		}

		public static List<string> getListOfPDFFilenames(string[] pdfFilesInDir, List<string> zipGroup) {
			foreach (string dir in pdfFilesInDir)
			{
				//string result = Path.GetFileName(dir);
				if (!String.IsNullOrEmpty(dir)) {
					string result = Path.GetFileNameWithoutExtension(dir);
					result = result.Trim();
					if (!String.IsNullOrEmpty (result)) {
						string tmpfilename = getFileNameWithoutPrefix (result).Trim ().ToLower ();
						if (!zipGroup.Contains (tmpfilename)) {
							printStatementToConsole ("EMAIL DOES NOT EXIST IN THE LIST. SO ADDING TO LIST:" + tmpfilename);
							zipGroup.Add (tmpfilename);
						}
					} else {
						File.Delete (dir);
						printStatementToConsole ("DELETING THE FILE WITH IMPROPER NAME: " + dir);
					}
				}  else {
					File.Delete (dir);
					printStatementToConsole ("DELETING THE FILE WITH IMPROPER NAME: " + dir);
				}
			}
			return zipGroup;
		}

		public static void zipPDFFilesAsPerFileName(List<string> zipGroup, string procdir) {
			string zipdir = "d:/conversion/emeabilling.zip/";
			string bkdir = "d:/conversion/emeabilling.bk/";
			foreach (string zipit in zipGroup)
			{
				printStatementToConsole("Starting ZIP PROCESS for :" + zipit);
				string ts = DateTime.Now.ToString("yyyyMMddHHmm");
				string zipname = ts+"_"+zipit+".zip";
				string fullzippath = zipdir+zipname;
				string filemask = "*"+zipit+".pdf";
				printStatementToConsole("FILEMASK SET TO:"+filemask);
				string[] pdfFilesListWithSpecFileName = Directory.GetFiles(procdir, filemask, SearchOption.TopDirectoryOnly);
				printStatementToConsole("PDF's Found: " + pdfFilesListWithSpecFileName.Length);
				if (pdfFilesListWithSpecFileName.Length < 1) {
					printStatementToConsole("No pdf files were found for file mask " + filemask + " so returning without zipping.");
				} else {
					printStatementToConsole("CREATE ZIP FILE:" + zipname);
					ZipFile zp = new ZipFile();
					foreach (string zdir in pdfFilesListWithSpecFileName)
					{
						zp.AddFile(zdir, "");                   
						printStatementToConsole("ADD TO ZIP:" + zdir);
					}
					printStatementToConsole("START SAVING TO ZIP:" + fullzippath);
					zp.Save(fullzippath);
					printStatementToConsole("COMPLETED SAVING TO ZIP:" + fullzippath);
					foreach (string zdir in pdfFilesListWithSpecFileName)
					{
						string dt = System.DateTime.Now.ToString("yyyyMMddHHmm");
						string basefile = Path.GetFileName(zdir);
						string backupFile = bkdir + basefile+dt;
						printStatementToConsole("Moving to Backup:" + zdir);
						printStatementToConsole("Moving:" + zdir);
						printStatementToConsole("    -->" + backupFile);
						File.Move(zdir, backupFile);
					}
				}
			}
		}

        static void Main(string[] args)
        {
			string procdir = "d:/conversion/emeabilling/";
            List<string> zipGroup = new List<string>();
             //Directory.GetFiles(procdir, "*", SearchOption.TopDirectoryOnly);
            string[] pdfFilesInDir = Directory.GetFiles(procdir, "*.pdf", SearchOption.TopDirectoryOnly);
			printStatementToConsole("PDF's Found: " + pdfFilesInDir.Length);
			zipGroup = getListOfPDFFilenames (pdfFilesInDir, zipGroup);
			zipPDFFilesAsPerFileName (zipGroup, procdir);
        }
    }
}
