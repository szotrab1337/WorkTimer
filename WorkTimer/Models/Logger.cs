using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace WorkTimer.Models
{
    public static class Logger
    {
        public static async Task<string> LogMessage(string Message, int type = 1)
        {
            string output = "";

            try
            {
                //StreamWriter sw = null;

                if (type < 0 || type >= msgTypes.Count)
                {
                    type = 1;
                }

                string sLogFormat = GetLogPrefix(type, false);
                string sFilename = "WorkTimer_Logs_";

                string sDate = DateTime.Now.ToString("yyyy-MM-dd");
                string sMsgType = "[" + msgTypes.ElementAt(type) + "]";

                sFilename = sFilename + sDate + ".txt";

                StorageFolder sFolder = ApplicationData.Current.LocalFolder;
                StorageFile sFile = await sFolder.CreateFileAsync(sFilename, Windows.Storage.CreationCollisionOption.OpenIfExists);
                await FileIO.AppendTextAsync(sFile, "\n" + sLogFormat + Message);

                output = sLogFormat + Message;
                Console.WriteLine(output);

                await App.Database.AddNewLog(new Log
                {
                    Description = Message,
                    CreatedOn = DateTime.Now,
                    Type = msgTypes.ElementAt(type)
                });
            }
            catch (Exception ex)
            {
            }

            return output;
        }

        private static string GetLogPrefix(int Code, bool ShortDateTime)
        {
            string Prefix = DateTime.Now.ToString((!ShortDateTime ? "yyyy-MM-dd " : "") + "HH:mm" + (!ShortDateTime ? ":ss.fff" : ""))
                    + " [" + msgTypes.ElementAt(Code) + "] ";

            return Prefix;
        }

        private static List<string> msgTypes = new List<string>()
        {
            "Success", "Info", "Warning", "Error"
        };
    }
}
