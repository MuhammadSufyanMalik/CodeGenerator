using EnvDTE;

namespace CodeGenerator.Models
{
    public class FileParameters
    {
        public ProjectItem ProjectItem { get; set; }
        public string ProjectTemplate { get; set; }
        public string FileNameWithoutExtension { get; set; }
        public string ProjectName { get; set; }
    }
}