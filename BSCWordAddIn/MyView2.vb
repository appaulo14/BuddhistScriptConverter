Imports Microsoft.Office.Core
Imports Microsoft.Office.Interop
Imports BuddhistScriptConverter
Imports System.Collections.Generic

Public Class MyView2
    Public WithEvents DdlScript As Office.CommandBarComboBox
    Private ToolBar As Office.CommandBar
    Public WithEvents Document As Word.Document
    'Private Property CmdBars As Office.CommandBars
    'Private Property AvailableScripts As List(Of String)
    Private OnScriptChanged As Action(Of CommandBarComboBox)
    Public Sub New(ByVal inDocument As Word.Document, ByVal inOnScriptChange As Action(Of CommandBarComboBox), _
                   ByVal inAvailableScripts As List(Of String), ByVal inDefaultScript As String)
        Me.Document = inDocument
        'Create a commandbar if necessary
        For Each commandBar As Office.CommandBar In Document.CommandBars
            If commandBar.Name = "Pali Buddhist Scripts" Then
                ToolBar = commandBar
            Else
                ToolBar = Me.Document.CommandBars.Add("Pali Buddhist Scripts", Office.MsoBarPosition.msoBarTop, False, True)
            End If
        Next
        ToolBar.Visible = True
        OnScriptChanged = inOnScriptChange
        CreateAndConfigureDdlScript(inAvailableScripts, inDefaultScript)
    End Sub

    Private Sub CreateAndConfigureDdlScript(ByVal availableScripts As List(Of String), _
                                            ByVal defaultScript As String)
        Try
            'Instantiate the drop down list and configure it
            Me.DdlScript = ToolBar.Controls.Add(Microsoft.Office.Core.MsoControlType.msoControlComboBox)
            With DdlScript
                .Style = Microsoft.Office.Core.MsoComboStyle.msoComboLabel
                .Caption = "Script"
                .Tag = "Script"
                .DescriptionText = "Select the script for the text"
                For Each availableScript As String In availableScripts
                    .AddItem(availableScript)
                Next
                .Text = defaultScript
            End With
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub ScriptChange(ByVal ctrl As CommandBarComboBox) Handles DdlScript.Change
        OnScriptChanged.Invoke(ctrl)
        DdlScript.Visible = Not DdlScript.Visible
    End Sub

    Public Sub goat()
        DdlScript.Visible = False
    End Sub
End Class
