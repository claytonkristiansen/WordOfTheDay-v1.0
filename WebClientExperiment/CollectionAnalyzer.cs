using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebClientExperiment
{
    enum SORT_DIRECTION { LOWEST_TO_HIGHEST, HIGHEST_TO_LOWEST};
    class CollectionAnalyzer
    {
        public static Word[] MostCommonWords(Dictionary<string, Word> collection, SORT_DIRECTION sortDirection)
        {
            //Initialize the tempWordArray to defualt size of 10
            Word[] tempWordArray = new Word[3];


            for (int tempCollectionIndex = 0; tempCollectionIndex < tempWordArray.Length; tempCollectionIndex++)
            {
                int i = 0;
                for (; i < collection.Count; i++)
                {
                    if (collection.ElementAtOrDefault(i).Value.m_NumberOfTimesAppears > tempWordArray[tempCollectionIndex].m_NumberOfTimesAppears)
                    {
                        tempWordArray[tempCollectionIndex] = collection.ElementAtOrDefault(i).Value;
                    }
                }
                Console.WriteLine(tempWordArray[tempCollectionIndex].m_Name + ", " + tempWordArray[tempCollectionIndex].m_NumberOfTimesAppears.ToString() + " times");
                collection.Remove(tempWordArray[tempCollectionIndex].m_Name);
                i = 0;
            }
            return tempWordArray;
        }


        public static Dictionary<string, Word> RemoveMembersBasedOnm_Name(Dictionary<string, Word> collection, string[] words)
        {
            //string[] words = { "the", "and", "of", "to", "is", "in", "class", "data", "span", "id", "or", "because", "why", "what", "how", "at", "so", "other", "many", "role", "answers", "on", "for", "like", "eat", "it", "s", "be", "this", "get", "out", "there", "from", "with", "amp", "that", "are" };
            for (int i = 0; i < collection.Count; i++)
            {
                for (int k = 0; k < words.Length; k++)
                {
                    if (collection.ElementAtOrDefault(i).Value.m_Name == words[k])
                    {
                        Console.WriteLine("Word removed: " + collection.ElementAtOrDefault(i).Value.m_Name + "                                                  ");
                        collection.Remove(collection.ElementAtOrDefault(i).Key);
                    }
                }
            }
            return collection;
            //f_EveryWordInEnglishLanguage.Close();
            //path = (Directory.GetCurrentDirectory() + @"\Assets\EveryWordInTheEnglishLanguage.txt"); //Gets the path to the HTML_Parsed.txt file
            //f_EveryWordInEnglishLanguage = new FileStream(path, FileMode.Truncate, FileAccess.ReadWrite);
            //f_EveryWordInEnglishLanguage.Position = 0;
            //int size = collection.Count;
            //for (int i = 0; i < size; i++)
            //{
            //    Console.Clear();
            //    int pos = (int)f_EveryWordInEnglishLanguage.Position;
            //    int lengthOfBytesToRead = Encoding.UTF8.GetByteCount(collection.ElementAtOrDefault(i).Value.m_Name);
            //    f_EveryWordInEnglishLanguage.Write(Encoding.UTF8.GetBytes(collection.ElementAtOrDefault(i).Value.m_Name), 0, lengthOfBytesToRead);
            //    f_EveryWordInEnglishLanguage.Write(Encoding.UTF8.GetBytes("\n"), 0, 1);
            //    Console.SetCursorPosition(0, 0);
            //    Console.WriteLine(collection.ElementAtOrDefault(i).Value.m_Name + "                                                  ");
            //}
        }
    }
}
