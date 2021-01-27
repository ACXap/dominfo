using System;
using System.Collections.Generic;
using System.IO;


namespace dominfo
{
    public class UserAgentRepository
    {
        private Random _rnd = new Random();

        private string _fileData = "userAgent.dat";

        private List<string> _collectionUserAgent = new List<string>();

        public UserAgentRepository()
        {
            if (File.Exists(_fileData))
            {
                using (StreamReader sr = new StreamReader(File.OpenRead(_fileData)))
                {
                    while (!sr.EndOfStream)
                    {
                        _collectionUserAgent.Add(sr.ReadLine());
                    }
                }
            }
        }
        public string GetRandomUserAgent()
        {
            return _collectionUserAgent[_rnd.Next(0, _collectionUserAgent.Count)];
        }
    }
}