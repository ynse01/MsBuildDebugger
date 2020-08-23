
namespace MsBuildDebugger
{
    public class Debugger
    {
        private ProjectAnalyzer analyzer;
        private string[] targets;
        private ConsoleUI ui;

        public Debugger(string projectFile, string[] targets)
        {
            analyzer = new ProjectAnalyzer(projectFile);
            this.targets = targets;
        }

        public void SetUI(ConsoleUI ui)
        {
            this.ui = ui;
        }

        public void OnTargetEnter(string name)
        {
            ui.OnTargetEnter(name);
        }

        public void OnTargetLeave(string name)
        {
            ui.OnTargetLeave(name);
        }
    }
}
