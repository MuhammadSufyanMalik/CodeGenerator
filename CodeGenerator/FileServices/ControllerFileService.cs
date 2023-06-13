using CodeGenerator.Models;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System.Text;

namespace CodeGenerator.Utilities
{
    public static class ControllerFileService
    {
        #region Controller
        public static void CreateController(FileParameters fileParameters)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var addedItem = fileParameters.ProjectItem.ProjectItems.AddFromTemplate(fileParameters.ProjectTemplate,
                                                                    $"{fileParameters.FileNameWithoutExtension}Controller.cs");

            var addedItemDocument = addedItem.Document;
            var textDocument = addedItemDocument.Object() as TextDocument;
            var p = textDocument.StartPoint.CreateEditPoint();
            p.Delete(textDocument.EndPoint);
            p.Insert(CreateControllerFileContent(fileParameters.FileNameWithoutExtension, fileParameters.ProjectName));
            p.SmartFormat(textDocument.StartPoint);
            addedItemDocument.Save();
        }

        private static string CreateControllerFileContent(string fileName, string projectName)
        {

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("using System;");
            //stringBuilder.AppendLine("using Ard.Framework.Controller;");
            stringBuilder.AppendLine("using Microsoft.AspNetCore.Authentication.JwtBearer;");
            stringBuilder.AppendLine("using Microsoft.AspNetCore.Authorization;");
            stringBuilder.AppendLine("using System.Security.Claims;");
            stringBuilder.AppendLine("using Microsoft.AspNetCore.Mvc;");
            stringBuilder.AppendLine($"using {projectName.Substring(0, projectName.IndexOf('.'))}.Business.Abstract;");
            stringBuilder.AppendLine($"using {projectName.Substring(0, projectName.IndexOf('.'))}.Entities.Concrete;");
            stringBuilder.AppendLine($"using {projectName.Substring(0, projectName.IndexOf('.'))}.Entities.Dto;");
            stringBuilder.AppendLine("\n");
            stringBuilder.AppendLine($"namespace {projectName}.Controllers");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine("[Route(\"api/v{version}/[controller]\")]");
            stringBuilder.AppendLine("[ApiController]");
            stringBuilder.AppendLine("[ApiVersion(\"1\")]");
            stringBuilder.AppendLine("[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]");
            stringBuilder.AppendLine($"public class {fileName}Controller : ControllerBase");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine($"private readonly I{fileName}Service _{fileName.ToLowerInvariant()}Service;");
            stringBuilder.AppendLine("\n");
            stringBuilder.AppendLine($"public {fileName}Controller(I{fileName}Service {fileName.ToLower()}Service)");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine($"_{fileName.ToLower()}Service = {fileName.ToLower()}Service;");
            stringBuilder.AppendLine("}");
            stringBuilder.AppendLine("\n");
            //Method Start

            stringBuilder.AppendLine($"[HttpGet(\"GetListForPaging\")]");

            stringBuilder.AppendLine($"public IActionResult GetListForPaging([FromQuery] {fileName}FilterDto {fileName}Dto)");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine($"int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);");
            stringBuilder.AppendLine($"var result = _{fileName.ToLower()}Service.GetListForPaging({fileName}Dto);");
            stringBuilder.AppendLine($"return Ok(result);");

            stringBuilder.AppendLine("}");
            stringBuilder.AppendLine("\n");

            stringBuilder.AppendLine($"[HttpGet]");

            stringBuilder.AppendLine($"public IActionResult GetList()");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine($"int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);");
            stringBuilder.AppendLine($"var result = _{fileName.ToLower()}Service.GetList();");
            stringBuilder.AppendLine($"return Ok(result);");

            stringBuilder.AppendLine("}");
            stringBuilder.AppendLine("\n");

            stringBuilder.AppendLine($"[HttpGet(\"{{id}}\")]");
            stringBuilder.AppendLine($"public IActionResult Get(int id)");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine($"int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);");
            stringBuilder.AppendLine($"var result = _{fileName.ToLower()}Service.Get(id);");
            stringBuilder.AppendLine($"return Ok(result);");
            stringBuilder.AppendLine("}");
            stringBuilder.AppendLine("\n");

            stringBuilder.AppendLine($"[HttpPost]");
            stringBuilder.AppendLine($"public IActionResult Post([FromBody]{fileName} {fileName.ToLower()})");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine($"int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);");
            stringBuilder.AppendLine($"var result = _{fileName.ToLower()}Service.Add({fileName.ToLower()});");
            stringBuilder.AppendLine($"return Ok(result);");
            stringBuilder.AppendLine("}");
            stringBuilder.AppendLine("\n");

            stringBuilder.AppendLine($"[HttpPut]");
            stringBuilder.AppendLine($"public IActionResult Put([FromBody]{fileName} {fileName.ToLower()})");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine($"int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);");
            stringBuilder.AppendLine($"var result = _{fileName.ToLower()}Service.Update({fileName.ToLower()});");
            stringBuilder.AppendLine($"return Ok(result);");
            stringBuilder.AppendLine("}");
            stringBuilder.AppendLine("\n");

            stringBuilder.AppendLine($"[HttpDelete(\"{{id}}\")]");
            stringBuilder.AppendLine($"public IActionResult Delete(int id)");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine($"int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);");
            stringBuilder.AppendLine($"var result = _{fileName.ToLower()}Service.Delete(id);");
            stringBuilder.AppendLine($"return Ok(result);");
            stringBuilder.AppendLine("}");
            stringBuilder.AppendLine("\n");


            //Method End
            stringBuilder.AppendLine("}");
            stringBuilder.AppendLine("}");

            return stringBuilder.ToString();

        }

        #endregion
    }
}
