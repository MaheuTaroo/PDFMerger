using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace PDFMerger
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IEnumerable<string> files = args.Length == 0 ? ReadLines("Document file to merge (one per line; leave blank to end): ") : args;

            string? output;
            do
            {
                Console.Write("Path to merged file:");
                output = Console.ReadLine();
            } while (output == null || output == string.Empty);

            Merge(files, output);
        }

        static List<string> ReadLines(string? before)
        {
            List<string> lines = [];

            string? line;
            while (true)
            {
                Console.Write(before);
                line = Console.ReadLine();
                if (line == null || line == string.Empty) break;
                lines.Add(line);
            }

            return lines;
        }

        static void Merge(IEnumerable<string> files, string outPath)
        {
            if (!files.Any())
            { 
                Console.WriteLine("Cannot merge: no files to work with");
                return;
            }

            using PdfDocument output = new(outPath);
            try
            {
                if (Directory.Exists(outPath)) outPath += Path.DirectorySeparatorChar + "merged.pdf";

                foreach (string file in files)
                {
                    string path = file.Contains('"') ? file.Replace("\"", string.Empty) : file;

                    if (!File.Exists(path))
                    {
                        Console.WriteLine($"File {path} does not exist; skipping...");
                        continue;
                    }

                    using PdfDocument doc = PdfReader.Open(path, PdfDocumentOpenMode.Import);
                    foreach (PdfPage page in doc.Pages)
                        output.AddPage(page);
                }

                output.Close();
            }
            catch (IOException io) 
            {
                Console.WriteLine("Could not merge the files: " + io.Message.ToLower());
            }
            finally
            {
                if (output.PageCount == 0)
                    Console.WriteLine("No files have been merged.");
            }
        }
    }
}
