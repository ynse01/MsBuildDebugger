using Microsoft.Build.Framework;
using System;

namespace MsBuildDebugger
{
    public class DebugLogger : ILogger
    {
        public LoggerVerbosity Verbosity { get; set; }
        public string Parameters { get; set; }

        private IEventSource3 source;
        private Debugger debugger;

        public void Initialize(IEventSource eventSource)
        {
            source = eventSource as IEventSource3;
            //source.ProjectStarted += OnProjectStarted;
            //source.ProjectFinished += OnProjectFinished;
            //source.BuildStarted += OnBuildStarted;
            //source.BuildFinished += OnBuildFinished;
            source.AnyEventRaised += OnAnyEvent;
        }

        public void Shutdown()
        {
            if (source != null)
            {
                source.ProjectStarted -= OnProjectStarted;
                source.ProjectFinished -= OnProjectFinished;
                source.BuildStarted -= OnBuildStarted;
                source.BuildFinished -= OnBuildFinished;
                source.AnyEventRaised -= OnAnyEvent;
            }
        }

        private void OnAnyEvent(object sender, BuildEventArgs args)
        {
            if (args is TargetStartedEventArgs targetStartedArgs)
            {
                debugger.OnTargetEnter(targetStartedArgs.TargetName);
            }
            else if (args is TargetFinishedEventArgs targetFinishedArgs)
            {
                debugger.OnTargetLeave(targetFinishedArgs.TargetName);
            }
            else if (args is BuildMessageEventArgs)
            {
                //Console.WriteLine("Message: " + ((BuildMessageEventArgs)args).Message);
            }
            else if (args is TaskStartedEventArgs)
            {
                // Nothing to do
            }
            else if (args is TaskFinishedEventArgs)
            {
                // Nothing to do
            }
            else if (args is BuildStartedEventArgs buildStartedArgs)
            {
                OnBuildStarted(sender, buildStartedArgs);
            }
            else if (args is BuildFinishedEventArgs buildFinishedArgs)
            {
                OnBuildFinished(sender, buildFinishedArgs);
            }
            else if (args is ProjectEvaluationStartedEventArgs)
            {
                // Nothing to do
            }
            else if (args is ProjectEvaluationFinishedEventArgs)
            {
                // Nothing to do
            }
            else if (args is ProjectStartedEventArgs projectStartedArgs)
            {
                OnProjectStarted(sender, projectStartedArgs);
            }
            else if (args is ProjectFinishedEventArgs projectFinishedArgs)
            {
                OnProjectFinished(sender, projectFinishedArgs);
            }
            else
            {
                Console.WriteLine("Unknown event: " + args.GetType().Name);
            }
        }

        private void OnProjectStarted(object sender, ProjectStartedEventArgs args) {
            debugger = new Debugger(args.ProjectFile, Helper.SplitValue(args.TargetNames));
            debugger.SetUI(new ConsoleUI(debugger));
        }

        private void OnProjectFinished(object sender, ProjectFinishedEventArgs args)
        {
        }

        private void OnBuildStarted(object sender, BuildStartedEventArgs args)
        {
        }

        private void OnBuildFinished(object sender, BuildFinishedEventArgs args)
        {
        }
    }
}
