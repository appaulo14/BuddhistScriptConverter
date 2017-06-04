Imports BuddhistScriptConverter
Imports Microsoft.Office.Core
Imports Microsoft.Office.Interop.Word
Imports System.Collections.Generic

Public Class MyController3
    'Public ActiveView As MyView2
    Private WithEvents App As Microsoft.Office.Interop.Word.Application
    'Roman script is the default
    Private PaliTransliterator As New PaliTransliterator(New RomanPaliScript)
    Private AvailableScripts As New List(Of String)
    'Private ViewDictionary As New Dictionary(Of Word.Document, MyView2)
    Private DefaultScript As String
    'Private cmdBar As Office.CommandBar
    Public WithEvents DdlScript As Office.CommandBarComboBox
    Private ToolBar As Office.CommandBar
    Private ViewDictionary As New Dictionary(Of Integer, String)

    Public Sub New(ByVal inApplication As Microsoft.Office.Interop.Word.Application)
        App = inApplication
        For Each workingScript As PaliScript In PaliTransliterator.WorkingScripts
            AvailableScripts.Add(workingScript.Name)
            If (workingScript.Name.ToLower.Contains("roman")) Then
                DefaultScript = workingScript.Name
            End If
        Next
        ToolBar = App.CommandBars.Add("Pali Buddhist Scripts", Office.MsoBarPosition.msoBarTop, False, True)
        ToolBar.Visible = True
        CreateAndConfigureDdlScript()
    End Sub

    Private Sub CreateAndConfigureDdlScript()
        Try
            'Instantiate the drop down list and configure it
            Me.DdlScript = ToolBar.Controls.Add(Microsoft.Office.Core.MsoControlType.msoControlComboBox)
            With DdlScript
                .Style = Microsoft.Office.Core.MsoComboStyle.msoComboLabel
                .Caption = "Script"
                .Tag = "Script"
                .DescriptionText = "Select the script for the text"
                For Each availableScript As String In AvailableScripts
                    .AddItem(availableScript)
                Next
                .Text = DefaultScript
            End With
            'ViewDictionary.Add(App.ActiveDocument.GetHashCode, DefaultScript)
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    'Private Sub x() Handles App.DocumentChange
    '    'Nothing to do if no active document
    '    If (App.Documents.Count = 0) Then
    '        Exit Sub
    '    End If
    '    'Either get an existing document or create a new one
    '    Try
    '        If ViewDictionary.ContainsKey(App.ActiveDocument.GetHashCode) = True Then
    '            Me.DdlScript.Text = ViewDictionary(App.ActiveDocument.GetHashCode)
    '        Else
    '            ViewDictionary.Add(App.ActiveDocument.GetHashCode, DefaultScript)
    '        End If
    '        'Show the new view
    '        'ActiveView.DdlScript.Enabled = True
    '    Catch ex As Exception
    '    End Try
    'End Sub

    Private Sub OnScriptChange(ByVal ctrl As CommandBarComboBox) Handles DdlScript.Change
        'Transliterate
        Dim scriptFound As Boolean = False
        For Each workingScript As PaliScript In PaliTransliterator.WorkingScripts
            If DdlScript.Text = workingScript.Name Then
                scriptFound = True
                PaliTransliterator.DestinationScript = workingScript
                App.Selection.Text = PaliTransliterator.Transliterate(App.Selection.Text)
            End If
        Next
        'Warn the user if the script they entered wasn't found
        If scriptFound = False Then
            MsgBox("The script '" & ctrl.Text & "' is not available on your system. Please try a different script")
        End If
    End Sub
    Private Sub OnWindowSelectionChange(ByVal Sel As Selection) Handles App.WindowSelectionChange
        'If selection is blank Then show roman script
        If Sel.Text.Trim = String.Empty Then
            DdlScript.Text = DefaultScript
            Exit Sub
        End If
        'if selection is one of the scripts, show that script
        For Each workingScipt As PaliScript In PaliTransliterator.WorkingScripts
            If workingScipt.IsScriptUsedOnlyByItselfIn(Sel.Text, PaliTransliterator.WorkingScripts) Then
                DdlScript.Text = workingScipt.Name
                Exit Sub
            End If
        Next
        'if selection is multiple scripts, show blank.
        DdlScript.Text = ""
    End Sub
    Private Sub goat() Handles App.DocumentChange
        OnWindowSelectionChange(App.ActiveWindow.Selection)
    End Sub
End Class
