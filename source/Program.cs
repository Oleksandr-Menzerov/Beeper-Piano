using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

namespace BeeperPiano
{
    static class Program
    {
        private const int etalonFrequency = 440;
        private const int milliseconds = 60000;
        public static int Tempo { get; set; }
        public static int Pitch { get; set; }
        public static int Duration { get; set; }
        public static bool NotesAppear { get; set; }
        public static bool DurationTimer { get; set; }
        public static bool Recording { get; set; }
        public static readonly List<string> newSong = new();
        static readonly Stopwatch stopWatch = new();
        public static readonly string startMelody = new("C5 1/8., B4 1/8., A4 1/8., G4 1/8., F4 1/8., E4 1/8., D4 1/8., C4 1/4, pause 1/4, C5 1/2");

        public static void ErrorMessage(string error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(error);
            Console.ResetColor();
            Console.Beep();
        }
        public static int GetFrequency(string toneName)
        {
            double toneIndex = 0;
            if (toneName.First() == 'C')
            { toneIndex -= 9; }
            else if (toneName.First() == 'D')
            { toneIndex -= 7; }
            else if (toneName.First() == 'E')
            { toneIndex -= 5; }
            else if (toneName.First() == 'F')
            { toneIndex -= 4; }
            else if (toneName.First() == 'G')
            { toneIndex -= 2; }
            else if (toneName.First() == 'B')
            { toneIndex += 2; }
            if (toneName.Substring(1, 1) == "#")
            { toneIndex++; }
            else if (toneName.Substring(1, 1) == "b")
            { toneIndex--; }

            toneIndex = toneIndex + (int.Parse(toneName.Last().ToString()) - 4) * 12 + Pitch;
            double power = toneIndex / 12;
            double dobleFreg = etalonFrequency * Math.Pow(2, power);
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

        public static void PlayKeys(string note)
        {

            if (Recording&& !DurationTimer&& stopWatch.IsRunning)
            {
                    stopWatch.Stop();
                    int recDuration = (int)stopWatch.ElapsedMilliseconds;
                    stopWatch.Reset();
                    if (recDuration > 2000) { recDuration = 2000; }
                    string newNote = new("Thread.Sleep("+recDuration+");");
                    newSong.Add(newNote);
            }

            int freq = GetFrequency(note);
            if (freq < 37) { freq = 37; }
            if (freq > 32767) { freq = 32767; }
            Console.Beep(freq, Duration);

            if (Recording)
            {
                int recDuration = Duration;
                if (DurationTimer)
                {
                    stopWatch.Stop();
                    recDuration = (int)stopWatch.ElapsedMilliseconds;
                    stopWatch.Reset();
                    stopWatch.Start();
                }
                if (recDuration > 2000) { recDuration = 2000; }
                if (freq < 37) { freq = 37; }
                if (freq > 32767) { freq = 32767; }
                string newNote = new("Console.Beep("+ freq+", " + recDuration + ");");
                newSong.Add(newNote);
                if (!DurationTimer)
                {
                    stopWatch.Start();
                }
            }

            if (NotesAppear) { Console.Write(note.Remove(note.Length - 1) + " "); }

        }

        public static void StartRec() {
            if (!Recording)
            {
                if (newSong.Count>0) { newSong.Clear(); }
                Recording = true;
                Console.WriteLine("Recording is started!");
                if (DurationTimer) { stopWatch.Start(); }
                Menu();
            }
            else Actions();
        }

        public static void StopRec()
        {
            if (Recording)
            {
                if (stopWatch.IsRunning) { stopWatch.Stop(); }
                stopWatch.Reset();
                Recording = false;
                Console.WriteLine("\nSong recording is ends\n");
                NewSongMenu();
            }
            else Actions();
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
                if (answer.ToLower() == "y")
                { Menu(); }
                else if (answer.ToLower() == "n")
                { NewSongMenu(); }
                else
                {
                    NewSongMenu();
                }
            }
        }
        public static void SaveNewSong()
        {
            Console.WriteLine("Type a name of your song:");
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
                    Console.WriteLine("Congratulations! The file "+ name + ".beepersong is saved! You can play it using built-in player or use in your program code!");
                }
                Menu();
            }
        }
        public static void Menu() {
            Console.WriteLine(
        "A-L for white keys. W, E, T, Y, U, O, P for black keys.\n" +
        "1-9 keys set pich shift in octavas.\n" +
        "UpArrow and DownArrow keys adjust pich shift in halftones.\n" +
        "Num 0 - Num 9 keys set note durations as 5, 10, 100, 200, 300, 400, 500, 600, 700, 800.\n" +
        "Num Plus and Num Minus keys adjust durations by 1 millisecond.");
            Console.WriteLine(
        "F1 key for information.");
            Console.WriteLine(
        "F2 key for adjust sound duration. Current sound duration is " + Duration + ".");
            Console.WriteLine(
        "F3 key for turn on/off notes names appearing in console. Current note appearing is " + NotesAppear + ".");
            Console.WriteLine(
        "F4 key to switch between legatto/stacatto recording. Now stakatto is " + DurationTimer + ".");
            if (!Recording)
            { Console.WriteLine("F5 key to start recording."); }
            else
            { Console.WriteLine("F6 key to stop recording."); }
            Console.WriteLine(
        "F9 to open song player menu.\n" +
        "Press Escape to quit.\n");
            Actions();
        }
        public static void Actions()
        {
            ConsoleKey key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.Escape) { Environment.Exit(0); }
            else if (key == ConsoleKey.F1) { Information(); }
            else if (key == ConsoleKey.F2) { SetSoundDuration(); }
            else if (key == ConsoleKey.F3) { SwitchNotesAppearing(); }
            else if (key == ConsoleKey.F4) { SwitchDurationTimer(); }
            else if (key == ConsoleKey.F5) { StartRec(); }
            else if (key == ConsoleKey.F6) { StopRec(); }
            else if (key == ConsoleKey.F9) { PlayerMenu(); }
            else if (key == ConsoleKey.A) { PlayKeys("C4"); Actions(); }
            else if (key == ConsoleKey.W) { PlayKeys("C#4"); Actions(); }
            else if (key == ConsoleKey.S) { PlayKeys("D4"); Actions(); }
            else if (key == ConsoleKey.E) { PlayKeys("D#4"); Actions(); }
            else if (key == ConsoleKey.D) { PlayKeys("E4"); Actions(); }
            else if (key == ConsoleKey.F) { PlayKeys("F4"); Actions(); }
            else if (key == ConsoleKey.T) { PlayKeys("F#4"); Actions(); }
            else if (key == ConsoleKey.G) { PlayKeys("G4"); Actions(); }
            else if (key == ConsoleKey.Y) { PlayKeys("G#4"); Actions(); }
            else if (key == ConsoleKey.H) { PlayKeys("A4"); Actions(); }
            else if (key == ConsoleKey.U) { PlayKeys("A#4"); Actions(); }
            else if (key == ConsoleKey.J) { PlayKeys("B4"); Actions(); }
            else if (key == ConsoleKey.K) { PlayKeys("C5"); Actions(); }
            else if (key == ConsoleKey.O) { PlayKeys("C#5"); Actions(); }
            else if (key == ConsoleKey.L) { PlayKeys("D5"); Actions(); }
            else if (key == ConsoleKey.P) { PlayKeys("D#5"); Actions(); }
            else if (key == ConsoleKey.D0 || key == ConsoleKey.D4) { Pitch = 0; Actions(); }
            else if (key == ConsoleKey.D1) { Pitch = -36; Actions(); }
            else if (key == ConsoleKey.D2) { Pitch = -24; Actions(); }
            else if (key == ConsoleKey.D3) { Pitch = -12; Actions(); }
            else if (key == ConsoleKey.D5) { Pitch = 12; Actions(); }
            else if (key == ConsoleKey.D6) { Pitch = 24; Actions(); }
            else if (key == ConsoleKey.D7) { Pitch = 36; Actions(); }
            else if (key == ConsoleKey.D8) { Pitch = 48; Actions(); }
            else if (key == ConsoleKey.D9) { Pitch = 60; Actions(); }
            else if (key == ConsoleKey.NumPad0) { Duration = 5; Actions(); }
            else if (key == ConsoleKey.NumPad1) { Duration = 10; Actions(); }
            else if (key == ConsoleKey.NumPad2) { Duration = 100; Actions(); }
            else if (key == ConsoleKey.NumPad3) { Duration = 200; Actions(); }
            else if (key == ConsoleKey.NumPad4) { Duration = 300; Actions(); }
            else if (key == ConsoleKey.NumPad5) { Duration = 400; Actions(); }
            else if (key == ConsoleKey.NumPad6) { Duration = 500; Actions(); }
            else if (key == ConsoleKey.NumPad7) { Duration = 600; Actions(); }
            else if (key == ConsoleKey.NumPad8) { Duration = 700; Actions(); }
            else if (key == ConsoleKey.NumPad9) { Duration = 800; Actions(); }
            else if (key == ConsoleKey.DownArrow)
            { if (Pitch > -30) { Pitch--;
                    Console.WriteLine("Pitch shift set to " + Pitch + " ");
                    Menu(); }
                else { Actions(); }
            }
            else if (key == ConsoleKey.UpArrow)
            { if (Pitch < 60) { Pitch++;
                    Console.WriteLine("Pitch shift set to " + Pitch + " ");
                    Menu(); }
                else { Actions(); }
            }
            else if (key == ConsoleKey.Subtract)
            {
                if (Duration > 800)
                {
                    Duration -= 100;
                    Console.WriteLine("Duration set to " + Duration + " ");
                    Menu();
                }
                else if(Duration > 500)
                {
                    Duration -= 50;
                    Console.WriteLine("Duration set to " + Duration + " ");
                    Menu();
                }
                else if (Duration > 200)
                {
                    Duration -= 10;
                    Console.WriteLine("Duration set to " + Duration + " ");
                    Menu();
                }
                else if (Duration > 2)
                {
                    Duration--;
                    Console.WriteLine("Duration set to " + Duration + " ");
                    Menu();
                }
                else { Actions(); }
            }
            else if (key == ConsoleKey.Add)
            {
                if (Duration < 25)
                {
                    Duration++;
                    Console.WriteLine("Duration set to " + Duration + " ");
                    Menu();
                }
                else if(Duration < 75)
                {
                    Duration += 10;
                    Console.WriteLine("Duration set to " + Duration + " ");
                    Menu();
                }
                else if(Duration < 200)
                {
                    Duration += 25;
                    Console.WriteLine("Duration set to " + Duration + " ");
                    Menu();
                }
                else if(Duration < 400)
                {
                    Duration += 45;
                    Console.WriteLine("Duration set to " + Duration + " ");
                    Menu();
                }
                else if(Duration < 700)
                {
                    Duration += 75;
                    Console.WriteLine("Duration set to " + Duration + " ");
                    Menu();
                }
                else if (Duration < 1000)
                {
                    Duration += 100;
                    Console.WriteLine("Duration set to " + Duration + " ");
                    Menu();
                }
                else { Actions(); }
            }
            else { Actions(); }
        }
        public static void SwitchNotesAppearing() {
            NotesAppear = !NotesAppear;
            if (NotesAppear) { Console.WriteLine("Notes apearing is turned on.\n"); }
            else { Console.WriteLine("Notes apearing is turned off.\n"); }
            Menu();
        }
        public static void SwitchDurationTimer()
        {
            DurationTimer = !DurationTimer;
            if (DurationTimer) { Console.WriteLine("Legatto recording is turned on.\n"); }
            else { Console.WriteLine("Stacatto recording is turned on.\n"); }
            Menu();
        }
        public static void SetSoundDuration()
        {
            Console.WriteLine("Current sound duration is " + Duration + ". Type new duration (from 2 to 1000) and press Enter:");
            string isDuration = Console.ReadLine();
            bool success = int.TryParse(isDuration, out int number);
            if (success && (number >= 2 && number <= 1000))
            {
                Duration = number;
                Console.WriteLine("New duration is " + Duration + "\n");
                Menu();
            }
            else
            {
                ErrorMessage("Invalid data!");
                Console.ForegroundColor = ConsoleColor.Red;
                SetSoundDuration();
            }
        }
        public static void PlayerMenu()
        {
            Console.WriteLine(
           "P Play the song from file\n" +
           "T Set the tempo. Current tempo is " + Tempo + "\n" +
           "S Set the pitch shift. Current pich shift is " + Pitch + "\n" +
           "Q Quit to the Beeper Piano");
            PlayerActions();
        }
        public static void PlayerActions()
        {
            ConsoleKey key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.Q) { Menu(); }
            else if (key == ConsoleKey.P) { PlaySong(); }
            else if (key == ConsoleKey.T) { SetUpTempo(); }
            else if (key == ConsoleKey.S) { SetUpPitchShift(); }
            else { PlayerActions(); }
        }
        public static void SetUpTempo() 
        {
            Console.WriteLine("Current tempo is " + Tempo);
            Console.WriteLine("Set up the tempo (from 10 to 200): ");
            string isTempo = Console.ReadLine();

            bool success = int.TryParse(isTempo, out int number);
            if (success&&(number >= 10 && number <= 200))
            {
                    Tempo = number;
                    Console.WriteLine("New tempo is " + Tempo + "\n");
                PlayerMenu();
            }
            else {
                ErrorMessage("Invalid data!");
                SetUpTempo(); 
            }
        }
        public static void SetUpPitchShift()
        {
            Console.WriteLine("Current pich shift is " + Pitch);
            Console.WriteLine("Set up the pitch shift (from -30 to 50): ");
            string isPitch = Console.ReadLine();
            bool success = int.TryParse(isPitch, out int number);
            if (success&&(number >= -30 && number <= 50))
            {
                    Pitch = number;
                    Console.WriteLine("New pitch shift is " + Pitch + "\n");
                    PlayerMenu();
            }
            else {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid data!");
                Console.ResetColor();
                Console.Beep();
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
                Console.WriteLine(song);
            }
            Console.WriteLine("Type song name to start playing or Exit to return to player menu");
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
                            Console.WriteLine("Playing " + songName);
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
                            Console.WriteLine("Playing " + songName);
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
            if (key == ConsoleKey.Escape) { Menu(); }
            else { InformationActions(); }
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Hello! It is a Beeper Piano! Enjoy!\n" + "\n");
            Tempo = 70;
            Pitch = 0;
            Duration = 150;
            NotesAppear = false;
            DurationTimer = true;
            Recording = false;
            PlayBeeps(startMelody);
            Menu();
        }
    }
}
