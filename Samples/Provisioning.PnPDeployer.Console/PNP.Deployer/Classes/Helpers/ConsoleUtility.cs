using System;
using System.Security;


// =======================================================
/// <author>
/// Simon-Pierre Plante (sp.plante@gmail.com)
/// </author>
// =======================================================
namespace PNP.Deployer
{
    public static class ConsoleUtility
    {
        #region Private Methods

        // ===========================================================================================================
        /// <summary>
        /// Prompts the user with the specified label and returns it's input
        /// </summary>
        /// <param name="label">The label that will be prompted to the user</param>
        /// <param name="isPassword">Whether the input should be of type 'password' or not</param>
        /// <returns>The user's input as a <b>String</b></returns>
        // ===========================================================================================================
        private static string GetInput(string label, bool isPassword)
        {
            ConsoleColor defaultForeground = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("{0} : ", label);
            Console.ForegroundColor = defaultForeground;

            string value = "";

            for (ConsoleKeyInfo keyInfo = Console.ReadKey(true); keyInfo.Key != ConsoleKey.Enter; keyInfo = Console.ReadKey(true))
            {
                if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (value.Length > 0)
                    {
                        value = value.Remove(value.Length - 1);
                        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                        Console.Write(" ");
                        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    }
                }
                else if (keyInfo.Key != ConsoleKey.Enter)
                {
                    if (isPassword)
                    {
                        Console.Write("*");
                    }
                    else
                    {
                        Console.Write(keyInfo.KeyChar);
                    }
                    value += keyInfo.KeyChar;
                }
            }
            Console.WriteLine("");

            return value;
        }

        #endregion


        #region Public Methods

        // ===========================================================================================================
        /// <summary>
        /// Prompts the user with the specified label and returns it's clear text answer
        /// </summary>
        /// <param name="label">The label that will be prompted to the user</param>
        /// <returns>The user's input as a clear string object</returns>
        // ===========================================================================================================
        public static string GetInputAsText(string label)
        {
            return GetInput(label, false);
        }


        // ===========================================================================================================
        /// <summary>
        /// Prompts the user with the specified label and returns it's secure answer
        /// </summary>
        /// <param name="label">The label that will be prompted to the user</param>
        /// <returns>The user's input as a SecureString object</returns>
        // ===========================================================================================================
        public static SecureString GetInputAsSecureString(string label)
        {
            string input = GetInput(label, true);
            return input.ToSecureString(); 
        }

        #endregion
    }
}
