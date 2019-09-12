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
        static void Main(string[] args)
        {
            string textToAnalyze = "";
            string path = (Directory.GetCurrentDirectory() + @"\Assets\MostCommonWord.txt"); //Gets the path to the HTML_Parsed.txt file
            FileStream f_MostCommonWord = new FileStream(path, FileMode.Open, FileAccess.Read);
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
                    // Set up your credentials (https://apps.twitter.com)
                    Auth.SetUserCredentials("rWTNstyPs5g6lxOczRAgeUo8K", "vcPD7MKooIecHmSvtwZgX3FosSFc0aHsoEIftTMX5WZEpt4AAu", "872103516558340097-jAseO4wGw4J5RXDbzNI2dRCCo4tIEla", "x9etB65WCJWrZHM1Ezb43tMadUvbKJ7glqxinMZpd5m58");

                    // Get my Home Timeline
                    IEnumerable<ITweet> tweets = Timeline.GetHomeTimeline();

                    //ITweet[] tweetArray = tweets.ToArray<ITweet>();

                    FileStream f_tweetOutput = new FileStream(Directory.GetCurrentDirectory() + @"\Assets\tweetOutput.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    //for (int i = 0; i < tweetArray.Length; i++)
                    //{
                    //    f_tweetOutput.Write(Encoding.UTF8.GetBytes(tweetArray[i].Text), 0, Encoding.UTF8.GetByteCount(tweetArray[i].Text));
                    //}
                    //var user = User.GetAuthenticatedUser();
                    //Console.WriteLine(user.ScreenName);
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

                    path = (Directory.GetCurrentDirectory() + @"\Assets\HTML_Parsed.txt"); //Gets the path to the HTML_Parsed.txt file
                    FileStream f_HTML_Parsed = new FileStream(path, FileMode.Truncate, FileAccess.ReadWrite); //Initializes the FileStream to the file with truncate and with Read and write permisions
                    f_HTML_Parsed.Write(Encoding.UTF8.GetBytes(textToAnalyze), 0, Encoding.UTF8.GetBytes(textToAnalyze).Length); //Writing the htmlText string to the file
                    f_HTML_Parsed.Position = 0; //Setting the read/write position to 0 bytes offset

                    path = (Directory.GetCurrentDirectory() + @"\Assets\HTML.txt"); //Gets the path to the HTML.txt file
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

            path = (Directory.GetCurrentDirectory() + @"\Assets\EveryWordInTheEnglishLanguage.txt"); //Gets the path to the HTML_Parsed.txt file
            FileStream f_EveryWordInEnglishLanguage = new FileStream(path, FileMode.Open, FileAccess.ReadWrite); //Initializes the FileStream to the file with truncate and with Read and write permisions
            f_EveryWordInEnglishLanguage.Position = 0; //Setting the read/write position to 0 bytes offset

            //Get string from EveryWordInTheEnglishLanguage.txt
            byte[] bytes = new byte[f_EveryWordInEnglishLanguage.Length]; //Sets the buffer size of the bytes variable to the size (in bytes) of the file
            int bytesToRead = (int)f_EveryWordInEnglishLanguage.Length; //Sets the number of bytes to be read from the file
            f_EveryWordInEnglishLanguage.Read(bytes, 0, bytesToRead); //Reading bytes from f_HTML_Parsed
            string EveryWordString = Encoding.UTF8.GetString(bytes); //Converting byte[] to a string
            Dictionary<string, Word> thatCollection = StringAnalyzer.CreateCollectionOfWords(EveryWordString);
            //string[] wordsToIgnore = { "the", "and", "of", "to", "is", "in", "class", "data", "span", "id", "or", "because", "why", "what", "how", "at", "so", "other", "many", "role", "answers", "on", "for", "like", "eat", "it", "s", "be", "this", "get", "out", "there", "from", "with", "amp", "that", "are" };
            //for (int i = 0; i < thatCollection.Count; i++)
            //{
            //    for (int k = 0; k < wordsToIgnore.Length; k++)
            //    {
            //        if (thatCollection.ElementAtOrDefault(i).Value.m_Name == wordsToIgnore[k])
            //        {
            //            Console.WriteLine("Word removed: " + thatCollection.ElementAtOrDefault(i).Value.m_Name);
            //            thatCollection.Remove(thatCollection.ElementAtOrDefault(i).Key);
            //        }
            //    }
            //}
            //f_EveryWordInEnglishLanguage.Close();
            //path = (Directory.GetCurrentDirectory() + @"\Assets\EveryWordInTheEnglishLanguage.txt"); //Gets the path to the HTML_Parsed.txt file
            //f_EveryWordInEnglishLanguage = new FileStream(path, FileMode.Truncate, FileAccess.ReadWrite);
            //f_EveryWordInEnglishLanguage.Position = 0;
            //int size = thatCollection.Count;
            //for (int i = 0; i < size; i++)
            //{
            //    Console.Clear();
            //    int pos = (int)f_EveryWordInEnglishLanguage.Position;
            //    int lengthOfBytesToRead = Encoding.UTF8.GetByteCount(thatCollection.ElementAtOrDefault(i).Value.m_Name);
            //    f_EveryWordInEnglishLanguage.Write(Encoding.UTF8.GetBytes(thatCollection.ElementAtOrDefault(i).Value.m_Name), 0, lengthOfBytesToRead);
            //    f_EveryWordInEnglishLanguage.Write(Encoding.UTF8.GetBytes("\n"), 0, 1);
            //    Console.SetCursorPosition(0, 0);
            //    Console.WriteLine(thatCollection.ElementAtOrDefault(i).Value.m_Name + "                                                  ");
            //}

            //----------------------------------------------------------------------------------------------------------------------------------------------------------//
            bool exit = false;
            while (!exit)
            {
                CHOICE userChoice;
                Console.Clear();
                Console.WriteLine("1. Scan for most common word");
                Console.WriteLine("2. Search for specific word");
                Console.WriteLine("3. Find already most common word");
                Console.WriteLine("4. Exit");
                char key = Console.ReadKey().KeyChar;
                if(key == '1')
                {
                    userChoice = CHOICE.SCAN_FOR_MOST_COMMON_WORD;
                }
                else if (key == '2')
                {
                    userChoice = CHOICE.SEARCH_FOR_SPECIFIC_WORD;
                }
                else if (key == '3')
                {
                    userChoice = CHOICE.MOST_COMMON_WORD_ALREADY_DETECTED;
                }
                else if (key == '4')
                {
                    userChoice = CHOICE.EXIT;
                }
                else
                {
                    userChoice = CHOICE.NOTHING;
                }

                switch (userChoice)
                {
                    case CHOICE.SCAN_FOR_MOST_COMMON_WORD:
                        {
                            //HTMLText = "i i i i i love love interesting interesting interesting butt a a a a a a a a a";
                            Dictionary<string, Word> constCollection = StringAnalyzer.CreateCollectionOfWords(EveryWordString);
                            Dictionary<string, Word> tempCollection = constCollection;
                            
                            int count = 0;
                            int countTotal = 0;
                            float timePassed = 0;
                            Stopwatch sw = new Stopwatch();
                            //textToAnalyze = "In society, there are certain actions and behaviors that are considered “kind” or reflective of an internal goodness. Kindness, however, comes from something not considered “kind” at all. Based on William Golding’s book Lord of the Flies, a study by the Baby Lab, The Stanford Prison Experiment, and The Milgram Experiment, humans strive to gain things that will give them advantage over other humans. People eventually realize that forming connections with others provide essential things that gives them an enormous advantage over others. Humans then use “kind” behavior to gain human connections that give them advantage. Kindness stems from humanity’s inherent selfishness because of a desire to form connections with other people in order to gain advantage.Humans seek things they believe will give them advantage over other humans in order to achieve a better chance of survival. This can go so far as a person stealing from another person and hurting them to give themselves a better chance of survival. This selfish tendency is exemplified throughout human history and literature. The book Lord of the Flies by William Golding follows the progress of young boys after they are stranded together on a deserted island. As groups begin to form on the island and they start to compete for resources, a group led by a boy named Jack raids another, smaller, one. After the raid, Jack walks away down the beach and, “From his left hand dangled Piggy’s glasses” (Golding, Pg. 168). On this island fire is necessary to survival because it provides warmth, heat to cook with, and smoke to attract rescue. These boys used Piggy’s glasses to make fire, and by taking them away, Jack has effectively increased his chances of survival while hurting the group he raided. By seeking out and taking the glasses Jack’s group gained an advantage over the other because they could now make fire and become stronger. A similar situation provided like results in a study by The Baby Lab. In this experiment, children between the ages of 6 and 8 were presented with 2 choices: getting a same number of chips as another child or receiving less chips, but more than the other child. The children routinely took the least available number of chips that were still greater than the number of chips other children took. By taking more chips than the other child they are displaying the want to have “more” than the other kids. Wanting “more” shows their need to have an advantage over the other children, and by having more chips they can potentially get something that will give them the upper hand. Similarly, the Stanford Prison Experiment, conducted by Phillip Zimbardo in 1973, also resulted in actions by the participants that reflected attempts to gain advantage. In the experiment, participants were divided into either a guard or a prisoner. They were then put in a simulated prison environment where the guards had absolute power over the prisoners. Eventually, “The prisoners were dependent on the guards for everything, so they tried to find ways to please the guards, such as telling on fellow prisoners” (McLeod. Pg. 3). In this situation, the prisoners seek advantage by telling on their fellow inmates. In these circumstances, having the approval of the guards is necessary to get what the prisoners need to survive, and by having this a prisoner would get advantage over another prisoner. Telling on a fellow prisoner not only gains advantage by getting on the guards’    good side, but it also takes the approval away from another prisoner. Therefore, the informant, the prisoner, who told,   gains an advantage by comparison. In each of these cases people are seeking out things that will give them advantage over others, and even steal from others to make themselves more powerful in comparison.There are many things in this world that people consider valuable and important that give them some sort of advantage. One of the most valuable things a person can acquire however is not a weapon, nor is it wealth; it is connection with other human beings. Connection can provide safety from a figure of authority, advice from someone wise, or other valuable materials from someone with access to them. The Stanford Prison Experiment is an example of safety from a figure of authority. Participants were divided into groups of prisoners and guards. As the experiment progressed the guards gained more power. Eventually, “the prisoners were depended on the guards for everything” (McLeod, Pg. 3). Because the prisoners were dependent on the guards for everything there was no way to get what they needed to survive unless a guard gave it to them. In order to get what they needed they would have to convince the guards to give it to them, and the only way to do that is to form a connection with the guards and make them want to help. In this case, the relationship with the guards is more important than any material object because the only way to get anything is to persuade the guards to give it up. This reflects how connections with others are important because someone else may have something that is needed to survive by another, and without forming a connection with that person to get it, the individual who needs it may die. In the book, Lord of the Flies, by William Golding, the boys on the island form a sort of tribe with a chief named Ralph. As Ralph is walking around and confronting his troubles with leading his people his thoughts wander to his friend Piggy, “Piggy could think. He could go step by step inside that fat head of his, only Piggy was no chief. But Piggy, for all his ludicrous body, had brains. Ralph was a specialist in thought now, and could recognize thought in another” (Golding, Pg. 78). As the chief of the tribe, Ralph has lots of responsibilities to take care of and many decisions to make. The decisions he makes are extremely important, and will affect whether he, and the other kids, will survive. Because Ralph recognizes Piggy as thinking like him and having the ability to think through things logically, he is now able to call upon Piggy’s mind to help him make important choices. There is nothing  on the island that can help Ralph in making decisions better than another person who is familiar with the situation at hand. By having Piggy as an advisor, Ralph can now consult him on critical choices and spend more time managing his tribe. The advantage gained   from human connection is exemplified by negotiating with others to get valuable materials. Ralph was able to gain materials he and his tribe  needed by bargaining with Jack Merridew and his hunters.     Jack has become stranded with Ralph and Piggy, but has created a separate group of hunters that acts almost independently of Ralph and his tribe . These hunters go hunting for pig, and after coming back from one of these hunts he is confronted by Ralph about choosing to hunt over monitoring the fire. Jack responds simply with, “We needed meat” (Golding Pg. 71). This statement by Jack is reflective of the needs of the kids on the island and how the hunting group is fulfilling those needs. In order to survive on the island, the group needs meat. The hunters go out and get the meat that the group needs to survive. Therefore, it is necessary to survival for all the other children on the island that a connection between themselves and the hunters is maintained. They are unable to hunt themselves, and without the meat they will die. Making the connection with the hunters the most important thing on the island. Whether the advantage is  extra protection from others, providing the necessary items to live, or supporting and advising a person on difficult decisions, the connections between people provide many necessary things; therefore, making human connections extremely important and advantageous. Humans seek advantage, and connections with other people help them achieve it. As they get older and gain more experience, they seek to make connections with other people through kind behavior . Humanity’s need to gain advantage leads them to pursue the advantages of human connection by being kind to others in order to form relationships with them. In a study by The Baby Lab reported on by CBS News, children ages 8 to 13 were presented with a table of chips that could be turned in for prizes. This was the same situation the younger children were put into when  they had to choose between varying numbers of chips for themselves and another child. As the children aged they made a conscious choice to be kind, and give the same amount, or more, chips to another child. This doesn’t quite make sense at first if humans are always looking for advantage, but they are still trying to gain advantage by giving away more chips. By being “kind” to the other child, the kids in the study are forming connections with them, and those connections are more valuable than the prizes the chips could be exchanged for. Human connections are especially sought in situations  where there is a distinct struggle for survival and advantage is necessary. In the Lord of the Flies the boys on the island must find every available resource to survive, and human connections are the most important one they can possess. After Jack splits the main group and takes some of the boys to form his own tribe of savages, Ralph and Piggy go to his bonfire and try to negotiate an arrangement where they can coexist. When they arrive, Jack tells his savages to, “Take them some meat” (Golding, Pg. 149). Because Ralph and Piggy are hungry at this point, Jack has made them indebted to him and thankful for the food he has offered. By making them indebted to him, Jack has formed a connection that he can take advantage of later; and giving food to Ralph and Piggy is reflective of Jack’s need to seek the advantage that comes from human connection. In both of these cases people are making conscious choices to be kind to others, and by being kind they are connecting with others because they will want to be treated the same again. Therefore, by being kind people are satisfying their need to gain advantage because that behavior gains human connection and those connections are extremely advantageous.Humans display kind behavior in order to satisfy their need to seek advantage over others. This behavior is used by humans to coerce other people into forming connections with them. These connections give people an advantage over others because of the important assets provided by them, and are therefore sought after as a result of humanities selfishness. Knowing that kind behavior originates from an inherent selfishness, humanity can begin to understand why there is selflessness in the world, and why there are those in society who refuse to be kind and become outcasts.";
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
                            //----------------------------------------------------------------------------------------------------------------------------------------------------------//
                            path = (Directory.GetCurrentDirectory() + @"\Assets\MostCommonWord.txt"); //Gets the path to the HTML_Parsed.txt file
                            f_MostCommonWord.Close();
                            f_MostCommonWord = new FileStream(path, FileMode.Truncate, FileAccess.ReadWrite); //Initializes the FileStream to the file with truncate and with Read and write permisions
                            Word[] MostCommonWordsArray = CollectionAnalyzer.MostCommonWords(tempCollection, SORT_DIRECTION.HIGHEST_TO_LOWEST);
                            f_MostCommonWord.Position = 0; //Setting the read/write position to 0 bytes offset
                            bytes = Encoding.UTF8.GetBytes(MostCommonWordsArray[0].m_Name);
                            bytesToRead = Encoding.UTF8.GetBytes(MostCommonWordsArray[0].m_Name).Length;
                            f_MostCommonWord.Write(bytes, 0, bytesToRead); //Reading bytes from f_HTML_Parsed
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
