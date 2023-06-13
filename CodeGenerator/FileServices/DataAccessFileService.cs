using CodeGenerator.Models;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System.Text;

namespace CodeGenerator.Utilities
{
    public static class DataAccessFileService
    {
        #region Abstract
        public static void CreateDalAbstract(FileParameters fileParameters)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                var addedItem = fileParameters.ProjectItem.ProjectItems.AddFromTemplate(fileParameters.ProjectTemplate,
                                                                        $"I{fileParameters.FileNameWithoutExtension}Dal.cs");

                var addedItemDocument = addedItem.Document;
                var textDocument = addedItemDocument.Object() as TextDocument;
                var p = textDocument.StartPoint.CreateEditPoint();
                p.Delete(textDocument.EndPoint);
                p.Insert(CreateDalAbstractFileContent(fileParameters.FileNameWithoutExtension, fileParameters.ProjectName));
                p.SmartFormat(textDocument.StartPoint);
                addedItemDocument.Save();
            }
            catch
            {
                throw;
            }
        }

        private static string CreateDalAbstractFileContent(string fileName, string projectName)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("using System;");
            //    stringBuilder.AppendLine("using Ard.Framework.DataAccess;");
            stringBuilder.AppendLine($"using {projectName.Substring(0, projectName.IndexOf('.'))}.Entities.Concrete;");
            stringBuilder.AppendLine($"using {projectName.Substring(0, projectName.IndexOf('.'))}.DataAccess.Abstract;");
            stringBuilder.AppendLine($"using {projectName.Substring(0, projectName.IndexOf('.'))}.Entities.Dto;");
            stringBuilder.AppendLine("\n");
            stringBuilder.AppendLine($"namespace {projectName}.Abstract");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine($"public interface I{fileName}Dal:IEntityRepository<{fileName}>");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine("}");
            stringBuilder.AppendLine("}");

            return stringBuilder.ToString();
        }
        #endregion

        #region Concrete
        public static void CreateDalConcrete(FileParameters fileParameters)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                var addedItem = fileParameters.ProjectItem.ProjectItems.AddFromTemplate(fileParameters.ProjectTemplate, $"Ef{fileParameters.FileNameWithoutExtension}Dal.cs");
                var addedItemDocument = addedItem.Document;
                var textDocument = addedItemDocument.Object() as TextDocument;
                var p = textDocument.StartPoint.CreateEditPoint();
                p.Delete(textDocument.EndPoint);
                p.Insert(CreateDalConcreteFileContent(fileParameters.FileNameWithoutExtension, fileParameters.ProjectName));
                p.SmartFormat(textDocument.StartPoint);
                addedItemDocument.Save();
            }
            catch
            {
                throw;
            }

        }
        private static string CreateDalConcreteFileContent(string fileName, string projectName)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("using System;");
            stringBuilder.AppendLine("using System.Linq;");
            //      stringBuilder.AppendLine("using Ard.Framework.DataAccess.EntityFramework;");
            stringBuilder.AppendLine($"using {projectName.Substring(0, projectName.IndexOf('.'))}.Entities.Concrete;");
            stringBuilder.AppendLine($"using {projectName.Substring(0, projectName.IndexOf('.'))}.DataAccess.Abstract;");
            stringBuilder.AppendLine($"using {projectName.Substring(0, projectName.IndexOf('.'))}.DataAccess.Concrete.EntityFramework.Context;");

            stringBuilder.AppendLine("\n");
            stringBuilder.AppendLine($"namespace {projectName}.Concrete.EntityFramework");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine($"public class Ef{fileName}Dal:EfEntityRepositoryBase<{fileName},ApplicationDbContext>,I{fileName}Dal");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine("}");
            stringBuilder.AppendLine("}");
            return stringBuilder.ToString();
        }
        #endregion
    }
}
