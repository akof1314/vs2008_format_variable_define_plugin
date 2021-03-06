using System;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using System.Resources;
using System.Reflection;
using System.Globalization;

namespace FormatVariableDefine
{
	/// <summary>The object for implementing an Add-in.</summary>
	/// <seealso class='IDTExtensibility2' />
	public class Connect : IDTExtensibility2, IDTCommandTarget
	{
		/// <summary>Implements the constructor for the Add-in object. Place your initialization code within this method.</summary>
		public Connect()
		{
		}
        //private InternalInfo internalInfo = InternalInfo.GetInternalInfoInstance();

		/// <summary>Implements the OnConnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being loaded.</summary>
		/// <param term='application'>Root object of the host application.</param>
		/// <param term='connectMode'>Describes how the Add-in is being loaded.</param>
		/// <param term='addInInst'>Object representing this Add-in.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
		{
			_applicationObject = (DTE2)application;
			_addInInstance = (AddIn)addInInst;
			if(connectMode == ext_ConnectMode.ext_cm_UISetup)
			{
				object []contextGUIDS = new object[] { };
				Commands2 commands = (Commands2)_applicationObject.Commands;
				string toolsMenuName;

				try
				{
					//If you would like to move the command to a different menu, change the word "Tools" to the 
					//  English version of the menu. This code will take the culture, append on the name of the menu
					//  then add the command to that menu. You can find a list of all the top-level menus in the file
					//  CommandBar.resx.
					string resourceName;
					ResourceManager resourceManager = new ResourceManager("FormatVariableDefine.CommandBar", Assembly.GetExecutingAssembly());
					CultureInfo cultureInfo = new CultureInfo(_applicationObject.LocaleID);
					
					if(cultureInfo.TwoLetterISOLanguageName == "zh")
					{
						System.Globalization.CultureInfo parentCultureInfo = cultureInfo.Parent;
						resourceName = String.Concat(parentCultureInfo.Name, "Tools");
					}
					else
					{
						resourceName = String.Concat(cultureInfo.TwoLetterISOLanguageName, "Tools");
					}
					toolsMenuName = resourceManager.GetString(resourceName);
				}
				catch
				{
					//We tried to find a localized version of the word Tools, but one was not found.
					//  Default to the en-US word, which may work for the current culture.
					toolsMenuName = "Tools";
				}

				//Place the command on the tools menu.
				//Find the MenuBar command bar, which is the top-level command bar holding all the main menu items:
				Microsoft.VisualStudio.CommandBars.CommandBar menuBarCommandBar = ((Microsoft.VisualStudio.CommandBars.CommandBars)_applicationObject.CommandBars)["MenuBar"];

				//Find the Tools command bar on the MenuBar command bar:
				CommandBarControl toolsControl = menuBarCommandBar.Controls[toolsMenuName];
				CommandBarPopup toolsPopup = (CommandBarPopup)toolsControl;

				//This try/catch block can be duplicated if you wish to add multiple commands to be handled by your Add-in,
				//  just make sure you also update the QueryStatus/Exec method to include the new command names.
				try
				{
					//Add a command to the Commands collection:
					Command command = commands.AddNamedCommand2(_addInInstance, "FormatVariableDefine", "Format Variable Define", "Executes the command for FormatVariableDefine", true, 59, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported+(int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);

					//Add a control for the command to the tools menu:
					if((command != null) && (toolsPopup != null))
					{
						command.AddControl(toolsPopup.CommandBar, 1);
					}
				}
				catch(System.ArgumentException)
				{
					//If we are here, then the exception is probably because a command with that name
					//  already exists. If so there is no need to recreate the command and we can 
                    //  safely ignore the exception.
				}

                //下面是添加右键菜单的代码
                CommandBars commandBars = ((CommandBars)_applicationObject.CommandBars);
                CommandBar commandBar = (CommandBar)commandBars["Code Window"];

                //下面的代码在右键菜单中建立一级菜单
                /*
                this.internalInfo.Pop =
                  (CommandBarPopup)(CommandBarControl)commandBar.Controls.Add(
                    MsoControlType.msoControlPopup, 1, null, 1, true );
                this.internalInfo.Pop.Caption = "&FormatVariableDefine"; //菜单项显示内容
                this.internalInfo.Pop.Enabled = true;   //可用
                this.internalInfo.Pop.Visible = true;   //可见
                this.internalInfo.Pop.BeginGroup = true;//添加分隔线
                 */

                try
                {
                    Command cmdFormatVarDef = commands.AddNamedCommand2(
                              _addInInstance,
                              "FormatVariableDefineRightClick",
                              "&FormaT Variable Define",
                              "Format Variable Define",
                              true,
                              0,
                              ref contextGUIDS,
                              (int)vsCommandStatus.vsCommandStatusSupported +
                                (int)vsCommandStatus.vsCommandStatusEnabled,
                              (int)vsCommandStyle.vsCommandStylePictAndText,
                              vsCommandControlType.vsCommandControlTypeButton
                            );
                    //cmdFormatVarDef.AddControl(this.internalInfo.Pop.CommandBar, 1);//加到右键菜单的internalInfo项下，成为二级菜单
                    cmdFormatVarDef.AddControl(commandBar, 1);//直接加在右键菜单里面
                }
                catch (System.ArgumentException)
                {
                }
			}
		}

		/// <summary>Implements the OnDisconnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being unloaded.</summary>
		/// <param term='disconnectMode'>Describes how the Add-in is being unloaded.</param>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
		{
            try
            {
                if (disconnectMode == ext_DisconnectMode.ext_dm_HostShutdown ||
                  disconnectMode == ext_DisconnectMode.ext_dm_UserClosed)
                {
                    Command tmp = null;
                    foreach (Command c in _applicationObject.Commands)
                    {
                        if (c.Name == "FormatVariableDefine.Connect.FormatVariableDefineRightClick")
                        {
                            tmp = c;
                            break;
                        }
                    }
                    if (tmp != null)
                    {
                        tmp.Delete();
                        tmp = null;
                    }

                    //this.internalInfo.Pop.Delete(false);
                }
            }
            catch (Exception /*ex*/)
            {
                //Debug.WriteLine(ex.Message);
            }
		}

		/// <summary>Implements the OnAddInsUpdate method of the IDTExtensibility2 interface. Receives notification when the collection of Add-ins has changed.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />		
		public void OnAddInsUpdate(ref Array custom)
		{
		}

		/// <summary>Implements the OnStartupComplete method of the IDTExtensibility2 interface. Receives notification that the host application has completed loading.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnStartupComplete(ref Array custom)
		{
		}

		/// <summary>Implements the OnBeginShutdown method of the IDTExtensibility2 interface. Receives notification that the host application is being unloaded.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnBeginShutdown(ref Array custom)
		{
		}
		
		/// <summary>Implements the QueryStatus method of the IDTCommandTarget interface. This is called when the command's availability is updated</summary>
		/// <param term='commandName'>The name of the command to determine state for.</param>
		/// <param term='neededText'>Text that is needed for the command.</param>
		/// <param term='status'>The state of the command in the user interface.</param>
		/// <param term='commandText'>Text requested by the neededText parameter.</param>
		/// <seealso class='Exec' />
		public void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText)
		{
			if(neededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
			{
                if (commandName == "FormatVariableDefine.Connect.FormatVariableDefine")
				{
					status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported|vsCommandStatus.vsCommandStatusEnabled;
					return;
				}
                if (commandName == "FormatVariableDefine.Connect.FormatVariableDefineRightClick")
                {
                    status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported|vsCommandStatus.vsCommandStatusEnabled;
                    return;
                }
			}
		}

		/// <summary>Implements the Exec method of the IDTCommandTarget interface. This is called when the command is invoked.</summary>
		/// <param term='commandName'>The name of the command to execute.</param>
		/// <param term='executeOption'>Describes how the command should be run.</param>
		/// <param term='varIn'>Parameters passed from the caller to the command handler.</param>
		/// <param term='varOut'>Parameters passed from the command handler to the caller.</param>
		/// <param term='handled'>Informs the caller if the command was handled or not.</param>
		/// <seealso class='Exec' />
		public void Exec(string commandName, vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled)
		{
			handled = false;
			if(executeOption == vsCommandExecOption.vsCommandExecOptionDoDefault)
			{
				if(commandName == "FormatVariableDefine.Connect.FormatVariableDefine" ||
                   commandName == "FormatVariableDefine.Connect.FormatVariableDefineRightClick")
				{
                    TextSelection select = ((TextSelection)_applicationObject.ActiveDocument.Selection);
                    int nTopLine = select.TopLine;
                    int nBottomLine = select.BottomLine;
                    bool bLastLineEmpty = select.BottomPoint.AtStartOfLine;
                    select.GotoLine(nTopLine, true);
                    select.LineDown(true, nBottomLine - nTopLine);
                    select.EndOfLine(true);

                    if (bLastLineEmpty)
                        select.StartOfLine(vsStartOfLineOptions.vsStartOfLineOptionsFirstColumn, true);
                    string selectedCode = select.Text;
                    string outCode = CodeSmart.AlignText(selectedCode);            //对齐选中文本
                    select.Insert(outCode, (int)vsInsertFlags.vsInsertFlagsCollapseToEnd);
					handled = true;
					return;
				}
			}
		}
		private DTE2 _applicationObject;
		private AddIn _addInInstance;
	}
}
