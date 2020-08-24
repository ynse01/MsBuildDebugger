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
            if (args is TargetStartedEventArgs)
            {
                debugger.OnTargetEnter(((TargetStartedEventArgs)args).TargetName);
            }
            else if (args is TargetFinishedEventArgs)
            {
                debugger.OnTargetLeave(((TargetFinishedEventArgs)args).TargetName);
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
            else if (args is BuildStartedEventArgs)
            {
                OnBuildStarted(sender, (BuildStartedEventArgs)args);
            }
            else if (args is BuildFinishedEventArgs)
            {
                OnBuildFinished(sender, (BuildFinishedEventArgs)args);
            }
            else if (args is ProjectEvaluationStartedEventArgs)
            {
                // Nothing to do
            }
            else if (args is ProjectEvaluationFinishedEventArgs)
            {
                // Nothing to do
            }
            else if (args is ProjectStartedEventArgs)
            {
                OnProjectStarted(sender, (ProjectStartedEventArgs)args);
            }
            else if (args is ProjectFinishedEventArgs)
            {
                OnProjectFinished(sender, (ProjectFinishedEventArgs)args);
            }
            else
            {
                Console.WriteLine("Unknown event: " + args.GetType().Name);
            }
        }

        private void OnProjectStarted(object sender, ProjectStartedEventArgs args) {
            Console.WriteLine("Project started with instance {0} and context {1}.", args.BuildEventContext.ProjectInstanceId, args.BuildEventContext.ProjectContextId);
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
