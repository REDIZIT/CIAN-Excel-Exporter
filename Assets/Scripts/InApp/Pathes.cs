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

        /// <summary>txt file with logs</summary>
        public string LogsFile { get; }

        /// <summary>Application.dataPath</summary>
        public string DataPath { get; }

        public Pathes()
        {
            DataPath = Application.dataPath;

            Data = GetParent(DataPath) + "/data";
            ProgramFolder = GetParent(Data);

            SourceTable = Data + "/source_table.xlsx";
            LogsFile = Application.persistentDataPath;

            Directory.CreateDirectory(Data);
        }

        private string GetParent(string folder)
        {
            return new DirectoryInfo(folder).Parent.FullName;
        }
    }
}