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


using HtmlAgilityPack;
using Tweetinvi.Models;

namespace WebClientExperiment
{

    public enum BETWEEN_TYPE { INCLUSIVE = 0, EXCLUSIVE};
    public enum CHOICE { SCAN_FOR_MOST_COMMON_WORD, SEARCH_FOR_SPECIFIC_WORD, MOST_COMMON_WORD_ALREADY_DETECTED, EXIT, NOTHING};
    public enum ANALYZE { TWITTER, HTML, REMOVE_WORDS};

    class Program
    {
        /// <summary>
        /// Returns an array of the most common words in a string as well as printing the most common word to a txt file
        /// </summary>
        /// <param name="f_MostCommonWord"></param>
        /// <param name="EveryWordString"></param>
        /// <param name="textToAnalyze"></param>
        /// <returns></returns>
        static Word[] ScanForMostCommonWord(FileStream f_MostCommonWord, string EveryWordString, string textToAnalyze)
        {
            string path = (/*Directory.GetCurrentDirectory() + */@"Assets\MostCommonWord.txt"); //Gets the path to the HTML_Parsed.txt file
            f_MostCommonWord.Dispose(); //Closes the file so it can be reopened again
            f_MostCommonWord = new FileStream(path, FileMode.Truncate, FileAccess.ReadWrite); //Initializes the FileStream to the file with truncate (erases the data in it) and with Read and write permisions
            Dictionary<string, Word> constCollection = StringAnalyzer.CreateCollectionOfWords(EveryWordString); //Creates a collection out of the EveryWordString where each set of characters between the \n and \n is an instance of the Word class
            Dictionary<string, Word> tempCollection = StringAnalyzer.NonAsyncGetWordCounts(constCollection, textToAnalyze); //Gets a Dictionary<string, Word> of all the word counts in the textToAnalyze
            Word[] MostCommonWordsArray = CollectionAnalyzer.MostCommonWords(tempCollection, SORT_DIRECTION.HIGHEST_TO_LOWEST);
            f_MostCommonWord.Position = 0; //Setting the read/write position to 0 bytes offset
            byte[] bytes = Encoding.UTF8.GetBytes(MostCommonWordsArray[0].m_Name);
            int bytesToRead = Encoding.UTF8.GetBytes(MostCommonWordsArray[0].m_Name).Length;
            f_MostCommonWord.Write(bytes, 0, bytesToRead); //Reading bytes from f_HTML_Parsed
            f_MostCommonWord.Dispose();
            //f_MostCommonWord.Close();
            //f_MostCommonWord = null;
            return MostCommonWordsArray;
        }
        static void Main(string[] args)
        {
            string textToAnalyze = "";
            string path = (/*Directory.GetCurrentDirectory() + */@"Assets\MostCommonWord.txt"); //Gets the path to the HTML_Parsed.txt file
            FileStream f_MostCommonWord = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            ANALYZE whatToAnalyze = ANALYZE.TWITTER;
            bool inputGot = false;
            while (!inputGot)
            {
                Console.WriteLine("1. Scan Twitter"); Console.WriteLine("2. Scan HTML");
                if(Console.ReadKey().KeyChar == '1')
                {
                    inputGot = true;
                    whatToAnalyze = ANALYZE.TWITTER;
                    Console.Clear();
                }
                else if(Console.ReadKey().KeyChar == '2')
                {
                    inputGot = true;
                    whatToAnalyze = ANALYZE.HTML;
                    Console.Clear();
                }
            }
            switch(whatToAnalyze)
            {
                case ANALYZE.TWITTER:

                    Console.WriteLine("Get new Twitter data? 1:Yes 2:No");
                    char input = Console.ReadKey().KeyChar;

                    FileStream f_tweetOutput = new FileStream(/*Directory.GetCurrentDirectory() + */@"Assets\tweetOutput.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);

                    if (input == '1')
                    {
                        // Set up your credentials (https://apps.twitter.com)
                        Auth.SetUserCredentials("rWTNstyPs5g6lxOczRAgeUo8K", "vcPD7MKooIecHmSvtwZgX3FosSFc0aHsoEIftTMX5WZEpt4AAu", "872103516558340097-jAseO4wGw4J5RXDbzNI2dRCCo4tIEla", "x9etB65WCJWrZHM1Ezb43tMadUvbKJ7glqxinMZpd5m58");

                        // Get my Home Timeline
                        IEnumerable<ITweet> tweets = Timeline.GetHomeTimeline();

                        ITweet[] tweetArray = tweets.ToArray<ITweet>();

                        
                        for (int i = 0; i < tweetArray.Length; i++)
                        {
                            f_tweetOutput.Write(Encoding.UTF8.GetBytes(tweetArray[i].Text), 0, Encoding.UTF8.GetByteCount(tweetArray[i].Text));
                        }
                        var user = User.GetAuthenticatedUser();
                        Console.WriteLine(user.ScreenName);
                    }
                    byte[] bytfd = new byte[f_tweetOutput.Length];
                    int readLength = bytfd.Length;
                    f_tweetOutput.Position = 0;
                    f_tweetOutput.Read(bytfd, 0, readLength);
                    textToAnalyze = Encoding.UTF8.GetString(bytfd);
                    break;
                case ANALYZE.HTML:
                    //WebClient stuff
                    System.Net.WebClient wc = new System.Net.WebClient(); //Initialize the WebClient wc
                    byte[] data = wc.DownloadData("https://answers.yahoo.com/question/index?qid=20111016172242AAoVIGH"); //Downloads the data associated with this URI
                    HtmlDocument htmlDoc = new HtmlDocument(); //Initialize the htmlDoc
                    htmlDoc.LoadHtml(Encoding.UTF8.GetString(data)); //Loads the HTML document into the HtmlDocument class
                    textToAnalyze = htmlDoc.DocumentNode.InnerText; //Parsing the HTML document for text between nodes

                    path = (/*Directory.GetCurrentDirectory() + */@"Assets\HTML_Parsed.txt"); //Gets the path to the HTML_Parsed.txt file
                    FileStream f_HTML_Parsed = new FileStream(path, FileMode.Truncate, FileAccess.ReadWrite); //Initializes the FileStream to the file with truncate and with Read and write permisions
                    f_HTML_Parsed.Write(Encoding.UTF8.GetBytes(textToAnalyze), 0, Encoding.UTF8.GetBytes(textToAnalyze).Length); //Writing the htmlText string to the file
                    f_HTML_Parsed.Position = 0; //Setting the read/write position to 0 bytes offset

                    path = (/*Directory.GetCurrentDirectory() + */@"Assets\HTML.txt"); //Gets the path to the HTML.txt file
                    FileStream f_HTML = new FileStream(path, FileMode.Truncate, FileAccess.ReadWrite); //Initializes the FileStream to the file with truncate and with Read and write permisions
                    f_HTML.Position = 0; //Setting the read/write position to 0 bytes offset
                    f_HTML.Write(data, 0, data.Length);  //Writing the data string to the file

                    //Get string from f_HTML_Parsed.txt
                    byte[] bytesFromHTML = new byte[f_HTML_Parsed.Length]; //Sets the buffer size of the bytes variable to the size (in bytes) of the file
                    int bytesToReadFromFile = (int)f_HTML_Parsed.Length; //Sets the number of bytes to be read from the file
                    f_HTML_Parsed.Read(bytesFromHTML, 0, bytesToReadFromFile); //Reading bytes from f_HTML_Parsed
                    textToAnalyze = Encoding.UTF8.GetString(bytesFromHTML); //Converting byte[] to a string
                    break;
            }
            
            //----------------------------------------------------------------------------------------------------------------------------------------------------------//
            

            //----------------------------------------------------------------------------------------------------------------------------------------------------------//

            path = (/*Directory.GetCurrentDirectory() + */@"Assets\EveryWordInTheEnglishLanguage.txt"); //Gets the path to the HTML_Parsed.txt file
            FileStream f_EveryWordInEnglishLanguage = new FileStream(path, FileMode.Open, FileAccess.ReadWrite); //Initializes the FileStream to the file with truncate and with Read and write permisions
            f_EveryWordInEnglishLanguage.Position = 0; //Setting the read/write position to 0 bytes offset

            //Get string from EveryWordInTheEnglishLanguage.txt
            byte[] bytes = new byte[f_EveryWordInEnglishLanguage.Length]; //Sets the buffer size of the bytes variable to the size (in bytes) of the file
            int bytesToRead = (int)f_EveryWordInEnglishLanguage.Length; //Sets the number of bytes to be read from the file
            f_EveryWordInEnglishLanguage.Read(bytes, 0, bytesToRead); //Reading bytes from f_HTML_Parsed
            string EveryWordString = Encoding.UTF8.GetString(bytes); //Converting byte[] to a string


            //----------------------------------------------------------------------------------------------------------------------------------------------------------//
            bool exit = false; //Bool that tells the loop whether to continue running or exit the application
            while (!exit)
            {
                CHOICE userChoice;                                                  //---------------------------------//
                Console.Clear();                                                    //---------------------------------//
                Console.WriteLine("1. Scan for most common word");                  //---------------------------------//
                Console.WriteLine("2. Search for specific word");                   //Menu that prompts user for choice
                Console.WriteLine("3. Find already most common word");              //---------------------------------//
                Console.WriteLine("4. Exit");                                       //---------------------------------//
                char key = Console.ReadKey().KeyChar;                               //---------------------------------//
                if (key == '1')
                {
                    userChoice = CHOICE.SCAN_FOR_MOST_COMMON_WORD; //Choice to rescan the textToAnalyze for a new most common word
                }
                else if (key == '2')
                {
                    userChoice = CHOICE.SEARCH_FOR_SPECIFIC_WORD; //Choice to scan the textToAnalyze for the number of times a specific word appears
                }
                else if (key == '3')
                {
                    userChoice = CHOICE.MOST_COMMON_WORD_ALREADY_DETECTED; //Choice to access the MostCommonWord.txt file and get the previously detected most common word
                }
                else if (key == '4')
                {
                    userChoice = CHOICE.EXIT; //Choice to exit the application
                }
                else
                {
                    userChoice = CHOICE.NOTHING; //Defualt choice to do nothing
                }

                switch (userChoice)
                {
                    case CHOICE.SCAN_FOR_MOST_COMMON_WORD:
                        {
                            //----------------------------------------------------------------------------------------------------------------------------------------------------------//
                            Word[] words = ScanForMostCommonWord(f_MostCommonWord, EveryWordString, textToAnalyze);
                            path = (/*Directory.GetCurrentDirectory() + */@"Assets\MostCommonWord.txt"); //Gets the path to the HTML_Parsed.txt file
                            f_MostCommonWord = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
                            if(whatToAnalyze == ANALYZE.TWITTER && User.GetAuthenticatedUser() != null)
                            {
                                Tweet.PublishTweet("The word of the day is: " + words[0].m_Name + " -" + words[0].m_definition);
                            }
                        }
                        break;
                    case CHOICE.SEARCH_FOR_SPECIFIC_WORD:
                        {
                            
                            Console.Write(textToAnalyze);
                            Console.WriteLine();
                            Console.WriteLine("Enter word to search: ");
                            string wordToSearch = Console.ReadLine();
                            Console.WriteLine(wordToSearch + " appears " + StringAnalyzer.NumWordCount(textToAnalyze, wordToSearch).ToString() + " times");
                            //while (Console.ReadKey().Key != ConsoleKey.Enter) { }
                            if (Console.ReadKey().Key == ConsoleKey.P) { break; }
                            Console.Clear();
                            
                        }
                        break;
                    case CHOICE.MOST_COMMON_WORD_ALREADY_DETECTED:
                        {
                            
                            if (f_MostCommonWord.Length > 1)
                            {
                                bytesToRead = (int)f_MostCommonWord.Length;
                                bytes = new byte[f_MostCommonWord.Length];
                                f_MostCommonWord.Position = 0;
                                f_MostCommonWord.Read(bytes, 0, bytesToRead);
                                string wordOutput = Encoding.UTF8.GetString(bytes);
                                Console.WriteLine("");
                                Console.WriteLine("The most common word detected was: " + wordOutput);
                                Console.ReadKey();
                            }
                            else
                            {
                                Console.WriteLine("No word detected");
                                Console.ReadKey();
                                break;
                            }
                        }
                        break;
                    case CHOICE.EXIT:
                        exit = true;
                        break;
                    case CHOICE.NOTHING:
                        break;
                }
            }   
        }

        
    }
}
