using CodeGenerator.Models;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System.Text;

namespace Ard.Code.Gen.Services
{
    public static class BusinessFileService
    {
        #region Abstract

        public static void CreateBusinessAbstract(FileParameters fileParameters)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var addedItem = fileParameters.ProjectItem.ProjectItems.AddFromTemplate(fileParameters.ProjectTemplate,
                                                                    $"I{fileParameters.FileNameWithoutExtension}Service.cs");
            var addedItemDocument = addedItem.Document;
            var textDocument = addedItemDocument.Object() as TextDocument;
            var p = textDocument.StartPoint.CreateEditPoint();
            p.Delete(textDocument.EndPoint);
            p.Insert(CreateBusinessAbstractFileContent(fileParameters.FileNameWithoutExtension, fileParameters.ProjectName));
            p.SmartFormat(textDocument.StartPoint);
            addedItemDocument.Save();
        }
        private static string CreateBusinessAbstractFileContent(string fileName, string projectName)
        {
            string camelCasedFileName = char.ToLowerInvariant(fileName[0]) + fileName.Substring(1);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("using System;");
            //     stringBuilder.AppendLine("using Ard.Framework.Utilities.Results;");
            stringBuilder.AppendLine($"using {projectName.Substring(0, projectName.IndexOf('.'))}.Entities.Concrete;");
            stringBuilder.AppendLine($"using {projectName.Substring(0, projectName.IndexOf('.'))}.Entities.Dto;");
            stringBuilder.AppendLine("using System.Collections.Generic;");
            stringBuilder.AppendLine("\n");
            stringBuilder.AppendLine($"namespace {projectName}.Abstract");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine($"public interface I{fileName}Service");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine($"IPagingResult<{fileName}> GetListForPaging({fileName}FilterDto {camelCasedFileName}Dto);");
            stringBuilder.AppendLine($"IDataResult <List<{fileName}>> GetList();");
            stringBuilder.AppendLine($"IDataResult<{fileName}> Get(int id);");
            stringBuilder.AppendLine($"IResult Add({fileName} {camelCasedFileName});");
            stringBuilder.AppendLine($"IResult Update({fileName} {camelCasedFileName});");
            stringBuilder.AppendLine($"IResult Delete(int id);");

            stringBuilder.AppendLine("}");

            stringBuilder.AppendLine("}");

            return stringBuilder.ToString();

        }
        #endregion

        #region Concrete
        public static void CreateBusinessConcrete(FileParameters fileParameters)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var addedItem = fileParameters.ProjectItem.ProjectItems.AddFromTemplate(fileParameters.ProjectTemplate, $"{fileParameters.FileNameWithoutExtension}Manager.cs");
            var addedItemDocument = addedItem.Document;
            var textDocument = addedItemDocument.Object() as TextDocument;
            var p = textDocument.StartPoint.CreateEditPoint();
            p.Delete(textDocument.EndPoint);
            p.Insert(CreateBusinessConcreteFileContent(fileParameters.FileNameWithoutExtension, fileParameters.ProjectName));
            p.SmartFormat(textDocument.StartPoint);
            addedItemDocument.Save();
        }

        private static string CreateBusinessConcreteFileContent(string fileName, string projectName)
        {
            string camelCasedFileName = char.ToLowerInvariant(fileName[0]) + fileName.Substring(1);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("using System;");
            //          stringBuilder.AppendLine("using Ard.Framework.Utilities.Results;");
            stringBuilder.AppendLine("using System.Linq;");
            stringBuilder.AppendLine("using System.Collections.Generic;");
            stringBuilder.AppendLine($"using {projectName.Substring(0, projectName.IndexOf('.'))}.Entities.Concrete;");
            stringBuilder.AppendLine($"using {projectName.Substring(0, projectName.IndexOf('.'))}.DataAccess.Abstract;");
            stringBuilder.AppendLine($"using {projectName.Substring(0, projectName.IndexOf('.'))}.Business.Abstract;");
            stringBuilder.AppendLine($"using {projectName.Substring(0, projectName.IndexOf('.'))}.Entities.Dto;");
            stringBuilder.AppendLine("\n");
            stringBuilder.AppendLine($"namespace {projectName}.Concrete");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine($"public class {fileName}Manager:I{fileName}Service");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine($"private readonly I{fileName}Dal _{camelCasedFileName}Dal;");
            stringBuilder.AppendLine($"public {fileName}Manager(I{fileName}Dal {camelCasedFileName}Dal)");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine($"_{camelCasedFileName}Dal={camelCasedFileName}Dal;");
            stringBuilder.AppendLine("}");

            stringBuilder.AppendLine($"public IPagingResult<{fileName}> GetListForPaging({fileName}FilterDto {camelCasedFileName}Dto)");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine($"return _{camelCasedFileName}Dal.GetListForPaging({camelCasedFileName}Dto.Page, {camelCasedFileName}Dto.PropertyName, {camelCasedFileName}Dto.Asc);");
            stringBuilder.AppendLine("}");

            stringBuilder.AppendLine($"public IDataResult <List<{fileName}>> GetList()");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine($"var result = _{camelCasedFileName}Dal.GetList();");
            stringBuilder.AppendLine($"return new SuccessDataResult<List<{fileName}>>(result);");
            stringBuilder.AppendLine("}");

            stringBuilder.AppendLine($"public  IDataResult<{fileName}> Get(int id)");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine($"var result = _{camelCasedFileName}Dal.Get(x => x.Id == id);");
            stringBuilder.AppendLine($" if (result != null)");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine($" return new SuccessDataResult<{fileName}>(result);");
            stringBuilder.AppendLine("}");
            stringBuilder.AppendLine("else");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine($" return new SuccessDataResult<{fileName}>(\"Not Found\");");
            stringBuilder.AppendLine("}");
            stringBuilder.AppendLine("}");


            stringBuilder.AppendLine($"public IResult Add({fileName} {camelCasedFileName})");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine($"var result = _{camelCasedFileName}Dal.Add({camelCasedFileName});");
            stringBuilder.AppendLine($"if (result)");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine($"return new SuccessResult();");
            stringBuilder.AppendLine("}");
            stringBuilder.AppendLine("else");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine($"return new ErrorResult();");
            stringBuilder.AppendLine("}");
            stringBuilder.AppendLine("}");

            stringBuilder.AppendLine($"public IResult Update({fileName} {camelCasedFileName})");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine($"var result = _{camelCasedFileName}Dal.Update({camelCasedFileName});");
            stringBuilder.AppendLine($"if (result)");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine($"return new SuccessResult();");
            stringBuilder.AppendLine("}");
            stringBuilder.AppendLine("else");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine($"return new ErrorResult();");
            stringBuilder.AppendLine("}");
            stringBuilder.AppendLine("}");



            stringBuilder.AppendLine($"public IResult  Delete(int id)");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine($"var insertedId = _{camelCasedFileName}Dal.Get(x => x.Id == id);");
            stringBuilder.AppendLine($" if (insertedId != null)");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine($"var result = _{camelCasedFileName}Dal.Delete(insertedId);");
            stringBuilder.AppendLine($"if (result)");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine($"return new SuccessResult();");
            stringBuilder.AppendLine("}");
            stringBuilder.AppendLine("else");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine($"return new ErrorResult();");
            stringBuilder.AppendLine("}");
            stringBuilder.AppendLine("}");
            stringBuilder.AppendLine("else");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine($" return new ErrorResult(\"Not Found\");");
            stringBuilder.AppendLine("}");
            stringBuilder.AppendLine("}");

            stringBuilder.AppendLine("}");
            stringBuilder.AppendLine("}");
            return stringBuilder.ToString();
        }
        #endregion
    }
}
