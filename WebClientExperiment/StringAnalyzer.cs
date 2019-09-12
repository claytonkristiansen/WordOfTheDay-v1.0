using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Timers;
using System.Diagnostics;
using Tweetinvi;

namespace WebClientExperiment
{
    public class StringAnalyzer
    {

        private string m_MainString;
        public StringAnalyzer(string MainString)
        {
            m_MainString = MainString;
        }

        public void SetMainString(string mainString)
        {
            m_MainString = mainString;
        }

        /// <summary>
        /// Finds the number of times word appears in m_MainString
        /// </summary>
        /// <param name="m_MainString"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        public static int NumWordCount(string m_MainString, string word)
        {
            ///<summary>
            ///Variable to contain each word that is detected
            ///</summary>
            char[] tempWord = new char[10];
            ///<summary>
            ///Are we reading from text that is after a space?
            ///</summary>
            bool onWord = false;
            int numberOfTimesWordAppears = 0;
            ///<summary>
            ///Idexer for the tempWord variable
            ///</summary>
            int index = 0;
            int numCharactersDelayed = 0;
            for (int i = 0; i < m_MainString.Length; i++)
            {
                int numCharacters = 0;
                if ((IsBetweenFunction(m_MainString[i], 'A', 'Z', BETWEEN_TYPE.INCLUSIVE)
                     ||
                     IsBetweenFunction(m_MainString[i], 'a', 'z', BETWEEN_TYPE.INCLUSIVE)
                     )
                     && onWord == false)
                {
                    onWord = true;
                    numCharacters = 0;
                    for (int k = i;
                        k <= m_MainString.Length - 1 && (IsBetweenFunction(m_MainString[k], 'A', 'Z', BETWEEN_TYPE.INCLUSIVE)
                        ||
                        IsBetweenFunction(m_MainString[k], 'a', 'z', BETWEEN_TYPE.INCLUSIVE));
                        k++)
                    {
                        numCharacters++;
                    }
                    tempWord = new char[numCharacters];
                    numCharactersDelayed = numCharacters;
                }

                if (onWord
                    &&
                    (IsBetweenFunction(m_MainString[i], 'A', 'Z', BETWEEN_TYPE.INCLUSIVE)
                    ||
                    IsBetweenFunction(m_MainString[i], 'a', 'z', BETWEEN_TYPE.INCLUSIVE)))
                {
                    int fsad = tempWord.Length;
                    int dasf = m_MainString.Length;
                    tempWord[index] = m_MainString[i];
                    index++;
                    if (index >= numCharactersDelayed)
                    {
                        string comparativeString = new string(tempWord);
                        if (comparativeString.ToLower() == word.ToLower())
                        {
                            numberOfTimesWordAppears++;
                            onWord = false;
                        }
                    }
                }
                else
                {
                    index = 0;
                    onWord = false;
                }
            }
            return numberOfTimesWordAppears;
        }

        /// <summary>
        /// Non asyncronously returns a Dictionary where each object in the collection has the number of times that word appears in the string
        /// </summary>
        /// <param name="constCollection"></param>
        /// <param name="stringToAnalyze"></param>
        /// <returns></returns>
        public static Dictionary<string, Word> NonAsyncGetWordCounts(Dictionary<string, Word> constCollection, string textToAnalyze)
        {
            Dictionary<string, Word> tempCollection = constCollection;

            int count = 0;
            int countTotal = 0;
            float timePassed = 0;
            Stopwatch sw = new Stopwatch();
            for (int i = 0; i < tempCollection.Count - 1; i++)
            {
                if (count == 0)
                {
                    sw.Start();
                }
                count++;
                countTotal++;
                Word word = tempCollection.ElementAtOrDefault(i).Value;
                word.m_NumberOfTimesAppears = StringAnalyzer.NumWordCount(textToAnalyze, tempCollection.ElementAtOrDefault(i).Value.m_Name);
                tempCollection[word.m_Name] = word;
                float numRemaining = (tempCollection.Count - (i + 1));
                float timeRemaining = 0;
                if (count >= 100)
                {
                    sw.Stop();
                    count = 0;
                    timePassed += (float)sw.ElapsedMilliseconds;
                    timeRemaining = (float)((timePassed / countTotal) * (float)numRemaining);

                    sw.Reset();
                    Console.Clear();
                    float secondsRemaining = timeRemaining / 1000;
                    int secondsRemainder = (int)secondsRemaining % 60;
                    int minutesRemaining = ((int)secondsRemaining - secondsRemainder) / 60;
                    Console.WriteLine("Word: " + word.m_Name);
                    Console.WriteLine("Time Remaining: " + minutesRemaining.ToString() + " min and " + secondsRemainder.ToString() + " sec ");
                }
            }
            return tempCollection;
        }

        /// <summary>
        /// Returns whether number is between min and max
        /// </summary>
        /// <param name="number"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="betweenType"></param>
        /// <returns></returns>
        public static bool IsBetweenFunction(int number, int min, int max, BETWEEN_TYPE betweenType)
        {
            if (betweenType == BETWEEN_TYPE.EXCLUSIVE)
            {
                if (number > min && number < max)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            if (betweenType == BETWEEN_TYPE.INCLUSIVE)
            {
                if (number >= min && number <= max)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Creates a collection of instances of te Word class based on the string in _String
        /// </summary>
        /// <param name="_String"></param>
        /// <returns></returns>
        public static Dictionary<string, Word> CreateCollectionOfWords(string _String)
        {
            Dictionary<string, Word> Collection = new Dictionary<string, Word>(); //Collection for storing the collection of words created from _String

            char[] tempWord = new char[256];

            bool onWord = false; //Bool used to incicate whether the program is reading a word
            
            int index = 0; //Idexer for the tempWord variable

            int numCharactersDelayed = 0;

            for (int i = 0; i < _String.Length; i++)
            {
                int numCharacters = 0;
                if ((IsBetweenFunction(_String[i], 'A', 'Z', BETWEEN_TYPE.INCLUSIVE)
                     ||
                     IsBetweenFunction(_String[i], 'a', 'z', BETWEEN_TYPE.INCLUSIVE)
                     )
                     && onWord == false)
                {
                    onWord = true;
                    numCharacters = 0;
                    for (int k = i;
                        k <= _String.Length - 1 && (IsBetweenFunction(_String[k], 'A', 'Z', BETWEEN_TYPE.INCLUSIVE)
                        ||
                        IsBetweenFunction(_String[k], 'a', 'z', BETWEEN_TYPE.INCLUSIVE));
                        k++)
                    {
                        numCharacters++;
                    }
                    tempWord = new char[numCharacters];
                    numCharactersDelayed = numCharacters;
                }

                if (onWord
                    &&
                    (IsBetweenFunction(_String[i], 'A', 'Z', BETWEEN_TYPE.INCLUSIVE)
                    ||
                    IsBetweenFunction(_String[i], 'a', 'z', BETWEEN_TYPE.INCLUSIVE)))
                {
                    tempWord[index] = _String[i];
                    index++;
                    if (index >= numCharactersDelayed)
                    {
                        string comparativeString = new string(tempWord);
                        Word word = new Word();
                        word.m_Name = comparativeString;
                        word.m_index = 0;
                        word.m_definition = "definition";
                        index = i;
                        if (!Collection.ContainsKey(word.m_Name))
                        {
                            Collection.Add(word.m_Name, word);
                        }
                        onWord = false;
                    }
                }
                else
                {
                    index = 0;
                    onWord = false;
                }
            }

            return Collection;
        }














    }











    public struct Word
    {
        public string m_Name;
        public int m_index;
        public int m_NumberOfTimesAppears;
        public string m_definition;
    }
}
