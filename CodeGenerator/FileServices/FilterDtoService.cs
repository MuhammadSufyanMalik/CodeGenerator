using CodeGenerator.Models;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System.Text;

namespace CodeGenerator.Utilities
{
    public static class FilterDtoServices
    {
        public static void CreateDto(FileParameters fileParameters)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var addedItem = fileParameters.ProjectItem.ProjectItems.AddFromTemplate(fileParameters.ProjectTemplate,
                                                                    $"{fileParameters.FileNameWithoutExtension}FilterDto.cs");

            var addedItemDocument = addedItem.Document;
            var textDocument = addedItemDocument.Object() as TextDocument;
            var p = textDocument.StartPoint.CreateEditPoint();
            p.Delete(textDocument.EndPoint);
            p.Insert(CreateDtoFileContent(fileParameters.FileNameWithoutExtension, fileParameters.ProjectName));
            p.SmartFormat(textDocument.StartPoint);
            addedItemDocument.Save();
        }

        private static string CreateDtoFileContent(string fileName, string projectName)
        {
            StringBuilder stringBuilder = new StringBuilder();
            //stringBuilder.AppendLine("using Ard.Framework.Entities;");
            //stringBuilder.AppendLine("\n");
            stringBuilder.AppendLine($"namespace {projectName}.Dto");
            stringBuilder.AppendLine("{");
            //stringBuilder.AppendLine($"public class {fileName}FilterDto : IPagingFilter");
            stringBuilder.AppendLine($"public class {fileName}FilterDto");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine($"public int Page {{ get; set; }}");
            stringBuilder.AppendLine($"public string PropertyName {{ get; set; }} = null!;");
            stringBuilder.AppendLine($"public bool Asc {{ get; set; }}");

            stringBuilder.AppendLine("}");
            stringBuilder.AppendLine("}");

            return stringBuilder.ToString();
        }
    }
}
