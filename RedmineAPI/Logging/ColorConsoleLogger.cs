/*
   Copyright 2011 - 2016 Adrian Popescu.

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/


namespace Redmine.Net.Api.Logging
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Redmine.Net.Api.Logging.ILogger" />
    public class ColorConsoleLogger : ILogger
    {
        private static readonly object locker = new object();
        private readonly System.ConsoleColor? defaultConsoleColor = null;

        /// <summary>
        /// </summary>
        /// <param name="entry"></param>
        public void Log(LogEntry entry)
        {
            lock (locker)
            {
                ConsoleColors colors = GetLogLevelConsoleColors(entry.Severity);
                switch (entry.Severity)
                {
                    case LoggingEventType.Debug:
                        System.Console.WriteLine(entry.Message, colors.Background, colors.Foreground);
                        break;
                    case LoggingEventType.Information:
                        System.Console.WriteLine(entry.Message, colors.Background, colors.Foreground);
                        break;
                    case LoggingEventType.Warning:
                        System.Console.WriteLine(entry.Message, colors.Background, colors.Foreground);
                        break;
                    case LoggingEventType.Error:
                        System.Console.WriteLine(entry.Message, colors.Background, colors.Foreground);
                        break;
                    case LoggingEventType.Fatal:
                        System.Console.WriteLine(entry.Message, colors.Background, colors.Foreground);
                        break;
                }
            }
        }

        /// <summary>
        /// Gets the log level console colors.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <returns></returns>
        private ConsoleColors GetLogLevelConsoleColors(LoggingEventType logLevel)
        {
            // do not change user's background color except for Critical
            switch (logLevel)
            {
                case LoggingEventType.Fatal:
                    return new ConsoleColors(System.ConsoleColor.White, System.ConsoleColor.Red);
                case LoggingEventType.Error:
                    return new ConsoleColors(System.ConsoleColor.Red, defaultConsoleColor);
                case LoggingEventType.Warning:
                    return new ConsoleColors(System.ConsoleColor.DarkYellow, defaultConsoleColor);
                case LoggingEventType.Information:
                    return new ConsoleColors(System.ConsoleColor.DarkGreen, defaultConsoleColor);
                default:
                    return new ConsoleColors(System.ConsoleColor.Gray, defaultConsoleColor);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private struct ConsoleColors
        {
            public ConsoleColors(System.ConsoleColor? foreground, System.ConsoleColor? background): this()
            {
                Foreground = foreground;
                Background = background;
            }

            /// <summary>
            /// Gets or sets the foreground.
            /// </summary>
            /// <value>
            /// The foreground.
            /// </value>
            public System.ConsoleColor? Foreground { get; private set; }

            /// <summary>
            /// Gets or sets the background.
            /// </summary>
            /// <value>
            /// The background.
            /// </value>
            public System.ConsoleColor? Background { get; private set; }
        }
    }
}