using System.IO;
using UnityEngine;

namespace InApp
{
    public class Pathes
    {
        public string SourceTable { get; }

        /// <summary>/data</summary>
        public string Data { get; }

        /// <summary>/</summary>
        public string ProgramFolder { get; }

        public Pathes()
        {
            Data = GetParent(Application.dataPath) + "/data";
            ProgramFolder = GetParent(Data);
            SourceTable = Data + "/source_table.xlsx";

            Directory.CreateDirectory(Data);
        }

        private string GetParent(string folder)
        {
            return new DirectoryInfo(folder).Parent.FullName;
        }
    }
}