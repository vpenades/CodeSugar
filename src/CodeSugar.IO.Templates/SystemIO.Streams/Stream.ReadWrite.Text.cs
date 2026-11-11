using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Collections.Generic;
using System.IO;

using System.Linq;


#nullable disable

using __STREAMFUNC = System.Func<System.IO.FileMode, System.IO.Stream>;
using __STREAMTASK = System.Func<System.IO.FileMode, System.Threading.CancellationToken, System.Threading.Tasks.Task<System.IO.Stream>>;

using __READSTREAM = System.IO.Stream;
using __WRITESTREAM = System.IO.Stream;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarExtensions
    {
        #if NETSTANDARD || NETFRAMEWORK
        private static readonly Encoding UTF8NoBOM = new UTF8Encoding(false);
        #endif

        public static async Task<IReadOnlyList<string>> ReadAllLinesAsync(this __STREAMTASK streamTask, CancellationToken ctoken = default, Encoding encoding = null)
        {
            using (var s = await streamTask.OpenReadAsync(ctoken).ConfigureAwait(false))
            {
                return await ReadAllLinesAsync(s, ctoken, encoding).ConfigureAwait(false);
            }
        }

        public static async Task<IReadOnlyList<string>> ReadAllLinesAsync(this __READSTREAM stream, CancellationToken ctoken = default, Encoding encoding = null)
        {
            using (var sr = CreateTextReader(stream, true, encoding))
            {
                string line;
                var lines = new List<string>();

                while ((line = await sr.ReadLineAsync().ConfigureAwait(false)) != null)
                {
                    lines.Add(line);
                }

                return lines;
            }
        }

        public static IReadOnlyList<string> ReadAllLines(this __STREAMFUNC openStream, Encoding encoding = null)
        {
            using (var s = openStream(_MODEREAD))
            {
                return ReadAllLines(s, encoding);
            }
        }

        public static IReadOnlyList<string> ReadAllLines(this __READSTREAM stream, Encoding encoding = null)
        {
            using (var sr = CreateTextReader(stream, true, encoding))
            {
                string line;
                var lines = new List<string>();

                while ((line = sr.ReadLine()) != null)
                {
                    lines.Add(line);
                }

                return lines;
            }
        }

        public static void WriteAllLines(this __STREAMFUNC streamFunc, Encoding encoding, params string[] lines)
        {
            using (var stream = streamFunc.OpenWrite())
            {
                WriteAllLines(stream, encoding, lines);
            }
        }

        public static void WriteAllLines(this __WRITESTREAM stream, Encoding encoding, params string[] lines)
        {
            WriteAllLines(stream, lines.AsEnumerable(), encoding);
        }

        public static void WriteAllLines(this __STREAMFUNC streamFunc, IEnumerable<string> lines, Encoding encoding = null)
        {
            using (var stream = streamFunc.OpenWrite())
            {
                WriteAllLines(stream, lines, encoding);
            }
        }

        public static void WriteAllLines(this __WRITESTREAM stream, IEnumerable<string> lines, Encoding encoding = null)
        {
            using (var sw = CreateTextWriter(stream, true, encoding))
            {
                foreach (var line in lines)
                {
                    sw.WriteLine(line);
                }
            }
        }

        /// <summary>
        /// Creates a <see cref="StreamWriter"/> from the given <see cref="System.IO.Stream"/>
        /// </summary>
        public static StreamWriter CreateTextWriter(this __WRITESTREAM stream, bool leaveStreamOpen = true, Encoding encoding = null)
        {
            GuardWriteable(stream);

            #if NETSTANDARD || NETFRAMEWORK
            // NetStd implementation matching net6
            return new StreamWriter(stream, encoding ?? UTF8NoBOM, 1024, leaveStreamOpen);
            #else
            return new StreamWriter(stream, encoding: encoding, leaveOpen: leaveStreamOpen);
            #endif
        }

        /// <summary>
        /// Creates a <see cref="StreamReader"/> from the given <see cref="System.IO.Stream"/>
        /// </summary>
        public static StreamReader CreateTextReader(this __READSTREAM stream, bool leaveStreamOpen = true, Encoding encoding = null)
        {
            GuardReadable(stream);

            #if NETSTANDARD || NETFRAMEWORK
            // NetStd implementation matching net6
            return new StreamReader(stream, encoding ?? Encoding.UTF8, true, 1024, leaveStreamOpen);
            #else
            return new StreamReader(stream, encoding: encoding, leaveOpen: leaveStreamOpen);
            #endif
        }

        public static void WriteAllText(this __STREAMFUNC stream, string contents, Encoding encoding = null)
        {
            using (var s = stream.OpenWrite())
            {
                WriteAllText(s, contents, encoding);
            }
        }

        /// <summary>
        /// writes all the text from the given stream.
        /// Equivalent to <see cref="System.IO.File.WriteAllText(string, string?, Encoding)"/>
        /// </summary>   
        public static void WriteAllText(this __WRITESTREAM stream, string contents, Encoding encoding = null)
        {
            GuardWriteable(stream);

            contents ??= string.Empty;

            using (var ss = CreateTextWriter(stream, true, encoding))
            {
                ss.Write(contents);
            }
        }

        public static string ReadAllText(this __STREAMFUNC openStream, Encoding encoding = null)
        {
            using (var s = openStream(_MODEREAD))
            {
                return ReadAllText(s, encoding);
            }
        }

        /// <summary>
        /// Reads all the text from the given stream.
        /// Equivalent to <see cref="System.IO.File.ReadAllText(string, Encoding)"/>
        /// </summary>   
        public static string ReadAllText(this __READSTREAM stream, Encoding encoding = null)
        {
            GuardReadable(stream);

            using (var sr = CreateTextReader(stream, true, encoding))
            {
                return sr.ReadToEnd();
            }
        }

        public static async Task<string> ReadAllTextAsync(this __STREAMTASK streamTask, CancellationToken ctoken, Encoding encoding = null)
        {
            using (var s = await streamTask.OpenReadAsync(ctoken).ConfigureAwait(false))
            {
                return await ReadAllTextAsync(s, ctoken, encoding).ConfigureAwait(false);
            }
        }

        public static async Task<string> ReadAllTextAsync(this __READSTREAM stream, CancellationToken ctoken, Encoding encoding = null)
        {
            GuardReadable(stream);

            using (var sr = CreateTextReader(stream, true, encoding))
            {
                return await sr.ReadToEndAsync().ConfigureAwait(false);
            }
        }
    }
}
