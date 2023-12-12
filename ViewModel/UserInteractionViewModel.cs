﻿using System.Collections.Generic;
using PROGRAMMATION_SYST_ME.Model;
using System.IO;
using System.Text.RegularExpressions;
using PROGRAMMATION_SYST_ME;
using System.Diagnostics;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace PROGRAMMATION_SYST_ME.ViewModel
{
    public class UserInteractionViewModel
    {
        public List<BackupJobDataModel> BackupJobsData { set; get; } = new List<BackupJobDataModel>();
        public BackupJobModel BackupJobs { set; get; }
        public bool? IsCrypt { set; get; }
        public List<Thread> Threads { set; get; }
        public List<RealTimeDataModel> RealTimeData { set; get; } = new List<RealTimeDataModel>();
        public RealTimeModel RealTime { set; get; } = new RealTimeModel();
        public LogModel LogFile { set; get; } = new LogModel();
        private long totalSaveSize = 0;
        private long totalNbFile = 0;
        private List<long> NbFilesCopied = new();
        private int indRTime = 0;
        private delegate void CopyType(FileInfo file, string destination);
        CopyType delegCopy;
        private string businessSoft = "CalculatorApp";
        private Mutex mut = new();
        public UserInteractionViewModel()
        {
            BackupJobs = new BackupJobModel(BackupJobsData);
            delegCopy = CopyFile;
            Threads = new List<Thread>();
        }
        public bool ChangeExtensionLog(string extLog)
        {
            BackupJobs.ChangeExtensionLog(extLog);
            LogFile.CreateLogFile();
            RealTime.CreateRealTimeFile();
            return true;
        }
        public int CreateJob(string name, string source, string Destination, int type)
        {
            BackupJobDataModel newJob = new BackupJobDataModel
            {
                Id = BackupJobsData.Count,
                Name = name,
                Source = source,
                Destination = Destination,
                Type = type
            };
            BackupJobsData.Add(newJob);
            BackupJobs.CreateJob(BackupJobsData);
            return newJob.Id;
        }
        public ErrorCode DeleteJobVM(int job)
        {
            BackupJobsData.RemoveAt(job);
            BackupJobs.DestroyNode(job);
            BackupJobs.SaveParam(BackupJobsData);
            BackupJobs.UpdateList(BackupJobsData);
            return ErrorCode.SUCCESS;
        }
        public ErrorCode UpdateJob(int jobChoice, string name, string source, string dest, int type)
        {
            BackupJobsData[jobChoice].Name = name;
            BackupJobsData[jobChoice].Source = source;
            BackupJobsData[jobChoice].Destination = dest;
            BackupJobsData[jobChoice].Type = type;
            BackupJobs.SaveParam(BackupJobsData);
            return ErrorCode.SUCCESS;
        }
        /// <summary>
        /// Method to execute backup jobs
        /// </summary>
        /// <param name="selection">input user</param>
        /// <returns>error code BUSINESS_SOFT_LAUNCHED or INPUT_USER or SOURCE_ERROR or SUCCESS</returns>
        public ErrorCode ExecuteJob(List<int> jobsToExec)
        {
            ErrorCode error = ErrorCode.SUCCESS;
            Process[] processes = Process.GetProcessesByName(businessSoft);
            if (processes.Length != 0)
            {
                error = ErrorCode.BUSINESS_SOFT_LAUNCHED;
                return error;
            }

            SetupRealTime(jobsToExec);
            indRTime = 0;
            foreach (int i in jobsToExec)
            {
                NbFilesCopied.Add(0);
                if (error == ErrorCode.SUCCESS)
                {
                    mut.WaitOne();
                    RealTimeData[indRTime].State = "ACTIVE";
                    RealTime.WriteRealTimeFile(RealTimeData);
                    mut.ReleaseMutex();
                    //Watch is wrong cause of multithreading
                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    totalSaveSize = 0;
                    if (BackupJobsData[i].Type == 0) // Full backup
                    {
                        delegCopy = CopyFile;
                    }
                    else // Differencial backup
                    {
                        delegCopy = CopyFileDiff;
                    }
                    if (Directory.Exists(BackupJobsData[i].Source))
                    {
                        Thread task = new(() => SaveDir(BackupJobsData[i].Source, BackupJobsData[i].Destination, delegCopy));
                        task.Name = i.ToString();
                        task.Start();
                        Threads.Add(task);
                    }
                    else if (File.Exists(BackupJobsData[i].Source))
                    {
                        Thread task = new(() => delegCopy(new FileInfo(BackupJobsData[i].Source), BackupJobsData[i].Destination));
                        task.Name = i.ToString();
                        task.Start();
                        Threads.Add(task);
                    }
                    else
                        error = ErrorCode.SOURCE_ERROR;
                    watch.Stop();

                    LogFile.WriteLogSave(
                        BackupJobsData[i],
                        watch.ElapsedMilliseconds,
                        totalSaveSize
                    );
                }
                else
                    break;
                indRTime++;
            }
            foreach (Thread t in Threads)
            {
                int j = int.Parse(t.Name);
                t.Join();
                mut.WaitOne();
                if (error == ErrorCode.SUCCESS)
                {
                    RealTimeData[j].State = "SUCCESSFUL";
                }
                else
                    RealTimeData[j].State = "ERROR";
                RealTime.WriteRealTimeFile(RealTimeData);
                mut.ReleaseMutex();
                j++;
            }

            return error;
        }
        /// <summary>
        /// Get the file size and length
        /// </summary>
        /// <param name="file">a file</param>
        /// <param name="destination">null by default (only here for delegate)</param>
        private void GetDirectoryInfo(FileInfo file, string destination = null)
        {
            totalSaveSize += file.Length;
            totalNbFile++;
        }
        /// <summary>
        /// Go through a directory recursively and use a delegate 
        /// to choose what to do on each file
        /// </summary>
        /// <param name="source">Source directory</param>
        /// <param name="destination">Destination directory</param>
        /// <returns>error code SUCCESS or SOURCE_ERROR</returns>
        private ErrorCode SaveDir(string source, string destination, CopyType deleg)
        {
            var dir = new DirectoryInfo(source);
            if (!dir.Exists)
                return ErrorCode.SOURCE_ERROR;
            DirectoryInfo[] dirs = dir.GetDirectories();
            var dirDest = new DirectoryInfo(destination);
            if (deleg == CopyFile)
                if (dirDest.Exists)
                    Directory.Delete(destination, true);
            Directory.CreateDirectory(destination);
            foreach (FileInfo file in dir.GetFiles())
            {
                deleg(file, destination);
            }
            foreach (DirectoryInfo subDir in dirs)
            {
                SaveDir(subDir.FullName, Path.Combine(destination, subDir.Name), deleg);
            }
            return ErrorCode.SUCCESS;
        }
        /// <summary>
        /// Copy a file while updating total copy info
        /// </summary>
        /// <param name="file">a file</param>
        /// <param name="destination">destination directory</param>
        private void CopyFile(FileInfo file, string destination)
        {
            Process[] processes = Process.GetProcessesByName(businessSoft);
            while (processes.Length != 0)
            {
                processes = Process.GetProcessesByName(businessSoft);
                Thread.Sleep(50);
            }
            if (IsCrypt == true)
            {
                Process process = new Process();
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.FileName = "Cryptosoft.exe";
                process.StartInfo.Arguments = file.FullName + " " + Path.Combine(destination, file.Name);
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardError = true;
                process.Start();
            }
            else
            {
                file.CopyTo(Path.Combine(destination, file.Name), true);
            }
            mut.WaitOne();

            var ind = int.Parse(Thread.CurrentThread.Name);
            NbFilesCopied[ind]++;
            RealTimeData[ind].NbFilesLeftToDo = RealTimeData[ind].TotalFilesToCopy - NbFilesCopied[ind];
            RealTimeData[ind].Progression = NbFilesCopied[ind] / RealTimeData[ind].TotalFilesToCopy;
            RealTime.WriteRealTimeFile(RealTimeData);

            mut.ReleaseMutex();
        }
        /// <summary>
        /// Copy a file if a change occured while updating total copy info
        /// </summary>
        /// <param name="file">a file</param>
        /// <param name="destination">destination directory</param>
        private void CopyFileDiff(FileInfo file, string destination)
        {
            var destPath = Path.Combine(destination, file.Name);
            var destFile = new FileInfo(destPath);
            if (file.LastWriteTime != destFile.LastWriteTime) // Condition to see if file changed
            {
                CopyFile(file, destination);
            }
        }
        /// <summary>
        /// Setup the real time log file
        /// </summary>
        /// <param name="jobsToExec">List of index that represent the backup jobs</param>
        private void SetupRealTime(List<int> jobsToExec)
        {
            RealTimeData.Clear();
            indRTime = 0;
            foreach (int i in jobsToExec)
            {
                RealTimeData.Add(new RealTimeDataModel());
                RealTimeData[indRTime].SaveData = BackupJobsData[i];
                RealTimeData[indRTime].State = "WAITING";
                if (Directory.Exists(BackupJobsData[i].Source))
                {
                    totalNbFile = 0;
                    totalSaveSize = 0;
                    delegCopy = GetDirectoryInfo;
                    SaveDir(BackupJobsData[i].Source, BackupJobsData[i].Destination, delegCopy);
                    RealTimeData[indRTime].TotalFilesSize = totalSaveSize;
                    RealTimeData[indRTime].TotalFilesToCopy = totalNbFile;
                }
                else if (File.Exists(BackupJobsData[i].Source))
                {
                    RealTimeData[indRTime].TotalFilesSize = new FileInfo(BackupJobsData[i].Source).Length;
                    RealTimeData[indRTime].TotalFilesToCopy = 1;
                }
                indRTime++;
            }
            RealTime.WriteRealTimeFile(RealTimeData);
        }
    }
}
