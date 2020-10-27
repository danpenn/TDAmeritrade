using System.IO;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace TDProvider {
    /// <summary>
    /// Streamer is the main class for streaming data with the TD Ameritrade service
    /// </summary>
    public partial class Streamer {
        // Member Variables
        CancellationTokenSource playbackCancellationTokenSource;

        /// <summary>
        /// StartCapturingData begins capturing data for the specified amount of time (in seconds)
        /// </summary>
        /// <param name="filename">Filename to store the captured data in</param>
        /// <param name="captureDurationSeconds">The number of seconds to capture data for</param>
        public void StartCapturingData(string filename, double captureDurationSeconds) {
            this.captureData = true;
            this.captureDataStreamWriter = new StreamWriter(filename, false);
            this.captureUntilTime = DateTime.Now.AddSeconds(captureDurationSeconds);
            this.captureDataFilename = filename;
        }

        /// <summary>
        /// StopCapturingData stops the capture process. Note that calling this is only helpful if you've
        /// started a capture and wish to stop the capture before the requested capture duration has completed
        /// </summary>
        public void StopCapturingData() {
            this.captureData = false;
            if (this.captureDataStreamWriter != null) {
                this.captureDataStreamWriter.Close();
                Log(LogType.Info, $"The data capture has completed. Results are saved in {this.captureDataFilename}");
            }
        }

        /// <summary>
        /// Plays back data items from a previously captured session
        /// </summary>
        /// <param name="filename">Filename of previously captured data from which to play back</param>
        /// <param name="interval">Specify an interval of 0 to have items played back at the same rate they were 
        /// initially captured.  Otherwise, specify the number of milliseconds as the interval and it will play the 
        /// items back at a consistent cadence.  1000 milliseconds equals 1 second</param>
        /// <returns></returns>
        public void PlaybackCapturedData(string filename, int interval) {
            // Start streaming data
            this.playbackCancellationTokenSource = new CancellationTokenSource();

            var tasks = new List<Task> {
                processCaptureFile(filename, interval)
            };
        }

        /// <summary>
        /// StopPlaybackOfCapturedData stops the simulated playback of captured data
        /// </summary>
        public void StopPlaybackOfCapturedData() {
            this.playbackCancellationTokenSource.Cancel();
        }

        private void writeDataForCapture(string responseData) {
            this.captureDataStreamWriter.WriteLine($"{DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt")}||{responseData}");

            // Check to see if we should stop capturing based on time
            if (DateTime.Now >= this.captureUntilTime) {
                this.captureData = false;
                this.captureDataStreamWriter.Close();
                Log(LogType.Info, $"The data capture has completed. Results are saved in {this.captureDataFilename}");
            }
        }

        private async Task processCaptureFile(string filename, int interval) {
            if (!File.Exists(filename)) {
                throw new FileNotFoundException();
            }
            
            System.IO.StreamReader file = new System.IO.StreamReader(filename);  
            string line;
            int counter = 0;
            DateTime previousTimestamp = DateTime.MinValue;

            while((line = file.ReadLine()) != null)  
            {  
                counter++;  
                Log(LogType.Info, $"{counter}: {line}");

                CaptureData captureData = new CaptureData(line);

                if (!captureData.isValidData) {
                    Log(LogType.Warning, $"Data read from the file is invalid - {line}");
                    continue;
                }

                if (interval > 0) {
                    await Task.Delay(interval);
                } else {
                    if (previousTimestamp == DateTime.MinValue) {
                        previousTimestamp = captureData.timestamp;
                    }

                    TimeSpan diff = captureData.timestamp - previousTimestamp;

                    await Task.Delay(diff.Milliseconds);
                    previousTimestamp = captureData.timestamp;
                }
                
                processDataResponse(captureData.payload);
            }  
            Log(LogType.Info, "Playback of captured data has completed.");
        }
    }

    class CaptureData {
        public DateTime timestamp;
        public string payload;
        public bool isValidData;

        public CaptureData(string captureFileLineData) {
            var parts = captureFileLineData.Split("||");
            if (parts.Length != 2) {
                this.isValidData = false;
                return;
            }

            this.timestamp = DateTime.Parse(parts[0]);
            this.payload = parts[1];
            this.isValidData = true;
        }
    }
}