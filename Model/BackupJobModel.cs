﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace PROGRAMMATION_SYST_ME.Model
{
    /// <summary>
    /// Model class for a list of 5 backup jobs
    /// </summary>
    class BackupJobModel
    {
        public XmlDocument Xml { set; get; } = new XmlDocument();
        private readonly string xmlPath;
        public BackupJobModel(List<BackupJobDataModel> jobList)
        {
            xmlPath = Path.Combine(Environment.CurrentDirectory, @"SaveJobsConfig.xml");
            // if the selected path is found, proceed. otherwise raise an error.
            try
            {  
                Xml.Load(xmlPath);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error : {e}");
                Environment.Exit(3);
            }
            foreach (XmlNode node in Xml.DocumentElement.SelectNodes("saveJob"))
            {
                BackupJobDataModel data = new BackupJobDataModel();
                data.Id = int.Parse(node.ChildNodes[0].InnerText);
                data.Name = node.ChildNodes[1].InnerText;
                data.Source = node.ChildNodes[2].InnerText;
                data.Destination = node.ChildNodes[3].InnerText;
                data.Type = int.Parse(node.ChildNodes[4].InnerText);
                jobList.Add(data);
            }
        }
        
        /// <summary>
        /// Method that allows for edits in the backup jobs
        /// </summary>
        public void SaveParam(List<BackupJobDataModel> jobList)
        {
            var i = 0;
            foreach (XmlNode node in Xml.DocumentElement.SelectNodes("saveJob"))
            {
                node.ChildNodes[0].InnerText = jobList[i].Id.ToString();
                node.ChildNodes[1].InnerText = jobList[i].Name;
                node.ChildNodes[2].InnerText = jobList[i].Source;
                node.ChildNodes[3].InnerText = jobList[i].Destination;
                node.ChildNodes[4].InnerText = jobList[i].Type.ToString();
                i++;
            }
            Xml.Save(xmlPath);
        }
    }
    class BackupJobDataModel
    {
        public int Id {  get; set; }
        public string Name { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public int Type { get; set; }// O is full backup and 1 is differential backup
    }
}
