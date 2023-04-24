using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ngram
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, string[]> nounIndices = new Dictionary<string, string[]>();
            Dictionary<string, string> nounDefinitions = new Dictionary<string, string>();
            string fileContent = "";
            string[][] input = null;
            try
            {
                if(args.Length > 0)
                {
                    using (StreamReader sr = new StreamReader(args[0]))
                    {
                        string line;
                        while (!sr.EndOfStream)
                        {
                            line = sr.ReadLine();
                            fileContent += line + "\n";
                            string[] sentences = line.Split('.', StringSplitOptions.RemoveEmptyEntries);
                            input = new string[sentences.Length][];
                            for (int i = 0; i < sentences.Length; i++)
                            {
                                input[i] = sentences[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                            }
                        }
                    }
                }
                using(StreamReader sr = new StreamReader("NounsIndex.txt"))
                {
                    string line;
                    while(!sr.EndOfStream)
                    {
                        line = sr.ReadLine();
                        string[] splitOnNoun = line.Split('|');
                        string[] splitOnIndices = splitOnNoun[1].Split(',').ToArray();
                        nounIndices.Add(splitOnNoun[0], splitOnIndices);
                    }
                }

                using (StreamReader sr = new StreamReader("NounsData.txt"))
                {
                    string line;
                    while(!sr.EndOfStream)
                    {
                        line = sr.ReadLine();
                        string[] split = line.Split('|');
                        nounDefinitions.Add(split[0], split[1]);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("missing file!");
                Console.WriteLine(ex.Message);
            }


            List<string> two_ngram = new List<string>();
            List<string> three_ngram = new List<string>();
            List<string> four_ngram = new List<string>();
            foreach (string[] sentence in input)
            {
                fileContent += "\n" + string.Join(" ",sentence) + "\n";

                for (int i = 0; i < sentence.Length - 1; i++)
                    two_ngram.Add(sentence[i] + "_" + sentence[i + 1]);

                for (int i = 0; i < sentence.Length - 2; i++) 
                    three_ngram.Add(sentence[i] + "_" + sentence[i + 1] + "_" + sentence[i+2]);
               
                for (int i = 0; i < sentence.Length - 3; i++)
                    four_ngram.Add(sentence[i] + "_" + sentence[i + 1] + "_" + sentence[i + 2] + "_" + sentence[i + 3]);


                fileContent += "\n2 level n-gram\n\n";
                foreach (string str in two_ngram)
                {
                    fileContent += str + ", ";

                    string strTemp = str.ToLower();
                    if (nounIndices.ContainsKey(strTemp))
                    {
                        foreach (string index in nounIndices[strTemp])
                        {
                            fileContent += nounDefinitions[index].Replace(";", "<< and >>");
                            fileContent += "<< or >>";
                        }

                        fileContent = fileContent.Remove(fileContent.Length - 8);
                    }

                    fileContent += "\n";
                }

                fileContent += "\n3 level n-gram\n\n";
                foreach (string str in three_ngram)
                {
                    fileContent += str + ", ";

                    string strTemp = str.ToLower();
                    if (nounIndices.ContainsKey(strTemp))
                    {
                        foreach (string index in nounIndices[strTemp])
                        {
                            fileContent += nounDefinitions[index].Replace(";", "<< and >>");
                            fileContent += "<< or >>";
                        }

                        fileContent = fileContent.Remove(fileContent.Length - 8);
                    }

                    fileContent += "\n";
                }

                fileContent += "\n4 level n-gram\n\n";
                foreach (string str in four_ngram)
                {
                    fileContent += str + ", ";
                    string strTemp = str.ToLower();
                    if (nounIndices.ContainsKey(strTemp))
                    {
                        foreach (string index in nounIndices[strTemp])
                        {
                            fileContent += nounDefinitions[index].Replace(";", "<< and >>");
                            fileContent += "<< or >>";
                        }

                        fileContent = fileContent.Remove(fileContent.Length - 8);
                    }

                    fileContent += "\n";
                }

                two_ngram.Clear();
                three_ngram.Clear();
                four_ngram.Clear();
            }

            Console.WriteLine(fileContent);

            try
            {
                StreamWriter sw = new StreamWriter("debug.txt");
                sw.Write(fileContent);
                sw.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }
    }
}
