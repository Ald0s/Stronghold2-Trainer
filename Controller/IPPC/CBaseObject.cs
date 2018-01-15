/*
== IPPC - Inter-Process Procedure Calling ==
A neat tool used to call procedures exported by an external module.
The tool is also able to get the result from the call, this can be cast into a managed struct and used from there.

By Alden Viljoen
https://github.com/ald0s
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPPC {
    public enum MessageType {
        Success,
        Information,
        Failure,

        Other = -1
    }

    public class CDebugWrapper {
        private bool bShouldPrint = true;

        public void SetShouldPrint(bool print) {
            bShouldPrint = print;
        }

        protected void ReportException(Exception exc) {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("=== UNHANDLED ERROR ===");
            sb.AppendLine("Exception Type: " + exc.GetType().Name);
            sb.AppendLine("Exception Message: " + exc.Message);
            sb.AppendLine("Exception Stack Trace: " + exc.StackTrace);

            Console.WriteLine(sb.ToString());
        }

        private void Write(MessageType type, string text) {
            if (!bShouldPrint) {
                return;
            }

            Print(type, text);
        }

        protected void WriteSuccess(string sText) {
            Write(MessageType.Success, sText);
        }

        protected void WriteInfo(string sText) {
            Write(MessageType.Information, sText);
        }

        protected void WriteError(string sText) {
            Write(MessageType.Failure, sText);
        }

        protected void Write(string sText) {
            Write(MessageType.Other, sText);
        }

        public void Print(MessageType type, string sMessageText) {
            ConsoleColor oldColour = Console.ForegroundColor;

            Console.Write("[");

            switch (type) {
                case MessageType.Success:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;

                case MessageType.Information:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;

                case MessageType.Failure:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;

                default:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
            }

            Console.Write("IPPC");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("] ");

            Console.WriteLine(sMessageText);
            Console.ForegroundColor = oldColour;
        }
    }
}
