using Ard.Code.Gen.Services;
using CodeGenerator.Models;
using CodeGenerator.Utilities;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.IO;
using Task = System.Threading.Tasks.Task;

namespace CodeGenerator
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class GenerateCode
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("e7f97d37-1217-4769-86e2-6bf442c85e25");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateCode"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private GenerateCode(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static GenerateCode Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        private IServiceProvider SyncServiceProvider
        {
            get
            {
                return package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in GenerateCode's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new GenerateCode(package, commandService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        /// 

        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            string message = string.Format("{0}", "Generated Successfully");

            string title = "Controller,Business and DataAccess Layer";

            EnvDTE.DTE dte;
            EnvDTE.SelectedItems selectedItems;
            EnvDTE.ProjectItem projectItem;
            EnvDTE80.Solution2 solution2;

            dte = SyncServiceProvider.GetService(typeof(DTE)) as DTE ?? throw new ArgumentException("My error msg", "e");
            solution2 = (Solution2)dte.Solution;
            selectedItems = dte.SelectedItems;
            if (selectedItems.Count > 1)
            {
                message = "Can't Support multiple creation yet.";
                VsShellUtilities.ShowMessageBox((IServiceProvider)ServiceProvider, message, title, OLEMSGICON.OLEMSGICON_INFO,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }
            if (selectedItems == null)
            {
                message = "There isn't any item selected";
                VsShellUtilities.ShowMessageBox((IServiceProvider)ServiceProvider, message, title, OLEMSGICON.OLEMSGICON_INFO,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                return;
            }

            foreach (EnvDTE.SelectedItem selectedItem in selectedItems)
            {
                projectItem = selectedItem.ProjectItem;
                if (!IsIEntityImplementation(projectItem))
                {
                    message = "Object must be Implemented from IEntity in order to generate";
                    VsShellUtilities.ShowMessageBox((IServiceProvider)ServiceProvider, message, title,
                        OLEMSGICON.OLEMSGICON_INFO, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                    return;
                }

                if (projectItem != null)
                {
                    foreach (EnvDTE.Project project in dte.Solution.Projects)
                    {

                        if (project.Name.EndsWith("DataAccess") || project.Name.EndsWith("Dal"))
                        {
                            if (!DoesProjectFolderExists(project.FullName, "Abstract"))
                            {
                                project.ProjectItems.AddFolder("Abstract");
                            }

                            if (!DoesProjectFolderExists(project.FullName, "Concrete\\EntityFramework"))
                            {
                                project.ProjectItems.AddFolder("Concrete\\EntityFramework");
                            }

                            foreach (EnvDTE.ProjectItem item in project.ProjectItems)
                            {
                                var projectTemplate = solution2.GetProjectItemTemplate("Interface", "CSharp");
                                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(projectItem.Name);
                                var fileParameters = new FileParameters
                                {
                                    ProjectItem = item,
                                    ProjectTemplate = projectTemplate,
                                    ProjectName = project.Name,
                                    FileNameWithoutExtension = fileNameWithoutExtension,

                                };
                                if (item.Name == "Abstract")
                                {
                                    DataAccessFileService.CreateDalAbstract(fileParameters);
                                }
                                if (item.Name == "Concrete")
                                {
                                    foreach (ProjectItem concrete in item.ProjectItems)
                                    {
                                        if (concrete.Name == "EntityFramework")
                                        {
                                            fileParameters.ProjectItem = concrete;
                                            DataAccessFileService.CreateDalConcrete(fileParameters);
                                        }
                                    }
                                }
                            }

                        }

                        if (project.Name.EndsWith("Business") || project.Name.EndsWith("Bll"))
                        {
                            if (!DoesProjectFolderExists(project.FullName, "Abstract"))
                            {
                                project.ProjectItems.AddFolder("Abstract");
                            }

                            if (!DoesProjectFolderExists(project.FullName, "Concrete"))
                            {
                                project.ProjectItems.AddFolder("Concrete");
                            }

                            foreach (EnvDTE.ProjectItem item in project.ProjectItems)
                            {
                                var projectTemplate = solution2.GetProjectItemTemplate("Interface", "CSharp");
                                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(projectItem.Name);
                                var fileParameters = new FileParameters
                                {
                                    FileNameWithoutExtension = fileNameWithoutExtension,
                                    ProjectItem = item,
                                    ProjectName = project.Name,
                                    ProjectTemplate = projectTemplate
                                };
                                if (item.Name == "Abstract")
                                {
                                    BusinessFileService.CreateBusinessAbstract(fileParameters);
                                }
                                if (item.Name == "Concrete")
                                {
                                    BusinessFileService.CreateBusinessConcrete(fileParameters);
                                }
                            }
                        }

                        if (project.Name.EndsWith("Api") || project.Name.EndsWith("api"))
                        {
                            if (!DoesProjectFolderExists(project.FullName, "Controllers"))
                            {
                                project.ProjectItems.AddFolder("Controllers");
                            }

                            foreach (EnvDTE.ProjectItem item in project.ProjectItems)
                            {
                                var projectTemplate = solution2.GetProjectItemTemplate("Interface", "CSharp");
                                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(projectItem.Name);
                                var fileParameters = new FileParameters
                                {
                                    FileNameWithoutExtension = fileNameWithoutExtension,
                                    ProjectItem = item,
                                    ProjectName = project.Name,
                                    ProjectTemplate = projectTemplate
                                };
                                if (item.Name == "Controllers")
                                {
                                    ControllerFileService.CreateController(fileParameters);
                                }
                            }
                        }

                        if (project.Name.EndsWith("Entities") || project.Name.EndsWith("entity"))
                        {
                            if (!DoesProjectFolderExists(project.FullName, "Dto"))
                            {
                                project.ProjectItems.AddFolder("Dto");
                            }

                            foreach (EnvDTE.ProjectItem item in project.ProjectItems)
                            {
                                var projectTemplate = solution2.GetProjectItemTemplate("Interface", "CSharp");
                                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(projectItem.Name);
                                var fileParameters = new FileParameters
                                {
                                    FileNameWithoutExtension = fileNameWithoutExtension,
                                    ProjectItem = item,
                                    ProjectName = project.Name,
                                    ProjectTemplate = projectTemplate
                                };
                                if (item.Name == "Dto")
                                {
                                    FilterDtoServices.CreateDto(fileParameters);
                                }
                            }
                        }
                    }

                }
            }
            // Show a message box to prove we were here
            VsShellUtilities.ShowMessageBox(
            this.package,
            message,
            title,
            OLEMSGICON.OLEMSGICON_INFO,
            OLEMSGBUTTON.OLEMSGBUTTON_OK,
            OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }


        private static bool IsIEntityImplementation(ProjectItem projectItem)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            foreach (CodeElement2 codeElement in projectItem.FileCodeModel.CodeElements)
            {
                if (codeElement is CodeNamespace)
                {
                    var nspace = codeElement as CodeNamespace;

                    foreach (CodeClass property in nspace.Members)
                    {

                        if (property is null)
                        {
                            continue;
                        }

                        foreach (CodeInterface iface in property.ImplementedInterfaces)
                        {
                            if (iface.Name == "IEntity")
                            {
                                return true;
                            }
                        }

                    }
                }
            }
            return false;
        }
        private bool DoesProjectFolderExists(string projectFullName, string folderNameToCheck)
        {
            var projectPath = Path.GetDirectoryName(projectFullName);

            return Directory.Exists(Path.Combine(projectPath, folderNameToCheck));
        }



        /*   private void Execute(object sender, EventArgs e)
           {
               ThreadHelper.ThrowIfNotOnUIThread();
               string message = string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", this.GetType().FullName);
               string title = "GenerateCode";

               // Show a message box to prove we were here
               VsShellUtilities.ShowMessageBox(
                   this.package,
                   message,
                   title,
                   OLEMSGICON.OLEMSGICON_INFO,
                   OLEMSGBUTTON.OLEMSGBUTTON_OK,
                   OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
           }*/


    }
}
