using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;
using Dangl.Calculator;

//---Commands available ----
// enable admin
// change password
// exit
// print <text>
// eval <expression>

namespace OSSimulator
{
     public class OperatingSystem
     {
          private System.Timers.Timer _timer;
          private bool _loading = true;
          private readonly string[] UserCliText = { "OS CLI (user)> ", "OS CLI (admin)> " };
          private int _userType = 0;
          private string _userName = "andy";
          private string _userPassword = "andy1";
          private string _adminPassword = "admin1";
          public void Run()
          {
               ShowBootUI();
               ShowAuthenticationUI();

               while (true)
               {
                    Console.Write(UserCliText[_userType]);
                    var userInput = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(userInput)) continue;

                    switch (userInput)
                    {
                         case var someVal when Regex.IsMatch(someVal, @"enable\s+admin"):
                              HandleAdminAuthCmd();
                              break;
                         case var someVal when Regex.IsMatch(someVal, @"change\s+password"):
                              HandlePasswordChangeCmd();
                              break;
                         case var someVal when Regex.IsMatch(someVal, @"exit"):
                              HandleExitCmd();
                              break;
                         case var someVal when Regex.IsMatch(someVal, @"print\s+\S+"):
                              HandlePrintCmd(someVal);
                              break;
                         case var someVal when Regex.IsMatch(someVal, @"eval\s+\S+"):
                              HandleEvalCmd(someVal);
                              break;
                         case var someVal when Regex.IsMatch(someVal, @"cls"):
                              Console.Clear();
                              break;
                         case var someVal when Regex.IsMatch(someVal, @"help"):
                              Console.WriteLine("The available commands are: \nenable admin, change password, exit, print <text>, eval <expression>, help, cls");
                              break;
                         default:
                              Console.WriteLine("Invalid command! Type help command to view the full list of commands.");
                              break;
                    }
               }
          }
          private void HandleEvalCmd(string evalCommand)
          {
               var formula = evalCommand.Replace("eval", "").Trim();
               var calculation = Calculator.Calculate(formula);
               if (!calculation.IsValid)
               {
                    Console.WriteLine("Invalid expression!");
                    return;
               }
               Console.WriteLine(calculation.Result);
          }
          private void HandlePrintCmd(string printCommand)
          {
               var printValue = printCommand.Replace("print", "").Trim();
               Console.WriteLine(printValue);
          }
          private void HandleExitCmd()
          {
               if (_userType == 0)
               {
                    Console.Clear();
                    ShowAuthenticationUI();
               }
               else if (_userType == 1)
               {
                    _userType = 0;
               }
          }
          private void HandlePasswordChangeCmd()
          {
               Console.WriteLine("Enter current password:");
               if (_userType == 0)
               {
                    var authResult = PasswordChecker(_userPassword);
                    if (!authResult) return;
                    while (true)
                    {
                         Console.WriteLine("\nEnter new user password:");
                         var newPass1 = ReadPasswordFromInput();
                         if (string.IsNullOrWhiteSpace(newPass1))
                         {
                              Console.WriteLine("\nEnter a valid password!"); continue;
                         }
                         Console.WriteLine("\nConfirm new user password:");
                         var newPass2 = ReadPasswordFromInput();
                         if (string.Equals(newPass1, newPass2))
                         {
                              Console.WriteLine("\nUser password has been changed!");
                              _userPassword = newPass1;
                              break;
                         }
                         else
                         {
                              Console.WriteLine("\nPasswords don't match. Try again!");
                         }
                    }
               }
               else if (_userType == 1)
               {
                    var authResult = PasswordChecker(_adminPassword);
                    if (!authResult) return;
                    while (true)
                    {
                         Console.WriteLine("\nEnter new admin password:");
                         var newPass1 = ReadPasswordFromInput();
                         if (string.IsNullOrWhiteSpace(newPass1))
                         {
                              Console.WriteLine("\nEnter a valid password!"); continue;
                         }
                         Console.WriteLine("\nConfirm new admin password:");
                         var newPass2 = ReadPasswordFromInput();
                         if (string.Equals(newPass1, newPass2))
                         {
                              Console.WriteLine("\nAdmin password has been changed!");
                              _userPassword = newPass1;
                              break;
                         }
                         else
                         {
                              Console.WriteLine("\nPasswords don't match. Try again!");
                         }
                    }
               }
          }
          private string ReadPasswordFromInput()
          {
               var pass = String.Empty;
               ConsoleKey key;
               do
               {
                    var keyInfo = Console.ReadKey(intercept: true);
                    key = keyInfo.Key;

                    if (key == ConsoleKey.Backspace && pass.Length > 0)
                    {
                         Console.Write("\b \b");
                         pass = pass[0..^1];
                    }
                    else if (!char.IsControl(keyInfo.KeyChar))
                    {
                         Console.Write("*");
                         pass += keyInfo.KeyChar;
                    }
               } while (key != ConsoleKey.Enter);
               return pass;
          }
          private bool PasswordChecker(string password)
          {
               while (true)
               {
                    var pass = ReadPasswordFromInput();
                    if (pass == password)
                    {
                         return true;
                    }
                    else if (Regex.IsMatch(pass, @"exit"))
                    {
                         Console.WriteLine("\n");
                         return false;
                    }
                    else
                    {
                         Console.WriteLine("\nInvalid password. Reenter the password or type exit command to return.");
                    }
               }
          }
          private void HandleAdminAuthCmd()
          {
               Console.WriteLine("Enter the administrator password:");
               var authResult = PasswordChecker(_adminPassword);
               if (!authResult) return;
               Console.WriteLine("\nAccess granted!\n");
               _userType = 1;
          }
          private void ShowAuthenticationUI()
          {
               Console.Write("Press <Enter> to log in or CTRL-C to exit! ");
               while (Console.ReadKey().Key != ConsoleKey.Enter) { }
               Console.WriteLine("\nEnter the OS username:");
               while (Console.ReadLine() != _userName)
               {
                    Console.WriteLine("Invalid username");
               }
               Console.WriteLine("Enter the OS user password:");
               var authResult = PasswordChecker(_userPassword);
               if(!authResult) Environment.Exit(0);
               Console.WriteLine("\nAccess granted!\n");
          }
          private void ShowBootUI()
          {
               Console.WriteLine("Booting into the OS...");
               _timer = new System.Timers.Timer(1000);
               _timer.Elapsed += OnTimedEvent;
               _timer.Enabled = true;
               var s = new ConsoleSpinner();
               while (_loading)
               {
                    Thread.Sleep(100);
                    s.UpdateProgress();
               }
               Console.Clear();
               Console.WriteLine(@"

________    _________   _________.__              .__          __                
\_____  \  /   _____/  /   _____/|__| _____  __ __|  | _____ _/  |_  ___________ 
 /   |   \ \_____  \   \_____  \ |  |/     \|  |  \  | \__  \\   __\/  _ \_  __ \
/    |    \/        \  /        \|  |  Y Y  \  |  /  |__/ __ \|  | (  <_> )  | \/
\_______  /_______  / /_______  /|__|__|_|  /____/|____(____  /__|  \____/|__|   
        \/        \/          \/          \/                \/                   ");

               Console.WriteLine(@"
**************************** WARNING ********************************
*                                                                   *
*  If you are not authorized to access this system, disconnect now. *
*                                                                   *    
*********************************************************************");
          }
          private void OnTimedEvent(Object source, ElapsedEventArgs e)
          {
               _loading = false;
          }
     }
}
