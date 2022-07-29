using System;
using System.IO;

namespace ChangingFindsByLocators
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] lines = System.IO.File.ReadAllLines(@"e:\ClassesToUpdate.txt");

            int findsByCounter = 0;

            for (int i = 0; i < lines.Length; i++)
            {
                findsByCounter = findsByCounter + ReadFile(lines[i]);
            }

            Console.WriteLine($"Total number of edited lines: {findsByCounter}");
        }

        public static int ReadFile(string filePath)
        {
            int findsByCounter = 0;
            bool fixNextLine = false;

            string findsByHow;
            string findsByLocator;
            string newPropertyName = "";
            string[] lines = System.IO.File.ReadAllLines(filePath);

            System.IO.File.Delete(filePath);

            using (StreamWriter sw = File.CreateText(filePath))
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains("[FindsBy"))
                    {
                        fixNextLine = true;
                        findsByCounter++;
                        findsByHow = ReturnHowType(lines[i]);
                        findsByLocator = ReturnLocator(lines[i]);
                        newPropertyName = ReturnNewProperty(findsByHow, findsByLocator, lines[i + 1]);
                    }
                    else
                    {
                        if (fixNextLine == true)
                        {
                            sw.WriteLine(newPropertyName);
                            System.Console.WriteLine(newPropertyName);
                            fixNextLine = false;
                        }
                        else
                        {
                            if(lines[i] != "using OpenQA.Selenium.Support.PageObjects;")
                            {
                                sw.WriteLine(lines[i]);
                                System.Console.WriteLine(lines[i]);
                            }
                        }
                    }
                }
            }
            
            Console.WriteLine($"Number of FindsBy: {findsByCounter}");

            return findsByCounter;
        }

        public static string ReturnHowType(string line)
        {
            //Divide by '.'
            string[] partOne = line.ToString().Split('.');

            //Divide by ','
            string[] partTwo = partOne[1].ToString().Split(',');

            return partTwo[0];
        }

        public static string ReturnLocator(string line)
        {
            int linelenght = line.Length;
            string locator = line.Substring(0, linelenght - 2);

            string[] partOne = locator.Split(new[] { "Using =" }, StringSplitOptions.None);
            locator = partOne[1];

            if (locator.Substring(0,1) == " ")
            {
                locator = locator.Substring(1, locator.Length - 1);
            }
            
            return locator;
        }

        public static string ReturnNewProperty(string howType, string locator, string oldProperty)
        {
            oldProperty = oldProperty.Substring(0, oldProperty.Length - 1);

            return $"{oldProperty} => webDriver.FindElement(By.{howType}({locator}));";
        }
    }
}
