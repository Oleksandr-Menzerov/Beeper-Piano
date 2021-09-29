using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

namespace BeeperPiano
{
    static class BeeperPiano
    {
        public const double etalonFrequency = 523.25;
        public const int milliseconds = 60000;
        public static int Tempo { get; set; }
        public static int Pitch { get; set; }
        public static int OctavaPitch { get; set; }
        public static int Duration { get; set; }
        public static bool IsStacatto { get; set; }
        public static bool IsPause { get; set; }
        public static bool IsRecording { get; set; }
        public static int OctavaStops { get; set; }
        public static int OctavaShift { get; set; }

        public static readonly List<string> newSong = new();
        public static bool IsQuerty { get; set; }
        public static int PrevFreq { get; set; }
        static readonly Stopwatch stopWatch = new();
        public static readonly string startMelody = new("C5 1/8., B4 1/8., A4 1/8., G4 1/8., F4 1/8., E4 1/8., D4 1/8., C4 1/4, pause 1/4, C5 1/2");

        public static void ErrorMessage(string error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(error);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Beep();
        }

        public static void InfoMessage(string msg)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.Green;
        }

        public static void InfoString(string msg)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(msg);
            Console.ForegroundColor = ConsoleColor.Green;
        }

        public static void SetStopsInOctava()
        {
            Console.WriteLine("Type number of stops in octava.\n" +
                "   (12 for standart halftones, 24 for quadtones, 18 for thirdtones etc.)\n" +
                "   or q to quite into Piano.\n" +
                "   Current count of stops in octava is " + OctavaStops + " .");
            string answer = Console.ReadLine();
            if (answer.ToLower() == "q") { if (IsQuerty) QWMenu(); else BWMenu(); }
            else {
                bool success = int.TryParse(answer, out int number);
                if (success && number > 0 && number < 150)
                {
                    OctavaStops = number;
                    if (IsQuerty&&OctavaStops > 24)
                    { OctavaShift =  24; }
                    else if (!IsQuerty&& OctavaStops> 20)
                    { OctavaShift =  20; }
                    else { OctavaShift = OctavaStops; }

                    InfoMessage("New octava stops count is " + OctavaStops + " .");
                    if (IsQuerty) QWMenu(); else BWMenu();
                }
                else
                {
                    ErrorMessage("Invalid data!");
                }
            }
        }
        public static int GetFrequency(string toneName)
        {
            double toneIndex = 0;
            if (toneName.First() == 'C')
            { toneIndex = 0; }
            else if (toneName.First() == 'D')
            { toneIndex += 2; }
            else if (toneName.First() == 'E')
            { toneIndex += 4; }
            else if (toneName.First() == 'F')
            { toneIndex += 5; }
            else if (toneName.First() == 'G')
            { toneIndex += 7; }
            else if (toneName.First() == 'A')
            { toneIndex += 9; }
            else if (toneName.First() == 'B')
            { toneIndex += 11; }

            if (toneName.Substring(1, 1) == "#")
            { toneIndex++; }
            else if (toneName.Substring(1, 1) == "b")
            { toneIndex--; }

            toneIndex = toneIndex + (int.Parse(toneName.Last().ToString()) -5 ) * OctavaShift + Pitch;
            double power = toneIndex / OctavaStops;
            double dobleFreg = etalonFrequency * Math.Pow(2, power);
            return (int)Math.Round(dobleFreg);
        }

        public static int GetFrequency(ConsoleKey key)
        {

            double toneIndex;
            if (IsQuerty)
            {
                if (key == ConsoleKey.Q)
                { toneIndex = -1; }
                else if (key == ConsoleKey.A)
                { toneIndex = 0; }
                else if (key == ConsoleKey.W)
                { toneIndex = 1; }
                else if (key == ConsoleKey.S)
                { toneIndex = 2; }
                else if (key == ConsoleKey.E)
                { toneIndex = 3; }
                else if (key == ConsoleKey.D)
                { toneIndex = 4; }
                else if (key == ConsoleKey.R)
                { toneIndex = 5; }
                else if (key == ConsoleKey.F)
                { toneIndex = 6; }
                else if (key == ConsoleKey.T)
                { toneIndex = 7; }
                else if (key == ConsoleKey.G)
                { toneIndex = 8; }
                else if (key == ConsoleKey.Y)
                { toneIndex = 9; }
                else if (key == ConsoleKey.H)
                { toneIndex = 10; }
                else if (key == ConsoleKey.U)
                { toneIndex = 11; }
                else if (key == ConsoleKey.J)
                { toneIndex = 12; }
                else if (key == ConsoleKey.I)
                { toneIndex = 13; }
                else if (key == ConsoleKey.K)
                { toneIndex = 14; }
                else if (key == ConsoleKey.O)
                { toneIndex = 15; }
                else if (key == ConsoleKey.L)
                { toneIndex = 16; }
                else if (key == ConsoleKey.P)
                { toneIndex = 17; }
                else if (key == ConsoleKey.Oem1)
                { toneIndex = 18; }
                else if (key == ConsoleKey.Oem4)
                { toneIndex = 19; }
                else if (key == ConsoleKey.Oem7)
                { toneIndex = 20; }
                else if (key == ConsoleKey.Oem6)
                { toneIndex = 21; }
                else if (key == ConsoleKey.Oem5)
                { toneIndex = 22; }
                else { return 0; }
            }

            else
            {
                if (key == ConsoleKey.Q)
                { toneIndex = -1; }
                else if (key == ConsoleKey.A)
                { toneIndex = 0; }
                else if (key == ConsoleKey.W)
                { toneIndex = 1; }
                else if (key == ConsoleKey.S)
                { toneIndex = 2; }
                else if (key == ConsoleKey.E)
                { toneIndex = 3; }
                else if (key == ConsoleKey.D)
                { toneIndex = 4; }
                else if (key == ConsoleKey.F)
                { toneIndex = 5; }
                else if (key == ConsoleKey.T)
                { toneIndex = 6; }
                else if (key == ConsoleKey.G)
                { toneIndex = 7; }
                else if (key == ConsoleKey.Y)
                { toneIndex = 8; }
                else if (key == ConsoleKey.H)
                { toneIndex = 9; }
                else if (key == ConsoleKey.U)
                { toneIndex = 10; }
                else if (key == ConsoleKey.J)
                { toneIndex = 11; }
                else if (key == ConsoleKey.K)
                { toneIndex = 12; }
                else if (key == ConsoleKey.O)
                { toneIndex = 13; }
                else if (key == ConsoleKey.L)
                { toneIndex = 14; }
                else if (key == ConsoleKey.P)
                { toneIndex = 15; }
                else if (key == ConsoleKey.Oem1)
                { toneIndex = 16; }
                else if (key == ConsoleKey.Oem7)
                { toneIndex = 17; }
                else if (key == ConsoleKey.Oem6)
                { toneIndex = 18; }
                else if (key == ConsoleKey.Oem5)
                { toneIndex = 19; }
                else { return 0; }
            }

            toneIndex = toneIndex + Pitch + OctavaPitch - OctavaShift;
            double power = toneIndex / OctavaStops;
            double dobleFreg = etalonFrequency * Math.Pow(2, power);
            return (int)Math.Round(dobleFreg);
        }
        public static int GetFrequency(int percNum)
        {
            double dobleFreg = 100 * Math.Pow(2, percNum*0.63);
            return (int)Math.Round(dobleFreg);
        }

        public static int GetDuration(string durationName)
        {
            int duration;
            if (durationName.Length == 1)
            { duration = milliseconds * int.Parse(durationName) / Tempo; }
            else if (durationName.Length == 2)
            { duration = milliseconds * int.Parse(durationName.First().ToString()) * 15 / (10 * Tempo); }
            else
            {
                string[] durationUnit = durationName.Split('/');
                if (durationUnit[1].Last() == '.')
                {
                    durationUnit[1] = durationUnit[1].Remove(durationUnit[1].Length - 1);
                    duration = milliseconds * int.Parse(durationUnit[0]) * 15 / (int.Parse(durationUnit[1]) * 10 * Tempo);
                }
                else
                { duration = milliseconds * int.Parse(durationUnit[0]) / (int.Parse(durationUnit[1]) * Tempo); }
            }
            return duration;
        }

        public static void PlayBeeps(string song)
        {
            if (OctavaStops != 12)
            { InfoMessage("Song in American notation system. Stops in octava will be set to 12."); 
              OctavaStops = 12; OctavaShift = 12; }

            string[] notes = song.Split(", ");
            for (int i = 0; i < notes.Length; i++)
            {
                string[] toneAndDuration = notes[i].Split(' ');
                string toneName = toneAndDuration[0];
                string durationName = toneAndDuration[1];
                int duration = GetDuration(durationName);
                if (toneName == "pause")
                { Thread.Sleep(duration); }
                else
                {
                    int freq = GetFrequency(toneName);
                    if (freq < 37) { freq = 37; }
                    if (freq > 32767) { freq = 32767; }
                    Console.Beep(freq, duration);
                }
            }
        }

        public static void PlayBeeps(List<string> song)
        {
            foreach (string note in song)
            {
                if (note.First() == 'C')
                {
                    string[] halfs = note.Split(", ");
                    string[] freqString = halfs[0].Split('(');
                    int freq = int.Parse(freqString[1]);
                    string[] duraString = halfs[1].Split(')');
                    int duration = int.Parse(duraString[0]);
                    if (freq < 37) { freq = 37; }
                    if (freq > 32767) { freq = 32767; }
                    Console.Beep(freq, duration);
                }
                else
                {
                    string[] duraString = note.Split('(');
                    duraString = duraString[1].Split(')');
                    int duration = int.Parse(duraString[0]);
                    Thread.Sleep(duration);
                }
            }
        }



        public static void PlayKeys(int percNum)
        {
            if (IsRecording && stopWatch.IsRunning)
            { RecPrevKey(); }

            int freq = GetFrequency(percNum);
            int recDuration = 10;
            if (IsRecording)
            {
                string newNote = new("Console.Beep(" + freq + ", " + recDuration + ");");
                newSong.Add(newNote);
                stopWatch.Start();
                IsPause = true;
            }
            Console.Beep(freq, recDuration);
        }

        public static void PlayKeys(ConsoleKey key)
        {
            int freq = GetFrequency(key);
            if (freq == 0) { if (IsQuerty) QWActions(); else BWActions(); }
            if (IsRecording && stopWatch.IsRunning)
            { RecPrevKey(); }

            if (freq < 37) { freq = 37; }
            if (freq > 32767) { freq = 32767; }

            if (IsRecording && !IsStacatto)
            {
                stopWatch.Start();
                PrevFreq = freq;
            }
            Console.Beep(freq, Duration);

            if (IsRecording && IsStacatto)
            {
                int recDuration = Duration;
                string newNote = new("Console.Beep(" + freq + ", " + recDuration + ");");
                newSong.Add(newNote);
                IsPause = true;
                stopWatch.Start();
            }
            if (IsQuerty) QWActions(); else BWActions();
        }
    
    public static void RecPrevKey()
        {
            stopWatch.Stop();
            int recDuration = (int)stopWatch.ElapsedMilliseconds;
            stopWatch.Reset();

            if (IsPause)
            {
                if (recDuration > 2000) { recDuration = 2000; }
                string newNote = new("Thread.Sleep(" + recDuration + ");");
                newSong.Add(newNote);
                IsPause = false;
            }
            else
            {
                string newNote = new("Console.Beep(" + PrevFreq + ", " + recDuration + ");");
                newSong.Add(newNote);
            }
        }  
        public static void StartRec() {
                if (newSong.Count>0) { newSong.Clear(); }
                IsRecording = true;
                InfoMessage("Recording is started!");
        }

        public static void StopRec()
        {
                RecPrevKey();
                IsRecording = false;
                InfoMessage("\nSong recording is ends\n");
                NewSongMenu();
        }

        public static void NewSongMenu()
        {
            Console.WriteLine(
                "P Play recorded song\n" +
                "S Save recorded song\n" +
                "Q Quit to keyboard (all unsaved data will be lost)");
            NewSongActions();
        }

        public static void NewSongActions()
        {
            ConsoleKey key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.P) { PlayBeeps(newSong); NewSongMenu(); }
            else if (key == ConsoleKey.S) { SaveNewSong(); }
            else if (key == ConsoleKey.Q)
            {
                ErrorMessage("Are you shure you want to quit? All unsaved data will be lost! Type Y or N");
                string answer = Console.ReadLine();
                if (answer.ToLower() == "y"|| answer.ToLower() == "н")
                { if (IsQuerty) QWMenu(); else BWMenu(); }
                else { NewSongMenu(); }
            }
            else
            { NewSongActions(); }

        }
        public static void SaveNewSong()
        {
            InfoMessage("Type a name of your song:");
            string name = Console.ReadLine();
            char[] invalidPathChars = Path.GetInvalidPathChars();
            foreach (char invalidChar in invalidPathChars)
            {
                if (name.Contains(invalidChar))
                {
                    ErrorMessage("Name contains not allowed symbols! Please, chose another one.");
                    NewSongMenu();
                }
                else if (File.Exists(name + ".beepersong"))
                {
                    ErrorMessage("This file name is currently exist! Please, chose another one.");
                    NewSongMenu();
                }
                else 
                {
                    File.WriteAllLines(name + ".beepersong", newSong);
                    InfoMessage("Congratulations! The file "+ name + ".beepersong is saved! You can play it using built-in player or use in your program code!");
                }
                BWMenu();
            }
        }

          public static void BWMenu()
        {
            Console.Write(
        "\nA-L for white keys. W, E, T, Y, U, O, P for black keys.\n" +
        "Spacebar, Enter and Z-M for percussions.\n" +
        "1-9 keys set pich shift in octavas.");
            Console.Write(
        "\nMinus and Plus keys adjust pich shift. Now pitch shift is "); InfoString(Pitch + ".\n");
            Console.WriteLine(
         "Num 0 - Num 9 keys set note duration.\n" +
        "UpArrow and DownArrow keys adjust duration.");
            Console.WriteLine(
        "F1 key for information.");
            Console.Write(
        "F2 key for adjust sound duration. Current sound duration is "); InfoString(Duration + ".\n");
            Console.Write(
        "F4 key to turn on/off stacatto recording. Now stacatto is "); InfoString(IsStacatto + ".\n");
            if (!IsRecording)
            { Console.WriteLine("F5 key to start recording."); }
            else
            { Console.WriteLine("F6 key to stop recording."); }
            Console.Write(
        "F9 to open song player menu.\n" +
        "F10 to switch to 24 keys QERTY keyboard.\n" +
        "F12 key to change number of stops in octava. Now in octava "); InfoString(OctavaStops + " stop(s).\n");
            Console.WriteLine(
        "Press Escape to quit.\n");
            BWActions();
        }
        public static void BWActions()
        {
            ConsoleKey key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.Escape)
            {
                if (IsRecording) { StopRec(); }
                else Environment.Exit(0);
            }
            else if (key == ConsoleKey.F1) { Information(); }
            else if (key == ConsoleKey.F2) { SetSoundDuration(); BWMenu(); }
            else if (key == ConsoleKey.F4) { IsStacatto = !IsStacatto; }

            else if (key == ConsoleKey.F5) { if (!IsRecording) { StartRec(); BWMenu(); } else BWActions(); }
            else if (key == ConsoleKey.F6) { if (IsRecording) StopRec(); else BWActions(); }
            else if (key == ConsoleKey.F9) { PlayerMenu(); }
            else if (key == ConsoleKey.F10) { IsQuerty = true; QWMenu(); }
           
            else if (key == ConsoleKey.D1) { OctavaPitch = -3* OctavaShift; BWActions(); }
            else if (key == ConsoleKey.D2) { OctavaPitch = -2* OctavaShift; BWActions(); }
            else if (key == ConsoleKey.D3) { OctavaPitch = -OctavaShift; BWActions(); }
            else if (key == ConsoleKey.D4) { OctavaPitch = 0; BWActions(); }
            else if (key == ConsoleKey.D5) { OctavaPitch = OctavaShift; BWActions(); }
            else if (key == ConsoleKey.D6) { OctavaPitch = 2* OctavaShift; BWActions(); }
            else if (key == ConsoleKey.D7) { OctavaPitch = 3* OctavaShift; BWActions(); }
            else if (key == ConsoleKey.D8) { OctavaPitch = 4* OctavaShift; BWActions(); }
            else if (key == ConsoleKey.D9) { OctavaPitch = 5* OctavaShift; BWActions(); }
            else if (key == ConsoleKey.D0) { OctavaPitch = 0; Pitch = 0; BWMenu(); }
            else if (key == ConsoleKey.F12) { SetStopsInOctava(); }

            else if (key == ConsoleKey.Spacebar) { PlayKeys(0); BWActions(); }
            else if (key == ConsoleKey.Z) { PlayKeys(1); BWActions(); }
            else if (key == ConsoleKey.X) { PlayKeys(2); BWActions(); }
            else if (key == ConsoleKey.C) { PlayKeys(3); BWActions(); }
            else if (key == ConsoleKey.V) { PlayKeys(4); BWActions(); }
            else if (key == ConsoleKey.B) { PlayKeys(5); BWActions(); }
            else if (key == ConsoleKey.N) { PlayKeys(6); BWActions(); }
            else if (key == ConsoleKey.M) { PlayKeys(7); BWActions(); }
            else if (key == ConsoleKey.OemComma) { PlayKeys(8); BWActions(); }
            else if (key == ConsoleKey.OemPeriod) { PlayKeys(9); BWActions(); }
            else if (key == ConsoleKey.Oem2) { PlayKeys(10); BWActions(); }
            else if (key == ConsoleKey.Enter) { PlayKeys(11); BWActions(); }
            else if (key == ConsoleKey.NumPad0) { Duration = 5; BWActions(); }
            else if (key == ConsoleKey.NumPad1) { Duration = 10; BWActions(); }
            else if (key == ConsoleKey.NumPad2) { Duration = 100; BWActions(); }
            else if (key == ConsoleKey.NumPad3) { Duration = 200; BWActions(); }
            else if (key == ConsoleKey.NumPad4) { Duration = 300; BWActions(); }
            else if (key == ConsoleKey.NumPad5) { Duration = 400; BWActions(); }
            else if (key == ConsoleKey.NumPad6) { Duration = 500; BWActions(); }
            else if (key == ConsoleKey.NumPad7) { Duration = 600; BWActions(); }
            else if (key == ConsoleKey.NumPad8) { Duration = 700; BWActions(); }
            else if (key == ConsoleKey.NumPad9) { Duration = 800; BWActions(); }
            else if (key == ConsoleKey.OemMinus)
            {
                if (Pitch > -OctavaShift)
                {
                    Pitch--;
                    BWMenu();
                }
                else { BWActions(); }
            }
            else if (key == ConsoleKey.OemPlus)
            {
                if (Pitch < OctavaShift)
                {
                    Pitch++;
                    BWMenu();
                }
                else { BWActions(); }
            }
            else if (key == ConsoleKey.DownArrow)
            {
                if (Duration > 3)
                {
                    Duration = (int)Math.Round(Convert.ToDouble(Duration) * 0.9);
                    BWMenu();
                }
                else { BWActions(); }
            }
            else if (key == ConsoleKey.UpArrow)
            {
                if (Duration < 1000)
                {
                    Duration = (int)Math.Round(Convert.ToDouble(Duration) * 1.167);
                    BWMenu();
                }
                else { BWActions(); }
            }
            else { PlayKeys(key); }
        }

        public static void QWMenu()
        {
            Console.WriteLine(
        "Q and A keyboard lines for play notes.\n" +
        "Spacebar, Enter and Z-M for percussions.\n" +
        "1-9 keys set pich shift in octavas.");
            Console.Write(
        "Minus and Plus keys adjust pich shift. Now pitch shift is "); InfoString(Pitch + ".\n");
            Console.WriteLine(
        "Num 0 - Num 9 keys set note duration.\n" +
        "UpArrow and DownArrow keys adjust duration.");
            Console.WriteLine(
        "F1 key for information.");
            Console.Write(
        "F2 key for adjust sound duration. Current sound duration is "); InfoString(Duration + ".\n");
            Console.Write(
        "F4 key to turn on/off stacatto recording. Now stacatto is "); InfoString(IsStacatto + ".\n");
            if (!IsRecording)
            { Console.WriteLine("F5 key to start recording."); }
            else
            { Console.WriteLine("F6 key to stop recording."); }
            Console.Write(
        "F9 to open song player menu.\n" +
        "F10 to switch to classic black and white keyboard.\n" +
        "F12 key to change number of stops in octava. Now in octava "); InfoString(OctavaStops + " stop(s).");
            Console.WriteLine(
        "\nPress Escape to quit.\n");
            QWActions();
        }
        public static void QWActions()
        {
            ConsoleKey key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.Escape)
            {
                if (IsRecording) { StopRec(); }
                else Environment.Exit(0);
            }
            else if (key == ConsoleKey.F1) { Information(); }
            else if (key == ConsoleKey.F2) { SetSoundDuration(); }
            else if (key == ConsoleKey.F4) { IsStacatto = !IsStacatto; QWMenu(); }
            else if (key == ConsoleKey.F5) { if (!IsRecording) { StartRec(); QWMenu(); } else QWActions(); }
            else if (key == ConsoleKey.F6) { if (IsRecording) StopRec(); else QWActions(); }
            else if (key == ConsoleKey.F9) { PlayerMenu(); }
            else if (key == ConsoleKey.F10) { IsQuerty = false; BWMenu(); }
            else if (key == ConsoleKey.F12) { SetStopsInOctava(); }

            else if (key == ConsoleKey.D1) { OctavaPitch = -3 * OctavaShift; QWActions(); }
            else if (key == ConsoleKey.D2) { OctavaPitch = -2 * OctavaShift; QWActions(); }
            else if (key == ConsoleKey.D3) { OctavaPitch = -OctavaShift; QWActions(); }
            else if (key == ConsoleKey.D4) { OctavaPitch = 0; QWActions(); }
            else if (key == ConsoleKey.D5) { OctavaPitch = OctavaShift; QWActions(); }
            else if (key == ConsoleKey.D6) { OctavaPitch = 2 * OctavaShift; QWActions(); }
            else if (key == ConsoleKey.D7) { OctavaPitch = 3 * OctavaShift; QWActions(); }
            else if (key == ConsoleKey.D8) { OctavaPitch = 4 * OctavaShift; QWActions(); }
            else if (key == ConsoleKey.D9) { OctavaPitch = 5 * OctavaShift; QWActions(); }
            else if (key == ConsoleKey.D0) { OctavaPitch = 0; Pitch = 0; QWMenu(); }

            else if (key == ConsoleKey.Spacebar) { PlayKeys(0); QWActions(); }
            else if (key == ConsoleKey.Z) { PlayKeys(1); QWActions(); }
            else if (key == ConsoleKey.X) { PlayKeys(2); QWActions(); }
            else if (key == ConsoleKey.C) { PlayKeys(3); QWActions(); }
            else if (key == ConsoleKey.V) { PlayKeys(4); QWActions(); }
            else if (key == ConsoleKey.B) { PlayKeys(5); QWActions(); }
            else if (key == ConsoleKey.N) { PlayKeys(6); QWActions(); }
            else if (key == ConsoleKey.M) { PlayKeys(7); QWActions(); }
            else if (key == ConsoleKey.OemComma) { PlayKeys(8); QWActions(); }
            else if (key == ConsoleKey.OemPeriod) { PlayKeys(9); QWActions(); }
            else if (key == ConsoleKey.Oem2) { PlayKeys(10); QWActions(); }
            else if (key == ConsoleKey.Enter) { PlayKeys(11); QWActions(); }
            else if (key == ConsoleKey.NumPad0) { Duration = 5; QWActions(); }
            else if (key == ConsoleKey.NumPad1) { Duration = 10; QWActions(); }
            else if (key == ConsoleKey.NumPad2) { Duration = 100; QWActions(); }
            else if (key == ConsoleKey.NumPad3) { Duration = 200; QWActions(); }
            else if (key == ConsoleKey.NumPad4) { Duration = 300; QWActions(); }
            else if (key == ConsoleKey.NumPad5) { Duration = 400; QWActions(); }
            else if (key == ConsoleKey.NumPad6) { Duration = 500; QWActions(); }
            else if (key == ConsoleKey.NumPad7) { Duration = 600; QWActions(); }
            else if (key == ConsoleKey.NumPad8) { Duration = 700; QWActions(); }
            else if (key == ConsoleKey.NumPad9) { Duration = 800; QWActions(); }
            else if (key == ConsoleKey.OemMinus)
            {
                if (Pitch > -OctavaShift)
                {
                    Pitch--;
                    QWMenu();
                }
                else { QWActions(); }
            }
            else if (key == ConsoleKey.OemPlus)
            {
                if (Pitch < OctavaShift)
                {
                    Pitch++;
                    QWMenu();
                }
                else { QWActions(); }
            }
            else if (key == ConsoleKey.DownArrow)
            {
                if (Duration > 3)
                {
                    Duration = (int)Math.Round(Convert.ToDouble(Duration) * 0.9);
                    QWMenu();
                }
                else { QWActions(); }
            }
            else if (key == ConsoleKey.UpArrow)
            {
                if (Duration < 1000)
                {
                    Duration = (int)Math.Round(Convert.ToDouble(Duration) * 1.167);
                    QWMenu();
                }
                else { QWActions(); }
            }
          
            else { PlayKeys(key); }
        }
        public static void SetSoundDuration()
        {
            InfoMessage("Current sound duration is " + Duration + ". Type new duration (from 2 to 1000) and press Enter:");
            string isDuration = Console.ReadLine();
            bool success = int.TryParse(isDuration, out int number);
            if (success && (number >= 2 && number <= 1000))
            {
                Duration = number;
                InfoMessage("New duration is " + Duration + "\n");
                BWMenu();
            }
            else
            {
                ErrorMessage("Invalid data!");
                SetSoundDuration();
            }
        }
        public static void PlayerMenu()
        {
            Console.WriteLine(
           "P Play the song from file\n" +
           "T Set the tempo. Current tempo is " + Tempo + "\n" +
           "S Set the pitch shift. Current pich shift is " + Pitch + "\n" +
           "Q Quit to the Start Menu");
            PlayerActions();
        }
        public static void PlayerActions()
        {
            ConsoleKey key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.Q) { if (IsQuerty) QWMenu(); else BWMenu(); }
            else if (key == ConsoleKey.P) { PlaySong(); }
            else if (key == ConsoleKey.T) { SetUpTempo(); }
            else if (key == ConsoleKey.S) { SetUpPitchShift(); }
            else { PlayerActions(); }
        }
        public static void SetUpTempo() 
        {
            InfoMessage("Current tempo is " + Tempo);
            InfoMessage("Set up the tempo (from 10 to 200): ");
            string isTempo = Console.ReadLine();

            bool success = int.TryParse(isTempo, out int number);
            if (success&&(number >= 10 && number <= 200))
            {
                    Tempo = number;
                    InfoMessage("New tempo is " + Tempo + "\n");
                PlayerMenu();
            }
            else {
                ErrorMessage("Invalid data!");
                SetUpTempo(); 
            }
        }
        public static void SetUpPitchShift()
        {
            InfoMessage("Current pich shift is " + Pitch);
            InfoMessage("Set up the pitch shift (from -30 to 50): ");
            string isPitch = Console.ReadLine();
            bool success = int.TryParse(isPitch, out int number);
            if (success&&(number >= -30 && number <= 50))
            {
                    Pitch = number;
                    InfoMessage("New pitch shift is " + Pitch + "\n");
                    PlayerMenu();
            }
            else {

                ErrorMessage("Invalid data!");
                SetUpPitchShift();
            }
        }
        public static void PlaySong()
        {
            string path = Directory.GetCurrentDirectory();
            string[] fileEntries = Directory.GetFiles(path, "*.beepersong");
            foreach (string entry in fileEntries)
            {
                string[] pathToSong = entry.Split('\\');
                string song = pathToSong.Last().Remove(pathToSong.Last().Length - 11);                
                InfoMessage(song);
            }
            InfoMessage("Type song name to start playing or Exit to return to player menu");
            string songName = Console.ReadLine();
            if (songName.ToLower() == "exit"|| songName.ToLower() == "учше")
            { PlayerMenu(); }
            else
            {
                string filename = path + '\\' + songName + ".beepersong";
                Console.WriteLine(filename);
                if (!File.Exists(filename))
                {
                    ErrorMessage("Song with name " + songName + " is not exist! Chose another one!");
                    PlaySong();
                }
                else
                {
                    string[] readSong = File.ReadAllLines(filename);
                    if (readSong.Length == 0) {
                        ErrorMessage("File " + songName + " is empty! Chose another one!");
                        PlaySong();
                        }
                    else if (readSong[0].Substring(1, 1) == "o")
                    {
                        List<string> playSong = new(readSong);
                        if (IsFileValid(playSong))
                        {
                            InfoMessage("Playing " + songName);
                            PlayBeeps(playSong);
                        }
                        else
                        {
                            ErrorMessage("File " + songName + " is invalid! Chose another one!");
                            PlaySong();
                        }
                    }
                    else
                    {
                        string playSong = new("");
                        foreach (string note in readSong)
                        { playSong = playSong + note + ' '; }
                        playSong = playSong.Remove(playSong.Length - 1);

                        if (IsFileValid(playSong))

                        {
                            InfoMessage("Playing " + songName);
                            PlayBeeps(playSong);
                        }
                        else
                        {
                            ErrorMessage("File " + songName + " is invalid! Chose another one!");
                            PlaySong();
                        }
                    }
                    PlayerMenu();
                }
            }
        }
        static bool IsFileValid(List<string> song)
        {
                foreach (string note in song)
                    {
                try
                    {
                        if (note.First() == 'C')
                        {
                            string[] halfs = note.Split(", ");
                            string[] freqString = halfs[0].Split('(');
                            int freq = int.Parse(freqString[1]);
                            string[] duraString = halfs[1].Split(')');
                            int duration = int.Parse(duraString[0]);
                            if (freq < 37 || freq > 32767 || duration < 2 || duration > 2000)
                            { return false; }
                        }
                        else
                        {
                            string[] duraString = note.Split('(');
                            duraString = duraString[1].Split(')');
                            int duration = int.Parse(duraString[0]);
                            if (duration < 2 || duration > 2000)
                            { return false; }
                        }
                    }
                catch { return false; }
                    }
            return true;
        }
        static bool IsFileValid(string song)
        {
            try
            {
                string[] notes = song.Split(", ");
                for (int i = 0; i < notes.Length; i++)
                {
                    string[] toneAndDuration = notes[i].Split(' ');
                    string toneName = toneAndDuration[0];
                    string durationName = toneAndDuration[1];
                    int duration = GetDuration(durationName);
                    if (duration < 2 || duration > 2000)
                    { return false; }
                    else if (toneName != "pause" && (GetFrequency(toneName) < 37 || GetFrequency(toneName) > 32767))
                    { return false; }
                }
            }
            catch { return false; }
            return true;
        }
        public static void Information()
        {
            Console.WriteLine(
                "Hello! It is a Beeper Piano!\n" + 
                "Author: Oleksandr Menzerov.\n" +
                "You can play beeps on your keyboard and record Console.Beep commands to the file.\n" +
                "Then you can add this code to your console C# programms.\n" +
                "Also you can create your own .beepersong files with songs in American notation system.\n" +
                "Look at the Anthem file for example.\n" +
                "If you want to add Italian notation system regognizing or song convertor to Console.Beep commands\n" +
                "write to alexmenzer@gmail.com\n" +
                "My king regards for using this app!\n" +
                "Press Escape to return to the Beeper Piano.\n");
            InformationActions();
        }
        public static void InformationActions()
        {
            ConsoleKey key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.Escape) { if (IsQuerty) QWMenu(); else BWMenu(); }
            else { InformationActions(); }
        }
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(
                        "Hello! It is a Beeper Piano!\n" +
                        "Play beeps, record beeps, reproduce beeps!\n");
            Tempo = 70;
            Pitch = 0;
            OctavaPitch = 0;
            Duration = 150;
            IsStacatto = false;
            IsRecording = false;
            IsPause = false;
            OctavaStops = 12;
            OctavaShift = OctavaStops;
            PlayBeeps(startMelody);
            BWMenu();
        }
    }
}
