Imports Microsoft.Office.Core
Imports Microsoft.Office.Interop
Imports BuddhistScriptConverter
Imports System.Collections.Generic

Public Class MyView
    Public WithEvents DdlScript As Office.CommandBarComboBox
    'Private Property ToolBar As Office.CommandBar
    Private WithEvents Document As Word.Document
    'Private Property CmdBars As Office.CommandBars
    'Private Property AvailableScripts As List(Of String)
    Private OnScriptChanged As Action(Of CommandBarComboBox)

    Public Sub New(ByVal inDocument As Word.Document, ByVal inOnScriptChange As Action(Of CommandBarComboBox), _
                   ByVal inAvailableScripts As List(Of String), ByVal inDefaultScript As String)
        Me.Document = inDocument
        Dim myToolBar As Office.CommandBar = Me.Document.CommandBars.Add("Pali Buddhist Scripts", Office.MsoBarPosition.msoBarTop, False, True)
        myToolBar.Visible = True
        OnScriptChanged = inOnScriptChange
        CreateAndConfigureDdlScript(myToolBar, inAvailableScripts, inDefaultScript)
    End Sub

    Private Sub CreateAndConfigureDdlScript(ByVal toolbar As Office.CommandBar, _
                                            ByVal availableScripts As List(Of String), _
                                            ByVal defaultScript As String)
        Try
            'Instantiate the drop down list and configure it
            Me.DdlScript = toolbar.Controls.Add(Microsoft.Office.Core.MsoControlType.msoControlComboBox)
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
    End Sub

    Private Sub DocumentOpened() Handles Document.Open
        Dim x As Integer = 0
    End Sub

    Private Sub NewDoc() Handles Document.[New]
        Dim x As Integer = 0
    End Sub
End Class
