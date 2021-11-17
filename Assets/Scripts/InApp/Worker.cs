using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace InApp
{
    public class Worker
    {
        public int UrlsCount => urls.Count;


        private List<string> urls = new List<string>();
        private readonly Pathes pathes;
        private readonly UrlsCreator urlsCreator;

        public Worker(Pathes pathes)
        {
            this.pathes = pathes;
            urlsCreator = new UrlsCreator(pathes);
        }

        public void Start(string folderPath)
        {
            if (Directory.Exists(folderPath) == false)
                throw new System.Exception($"Can't start working because there is no folder at: '{folderPath}'");

            urls = urlsCreator.Create();
        }

        
    }

    public class Rule
    {
        /// <summary>List of ranges (areas) by rooms count</summary>
        public Dictionary<int, List<Range>> ranges = new Dictionary<int, List<Range>>();

        public void AddRange(int room, Range range)
        {
            if (ranges.ContainsKey(room) == false)
            {
                ranges.Add(room, new List<Range>());
            }
            ranges[room].Add(range);
        }
    }

    public class Range
    {
        public int? min;
        public int? max;

        public Range(string condition)
        {
            if (condition.Contains("<"))
            {
                max = int.Parse(condition.Replace("<", ""));
            }
            else if (condition.Contains(">"))
            {
                min = int.Parse(condition.Replace(">", ""));
            }
            else if (condition.Contains("-"))
            {
                string[] splitted = condition.Split('-');
                min = int.Parse(splitted[0]);
                max = int.Parse(splitted[1]);
            }
            else
            {
                throw new System.Exception($"Can't create range from string condition '{condition}'");
            }
        }

        public string GetUrlArguments()
        {
            string arg = "";

            if (min != null) arg += "&mintarea=" + min.Value;
            if (max != null) arg += "&maxtarea=" + max.Value;

            return arg;
        }
    }
}